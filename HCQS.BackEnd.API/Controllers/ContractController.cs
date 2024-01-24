using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("contract")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpPut("sign-contract")]
        public async Task<AppActionResult> SignContract(Guid contractId, Guid accountId, string verificationCode)
        {
            return await _contractService.SignContract(contractId, accountId, verificationCode);
        }
    }
}