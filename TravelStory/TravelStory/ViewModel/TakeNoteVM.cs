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

namespace TravelStory.ViewModel
{
    public class TakeNoteVM: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string PageTitle { get; set; }
        public string Description { get; set; }
        public ICommand SaveNoteCmd { get; set; }

        public bool ShowSave { get; set; }

        public string CurrentTravelID;
        private MaintenanceDB<MediaM> connMedia;

        public TakeNoteVM()
        {
            PageTitle = Dictionary.ResourceManager.GetString("TakeNotePage", Dictionary.Culture);
            CurrentTravelID = App.Current_User.ActiveTravel;

            connMedia = new MaintenanceDB<MediaM>();
            SaveNoteCmd = new Command(SaveNoteFunc);
            ShowSave = true;
        }


        private async void SaveNoteFunc()
        {          
            if (!String.IsNullOrEmpty(Description))
            {
                ShowSave = false;

                if (GeneralFunc.Position != null)
                {
                    var media = new MediaM();
                    media.IdTravel = CurrentTravelID;
                    media.Type = MediaType.TEXT.ToString();
                    media.Text = Description;
                    media.Latitude = GeneralFunc.Position.Latitude;
                    media.Longitude = GeneralFunc.Position.Longitude;
                    media.CompleteAddress = await GeneralFunc.GetCurrentAddress(media.Latitude, media.Longitude);
                    media.TimeStamp = DateTime.Now;

                    connMedia.Save(media);

                    var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                    travel.ModType = ModType.UPDATED;
                    travel.Synced = false;
                    var connTravel = new MaintenanceDB<TravelM>();
                    connTravel.SaveEdit(travel);

                    MessagingCenter.Send(new PageSelected(typeof(HomeTravel)), "");

                    //await GeneralFunc.UploadToAzure();
                }               
            }
            else
            {
                var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                var mex = Dictionary.ResourceManager.GetString("AlertErrMex", Dictionary.Culture);
                var notificator = DependencyService.Get<IToastNotificator>();
                await notificator.Notify(ToastNotificationType.Warning, title, mex, TimeSpan.FromSeconds(3), null, true);
            }
        }

    }
}
