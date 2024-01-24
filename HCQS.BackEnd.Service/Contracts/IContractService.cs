using HCQS.BackEnd.Common.Dto;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IContractService
    {
        public Task<AppActionResult> SignContract(Guid contractId, Guid accountId, string verificationCode);
    }
}