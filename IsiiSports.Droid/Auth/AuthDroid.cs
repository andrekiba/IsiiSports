using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Auth;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Plus;
using Android.OS;
using Android.Runtime;
using IsiiSports.Auth;
using IsiiSports.Droid.Auth;
using IsiiSports.Droid.Base;
using IsiiSports.Helpers;
using Microsoft.WindowsAzure.MobileServices;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.CurrentActivity;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Forms;
using Debug = System.Diagnostics.Debug;

[assembly: Dependency(typeof(AuthDroid))]
namespace IsiiSports.Droid.Auth
{
    public class AuthDroid : IAuthentication
    {
        public async Task<AuthUser> LoginAsync(IMobileServiceClient client, string provider, IDictionary<string, string> parameters = null)
        {
            try
            {
                var authProvider = (MobileServiceAuthenticationProvider)Enum.Parse(typeof(MobileServiceAuthenticationProvider), Settings.AuthProvider);
                var authUser = new AuthUser();

                #region Client Flow

                JObject jToken = null;


                if (authProvider == MobileServiceAuthenticationProvider.Facebook)
                {
                    var facebookUser = await LoginFacebookAsync();
                    authUser.FacebookUser = facebookUser;

                    jToken = JObject.FromObject(new
                    {
                        access_token = facebookUser.AccessToken,
                        //authorization_code = googleUser.ServerAuthCode,
                        //id_token = googleUser.Authentication.IdToken,
                    });
                }

                if (authProvider == MobileServiceAuthenticationProvider.Google)
                {
                    var googleUser = await LoginGoogleAsync();
                    authUser.GoogleUser = googleUser;

                    jToken = JObject.FromObject(new
                    {
                        access_token = googleUser.AccessToken,
                        authorization_code = googleUser.ServerAuthCode,
                        id_token = googleUser.IdToken
                    });
                }

                authUser.MobileServiceUser = await client.LoginAsync(authProvider, jToken);

                #endregion

                #region Server Flow

                //authUser.MobileServiceUser = await client.LoginAsync(Forms.Context, authProvider, null);

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
                var authProvider = (MobileServiceAuthenticationProvider)Enum.Parse(typeof(MobileServiceAuthenticationProvider), provider);

                #region Client Flow

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
            catch (Exception)
            {
                //Insights.Report(ex, Insights.Severity.Error);
            }

            return false;
        }

        public void ClearCookies()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt >= 21)
                    Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #region Facebook Client Flow

        public async Task<FacebookUser> LoginFacebookAsync()
        {
            var facebookLoginTcs = new TaskCompletionSource<FacebookUser>();

            var callbackManager = CallbackManagerFactory.Create();
            var facebookCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = async loginResult =>
                {
                    if (loginResult.AccessToken != null)
                    {
                        var facebookUser = await GetFacebookProfileInfo(loginResult.AccessToken);

                        facebookLoginTcs.TrySetResult(facebookUser);
                    }
                    else
                    {
                        facebookLoginTcs.TrySetException(new Exception("Facebook Client Flow Login Failed"));
                    }
                },
                HandleCancel = () =>
                {
                    //facebookLoginTcs.TrySetException(new Exception("Facebook Client Flow Login Failed"));
                },
                HandleError = loginError =>
                {
                    facebookLoginTcs.TrySetException(new Exception("Facebook Client Flow Login Failed"));
                }
            };
            LoginManager.Instance.RegisterCallback(callbackManager, facebookCallback);
            LoginManager.Instance.LogInWithReadPermissions(CrossCurrentActivity.Current.Activity, new[] { "public_profile" });

            return await facebookLoginTcs.Task;
        }

        private static async Task<FacebookUser> GetFacebookProfileInfo(AccessToken token)
        {
            FacebookUser userProfile;
            var taskCompletionSource = new TaskCompletionSource<FacebookUser>();
            var parameters = new Bundle();
            parameters.PutString("fields", "name,email,picture.type(large)");

            var response = new FacebookGraphResponse
            {
                HandleSuccess = result =>
                {
                    userProfile = new FacebookUser
                    {
                        AccessToken = token.Token,
                        UserId = token.UserId,
                        UserName = result.JSONObject.GetString("name"),
                        Email = result.JSONObject.GetString("email"),
                        ProfileImageUrl = result.JSONObject.GetJSONObject("picture").GetJSONObject("data").GetString("url")
                    };

                    taskCompletionSource.SetResult(userProfile);
                }
            };

            //var graphRequest = new GraphRequest(AccessToken.CurrentAccessToken, "/" + AccessToken.CurrentAccessToken.UserId, parameters, HttpMethod.Get, response);
            var graphRequest = new GraphRequest(token, "/" + token.UserId, parameters, Xamarin.Facebook.HttpMethod.Get, response);
            graphRequest.ExecuteAsync();
            return await taskCompletionSource.Task;
        }

        private class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
        {
            public Action HandleCancel { private get; set; }
            public Action<FacebookException> HandleError { private get; set; }
            public Action<TResult> HandleSuccess { private get; set; }

            public void OnCancel()
            {
                var c = HandleCancel;
                c?.Invoke();
            }

            public void OnError(FacebookException error)
            {
                var c = HandleError;
                c?.Invoke(error);
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                var c = HandleSuccess;
                c?.Invoke(result.JavaCast<TResult>());
            }
        }

        private class FacebookGraphResponse : Java.Lang.Object, GraphRequest.ICallback
        {
            public Action<GraphResponse> HandleSuccess { private get; set; }

            public void OnCompleted(GraphResponse response)
            {
                var c = HandleSuccess;
                c?.Invoke(response);
            }
        }

        public void LogoutFacebook()
        {
            LoginManager.Instance.LogOut();
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

        public async Task<GoogleUser> LoginGoogleAsync()
        {
            var googleLoginTcs = new TaskCompletionSource<GoogleUser>();

            SharedGoogleApiClient.Instance.HandleConnectionResult = result =>
            {
                if (result.IsSuccess)
                {
                    var account = result.SignInAccount;

                    var googleUser = new GoogleUser
                    {
                        UserId = account.Id,
                        AccessToken = account.ServerAuthCode,
                        IdToken = account.IdToken,
                        Email = account.Email,
                        UserName = account.DisplayName,
                        ProfileImageUrl = account.PhotoUrl.Path
                    };

                    googleLoginTcs.TrySetResult(googleUser);
                }
                else
                {
                    googleLoginTcs.TrySetException(new Exception("Google Client Flow Login Failed"));
                }
            };

            var signInIntent = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInIntent(SharedGoogleApiClient.Instance.GoogleApiClient);
            CrossCurrentActivity.Current.Activity.StartActivityForResult(signInIntent, SharedGoogleApiClient.GoogleSignInCode);

            return await googleLoginTcs.Task;
        }

        public void LogoutGoogle()
        {
            SharedGoogleApiClient.Instance.GoogleApiClient.Disconnect();
        }

        #endregion

        #region Google Server Flow

        public async Task<GoogleUser> GetGoogleProfileAsync(string token)
        {
            GoogleUser googleUser;

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                const string url = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json";
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var json = await client.GetStringAsync(url);
                googleUser = JsonConvert.DeserializeObject<GoogleUser>(json);
            }

            googleUser.AccessToken = token;

            return googleUser;
        }

        #endregion
    }
}