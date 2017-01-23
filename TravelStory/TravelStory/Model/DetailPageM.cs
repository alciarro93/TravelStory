using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Resources;
using TravelStory.View;
using Xamarin.Forms;

namespace TravelStory.Model
{
    public class DetailPageM
    {
        public Type PageType { get; private set; }
        public string PageName { get; private set; }
        public string PageIcon { get; private set; }

        public DetailPageM()
        {

        }

        public DetailPageM(Type pageType)
        {
            if (pageType.Namespace != "TravelStory.View")
            {
                PageType = typeof(Home);
                PageName = Dictionary.ResourceManager.GetString("Home", Dictionary.Culture);
                PageIcon = "Home.png";
            }

            PageType = pageType;
            PageName = Dictionary.ResourceManager.GetString(pageType.Name, Dictionary.Culture);
            PageIcon = pageType.Name + ".png";         
        }

        public override string ToString()
        {
            return PageName;
        }
    }
}
