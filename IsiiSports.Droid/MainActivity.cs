using System.IO;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Permissions;
using ImageCircle.Forms.Plugin.Droid;
using IsiiSports.Services;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

namespace IsiiSports.Droid
{
    [Activity(Label = "IsiiSports", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const string MobileCenterAppKey = "6487a36e-c568-4569-9800-a9167dbdb3d5";

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

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

            MobileCenter.Start(MobileCenterAppKey, typeof(Analytics), typeof(Crashes));

            #endregion 

            global::Xamarin.Forms.Forms.Init(this, bundle);
            ImageCircleRenderer.Init();
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

