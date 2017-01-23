using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Messages
{
    public class UserLogged
    {
        public bool Logged { get; set; }

        public UserLogged(bool logged)
        {
            Logged = logged;
        }
    }
}
