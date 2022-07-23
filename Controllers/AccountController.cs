using BackEnd.Data.Authentication;
using BackEnd.Models;
using BackEnd.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;

        private IAuthenticationService _authenticationService;

        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, IAuthenticationService authenticationService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
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
                if(ModelState.IsValid){
                    _logger.LogInformation("Выполнен вход");
                    JwtToken jwtToken = await _authenticationService.AuthenticateAsync(login.userName, login.password);
                    return Ok(jwtToken);
                }
                return BadRequest(ModelState);
                
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}