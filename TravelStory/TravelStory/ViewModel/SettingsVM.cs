using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TravelStory.Messages;
using TravelStory.Model;
using TravelStory.Resources;
using TravelStory.View;
using Xamarin.Forms;

namespace TravelStory.ViewModel
{
    public class SettingsVM: INotifyPropertyChanged
    {

        public string PageTitle { get; set; }
        public string Languages { get; set; }
        public string SyncText { get; set; }
        public string GeneralSettings { get; set; }
        public string DateFormatPickerTitle { get; set; }

        public bool IsSyncTogg { get; set; }

        public ICommand ChangeLanguageITCmd { get; set; }
        public ICommand ChangeLanguageENCmd { get; set; }

        public ImageSource FlagITico { get; set; }
        public ImageSource FlagENico { get; set; }

        public MaintenanceDB<SettingsM> connSett;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsVM()
        {
            PageTitle = Dictionary.ResourceManager.GetString("Settings", Dictionary.Culture);
            GeneralSettings = Dictionary.ResourceManager.GetString("GeneralSettings", Dictionary.Culture);
            Languages = Dictionary.ResourceManager.GetString("Languages", Dictionary.Culture);
            SyncText = Dictionary.ResourceManager.GetString("SyncText", Dictionary.Culture);
            DateFormatPickerTitle = Dictionary.ResourceManager.GetString("DateFormatPickerTitle", Dictionary.Culture)+ ": ";

            if (bool.Parse(ManageDB.mainConnection.GetSetting(ConstantStrings.REMOTE_SYNC).Value))
            {
                IsSyncTogg = true;
            }
            else
            {
                IsSyncTogg = false;
            }

            FlagITico = ImageSource.FromResource("TravelStory.Resources.Icons.it.png");
            FlagENico = ImageSource.FromResource("TravelStory.Resources.Icons.en-GB.png");

            ChangeLanguageITCmd = new Command(ChangeLanguageIT);
            ChangeLanguageENCmd = new Command(ChangeLanguageEN);

            connSett = new MaintenanceDB<SettingsM>();
        }

        private void ChangeLanguageEN()
        {
            ChangeLanguageFunc("en-GB");
        }

        private void ChangeLanguageIT()
        {
            ChangeLanguageFunc("it");
        }

        private void ChangeLanguageFunc(string lang)
        {
            Dictionary.Culture = new CultureInfo(lang);
            var item = new SettingsM(ConstantStrings.LANG_APP, lang);
            connSett.SaveEdit(item);

            MessagingCenter.Send(new RefreshMaster(true), "");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RefreshMaster"));

            MessagingCenter.Send(new PageSelected(typeof(Settings)), "");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));
        }

    }
}
