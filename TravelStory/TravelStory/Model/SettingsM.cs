using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Model
{
    public class SettingsM
    {
        [PrimaryKey][Unique][NotNull]
        public string Key { get; set; }

        [NotNull]
        public string Value { get; set; }

        public SettingsM()
        {
            Key = "";
            Value = "";
        }

        public SettingsM(string key, string value)
        {
            Key = key;
            Value = value; ;
        }
    }
}
