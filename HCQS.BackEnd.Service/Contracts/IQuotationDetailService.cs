using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IQuotationDetailService
    {
        Task<AppActionResult> GetQuotationDetailById(Guid id);

        Task<AppActionResult> GetAllQuotationDetailByQuotationId(Guid id);

        Task<AppActionResult> GetAllApprovedQuotationDetailByProjectId(Guid id);

        Task<AppActionResult> CreateQuotationDetail(QuotationDetailDto quotationDetail);

        Task<AppActionResult> CreateListQuotationDetail(List<QuotationDetailDto> quotationDetail);

        Task<AppActionResult> UpdateQuotationDetail(QuotationDetailDto quotationDetail);

        Task<AppActionResult> DeleteQuotationDetail(Guid id);
    }
}