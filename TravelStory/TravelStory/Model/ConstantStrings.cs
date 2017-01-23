using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TravelStory.Model
{
    public class ConstantStrings
    {
        public static readonly string dbName = "TravelBlogging.db3";

        //Date and time formats
        public static readonly List<string> DateFormats = new List<string>(new string[] { DateFormat1Ex, DateFormat2Ex, DateFormat3Ex } );

        public static readonly string DateFormat1 = "dd/MM/yyyy";
        public const string DateFormat1Ex = "24/05/2012";

        public static readonly string DateFormat2 = "MM/dd/yyyy";
        public const string DateFormat2Ex = "05/24/2012";

        public static readonly string DateFormat3 = "yyyy/MM/dd";
        public const string DateFormat3Ex = "2012/05/24";

        public static readonly string TimeFormt = "HH:mm";

        //Azure
        public static readonly string ApplicationURL = @"https://travelstory.azurewebsites.net";
        //public static string Account = "travelstorystorage";
        //public static string SharedKeyAuthorizationScheme = "?sv=2015-04-05&ss=bfqt&srt=sco&sp=rwdlacup&se=2016-07-30T00:09:51Z&st=2016-07-24T16:09:51Z&spr=https,http&sig=t2rv2ENtKCQLDdWz1Ub9O4PKndnRkMdjkpgDVtlrzCE%3D";
        //public static string BlobEndPoint = "https://travelstorystorage.blob.core.windows.net/";
        //public static string Key = "Ksq05W9gNV+S0HzVmXJ6+QhT9shtn/ng0Ycv2xOU+vKu0KvHlJAvzQIauG4d9gs3Sh48rca4AM2LBUyqrp3ZSw==";
        //public static string ContainerName = "travelstoryfiles";
        //public static string FileLocation = BlobEndPoint + ContainerName;

        //settings
        public static readonly string LANG_APP = "LANG_APP";
        public static readonly string DEFAULT_LANG = "it"; 
        public static readonly string LAST_LAT = "LAST_LAT";
        public static readonly string LAST_LONG = "LAST_LONG";
        public static readonly string CURRENT_USERNAME = "USERNAME";
        public static readonly string REMOTE_SYNC = "REMOTE_SYNC";
        public static readonly string DATEFORMAT = "DATEFORMAT";

        //colors
        public static readonly Color primary = Color.FromHex("009688");
        public static readonly Color primaryDark = Color.FromHex("00796B");
        public static readonly Color accent = Color.FromHex("FBC02D");
        public static readonly Color alter_accent = Color.FromHex("FFEB3B");
        public static readonly Color window_background = Color.FromHex("F5F5F5");
        public static readonly Color textbox_background = Color.FromHex("f2f2f2");
        public static readonly Color delete_color = Color.FromHex("E53935");
        public static readonly Color text_primary = Color.FromHex("262626");
        public static readonly Color text_secondary = Color.FromHex("737373");
        public static readonly Color text_hint = Color.FromHex("b3b3b3");
        public static readonly Color header_menu = Color.FromHex("FBC02D");
    }
}
