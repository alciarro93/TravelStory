using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Model
{
    public class Interfaces
    {
        public interface ILocalDB
        {
            SQLiteConnection GetConnection(string dbName);
        }
        
        public interface ILocalSettings
        {
            string GetMediaSavePath();
            void GPS_Setting();
            void HideKeyboard();
        }

        public interface IRestSharp
        {
            Task<bool> VerifyEmail(string email);
        }

    }
}
