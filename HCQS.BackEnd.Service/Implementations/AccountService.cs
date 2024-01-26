using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using HCQS.BackEnd.Common.ConfigurationModel;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.Dto;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Transactions;
using Utility = HCQS.BackEnd.DAL.Util.Utility;

namespace HCQS.BackEnd.Service.Implementations
{
    public class AccountService : GenericBackendService, IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly TokenDto _tokenDto;
        private IAccountRepository _accountRepository;
        private BackEndLogger _logger;

        public AccountService(
            IAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            UserManager<Account> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<Account> signInManager,
           IServiceProvider serviceProvider,
           BackEndLogger logger

            ) : base(serviceProvider)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenDto = new();
            _logger = logger;
        }

        public async Task<AppActionResult> Login(LoginRequestDto loginRequest)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var user = await _accountRepository.GetByExpression(u => u.Email.ToLower() == loginRequest.Email.ToLower() && u.IsDeleted == false);
                if (user == null)
                {
                    result = BuildAppActionResultError(result, $"The user with username {loginRequest.Email} not found");
                }
                else if (user.IsVerified == false)
                {
                    result = BuildAppActionResultError(result, "The account is not verified !");
                }

                var PasswordSignIn = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, false, false);
                if (!PasswordSignIn.Succeeded)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.LOGIN_FAILED);
                }

                if (!BuildAppActionResultIsError(result))
                {
                    result = await LoginDefault(loginRequest.Email, user);
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }

            return result;
        }

        public async Task<AppActionResult> VerifyLoginGoogle(string email, string verifyCode)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var user = await _accountRepository.GetByExpression(u => u.Email.ToLower() == email.ToLower() && u.IsDeleted == false);
                if (user == null)
                {
                    result = BuildAppActionResultError(result, $"The user with username {email} not found");
                }
                else if (user.IsVerified == false)
                {
                    result = BuildAppActionResultError(result, "The account is not verified !");
                }
                else if (user.VerifyCode != verifyCode)
                {
                    result = BuildAppActionResultError(result, "The  verify code is wrong !");
                }

                if (!BuildAppActionResultIsError(result))
                {
                    result = await LoginDefault(email, user);
                    user.VerifyCode = null;
                    await _unitOfWork.SaveChangeAsync();
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        private async Task<AppActionResult> LoginDefault(string email, Account? user)
        {
            AppActionResult result = new AppActionResult();

            try
            {
                var jwtService = Resolve<IJwtService>();
                var utility = Resolve<Utility>();
                string token = await jwtService.GenerateAccessToken(new LoginRequestDto { Email = email });

                if (user.RefreshToken == null)
                {
                    user.RefreshToken = jwtService.GenerateRefreshToken();
                    user.RefreshTokenExpiryTime = utility.GetCurrentDateInTimeZone().AddDays(1);
                }
                if (user.RefreshTokenExpiryTime <= utility.GetCurrentDateInTimeZone())
                {
                    user.RefreshTokenExpiryTime = utility.GetCurrentDateInTimeZone().AddDays(1);
                    user.RefreshToken = jwtService.GenerateRefreshToken();
                }

                _tokenDto.Token = token;
                _tokenDto.RefreshToken = user.RefreshToken;
                result.Result.Data = _tokenDto;
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> CreateAccount(SignUpRequestDto signUpRequest, bool isGoogle)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var userRoleRepository = Resolve<IUserRoleRepository>();
                    var identityRoleRepository = Resolve<IIdentityRoleRepository>();
                    if (await _accountRepository.GetByExpression(r => r.UserName == signUpRequest.Email) != null)
                    {
                        result = BuildAppActionResultError(result, "The email or username is existed");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        var emailService = Resolve<IEmailService>();
                        string verifyCode = null;
                        if (!isGoogle)
                        {
                            verifyCode = Guid.NewGuid().ToString("N").Substring(0, 6);
                        }

                        var user = new Account
                        {
                            Email = signUpRequest.Email,
                            UserName = signUpRequest.Email,
                            FirstName = signUpRequest.FirstName,
                            LastName = signUpRequest.LastName,
                            PhoneNumber = signUpRequest.PhoneNumber,
                            Gender = signUpRequest.Gender,
                            VerifyCode = verifyCode,
                            IsVerified = isGoogle ? true : false
                        };
                        var resultCreateUser = await _userManager.CreateAsync(user, signUpRequest.Password);
                        if (resultCreateUser.Succeeded)
                        {
                            result.Result.Data = user;
                            result = BuildAppActionResultSuccess(result, $"{SD.ResponseMessage.CREATE_SUCCESSFUL} USER");
                            if (!isGoogle)
                            {
                                emailService.SendEmail(user.Email, SD.SubjectMail.VERIFY_ACCOUNT, verifyCode);
                            }
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, $"{SD.ResponseMessage.CREATE_FAILED} USER");
                        }

                        var resultCreateRole = await _userManager.AddToRoleAsync(user, Permission.CUSTOMER);
                        if (resultCreateRole.Succeeded)
                        {
                            BuildAppActionResultSuccess(result, $"ASSIGN ROLE SUCCESSFUL");
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, $"ASSIGN ROLE FAILED");
                        }
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> UpdateAccount(UpdateAccountRequestDto accountRequest)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var account = await _accountRepository.GetByExpression(a => a.UserName.ToLower() == accountRequest.Email.ToLower());
                    if (account == null)
                    {
                        result = BuildAppActionResultError(result, $"The user with email {account.Email} not found");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        account.FirstName = accountRequest.FirstName;
                        account.LastName = accountRequest.LastName;
                        account.PhoneNumber = accountRequest.PhoneNumber;
                        result.Result.Data = await _accountRepository.Update(account);
                    }
                    await _unitOfWork.SaveChangeAsync();
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                        result = BuildAppActionResultSuccess(result, SD.ResponseMessage.UPDATE_SUCCESSFUL);
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> GetAccountByUserId(string id)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var account = await _accountRepository.GetById(id);
                if (account == null)
                {
                    result = BuildAppActionResultError(result, $"The user with id {id} not found");
                }
                if (!BuildAppActionResultIsError(result))
                {
                    result.Result.Data = account;
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
            }

            return result;
        }

        public async Task<AppActionResult> GetAllAccount(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var userRoleRepository = Resolve<IUserRoleRepository>();
                var identityRoleRepository = Resolve<IIdentityRoleRepository>();
                List<AccountResponse> accounts = new List<AccountResponse>();
                var list = await _accountRepository.GetAllDataByExpression(null, null);
                if (pageIndex <= 0) pageIndex = 1;
                if (pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                int totalPage = DataPresentationHelper.CalculateTotalPageSize(list.Count(), pageSize);

                foreach (var account in list)
                {
                    var userRole = await userRoleRepository.GetAllDataByExpression(s => s.UserId == account.Id, null);
                    var listRole = new List<IdentityRole>();
                    foreach (var role in userRole)
                    {
                        var item = await identityRoleRepository.GetById(role.RoleId);
                        listRole.Add(item);
                    }
                    accounts.Add(new AccountResponse { User = account, Role = listRole });
                }
                var data = accounts.OrderBy(x => x.User.Id).ToList();
                if (sortInfos != null)
                {
                    data = DataPresentationHelper.ApplySorting(data, sortInfos);
                }
                if (pageIndex > 0 && pageSize > 0)
                {
                    data = DataPresentationHelper.ApplyPaging(data, pageIndex, pageSize);
                }
                result.Result.Data = data;
                result.Result.TotalPage = totalPage;
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();

                try
                {
                    if (await _accountRepository.GetByExpression(c => c.Email == changePasswordDto.Email && c.IsDeleted == false) == null)
                    {
                        result = BuildAppActionResultError(result, $"The user with email {changePasswordDto.Email} not found");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var user = await _accountRepository.GetByExpression(c => c.Email == changePasswordDto.Email && c.IsDeleted == false);
                        var ChangePassword = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
                        if (ChangePassword.Succeeded)
                        {
                            result = BuildAppActionResultSuccess(result, SD.ResponseMessage.UPDATE_SUCCESSFUL);
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, SD.ResponseMessage.CREATE_FAILED);
                        }
                    }
                    await _unitOfWork.SaveChangeAsync();
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> SearchApplyingSortingAndFiltering(BaseFilterRequest filterRequest)
        {
            AppActionResult result = new AppActionResult();

            try
            {
                var source = await _accountRepository.GetAllDataByExpression(a => !(bool)a.IsDeleted, null);

                int pageSize = filterRequest.pageSize;
                if (filterRequest.pageSize <= 0) pageSize = SD.MAX_RECORD_PER_PAGE;
                int totalPage = DataPresentationHelper.CalculateTotalPageSize(source.Count(), pageSize);
                if (filterRequest != null)
                {
                    if (filterRequest.pageIndex <= 0 || filterRequest.pageSize <= 0)
                    {
                        result = BuildAppActionResultError(result, $"Invalid value of pageIndex or pageSize");
                    }
                    else
                    {
                        if (filterRequest.keyword != "")
                        {
                            source = await _accountRepository.GetAllDataByExpression(c => (bool)!c.IsDeleted && c.UserName.Contains(filterRequest.keyword));
                        }
                        if (filterRequest.filterInfoList != null)
                        {
                            source = DataPresentationHelper.ApplyFiltering(source, filterRequest.filterInfoList);
                        }
                        totalPage = DataPresentationHelper.CalculateTotalPageSize(source == null ? 0 : source.Count(), filterRequest.pageSize);
                        if (filterRequest.sortInfoList != null)
                        {
                            source = DataPresentationHelper.ApplySorting(source, filterRequest.sortInfoList);
                        }
                        source = DataPresentationHelper.ApplyPaging(source, filterRequest.pageIndex, filterRequest.pageSize);
                        result.Result.Data = source;
                    }
                }
                else
                {
                    result.Result.Data = source;
                }
                result.Result.TotalPage = totalPage;
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> AssignRoleForUserId(string userId, IList<string> roleId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var user = await _accountRepository.GetById(userId);
                    var userRoleRepository = Resolve<IUserRoleRepository>();
                    var identityRoleRepository = Resolve<IIdentityRoleRepository>();
                    if (user == null)
                    {
                        result = BuildAppActionResultError(result, $"The user with id {userId} is not existed");
                    }
                    foreach (var role in roleId)
                    {
                        if (await identityRoleRepository.GetById(role) == null)
                        {
                            result = BuildAppActionResultError(result, $"The role with id {role} is not existed");
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        foreach (var role in roleId)
                        {
                            var roleDB = await identityRoleRepository.GetById(role);
                            var resultCreateRole = await _userManager.AddToRoleAsync(user, roleDB.NormalizedName);
                            if (resultCreateRole.Succeeded)
                            {
                                result = BuildAppActionResultSuccess(result, $"ASSIGN ROLE {role} SUCCESSFUL");
                            }
                            else
                            {
                                result = BuildAppActionResultError(result, $"ASSIGN ROLE {role}  FAILED");
                            }
                        }
                    }
                    await _unitOfWork.SaveChangeAsync();
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> RemoveRoleForUserId(string userId, IList<string> roleId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();

                try
                {
                    var user = await _accountRepository.GetById(userId);
                    var userRoleRepository = Resolve<IUserRoleRepository>();
                    var identityRoleRepository = Resolve<IIdentityRoleRepository>();
                    if (user == null)
                    {
                        result = BuildAppActionResultError(result, $"The user with id {userId} is not existed");
                    }
                    foreach (var role in roleId)
                    {
                        if (await identityRoleRepository.GetById(role) == null)
                        {
                            result = BuildAppActionResultError(result, $"The role with id {role} is not existed");
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        foreach (var role in roleId)
                        {
                            var roleDB = await identityRoleRepository.GetById(role);
                            var resultCreateRole = await _userManager.RemoveFromRoleAsync(user, roleDB.NormalizedName);
                            if (resultCreateRole.Succeeded)
                            {
                                result = BuildAppActionResultSuccess(result, $"REMOVE ROLE {role} SUCCESSFUL");
                            }
                            else
                            {
                                result = BuildAppActionResultError(result, $"REMOVE ROLE {role}  FAILED");
                            }
                        }
                    }

                    await _unitOfWork.SaveChangeAsync();
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> GetNewToken(string refreshToken, string userId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();

                try
                {
                    var user = await _accountRepository.GetById(userId);
                    if (user == null)
                    {
                        result = BuildAppActionResultError(result, "The user is not existed");
                    }
                    else if (user.RefreshToken != refreshToken)
                    {
                        result = BuildAppActionResultError(result, "The refresh token is not exacted");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        var jwtService = Resolve<IJwtService>();
                        result.Result.Data = await jwtService.GetNewToken(refreshToken, userId);
                    }
                    await _unitOfWork.SaveChangeAsync();
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();

                try
                {
                    var user = await _accountRepository.GetByExpression(a => a.Email == dto.Email && a.IsDeleted == false && a.IsVerified == true);
                    if (user == null)
                    {
                        result = BuildAppActionResultError(result, "The user is not existed or is not verified");
                    }
                    else if (user.VerifyCode != dto.RecoveryCode)
                    {
                        result = BuildAppActionResultError(result, "The verification code is wrong.");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        await _userManager.RemovePasswordAsync(user);
                        var AddPassword = await _userManager.AddPasswordAsync(user, dto.NewPassword);
                        if (AddPassword.Succeeded)
                        {
                            user.VerifyCode = null;
                            result = BuildAppActionResultSuccess(result, "Change password successful");
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, "Change password failed");
                        }
                    }
                    await _unitOfWork.SaveChangeAsync();
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> ActiveAccount(string email, string verifyCode)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var user = await _accountRepository.GetByExpression(a => a.Email == email && a.IsDeleted == false && a.IsVerified == false);
                    if (user == null)
                    {
                        result = BuildAppActionResultError(result, "The user is not existed ");
                    }
                    else if (user.VerifyCode != verifyCode)
                    {
                        result = BuildAppActionResultError(result, "The verification code is wrong.");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        user.IsVerified = true;
                        user.VerifyCode = null;
                        result = BuildAppActionResultSuccess(result, "Active successfully");
                    }
                    await _unitOfWork.SaveChangeAsync();
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> SendEmailForgotPassword(string email)
        {
            AppActionResult result = new AppActionResult();

            try
            {
                var user = await _accountRepository.GetByExpression(a => a.Email == email && a.IsDeleted == false && a.IsVerified == true);
                if (user == null)
                {
                    result = BuildAppActionResultError(result, "The user is not existed or is not verified");
                }

                if (!BuildAppActionResultIsError(result))
                {
                    var emailService = Resolve<IEmailService>();
                    string code = await GenerateVerifyCode(user.Email, true);
                    emailService.SendEmail(email, SD.SubjectMail.PASSCODE_FORGOT_PASSWORD, code);
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> SendEmailForActiveCode(string email)
        {
            AppActionResult result = new AppActionResult();

            try
            {
                var user = await _accountRepository.GetByExpression(a => a.Email == email && a.IsDeleted == false && a.IsVerified == false);
                if (user == null)
                {
                    result = BuildAppActionResultError(result, "The user does not existed or is verified");
                }

                if (!BuildAppActionResultIsError(result))
                {
                    var emailService = Resolve<IEmailService>();
                    string code = await GenerateVerifyCode(user.Email, false);
                    emailService.SendEmail(email, SD.SubjectMail.VERIFY_ACCOUNT, code);
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<string> GenerateVerifyCode(string email, bool isForForgettingPassword)
        {
            string code = string.Empty;
            try
            {
                var user = await _accountRepository.GetByExpression(a => a.Email == email && a.IsDeleted == false && a.IsVerified == isForForgettingPassword);

                if (user != null)
                {
                    code = Guid.NewGuid().ToString("N").Substring(0, 6);
                    user.VerifyCode = code;
                }
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
            return code;
        }

        public async Task<string> GenerateVerifyCodeGoogle(string email)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string code = string.Empty;
                try
                {
                    var user = await _accountRepository.GetByExpression(a => a.Email == email && a.IsDeleted == false);

                    if (user != null)
                    {
                        code = Guid.NewGuid().ToString("N").Substring(0, 6);
                        user.VerifyCode = code;
                    }
                    await _unitOfWork.SaveChangeAsync();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, this);
                }
                return code;
            }
        }

        public async Task<AppActionResult> GoogleCallBack(string accessTokenFromGoogle)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var existingFirebaseApp = FirebaseApp.DefaultInstance;
                if (existingFirebaseApp == null)
                {
                    var config = Resolve<FirebaseAdminSDK>();
                    var credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(new
                    {
                        type = config.Type,
                        project_id = config.Project_id,
                        private_key_id = config.Private_key_id,
                        private_key = config.Private_key,
                        client_email = config.Client_email,
                        client_id = config.Client_id,
                        auth_uri = config.Auth_uri,
                        token_uri = config.Token_uri,
                        auth_provider_x509_cert_url = config.Auth_provider_x509_cert_url,
                        client_x509_cert_url = config.Client_x509_cert_url
                    }));
                    var firebaseApp = FirebaseApp.Create(new AppOptions
                    {
                        Credential = credential
                    });
                }

                var verifiedToken = await FirebaseAuth.DefaultInstance
               .VerifyIdTokenAsync(accessTokenFromGoogle);
                var emailClaim = verifiedToken.Claims.FirstOrDefault(c => c.Key == "email");
                var nameClaim = verifiedToken.Claims.FirstOrDefault(c => c.Key == "name");
                var name = nameClaim.Value.ToString();
                var userEmail = emailClaim.Value.ToString();

                if (userEmail != null)
                {
                    var user = await _accountRepository.GetByExpression(a => a.Email == userEmail && a.IsDeleted == false);
                    if (user == null)
                    {
                        var resultCreate = await CreateAccount(new SignUpRequestDto { Email = userEmail, FirstName = name, Gender = true, LastName = string.Empty, Password = "Google123@", PhoneNumber = string.Empty }, true);
                        if (resultCreate != null && resultCreate.IsSuccess)
                        {
                            Account account = (Account)resultCreate.Result.Data;
                            result = await LoginDefault(userEmail, account);
                        }
                    }
                    result = await LoginDefault(userEmail, user);
                }
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }

            return result;
        }
    }
}