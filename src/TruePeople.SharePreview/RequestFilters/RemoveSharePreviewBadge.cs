using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace TruePeople.SharePreview.RequestFilters
{
    public class RemoveSharePreviewBadge : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction || !filterContext.HttpContext.Request.Url.AbsolutePath.Contains("umbraco/sharepreview/index/"))
                return;

            var originalFilter = filterContext.HttpContext.Response.Filter;
            filterContext.HttpContext.Response.Filter = new PreviewUrlFilter(originalFilter);
            base.OnActionExecuting(filterContext);
        }
    }

    public class PreviewUrlFilter : MemoryStream
    {
        private readonly Stream responseStream;

        public PreviewUrlFilter(Stream stream)
        {
            responseStream = stream;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string html = Encoding.UTF8.GetString(buffer);
            var regex = @"(?s)<div[^>]*id=""umbracoPreviewBadge"".*<\/div>";
            Regex.Replace(html, regex, "");

            buffer = Encoding.UTF8.GetBytes(html);
            responseStream.Write(buffer, offset, buffer.Length);
        }
    }
}
