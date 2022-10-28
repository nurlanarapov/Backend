using Identity.API.Models.ServiceModels;
using System.Threading.Tasks;

namespace Identity.API.Services
{
    /// <summary>
    /// Интерфейс авторизации
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
        Task<bool> InvokeAsync(string RefreshToken);

        /// <summary>
        /// Обновить токен по RefreshToken
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="RefreshToken"></param>
        /// <returns></returns>
        Task<JwtToken> RefreshToken(string AccessToken, string RefreshToken);
    }
}