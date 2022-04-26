using BackEnd.Data.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackEnd.Helpers.Managers
{
    /// <summary>
    /// Менеджер по Jwt токену
    /// </summary>
    public class JwtManager : IJwtManager  {

        #region variables
        
        /// <summary>
        /// Jwt опции
        /// </summary>
        private JwtOptions _jwtOptions { get; set; }
        
        #endregion

        #region constuctor
       
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="jwtOptions"></param>
        public JwtManager(JwtOptions jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }

        #endregion

        #region methods

        /// <summary>
        /// Генерация токена, Исключении SecurityTokenException
        /// </summary>
        /// <returns></returns>
        public string GenerateAccessToken(IEnumerable<Claim> claim)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_jwtOptions.Issuer,
              _jwtOptions.Audience,
              claim,
              expires: DateTime.Now.AddDays(_jwtOptions.AccessTokenLifeTime),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Генерация токена обновителя по AccessToken
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken(string AccessToken)
        {
            ClaimsPrincipal claimsPrincipal = GetPrincipalFromExpiredToken(AccessToken);
            return GenerateRefreshToken();
        }

        /// <summary>
        /// Генерация токена обновителя
        /// </summary>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Получить Claims по токену
        /// </summary>
        /// <param name="token">токен</param>
        /// <returns></returns>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string AccessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(AccessToken, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        
        #endregion
    }

    /// <summary>
    /// Менеджер по Jwt токену
    /// </summary>
    public interface IJwtManager
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
        /// <param name="AccessToken"></param>
        /// <returns></returns>
        string GenerateRefreshToken(string AccessToken);
    }
}