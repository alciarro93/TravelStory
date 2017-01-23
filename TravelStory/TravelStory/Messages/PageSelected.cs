using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Messages
{
    public class PageSelected
    {
        public Type PageType { get; private set; }
        public bool NavigationPush { get; private set; }
        public bool NavigationPop { get; private set; }

        public PageSelected(Type pageType)
        {
            PageType = pageType;
            NavigationPush = false;
            NavigationPop = false;
        }

        public PageSelected(Type pageType, bool navPush, bool navPop)
        {
            PageType = pageType;
            NavigationPush = navPush;
            NavigationPop = navPop;
        }
    }
}
