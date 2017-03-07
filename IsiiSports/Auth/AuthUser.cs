using Microsoft.WindowsAzure.MobileServices;

namespace IsiiSports.Auth
{
    public class AuthUser
    {
        public MobileServiceUser MobileServiceUser { get; set; }
        public UserInfo UserInfo { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
