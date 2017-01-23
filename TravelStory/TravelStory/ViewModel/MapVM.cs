using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Messages;
using TravelStory.Model;
using TravelStory.Resources;
using Xamarin.Forms;
using static TravelStory.Model.Interfaces;

namespace TravelStory.ViewModel
{
    public class MapVM
    {
        public string PageTitle { get; set; }

        public MapVM()
        {
            PageTitle = Dictionary.ResourceManager.GetString("Map", Dictionary.Culture);
        }

    }
}
