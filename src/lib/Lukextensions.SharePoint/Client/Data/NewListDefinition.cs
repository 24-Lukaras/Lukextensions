
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lukextensions.SharePoint
{
    public class NewListDefinition
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("columns")]
        public List<Dictionary<string, object>> Columns { get; set; } = new List<Dictionary<string, object>>();

    }
}
