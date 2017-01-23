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
    public partial class Menu : ContentPage
    {
        public Menu()
        {
            InitializeComponent();
            BindingContext = new ViewModel.MenuVM();

            MenuPageLV.ItemSelected += MenuPageLV_ItemSelected;
        }

        private void MenuPageLV_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem!=null)
            {
                var pagina = e.SelectedItem as DetailPageM;
                MessagingCenter.Send(new PageSelected(pagina.PageType, false, false), "");
                MenuPageLV.SelectedItem = null;
            }
            
        }
    }
}
