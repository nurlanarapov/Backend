using BackEnd.Data.Authentication;
using BackEnd.Models;
using BackEnd.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private IAuthenticationService _authenticationService;

        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthenticationService authenticationService, ILogger<AccountController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        /// <summary>
        /// Информация по сервису
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Сервис авторизации. Mode = Информационные данные допилить ");
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="UserName">Логин </param>
        /// <param name="Password">Пароль</param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                if (ModelState.IsValid) {
                    JwtToken jwtToken = await _authenticationService.AuthenticateAsync(login.userName, login.password);
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
        [Route("refreshToken")]
        public async Task<IActionResult> RefreshToken(JwtToken jwtToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    JwtToken jwtResult = await _authenticationService.RefreshToken(jwtToken.AccessToken, jwtToken.RefreshToken);
                    return Ok(jwtResult);
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
        [Route("invokeToken")]
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
    }
}