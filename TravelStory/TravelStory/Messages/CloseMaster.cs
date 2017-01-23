using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Messages
{
    public class CloseMaster
    {
        public bool CloseMasterMenu { get; set; }

        public CloseMaster(bool closeMaster)
        {
            CloseMasterMenu = closeMaster;
        }
    }
}
