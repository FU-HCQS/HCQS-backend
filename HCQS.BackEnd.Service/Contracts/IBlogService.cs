using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IBlogService
    {
        public Task<AppActionResult> GetBlogById(Guid id);
        Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateBlog(BlogRequest blogRequest);
        public Task<AppActionResult> UpdateBlog(BlogRequest blogRequest);
        public Task<AppActionResult> DeleteBlogById(Guid id);
    }
}
