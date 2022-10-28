using System;

namespace Identity.API.Models.DtoModels.Response
{
    /// <summary>
    /// Токены обновители
    /// </summary>
    public class RefreshTokens
    {
        /// <summary>
        /// Ключ
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Токен обновитель
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Срок окончания токена обновителя
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Create { get; set; }

        /// <summary>
        /// Аннулировано
        /// </summary>
        public bool Revoked { get; set; }

        /// <summary>
        /// Является ли этот токен активным
        /// </summary>
        private bool IsActive => DateTime.Now > ExpiryDate && !Revoked;

        /// <summary>
        /// Устройство
        /// </summary>
        public string Device { get; set; }
    }
}
