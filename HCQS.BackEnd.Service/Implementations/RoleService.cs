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

        public async Task<AppActionResult> AssignRoleForUser(string userId, string roleName)
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
                    if (await _roleRepository.GetByExpression(r => r.Name.ToLower() == roleName.ToLower()) == null)
                    {
                        result = BuildAppActionResultError(result, $"The role with name {roleName} not found");
                    }

                    if (!BuildAppActionResultIsError(result))
                    {
                        var user = await accountRepository.GetById(userId);
                        var AddToRole = await _userManager.AddToRoleAsync(user, roleName);
                        if (AddToRole.Succeeded)
                        {
                            result = BuildAppActionResultSuccess(result, $"ASSIGN ROLE SUCCESSFUL");
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, $"ASSIGN ROLE FAILED");
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

        public async Task<AppActionResult> CreateRole(string roleName)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    if (await _roleRepository.GetByExpression(r => r.Name.ToLower() == roleName.ToLower()) == null)
                    {
                        result = BuildAppActionResultError(result, $"The role with name {roleName} is existed");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var role = new IdentityRole();
                        role.Name = roleName;
                        var CreateAsyncResult = await _roleManager.CreateAsync(role);

                        if (CreateAsyncResult.Succeeded)
                        {
                            result = BuildAppActionResultSuccess(result, $"{SD.ResponseMessage.CREATE_SUCCESSFUL} ROLE");
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, $"{SD.ResponseMessage.CREATE_FAILED} ROLE");
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

        public async Task<AppActionResult> GetAllRole()
        {
            AppActionResult result = new AppActionResult();
            try
            {
                result.Result.Data = await _roleRepository.GetAllDataByExpression(null,null);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, SD.ResponseMessage.INTERNAL_SERVER_ERROR, true);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> RemoveRoleForUser(string userId, string roleName)
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
                    if (await _roleRepository.GetByExpression(r => r.Name.ToLower() == roleName.ToLower()) == null)
                    {
                        result = BuildAppActionResultError(result, $"The role with name {roleName} not found");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var user = await accountRepository.GetById(userId);
                        var RemoveFromRoleAsyncResult = await _userManager.RemoveFromRoleAsync(user, roleName);
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

        public async Task<AppActionResult> UpdateRole(IdentityRole role)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                try
                {
                    if (await _roleRepository.GetByExpression(r => r.Name.ToLower() == role.Name.ToLower()) == null)
                    {
                        result = BuildAppActionResultError(result, $"The role with name {role.Name} not found");
                    }
                    if (!BuildAppActionResultIsError(result))
                    {
                        var UpdateAsyncResult = await _roleManager.UpdateAsync(role);
                        if (UpdateAsyncResult.Succeeded)
                        {
                            result = BuildAppActionResultSuccess(result, $"{SD.ResponseMessage.UPDATE_SUCCESSFUL} ROLE");
                        }
                        else
                        {
                            result = BuildAppActionResultError(result, $"{SD.ResponseMessage.UPDATE_FAILED} ROLE");
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