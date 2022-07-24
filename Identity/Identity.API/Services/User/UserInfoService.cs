using Identity.API.Models;
using Identity.API.Models.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Services.User
{
    public class UserInfoService
    {
        #region variables

        private IHttpContextAccessor _httpContextAccessor { get; set; }

        private UserManager<AppUser> _userManager { get; set; }

        private AppDbContext _appDbContext { get; set; }

        #endregion

        #region constructor

        public UserInfoService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, AppDbContext appDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _appDbContext = appDbContext;
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
                throw new SecurityTokenException("Invalid Current User");
        }

        #endregion
    }
}
