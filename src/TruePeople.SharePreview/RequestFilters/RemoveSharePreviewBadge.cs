//using Microsoft.AspNetCore.Http.Extensions;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace TruePeople.SharePreview.RequestFilters
//{
//    public class RemoveSharePreviewBadge : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            if (filterContext.IsChildAction || !filterContext.HttpContext.Request.GetDisplayUrl().Contains("umbraco/sharepreview/index/"))
//                return;

//            var originalFilter = filterContext.HttpContext.Response.Filter;
//            filterContext.HttpContext.Response..Filter = new PreviewUrlFilter(originalFilter);
//            base.OnActionExecuting(filterContext);
//        }
//    }

//    public class PreviewUrlFilter : MemoryStream
//    {
//        private readonly StringBuilder _data = new StringBuilder();
//        private readonly Stream responseStream;

//        public PreviewUrlFilter(Stream stream)
//        {
//            responseStream = stream;
//        }

//        public override void Write(byte[] buffer, int offset, int count)
//        {
//            _data.Append(Encoding.UTF8.GetString(buffer, offset, count));
//        }

//        public override void Close()
//        {
//            var html = _data.ToString();
//            var regex = @"(?s)<div[^>]*id=""umbracoPreviewBadge"".*<\/div>";
//            html = Regex.Replace(html, regex, "");

//            var output = Encoding.UTF8.GetBytes(html);
//            responseStream.Write(output, 0, output.Length);
//            responseStream.Flush();
//            _data.Clear();
//        }
//    }
//}
