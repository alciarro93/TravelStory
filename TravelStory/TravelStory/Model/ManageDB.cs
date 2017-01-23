using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Resources;
using Xamarin.Forms;
using static TravelStory.Model.Interfaces;
namespace TravelStory.Model
{
    public class ManageDB
    {
        public static SQLiteConnection localDB;
        public static ManageDB mainConnection;

        public ManageDB(){ }

        #region Inizialize db and general settings
        public static void InizializeApp()
        {
            mainConnection = new ManageDB();

            localDB = DependencyService.Get<ILocalDB>().GetConnection(ConstantStrings.dbName);
            localDB.CreateTable<SettingsM>();
            localDB.CreateTable<TravelM>();
            localDB.CreateTable<MediaM>();
            localDB.CreateTable<ProfileM>();

            //controllo lingua app
            if (mainConnection.GetSetting(ConstantStrings.LANG_APP) == null)
            {
                var connSettings = new MaintenanceDB<SettingsM>();
                var itemS = new SettingsM(ConstantStrings.LANG_APP, ConstantStrings.DEFAULT_LANG);
                connSettings.Save(itemS);
                Dictionary.Culture = new CultureInfo(ConstantStrings.DEFAULT_LANG);
            }
            else
            {
                Dictionary.Culture = new CultureInfo(mainConnection.GetSetting(ConstantStrings.LANG_APP).Value);
            }

            //controllo impostazione di sincronizzazione
            if (mainConnection.GetSetting(ConstantStrings.REMOTE_SYNC) == null)
            {
                var connSettings = new MaintenanceDB<SettingsM>();
                var itemS = new SettingsM(ConstantStrings.REMOTE_SYNC, "true");
                connSettings.Save(itemS);
            }

            //controllo impostazione del formato della data
            if (mainConnection.GetSetting(ConstantStrings.DATEFORMAT) == null)
            {
                var connSettings = new MaintenanceDB<SettingsM>();
                var itemS = new SettingsM(ConstantStrings.DATEFORMAT, ConstantStrings.DateFormat1);
                connSettings.Save(itemS);
            }

            //controllo se un utente è loggato
            if (mainConnection.GetSetting(ConstantStrings.CURRENT_USERNAME) != null)
            {
                var email = mainConnection.GetSetting(ConstantStrings.CURRENT_USERNAME).Value;
                App.Current_User = mainConnection.GetProfile(email);
                App.isLoggedIn = true;
            }
            else
            {
                App.isLoggedIn = false;
            }

        }
        #endregion

        #region PROFILES
        public ProfileM GetProfile(string email)
        {
            return localDB.Table<ProfileM>().FirstOrDefault(a => a.Email == email);
        }
        #endregion

        #region SETTINGS
        public SettingsM GetSetting(string key)
        {
            return localDB.Table<SettingsM>().FirstOrDefault(a => a.Key == key);
        }
        #endregion

        #region TRAVELS
        public TravelM GetTravel(string idTravel)
        {
            return localDB.Table<TravelM>().FirstOrDefault(a => a.IdTravel == idTravel &&
                                                           a.TravelEmail == App.Current_User.Email &&
                                                           a.ModType != ModType.DELETED &&
                                                           a.Deleted == false);
        }

        public IList<TravelM> GetEndedTravels()
        {
            return localDB.Table<TravelM>().Where(a => a.Ended == true &&
                                                  a.TravelEmail == App.Current_User.Email &&
                                                  a.ModType != ModType.DELETED &&
                                                  a.Deleted == false).OrderByDescending(a => a.EndDate).ToList();
        }

        public IEnumerable<TravelM> GetNonSyncedTravels()
        {
            return localDB.Table<TravelM>().Where(a => a.Synced == false &&
                                                  a.TravelEmail == App.Current_User.Email &&
                                                  a.Deleted == false).ToList();
        }
        #endregion

        #region MEDIA
        public IList<MediaM> GetMediasTravel(string idTravel)
        {
            return localDB.Table<MediaM>().Where(a => a.IdTravel == idTravel &&
                                                    a.ModType != ModType.DELETED &&
                                                    a.Deleted == false).ToList();
        }

        public IEnumerable<MediaM> GetMediasNonSynced(string idTravel)
        {
            return localDB.Table<MediaM>().Where(a => a.IdTravel == idTravel &&
                                                    a.Synced == false &&
                                                    a.Deleted == false).ToList();
        }

        public MediaM GetMedia(string idMedia)
        {
            return localDB.Table<MediaM>().FirstOrDefault(a => a.IdMedia == idMedia &&
                                                          a.Deleted == false);
        }
        #endregion

    }
}
