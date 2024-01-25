using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IBlogService
    {
        public Task<AppActionResult> GetBlogById(Guid id);

        public Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateBlog(BlogRequest blogRequest);

        public Task<AppActionResult> UpdateBlog(BlogRequest blogRequest);

        public Task<AppActionResult> DeleteBlogById(Guid id);
    }
}