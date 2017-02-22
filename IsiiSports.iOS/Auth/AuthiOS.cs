using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Google.SignIn;
using IsiiSports.Auth;
using IsiiSports.iOS.Auth;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Settings = IsiiSports.Helpers.Settings;

[assembly: Dependency(typeof(AuthiOS))]
namespace IsiiSports.iOS.Auth
{
    public class AuthiOS : IAuthentication
    {
        public async Task<AuthUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null)
        {
            try
            {
                var authUser = new AuthUser();
                string accessToken = null;

                if (provider == MobileServiceAuthenticationProvider.Facebook)
                {
                    var facebookToken = await LoginFacebookAsync();
                    accessToken = facebookToken.TokenString;
                    authUser.FacebookUser = new FacebookUser
                    {
                        UserId = facebookToken.UserID,
                        AccessToken = facebookToken.TokenString
                    };                    
                }

                if (provider == MobileServiceAuthenticationProvider.Google)
                {
                    var googleUser = await LoginGoogleAsync();
                    accessToken = googleUser.Authentication.AccessToken;

                    authUser.GoogleUser = new IsiiSports.Auth.GoogleUser
                    {
                        UserId = googleUser.UserID,
                        AccessToken = accessToken,
                        IdToken = googleUser.Authentication.IdToken,
                        Email = googleUser.Profile.Email,
                        UserName = googleUser.Profile.Name,
                        ProfileImageUrl = googleUser.Profile.HasImage ? googleUser.Profile.GetImageUrl(100).AbsoluteString : null
                    };
                    
                }

                var zumoPayload = new Dictionary<string, string> {{"access_token", accessToken}};

                authUser.MobileServiceUser = await client.LoginAsync(GetController(), provider, zumoPayload);

                return authUser;

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

        public async Task<AccessToken> LoginFacebookAsync()
        {
            var facebookLoginTcs = new TaskCompletionSource<AccessToken>();
            var loginManager = new LoginManager();

            loginManager.LogInWithReadPermissions(new[] { "public_profile" }, GetController(),
                (loginResult, error) =>
                {
                    if (loginResult.Token != null)
                    {
                        facebookLoginTcs.TrySetResult(loginResult.Token); 
                    }            
                    else
                        facebookLoginTcs.TrySetException(new Exception("Facebook Client Flow Login Failed"));
                });

            return await facebookLoginTcs.Task;
        }

        #endregion

        #region Google Client Flow

        public async Task<Google.SignIn.GoogleUser> LoginGoogleAsync()
        {
            var googleLoginTcs = new TaskCompletionSource<Google.SignIn.GoogleUser>();

            //SignIn.SharedInstance.UIDelegate = this;

            SignIn.SharedInstance.SignedIn += (sender, e) => {

                if (e.User != null && e.Error == null)
                {
                    googleLoginTcs.TrySetResult(e.User);
                }
                else
                {
                    googleLoginTcs.TrySetException(new Exception("Google Client Flow Login Failed"));
                }
            };

            SignIn.SharedInstance.Disconnected += (sender, e) => {

            };

            SignIn.SharedInstance.SignInUserSilently();

            return await googleLoginTcs.Task;
        }

        #endregion
    }
}
