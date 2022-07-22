using BackEnd.Data.Authentication;
using BackEnd.Models;
using BackEnd.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AccountController(UserManager<AppUser> userManager, IAuthenticationService authenticationService)
        {
            _userManager = userManager;
            _authenticationService = authenticationService;
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
                JwtToken jwtToken = await _authenticationService.AuthenticateAsync(login.UserName, login.Password);
                return Ok(jwtToken); 
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