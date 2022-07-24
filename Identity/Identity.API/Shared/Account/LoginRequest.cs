using System.ComponentModel.DataAnnotations;

namespace Identity.API.Shared.Account
{
    /// <summary>
    /// Авторизация
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Введите логин")]
        public string UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
    }
}