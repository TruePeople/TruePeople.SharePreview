using Newtonsoft.Json;

namespace TruePeople.SharePreview.Models
{
    public class ShareLink
    {
        public ShareLink(string cultureName, string link)
        {
            CultureName = cultureName;
            Link = link;
        }

        [JsonProperty("cultureName")]
        public string CultureName { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }
}