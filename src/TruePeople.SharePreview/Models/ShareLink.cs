namespace TruePeople.SharePreview.Models
{
    public class ShareLink
    {
        public ShareLink(string cultureName, string link)
        {
            CultureName = cultureName;
            Link = link;
        }

        public string CultureName { get; set; }

        public string Link { get; set; }
    }
}