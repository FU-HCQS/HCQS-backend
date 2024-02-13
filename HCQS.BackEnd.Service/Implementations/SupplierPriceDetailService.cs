using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.Service.Contracts;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class SupplierPriceDetailService : GenericBackendService, ISupplierPriceDetailService
    {
        public ISupplierPriceDetailRepository _supplierPriceDetailrepository;
        public IMapper _mapper;
        public BackEndLogger _logger;
        public IUnitOfWork _unitOfWork;

        public SupplierPriceDetailService(ISupplierPriceDetailRepository repository, IMapper mapper, BackEndLogger logger, IUnitOfWork unitOfWork, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _supplierPriceDetailrepository = repository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var supplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(null, s => s.Material, s => s.SupplierPriceQuotation.Supplier);
                    if (supplierPriceDetailDb.Any())
                    {
                        if (pageIndex <= 0) pageIndex = 1;
                        if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                        int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                        if (sortInfos != null)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                        }
                        if (pageIndex > 0 && pageSize > 0)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                        }
                        result.Result.Data = supplierPriceDetailDb;
                        result.Result.TotalPage = totalPage;
                    }
                    else
                    {
                        result.Messages.Add("Empty supplier list");
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

        public async Task<AppActionResult> GetLatestQuotationPricesByMaterialId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var supplierPriceQuotationRepository = Resolve<ISupplierPriceQuotationRepository>();
                //find latest supplier quotation
                var supplierQuotation = await supplierPriceQuotationRepository.GetAllDataByExpression(s => s.Id != Guid.Empty, s => s.Supplier);
                var latestQuotationIds = supplierQuotation.GroupBy(q => q.SupplierId)
                                                            .Select(group => group.OrderByDescending(q => q.Date).First().Id)
                                                            .ToList();
                var supplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(s => s.MaterialId == Id && latestQuotationIds.Contains((Guid)s.SupplierPriceQuotationId), s => s.Material, s => s.SupplierPriceQuotation);
                if (supplierPriceDetailDb != null)
                {
                    if (supplierPriceDetailDb.Any())
                    {
                        if (pageIndex <= 0) pageIndex = 1;
                        if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                        int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                        if (sortInfos != null)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                        }
                        if (pageIndex > 0 && pageSize > 0)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                        }
                        result.Result.Data = supplierPriceDetailDb;
                        result.Result.TotalPage = totalPage;
                    }
                    else
                    {
                        result.Messages.Add("Empty sample supplier price detail list");
                    }
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetLatestQuotationPricesByMaterialName(string name, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var materialRepository = Resolve<IMaterialRepository>();
                var materialDb = await materialRepository.GetByExpression(m => m.Name.ToLower().Equals(name.ToLower()));
                if (materialDb == null)
                {
                    result = BuildAppActionResultError(result, $"Material with name:{name} does not exist");
                }
                if (!BuildAppActionResultIsError(result))
                {
                    var supplierPriceQuotationRepository = Resolve<ISupplierPriceQuotationRepository>();
                    //find latest supplier quotation
                    var supplierQuotation = await supplierPriceQuotationRepository.GetAllDataByExpression(s => s.Id != Guid.Empty, s => s.Supplier);
                    var latestQuotationIds = supplierQuotation.GroupBy(q => q.SupplierId)
                                                                .Select(group => group.OrderByDescending(q => q.Date).First().Id)
                                                                .ToList();
                    var supplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(s => s.MaterialId == materialDb.Id && latestQuotationIds.Contains((Guid)(s.SupplierPriceQuotationId)), s => s.SupplierPriceQuotation, s => s.Material);

                    if (supplierPriceDetailDb != null)
                    {
                        if (supplierPriceDetailDb.Any())
                        {
                            if (pageIndex <= 0) pageIndex = 1;
                            if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                            int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                            if (sortInfos != null)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                            }
                            if (pageIndex > 0 && pageSize > 0)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                            }
                            result.Result.Data = supplierPriceDetailDb;
                            result.Result.TotalPage = totalPage;
                        }
                        else
                        {
                            result.Messages.Add("Empty sample supplier price detail list");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetLatestQuotationPricesBySupplierId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var supplierRepository = Resolve<ISupplierRepository>();
                var supplierDb = await supplierRepository.GetById(Id);
                if (supplierDb == null)
                {
                    result = BuildAppActionResultError(result, $"Supplier with Id:{Id} does not exist");
                }

                if (!BuildAppActionResultIsError(result))
                {
                    var supplierPriceQuotation = Resolve<ISupplierPriceQuotationRepository>();
                    var supplierPriceQuotationList = await supplierPriceQuotation.GetAllDataByExpression(s => s.SupplierId == Id, s => s.Supplier);
                    var supplierPriceQuotationIds = supplierPriceQuotationList.Select(s => s.Id).ToList();
                    var allSupplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(s => supplierPriceQuotationIds.Contains((Guid)s.SupplierPriceQuotationId), s => s.SupplierPriceQuotation, s => s.Material);
                    var latestSupplierPriceQuotationOfEachMaterial = allSupplierPriceDetailDb.GroupBy(s => s.MaterialId)
                                                    .ToDictionary(
                                                        group => group.Key,
                                                        group => group.Max(s => s.SupplierPriceQuotation.Date)
                                                    );

                    var supplierPriceDetailDb = allSupplierPriceDetailDb
                                                    .Where(detail => latestSupplierPriceQuotationOfEachMaterial.ContainsKey(detail.MaterialId) &&
                                                                     latestSupplierPriceQuotationOfEachMaterial[detail.MaterialId] == detail.SupplierPriceQuotation.Date)
                                                    .ToList();

                    if (supplierPriceDetailDb != null)
                    {
                        if (supplierPriceDetailDb.Any())
                        {
                            if (pageIndex <= 0) pageIndex = 1;
                            if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                            int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                            if (sortInfos != null)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                            }
                            if (pageIndex > 0 && pageSize > 0)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                            }
                            result.Result.Data = supplierPriceDetailDb;
                            result.Result.TotalPage = totalPage;
                        }
                        else
                        {
                            result.Messages.Add("Empty sample supplier price detail list");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetLatestQuotationPricesBySupplierName(string name, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var supplierRepository = Resolve<ISupplierRepository>();
                var supplierDb = await supplierRepository.GetByExpression(s => s.SupplierName.ToLower().Equals(name.ToLower()));
                if (supplierDb == null)
                {
                    result = BuildAppActionResultError(result, $"Supplier with name:{name} does not exist");
                }

                if (!BuildAppActionResultIsError(result))
                {
                    var supplierPriceQuotation = Resolve<ISupplierPriceQuotationRepository>();
                    var supplierPriceQuotationList = await supplierPriceQuotation.GetAllDataByExpression(s => s.SupplierId == supplierDb.Id, s => s.Supplier);
                    var supplierPriceQuotationIds = supplierPriceQuotationList.Select(s => s.Id).ToList();
                    var allSupplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(s => supplierPriceQuotationIds.Contains((Guid)s.SupplierPriceQuotationId), s => s.Material, s => s.SupplierPriceQuotation);
                    var latestSupplierPriceQuotationOfEachMaterial = allSupplierPriceDetailDb.GroupBy(s => s.MaterialId)
                                                    .ToDictionary(
                                                        group => group.Key,
                                                        group => group.Max(s => s.SupplierPriceQuotation.Date)
                                                    );

                    var supplierPriceDetailDb = allSupplierPriceDetailDb
                                                    .Where(detail => latestSupplierPriceQuotationOfEachMaterial.ContainsKey(detail.MaterialId) &&
                                                                     latestSupplierPriceQuotationOfEachMaterial[detail.MaterialId] == detail.SupplierPriceQuotation.Date)
                                                    .ToList();
                    if (supplierPriceDetailDb != null)
                    {
                        if (supplierPriceDetailDb.Any())
                        {
                            if (pageIndex <= 0) pageIndex = 1;
                            if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                            int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                            if (sortInfos != null)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                            }
                            if (pageIndex > 0 && pageSize > 0)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                            }
                            result.Result.Data = supplierPriceDetailDb;
                            result.Result.TotalPage = totalPage;
                        }
                        else
                        {
                            result.Messages.Add("Empty sample supplier price detail list");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetQuotationPriceById(Guid Id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var supplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(s => s.SupplierPriceQuotationId == Id, s => s.Material, s => s.SupplierPriceQuotation.Supplier);
                if (supplierPriceDetailDb != null)
                {
                    result.Result.Data = supplierPriceDetailDb;
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetQuotationPricesByMaterialId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var supplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(s => s.MaterialId == Id, s => s.Material, s => s.SupplierPriceQuotation.Supplier);
                if (supplierPriceDetailDb != null)
                {
                    if (supplierPriceDetailDb.Any())
                    {
                        if (pageIndex <= 0) pageIndex = 1;
                        if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                        int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                        if (sortInfos != null)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                        }
                        if (pageIndex > 0 && pageSize > 0)
                        {
                            supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                        }
                        result.Result.Data = supplierPriceDetailDb;
                        result.Result.TotalPage = totalPage;
                    }
                    else
                    {
                        result.Messages.Add("Empty sample supplier price detail list");
                    }
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetQuotationPricesByMaterialName(string name, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var materialRepository = Resolve<IMaterialRepository>();
                var materialDb = await materialRepository.GetByExpression(m => m.Name.ToLower().Equals(name.ToLower()));
                if (materialDb == null)
                {
                    result = BuildAppActionResultError(result, $"Material with name:{name} does not exist");
                }
                if (!BuildAppActionResultIsError(result))
                {
                    var supplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(s => s.MaterialId == materialDb.Id, s => s.Material, s => s.SupplierPriceQuotation.Supplier);
                    if (supplierPriceDetailDb != null)
                    {
                        if (supplierPriceDetailDb.Any())
                        {
                            if (pageIndex <= 0) pageIndex = 1;
                            if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                            int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                            if (sortInfos != null)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                            }
                            if (pageIndex > 0 && pageSize > 0)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                            }
                            result.Result.Data = supplierPriceDetailDb;
                            result.Result.TotalPage = totalPage;
                        }
                        else
                        {
                            result.Messages.Add("Empty sample supplier price detail list");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetQuotationPricesBySupplierId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var supplierRepository = Resolve<ISupplierRepository>();
                var supplierDb = await supplierRepository.GetById(Id);
                if (supplierDb == null)
                {
                    result = BuildAppActionResultError(result, $"Supplier with Id:{Id} does not exist");
                }

                if (!BuildAppActionResultIsError(result))
                {
                    var supplierPriceQuotation = Resolve<ISupplierPriceQuotationRepository>();
                    var supplierPriceQuotationList = await supplierPriceQuotation.GetAllDataByExpression(s => s.SupplierId == Id, s => s.Supplier);
                    var supplierPriceQuotationIds = supplierPriceQuotationList.Select(s => s.Id).ToList();
                    var supplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(s => supplierPriceQuotationIds.Contains((Guid)s.SupplierPriceQuotationId), s => s.Material, s => s.SupplierPriceQuotation);
                    if (supplierPriceDetailDb != null)
                    {
                        if (supplierPriceDetailDb.Any())
                        {
                            if (pageIndex <= 0) pageIndex = 1;
                            if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                            int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                            if (sortInfos != null)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                            }
                            if (pageIndex > 0 && pageSize > 0)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                            }
                            result.Result.Data = supplierPriceDetailDb;
                            result.Result.TotalPage = totalPage;
                        }
                        else
                        {
                            result.Messages.Add("Empty sample supplier price detail list");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetQuotationPricesBySupplierName(string name, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var supplierRepository = Resolve<ISupplierRepository>();
                var supplierDb = await supplierRepository.GetByExpression(s => s.SupplierName.ToLower().Equals(name.ToLower()));
                if (supplierDb == null)
                {
                    result = BuildAppActionResultError(result, $"Supplier with name:{name} does not exist");
                }

                if (!BuildAppActionResultIsError(result))
                {
                    var supplierPriceQuotation = Resolve<ISupplierPriceQuotationRepository>();
                    var supplierPriceQuotationList = await supplierPriceQuotation.GetAllDataByExpression(s => s.SupplierId == supplierDb.Id, s => s.Supplier);
                    var supplierPriceQuotationIds = supplierPriceQuotationList.Select(s => s.Id).ToList();
                    var supplierPriceDetailDb = await _supplierPriceDetailrepository.GetAllDataByExpression(s => supplierPriceQuotationIds.Contains((Guid)s.SupplierPriceQuotationId), s => s.SupplierPriceQuotation, s => s.Material);
                    if (supplierPriceDetailDb != null)
                    {
                        if (supplierPriceDetailDb.Any())
                        {
                            if (pageIndex <= 0) pageIndex = 1;
                            if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                            int totalPage = DataPresentationHelper.CalculateTotalPageSize(supplierPriceDetailDb.Count(), pageSize);

                            if (sortInfos != null)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplySorting(supplierPriceDetailDb, sortInfos);
                            }
                            if (pageIndex > 0 && pageSize > 0)
                            {
                                supplierPriceDetailDb = DataPresentationHelper.ApplyPaging(supplierPriceDetailDb, pageIndex, pageSize);
                            }
                            result.Result.Data = supplierPriceDetailDb;
                            result.Result.TotalPage = totalPage;
                        }
                        else
                        {
                            result.Messages.Add("Empty sample supplier price detail list");
                        }
                    }
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