using FluentValidation;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("contract-progress-payment")]
    [ApiController]
    public class ContractProgressPaymentController : ControllerBase
    {
        private readonly IValidator<ContractProgressPaymentDto> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private IContractProgressPaymentService _service;

        public ContractProgressPaymentController(IValidator<ContractProgressPaymentDto> validator, IContractProgressPaymentService service, HandleErrorValidator handleErrorValidator)
        {
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _service = service;
        }

        [HttpGet("get-contract-progress-payment-by-contractId/{contractId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> GetContractProgressPaymentByContractId(Guid contractId)
        {
            return await _service.GetContractProgressPaymentByContractId(contractId);
        }

        [HttpPost("create-contract-progress-payment")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> CreateContractProgressPayment(List<ContractProgressPaymentDto> list)
        {
            foreach (var item in list)
            {
                var result = await _validator.ValidateAsync(item);
                if (!result.IsValid)
                {
                    return _handleErrorValidator.HandleError(result);
                }
            }

            return await _service.CreateContractProgressPayment(list);
        }

        [HttpDelete("delete-contract-progress-payment-by-contract-id/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> DeleteContractProgressPaymentByContractId(Guid id)
        {
            return await _service.DeleteContractProgressPaymentByContractId(id);
        }
    }
}