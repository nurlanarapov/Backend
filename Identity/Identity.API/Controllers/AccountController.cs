using Identity.API.Models;
using Identity.API.Services;
using Identity.API.Services.User;
using Identity.API.Shared.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        #region variables
        private IAuthenticationService _authenticationService;

        private readonly ILogger<AccountController> _logger;

        private IHttpContextAccessor _httpContextAccessor;

        private UserInfoService _userInfoService;

        #endregion

        #region constructor
        public AccountController(IAuthenticationService authenticationService,  
                                 ILogger<AccountController> logger, 
                                 IHttpContextAccessor httpContextAccessor,
                                 UserInfoService userInfoService)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userInfoService = userInfoService;
        }

        #endregion

        #region methods

        /// <summary>
        /// Информация по сервису
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                AppUser CurrentUser = await _userInfoService.GetCurrentUser();

                User user = new User()
                {
                    Id = CurrentUser.Id,
                     Name = CurrentUser.Name,
                     SurName = CurrentUser.SurName,
                     MiddleName = CurrentUser.MiddleName,
                     RefreshTokens = CurrentUser.RefreshTokens
                };

                return Ok(user);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception)
            {
               return BadRequest();
            }            
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="UserName">Логин </param>
        /// <param name="Password">Пароль</param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            try
            {
                if (ModelState.IsValid) {
                    Data.Authentication.JwtToken jwtToken = await _authenticationService.AuthenticateAsync(login.UserName, login.Password);
                    JwtToken token = new JwtToken()
                    {
                        AccessToken = jwtToken.AccessToken,
                        RefreshToken = jwtToken.RefreshToken,
                        ExpiryDate = jwtToken.ExpiryDate
                    };
                    return Ok(jwtToken);
                }
                return BadRequest(ModelState);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обновить токен
        /// </summary>
        /// <param name="jwtToken">Jwt Token</param>
        /// <returns></returns>
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(JwtToken jwtToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Data.Authentication.JwtToken jwtResult = await _authenticationService.RefreshToken(jwtToken.AccessToken, jwtToken.RefreshToken);
                    
                    JwtToken token = new JwtToken()
                    {
                        AccessToken = jwtResult.AccessToken,
                        RefreshToken = jwtResult.RefreshToken,
                        ExpiryDate = jwtResult.ExpiryDate
                    };

                    return Ok(token);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Аннулировать токен
        /// </summary>
        /// <param name="jwtToken">Jwt Token</param>
        /// <returns></returns>
        [HttpPost]
        [Route("invoke-token")]
        [Authorize]
        public async Task<IActionResult> InvokeToken(JwtToken jwtToken)
        {
            try
            {
              bool Invoked = await _authenticationService.InvokeAsync(jwtToken.RefreshToken);
                if (Invoked)
                    return Ok(jwtToken);
                else
                    return BadRequest();
            }
            catch(SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}