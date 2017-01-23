using System;
using System.Collections.Generic;
using System.Text;
using SQLite.Net;
using TravelStory.iOS.Servicies;
using Xamarin.Forms;
using static TravelStory.Model.Interfaces;
using System.IO;
using SQLite.Net.Platform.XamarinIOS;

[assembly: Dependency (typeof(DB_iOS))]
namespace TravelStory.iOS.Servicies
{
    public class DB_iOS : ILocalDB
    {
        public SQLiteConnection GetConnection(string dbName)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, dbName);

            var conn = new SQLiteConnection(new SQLitePlatformIOS(), path);
            return conn;
        }
    }
}
