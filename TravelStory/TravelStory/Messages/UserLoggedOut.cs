using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Messages
{
    public class UserLoggedOut
    {
        public bool LoggedOut { get; set; }

        public UserLoggedOut(bool loggedOut)
        {
            LoggedOut = loggedOut;
        }
    }
}
