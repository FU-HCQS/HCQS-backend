using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto;
using MathNet.Numerics.RootFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
