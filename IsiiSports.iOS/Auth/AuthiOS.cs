﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Google.SignIn;
using IsiiSports.Auth;
using IsiiSports.iOS.Auth;
using Microsoft.WindowsAzure.MobileServices;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Settings = IsiiSports.Helpers.Settings;

[assembly: Dependency(typeof(AuthiOS))]
namespace IsiiSports.iOS.Auth
{
	public class AuthiOS : IAuthentication, ISignInUIDelegate
    {
		public async Task<AuthUser> LoginAsync(IMobileServiceClient client, string provider, IDictionary<string, string> parameters = null)
        {
            try
            {
                var authProvider = (MobileServiceAuthenticationProvider)Enum.Parse(typeof(MobileServiceAuthenticationProvider), provider);
				var authUser = new AuthUser();

                #region Client Flow

                JObject jToken = null;

                if (authProvider == MobileServiceAuthenticationProvider.Facebook)
                {
                    var facebookToken = await LoginFacebookAsync();
                    authUser.FacebookUser = new FacebookUser
                    {
                        UserId = facebookToken.UserID,
                        AccessToken = facebookToken.TokenString
                    };

                    jToken = JObject.FromObject(new
                    {
                        access_token = facebookToken.TokenString,
                        //authorization_code = googleUser.ServerAuthCode,
                        //id_token = googleUser.Authentication.IdToken,
                    });
                }

                if (authProvider == MobileServiceAuthenticationProvider.Google)
                {
                    var googleUser = await LoginGoogleAsync();

                    authUser.GoogleUser = new IsiiSports.Auth.GoogleUser
                    {
                        UserId = googleUser.UserID,
                        AccessToken = googleUser.Authentication.AccessToken,
                        IdToken = googleUser.Authentication.IdToken,
                        ServerAuthCode = googleUser.ServerAuthCode,
                        Email = googleUser.Profile.Email,
                        UserName = googleUser.Profile.Name,
                        ProfileImageUrl = googleUser.Profile.HasImage ? googleUser.Profile.GetImageUrl(100).AbsoluteString : null
                    };

                    jToken = JObject.FromObject(new {
                        access_token = googleUser.Authentication.AccessToken,
                        authorization_code = googleUser.ServerAuthCode,
                        id_token = googleUser.Authentication.IdToken
                    });
                }

                authUser.MobileServiceUser = await client.LoginAsync(authProvider, jToken);
                
                #endregion

                #region Server Flow

                //authUser.MobileServiceUser = await client.LoginAsync(GetController(), authProvider, null);

                #endregion

                return authUser;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<bool> LogoutAsync(IMobileServiceClient client, string provider)
        {
            try
            {
                #region Client Flow

                var authProvider = (MobileServiceAuthenticationProvider)Enum.Parse(typeof(MobileServiceAuthenticationProvider), provider);

                if (authProvider == MobileServiceAuthenticationProvider.Facebook)
                {
                    LogoutFacebook();
                }

                if (authProvider == MobileServiceAuthenticationProvider.Google)
                {
                    LogoutGoogle();
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

			throw new Exception("Facebook Client Flow Login Failed");
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

        public async Task<FacebookUser> GetFacebookProfileAsync(string token)
        {
            var facebookUser = new FacebookUser();

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                using (var response = await client.GetAsync("https://graph.facebook.com/v2.8/me?fields=id,name,email,picture{url}&access_token=" + token))
                {
                    var o = JObject.Parse(await response.Content.ReadAsStringAsync());
                    facebookUser.UserName = o["name"].ToString();
                    facebookUser.UserId = o["id"].ToString();
                    facebookUser.Email = o["email"].ToString();
                    facebookUser.ProfileImageUrl = o["picture"]["data"]["url"].ToString();
                }
            }

            facebookUser.AccessToken = token;

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
                    googleLoginTcs.TrySetException(new Exception("Google Client Flow Login Failed"));
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

        public async Task<IsiiSports.Auth.GoogleUser> GetGoogleProfileAsync(string token)
        {
            IsiiSports.Auth.GoogleUser googleUser;

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                const string url = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json";
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var json = await client.GetStringAsync(url);
                googleUser = JsonConvert.DeserializeObject<IsiiSports.Auth.GoogleUser>(json);
            }

            googleUser.AccessToken = token;

            return googleUser;
        }

        #endregion
    }
}
