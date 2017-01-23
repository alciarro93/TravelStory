using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using TravelStory.Droid.Servicies;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;

[assembly: Dependency(typeof(DB_Droid))]
namespace TravelStory.Droid.Servicies
{
    public class DB_Droid : Model.Interfaces.ILocalDB
    {
        public SQLiteConnection GetConnection(string dbName)
        {
            string dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var dbFilePath = System.IO.Path.Combine(dbPath, dbName);
            var conn = new SQLiteConnection(new SQLitePlatformAndroid(), dbFilePath);
            return conn;
        }
    }
}