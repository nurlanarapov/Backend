using Identity.API.Middlewares.Exceptions;
using Identity.API.Models.DbModels;
using Identity.API.Models.DbModels.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Services.User
{
    public class UserService
    {
        #region variables

        private IHttpContextAccessor _httpContextAccessor { get; set; }

        private UserManager<AppUser> _userManager { get; set; }

        private AppDbContext _appDbContext { get; set; }

        private IStringLocalizer<UserService> _localizer;

        #endregion

        #region constructor

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, AppDbContext appDbContext, IStringLocalizer<UserService> localizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _localizer = localizer;
        }

        #endregion

        #region methods

        /// <summary>
        /// Получить текущего пользователя
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        public async Task<AppUser> GetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                appUser.RefreshTokens = _appDbContext.RefreshTokens.Where(x => x.UserId == appUser.Id).ToList();
                return appUser;
            }
            else
                throw new SecurityTokenException(_localizer["UserNotFound"]);
        }

        /// <summary>
        /// Регистрация пользователей
        /// </summary>
        /// <param name="appUser">Юзер</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        /// <exception cref="System.Exception"></exception>
        private async Task<AppUser> register(AppUser appUser, string role, string password)
        {
            AppUser user = await _userManager.FindByEmailAsync(appUser.Email);

            if (user != null)
                throw new ApplicationException(_localizer["UserExist"]);

            if (string.IsNullOrEmpty(role))
                throw new System.Exception("Empty Role");

            var result = await _userManager.CreateAsync(appUser, password);

            if (!result.Succeeded)
                throw new ApplicationException(result.Errors.Select(x => x.Description).FirstOrDefault(), System.Net.HttpStatusCode.BadRequest);

            result = await _userManager.AddToRoleAsync(appUser, role);

            if (!result.Succeeded)
                throw new ApplicationException(result.Errors.Select(x => x.Description).FirstOrDefault(), System.Net.HttpStatusCode.BadRequest);

            return appUser;
        }

        /// <summary>
        /// Регистрация клиента
        /// </summary>
        /// <param name="appUser">Юзер</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        public async Task<AppUser> ClientRegister(AppUser appUser, string password)
        {
            return await register(appUser, "User", password);
        }

        #endregion
    }
}