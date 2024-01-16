using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Service.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
