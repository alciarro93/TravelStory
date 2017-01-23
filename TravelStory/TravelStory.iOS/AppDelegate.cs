using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Plugin.Toasts;

namespace TravelStory.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
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
            var tint = UIColor.FromRGB(0,150,136);
            UINavigationBar.Appearance.BarTintColor = tint; //bar background
            UINavigationBar.Appearance.TintColor = tint; //Tint color of button items
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
            {
                Font = UIFont.FromName("Avenir-Medium", 17f),
                TextColor = UIColor.Black
            });

            UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes
            {
                Font = UIFont.FromName("Avenir-Medium", 17f),
                ForegroundColor = UIColor.Black
            };

            LoadApplication(new App());
            Forms.Init();

            DependencyService.Register<ToastNotificatorImplementation>(); // Register your dependency
            ToastNotificatorImplementation.Init(); //you can pass additional parameters here
                                                   
            Xamarin.FormsMaps.Init();
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            SQLitePCL.CurrentPlatform.Init();

            return base.FinishedLaunching(app, options);
        }
    }
}
