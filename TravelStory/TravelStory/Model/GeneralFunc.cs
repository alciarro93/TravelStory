using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Plugin.Geolocator.Abstractions;
using Plugin.Geolocator;
using System.Threading;
using Xamarin.Forms;
using TravelStory.Messages;
using static TravelStory.Model.Interfaces;
using Plugin.Connectivity;
using TravelStory.Resources;
using System.Net.Http;
using System.Security.Cryptography;
using System.Globalization;
using SQLite.Net;
using System.IO;
using Java.IO;

namespace TravelStory.Model
{
    public class GeneralFunc
    {
        public static Plugin.Geolocator.Abstractions.Position Position = null;
        public static CancellationTokenSource tokenSource;

        public GeneralFunc() { }

        public static async Task<string> GetCurrentAddress(double lat, double log)
        {
            if (CheckInternet())
            {
                Geocoder geocoder = new Geocoder();
                Xamarin.Forms.Maps.Position position = new Xamarin.Forms.Maps.Position(lat, log);
                IEnumerable<string> address = await geocoder.GetAddressesForPositionAsync(position);
                return address.ToList()[0];
            }
            else
            {
                var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                var mex = Dictionary.ResourceManager.GetString("NoInternet", Dictionary.Culture);
                await App.CurrentPage.DisplayAlert(title, mex, "OK");
                return "";
            }
        }

        public static async Task GetCurretPosition()
        {
            Position = null;
            var locator = CrossGeolocator.Current;

            if (locator.IsGeolocationEnabled)
            {
                if (locator.IsGeolocationAvailable)
                {
                    locator.DesiredAccuracy = 25;
                    locator.AllowsBackgroundUpdates = true;
                    try
                    {
                        tokenSource = new CancellationTokenSource();
                        Position = await locator.GetPositionAsync(-1, tokenSource.Token); //timeoutMilliseconds: 10000

                        //salvo le coordinate gps tra le ultime registrate
                        var connSett = new MaintenanceDB<SettingsM>();

                        var item = new SettingsM(ConstantStrings.LAST_LAT, Position.Latitude.ToString());
                        connSett.SaveEdit(item);

                        item = new SettingsM(ConstantStrings.LAST_LONG, Position.Longitude.ToString());
                        connSett.SaveEdit(item);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            }
            else
            {
                DependencyService.Get<ILocalSettings>().GPS_Setting();
            }
        }

        public static bool CheckInternet()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                return true;
            }
            else
            {               
                return false;
            }
        }

        public static void Sync_Current_User(string email)
        {
            App.Current_User = ManageDB.mainConnection.GetProfile(email);
        }
     
        public static async Task UploadToAzure(bool forcedSync = false)
        {
            if (forcedSync)
            {
                await UploadToRemote();
            }
            else
            {
                if (bool.Parse(ManageDB.mainConnection.GetSetting(ConstantStrings.REMOTE_SYNC).Value))
                {
                    await UploadToRemote();
                }
            }                       
        }

