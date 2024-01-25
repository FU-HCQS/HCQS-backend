using AutoMapper;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class BlogService : GenericBackendService, IBlogService
    {
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IBlogRepository _blogRepository;
        private IMapper _mapper;

        public BlogService(BackEndLogger logger, IMapper mapper, IUnitOfWork unitOfWork, IBlogRepository blogRepository, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _blogRepository = blogRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AppActionResult> CreateBlog(BlogRequest blogRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var blog = _mapper.Map<Blog>(blogRequest);
                    blog.Id = Guid.NewGuid();
                    blog.ImageUrl = string.Empty;
                    var blogDb = await _blogRepository.GetByExpression(b => b.Header.ToLower().Equals(blog.Header));
                    var accountRepository = Resolve<IAccountRepository>();
                    var accountId = await accountRepository.GetByExpression(b => b.Id == blog.AccountId);
                    if (blogDb != null)
                    {
                        result = BuildAppActionResultError(result, $"The blog with header is existed! {blogDb.Header}");
                    }

               result.Result.Data=      await _blogRepository.Insert(blog);
                    await _unitOfWork.SaveChangeAsync();

                    if (!BuildAppActionResultIsError(result))
                    {
                        var fileService = Resolve<IFileService>();
                        string url = $"{SD.FirebasePathName.BLOG_PREFIX}{blog.Id}";
                        var resultFirebase = await fileService.UploadImageToFirebase(blogRequest.ImageUrl, url);

                        if (resultFirebase != null && resultFirebase.IsSuccess)
                        {
                            blog.ImageUrl = Convert.ToString(resultFirebase.Result.Data);
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
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> DeleteBlogById(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var blogDb = await _blogRepository.GetById((id));
                    if (blogDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The blog with {id} not found !");
                    }
                    else
                    {
                        var fileService = Resolve<IFileService>();
                        string url = $"{SD.FirebasePathName.BLOG_PREFIX}{blogDb.Id}";
                        var resultFirebase = await fileService.DeleteImageFromFirebase(url);

                        if (resultFirebase != null && resultFirebase.IsSuccess)
                        {
                            result.Result.Data = await _blogRepository.DeleteById(blogDb.Id);
                            await _unitOfWork.SaveChangeAsync();
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
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

        public async Task<AppActionResult> GetBlogById(Guid id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var blogDb = await _blogRepository.GetById(id);
                if (blogDb != null)
                {
                    result.Result.Data = blogDb;
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> UpdateBlog(BlogRequest blogRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var blogDb = await _blogRepository.GetByExpression(b => b.Id.Equals(blogRequest.Id));
                    if (blogDb == null)
                    {
                        result = BuildAppActionResultError(result, $"The blog with {blogRequest.Id} not found !");
                    }
                    else
                    {
                        var fileService = Resolve<IFileService>();
                        string url = $"{SD.FirebasePathName.BLOG_PREFIX}{blogDb.Id}";
                        var resultFirebase = await fileService.DeleteImageFromFirebase(url);

                        if (resultFirebase != null && resultFirebase.IsSuccess)
                        {
                            var uploadFileResult = await fileService.UploadImageToFirebase(blogRequest.ImageUrl, url);
                            if (uploadFileResult.IsSuccess)
                            {
                                var blog = _mapper.Map<Blog>(blogRequest);
                                blogDb.ImageUrl = Convert.ToString(uploadFileResult.Result.Data);
                                blogDb.Content = blog.Content;
                                blogDb.Header = blog.Header;
                                result.Result.Data = blogDb;
                             await _unitOfWork.SaveChangeAsync();
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

        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var blogList = await _blogRepository.GetAllDataByExpression(null, b=> b.Account);
                var fileService = Resolve<IFileService>();
                var SD = Resolve<HCQS.BackEnd.DAL.Util.SD>();



                if (blogList.Any())
                {
                    if (pageIndex <= 0) pageIndex = 1;
                    if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                    int totalPage = DataPresentationHelper.CalculateTotalPageSize(blogList.Count(), pageSize);

                    if (sortInfos != null)
                    {
                        blogList = DataPresentationHelper.ApplySorting(blogList, sortInfos);
                    }
                    if (pageIndex > 0 && pageSize > 0)
                    {
                        blogList = DataPresentationHelper.ApplyPaging(blogList, pageIndex, pageSize);
                    }
                    result.Result.Data = blogList;
                    result.Result.TotalPage = totalPage;
                }
                else
                {
                    result.Messages.Add("Empty blog list");
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