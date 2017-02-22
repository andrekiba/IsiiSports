﻿using System.IO;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Permissions;
using ImageCircle.Forms.Plugin.Droid;
using IsiiSports.Base;
using IsiiSports.Services;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Acr.UserDialogs;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Plus;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace IsiiSports.Droid
{
    [Activity(Label = "IsiiSports", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        //GoogleApiClient googleApiClient;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            #region Facebook Auth

            FacebookSdk.SdkInitialize(this);

            #endregion

            #region Google Auth

            //googleApiClient = new GoogleApiClient.Builder(this)
            //    .AddConnectionCallbacks(this)
            //    .AddOnConnectionFailedListener(this)
            //    .AddApi(PlusClass.API)
            //    .AddScope(new Scope(Scopes.Profile))
            //    .Build();
            
            #endregion

            #region Azure

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            //sqlite db
            AzureService.DbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), AzureService.DbPath);
            if (!File.Exists(AzureService.DbPath))
            {
                File.Create(AzureService.DbPath).Dispose();
            }

            #endregion

            #region Mobile Center

            MobileCenter.Start(Configuration.MobileCenterAppKey, typeof(Analytics), typeof(Crashes));

            #endregion 

            global::Xamarin.Forms.Forms.Init(this, bundle);
            ImageCircleRenderer.Init();
			UserDialogs.Init(this);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

