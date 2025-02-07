using System;
using System.Linq;
using TruePeople.SharePreview.Helpers;
using TruePeople.SharePreview.Services;
using Newtonsoft.Json;
using TruePeople.SharePreview.Models;
using System.Globalization;
using System.Security.Cryptography;
using System.Threading;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Models.PublishedContent;
using Microsoft.AspNetCore.Mvc.Filters;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Routing;
using Microsoft.AspNetCore.Http.Extensions;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using System.Collections.Generic;
using Umbraco.Cms.Core.PublishedCache;

namespace TruePeople.SharePreview.Controllers.FrontendControllers
{
    public class SharePreviewController : UmbracoPageController, IVirtualPageController
    {
        public const string AcceptPreviewMode = "UMB-WEBSITE-PREVIEW-ACCEPT";

        private readonly ISharePreviewSettingsService _sharePreviewSettings;
        private readonly IContentService _contentService;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<SharePreviewController> _logger;
        private readonly UriUtility _uriUtility;
        private readonly IPublishedRouter _publishedRouter;

        public SharePreviewController(
            ISharePreviewSettingsService settingsService,
            IContentService contentService,
            IVariationContextAccessor variationContextAccessor,
            IUmbracoContextAccessor umbracoContextAccessor,
            ILocalizationService localizationService,
            ILogger<SharePreviewController> logger,
            ICompositeViewEngine viewEngine,
            IPublishedRouter publishedRouter,
            UriUtility uriUtility) : base(logger, viewEngine)
        {
            _sharePreviewSettings = settingsService;
            _contentService = contentService;
            _variationContextAccessor = variationContextAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
            _localizationService = localizationService;
            _logger = logger;
            _publishedRouter = publishedRouter;
            _uriUtility = uriUtility;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // We set the variationContext again because on some umbraco versions it gets overridden.
            var settings = _sharePreviewSettings.GetSettings();
            var decrypted = TPEncryptHelper.DecryptString(RouteData.Values["pageId"].ToString(), settings.PrivateKey);
            var sharePreviewContext = JsonConvert.DeserializeObject<SharePreviewContext>(decrypted);
            _variationContextAccessor.VariationContext = new VariationContext(sharePreviewContext.Culture);

            var viewResult = View(CurrentPage.GetTemplateAlias(), CurrentPage);
            return viewResult;
        }

