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
    public partial class Travels : ContentPage
    {
        protected bool ErrorPickDateBegin = false;
        protected bool ErrorPickDateEnd = false;

        public Travels()
        {
            InitializeComponent();
            BindingContext = new ViewModel.TravelsVM();

            ShowFiltersBtn.Text = Dictionary.ResourceManager.GetString("ShowFilters", Dictionary.Culture);

            TravelsList.ItemSelected += TravelsList_ItemSelected;

            DPBegin.DateSelected += DPBegin_DateSelected;
            DPEnd.DateSelected += DPEnd_DateSelected;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var ListEndedTravels = ManageDB.mainConnection.GetEndedTravels();
            if (ListEndedTravels.Count>0)
            {
                TravelsList.ItemsSource = ListEndedTravels;

                var beginDate = ListEndedTravels.LastOrDefault().StartDate;
                var endDate = ListEndedTravels.FirstOrDefault().EndDate;

                if (beginDate != null)
                {
                    DPBegin.Date = beginDate;
                }
                else
                {
                    DPBegin.Date = DateTime.Now;
                }

                if (endDate != null)
                {
                    DPEnd.Date = endDate;
                }
                else
                {
                    DPEnd.Date = DateTime.Now;
                }
            }         
        }

        #region RANGE DATES
        private void DPEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (e.NewDate < DPBegin.Date)
            {
                DPEnd.Date = e.OldDate;
                ErrorPickDateEnd = true;
            }
            else
            {
                if (!ErrorPickDateEnd)
                {
                    TravelsList.ItemsSource = ManageDB.mainConnection.GetEndedTravels().Where(a => a.EndDate.Date <= e.NewDate);
                    ErrorPickDateEnd = false;
                }                
            }          
        }

        private void DPBegin_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (e.NewDate>DPEnd.Date)
            {
                DPBegin.Date = e.OldDate;
                ErrorPickDateBegin = true;
            }
            else
            {
                if (!ErrorPickDateBegin)
                {
                    TravelsList.ItemsSource = ManageDB.mainConnection.GetEndedTravels().Where(a => a.StartDate.Date >= e.NewDate);
                    ErrorPickDateBegin = false;
                }                
            }
            
        }
        #endregion

        #region SEARCHBAR
        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TravelsList.BeginRefresh();

            if (!string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                TravelsList.ItemsSource = ManageDB.mainConnection.GetEndedTravels().Where(a => a.Title.Contains(e.NewTextValue));
            }
            else
            {
                TravelsList.ItemsSource = ManageDB.mainConnection.GetEndedTravels();
            }

            TravelsList.EndRefresh();
        }

        private void OnSearch(object sender, EventArgs e)
        {
            TravelsList.BeginRefresh();

            if (!string.IsNullOrWhiteSpace(SearchBarEndedTravel.Text))
            {
                TravelsList.ItemsSource = ManageDB.mainConnection.GetEndedTravels().Where(a => a.Title.Contains(SearchBarEndedTravel.Text));
            }
            else
            {
                TravelsList.ItemsSource = ManageDB.mainConnection.GetEndedTravels();
            }

            TravelsList.EndRefresh();
        }
        #endregion


        //quando si seleziona un viaggio

        private void TravelsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem!=null)
            {
                App.SelectedObj = (TravelM)e.SelectedItem;
                MessagingCenter.Send<PageSelected>(new PageSelected(typeof(HomeTravel), true, false), "");
                TravelsList.SelectedItem = null;
            }
        }
    }
}
