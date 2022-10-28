using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Identity.API.Models.DtoModels
{
    public class JwtToken
    {
        /// <summary>
        /// JwtToken
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// RefreshToken токен обновитель
        /// </summary>
        [JsonPropertyName("refresh_token")]
        [Required(ErrorMessage = "Error in RefreshToken")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Срок жизни токена обновителя
        /// </summary>
        [JsonPropertyName("expiry_date")]
        public DateTime ExpiryDate { get; set; }
    }
}
