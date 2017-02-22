using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsiiSports.Auth
{
    public class GoogleUser
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
