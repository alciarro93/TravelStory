using Microsoft.WindowsAzure.MobileServices;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Model
{
    public class AzureDataService<T>
    {
        public MobileServiceClient MobileService { get; set; }

        public AzureDataService()
        {
            MobileService = new MobileServiceClient(ConstantStrings.ApplicationURL);
        }

        public async Task<IEnumerable<ProfileM>> CheckLogin(string email, string password)
        {
            return await MobileService.GetTable<ProfileM>().Where(a => a.Email == email &&
                                                                        a.Password == password &&
                                                                        a.Deleted == false).ToEnumerableAsync();
        }

        public async Task<IEnumerable<ProfileM>> CheckEmail(string email)
        {
            return await MobileService.GetTable<ProfileM>().Where(a => a.Email == email &&
                                                                    a.Deleted == false).ToEnumerableAsync();
        }

        public async Task<IEnumerable<TravelM>> DownloadTravels(string email)
        {
            return await MobileService.GetTable<TravelM>().Where(a => a.TravelEmail == email &&
                                                                    a.Deleted == false).ToEnumerableAsync();
        }

        public async Task<IEnumerable<MediaM>> DownloadMedias(string idTravel)
        {
            return await MobileService.GetTable<MediaM>().Where(a => a.IdTravel == idTravel &&
                                                                    a.Deleted == false).ToEnumerableAsync();
        }


        public async Task InsertRow(T item)
        {
            await MobileService.GetTable<T>().InsertAsync(item);
        }

        public async Task UpdateRow(T item)
        {
            await MobileService.GetTable<T>().UpdateAsync(item);
        }

        public async Task DeleteRow(T item)
        {
            await MobileService.GetTable<T>().DeleteAsync(item);
        }


    }
}
