using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Service.Dto;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IJwtService
    {
        string GenerateRefreshToken();

        Task<string> GenerateAccessToken(LoginRequestDto loginRequest);

        Task<TokenDto> GetNewToken(string refreshToken, string accountId);
    }
}