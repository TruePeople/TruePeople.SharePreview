using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TruePeople.SharePreview.Controllers.FrontendControllers;
using TruePeople.SharePreview.Middlewares;
using TruePeople.SharePreview.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace TruePeople.SharePreview.Composers
{
    internal class TPSharePreviewComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter(nameof(SharePreviewController))
                {
                    Endpoints = app => app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllerRoute(
                            "TPSharePreview",
                            "umbraco/sharepreview/{pageId}",
                            new { Controller = "SharePreview", Action = "Index" });

                       
                    }),
                    PrePipeline = app =>
                    {
                        app.UseMiddleware<RemovePreviewBadgeMiddleware>();
                    }
                });
            });

            builder.Services.AddSingleton<ISharePreviewSettingsService, SharePreviewSettingsService>();
        }
    }
}