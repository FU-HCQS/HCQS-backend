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
using static HCQS.BackEnd.Service.UtilityService.BuildingUtility;

namespace HCQS.BackEnd.Service.Implementations
{
    public class ProjectService : GenericBackendService, IProjectService
    {
        private IProjectRepository _projectRepository;
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, BackEndLogger logger, IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _projectRepository = projectRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppActionResult> CreateProjectByUser(ProjectDto projectDto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var accountRepository = Resolve<IAccountRepository>();
                    var accountDb = await accountRepository.GetById(projectDto.AccountId);
                    var utility = Resolve<Utility>();
                    if (accountDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The user with id {projectDto.AccountId} not found");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var project = _mapper.Map<Project>(projectDto);
                        project.Id = Guid.NewGuid();
                        project.CreateDate = utility.GetCurrentDateTimeInTimeZone();
                        project.ProjectStatus = Project.Status.Pending;
                        await _projectRepository.Insert(project);
                        await _unitOfWork.SaveChangeAsync();

                        var fileRepository = Resolve<IFileService>();
                        var imgUrl = await fileRepository.UploadImageToFirebase(projectDto.LandDrawingFile, $"landdrawing/{project.Id}");
                        if (imgUrl.Result.Data != null && result.IsSuccess)
                        {
                            project.LandDrawingFileUrl = Convert.ToString(imgUrl.Result.Data);
                        }
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

        public async Task<AppActionResult> ConfigProject(ConfigProjectRequest project)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var materialRepository = Resolve<IMaterialRepository>();
                    var quotationRepository = Resolve<IQuotationRepository>();
                    var quotationDetailRepository = Resolve<IQuotationDetailRepository>();
                    var exportPriceMaterialRepository = Resolve<IExportPriceMaterialRepository>();
                    var workerForProjectRepository = Resolve<IWorkerForProjectRepository>();
                    var workerPriceRepository = Resolve<IWorkerPriceRepository>();
                    var projectDb = await _projectRepository.GetById(project.Id);
                    if (projectDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The project with id {project.Id} not found ");
                    }

                    foreach (var laborRequest in project.LaborRequests)
                    {
                        var workerPriceDb = await workerPriceRepository.GetById(laborRequest.WorkerPriceId);
                        if (workerPriceDb == null)
                        {
                            result = BuildAppActionResultError(result, $"The Worker Price with id {laborRequest.WorkerPriceId} not found ");
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        Quotation quotation = new Quotation
                        {
                            Id = Guid.NewGuid(),
                            ProjectId = (Guid)project.Id,
                            QuotationStatus = Quotation.Status.Pending,
                            FurnitureDiscount =project.FurnitureDiscount,
                            LaborDiscount = project.LaborDiscount,
                            RawMaterialDiscount = project.LaborDiscount
                        };
                        double totalLaborPrice = 0;
                        List<WorkerForProject> workers = new List<WorkerForProject>();
                        foreach (var worker in project.LaborRequests)
                        {
                            var workerDb = await workerPriceRepository.GetById(worker.WorkerPriceId);
                            if (workerDb == null)
                            {
                                result = BuildAppActionResultError(result, $"The worker with id  {worker.WorkerPriceId} is not existed");
                            }
                            else
                            {
                                if (worker.ExportLaborCost < workerDb.LaborCost)
                                {
                                    result = BuildAppActionResultError(result, $"The ExportLaborCost with  workerid  must greater than original price");
                                }
                                else
                                {
                                    totalLaborPrice = totalLaborPrice + (worker.Quantity * worker.ExportLaborCost);
                                    workers.Add(new WorkerForProject
                                    {
                                        Id = Guid.NewGuid(),
                                        ExportLaborCost = worker.ExportLaborCost,
                                        WorkerPriceId = worker.WorkerPriceId,
                                        Quantity = worker.Quantity,
                                        QuotationId = quotation.Id
                                    });
                                }
                            }
                        }

                        BuildingInputModel buildingInputModel = new BuildingInputModel()
                        {
                            CementRatio = project.CementMixingRatio,
                            SandRatio = project.SandMixingRatio,
                            StoneRatio = project.StoneMixingRatio,
                            WallHeight = project.WallHeight,
                            WallLength = project.WallLength
                        };
                        int birckCount = BuildingUtility.CalculateBrickCount(wallLength: buildingInputModel.WallLength, wallHeight: buildingInputModel.WallHeight);
                        var buildingMaterial = BuildingUtility.CalculateMaterials(buildingInputModel);

                        List<QuotationDetail> quotationDetailList = new List<QuotationDetail>();
                        var brickDb = await materialRepository.GetByExpression(b => b.Name.ToLower() == "Brick".ToLower());
                        var sandDb = await materialRepository.GetByExpression(b => b.Name.ToLower() == "Sand".ToLower());
                        var stoneDb = await materialRepository.GetByExpression(b => b.Name.ToLower() == "Stone".ToLower());
                        var cementDb = await materialRepository.GetByExpression(b => b.Name.ToLower() == "Cement".ToLower());
                        double total = 0;
                        var brickHistoryExport = brickDb != null ? await exportPriceMaterialRepository.GetAllDataByExpression(a => a.MaterialId == brickDb.Id) : null;
                        brickHistoryExport?.OrderBy(a => a.Date).ThenByDescending(a => a.Date);

                        var sandHistoryExport = sandDb != null ? await exportPriceMaterialRepository.GetAllDataByExpression(a => a.MaterialId == sandDb.Id) : null;
                        sandHistoryExport?.OrderBy(a => a.Date).ThenByDescending(a => a.Date);

                        var cementHistoryExport = cementDb != null ? await exportPriceMaterialRepository.GetAllDataByExpression(a => a.MaterialId == cementDb.Id) : null;
                        cementHistoryExport?.OrderBy(a => a.Date).ThenByDescending(a => a.Date);

                        var stoneHistoryExport = stoneDb != null ? await exportPriceMaterialRepository.GetAllDataByExpression(a => a.MaterialId == stoneDb.Id) : null; ;
                        stoneHistoryExport?.OrderBy(a => a.Date).ThenByDescending(a => a.Date);
                        if (brickDb == null)
                        {
                            result = BuildAppActionResultError(result, "The brick is not existed in the system");
                        }
                        else
                        {
                            var price = brickHistoryExport?.First();
                            quotationDetailList.Add(new QuotationDetail { Id = Guid.NewGuid(), Quantity = birckCount, MaterialId = brickDb.Id, QuotationId = quotation.Id, Total = birckCount * price.Price });
                        }
                        if (sandDb == null)
                        {
                            result = BuildAppActionResultError(result, "The sand is not existed in the system");
                        }
                        else
                        {
                            var price = sandHistoryExport?.First();
                            total = total + (buildingMaterial.SandVolume * price.Price);
                            quotationDetailList.Add(new QuotationDetail { Id = Guid.NewGuid(), Quantity = (int)buildingMaterial.SandVolume, MaterialId = sandDb.Id, QuotationId = quotation.Id, Total = buildingMaterial.SandVolume * price.Price });
                        }

                        if (stoneDb == null)
                        {
                            result = BuildAppActionResultError(result, "The stone is not existed in the system");
                        }
                        else
                        {
                            var price = stoneHistoryExport?.First();
                            total = total + (buildingMaterial.StoneVolume * price.Price);

                            quotationDetailList.Add(new QuotationDetail { Id = Guid.NewGuid(), Quantity = (int)buildingMaterial.StoneVolume, MaterialId = stoneDb.Id, QuotationId = quotation.Id, Total = buildingMaterial.StoneVolume * price.Price });
                        }
                        if (cementDb == null)
                        {
                            result = BuildAppActionResultError(result, "The cement is not existed in the system");
                        }
                        else
                        {
                            var price = cementHistoryExport?.First();
                            total = total + (buildingMaterial.CementVolume * price.Price);

                            quotationDetailList.Add(new QuotationDetail { Id = Guid.NewGuid(), Quantity = (int)buildingMaterial.CementVolume, MaterialId = cementDb.Id, QuotationId = quotation.Id, Total = buildingMaterial.CementVolume * price.Price });
                        }
                        if (!BuildAppActionResultIsError(result))
                        {
                            result.Result.Data = await quotationRepository.Insert(quotation);
                            projectDb.ProjectStatus = Project.Status.Processing;
                            projectDb.SandMixingRatio = (int)buildingInputModel.SandRatio;
                            projectDb.CementMixingRatio = (int)buildingInputModel.CementRatio;
                            projectDb.StoneMixingRatio = (int)buildingInputModel.StoneRatio;
                            quotation.QuotationStatus = Quotation.Status.Pending;
                            quotation.RawMaterialPrice = (total * (100 - quotation.RawMaterialDiscount)) / 100;
                            quotation.RawMaterialDiscount = quotation.RawMaterialDiscount;
                            quotation.LaborPrice = (double)(((totalLaborPrice * (100 - quotation.LaborDiscount)) / 100) * project.EstimatedTimeOfCompletion);
                            quotation.LaborDiscount = quotation.LaborDiscount;
                            quotation.Total = quotation.LaborPrice + quotation.RawMaterialPrice + quotation.FurniturePrice;
                            await quotationDetailRepository.InsertRange(quotationDetailList);
                            await workerForProjectRepository.InsertRange(workers);
                            await _unitOfWork.SaveChangeAsync();
                        }
                        if (!BuildAppActionResultIsError(result))
                        {
                            scope.Complete();
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

        public async Task<AppActionResult> GetAllProjectByAccountId(string accountId)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await GetAllProject(accountId);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetAllProject()
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await GetAllProject(null);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        private async Task<List<Project>> GetAllProject(string accountId)
        {
            return accountId != null ? await _projectRepository.GetAllDataByExpression(filter: a => a.AccountId == accountId, a => a.Account) : await _projectRepository.GetAllDataByExpression(filter: null, a => a.Account);
        }

        public async Task<AppActionResult> GetProjectById(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await GetProjectById(id, false);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        private async Task<ProjectResponse> GetProjectById(Guid id, bool isCustomer)
        {
            var quotationDealingRepository = Resolve<IQuotationDealingRepository>();
            var quotationRepository = Resolve<IQuotationRepository>();
            var workerForProjectRepository = Resolve<IWorkerForProjectRepository>();
            Project project = await _projectRepository.GetByExpression(filter: a => a.Id == id, a => a.Account, a => a.Contract);
            var result = new ProjectResponse
            {
                Project = project,
                QuotationDealings = project != null ? await quotationDealingRepository.GetAllDataByExpression(filter: a => a.Quotation.ProjectId == id) : null,
                Quotations = isCustomer == true ? await quotationRepository.GetAllDataByExpression(filter: a => a.ProjectId == id && a.QuotationStatus != Quotation.Status.Pending) : await quotationRepository.GetAllDataByExpression(filter: a => a.ProjectId == id)
            };

            return result;
        }

        public async Task<AppActionResult> GetProjectByIdForCustomer(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await GetProjectById(id, true);
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