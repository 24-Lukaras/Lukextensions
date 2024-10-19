using Newtonsoft.Json;

namespace Lukextensions.SharePoint
{
    public class SharepointProjectSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string SiteId { get; set; } = string.Empty;

        public static SharepointProjectSettings FromJson(string json) => JsonConvert.DeserializeObject<SharepointProjectSettings>(json);
        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
