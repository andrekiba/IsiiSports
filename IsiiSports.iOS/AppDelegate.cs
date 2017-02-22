using System;
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

            #endregion

            #region Facebook Auth

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

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return SignIn.SharedInstance.HandleUrl(url, sourceApplication, annotation);
        }
    }
}
