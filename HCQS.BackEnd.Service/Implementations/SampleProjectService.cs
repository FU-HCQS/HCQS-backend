using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Implementations;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
                                StaticFile staticFile = new StaticFile { Id = Guid.NewGuid(), SampleProjectId = project.Id, Url = Convert.ToString(resultFirebase.Result.Data) };
                                await staticFileRepository.Insert(staticFile);
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
                        await _sampleProjectRepository.DeleteById(id);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var fileService = Resolve<IFileService>();
                        var staticFileRepository = Resolve<IStaticFileRepository>();
                        var listStaticFile = await staticFileRepository.GetListByExpression(f => f.SampleProjectId == id);

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
                var sampleList = await _sampleProjectRepository.GetAll();
                var fileService = Resolve<IFileService>();
                var SD = Resolve<HCQS.BackEnd.DAL.Util.SD>();

                var samples = Utility.ConvertIOrderQueryAbleToList(sampleList);

                sampleList = Utility.ConvertListToIOrderedQueryable(samples);

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
                    var projectDb = await _sampleProjectRepository.GetByExpression(b => b.Id  == sampleProjectRequest.Id);
                    if (projectDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The project with id {sampleProjectRequest.Id} is existed!");
                    }


                    await _sampleProjectRepository.Update(project);
                    await _unitOfWork.SaveChangeAsync();

                    if (!BuildAppActionResultIsError(result))
                    {
                        var fileService = Resolve<IFileService>();
                        var staticFileRepository = Resolve<IStaticFileRepository>();
                        var listStaticFile = await staticFileRepository.GetListByExpression(f => f.SampleProjectId == sampleProjectRequest.Id);

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
                                StaticFile staticFile = new StaticFile { Id = Guid.NewGuid(), SampleProjectId = project.Id, Url = Convert.ToString(resultFirebase.Result.Data) };
                                await staticFileRepository.Insert(staticFile);
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
    }
}
