using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface INewsService
    {
        public Task<AppActionResult> GetNewsById(Guid id);

        Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateNews(NewsRequest NewsRequest);

        public Task<AppActionResult> UpdateNews(NewsRequest NewsRequest);

        public Task<AppActionResult> DeleteNewsById(Guid id);
    }
}