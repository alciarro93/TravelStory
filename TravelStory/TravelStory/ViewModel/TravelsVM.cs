using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TravelStory.Model;
using TravelStory.Resources;
using Xamarin.Forms;

namespace TravelStory.ViewModel
{
    public class TravelsVM: INotifyPropertyChanged
    {
        public string PageTitle { get; private set; }
        public string SearchTitle { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string PropertyDateFormat { get; set; }

        public bool ShowFilters { get; set; }

        public ICommand ShowFiltersCmd { get; set; }

        //public IList<TravelM> EndedTravels { get; set; }

        public TravelsVM()
        {
            PageTitle = Dictionary.ResourceManager.GetString("Travels", Dictionary.Culture);
            SearchTitle = Dictionary.ResourceManager.GetString("SearchTitle", Dictionary.Culture);
            DateFrom = Dictionary.ResourceManager.GetString("DateFrom", Dictionary.Culture);
            DateTo = Dictionary.ResourceManager.GetString("DateTo", Dictionary.Culture);
            PropertyDateFormat = ManageDB.mainConnection.GetSetting(ConstantStrings.DATEFORMAT).Value;

            ShowFilters = false;
            ShowFiltersCmd = new Command(ShowFiltersFunc);

            //EndedTravels = ManageDB.mainConnection.GetEndedTravels();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ShowFiltersFunc()
        {
            ShowFilters = !ShowFilters;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowFilters"));
        }
    }
}
