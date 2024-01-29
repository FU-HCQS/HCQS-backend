using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IQuotationService
    {
        Task<AppActionResult> CreateQuotationDealingRequest(QuotationDealingDto quotationDealingDto);

        Task<AppActionResult> DealQuotation(Guid quotationId, bool status);

        Task<AppActionResult> CreateQuotationDealingByStaff(CreateQuotationDeallingStaffRequest request);

        Task<AppActionResult> PublicQuotationForCustomer(Guid quotationId);

        Task<AppActionResult> GetQuotationById(Guid id);
    }
}