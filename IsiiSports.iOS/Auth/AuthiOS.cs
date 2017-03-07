using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Google.SignIn;
using IsiiSports.Auth;
using IsiiSports.Base;
using IsiiSports.iOS.Auth;
using Microsoft.WindowsAzure.MobileServices;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Settings = IsiiSports.Helpers.Settings;
using Task = System.Threading.Tasks.Task;

[assembly: Dependency(typeof(AuthiOS))]
namespace IsiiSports.iOS.Auth
{
	public class AuthiOS : IAuthentication, ISignInUIDelegate
    {
		public async Task<AuthUser> LoginAsync(IMobileServiceClient client, string provider, IDictionary<string, string> parameters = null, bool clientFlow = false)
        {
            try
            {
                var authProvider = (MobileServiceAuthenticationProvider)Enum.Parse(typeof(MobileServiceAuthenticationProvider), provider);
				var authUser = new AuthUser();

                if (clientFlow)
                {
                    #region Client Flow

                    JObject jToken = null;

                    if (authProvider == MobileServiceAuthenticationProvider.Facebook)
                    {
                        var facebookToken = await LoginFacebookAsync();
                        authUser.UserInfo = await GetFacebookProfileAsync(facebookToken.TokenString);
                        authUser.AccessToken = facebookToken.TokenString;
                        //authUser.RefreshToken = ...

                        jToken = JObject.FromObject(new
                        {
                            access_token = facebookToken.TokenString,
                            //authorization_code = ...
                            //id_token = ...
                        });
                    }

                    if (authProvider == MobileServiceAuthenticationProvider.Google)
                    {
                        var googleUser = await LoginGoogleAsync();

                        authUser.UserInfo = new UserInfo
                        {
                            UserId = googleUser.UserID,
                            Email = googleUser.Profile.Email,
                            UserName = googleUser.Profile.Name,
                            ProfileImageUrl = googleUser.Profile.HasImage ? googleUser.Profile.GetImageUrl(100).AbsoluteString : null
                        };
                        authUser.AccessToken = googleUser.Authentication.AccessToken;
                        authUser.RefreshToken = googleUser.Authentication.RefreshToken;

                        jToken = JObject.FromObject(new
                        {
                            access_token = googleUser.Authentication.AccessToken,
                            authorization_code = googleUser.ServerAuthCode,
                            id_token = googleUser.Authentication.IdToken
                        });
                    }

                    authUser.MobileServiceUser = await client.LoginAsync(authProvider, jToken);

                    #endregion
                }
                else
                {
                    #region Server Flow

                    authUser.MobileServiceUser = await client.LoginAsync(GetController(), authProvider, new Dictionary<string, string> { { "access_type", "offline" } });
                    await SetIdentityValues(authUser);

                    if (authProvider == MobileServiceAuthenticationProvider.Facebook)
                    {
                        authUser.UserInfo = await GetFacebookProfileAsync(authUser.AccessToken);
                    }
                    if (authProvider == MobileServiceAuthenticationProvider.Google)
                    {
                        authUser.UserInfo = await GetGoogleProfileAsync(authUser.AccessToken);
                    }

                    #endregion
                }

                return authUser;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }
        public async Task<bool> LogoutAsync(IMobileServiceClient client, string provider, bool clientFlow = false)
        {
            try
            {
                #region Client Flow

                if (clientFlow)
                {
                    var authProvider = (MobileServiceAuthenticationProvider)Enum.Parse(typeof(MobileServiceAuthenticationProvider), provider);

                    if (authProvider == MobileServiceAuthenticationProvider.Facebook)
                    {
                        LogoutFacebook();
                    }

                    if (authProvider == MobileServiceAuthenticationProvider.Google)
                    {
                        LogoutGoogle();
                    }
                }

                #endregion

                await client.LogoutAsync();
                ClearCookies();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return false;
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
        private static async Task SetIdentityValues(AuthUser user)
        {
            //Manually calling /.auth/me against remote server because InvokeApiAsync against a local instance of Azure will never hit the remote endpoint
            //If you are not debugging your service locally, you can just use InvokeApiAsync("/.auth/me") 
            JToken identity;
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                client.DefaultRequestHeaders.Add("X-ZUMO-AUTH", user.MobileServiceUser.MobileServiceAuthenticationToken);
                var json = await client.GetStringAsync($"{Keys.AppUrl}/.auth/me");
                identity = JsonConvert.DeserializeObject<JToken>(json);
            }

            if (identity != null)
            {
                user.AccessToken = identity[0].Value<string>("access_token");
                user.RefreshToken = identity[0].Value<string>("refresh_token");
            }
        }

        #region Facebook Client Flow

        public async Task<AccessToken> LoginFacebookAsync()
        {           
			var loginManager = new LoginManager
            {
				LoginBehavior = LoginBehavior.Native
            };

			var loginResult = await loginManager.LogInWithReadPermissionsAsync(new[] { "public_profile" }, GetController());

			if (loginResult.Token != null)
				return loginResult.Token;

            var error = new StringBuilder();
            error.AppendLine("Facebook Client Flow Login Failed");
            throw new Exception(error.ToString());
        }

        public void LogoutFacebook()
        {
            var loginManager = new LoginManager
            {
                LoginBehavior = LoginBehavior.Native
            };

            loginManager.LogOut();
        }

        #endregion

        #region Facebook Server Flow

        public async Task<UserInfo> GetFacebookProfileAsync(string token)
        {
            var facebookUser = new UserInfo();

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                using (var response = await client.GetAsync("https://graph.facebook.com/v2.8/me?fields=id,name,email,picture{url}&access_token=" + token))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var o = JObject.Parse(await response.Content.ReadAsStringAsync());
                        facebookUser.UserName = o["name"].ToString();
                        facebookUser.UserId = o["id"].ToString();
                        facebookUser.Email = o["email"].ToString();
                        facebookUser.ProfileImageUrl = o["picture"]["data"]["url"].ToString();
                    }
                    else
                    {
                        throw new Exception(string.Join(" ", (int)response.StatusCode, response.StatusCode));
                    }                   
                }
            }

