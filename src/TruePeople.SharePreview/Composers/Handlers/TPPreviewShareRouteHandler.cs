using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Routing;
using TruePeople.SharePreview.Helpers;
using TruePeople.SharePreview.Services;
using System.Web;
using Newtonsoft.Json;
using TruePeople.SharePreview.Models;
using System.Globalization;
using System.Security.Cryptography;
using System.Threading;

namespace TruePeople.SharePreview.Composers.Handlers
{
    internal class TPPreviewShareRouteHandler : UmbracoVirtualNodeRouteHandler
    {
        protected override IPublishedContent FindContent(RequestContext requestContext, UmbracoContext umbracoContext)
        {
            var contentService = Umbraco.Web.Composing.Current.Services.ContentService;
            var sharePreviewSettingsService = Umbraco.Web.Composing.Current.Factory.GetInstance(typeof(SharePreviewSettingsService)) as SharePreviewSettingsService;
            var settings = sharePreviewSettingsService.GetSettings();

            //Decode first layer of the base64 string;
            //Then decode second layer and decrypt it with private key.
            try
            {
                var decrypted = TPEncryptHelper.DecryptString(requestContext.RouteData.Values["pageId"].ToString(), settings.PrivateKey);
                var sharePreviewContext = JsonConvert.DeserializeObject<SharePreviewContext>(decrypted);

                if (sharePreviewContext.NodeId == default || sharePreviewContext.NewestVersionId == default)
                {
                    RedirectToInvalidUrl(settings.NotValidUrl);
                }

                var latestNodeVersion = contentService.GetVersionsSlim(sharePreviewContext.NodeId, 0, 1).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(sharePreviewContext.Culture))
                {
                    var latestPublishDate = latestNodeVersion.GetPublishDate(sharePreviewContext.Culture).GetValueOrDefault();

                    if (latestNodeVersion.EditedCultures.Any(x => x.Equals(sharePreviewContext.Culture))
                        && latestPublishDate.Ticks <= sharePreviewContext.DateTicks)
                    {
                        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(sharePreviewContext.Culture);
                        Umbraco.Web.Composing.Current.VariationContextAccessor.VariationContext = new VariationContext(sharePreviewContext.Culture);

                        var page = Umbraco.Web.Composing.Current.UmbracoContext.Content.GetById(true, sharePreviewContext.NodeId);
                        return page;
                    }
                }
                else if (latestNodeVersion != null && latestNodeVersion.VersionId == sharePreviewContext.NewestVersionId && latestNodeVersion.Edited)
                {
                    var page = Umbraco.Web.Composing.Current.UmbracoContext.Content.GetById(true, sharePreviewContext.NodeId);

                    //Since we don't use the base.PreparePublishedRequest, the culture isn't being set correctly.
                    //Set it like umbraco does for previews.
                    var defaultCulture = Umbraco.Web.Composing.Current.Services.LocalizationService.GetDefaultLanguageIsoCode();
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(defaultCulture);

                    return page;
                }

                RedirectToInvalidUrl(settings.NotValidUrl);

                //When it gets here something went wrong...
                return null;
            }
            catch (CryptographicException)
            {
                //Probably means someone changed their key and still have a url that was encrypted with the old key.
                RedirectToInvalidUrl(settings.NotValidUrl);
                return null;
            }
            catch (Exception ex)
            {
                Umbraco.Web.Composing.Current.Logger.Error(typeof(TPPreviewShareRouteHandler), ex, "Error occured when rendering shareable preview content.");
                RedirectToInvalidUrl(settings.NotValidUrl);
                return null;
            }
        }
        protected override void PreparePublishedContentRequest(PublishedRequest request)
        {
            var def = new RouteDefinition
            {
                ActionName = request.UmbracoContext.HttpContext.Request.RequestContext.RouteData.GetRequiredString("action"),
                ControllerName = request.UmbracoContext.HttpContext.Request.RequestContext.RouteData.GetRequiredString("controller"),
                PublishedRequest = request
            };

            // set the special data token to the current route definition
            request.UmbracoContext.HttpContext.Request.RequestContext.RouteData.DataTokens[Umbraco.Core.Constants.Web.UmbracoRouteDefinitionDataToken] = def;
            request.UmbracoContext.HttpContext.Request.RequestContext.RouteData.Values["action"] = request.PublishedContent.GetTemplateAlias();

            // We set it here again, because it gets overwritten in the pipeline.
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(Umbraco.Web.Composing.Current.VariationContextAccessor.VariationContext.Culture);
        }


        private void RedirectToInvalidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                HttpContext.Current.Response.Redirect("/");
            }
            else
            {
                //Redirect to configured page.
                HttpContext.Current.Response.Redirect(url);
            }
        }
    }
}
