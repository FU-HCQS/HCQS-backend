using HCQS.BackEnd.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IWorkerForProjectService
    {
        Task<AppActionResult> GetListWorkerByQuotationId(Guid id);

    }
}
