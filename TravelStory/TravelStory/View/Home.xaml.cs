using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Messages;
using Xamarin.Forms;

namespace TravelStory.View
{
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
            BindingContext = new ViewModel.HomeVM();

            this.StartTravelBtn.IsEnabled = false;
            this.TravelTitleEntry.TextChanged += TravelTitleEntry_TextChanged;
        }

        private void TravelTitleEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "")
            {
                this.StartTravelBtn.IsEnabled = true;
            }
            else
            {
                this.StartTravelBtn.IsEnabled = false;
            }
        }
    }
}
