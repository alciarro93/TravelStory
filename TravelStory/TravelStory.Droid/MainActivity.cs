using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using System.Net;
using Xamarin.Forms;
using static TravelStory.Model.Interfaces;
using Plugin.Toasts;

namespace TravelStory.Droid
{
    [Activity(MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ToolbarResource = Resource.Layout.toolbar;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            base.OnCreate(bundle);
            Forms.Init(this, bundle);

            DependencyService.Register<ToastNotificatorImplementation>(); 
            ToastNotificatorImplementation.Init(this); 
            Xamarin.FormsMaps.Init(this, bundle);
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            LoadApplication(new App());
        }
    }
}

