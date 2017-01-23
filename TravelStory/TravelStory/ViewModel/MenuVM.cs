using Plugin.Toasts;
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
using static TravelStory.Model.Interfaces;

namespace TravelStory.ViewModel
{
    public class MenuVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DetailPageM[] MenuItems { get; private set; }

        public string CompleteName { get; set; }
        public string Username { get; set; }
        public string SyncText { get; set; }
        public string LogoutText { get; set; }
        public string EditProfile { get; set; }
        public string RegisterUser { get; set; }

        public ICommand ChangeUserCmd { get; set; }
        public ICommand SyncCmd { get; set; }
        public ICommand LogoutCmd { get; set; }
        public ICommand EditProfileCmd { get; set; }
        public ICommand RegisterCmd { get; set; }

        public ImageSource ExpandSource { get; set; }
        public ImageSource CollapseSource { get; set; }       

        public bool ExpandVisible { get; set; }
        public bool CollapseVisible { get; set; }
        public bool ListViewPages { get; set; }
        public bool ListViewAccount { get; set; }

        public MenuVM()
        {
            MenuItems = new DetailPageM[]
            {
                new DetailPageM(typeof(Home)),
                new DetailPageM(typeof(Map)),
                new DetailPageM(typeof(Travels)),
                new DetailPageM(typeof(Settings)),
            };

            ListViewPages = true;
            ListViewAccount = false;

            if (App.isLoggedIn)
            {
                CompleteName = App.Current_User.Name + " " + App.Current_User.Surname;
                Username = App.Current_User.Email;                
            }

            SyncText = Dictionary.ResourceManager.GetString("SyncTravls", Dictionary.Culture);
            SyncCmd = new Command(SyncFunc);

            ChangeUserCmd = new Command(ChangeUserFunc);
            ExpandSource = ImageSource.FromResource("TravelStory.Resources.Icons.Expand.png");
            ExpandVisible = true;
            CollapseSource = ImageSource.FromResource("TravelStory.Resources.Icons.Collapse.png");
            CollapseVisible = false;

            LogoutText = Dictionary.ResourceManager.GetString("Logout", Dictionary.Culture);
            LogoutCmd = new Command(LogoutFunc);

            EditProfile = Dictionary.ResourceManager.GetString("EditProfile", Dictionary.Culture);
            EditProfileCmd = new Command(EditProfileFunc);

            RegisterUser = Dictionary.ResourceManager.GetString("RegisterUser", Dictionary.Culture);
            RegisterCmd = new Command(RegisterFunc);
        }

        private async void ChangeUserFunc()
        {
            await Task.Delay(50);

            ExpandVisible = !ExpandVisible;
            CollapseVisible = !CollapseVisible;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpandVisible"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CollapseVisible"));

            ListViewPages = !ListViewPages;
            ListViewAccount = !ListViewAccount;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ListViewPages"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ListViewAccount"));
        }

        private async void LogoutFunc()
        {           
            var title = Dictionary.ResourceManager.GetString("TitleConfMex", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("MexLogout", Dictionary.Culture);
            var yes = Dictionary.ResourceManager.GetString("Yes", Dictionary.Culture);
            var no = Dictionary.ResourceManager.GetString("No", Dictionary.Culture);

            var answer = await App.CurrentPage.DisplayAlert(title, mex, yes, no);
            if (answer)
            {
                var connSett = new MaintenanceDB<SettingsM>();
                var itemSett = ManageDB.mainConnection.GetSetting(ConstantStrings.CURRENT_USERNAME);
                connSett.Delete(itemSett);

                MessagingCenter.Send(new UserLoggedOut(true), "");

                App.Current_User = null;
                App.isLoggedIn = false;
            }     
        }

        private void RegisterFunc()
        {
            App.RegisterNewUser = true;
            MessagingCenter.Send(new PageSelected(typeof(RegisterUser), true, false), "");
        }

        private void EditProfileFunc()
        {
            App.SelectedObj = App.Current_User;
            MessagingCenter.Send(new PageSelected(typeof(RegisterUser), true, false), "");
        }

        private async void SyncFunc()
        {
            var title = Dictionary.ResourceManager.GetString("Info", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("SyncOnGoing", Dictionary.Culture);
            var notificator = DependencyService.Get<IToastNotificator>();
            notificator.Notify(ToastNotificationType.Info, title, mex, TimeSpan.FromSeconds(1));

            await GeneralFunc.UploadToAzure(true);

            MessagingCenter.Send(new CloseMaster(true), "");

            title = Dictionary.ResourceManager.GetString("Success", Dictionary.Culture);
            mex = Dictionary.ResourceManager.GetString("SyncOK", Dictionary.Culture);
            notificator = DependencyService.Get<IToastNotificator>();
            await notificator.Notify(ToastNotificationType.Success, title, mex, TimeSpan.FromSeconds(1));        
        }

    }
}
