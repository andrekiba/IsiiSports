using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Microsoft.Azure.Mobile;
using UIKit;

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

        private const string MobileCenterAppKey = "6487a36e-c568-4569-9800-a9167dbdb3d5";

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            #region Azure

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            #endregion

            #region Mobile Center

            MobileCenter.Configure(MobileCenterAppKey);

            #endregion

            global::Xamarin.Forms.Forms.Init();
            ImageCircleRenderer.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
