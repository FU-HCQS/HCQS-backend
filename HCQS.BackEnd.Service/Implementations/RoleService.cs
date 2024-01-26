using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class RoleService : GenericBackendService, IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Account> _userManager;
        private IIdentityRoleRepository _roleRepository;
        private BackEndLogger _logger;

        public RoleService(
            IIdentityRoleRepository identityRoleRepository,
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            UserManager<Account> userManager,
            IServiceProvider serviceProvider,
            BackEndLogger logger) : base(serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
            _roleRepository = identityRoleRepository;
            _logger = logger;
        }

        public async Task<AppActionResult> AssignRoleForUser(string userId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var accountRepository = Resolve<IAccountRepository>();
                    var account = await accountRepository.GetByExpression(u => u.Id == userId && u.IsDeleted == false && u.IsVerified == true);
                    if (account == null)
                    {
                        result = BuildAppActionResultError(result, $"The user with id {userId} not found");
                    }
                    var userRoleRepository = Resolve<IUserRoleRepository>();
                    var userRole = await userRoleRepository.GetAllDataByExpression(s => s.UserId == userId);
                    var staffRole = await _roleRepository.GetByExpression(s => s.Name.ToLower() == Permission.STAFF.ToLower());

                    foreach (var role in userRole)
                    {
                        if (role.RoleId == staffRole.Id)
                        {
                            result = BuildAppActionResultError(result, $"The user with id {userId} has the current role of staff");
                        }
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        var user = await accountRepository.GetById(userId);
                        var addToRole = await _userManager.AddToRoleAsync(user, Permission.STAFF.ToLower());
                        if (addToRole.Succeeded)
                        {
                            result = BuildAppActionResultSuccess(result, $"ASSIGN ROLE SUCCESSFUL");
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, $"ASSIGN ROLE FAILED");
                        }

                        await _unitOfWork.SaveChangeAsync();
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

        public async Task<AppActionResult> GetAllRole()
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await _roleRepository.GetAllDataByExpression(null, null);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> RemoveRoleForUser(string userId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    var accountRepository = Resolve<IAccountRepository>();
                    if (await accountRepository.GetByExpression(u => u.Id == userId && u.IsDeleted == false) == null)
                    {
                        result = BuildAppActionResultError(result, $"The user with id {userId} not found");
                    }
                    var userRoleRepository = Resolve<IUserRoleRepository>();
                    var userRole = await userRoleRepository.GetAllDataByExpression(s => s.UserId == userId);
                    var staffRole = await _roleRepository.GetByExpression(s => s.Name.ToLower() == Permission.STAFF.ToLower());
                    bool flag = false;
                    foreach (var role in userRole)
                    {
                        if (role.RoleId == staffRole.Id)
                        {
                            flag = true;
                        }
                    }

                    if (flag != true)
                    {
                        result = BuildAppActionResultError(result, $"The user with id don't have staff role");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        var user = await accountRepository.GetById(userId);
                        var RemoveFromRoleAsyncResult = await _userManager.RemoveFromRoleAsync(user, Permission.STAFF.ToLower());
                        if (RemoveFromRoleAsyncResult.Succeeded)
                        {
                            result = BuildAppActionResultSuccess(result, $"REMOVE ROLE SUCCESSFUL");
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, $"REMOVE ROLE FAILED");
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
    }
}