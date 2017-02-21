using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Facebook.LoginKit;
using Foundation;
using IsiiSports.Auth;
using IsiiSports.Helpers;
using IsiiSports.iOS.Auth;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthiOS))]
namespace IsiiSports.iOS.Auth
{
    public class AuthiOS : IAuthentication
    {
        private TaskCompletionSource<string> loginTcs;

        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null)
        {
            try
            {
                string accessToken = null;

                if (provider == MobileServiceAuthenticationProvider.Facebook)
                    accessToken = await LoginFacebookAsync();               

                if (provider == MobileServiceAuthenticationProvider.Google)
                    accessToken = await LoginGoogleAsync();

                //var zumoPayload = new JObject {["access_token"] = accessToken};
                var zumoPayload = new Dictionary<string, string> { {"access_token", accessToken} };

                return await client.LoginAsync(GetController(), provider, zumoPayload);

            }
            catch (Exception e)
            {
                e.Data["method"] = "LoginAsync";
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
                    Settings.AuthToken = user.MobileServiceAuthenticationToken;
                    Settings.UserId = user.UserId;
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
            var store = Foundation.NSHttpCookieStorage.SharedStorage;
            var cookies = store.Cookies;

            foreach (var c in cookies)
            {
                store.DeleteCookie(c);
            }
        }
        private static UIKit.UIViewController GetController()
        {
            var window = UIKit.UIApplication.SharedApplication.KeyWindow;
            var root = window.RootViewController;
            if (root == null)
                return null;

            var current = root;
            while (current.PresentedViewController != null)
            {
                current = current.PresentedViewController;
            }

            return current;
        }

        #region Facebook Client Flow

        public async Task<string> LoginFacebookAsync()
        {
            loginTcs = new TaskCompletionSource<string>();
            var loginManager = new LoginManager();

            loginManager.LogInWithReadPermissions(new[] { "public_profile" }, GetController(), LoginTokenHandler);
            return await loginTcs.Task;
        }

        private void LoginTokenHandler(LoginManagerLoginResult loginResult, NSError error)
        {
            if (loginResult.Token != null)
            {
                loginTcs.TrySetResult(loginResult.Token.TokenString);
            }
            else
            {
                loginTcs.TrySetException(new Exception("Facebook Client Flow Login Failed"));
            }
        }

        #endregion

        #region Google Client Flow

        public async Task<string> LoginGoogleAsync()
        {
            loginTcs = new TaskCompletionSource<string>();
            //var loginManager = new LoginManager();

            //loginManager.LogInWithReadPermissions(new[] { "public_profile" }, GetController(), LoginTokenHandler);
            return await loginTcs.Task;
        }

        #endregion
    }
}
