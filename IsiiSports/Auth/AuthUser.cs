using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
