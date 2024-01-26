using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
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
                    var projectDb = await _projectRepository.GetById(project.Id);
                    if (projectDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The project with id {project.Id} not found ");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        var materialRepository = Resolve<IMaterialRepository>();
                        var quotationRepository = Resolve<IQuotationRepository>();
                        var quotationDetailRepository = Resolve<IQuotationDetailRepository>();
                        var exportPriceMaterialRepository = Resolve<IExportPriceMaterialRepository>();
                        BuildingInputModel buildingInputModel = new BuildingInputModel()
                        {
                            CementRatio = (double)project.CementMixingRatio,
                            SandRatio = (double)project.SandMixingRatio,
                            StoneRatio = (double)project.StoneMixingRatio,
                            WallHeight = (double)project.WallHeight,
                            WallLength = (double)project.WallLength
                        };
                        int birckCount = BuildingUtility.CalculateBrickCount(wallLength: buildingInputModel.WallLength, wallHeight: buildingInputModel.WallHeight);
                        var buildingMaterial = BuildingUtility.CalculateMaterials(buildingInputModel);

                        Quotation quotation = new Quotation { Id = Guid.NewGuid(), ProjectId = (Guid)project.Id, QuotationStatus = Quotation.Status.Pending };
                        List<QuotationDetail> quotationDetailList = new List<QuotationDetail>();
                        var brickDb = await materialRepository.GetByExpression(b => b.Name.ToLower() == "Brick".ToLower());
                        var sandDb = await materialRepository.GetByExpression(b => b.Name.ToLower() == "Sand".ToLower());
                        var stoneDb = await materialRepository.GetByExpression(b => b.Name.ToLower() == "Stone".ToLower());
                        var cementDb = await materialRepository.GetByExpression(b => b.Name.ToLower() == "Cement".ToLower());
                        double total = 0;
                        var brickHistoryExport = await exportPriceMaterialRepository.GetAllDataByExpression(a => a.MaterialId == brickDb.Id);
                        brickHistoryExport.OrderBy(a => a.Date).ThenByDescending(a => a.Date);

                        var sandHistoryExport = await exportPriceMaterialRepository.GetAllDataByExpression(a => a.MaterialId == sandDb.Id);
                        sandHistoryExport.OrderBy(a => a.Date).ThenByDescending(a => a.Date);

                        var cementHistoryExport = await exportPriceMaterialRepository.GetAllDataByExpression(a => a.MaterialId == cementDb.Id);
                        cementHistoryExport.OrderBy(a => a.Date).ThenByDescending(a => a.Date);

                        var stoneHistoryExport = await exportPriceMaterialRepository.GetAllDataByExpression(a => a.MaterialId == stoneDb.Id);
                        stoneHistoryExport.OrderBy(a => a.Date).ThenByDescending(a => a.Date);
                        if (brickDb == null)
                        {
                            result = BuildAppActionResultError(result, "The brick is not existed in the system");
                        }
                        else
                        {
                            var price = brickHistoryExport.First();
                            quotationDetailList.Add(new QuotationDetail { Id = Guid.NewGuid(), Quantity = birckCount, MaterialId = brickDb.Id, QuotationId = quotation.Id, Total = birckCount * price.Price });
                        }
                        if (sandDb == null)
                        {
                            result = BuildAppActionResultError(result, "The sand is not existed in the system");
                        }
                        else
                        {
                            var price = sandHistoryExport.First();
                            total = total + price.Price;
                            quotationDetailList.Add(new QuotationDetail { Id = Guid.NewGuid(), Quantity = (int)buildingMaterial.SandVolume, MaterialId = sandDb.Id, QuotationId = quotation.Id, Total = buildingMaterial.SandVolume * price.Price });
                        }

                        if (stoneDb == null)
                        {
                            result = BuildAppActionResultError(result, "The stone is not existed in the system");
                        }
                        else
                        {
                            var price = stoneHistoryExport.First();
                            total = total + price.Price;

                            quotationDetailList.Add(new QuotationDetail { Id = Guid.NewGuid(), Quantity = (int)buildingMaterial.StoneVolume, MaterialId = stoneDb.Id, QuotationId = quotation.Id, Total = buildingMaterial.StoneVolume * price.Price });
                        }
                        if (cementDb == null)
                        {
                            result = BuildAppActionResultError(result, "The cement is not existed in the system");
                        }
                        else
                        {
                            var price = cementHistoryExport.First();
                            total = total + price.Price;

                            quotationDetailList.Add(new QuotationDetail { Id = Guid.NewGuid(), Quantity = (int)buildingMaterial.CementVolume, MaterialId = cementDb.Id, QuotationId = quotation.Id, Total = buildingMaterial.CementVolume * price.Price });
                        }
                        if (!BuildAppActionResultIsError(result))
                        {
                            await quotationRepository.Insert(quotation);
                            projectDb.ProjectStatus = Project.Status.Processing;
                            projectDb.SandMixingRatio = (int)buildingInputModel.SandRatio;
                            projectDb.CementMixingRatio = (int)buildingInputModel.CementRatio;
                            projectDb.StoneMixingRatio = (int)buildingInputModel.StoneRatio;
                            quotation.QuotationStatus = Quotation.Status.Pending;
                            quotation.RawMaterialPrice = total;
                            await quotationDetailRepository.InsertRange(quotationDetailList);
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

        public async Task<AppActionResult> GetAllProject()
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await _projectRepository.GetAllDataByExpression(filter: null, a => a.Account);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetProjectById(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var quotationDealing = Resolve<IQuotationDealingRepository>();
                result.Result.Data = new
                {
                    Project = await _projectRepository.GetByExpression(filter: a => a.Id == id, a => a.Account, a => a.Contract),
                    DealingQuotation = await quotationDealing.GetAllDataByExpression(filter: a => a.Quotation.ProjectId == id)
                };
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