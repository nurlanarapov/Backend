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

namespace BackEnd.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        #region variables

        /// <summary>
        /// Jwt Менеджер
        /// </summary>
        private JwtService _jwtService{ get; set; }

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

        #endregion

        #region constructor

        public AuthenticationService(JwtOptions jwtOptions, 
                                     JwtService jwtService, 
                                     UserManager<AppUser> userManager,
                                     SignInManager<AppUser> signInManager,
                                     AppDbContext appDbContext)
        {
            _jwtService = jwtService;
            _jwtOptions = jwtOptions;
            _userManager = userManager;
            _singInManager = signInManager;
            _appDbContext = appDbContext;
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

            IEnumerable<Claim> claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.UserName, ClaimValueTypes.String),
                new Claim(ClaimTypes.Name, appUser.Name, ClaimValueTypes.String),
                new Claim(ClaimTypes.Email, appUser.Email, ClaimValueTypes.String)
            };

            string accessToken = _jwtService.GenerateAccessToken(claims);
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
                RefreshToken = refreshToken
            };

        }

        /// <summary>
        /// Аннулировать токен
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="RefreshToken"></param>
        /// <returns></returns>
        public async Task<bool> InvokeAsync(string AccessToken, string RefreshToken)
        {
            ClaimsPrincipal claimsPrincipal = _jwtService.GetPrincipalFromExpiredToken(AccessToken);
            
            AppUser appUser = await _userManager.GetUserAsync(claimsPrincipal);
            if (appUser is null)
                throw new SecurityTokenException("Invalid token");

            RefreshTokens refreshToken = _appDbContext.RefreshTokens.FirstOrDefault(x => x.RefreshToken == RefreshToken);
            if(refreshToken is null)
                throw new SecurityTokenException("Invalid token");

            _appDbContext.RefreshTokens.Remove(refreshToken);
            return _appDbContext.SaveChanges() > 0;             
        }

        #endregion
    }

    /// <summary>
    /// Интерфейс 
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="UserName">Имя пользователя</param>
        /// <param name="Password">Пароль</param>
        /// <returns></returns>
        Task<JwtToken> AuthenticateAsync(string UserName, string Password);

        /// <summary>
        /// Invoke Token
        /// </summary>
        /// <param name="AccessToken">AccessToken</param>
        /// <param name="RefreshToken">RefreshToken</param>
        /// <returns></returns>
        Task<bool> InvokeAsync(string AccessToken, string RefreshToken);

        /// <summary>
        /// Обновить токен по RefreshToken
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="RefreshToken"></param>
        /// <returns></returns>
        //Task<JwtToken> RefreshToken(string AccessToken, string RefreshToken);
    }
}