using HCQS.BackEnd.Common.Dto;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IContractService
    {
        public Task<AppActionResult> SignContract(Guid contractId, string accountId, string verificationCode);

        public Task<AppActionResult> GetContractById(Guid contractId);

        public Task<AppActionResult> ReSendVerificationCode(Guid contractId);
    }
}