using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TruePeople.SharePreview.Middlewares
{
    internal class RemovePreviewBadgeMiddleware
    {
        private readonly RequestDelegate _next;

        public RemovePreviewBadgeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Value.StartsWith("/umbraco/sharepreview/"))
            {
                await _next(context);
                return;
            }

            // Set the body to our stream
            var originalBody = context.Response.Body;
            var newBody = new MemoryStream();
            context.Response.Body = newBody;

            // Execute other middlewares so we have the full output
            await _next(context);

            // Reset to 0
            newBody.Position = 0;

            // Read all content and replace
            var content = await new StreamReader(newBody).ReadToEndAsync();
            var regex = @"(?s)<div[^>]*id=""umbracoPreviewBadge"".*<\/div>";
            content = Regex.Replace(content, regex, "");
            var updatedStream = GenerateStreamFromString(content);
            await updatedStream.CopyToAsync(originalBody);

            context.Response.Body = originalBody;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}