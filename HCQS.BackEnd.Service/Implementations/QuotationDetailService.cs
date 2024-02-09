using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class QuotationDetailService : GenericBackendService, IQuotationDetailService
    {
        private IQuotationDetailRepository _quotationDetailRepository;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public QuotationDetailService(IQuotationDetailRepository projectRepository, BackEndLogger logger, IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _quotationDetailRepository = projectRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppActionResult> CreateListQuotationDetail(List<QuotationDetailDto> quotationDetails)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var materialRepository = Resolve<IMaterialRepository>();
                    var quotationRepository = Resolve<IQuotationRepository>();
                    var quotationDetailRepository = Resolve<IQuotationDetailRepository>();

                    var exportMaterialPriceRepository = Resolve<IExportPriceMaterialRepository>();
                    var quotationsMap = _mapper.Map<List<QuotationDetail>>(quotationDetails);
                    var quotationDb = await quotationRepository.GetByExpression(q => q.Id == quotationDetails.First().QuotationId, q => q.Project);
                    double rawMaterialTotal = 0;
                    double furnitureMaterialTotal = 0;
                    var quotationDetailsDb = await quotationDetailRepository.GetAllDataByExpression(q => q.QuotationId == quotationDb.Id, q => q.Material);
                    
                    if (quotationDetailsDb.Any())
                    {
                        foreach (var quotationDetail in quotationDetailsDb)
                        {
                            if (quotationDetail.Material.MaterialType == Material.Type.RawMaterials)
                            {
                                rawMaterialTotal = rawMaterialTotal + quotationDetail.Total;
                            }
                            else if (quotationDetail.Material.MaterialType == Material.Type.Furniture)
                            {
                                furnitureMaterialTotal = furnitureMaterialTotal + quotationDetail.Total;
                            }
                        }
                    }
                    foreach (var quotationDetail in quotationsMap)
                    {
                        var exportMaterialPrice = await exportMaterialPriceRepository.GetAllDataByExpression(filter: a => a.MaterialId == quotationDetail.MaterialId);
                        exportMaterialPrice.OrderBy(a => a.Date).ThenByDescending(a => a.Date);
                        quotationDetail.Id = Guid.NewGuid();
                        var material = await materialRepository.GetById(quotationDetail.MaterialId);
                     
                        if (material == null)
                        {
                            result = BuildAppActionResultError(result, $"The material with id {quotationDetail.MaterialId} is not existed");
                        }
                        if (quotationDb == null)
                        {
                            result = BuildAppActionResultError(result, $"The quotation with id {quotationDetail.QuotationId} is not existed");
                        }
                        if (!exportMaterialPrice.Any())
                        {
                            result = BuildAppActionResultError(result, $"The export price with material id {quotationDetail.MaterialId} is not existed");
                        }
                        
                        if (!BuildAppActionResultIsError(result))
                        {
                            quotationDetail.Total = exportMaterialPrice.FirstOrDefault().Price * quotationDetail.Quantity;
                            if (material.MaterialType == Material.Type.Furniture)
                            {
                                furnitureMaterialTotal += quotationDetail.Total;
                            }
                            else if (material.MaterialType == Material.Type.RawMaterials)
                            {
                                rawMaterialTotal += quotationDetail.Total;
                            }
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        quotationDb.FurniturePrice = furnitureMaterialTotal;
                        quotationDb.RawMaterialPrice = rawMaterialTotal;
                        result.Result.Data = await _quotationDetailRepository.InsertRange(quotationsMap);
                        await _unitOfWork.SaveChangeAsync();
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> CreateQuotationDetail(QuotationDetailDto quotationDetail)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var quotationMap = _mapper.Map<QuotationDetail>(quotationDetail);
                    quotationMap.Id = Guid.NewGuid();

                    var materialRepository = Resolve<IMaterialRepository>();
                    var quotationRepository = Resolve<IQuotationRepository>();
                    var exportMaterialPriceRepository = Resolve<IExportPriceMaterialRepository>();
                    var material = await materialRepository.GetById(quotationDetail.MaterialId);
                    var quotation = await quotationRepository.GetById(quotationDetail.QuotationId);
                    var exportMaterialPrice = await exportMaterialPriceRepository.GetAllDataByExpression(filter: a => a.MaterialId == quotationMap.MaterialId);
                    exportMaterialPrice.OrderBy(a => a.Date).ThenByDescending(a => a.Date);
                    if (material == null)
                    {
                        result = BuildAppActionResultError(result, $"The material with id {quotationDetail.MaterialId} is not existed");
                    }
                    if (quotation == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {quotationDetail.QuotationId} is not existed");
                    }
                    if (exportMaterialPrice.Any())
                    {
                        result = BuildAppActionResultError(result, $"The export price with material id {quotationDetail.MaterialId} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        quotationMap.Total = exportMaterialPrice.FirstOrDefault().Price * quotationMap.Quantity;
                        result.Result.Data = await _quotationDetailRepository.Insert(quotationMap);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> DeleteQuotationDetail(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var quotationDetailDb = await _quotationDetailRepository.GetById(id);
                if (quotationDetailDb == null)
                {
                    result = BuildAppActionResultError(result, $"The quotation detail with id {id} is not existed");
                }
                if (!BuildAppActionResultIsError(result))
                {
                    result.Result.Data = await _quotationDetailRepository.DeleteById(quotationDetailDb);
                    await _unitOfWork.SaveChangeAsync();
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetAllQuotationDetailByQuotationId(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var quotationDetailDb = await _quotationDetailRepository.GetAllDataByExpression(filter: a => a.QuotationId == id, includes: i => i.Material);
                if (!quotationDetailDb.Any())
                {
                    result = BuildAppActionResultError(result, $"The list quotation detail with quotation id: {id} is not existed");
                }
                if (!BuildAppActionResultIsError(result))
                {
                    result.Result.Data = quotationDetailDb;
                    await _unitOfWork.SaveChangeAsync();
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetQuotationDetailById(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var quotationDetailDb = await _quotationDetailRepository.GetByExpression(filter: i => i.Id == id, includeProperties: i => i.Material);
                if (quotationDetailDb == null)
                {
                    result = BuildAppActionResultError(result, $"The quotation detail with id {id} is not existed");
                }
                if (!BuildAppActionResultIsError(result))
                {
                    result.Result.Data = quotationDetailDb;
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> UpdateQuotationDetail(QuotationDetailDto quotationDetail)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var quotationMap = _mapper.Map<QuotationDetail>(quotationDetail);

                    var materialRepository = Resolve<IMaterialRepository>();
                    var quotationRepository = Resolve<IQuotationRepository>();
                    var exportMaterialPriceRepository = Resolve<IExportPriceMaterialRepository>();

                    var quotationDetailDb = await _quotationDetailRepository.GetById(quotationDetail.Id);
                    var material = await materialRepository.GetById(quotationDetail.MaterialId);
                    var quotation = await quotationRepository.GetById(quotationDetail.QuotationId);
                    var exportMaterialPrice = await exportMaterialPriceRepository.GetAllDataByExpression(filter: a => a.MaterialId == quotationMap.MaterialId);
                    exportMaterialPrice.OrderBy(a => a.Date).ThenByDescending(a => a.Date);
                    if (quotationDetailDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation detail with id {quotationDetail.Id} is not existed");
                    }
                    if (material == null)
                    {
                        result = BuildAppActionResultError(result, $"The material with id {quotationDetail.MaterialId} is not existed");
                    }
                    if (quotation == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {quotationDetail.QuotationId} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        quotationDetailDb = quotationMap;
                        quotationMap.Total = exportMaterialPrice.FirstOrDefault().Price * quotationMap.Quantity;

                        result.Result.Data = await _quotationDetailRepository.Update(quotationDetailDb);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }
    }
}