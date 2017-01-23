using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TravelStory.Messages;
using TravelStory.Model;
using TravelStory.Resources;
using TravelStory.View;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.Connectivity;
using static TravelStory.Model.Interfaces;
using Plugin.Toasts;

namespace TravelStory.ViewModel
{
    public class LoginVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string PageTitle { get; set; }
        public string Login { get; set; }
        public string RegisterUser { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public bool EnableLoginBtn { get; set; }

        public ICommand LoginCmd { get; set; }

        private AzureDataService<ProfileM> AzureProfile;

        public LoginVM()
        {
            PageTitle = Dictionary.ResourceManager.GetString("Login", Dictionary.Culture);
            Login = Dictionary.ResourceManager.GetString("Login", Dictionary.Culture);
            RegisterUser = Dictionary.ResourceManager.GetString("RegisterUser", Dictionary.Culture);

            LoginCmd = new Command(LoginFunc);
            EnableLoginBtn = true;

            AzureProfile = new AzureDataService<ProfileM>();
        }

        private async void LoginFunc()
        {
            if (Username != null)
            {
                Username = Username.Trim();
            }
            if (Password != null)
            {
                Password = Password.Trim();
            }
            if (Username != "" && Password != "" && Username!= null && Password != null)
            {
                if (GeneralFunc.CheckInternet())
                {
                    EnableLoginBtn = false;
                    Login = Dictionary.ResourceManager.GetString("Logging", Dictionary.Culture);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableLoginBtn"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Login"));

                    var account = await AzureProfile.CheckLogin(Username, Password);
                    if (account.Count() > 0)
                    {
                        var acc = account.FirstOrDefault();
                       
                        var connProfile = new MaintenanceDB<ProfileM>();
                        var profile = ManageDB.mainConnection.GetProfile(acc.Email);
                        if (profile == null)
                        {
                            var itemP = new ProfileM(acc.IdProfile, acc.Email, acc.Password, acc.Name, acc.Surname, acc.ActiveTravel);
                            connProfile.SaveEdit(itemP);
                        }

                        profile = ManageDB.mainConnection.GetProfile(acc.Email);
                        if (profile.FirstLogin)
                        {
                            await GeneralFunc.DownloadFromAzure(acc.Email);
                            profile.FirstLogin = false;
                        }

                        App.isLoggedIn = true;

                        var connSettings = new MaintenanceDB<SettingsM>();
                        var itemS = new SettingsM(ConstantStrings.CURRENT_USERNAME, acc.Email);
                        connSettings.SaveEdit(itemS);

                        GeneralFunc.Sync_Current_User(acc.Email);

                        var title = Dictionary.ResourceManager.GetString("Success", Dictionary.Culture);
                        var mex = Dictionary.ResourceManager.GetString("Loggedin", Dictionary.Culture);
                        var notificator = DependencyService.Get<IToastNotificator>();
                        notificator.Notify(ToastNotificationType.Success,title, mex, TimeSpan.FromSeconds(3));

                        MessagingCenter.Send(new UserLogged(true), "");
                        //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UserLogged"));
                    }
                    else
                    {
                        EnableLoginBtn = true;
                        Login = Dictionary.ResourceManager.GetString("Login", Dictionary.Culture);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableLoginBtn"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Login"));

                        ErrorLogin();
                    }
                }
                else
                {
                    var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                    var mex = Dictionary.ResourceManager.GetString("NoInternet", Dictionary.Culture);

                    var notificator = DependencyService.Get<IToastNotificator>();
                    bool tapped = await notificator.Notify(ToastNotificationType.Warning,
                        title, mex, TimeSpan.FromSeconds(3));
                }
            }
            else
            {
                ErrorLogin();
            }                   
        }

        private async void ErrorLogin()
        {
            var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("AlertErrMexLogin", Dictionary.Culture);

            var notificator = DependencyService.Get<IToastNotificator>();
            bool tapped = await notificator.Notify(ToastNotificationType.Error,
                title, mex, TimeSpan.FromSeconds(3));
        }

    }
}
