using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TravelStory.Droid.Servicies;
using static TravelStory.Model.Interfaces;
using Java.IO;
using Android.Locations;
using Xamarin.Forms;
using Plugin.CurrentActivity;
using TravelStory.Resources;
using Android.Views.InputMethods;

[assembly: Dependency(typeof(Settings_Droid))]
namespace TravelStory.Droid.Servicies
{
    public class Settings_Droid : ILocalSettings
    {
        public string GetMediaSavePath()
        {
            File pictureDirectory;

            pictureDirectory = new File("Travel_Blogging");

            return pictureDirectory.ToString();
        }

        public void GPS_Setting()
        {
            var lm = (LocationManager)Forms.Context.GetSystemService(Context.LocationService);
            if (lm.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                var alert = new AlertDialog.Builder(CrossCurrentActivity.Current.Activity);
                alert.SetTitle(Dictionary.ResourceManager.GetString("TitleGPSMex", Dictionary.Culture));
                string text = Dictionary.ResourceManager.GetString("GPSMex", Dictionary.Culture);
                text = text.Replace("@", "" + System.Environment.NewLine);
                alert.SetMessage(text);
                alert.SetPositiveButton(Dictionary.ResourceManager.GetString("Yes", Dictionary.Culture), (senderAlert, args) => {
                    Intent gpsSettingIntent = new Intent(Settings.ActionLocationSourceSettings);
                    Forms.Context.StartActivity(gpsSettingIntent);
                });

                alert.SetNegativeButton(Dictionary.ResourceManager.GetString("No", Dictionary.Culture), (senderAlert, args) => {
                });
                var dialog = alert.Create();
                dialog.Show();
            }
        }

        public void HideKeyboard()
        {
            var inputMethodManager = Forms.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && Forms.Context is Activity)
            {
                var activity = Forms.Context as Activity;
                var token = activity.CurrentFocus == null ? null : activity.CurrentFocus.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, 0);
            }
        }

    }
}