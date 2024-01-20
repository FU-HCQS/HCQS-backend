using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.Dto;
using System.Transactions;
using static HCQS.BackEnd.DAL.Util.Utility;

namespace HCQS.BackEnd.Service.Implementations
{
    public class SampleProjectService : GenericBackendService, ISampleProjectService
    {
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private ISampleProjectRepository _sampleProjectRepository;

        public SampleProjectService(BackEndLogger backEndLogger, IUnitOfWork unitOfWork, IMapper mapper, ISampleProjectRepository sampleProjectRepository, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = backEndLogger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sampleProjectRepository = sampleProjectRepository;
        }

        public async Task<AppActionResult> CreateSampleProject(SampleProjectRequest sampleProjectRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var project = _mapper.Map<SampleProject>(sampleProjectRequest);
                    project.Id = Guid.NewGuid();
                    var projectDb = await _sampleProjectRepository.GetByExpression(b => b.Header.ToLower().Equals(project.Header));
                    if (projectDb != null)
                    {
                        result = BuildAppActionResultError(result, $"The project with header {projectDb.Header} is existed!");
                    }
                    var accountRepository = Resolve<IAccountRepository>();

                    var account = await accountRepository.GetByExpression(a => a.Id == sampleProjectRequest.AccountId);
                    if (account == null) {
                        result = BuildAppActionResultError(result, $"The account with id {sampleProjectRequest.AccountId} doesn't existed!");
                    }
                    await _sampleProjectRepository.Insert(project);
                    await _unitOfWork.SaveChangeAsync();

                    if (!BuildAppActionResultIsError(result))
                    {
                        var fileService = Resolve<IFileService>();
                        var staticFileRepository = Resolve<IStaticFileRepository>();

                        foreach (var item in sampleProjectRequest.ImageFiles)
                        {
                            var id = Guid.NewGuid();
                            var resultFirebase = await fileService.UploadImageToFirebase(item, $"{SD.FirebasePathName.SAMPLE_HOUSE_PREFIX}{id}");
                            if (resultFirebase != null && resultFirebase.IsSuccess)
                            {
                                string url = resultFirebase.Result.Data.ToString();
                                var typeFile = FileChecker.CheckFileType(item);
                                var type = StaticFile.Type.Image;
                                if (typeFile == FileChecker.FileType.IsImage)
                                {
                                    type = StaticFile.Type.Image;
                                }
                                else if (typeFile == FileChecker.FileType.IsVideo)
                                {
                                    type = StaticFile.Type.Video;
                                }
                                else
                                {
                                    result = BuildAppActionResultError(result, "The system is only support video and image file");
                                }
                                if (!BuildAppActionResultIsError(result))
                                {
                                    StaticFile staticFile = new StaticFile { Id = Guid.NewGuid(), SampleProjectId = project.Id, Url = url, StaticFileType = type };
                                    result.Result.Data = await staticFileRepository.Insert(staticFile);
                                    await _unitOfWork.SaveChangeAsync();
                                }
                            }
                        }
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> DeleteSampleProjectById(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var projectDb = await _sampleProjectRepository.GetByExpression(b => b.Id == id);
                    if (projectDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The project with id {projectDb.Header} is not existed!");
                    }

                    else
                    {
                        result.Result.Data = await _sampleProjectRepository.DeleteById(id);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var fileService = Resolve<IFileService>();
                        var staticFileRepository = Resolve<IStaticFileRepository>();
                        var listStaticFile = await staticFileRepository.GetAllDataByExpression(f => f.SampleProjectId == id && f.StaticFileType != StaticFile.Type.Pdf);

                        foreach (var item in listStaticFile)
                        {
                            var resultFirebase = await fileService.DeleteImageFromFirebase(item.Url);
                            if (resultFirebase != null && resultFirebase.IsSuccess)
                            {
                                await staticFileRepository.DeleteById(item.Id);
                                await _unitOfWork.SaveChangeAsync();
                            }
                        }
                        if (!BuildAppActionResultIsError(result))
                        {
                            scope.Complete();
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var sampleList = await _sampleProjectRepository.GetAllDataByExpression(null,null);
                List<SampleProjectResponse> sampleProjects = new List<SampleProjectResponse>();
                var staticFileRepository = Resolve<IStaticFileRepository>();

                foreach (var sample in sampleList)
                {
                    sampleProjects.Add(new SampleProjectResponse { SampleProject = sample, StaticFiles = await staticFileRepository.GetAllDataByExpression(S => S.SampleProjectId == sample.Id && S.StaticFileType == StaticFile.Type.Image || S.StaticFileType == StaticFile.Type.Video) });
                }

                var SD = Resolve<HCQS.BackEnd.DAL.Util.SD>();


                if (sampleList.Any())
                {
                    if (pageIndex <= 0) pageIndex = 1;
                    if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                    int totalPage = DataPresentationHelper.CalculateTotalPageSize(sampleList.Count(), pageSize);

                    if (sortInfos != null)
                    {
                        sampleList = DataPresentationHelper.ApplySorting(sampleList, sortInfos);
                    }
                    if (pageIndex > 0 && pageSize > 0)
                    {
                        sampleList = DataPresentationHelper.ApplyPaging(sampleList, pageIndex, pageSize);
                    }
                    result.Result.Data = sampleList;
                    result.Result.TotalPage = totalPage;
                }
                else
                {
                    result.Messages.Add("Empty sample project list");
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetBlogById(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var sampleDb = await _sampleProjectRepository.GetById(id);
                if (sampleDb != null)
                {
                    result.Result.Data = sampleDb;
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> UpdateSampleProject(SampleProjectRequest sampleProjectRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var project = _mapper.Map<SampleProject>(sampleProjectRequest);
                    var projectDb = await _sampleProjectRepository.GetByExpression(b => b.Id == sampleProjectRequest.Id);
                    if (projectDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The project with id {sampleProjectRequest.Id} is existed!");
                    }
                    var accountRepository = Resolve<IAccountRepository>();

                    var account = await accountRepository.GetByExpression(a => a.Id == sampleProjectRequest.AccountId);
                    if (account == null)
                    {
                        result = BuildAppActionResultError(result, $"The account with id {sampleProjectRequest.AccountId} doesn't existed!");
                    }
                    result.Result.Data = await _sampleProjectRepository.Update(project);
                    await _unitOfWork.SaveChangeAsync();

                    if (!BuildAppActionResultIsError(result))
                    {
                        var fileService = Resolve<IFileService>();
                        var staticFileRepository = Resolve<IStaticFileRepository>();
                        var listStaticFile = await staticFileRepository.GetAllDataByExpression(f => f.SampleProjectId == sampleProjectRequest.Id);

                        foreach (var item in listStaticFile)
                        {
                            var resultFirebase = await fileService.DeleteImageFromFirebase(item.Url);
                            if (resultFirebase != null && resultFirebase.IsSuccess)
                            {
                                await staticFileRepository.DeleteById(item.Id);
                                await _unitOfWork.SaveChangeAsync();
                            }
                        }
                        foreach (var item in sampleProjectRequest.ImageFiles)
                        {
                            var id = Guid.NewGuid();
                            var resultFirebase = await fileService.UploadImageToFirebase(item, $"{SD.FirebasePathName.SAMPLE_HOUSE_PREFIX}{id}");
                            if (resultFirebase != null && resultFirebase.IsSuccess)
                            {
                                var typeFile = FileChecker.CheckFileType(item);
                                var type = StaticFile.Type.Image;
                                if (typeFile == FileChecker.FileType.IsImage)
                                {
                                    type = StaticFile.Type.Image;
                                }
                                else if (typeFile == FileChecker.FileType.IsVideo)
                                {
                                    type = StaticFile.Type.Video;
                                }
                                else
                                {
                                    result = BuildAppActionResultError(result, "The system is only support video and image file");
                                }
                                if (!BuildAppActionResultIsError(result))
                                {
                                    StaticFile staticFile = new StaticFile { Id = Guid.NewGuid(), SampleProjectId = project.Id, Url = resultFirebase.Result.Data.ToString(), StaticFileType = type };
                                    await staticFileRepository.Insert(staticFile);
                                    await _unitOfWork.SaveChangeAsync();
                                }
                            }
                        }
                        if (!BuildAppActionResultIsError(result))
                        {
                            scope.Complete();
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }
    }
}