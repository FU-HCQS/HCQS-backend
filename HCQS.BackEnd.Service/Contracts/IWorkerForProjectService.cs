using HCQS.BackEnd.Common.Dto;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IWorkerForProjectService
    {
        Task<AppActionResult> GetListWorkerByQuotationId(Guid id);
    }
}