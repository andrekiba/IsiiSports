using System;
using Facebook.CoreKit;
using Foundation;
using Google.Core;
using Google.SignIn;
using ImageCircle.Forms.Plugin.iOS;
using Microsoft.Azure.Mobile;
using UIKit;
using Configuration = IsiiSports.Base.Configuration;

namespace IsiiSports.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            #region Google Auth

            const string clientId = "710855082545-4cu6h49jn1679msd5tvijt607rbqjjfn.apps.googleusercontent.com";

            NSError configureError;
            Context.SharedInstance.Configure(out configureError);
            if (configureError != null)
            {
                // If something went wrong, assign the clientID manually
                Console.WriteLine("Error configuring the Google context: {0}", configureError);
                SignIn.SharedInstance.ClientID = clientId;
            }

			// define useragent android like
			string userAgent = "Mozilla/5.0 (Linux; Android 5.1.1; Nexus 5 Build/LMY48B; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/43.0.2357.65 Mobile Safari/537.36";
			var dictionary = NSDictionary.FromObjectAndKey(FromObject(userAgent), FromObject("UserAgent"));
			NSUserDefaults.StandardUserDefaults.RegisterDefaults(dictionary);

            #endregion

            #region Facebook Auth

			Profile.EnableUpdatesOnAccessTokenChange(true);
            Facebook.CoreKit.Settings.AppID = "157854894725705"; //Facebook AppId
            Facebook.CoreKit.Settings.DisplayName = "Isii Sports";

            #endregion

            #region Azure

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            #endregion

            #region Mobile Center

            MobileCenter.Configure(Configuration.MobileCenterAppKey);

            #endregion

            global::Xamarin.Forms.Forms.Init();
            ImageCircleRenderer.Init();

            LoadApplication(new App());

            //Facebook
            ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            //Facebook
            if (Helpers.Settings.AuthProvider == "Facebook")            
                return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
            
            //Google
            return Helpers.Settings.AuthProvider == "Google" && SignIn.SharedInstance.HandleUrl(url, sourceApplication, annotation);
        }
    }
}
