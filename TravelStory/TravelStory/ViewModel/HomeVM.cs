using Plugin.Connectivity;
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
using Xamarin.Forms;
using static TravelStory.Model.Interfaces;

namespace TravelStory.ViewModel
{
    public class HomeVM: INotifyPropertyChanged
    {
        //Testo fisso app
        public string PageTitle { get; private set; }
        public string HintTravelTitle { get; set; }
        public string StartTravel { get; set; }

        //testo scritto come titolo per il nuovo viaggio
        public string TravelTitle { get; set; }

        public ICommand StartTravelCmd { get; set; }

        protected MaintenanceDB<ProfileM> connProfile;
        protected MaintenanceDB<TravelM> connTravel;

        public AzureDataService<ProfileM> AzureProfile;
        public AzureDataService<TravelM> AzureTravel;


        public event PropertyChangedEventHandler PropertyChanged;

        public HomeVM()
        {
            PageTitle = Dictionary.ResourceManager.GetString("Home", Dictionary.Culture);
            HintTravelTitle = Dictionary.ResourceManager.GetString("HintTravelTitle", Dictionary.Culture);
            StartTravel = Dictionary.ResourceManager.GetString("StartTravel", Dictionary.Culture);

            StartTravelCmd = new Command(Start);

            connProfile = new MaintenanceDB<ProfileM>();
            connTravel = new MaintenanceDB<TravelM>();

            AzureProfile = new AzureDataService<ProfileM>();
            AzureTravel = new AzureDataService<TravelM>();
        }

        private void Start()
        {
            if (TravelTitle != null && TravelTitle != "")
            {
                var travel = new TravelM();
                travel.Title = TravelTitle;
                travel.Ended = false;
                travel.TravelEmail = App.Current_User.Email;
                travel.Synced = false;
                connTravel.Save(travel);

                App.Current_User.ActiveTravel = travel.IdTravel;
                App.Current_User.Synced = false;
                connProfile.SaveEdit(App.Current_User);               

                MessagingCenter.Send(new PageSelected(typeof(View.HomeTravel)), "");
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));

                //await GeneralFunc.UploadToAzure();
            }
        }

    }
}
