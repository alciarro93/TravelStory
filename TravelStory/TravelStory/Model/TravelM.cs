using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Resources;

namespace TravelStory.Model
{
    public class TravelM
    {
        [Newtonsoft.Json.JsonProperty("Id")]
        [PrimaryKey, Unique]
        public string IdTravel { get; set; }

        [Newtonsoft.Json.JsonProperty("Deleted")]
        public bool Deleted { get; set; }

        [NotNull]
        public string Title { get; set; }

        [NotNull]
        public DateTime StartDate { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string StartDateString
        {
            get
            {
                var dateFormat = ManageDB.mainConnection.GetSetting(ConstantStrings.DATEFORMAT).Value;
                return StartDate.ToLocalTime().ToString(dateFormat, Dictionary.Culture);
            }
        }

        public DateTime EndDate { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string EndDateString
        {
            get
            {
                var dateFormat = ManageDB.mainConnection.GetSetting(ConstantStrings.DATEFORMAT).Value;
                return EndDate.ToLocalTime().ToString(dateFormat, Dictionary.Culture);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public string RangeDatesString
        {
            get
            {
                return StartDateString + " - " + EndDateString;
            }
        }

        [NotNull]
        public bool Ended { get; set; }

        [NotNull]
        public string TravelEmail { get; set; }

        [NotNull]
        [Newtonsoft.Json.JsonIgnore]
        public bool Synced { get; set; }

        [NotNull]
        [Newtonsoft.Json.JsonIgnore]
        public bool PresenteOnline { get; set; }

        [NotNull]
        [Newtonsoft.Json.JsonIgnore]
        public ModType ModType { get; set; }

        public TravelM()
        {
            IdTravel = Guid.NewGuid().ToString();
            StartDate = DateTime.Now;
            Ended = false;

            Synced = false;
            PresenteOnline = false;
            ModType = ModType.ADDED;
        }

    }
}
