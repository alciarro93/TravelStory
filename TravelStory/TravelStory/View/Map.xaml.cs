using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Messages;
using TravelStory.Model;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TravelStory.View
{
    public partial class Map : ContentPage
    {
        public Map()
        {
            InitializeComponent();
            BindingContext = new ViewModel.MapVM();
            customMap.IsShowingUser = true;

            IEnumerable<MediaM> collectionMedia = null;
            if (App.CollectionMedia != null)
            {
                collectionMedia = App.CollectionMedia;                
            }
            App.CollectionMedia = null;

            if (collectionMedia == null)
            {
                if (ManageDB.mainConnection.GetSetting(ConstantStrings.LAST_LAT) != null && ManageDB.mainConnection.GetSetting(ConstantStrings.LAST_LAT) != null)
                {
                    var lat = double.Parse(ManageDB.mainConnection.GetSetting(ConstantStrings.LAST_LAT).Value);
                    var lon = double.Parse(ManageDB.mainConnection.GetSetting(ConstantStrings.LAST_LONG).Value);

                    customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(lat, lon), Distance.FromKilometers(1)));
                }
            }
            else
            {
                if (collectionMedia.Count() > 0)
                {
                    Pin pin = null;
                    foreach (var item in collectionMedia)
                    {
                        pin = new Pin()
                        {
                            Position = new Position(item.Latitude, item.Longitude),
                            Label = item.CompleteAddress,
                            Address = item.CompleteAddress,
                        };
                        customMap.Pins.Add(pin);
                        customMap.RouteCoordinates.Add(new Position(item.Latitude, item.Longitude));
                    }
                    customMap.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromKilometers(1)));                    
                }
            }           

        }
    }
}
