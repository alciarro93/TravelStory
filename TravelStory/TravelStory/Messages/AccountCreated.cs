using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Messages
{
    public class AccountCreated
    {
        public bool NewAccountCreated { get; set; }

        public AccountCreated(bool accountCreated)
        {
            NewAccountCreated = accountCreated;
        }
    }
}
