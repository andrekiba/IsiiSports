using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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
using Plugin.CurrentActivity;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Forms;
using HttpMethod = Xamarin.Facebook.HttpMethod;

[assembly: Dependency(typeof(AuthDroid))]
namespace IsiiSports.Droid.Auth
{
    public class AuthDroid : IAuthentication
    {
        public async Task<AuthUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null)
        {
            try
            {
                var authUser = new AuthUser();
                string accessToken = null;

                if (provider == MobileServiceAuthenticationProvider.Facebook)
                {
                    var facebookUser = await LoginFacebookAsync();
                    accessToken = facebookUser.AccessToken;
                    authUser.FacebookUser = facebookUser;
                }

                if (provider == MobileServiceAuthenticationProvider.Google)
                {
                    var googleUser = await LoginGoogleAsync();
                    accessToken = googleUser.AccessToken;
                    authUser.GoogleUser = googleUser;
                }

                var zumoPayload = new Dictionary<string, string> {{"access_token", accessToken}};

                authUser.MobileServiceUser = await client.LoginAsync(Forms.Context, provider, zumoPayload);

                return authUser;

            }
            catch (Exception)
            {
                //Insights.Report(ex, Insights.Severity.Error);
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
                if ((int) Build.VERSION.SdkInt >= 21)
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
            LoginManager.Instance.LogInWithReadPermissions(CrossCurrentActivity.Current.Activity, new[] {"public_profile"});

            return await facebookLoginTcs.Task;
        }

        private static async Task<FacebookUser> GetFacebookProfileInfo(AccessToken token)
        {
            FacebookUser userProfile;
            var taskCompletionSource = new TaskCompletionSource<FacebookUser>();
            var parameters = new Bundle();
            parameters.PutString("fields", "name,email,picture.type(large)");
            //var webClient = new WebClient();
                   
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

                    //var pictureUrl = result.JSONObject.GetJSONObject("picture").GetJSONObject("data").GetString("url");
                    //var pictureData = webClient.DownloadData(pictureUrl);
                    //userProfile.ProfilePicture = pictureData;

                    taskCompletionSource.SetResult(userProfile);
                }
            };

            //var graphRequest = new GraphRequest(AccessToken.CurrentAccessToken, "/" + AccessToken.CurrentAccessToken.UserId, parameters, HttpMethod.Get, response);
            var graphRequest = new GraphRequest(token, "/" + token.UserId, parameters, HttpMethod.Get, response);
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

        #endregion

        #region Google Client Flow

        public async Task<GoogleUser> LoginGoogleAsync()
        {
            var googleLoginTcs = new TaskCompletionSource<GoogleUser>();

            var googleCallback = new GoogleCallback
            {
                HandleConnected = async connected =>
                {
                    if (connected)
                    {
                        const string scopes = "oauth2:https://www.googleapis.com/auth/userinfo.profile";
                        var context = SharedGoogleApiClient.Instance.GoogleApiClient.Context;
                        var accountName = PlusClass.AccountApi.GetAccountName(SharedGoogleApiClient.Instance.GoogleApiClient);
                        var token = GoogleAuthUtil.GetToken(context, accountName, scopes);
                        var userId = GoogleAuthUtil.GetAccountId(context, accountName);
                        var person = PlusClass.PeopleApi.GetCurrentPerson(SharedGoogleApiClient.Instance.GoogleApiClient);

                        var googleUser = new GoogleUser
                        {
                            UserId = userId,
                            AccessToken = token,
                            IdToken = string.Empty,
                            Email = string.Empty,
                            UserName = person.DisplayName,
                            ProfileImageUrl = person.HasImage ? person.Image.Url : null
                        };

                        #region Test

                        using (var client = new HttpClient(new NativeMessageHandler()))
                        {
                            const string url = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json";
                            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                            var json = await client.GetStringAsync(url);
                            googleUser = JsonConvert.DeserializeObject<GoogleUser>(json);
                        }

                        googleUser.AccessToken = token;

                        #endregion

                        googleLoginTcs.TrySetResult(googleUser);
                    }
                    else
                    {
                        googleLoginTcs.TrySetException(new Exception("Google Client Flow Login Failed"));
                    }
                },
                HandleConnectionSuspended = cause =>
                {
                    //googleLoginTcs.TrySetException(new Exception("Google Client Flow Login Failed"));
                },
                HandleConnectionFailed = connectionResult =>
                {
                    googleLoginTcs.TrySetException(new Exception("Google Client Flow Login Failed"));
                }
            };

            SharedGoogleApiClient.Instance.GoogleApiClient.RegisterConnectionCallbacks(googleCallback);
            SharedGoogleApiClient.Instance.GoogleApiClient.RegisterConnectionFailedListener(googleCallback);
            SharedGoogleApiClient.Instance.GoogleApiClient.Connect();

            return await googleLoginTcs.Task;
        }

        private class GoogleCallback : Java.Lang.Object, 
                                       GoogleApiClient.IConnectionCallbacks,
                                       GoogleApiClient.IOnConnectionFailedListener
        {

            public Action<bool> HandleConnected { private get; set; }
            public Action<int> HandleConnectionSuspended { private get; set; }
            public Action<ConnectionResult> HandleConnectionFailed { private get; set; }

            public void OnConnected(Bundle connectionHint)
            {
                var c = HandleConnected;
                c?.Invoke(true);
            }

            public void OnConnectionSuspended(int cause)
            {
                var c = HandleConnectionSuspended;
                c?.Invoke(cause);
            }

            public void OnConnectionFailed(ConnectionResult result)
            {
                var c = HandleConnectionFailed;
                c?.Invoke(result);
            }
        }

        #endregion
    }
}