using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface ISampleProjectService
    {
        public Task<AppActionResult> GetBlogById(Guid id);

        public Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateSampleProject(SampleProjectRequest blogRequest);

        public Task<AppActionResult> UpdateSampleProject(SampleProjectRequest blogRequest);

        public Task<AppActionResult> DeleteSampleProjectById(Guid id);
    }
}