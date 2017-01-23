using SQLite.Net;
using SQLite.Net.Async;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TravelStory.Model;
using TravelStory.Resources;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using static TravelStory.Model.Interfaces;
using TravelStory.Messages;
using TravelStory.View;

namespace TravelStory
{
    public class App : Application
    {
        public static Page CurrentPage = new Page();
        public static object SelectedObj = null;
        public static IEnumerable<MediaM> CollectionMedia = null;

        public static ProfileM Current_User = null;
        public static bool isLoggedIn = false;
        public static bool RegisterNewUser = false;

        public App()
        {
            ManageDB.InizializeApp();
            if (!isLoggedIn)
            {
                MainPage = new NavigationPage(new Login());
            }
            else
            {
                MainPage = new Main();
            }

            MessagingCenter.Subscribe<UserLogged>(this, "", (sender) =>
            {
                if (sender.Logged)
                {
                    MainPage = new Main();
                }
            });

            MessagingCenter.Subscribe<UserLoggedOut>(this, "", (sender) =>
            {
                if (sender.LoggedOut)
                {
                    MainPage = new NavigationPage(new Login());
                }
            });

        }

       
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            if (GeneralFunc.tokenSource != null)
            {
                GeneralFunc.tokenSource.Cancel(); //smetto ci trovare la posizione
            }
        }

        protected override void OnResume()
        {

        }
    }
}
