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
using TravelStory.Droid.Servicies;
using static TravelStory.Model.Interfaces;
using Xamarin.Forms;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TravelStory.Model;

[assembly: Dependency(typeof(RestSharp_Droid))]
namespace TravelStory.Droid.Servicies
{
    public class RestSharp_Droid : IRestSharp
    {
        public async Task<bool> VerifyEmail(string email)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", "pubkey-1574e014947d3aff61d26f214b4e5bd3");
            RestRequest request = new RestRequest();
            request.Resource = "/address/validate";
            request.AddParameter("address", email);
            var response = await client.ExecuteGetTaskAsync<Rootobject>(request);

            return response.Data.is_valid;

        }
    }
}