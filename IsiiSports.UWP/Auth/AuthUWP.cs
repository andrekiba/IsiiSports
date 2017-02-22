using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using IsiiSports.Auth;
using IsiiSports.Helpers;
using IsiiSports.UWP.Auth;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthUWP))]
namespace IsiiSports.UWP.Auth
{
    public class AuthUWP : IAuthentication
    {
        public async Task<AuthUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null)
        {
            try
            {
                var authUser = new AuthUser {MobileServiceUser = await client.LoginAsync(provider, parameters)};
                return authUser;
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public virtual async Task<bool> RefreshUser(IMobileServiceClient client)
        {
            try
            {
                var user = await client.RefreshUserAsync();

                if (user != null)
                {
                    client.CurrentUser = user;
                    Settings.AzureAuthToken = user.MobileServiceAuthenticationToken;
                    Settings.AzureUserId = user.UserId;
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to refresh user: " + e);
            }

            return false;
        }

        public void ClearCookies()
        {

        }
    }
}
