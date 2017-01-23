using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Model;
using Xamarin.Forms;

namespace TravelStory.View
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            BindingContext = new ViewModel.SettingsVM();
            //SyncSwitch.Toggled += SyncSwitch_Toggled;

            foreach (var item in ConstantStrings.DateFormats)
            {
                DateFormatPicker.Items.Add(item);
            }

            var dateformat = ManageDB.mainConnection.GetSetting(ConstantStrings.DATEFORMAT).Value;
            if (dateformat == ConstantStrings.DateFormat1)
            {
                DateFormatPicker.SelectedIndex = 0;
            }
            if (dateformat == ConstantStrings.DateFormat2)
            {
                DateFormatPicker.SelectedIndex = 1;
            }
            if (dateformat == ConstantStrings.DateFormat3)
            {
                DateFormatPicker.SelectedIndex = 2;
            }

            DateFormatPicker.SelectedIndexChanged += DateFormatPicker_SelectedIndexChanged;
        }

        private void DateFormatPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DateFormatPicker.SelectedIndex == -1)
            {
                
            }
            else
            {
                var selectedFormat = DateFormatPicker.Items[DateFormatPicker.SelectedIndex];
                var stringFormat = ConstantStrings.DateFormat1;
                switch (selectedFormat)
                {
                    case ConstantStrings.DateFormat1Ex:
                        stringFormat = ConstantStrings.DateFormat1;
                        break;
                    case ConstantStrings.DateFormat2Ex:
                        stringFormat = ConstantStrings.DateFormat2;
                        break;
                    case ConstantStrings.DateFormat3Ex:
                        stringFormat = ConstantStrings.DateFormat3;
                        break;
                }
                var item = new SettingsM(ConstantStrings.DATEFORMAT, stringFormat);
                var connSett = new MaintenanceDB<SettingsM>();
                connSett.SaveEdit(item);
            }
        }

        //private void SyncSwitch_Toggled(object sender, ToggledEventArgs e)
        //{
        //    if (SyncSwitch.IsToggled)
        //    {
        //        var item = new SettingsM(ConstantStrings.REMOTE_SYNC, "true");
        //        var connSett = new MaintenanceDB<SettingsM>();
        //        connSett.SaveEdit(item);
        //    }
        //    else
        //    {
        //        var item = new SettingsM(ConstantStrings.REMOTE_SYNC, "false");
        //        var connSett = new MaintenanceDB<SettingsM>();
        //        connSett.SaveEdit(item);
        //    }
        //}
    }
}
