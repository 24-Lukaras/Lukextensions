using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lukextensions.SharePoint
{
    public class SharepointColumnsResponse
    {
        [JsonProperty("value")]
        public List<SharepointColumn> Columns { get; set; } = new List<SharepointColumn>();
    }

    public class SharepointColumn
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }
        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("boolean")]
        public Dictionary<string, object> Boolean { get; set; }        
        [JsonProperty("text")]
        public Dictionary<string, object> Text { get; set; }
        [JsonProperty("number")]
        public Dictionary<string, object> Number { get; set; }
        [JsonProperty("dateTime")]
        public Dictionary<string, object> DateTime { get; set; }
    }
}
