using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IQuotationService
    {
        Task<AppActionResult> CreateQuotationDealingRequest(QuotationDealingDto quotationDealingDto);

        Task<AppActionResult> DealQuotation(Guid quotationId, bool status);

        Task<AppActionResult> CreateQuotationDealingByStaff(CreateQuotationDeallingStaffRequest request);

        Task<AppActionResult> PublicQuotationForCustomer(Guid quotationId);

        Task<AppActionResult> GetQuotationById(Guid id);
        Task<AppActionResult> GetListQuotationByStatus(Quotation.Status status);
    }
}