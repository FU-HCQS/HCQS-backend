using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IQuotationService
    {
        Task<AppActionResult> GetAllQuotationByProjectId(Guid projectId);

        Task<AppActionResult> CreateQuotationDealingRequest(QuotationDealingDto quotationDealingDto);

        Task<AppActionResult> DealQuotation(Guid quotationId, bool status);

        Task<AppActionResult> CreateQuotationDealingByStaff(CreateQuotationDeallingStaffRequest request);
    }
}