using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models.DtoModels.Request
{
    /// <summary>
    /// Авторизация
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "LoginRequired")]
        public string UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "PasswordRequired")]
        public string Password { get; set; }
    }
}