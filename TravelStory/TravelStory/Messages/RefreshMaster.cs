using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Messages
{
    public class RefreshMaster
    {
        public bool RefreshMenu { get; set; }

        public RefreshMaster(bool refreshMaster)
        {
            RefreshMenu = refreshMaster;
        }
    }

}
