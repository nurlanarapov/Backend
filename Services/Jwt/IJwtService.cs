using System.Collections.Generic;
using System.Security.Claims;

namespace BackEnd.Services.Jwt
{
    /// <summary>
    /// Менеджер по Jwt токену
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Генерация AccessToken
        /// </summary>
        /// <param name="claim">Claims</param>
        /// <returns></returns>
        string GenerateAccessToken(IEnumerable<Claim> claim);

        /// <summary>
        /// Генерация токена обновителя по AccessToken
        /// </summary>
        /// <param name="AccessToken">AccessToken</param>
        /// <returns></returns>
        string GenerateRefreshToken(string AccessToken);

        /// <summary>
        /// Получить ClaimsPrincipial из AccessToken
        /// </summary>
        /// <param name="AccessToken">AccessToken</param>
        /// <returns></returns>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string AccessToken);
    }
}