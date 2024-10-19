
using Newtonsoft.Json;

namespace Lukextensions.SharePoint
{
    internal class GraphOAuthResponse
    {
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

    }
}
