using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
