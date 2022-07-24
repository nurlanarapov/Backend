using BackEnd.Data.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackEnd.Services.Jwt
{
    /// <summary>
    /// Менеджер по Jwt токену
    /// </summary>
    public class JwtService : IJwtService  {

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
        public JwtService(JwtOptions jwtOptions)
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
              expires: DateTime.Now.AddMinutes(_jwtOptions.AccessTokenLifeTime),
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
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string AccessToken)
        {
            try
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
            catch (Exception)
            {
                throw new SecurityTokenException("Invalid token");
            }         
        }
        
        #endregion
    }
}