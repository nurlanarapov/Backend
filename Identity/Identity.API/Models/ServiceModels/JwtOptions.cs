namespace Identity.API.Models.ServiceModels
{
    /// <summary>
    /// Опции Jwt токена
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Издатель
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string Audience { get; set; }
         
        /// <summary>
        /// Срок активности в минутах
        /// </summary>
        public int AccessTokenLifeTime { get; set; }

        /// <summary>
        /// Срок активности в минутах
        /// </summary>
        public int RefreshTokenLifeTime { get; set; }
    }
}
