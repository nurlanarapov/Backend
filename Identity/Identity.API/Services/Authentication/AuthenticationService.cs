using BackEnd.Data.Authentication;
using BackEnd.Services.Jwt;
using Microsoft.AspNetCore.Identity;
using BackEnd.Models;
using System.Threading.Tasks;
using System;
using System.Security.Authentication;
using System.Security.Claims;
using System.Collections.Generic;
using BackEnd.Models.Context;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        #region variables

        /// <summary>
        /// Jwt Менеджер
        /// </summary>
        private IJwtService _jwtService{ get; set; }

        /// <summary>
        /// Конфигурация Jwt Токена
        /// </summary>
        private JwtOptions _jwtOptions { get; set; }

        /// <summary>
        /// Управление пользователями
        /// </summary>
        private UserManager<AppUser> _userManager { get; set; }

        /// <summary>
        /// Менеджер по подписи пользователей
        /// </summary>
        private SignInManager<AppUser> _singInManager { get; set; }

        /// <summary>
        /// Context БД
        /// </summary>
        private AppDbContext _appDbContext { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region constructor

        public AuthenticationService(JwtOptions jwtOptions,
                                     IJwtService jwtService, 
                                     UserManager<AppUser> userManager,
                                     SignInManager<AppUser> signInManager,
                                     AppDbContext appDbContext,
                                     IHttpContextAccessor httpContextAccessor)
        {
            _jwtService = jwtService;
            _jwtOptions = jwtOptions;
            _userManager = userManager;
            _singInManager = signInManager;
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region methods

        /// <summary>
        /// Аутентификация
        /// </summary>
        /// <param name="UserName">Логин</param>
        /// <param name="Password">Пароль</param>
        /// <returns>JwtToken</returns>
        public async Task<JwtToken> AuthenticateAsync(string UserName, string Password)
        {
            AppUser appUser = await _userManager.FindByNameAsync(UserName);
            if (appUser is null)
                throw new AuthenticationException("Login or Password failed");

            SignInResult result = await _singInManager.CheckPasswordSignInAsync(appUser, Password, true);
            if(!result.Succeeded)
                throw new AuthenticationException("Login or Password failed");

            string accessToken = GenerateAccessToken(appUser);
            string refreshToken = _jwtService.GenerateRefreshToken(accessToken);

            RefreshTokens refreshTokens = new RefreshTokens()
            {
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.Now.AddDays(_jwtOptions.RefreshTokenLifeTime),
                Create = DateTime.Now,
                UserId = appUser.Id
            };
            _appDbContext.RefreshTokens.Add(refreshTokens);
            _appDbContext.SaveChanges();

            return new JwtToken()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryDate = refreshTokens.ExpiryDate
            };
        }

        /// <summary>
        /// Аннулировать токен
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="RefreshToken"></param>
        /// <returns></returns>
        public async Task<bool> InvokeAsync(string RefreshToken)
        {            
            AppUser appUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (appUser is null)
                throw new SecurityTokenException("Invalid User");

            RefreshTokens refreshToken = _appDbContext.RefreshTokens.FirstOrDefault(x => x.RefreshToken == RefreshToken);
            if(refreshToken is null)
                throw new SecurityTokenException("Invalid token");

            _appDbContext.RefreshTokens.Remove(refreshToken);

            if (_appDbContext.SaveChanges() <= 0)
                throw new Exception();

            return true;             
        }

        /// <summary>
        /// Обновить токен
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="RefreshToken"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException">Invalid token</exception>
        /// <exception cref="Exception">Error in insert db</exception>
        public async Task<JwtToken> RefreshToken(string AccessToken, string RefreshToken)
        {
            ClaimsPrincipal claimsPrincipal = _jwtService.GetPrincipalFromExpiredToken(AccessToken);

            AppUser appUser = await _userManager.GetUserAsync(claimsPrincipal);
            
            if (appUser is null)
                throw new SecurityTokenException("Invalid token");

            RefreshTokens refreshToken = appUser.RefreshTokens.FirstOrDefault(x => x.RefreshToken.Equals(RefreshToken) 
                                                                               && !x.Revoked);

            if (refreshToken is null)
                throw new SecurityTokenException("Invalid refresh token");

            appUser.RefreshTokens.Remove(refreshToken);

            string accessToken = GenerateAccessToken(appUser);

            refreshToken = new RefreshTokens()
            {
                RefreshToken = _jwtService.GenerateRefreshToken(accessToken),
                ExpiryDate = DateTime.Now.AddDays(_jwtOptions.RefreshTokenLifeTime),
                Create = DateTime.Now,
                UserId = appUser.Id
            };

            _appDbContext.RefreshTokens.Add(refreshToken);

            if (_appDbContext.SaveChanges() <= 0)
                throw new Exception("Error in service");

            JwtToken jwtToken = new JwtToken()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.RefreshToken,
                ExpiryDate = refreshToken.ExpiryDate
            };

            return jwtToken;
        }

        /// <summary>
        /// Сгенерировать токен для данного пользователя
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        private string GenerateAccessToken(AppUser appUser)
        {
            IEnumerable<Claim> claims = new Claim[]
           {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id, ClaimValueTypes.String),
                new Claim(ClaimTypes.Name, appUser.UserName, ClaimValueTypes.String),
                new Claim(ClaimTypes.Email, appUser.Email, ClaimValueTypes.String)
           };
           return _jwtService.GenerateAccessToken(claims);
        }

        #endregion
    }
}