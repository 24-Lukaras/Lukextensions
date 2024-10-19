using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lukextensions.SharePoint
{
    public class SharepointListsResponse
    {
        [JsonProperty("value")]
        public List<SharepointListResponse> Items { get; set; }
    }

    public class SharepointListResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}
