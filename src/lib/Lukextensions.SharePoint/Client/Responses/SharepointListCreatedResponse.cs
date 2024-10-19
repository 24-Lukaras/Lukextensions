using Newtonsoft.Json;

namespace Lukextensions.SharePoint
{
    public class SharepointListCreatedResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
