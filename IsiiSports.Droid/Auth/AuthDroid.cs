using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Plus;
using Android.Gms.Plus.Model.People;
using Android.OS;
using Android.Runtime;
using IsiiSports.Auth;
using IsiiSports.Droid.Auth;
using IsiiSports.Helpers;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.CurrentActivity;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Share.Model;
using Xamarin.Forms;

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
                    var facebookToken = await LoginFacebookAsync();
                    accessToken = facebookToken.Token;
                    authUser.FacebookUser = await GetFacebookProfileInfo(facebookToken);
                }

                if (provider == MobileServiceAuthenticationProvider.Google)
                {
                    var person = await LoginGoogleAsync();
                    //accessToken = person.
                    accessToken = string.Empty;

                    //authUser.GoogleUser = new IsiiSports.Auth.GoogleUser
                    //{
                    //    UserId = googleUser.UserID,
                    //    AccessToken = accessToken,
                    //    IdToken = googleUser.Authentication.IdToken,
                    //    Email = googleUser.Profile.Email,
                    //    UserName = googleUser.Profile.Name,
                    //    ProfileImageUrl = googleUser.Profile.HasImage ? googleUser.Profile.GetImageUrl(100).AbsoluteString : null
                    //};
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

        public async Task<AccessToken> LoginFacebookAsync()
        {
            var facebookLoginTcs = new TaskCompletionSource<AccessToken>();

            var callbackManager = CallbackManagerFactory.Create();
            var facebookCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult =>
                {
                    if (loginResult.AccessToken != null)
                    {
                        facebookLoginTcs.TrySetResult(loginResult.AccessToken);
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
            LoginManager.Instance.LogInWithReadPermissions(CrossCurrentActivity.Current.Activity,
                new[] {"public_profile"});

            return await facebookLoginTcs.Task;
        }

        public async Task<FacebookUser> GetFacebookProfileInfo(AccessToken token)
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

        public async Task<IPerson> LoginGoogleAsync()
        {
            var googleLoginTcs = new TaskCompletionSource<IPerson>();

            GoogleApiClient googleApiClient = null;

            var googleCallback = new GoogleCallback
            {
                HandleConnected = connected =>
                {
                    if (connected)
                    {
                        var person = PlusClass.PeopleApi.GetCurrentPerson(googleApiClient);
                        googleLoginTcs.TrySetResult(person);
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

            googleApiClient = new GoogleApiClient.Builder(Forms.Context)
                .AddConnectionCallbacks(googleCallback)
                .AddOnConnectionFailedListener(googleCallback)
                .AddApi(PlusClass.API)
                .AddScope(new Scope(Scopes.Profile))
                .Build();

            googleApiClient.Connect();

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