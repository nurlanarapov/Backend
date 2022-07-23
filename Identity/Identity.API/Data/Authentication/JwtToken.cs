namespace BackEnd.Data.Authentication
{
    /// <summary>
    /// JwtToken
    /// </summary>
    public class JwtToken
    {
        /// <summary>
        /// AccessTOken
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// RefreshToken токен обновитель
        /// </summary>
        public string RefreshToken { get; set; }
    }
}