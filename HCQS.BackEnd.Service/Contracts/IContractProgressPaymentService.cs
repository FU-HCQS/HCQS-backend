using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IContractProgressPaymentService
    {
        Task<AppActionResult> CreateContractProgressPayment(List<ContractProgressPaymentDto> list);

        Task<AppActionResult> DeleteContractProgressPaymentByContractId(Guid id);

        Task<AppActionResult> GetContractProgressPaymentByContractId(Guid contractId);

        Task SendPaymentRemindEmail();
    }
}