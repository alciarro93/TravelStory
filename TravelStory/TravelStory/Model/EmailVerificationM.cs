using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Model
{
    /* Autogenerate da Visual Studio. Menu Modifica -> Incolla speciale -> Incolla JSON come classi */
    public class Rootobject
    {
        public bool is_valid { get; set; }
        public string address { get; set; }
        public Parts parts { get; set; }
        public object did_you_mean { get; set; }
    }

    public class Parts
    {
        public string local_part { get; set; }
        public string domain { get; set; }
    }
}
