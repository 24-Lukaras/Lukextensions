using Lukextensions.SharePoint.Requests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Lukextensions.SharePoint
{
    public class GraphClient
    {        
        private readonly string _clientId;
        private readonly string _tenantId;
        private readonly string _clientSecret;

        private HttpClient _authClient;
        private HttpClient _requestsClient;

        private string accessToken;
        private DateTime tokenValidUntil;

        public GraphClient(SharepointProjectSettings settings) : this(settings.ClientId, settings.TenantId, settings.ClientSecret) { }
        public GraphClient(string clientId, string tenantId, string clientSecret)
        {
            _clientId = clientId;
            _tenantId = tenantId;
            _clientSecret = clientSecret;

            _authClient = new HttpClient();
            _authClient.BaseAddress = new Uri("https://login.microsoftonline.com");

            _requestsClient = new HttpClient();
            _requestsClient.BaseAddress = new Uri("https://graph.microsoft.com");
            _requestsClient.DefaultRequestHeaders.Add("Accept", "*/*");
        }


        private async Task Authenticate()
        {
            var httpResponse = await _authClient.PostAsync($"{_tenantId}/oauth2/v2.0/token", new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "client_id", _clientId },
                { "scope", "https://graph.microsoft.com/.default" },
                { "client_secret", _clientSecret },
                { "grant_type", "client_credentials" }
            }));

            var response = JsonConvert.DeserializeObject<GraphOAuthResponse>(await httpResponse.Content.ReadAsStringAsync());
            accessToken = response.AccessToken;
            tokenValidUntil = DateTime.Now.AddSeconds(response.ExpiresIn);

            _requestsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<T> Request<T>(GraphRequestBase<T> request)
        {
            if (tokenValidUntil == default || DateTime.Now >= tokenValidUntil.AddMinutes(-10))
            {
                await Authenticate();
            }

            return await request.ExecuteRequest(_requestsClient);
        }

    }
}
