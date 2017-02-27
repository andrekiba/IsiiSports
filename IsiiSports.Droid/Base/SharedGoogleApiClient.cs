using System;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;

namespace IsiiSports.Droid.Base
{
    public sealed class SharedGoogleApiClient
    {
        public GoogleApiClient GoogleApiClient { get; set; }
        public Action<GoogleSignInResult> HandleConnectionResult { private get; set; }

        public const int GoogleSignInCode = 66;

        private static readonly Lazy<SharedGoogleApiClient>  lazy = new Lazy<SharedGoogleApiClient>(() => new SharedGoogleApiClient());
        public static SharedGoogleApiClient Instance = lazy.Value;

        private SharedGoogleApiClient()
        {
        }

        public void OnConnectionResult(GoogleSignInResult result)
        {
            var c = HandleConnectionResult;
            c?.Invoke(result);
        }
    }
}