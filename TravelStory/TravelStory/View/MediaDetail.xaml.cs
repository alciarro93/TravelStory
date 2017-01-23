using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Model;
using TravelStory.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TravelStory.View
{
    public partial class MediaDetail : ContentPage
    {
        MediaM obj = new MediaM();

        public MediaDetail()
        {
            InitializeComponent();
            BindingContext = new ViewModel.MediaDetailVM();

            DeleteMediaBtn.Text = Dictionary.ResourceManager.GetString("Delete", Dictionary.Culture)+" "+ Dictionary.ResourceManager.GetString("Media", Dictionary.Culture);
            SaveMediaBtn.Text = Dictionary.ResourceManager.GetString("Save", Dictionary.Culture) + " " + Dictionary.ResourceManager.GetString("Media", Dictionary.Culture);

            //prendo l'oggetto passato poi lo rimetto a null
            obj = (MediaM)App.SelectedObj;
            App.SelectedObj = null;

            //creo il pin sulla mappa
            Pin pin = null;
            pin = new Pin()
            {
                Position = new Position(obj.Latitude, obj.Longitude),
                Address = obj.CompleteAddress,
                Label = obj.Text
            };

            MapDetail.Pins.Add(pin);
            MapDetail.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromKilometers(0.3)));

            EditorText.TextChanged += EditorText_TextChanged;
        }

        private void EditorText_TextChanged(object sender, TextChangedEventArgs e)
        {
            string testo = String.Empty;
            if (e.OldTextValue != e.NewTextValue)
            {
                testo = e.NewTextValue;
                testo = testo.Trim();

                if (testo != String.Empty && obj.Path == String.Empty)
                {
                    obj.Type = MediaType.TEXT.ToString();
                }
                if (testo == String.Empty && obj.Path == String.Empty)
                {
                    obj.Type = MediaType.LOCATION.ToString();
                }

                obj.Synced = false;
                obj.ModType = ModType.UPDATED;
                obj.Text = testo;
                var connMedia = new MaintenanceDB<MediaM>();
                connMedia.SaveEdit(obj);

                var travel = ManageDB.mainConnection.GetTravel(obj.IdTravel);
                travel.Synced = false;
                var connTravel = new MaintenanceDB<TravelM>();
                connTravel.SaveEdit(travel);
            }
        }

    }
}
