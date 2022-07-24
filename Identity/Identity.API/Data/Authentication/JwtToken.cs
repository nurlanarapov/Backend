using System;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Data.Authentication
{
    /// <summary>
    /// JwtToken
    /// </summary>
    public class JwtToken
    {
        /// <summary>
        /// AccessToken
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// RefreshToken токен обновитель
        /// </summary>
        [Required(ErrorMessage = "Error in RefreshToken")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Срок жизни токена обновителя
        /// </summary>
        public DateTime ExpiryDate { get; set; }
    }
}