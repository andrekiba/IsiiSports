using System;
using Android.Gms.Common.Apis;

namespace IsiiSports.Droid.Base
{
    public sealed class SharedGoogleApiClient
    {
        public GoogleApiClient GoogleApiClient { get; set; }

        private static readonly Lazy<SharedGoogleApiClient>  lazy = new Lazy<SharedGoogleApiClient>(() => new SharedGoogleApiClient());

        public static SharedGoogleApiClient Instance = lazy.Value;

        private SharedGoogleApiClient()
        {
        }
    }
}