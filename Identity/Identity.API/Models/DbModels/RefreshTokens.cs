using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Identity.API.Models.DbModels
{
    /// <summary>
    /// Токены обновители
    /// </summary>
    public class RefreshTokens
    {
        /// <summary>
        /// Ключ
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Токен обновитель
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Срок окончания токена обновителя
        /// </summary>
        [Required]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [Required]
        public DateTime Create { get; set; }

        /// <summary>
        /// Аннулировано
        /// </summary>
        [Required]
        public bool Revoked { get; set; }

        /// <summary>
        /// Является ли этот токен активным
        /// </summary>
        [NotMapped]
        private bool IsActive => DateTime.Now > ExpiryDate && !Revoked;

        /// <summary>
        /// Устройство
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [Required]
        [JsonIgnore]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public AppUser User { get; set; }
    }
}