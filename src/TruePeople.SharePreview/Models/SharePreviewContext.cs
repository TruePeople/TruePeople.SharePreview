namespace TruePeople.SharePreview.Models
{
    public class SharePreviewContext
    {
        public int NodeId { get; set; }

        public int NewestVersionId { get; set; }

        public string Culture { get; set; }

        public long DateTicks { get; set; }
    }
}