        public IPublishedContent FindContent(ActionExecutingContext actionExecutingContext)
        {
            // We need a UmbracoContext
            if (!_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
                return null;

            var settings = _sharePreviewSettings.GetSettings();
            
            //Decode first layer of the base64 string;
            //Then decode second layer and decrypt it with private key.
            try
            {
                var decrypted = TPEncryptHelper.DecryptString(actionExecutingContext.RouteData.Values["pageId"].ToString(), settings.PrivateKey);
                var sharePreviewContext = JsonConvert.DeserializeObject<SharePreviewContext>(decrypted);

                if (sharePreviewContext.NodeId == default || sharePreviewContext.NewestVersionId == default)
                {
                    RedirectToInvalidUrl(actionExecutingContext, settings.NotValidUrl);
                }

                var latestNodeVersion = _contentService.GetVersionsSlim(sharePreviewContext.NodeId, 0, 1).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(sharePreviewContext.Culture))
                {
                    var latestPublishDate = latestNodeVersion.GetPublishDate(sharePreviewContext.Culture).GetValueOrDefault();

                    if (latestNodeVersion.EditedCultures.Any(x => x.Equals(sharePreviewContext.Culture))
                        && latestPublishDate.Ticks <= sharePreviewContext.DateTicks)
                    {
                        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(sharePreviewContext.Culture);
                        _variationContextAccessor.VariationContext = new VariationContext(sharePreviewContext.Culture);

                        EnableForcedPreview(umbracoContext);

                        var page = umbracoContext.Content.GetById(true, sharePreviewContext.NodeId);
                        PrepareRequest(actionExecutingContext);
                        umbracoContext.PublishedRequest ??= CreatePublishedRequest(actionExecutingContext.HttpContext, page, sharePreviewContext.Culture);                            
                        return page;
                    }
                }
                else if (latestNodeVersion != null && latestNodeVersion.VersionId == sharePreviewContext.NewestVersionId && latestNodeVersion.Edited)
                {
                    EnableForcedPreview(umbracoContext);
                    var page = umbracoContext.Content.GetById(true, sharePreviewContext.NodeId);

                    //Since we don't use the base.PreparePublishedRequest, the culture isn't being set correctly.
                    //Set it like umbraco does for previews.
                    var defaultCulture = _localizationService.GetDefaultLanguageIsoCode();
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(defaultCulture);
                    PrepareRequest(actionExecutingContext);
                    umbracoContext.PublishedRequest ??= CreatePublishedRequest(actionExecutingContext.HttpContext, page, sharePreviewContext.Culture);
                    return page;
                }

                RedirectToInvalidUrl(actionExecutingContext, settings.NotValidUrl);
            }
            catch (CryptographicException)
            {
                //Probably means someone changed their key and still have a url that was encrypted with the old key.
                RedirectToInvalidUrl(actionExecutingContext, settings.NotValidUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured when rendering shareable preview content.");
                RedirectToInvalidUrl(actionExecutingContext, settings.NotValidUrl);
            }

            //When it gets here something went wrong...
            return null;
        }
        // We create our own published request because umbraco does not make a published request in a custom mvc route's
        private IPublishedRequest CreatePublishedRequest(HttpContext httpContext, IPublishedContent publishedContent, string culture)
        {
            var originalRequestUrl = new Uri(httpContext.Request.GetEncodedUrl());
            Uri cleanedUri = _uriUtility.UriToUmbraco(originalRequestUrl);
            var domainAndUri = GetDomainAndUriForRoute(cleanedUri, culture);
            IPublishedRequestBuilder requestBuilder = _publishedRouter.CreateRequestAsync(cleanedUri).GetAwaiter().GetResult();

            requestBuilder.SetPublishedContent(publishedContent);
            requestBuilder.SetDomain(domainAndUri);
            requestBuilder.SetCulture(culture);
            
            return requestBuilder.Build();
        }
        private DomainAndUri? GetDomainAndUriForRoute(Uri contentUrl, string culture)
        {
            IDomainCache? domainCache = _umbracoContextAccessor.GetRequiredUmbracoContext().PublishedSnapshot.Domains;
            if (domainCache == null)
            {
                throw new InvalidOperationException("Could not obtain the domain cache in the current context");
            }

            IEnumerable<DomainAndUri> domains = domainCache.GetAll(false).Where(d => !d.IsWildcard).Select(d => new DomainAndUri(d, contentUrl));

            return DomainUtilities.SelectDomain(domains, contentUrl, culture, domainCache.DefaultCulture);
        }

        private void PrepareRequest(ActionExecutingContext req)
        {
            // Since Umbraco 8.10 they show a "Exit Preview mode" message if you visit the site in preview mode. We don't want this in this package.
            // They check if the message should be shown by using a cookie. If this cookie doesn't exists, we set the cookie with the same expiration.
            if (req.HttpContext != null)
            {
                req.HttpContext.Response.Cookies.Append(AcceptPreviewMode, "true", new CookieOptions { Expires = DateTime.Now.AddMinutes(5) });
            }
        }

        private void RedirectToInvalidUrl(ActionExecutingContext req, string url)
        {
            var httpContext = req.HttpContext;
            if (string.IsNullOrWhiteSpace(url))
            {
                httpContext.Response.Redirect("/");
            }
            else
            {
                //Redirect to configured page.
                httpContext.Response.Redirect(url);
            }
        }

        /// <summary>
        /// Sets the publishedsnapshot to previewmode. This is used to also have the latest version of picked content (MNTP, Content Picker, etc.)
        /// </summary>
        private void EnableForcedPreview(IUmbracoContext umbracoContext)
        {
            umbracoContext.PublishedSnapshot.ForcedPreview(true, orig =>
            {
                umbracoContext.PublishedSnapshot.ForcedPreview(orig);
            });
        }
    }
}