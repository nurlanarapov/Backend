using Newtonsoft.Json;

namespace BackEnd.Data.Authentication
{
    /// <summary>
    /// JwtToken
    /// </summary>
    public class JwtToken
    {
        /// <summary>
        /// Токен
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}