        public static async Task UploadToRemote()
        {
            if (CheckInternet())
            {
                if (!App.Current_User.Synced)
                {
                    var ProfileAzure = new AzureDataService<ProfileM>();
                    await ProfileAzure.UpdateRow(App.Current_User);
                    App.Current_User.Synced = true;
                    var ProfileLocal = new MaintenanceDB<ProfileM>();
                    ProfileLocal.SaveEdit(App.Current_User);
                }

                var MediaAzure = new AzureDataService<MediaM>();
                var MediaLocal = new MaintenanceDB<MediaM>();

                var TravelAzure = new AzureDataService<TravelM>();
                var TravelLocal = new MaintenanceDB<TravelM>();

                var AllTravels = ManageDB.mainConnection.GetNonSyncedTravels();

                foreach (var itemT in AllTravels)
                {
                    switch (itemT.ModType)
                    {
                        case ModType.DELETED:
                            itemT.Deleted = true;
                            TravelLocal.SaveEdit(itemT);

                            if (itemT.PresenteOnline)
                            {
                                await TravelAzure.DeleteRow(itemT);                                                                                             
                            }

                            itemT.Synced = true;
                            TravelLocal.SaveEdit(itemT);
                            var test1 = ManageDB.mainConnection.GetNonSyncedTravels();
                            //TravelLocal.Delete(itemT);
                            break;

                        case ModType.ADDED:
                            if (!itemT.PresenteOnline)
                            {                               
                                await TravelAzure.InsertRow(itemT);
                                itemT.PresenteOnline = true;
                                itemT.Synced = true;
                                TravelLocal.SaveEdit(itemT);
                            }
                            break;

                        case ModType.UPDATED:
                            if (itemT.PresenteOnline)
                            {
                                await TravelAzure.UpdateRow(itemT);
                                itemT.Synced = true;
                                TravelLocal.SaveEdit(itemT);
                            }
                            if (!itemT.PresenteOnline)
                            {
                                await TravelAzure.InsertRow(itemT);
                                itemT.PresenteOnline = true;
                                itemT.Synced = true;
                                TravelLocal.SaveEdit(itemT);
                            }
                            break;
                    }

                    var AllMedias = ManageDB.mainConnection.GetMediasNonSynced(itemT.IdTravel);

                    foreach (var itemM in AllMedias)
                    {
                        switch (itemM.ModType)
                        {
                            case ModType.DELETED:

                                itemT.Deleted = true;
                                TravelLocal.SaveEdit(itemT);

                                if (itemM.PresenteOnline)
                                {
                                    await MediaAzure.DeleteRow(itemM);
                                }

                                itemT.Synced = true;
                                TravelLocal.SaveEdit(itemT);
                                //MediaLocal.Delete(itemM);
                                var test2 = ManageDB.mainConnection.GetMediasNonSynced(itemT.IdTravel);
                                break;

                            case ModType.ADDED:
                                if (!itemM.PresenteOnline)
                                {
                                    //if (itemM.Type == MediaType.PHOTO.ToString() || itemM.Type == MediaType.VIDEO.ToString())
                                    //{
                                    //    //var byteArray = ByteArrayFromFile(itemM.Path);
                                    //    //await uploadToBlobStorage_async(byteArray, itemM.Name);
                                    //}
                                    await MediaAzure.InsertRow(itemM);
                                    itemM.PresenteOnline = true;
                                    itemM.Synced = true;
                                    MediaLocal.SaveEdit(itemM);
                                }
                                break;

                            case ModType.UPDATED:
                                if (itemM.PresenteOnline)
                                {
                                    await MediaAzure.UpdateRow(itemM);
                                    itemM.Synced = true;
                                    MediaLocal.SaveEdit(itemM);
                                }
                                if (!itemM.PresenteOnline)
                                {
                                    await MediaAzure.InsertRow(itemM);
                                    itemM.PresenteOnline = true;
                                    itemM.Synced = true;
                                    MediaLocal.SaveEdit(itemM);
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                var mex = Dictionary.ResourceManager.GetString("NoInternet", Dictionary.Culture);
                await App.CurrentPage.DisplayAlert(title, mex, "OK");
            }
        }

        public static async Task DownloadFromAzure(string email)
        {
            var MediaAzure = new AzureDataService<MediaM>();
            var MediaLocal = new MaintenanceDB<MediaM>();

            var TravelAzure = new AzureDataService<TravelM>();
            var TravelLocal = new MaintenanceDB<TravelM>();

            var travels = await TravelAzure.DownloadTravels(email);
            foreach (var itemT in travels)
            {
                var travel = ManageDB.mainConnection.GetTravel(itemT.IdTravel);
                if (travel == null)
                {
                    TravelLocal.SaveEdit(itemT);                  
                }
                
                var medias = await MediaAzure.DownloadMedias(itemT.IdTravel);
                foreach (var itemM in medias)
                {
                    var media = ManageDB.mainConnection.GetMedia(itemM.IdMedia);
                    if (media==null)
                    {
                        MediaLocal.SaveEdit(itemM);
                    }                  
                }
            }
        }

        #region test storage
        //public static async Task<string> uploadToBlobStorage_async(byte[] blobContent, string fileName)
        //{
        //    string containerName = ConstantStrings.ContainerName;
        //    return await PutBlob_async(containerName, fileName, blobContent);
        //}

        //private static async Task<string> PutBlob_async(string containerName, string blobName, byte[] blobContent)
        //{
        //    string requestMethod = "PUT";
        //    Int32 blobLength = blobContent.Length;

        //    const String blobType = "BlockBlob";

        //    String urlPath = String.Format("{0}/{1}", containerName, blobName);
        //    String msVersion = "2009-09-19";
        //    String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

        //    String canonicalizedHeaders = String.Format("x-ms-blob-type:{0}\nx-ms-date:{1}\nx-ms-version:{2}", blobType, dateInRfc1123Format, msVersion);
        //    String canonicalizedResource = String.Format("/{0}/{1}", ConstantStrings.Account, urlPath);
        //    String stringToSign = String.Format("{0}\n\n\n{1}\n\n\n\n\n\n\n\n\n{2}\n{3}", requestMethod, blobLength, canonicalizedHeaders, canonicalizedResource);

        //    String authorizationHeader = CreateAuthorizationHeader(stringToSign);


        //    string uri = ConstantStrings.BlobEndPoint + urlPath;
        //    HttpClient client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("x-ms-blob-type", blobType);
        //    client.DefaultRequestHeaders.Add("x-ms-date", dateInRfc1123Format);
        //    client.DefaultRequestHeaders.Add("x-ms-version", msVersion);

        //    client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

        //    HttpContent requestContent = new ByteArrayContent(blobContent);
        //    HttpResponseMessage response = await client.PutAsync(uri, requestContent);

        //    if (response.IsSuccessStatusCode == true)
        //    {
        //        foreach (var aHeader in response.Headers)
        //        {
        //            if (aHeader.Key == "ETag") return aHeader.Value.ElementAt(0);
        //        }
        //    }

        //    return null;
        //}

        //private static String CreateAuthorizationHeader(String canonicalizedString)
        //{
        //    if (String.IsNullOrEmpty(canonicalizedString))
        //    {
        //        throw new ArgumentNullException("canonicalizedString");
        //    }

        //    String signature = CreateHmacSignature(canonicalizedString, Convert.FromBase64String(ConstantStrings.Key));
        //    String authorizationHeader = String.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", ConstantStrings.SharedKeyAuthorizationScheme, ConstantStrings.Account, signature);

        //    return authorizationHeader;
        //}

        //private static String CreateHmacSignature(String unsignedString, Byte[] key)
        //{
        //    if (String.IsNullOrEmpty(unsignedString))
        //    {
        //        throw new ArgumentNullException("unsignedString");
        //    }

        //    if (key == null)
        //    {
        //        throw new ArgumentNullException("key");
        //    }

        //    Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(unsignedString);
        //    using (HMACSHA256 hmacSha256 = new HMACSHA256(key))
        //    {
        //        return Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
        //    }
        //}

        //public static byte[] ByteArrayFromFile(string location)
        //{
        //    byte[] byteArray = Encoding.UTF8.GetBytes(location);

        //    return byteArray;
        //    using (var streamReader = new StreamReader(location))
        //    {
        //        var bytes = default(byte[]);
        //        using (var memstream = new MemoryStream())
        //        {
        //            streamReader.BaseStream.CopyTo(memstream);
        //            bytes = memstream.ToArray();
        //            return bytes;
        //        }
        //    }
        //}
        #endregion

    }
}
