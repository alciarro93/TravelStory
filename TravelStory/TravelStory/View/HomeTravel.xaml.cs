using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Messages;
using TravelStory.Model;
using TravelStory.Resources;
using Xamarin.Forms;

namespace TravelStory.View
{
    public partial class HomeTravel : ContentPage
    {
        public HomeTravel()
        {
            InitializeComponent();
            BindingContext = new ViewModel.HomeTravelVM();

            MediaCurrTravelLV.ItemSelected += MediaCurrTravel_ItemSelected;

            RouteBtn.Text = Dictionary.ResourceManager.GetString("ShowPins", Dictionary.Culture);
            DecodeAddBtn.Text = Dictionary.ResourceManager.GetString("DecodeAdd", Dictionary.Culture);
            DeleteBtn.Text = Dictionary.ResourceManager.GetString("Delete", Dictionary.Culture)
                                     + " " +
                                     Dictionary.ResourceManager.GetString("Travel", Dictionary.Culture);
            EditTitleBtn.Text = Dictionary.ResourceManager.GetString("EditTitle", Dictionary.Culture);

            EndBtn.Text = Dictionary.ResourceManager.GetString("EndTravel", Dictionary.Culture);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MediaCurrTravelLV.RefreshCommand.Execute(null);
        }

        private void MediaCurrTravel_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                App.SelectedObj = (MediaM)e.SelectedItem;
                MessagingCenter.Send(new PageSelected(typeof(MediaDetail),true,false), "");
                MediaCurrTravelLV.SelectedItem = null;
            }
        }
    }
}