            return facebookUser;
        }

        #endregion

        #region Google Client Flow

        public async Task<Google.SignIn.GoogleUser> LoginGoogleAsync()
        {
            var googleLoginTcs = new TaskCompletionSource<Google.SignIn.GoogleUser>();

            //SignIn.SharedInstance.Delegate = this;
			SignIn.SharedInstance.UIDelegate = this;

            SignIn.SharedInstance.SignedIn += (sender, e) => {

                if (e.User != null && e.Error == null)
                {
                    googleLoginTcs.TrySetResult(e.User);
                }
                else
                {
                    var error = new StringBuilder();
                    error.AppendLine("Google Client Flow Login Failed");
                    error.AppendLine("Error Code: " + e.Error.Code);
                    error.AppendLine("Error Message: " + e.Error.LocalizedDescription + e.Error.LocalizedFailureReason);
                    googleLoginTcs.TrySetException(new Exception(error.ToString()));
                }
            };

            SignIn.SharedInstance.Disconnected += (sender, e) => {

            };

			//SignIn.SharedInstance.SignInUserSilently();
			SignIn.SharedInstance.SignInUser();

            return await googleLoginTcs.Task;
        }

        public void LogoutGoogle()
        {
            SignIn.SharedInstance.SignOutUser();
        }

        public void Dispose()
		{
			SignIn.SharedInstance.Dispose();
		}

		public IntPtr Handle => GetController().Handle;

        #endregion

        #region Google Server Flow

        public async Task<UserInfo> GetGoogleProfileAsync(string token)
        {
            var googleUser = new UserInfo();

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                const string url = "https://www.googleapis.com/oauth2/v2/userinfo";
                //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

                using (var response = await client.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var o = JObject.Parse(await response.Content.ReadAsStringAsync());
                        googleUser.UserName = o["name"].ToString();
                        googleUser.UserId = o["id"].ToString();
                        googleUser.Email = o["email"].ToString();
                        googleUser.ProfileImageUrl = o["picture"].ToString();
                    }
                    else
                    {
                        throw new Exception(string.Join(" ", (int)response.StatusCode, response.StatusCode));
                    }
                }
            }

            return googleUser;
        }

        #endregion
    }
}
