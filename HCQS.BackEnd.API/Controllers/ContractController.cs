using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.CUSTOMER)]
        public async Task<AppActionResult> SignContract(Guid contractId, string accountId, string verificationCode)
        {
            return await _contractService.SignContract(contractId, accountId, verificationCode);
        }
        [HttpGet("get-contract-by-id/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]
        public async Task<AppActionResult> GetContractById(Guid id)
        {
            return await _contractService.GetContractById(id);
        }
    }
}