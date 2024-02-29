using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto.Response;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.UtilityService;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class QuotationService : GenericBackendService, IQuotationService
    {
        private IQuotationRepository _quotationRepository;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public QuotationService(IQuotationRepository quotationRepository, BackEndLogger logger, IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _quotationRepository = quotationRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppActionResult> CreateQuotationDealingByStaff(CreateQuotationDeallingStaffRequest request)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var utility = Resolve<Utility>();
                    var quotationDetailRepository = Resolve<IQuotationDetailRepository>();
                    var workerForProjectRepository = Resolve<IWorkerForProjectRepository>();
                    var quotationDb = await _quotationRepository.GetById(request.QuotationId);
                    var workers = await workerForProjectRepository.GetAllDataByExpression(a=> a.QuotationId == request.QuotationId);
                    var quotationDetailsDb = await quotationDetailRepository.GetAllDataByExpression(filter: a => a.QuotationId == request.QuotationId);
                    if (quotationDetailsDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {request.QuotationId} is empty");
                    }

                    if (!quotationDetailsDb.Any() && quotationDetailsDb != null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation details with quotation id {request.QuotationId} is empty");
                    }
                    if (!workers.Any())
                    {
                        result = BuildAppActionResultError(result, $"The list workers with quotation id {request.QuotationId} are empty");

                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        double total = utility.CaculateDiscount(quotationDb.RawMaterialPrice, request.RawMaterialDiscount) +
                                     utility.CaculateDiscount(quotationDb.FurniturePrice, request.FurnitureDiscount) +
                                      utility.CaculateDiscount(quotationDb.LaborPrice, request.LaborDiscount);
                        Quotation quotation = new Quotation
                        {
                            Id = Guid.NewGuid(),
                            FurnitureDiscount = request.FurnitureDiscount,
                            LaborDiscount = request.LaborDiscount,
                            RawMaterialDiscount = request.RawMaterialDiscount,
                            FurniturePrice = utility.CaculateDiscount(quotationDb.FurniturePrice, request.FurnitureDiscount),
                            LaborPrice = utility.CaculateDiscount(quotationDb.LaborPrice, request.LaborDiscount),
                            RawMaterialPrice = utility.CaculateDiscount(quotationDb.RawMaterialPrice, request.RawMaterialDiscount),
                            CreateDate = utility.GetCurrentDateTimeInTimeZone(),
                            Total = total,
                            ProjectId = quotationDb.ProjectId
                        };
                        List<QuotationDetail> quotationDetails = new List<QuotationDetail>();
                        foreach (var item in quotationDetailsDb)
                        {
                            quotationDetails.Add(new QuotationDetail
                            {
                                Id = Guid.NewGuid(),
                                MaterialId = item.MaterialId,
                                Quantity = item.Quantity,
                                QuotationId = quotation.Id,
                                Total = item.Total,
                            });
                        }
                        List<WorkerForProject> workerForProject = new List<WorkerForProject>();
                        foreach(var item in workers)
                        {
                            workerForProject.Add(new WorkerForProject { 
                            Id = Guid.NewGuid(),
                            ExportLaborCost = item.ExportLaborCost,
                            Quantity= item.Quantity,
                            QuotationId= quotation.Id,
                            WorkerPrice = item.WorkerPrice,
                            WorkerPriceId = item.WorkerPriceId,
                            });
                        }
                        await _quotationRepository.Insert(quotation);
                        quotation.QuotationStatus = Quotation.Status.Pending;
                        quotationDb.QuotationStatus = Quotation.Status.Cancel;
                        await quotationDetailRepository.InsertRange(quotationDetails);
                        await workerForProjectRepository.InsertRange(workerForProject);
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

        public async Task<AppActionResult> CreateQuotationDealingRequest(QuotationDealingDto quotationDealingDto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var quotationMap = _mapper.Map<QuotationDealing>(quotationDealingDto);
                    quotationMap.Id = Guid.NewGuid();
                    var quotationDb = await _quotationRepository.GetById(quotationDealingDto.QuotationId);
                    if (quotationDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {quotationDealingDto.Id} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var quotationDealingRepository = Resolve<IQuotationDealingRepository>();
                        result.Result.Data = await quotationDealingRepository.Insert(quotationMap);
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

        public async Task<AppActionResult> DealQuotation(Guid quotationId, bool status)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var contractRepository = Resolve<IContractRepository>();
                    var quotationDetailRepository = Resolve<IQuotationDetailRepository>();
                    var projectRepository = Resolve<IProjectRepository>();
                    var accountRepository = Resolve<IAccountRepository>();
                    var quotationDb = await _quotationRepository.GetById(quotationId);
                    string code = Guid.NewGuid().ToString("N").Substring(0, 6);
                    var project = await projectRepository.GetById(quotationDb.ProjectId);
                    var account = await accountRepository.GetById(project.AccountId);
                    if (quotationDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {quotationId} is not existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var quotationDetails = await quotationDetailRepository.GetAllDataByExpression(filter: a => a.QuotationId == quotationId);

                        if (!quotationDetails.Any())
                        {
                            result = BuildAppActionResultError(result, $"The list quotation details with id {quotationId} is not existed");
                        }
                        var utility = Resolve<Utility>();
                        double total = 0;
                        foreach (var quotationDetail in quotationDetails)
                        {
                            total = total + quotationDetail.Total;
                        }

                        if (status == true)
                        {
                            quotationDb.QuotationStatus = Quotation.Status.Approved;
                            var contract = new Contract
                            {
                                Id = Guid.NewGuid(),
                                ProjectId = quotationDb.ProjectId,
                                DateOfContract = utility.GetCurrentDateTimeInTimeZone(),
                                EndDate = utility.GetCurrentDateInTimeZone().AddYears(5),
                                MaterialPrice = total,
                                Total = utility.CaculateDiscount(quotationDb.RawMaterialPrice, quotationDb.RawMaterialDiscount) + utility.CaculateDiscount(quotationDb.LaborPrice, quotationDb.LaborDiscount) + utility.CaculateDiscount(quotationDb.FurniturePrice, quotationDb.FurnitureDiscount),
                                Content = string.Empty,
                                StartDate = utility.GetCurrentDateTimeInTimeZone(),
                                LaborPrice = utility.CaculateDiscount(quotationDb.LaborPrice, quotationDb.LaborDiscount),
                                FurniturePrice = utility.CaculateDiscount(quotationDb.FurniturePrice, quotationDb.FurnitureDiscount),
                                ContractStatus = Contract.Status.NEW
                            };
                            await contractRepository.Insert(contract);
                          
                        }
                        else
                        {
                            quotationDb.QuotationStatus = Quotation.Status.Cancel;
                        }
                        await _quotationRepository.Update(quotationDb);
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
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

        public async Task<AppActionResult> GetListQuotationByStatus(Quotation.Status status)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                if (status == Quotation.Status.Pending)
                {
                    result.Result.Data = await _quotationRepository.GetAllDataByExpression(q => q.QuotationStatus == Quotation.Status.Pending);
                }
                else if (status == Quotation.Status.WaitingForCustomerResponse)
                {
                    result.Result.Data = await _quotationRepository.GetAllDataByExpression(q => q.QuotationStatus == Quotation.Status.WaitingForCustomerResponse);
                }
                else if (status == Quotation.Status.Approved)
                {
                    result.Result.Data = await _quotationRepository.GetAllDataByExpression(q => q.QuotationStatus == Quotation.Status.Approved);
                }
                else if (status == Quotation.Status.Cancel)
                {
                    result.Result.Data = await _quotationRepository.GetAllDataByExpression(q => q.QuotationStatus == Quotation.Status.Cancel);
                }

            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetQuotationById(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var quotationDealingRepository = Resolve<IQuotationDealingRepository>();
                var quotationDetailsRepository = Resolve<IQuotationDetailRepository>();
                var workerForProjectRepository = Resolve<IWorkerForProjectRepository>();
                var contractRepository = Resolve<IContractRepository>();
                result.Result.Data = new QuotationResponse
                {
                    Quotation = await _quotationRepository.GetById(id),
                    QuotationDealings = await quotationDealingRepository.GetAllDataByExpression(q => q.QuotationId == id),
                    QuotationDetails = await quotationDetailsRepository.GetAllDataByExpression(q => q.QuotationId == id, q => q.Material),
                    WorkerForProjects = await workerForProjectRepository.GetAllDataByExpression(q => q.QuotationId == id, q => q.WorkerPrice),
                };
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> PublicQuotationForCustomer(Guid quotationId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                var quotationDetailRepository = Resolve<IQuotationDetailRepository>();
                var utility = Resolve<Utility>();
                try
                {
                    var quotationDb = await _quotationRepository.GetByExpression(q => q.Id == quotationId, q => q.Project);
                    var quotationDetails = await quotationDetailRepository.GetAllDataByExpression(filter: a => a.QuotationId == quotationId, includes: a => a.Material);
                    bool isValid = false;
                    if (quotationDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {quotationId} is not existed");
                    }
                    else if (quotationDb.Project.ConstructionType == Project.ProjectConstructionType.CompleteConstruction)
                    {
                        foreach (var item in quotationDetails)
                        {
                            if (item.Material.MaterialType == Material.Type.Furniture)
                            {
                                isValid = true;
                            }
                        }
                        if (!isValid)
                        {
                            result = BuildAppActionResultError(result, $"This is a completed project, please add interior materials");
                        }
                    }

                    if (quotationDetails == null)
                    {
                        result = BuildAppActionResultError(result, $"The quotation details is empty");
                    }

                    if (quotationDb.QuotationStatus != Quotation.Status.Pending)
                    {
                        result = BuildAppActionResultError(result, $"The quotation with id {quotationId} has been made public");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        if (quotationDb.QuotationStatus == Quotation.Status.Pending)
                        {
                            quotationDb.QuotationStatus = Quotation.Status.WaitingForCustomerResponse;
                            var price = await GetTotalPriceByQuotationId(quotationId);
                            quotationDb.RawMaterialPrice = utility.CaculateDiscount(price.RawPrice, quotationDb.RawMaterialDiscount);
                            quotationDb.FurniturePrice = utility.CaculateDiscount(price.FurniturePrice, quotationDb.FurnitureDiscount);
                            quotationDb.LaborPrice = utility.CaculateDiscount(price.LaborPrice, quotationDb.LaborDiscount);
                            quotationDb.Total = quotationDb.RawMaterialPrice + quotationDb.FurniturePrice + quotationDb.LaborPrice;
                            await _quotationRepository.Update(quotationDb);
                            await _unitOfWork.SaveChangeAsync();
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, $"The api is support for quotation is not public");
                        }
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

        private async Task<QuotationPriceDto> GetTotalPriceByQuotationId(Guid id)
        {
            QuotationPriceDto dto = new QuotationPriceDto();
            var quotationDb = await _quotationRepository.GetById(id);
            var quotationDetailRepository = Resolve<IQuotationDetailRepository>();
            var workerForProjectRepository = Resolve<IWorkerForProjectRepository>();
            var projectRepository = Resolve<IProjectRepository>();
            var project = await projectRepository.GetByExpression(a => a.Id == quotationDb.ProjectId);

            if (quotationDb != null && project != null)
            {
                var quotationDetails = await quotationDetailRepository.GetAllDataByExpression(q => q.QuotationId == id, q => q.Material);
                var workerForProjects = await workerForProjectRepository.GetAllDataByExpression(q => q.QuotationId == id);
                double rawMaterialPrice = 0;
                double furniturePrice = 0;
                double laborPrice = 0;
                foreach (var quotationDetail in quotationDetails)
                {
                    if (quotationDetail.Material.MaterialType == Material.Type.RawMaterials)
                    {
                        rawMaterialPrice = rawMaterialPrice + quotationDetail.Total;
                    }
                    else if (quotationDetail.Material.MaterialType == Material.Type.Furniture)
                    {
                        furniturePrice = furniturePrice + quotationDetail.Total;
                    }
                }

                foreach (var workerForProject in workerForProjects)
                {
                    laborPrice = laborPrice + (workerForProject.ExportLaborCost * workerForProject.Quantity * project.EstimatedTimeOfCompletion);
                }

                dto.FurniturePrice = furniturePrice;
                dto.RawPrice = rawMaterialPrice;
                dto.LaborPrice = laborPrice;
            }
            return dto;
        }
    }
}