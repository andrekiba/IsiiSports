﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace IsiiSports.Auth
{
    public interface IAuthentication
    {
        Task<AuthUser> LoginAsync(IMobileServiceClient client, string provider, IDictionary<string, string> paramameters = null, bool clientFlow = false);
        Task<bool> LogoutAsync(IMobileServiceClient client, string provider, bool clientFlow = false);
        Task<bool> RefreshUser(IMobileServiceClient client);
        void ClearCookies();
    }
}
