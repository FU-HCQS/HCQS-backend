using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class MaterialService : GenericBackendService, IMaterialService
    {
        private IMaterialRepository _materialRepository;
        private IMapper _mapper;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;

        public MaterialService(IMaterialRepository materialRepository, IMapper mapper, BackEndLogger logger, IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _materialRepository = materialRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppActionResult> CreateMaterial(MaterialRequest MaterialRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var materialDb = await _materialRepository.GetByExpression(n => n.Name.Equals(MaterialRequest.Name));
                    if (materialDb != null)
                    {
                        result = BuildAppActionResultError(result, $"The material whose name is {MaterialRequest.Name} has already existed!");
                    }
                    else
                    {
                        var material = _mapper.Map<Material>(MaterialRequest);
                        material.Id = Guid.NewGuid();
                        result.Result.Data = await _materialRepository.Insert(material);
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

        public async Task<AppActionResult> DeleteMaterialById(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var materialDb = await _materialRepository.GetById(id);
                    if (materialDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The material with {id} not found !");
                    }
                    else
                    {
                        result.Result.Data = await _materialRepository.DeleteById(id);
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

        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var materialList = await _materialRepository.GetAllDataByExpression(null, null);

                    if (materialList.Any())
                    {
                        if (pageIndex <= 0) pageIndex = 1;
                        if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                        int totalPage = DataPresentationHelper.CalculateTotalPageSize(materialList.Count(), pageSize);

                        if (sortInfos != null)
                        {
                            materialList = DataPresentationHelper.ApplySorting(materialList, sortInfos);
                        }
                        if (pageIndex > 0 && pageSize > 0)
                        {
                            materialList = DataPresentationHelper.ApplyPaging(materialList, pageIndex, pageSize);
                        }
                        result.Result.Data = materialList;
                        result.Result.TotalPage = totalPage;
                    }
                    else
                    {
                        result.Messages.Add("Empty material list");
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

        public async Task<AppActionResult> GetMaterialById(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var materialDb = await _materialRepository.GetById(id);
                if (materialDb != null)
                {
                    result.Result.Data = materialDb;
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetMaterialByName(string name)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var materialDb = await _materialRepository.GetByExpression(m => m.Name.ToLower().Equals(name.ToLower()));
                if (materialDb != null)
                {
                    result.Result.Data = materialDb;
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> UpdateMaterial(MaterialRequest MaterialRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var materialDb = await _materialRepository.GetByExpression(n => n.Id.Equals(MaterialRequest.Id));
                    if (materialDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The material with {MaterialRequest.Id} not found !");
                    }
                    else
                    {
                        var material = _mapper.Map<Material>(MaterialRequest);
                        materialDb.Name = material.Name;
                        materialDb.MaterialType = material.MaterialType;
                        materialDb.UnitMaterial = material.UnitMaterial;
                        materialDb.Quantity = material.Quantity;
                        result.Result.Data = await _materialRepository.Update(materialDb);
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

        public async Task<AppActionResult> UpdateQuantityById(Guid id, int addedQuantity)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var materialDb = await _materialRepository.GetById(id);
                    if (materialDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The material with {id} not found !");
                    }
                    else
                    {
                        materialDb.Quantity += addedQuantity;
                        result.Result.Data = await _materialRepository.Update(materialDb);
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