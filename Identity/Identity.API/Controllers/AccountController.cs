using Request = Identity.API.Models.DtoModels.Request;
using Response = Identity.API.Models.DtoModels.Response;
using Service = Identity.API.Models.ServiceModels;
using Identity.API.Services;
using Identity.API.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Identity.API.Models.DbModels;
using Identity.API.Models.DtoModels;
using System.Linq;
using Identity.API.Middlewares.Exceptions;
using Identity.API.Filters;

namespace Identity.API.Controllers
{
    [ValidationAttributeFilter]
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        #region variables

        private IAuthenticationService _authenticationService;

        private readonly ILogger<AccountController> _logger;

        private UserService _userInfoService;

        #endregion

        #region constructor

        public AccountController(IAuthenticationService authenticationService,
                                 ILogger<AccountController> logger,
                                 UserService userInfoService)
        {
            _authenticationService = authenticationService;
            _logger = logger;
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
            AppUser CurrentUser = await _userInfoService.GetCurrentUser();

            Response.UserResponse user = new Response.UserResponse()
            {
                Id = CurrentUser.Id,
                Name = CurrentUser.Name,
                SurName = CurrentUser.SurName,
                MiddleName = CurrentUser.MiddleName
            };

            CurrentUser.RefreshTokens.ToList().ForEach(x => user.RefreshTokens.Add(new Response.RefreshTokens
            {
                Id = x.Id,
                RefreshToken = x.RefreshToken,
                Device = x.Device,
                Create = x.Create,
                ExpiryDate = x.ExpiryDate
            }));

            return Ok(user);
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="UserName">Логин </param>
        /// <param name="Password">Пароль</param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(Request.Login login)
        {
            Service.JwtToken jwtToken = await _authenticationService.AuthenticateAsync(login.UserName, login.Password);
            JwtToken token = new JwtToken()
            {
                AccessToken = jwtToken.AccessToken,
                RefreshToken = jwtToken.RefreshToken,
                ExpiryDate = jwtToken.ExpiryDate
            };
            return Ok(jwtToken);
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
            Service.JwtToken jwtResult = await _authenticationService.RefreshToken(jwtToken.AccessToken, jwtToken.RefreshToken);

            JwtToken token = new JwtToken()
            {
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken,
                ExpiryDate = jwtResult.ExpiryDate
            };

            return Ok(token);
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
                    return Ok();
                else
                    return BadRequest();
            }
            catch (SecurityTokenException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Регистрация обычного пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("client-register")]
        public async Task<IActionResult> ClientRegister(Request.User user)
        {
            AppUser appUser = new AppUser()
            {
                Email = user.Email,
                Name = user.Name,
                SurName = user.SurName,
                MiddleName = user.MiddleName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName
            };
            string password = user.Password;
            await _userInfoService.ClientRegister(appUser, password);
            return Ok();
        }

        #endregion
    }
}