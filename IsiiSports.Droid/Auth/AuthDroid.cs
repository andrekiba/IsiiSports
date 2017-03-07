using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.Gms.Auth.Api.SignIn;
using Android.OS;
using Android.Runtime;
using IsiiSports.Auth;
using IsiiSports.Base;
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
        public async Task<AuthUser> LoginAsync(IMobileServiceClient client, string provider, IDictionary<string, string> parameters = null, bool clientFlow = false)
        {
            try
            {
                var authProvider = (MobileServiceAuthenticationProvider)Enum.Parse(typeof(MobileServiceAuthenticationProvider), Settings.AuthProvider);
                var authUser = new AuthUser();

                if (clientFlow)
                {
                    #region Client Flow

                    JObject jToken = null;

                    if (authProvider == MobileServiceAuthenticationProvider.Facebook)
                    {
                        var loginResult = await LoginFacebookAsync();
                        authUser.UserInfo = await GetFacebookProfileAsync(loginResult.AccessToken.Token);
                        authUser.AccessToken = loginResult.AccessToken.Token;
                        //authUser.RefreshToken = ...

                        jToken = JObject.FromObject(new
                        {
                            access_token = loginResult.AccessToken.Token,
                            //authorization_code = ...,
                            //id_token = ...,
                        });
                    }

                    if (authProvider == MobileServiceAuthenticationProvider.Google)
                    {
                        var account = await LoginGoogleAsync();
                        authUser.UserInfo = new UserInfo
                        {
                            UserId = account.Id,
                            Email = account.Email,
                            UserName = account.DisplayName,
                            ProfileImageUrl = account.PhotoUrl.Path
                        };
                        authUser.AccessToken = account.IdToken;
                        //authUser.RefreshToken = ...
                        
                        jToken = JObject.FromObject(new
                        {
                            access_token = account.IdToken,
                            authorization_code = account.ServerAuthCode,
                            id_token = account.IdToken
                        });
                    }

                    authUser.MobileServiceUser = await client.LoginAsync(authProvider, jToken);

                    #endregion
                }
                else
                {
                    #region Server Flow

                    authUser.MobileServiceUser = await client.LoginAsync(Forms.Context, authProvider, new Dictionary<string, string> { { "access_type", "offline" } });
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

        public async Task<LoginResult> LoginFacebookAsync()
        {
            var facebookLoginTcs = new TaskCompletionSource<LoginResult>();

            var callbackManager = CallbackManagerFactory.Create();
            var facebookCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult =>
                {
                    if (loginResult.AccessToken != null)
                    {
                        
                       
                        facebookLoginTcs.TrySetResult(loginResult);
                    }
                    else
                    {
                        var error = new StringBuilder();
                        error.AppendLine("Facebook Client Flow Login Failed");
                        facebookLoginTcs.TrySetException(new Exception(error.ToString()));
                    }
                },
                HandleCancel = () =>
                {
                    //facebookLoginTcs.TrySetException(new Exception("Facebook Client Flow Login Failed"));
                },
                HandleError = loginError =>
                {
                    var error = new StringBuilder();
                    error.AppendLine("Facebook Client Flow Login Failed");
                    error.AppendLine("Message: " + loginError.Message);
                    error.AppendLine("StackTrace: " + loginError.StackTrace);
                    facebookLoginTcs.TrySetException(new Exception(error.ToString()));
                }
            };
            LoginManager.Instance.RegisterCallback(callbackManager, facebookCallback);
            LoginManager.Instance.LogInWithReadPermissions(CrossCurrentActivity.Current.Activity, new[] { "public_profile" });

            return await facebookLoginTcs.Task;
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

        public void LogoutFacebook()
        {
            LoginManager.Instance.LogOut();
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

        public async Task<GoogleSignInAccount> LoginGoogleAsync()
        {
            var googleLoginTcs = new TaskCompletionSource<GoogleSignInAccount>();

            SharedGoogleApiClient.Instance.HandleConnectionResult = result =>
            {
                if (result.IsSuccess)
                {
                    var account = result.SignInAccount;
                    googleLoginTcs.TrySetResult(account);
                }
                else
                {
                    var error = new StringBuilder();
                    error.AppendLine("Google Client Flow Login Failed");
                    error.AppendLine("Status Code: " + result.Status.StatusCode.ToString());
                    error.AppendLine("Status Message: " + result.Status.StatusMessage);
                    googleLoginTcs.TrySetException(new Exception(error.ToString()));
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