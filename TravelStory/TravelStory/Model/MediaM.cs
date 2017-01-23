using Plugin.Connectivity;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Resources;
using Xamarin.Forms;

namespace TravelStory.Model
{
    public class MediaM
    {
        [Newtonsoft.Json.JsonProperty("Id")]
        [PrimaryKey]
        public string IdMedia { get; set; }

        [Newtonsoft.Json.JsonProperty("Deleted")]
        public bool Deleted { get; set; }

        [NotNull]
        public string IdTravel { get; set; }

        [NotNull]
        public string Type { get; set; }

        //MEDIA
        public string Name { get; set; }
        public string Path { get; set; }
        public string Text { get; set; }

        //GPS
        [NotNull]
        public double Latitude { get; set; }
        [NotNull]
        public double Longitude { get; set; }
        public string CompleteAddress { get; set; }

        //DATE and TIME
        public DateTime TimeStamp { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string TimeStampString
        {
            get
            {
                var dateFormat = ManageDB.mainConnection.GetSetting(ConstantStrings.DATEFORMAT).Value;
                var item = TimeStamp.ToLocalTime().ToString(dateFormat, Dictionary.Culture);
                item = item + " " +Dictionary.ResourceManager.GetString("At", Dictionary.Culture)+" ";
                item = item + TimeStamp.ToLocalTime().ToString(ConstantStrings.TimeFormt, Dictionary.Culture);
                return item;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public ImageSource ImageList
        {
            get
            {
                if (Type == MediaType.PHOTO.ToString())
                {
                    return ImageSource.FromResource("TravelStory.Resources.Icons.PhotoListView.png");
                }
                else
                {
                    if (Type == MediaType.VIDEO.ToString())
                    {
                        return ImageSource.FromResource("TravelStory.Resources.Icons.VideoListView.png");
                    }
                    else
                    {
                        if (Type == MediaType.TEXT.ToString())
                        {
                            return ImageSource.FromResource("TravelStory.Resources.Icons.TextListView.png"); 
                        }
                        else
                        {
                            return ImageSource.FromResource("TravelStory.Resources.Icons.LocationListView.png");
                        }                        
                    }
                    
                }
                
            }
        }

        [NotNull]
        [Newtonsoft.Json.JsonIgnore]
        public bool Synced { get; set; }

        [NotNull]
        [Newtonsoft.Json.JsonIgnore]
        public bool PresenteOnline { get; set; }

        [NotNull]
        [Newtonsoft.Json.JsonIgnore]
        public ModType ModType { get; set; }

        public MediaM()
        {
            IdMedia = Guid.NewGuid().ToString();
            Name = "";
            Path = "";
            Text = "";
            CompleteAddress = "";
            TimeStamp = DateTime.Now;

            Synced = false;
            PresenteOnline = false;
            ModType = ModType.ADDED;
        }

    }
}
