using IdentityModel.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeedzDrive
{
    public sealed class IsAliveService
    {

        private static IsAliveService? _instance;

        private IsAliveService()
        {
        }

        public static IsAliveService GetInstance()
        {
            return _instance ??= new IsAliveService();
        }

        public void Check()
        {
            var timer = new Timer(callback: Call, null, 0, (5 * 1000));
        }

        private  static void Call(object o) 
        {
            try
            {
                var client = new HttpClient();
                var token = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = Preferences.Default.AuthUri,
                    ClientId = Preferences.Default.ClientId,
                    ClientSecret = Preferences.Default.Secret,
                    Scope = "https://api-drive.integration.seedz.ag/tenantid"
                }).Result;

                var restClient = new RestClient("https://api-drive.integration.seedz.ag/is-alive");
                var request = new RestRequest("", Method.Head);

                request.AddHeader("Authorization", $"Bearer {token.AccessToken}");

                var response = restClient.Execute(request);
                response.ThrowIfError(); //todo: verify
            }
            catch (Exception exception)
            {
                LogService.GetInstance().Write($"{DateTime.Now} - {exception.Message}");
                IconState.GetInstance().Current = IconState.GetInstance().IconRed;
            }
        }
    }
}
