using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackEnd.Data.Authentication
{
    /// <summary>
    /// Данные для авторизации
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Логин
        /// </summary>
        [Required(ErrorMessage = "Введите логин")]
        [JsonPropertyName("userName")]
        public string userName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Введите пароль")]
        [JsonPropertyName("password")]
        public string password { get; set; }
    }
}