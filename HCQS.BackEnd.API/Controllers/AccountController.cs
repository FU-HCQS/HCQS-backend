using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<Account> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            IAccountService accountService,
            SignInManager<Account> signInManager,
            IConfiguration configuration)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("create-account")]
        public async Task<AppActionResult> CreateAccount(SignUpRequestDto request)
        {
            return await _accountService.CreateAccount(request, false);
        }

        [HttpPost("login")]
        public async Task<AppActionResult> Login(LoginRequestDto request)
        {
            return await _accountService.Login(request);
        }

        [HttpPut("update-account")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]
        public async Task<AppActionResult> UpdateAccount(UpdateAccountRequestDto request)
        {
            return await _accountService.UpdateAccount(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]
        [HttpPost("get-account-by-id/{id}")]
        public async Task<AppActionResult> GetAccountByUserId(string id)
        {
            return await _accountService.GetAccountByUserId(id);
        }

        [HttpPost("get-all-account")]
        public async Task<AppActionResult> GetAllAccount(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _accountService.GetAllAccount(pageIndex, pageSize, sortInfos);
        }

        [HttpPut("change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]
        public async Task<AppActionResult> ChangePassword(ChangePasswordDto dto)
        {
            return await _accountService.ChangePassword(dto);
        }

        [HttpPost("get-accounts-with-searching")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.MANAGEMENT)]
        public async Task<AppActionResult> GetAccountWithSearching(BaseFilterRequest baseFilterRequest)
        {
            return await _accountService.SearchApplyingSortingAndFiltering(baseFilterRequest);
        }

        [HttpPost("get-new-token")]
        public async Task<AppActionResult> GetNewToken([FromBody] string refreshToken, string userId)
        {
            return await _accountService.GetNewToken(refreshToken, userId);
        }

        [HttpPut("forgot-password")]
        public async Task<AppActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            return await _accountService.ForgotPassword(dto);
        }

        [HttpPut("active-account")]
        public async Task<AppActionResult> ActiveAccount(string email, string verifyCode)
        {
            return await _accountService.ActiveAccount(email, verifyCode);
        }

        [HttpPost("send-email-forgot-password/{email}")]
        public async Task<AppActionResult> SendEmailForgotPassword(string email)
        {
            return await _accountService.SendEmailForgotPassword(email);
        }

        [HttpPost("send-email-for-active-account/{email}")]
        public async Task<AppActionResult> SendEmailForActiveCode(string email)
        {
            return await _accountService.SendEmailForActiveCode(email);
        }

        [HttpPost("google-callback")]
        public async Task<AppActionResult> GoogleCallBack(string accessTokenFromGoogle)
        {
            return await _accountService.GoogleCallBack(accessTokenFromGoogle);
        }
    }
}