using HCQS.BackEnd.Common.ConfigurationModel;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class JwtService : GenericBackendService, IJwtService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Account> _userManager;
        private BackEndLogger _logger;
        private JWTConfiguration _jwtConfiguration;

        public JwtService(
            IUnitOfWork unitOfWork,
            UserManager<Account> userManager,
            IServiceProvider serviceProvider,
            BackEndLogger logger
            )
            : base(serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
            _jwtConfiguration = Resolve<JWTConfiguration>();
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateAccessToken(LoginRequestDto loginRequest)
        {
            try
            {
                var accountRepository = Resolve<IAccountRepository>();
                var utility = Resolve<DAL.Util.Utility>();
                var user = await accountRepository.GetByExpression(u => u.Email.ToLower() == loginRequest.Email.ToLower());

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        var claims = new List<Claim>
                    {
                       new Claim (ClaimTypes.Email, loginRequest.Email),
                       new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                       new Claim("AccountId", user.Id)
                    };
                        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                        var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
                        var token = new JwtSecurityToken(
                            issuer: _jwtConfiguration.Issuer,
                            audience: _jwtConfiguration.Audience,
                            expires: utility.GetCurrentDateInTimeZone().AddDays(1),
                            claims: claims,
                            signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                            );
                        return new JwtSecurityTokenHandler().WriteToken(token);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
            return string.Empty;
        }

        public async Task<TokenDto> GetNewToken(string refreshToken, string accountId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string accessTokenNew = "";
                string refreshTokenNew = "";
                try
                {
                    var accountRepository = Resolve<IAccountRepository>();
                    var utility = Resolve<DAL.Util.Utility>();

                    var user = await accountRepository.GetByExpression(u => u.Id.ToLower() == accountId);

                    if (user != null && user.RefreshToken == refreshToken)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var claims = new List<Claim>
                {
                    new Claim (ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("AccountId", user.Id)
                };
                        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                        var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
                        var token = new JwtSecurityToken
                        (
                            issuer: _jwtConfiguration.Issuer,
                            audience: _jwtConfiguration.Audience,
                            expires: utility.GetCurrentDateInTimeZone().AddDays(1),
                            claims: claims,
                            signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                        );

                        accessTokenNew = new JwtSecurityTokenHandler().WriteToken(token);
                        if (user.RefreshTokenExpiryTime <= utility.GetCurrentDateInTimeZone())
                        {
                            user.RefreshToken = GenerateRefreshToken();
                            user.RefreshTokenExpiryTime = utility.GetCurrentDateInTimeZone().AddDays(1);
                            refreshTokenNew = user.RefreshToken;
                        }
                        else
                        {
                            refreshTokenNew = refreshToken;
                        }
                    }
                    await _unitOfWork.SaveChangeAsync();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, this);
                }
                return new TokenDto { Token = accessTokenNew, RefreshToken = refreshTokenNew };
            }
        }
    }
}