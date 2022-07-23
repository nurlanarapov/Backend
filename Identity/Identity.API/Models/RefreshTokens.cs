using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
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
        /// Пользователь
        /// </summary>
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
    }
}