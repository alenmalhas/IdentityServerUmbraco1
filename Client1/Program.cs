using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client1
{
    class Program
    {
        static void Main(string[] args)
        {
            var task  = CallApi();
            task.Wait();

            Console.ReadLine();
        }

        static async Task CallApi(string authorityUrl = "http://localhost:5000", 
            string clientId = "client", string clientSecret ="secret", 
            string scope = "api1", 
            string apiToCall = "http://localhost:5001/api/identity"
            )
        { 
            //Console.WriteLine("Hello World!");
            var disco = await DiscoveryClient.GetAsync(authorityUrl);
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, clientSecret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(scope);
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            #region call the api

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var response = await client.GetAsync(apiToCall);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);


            #endregion
        }
    }
}
