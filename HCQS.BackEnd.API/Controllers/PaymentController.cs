using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.UtilityService.Payment.PaymentRespone;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("create-payment-url-vnpay")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]

        public async Task<AppActionResult> CreatePaymentUrlVNPay(Guid paymentId)
        {
            return await _paymentService.CreatePaymentUrlVNPay(paymentId, HttpContext);
        }

        [HttpGet("create-payment-url-momo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]

        public async Task<AppActionResult> CreatePaymentUrlMomo(Guid paymentId)
        {
            return await _paymentService.CreatePaymentUrlMomo(paymentId);
        }
        [HttpGet("get-all-payment")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]

        public async Task<AppActionResult> GetAllPayment(int pageIndex=1, int pageSize=10)
        {
            return await _paymentService.GetAllPayment(pageIndex, pageSize);
        }
        [HttpGet("get-all-payment-by-contractId/{contractId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]

        public async Task<AppActionResult> GetAllPayment(Guid contractId)
        {
            return await _paymentService.GetAllPaymentByContractId(contractId);
        }
        [HttpGet("VNPayIpn")]
        public async Task<IActionResult> VNPayIPN()
        {
            try
            {
                var response = new VNPayResponseDto
                {
                    PaymentMethod = Request.Query["vnp_BankCode"],
                    OrderDescription = Request.Query["vnp_OrderInfo"],
                    OrderId = Request.Query["vnp_TxnRef"],
                    PaymentId = Request.Query["vnp_TransactionNo"],
                    TransactionId = Request.Query["vnp_TransactionNo"],
                    Token = Request.Query["vnp_SecureHash"],
                    VnPayResponseCode = Request.Query["vnp_ResponseCode"],
                    PayDate = Request.Query["vnp_PayDate"],
                    Amount = Request.Query["vnp_Amount"],
                    Success = true
                };

                if (response.VnPayResponseCode == "00")
                {
                    await _paymentService.UpdatePaymentStatus(response.OrderId, true, 0);
                }
                else
                {
                    await _paymentService.UpdatePaymentStatus(response.OrderId, false, 0);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("MomoIpn")]
        public async Task<IActionResult> MomoIPN(MomoResponseDto momo)
        {
            try
            {
                if (momo.resultCode == 0)
                {
                    await _paymentService.UpdatePaymentStatus(momo.extraData, true, 1);
                }
                else
                {
                    await _paymentService.UpdatePaymentStatus(momo.extraData, false, 1);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}