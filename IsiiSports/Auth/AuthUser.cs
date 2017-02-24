using Microsoft.WindowsAzure.MobileServices;

namespace IsiiSports.Auth
{
    public class AuthUser
    {
        public  MobileServiceUser MobileServiceUser { get; set; }
        public GoogleUser GoogleUser { get; set; }
        public FacebookUser FacebookUser { get; set; }
    }
}
