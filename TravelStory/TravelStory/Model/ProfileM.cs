using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Model
{
    public class ProfileM
    {
        [Newtonsoft.Json.JsonProperty("Id")]
        [PrimaryKey]
        public string IdProfile { get; set; }

        [Newtonsoft.Json.JsonProperty("Deleted")]
        public bool Deleted { get; set; }

        [NotNull]
        public string Email { get; set; }

        public string Password { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string Surname { get; set; }        

        [NotNull]
        public string ActiveTravel { get; set; }

        [NotNull]
        [Newtonsoft.Json.JsonIgnore]
        public bool Synced { get; set; }

        [NotNull]
        [Newtonsoft.Json.JsonIgnore]
        public bool FirstLogin { get; set; }

        public ProfileM()
        {
            IdProfile = Guid.NewGuid().ToString();
            Synced = false;
            FirstLogin = true;
        }

        public ProfileM(string email,string password,string name, string surname,string activetravel)
        {
            IdProfile = Guid.NewGuid().ToString();
            Email = email;
            Password = password;
            Name = name;
            Surname = surname;
            ActiveTravel = activetravel;
            Synced = false;
            FirstLogin = true;
        }

        public ProfileM(string id, string email, string password, string name, string surname, string activetravel)
        {
            IdProfile = id;
            Email = email;
            Password = password;
            Name = name;
            Surname = surname;
            ActiveTravel = activetravel;
            Synced = true;
            FirstLogin = true;
        }

    }
}
