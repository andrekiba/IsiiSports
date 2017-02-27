using Newtonsoft.Json;

namespace IsiiSports.Auth
{
    public class GoogleUser
    {
        [JsonProperty("id")]
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public string ServerAuthCode { get; set; }

        [JsonProperty("picture")]
        public string ProfileImageUrl { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string UserName { get; set; }
    }
}
