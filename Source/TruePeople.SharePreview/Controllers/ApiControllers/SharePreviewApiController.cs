using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TruePeople.SharePreview.Helpers;
using TruePeople.SharePreview.Models;
using TruePeople.SharePreview.Services;
using Umbraco.Core.Services;
using Umbraco.Web.Composing;
using Umbraco.Web.WebApi;

namespace TruePeople.SharePreview.Controllers.ApiControllers
{
    public class SharePreviewApiController : UmbracoAuthorizedApiController
    {
        private readonly IContentService _contentService;
        private readonly SharePreviewSettingsService _sharePreviewSettingsService;

        public SharePreviewApiController(IContentService contentService, SharePreviewSettingsService sharePreviewSettingsService)
        {
            _contentService = contentService;
            _sharePreviewSettingsService = sharePreviewSettingsService;
        }

        [HttpGet]
        public bool HasShareableLink(int nodeId)
        {
            var content = _contentService.GetById(nodeId);
            return (content.Edited || content.EditedCultures.Any()) && content.TemplateId != null;
        }

        [HttpGet]
        public string GetShareableLink(int nodeId, string culture = null)
        {
            try
            {
                return GenerateShareLink(nodeId, culture);
            }
            catch (Exception ex)
            {
                Current.Logger.Error(typeof(SharePreviewApiController), ex, "Error occured whilst trying to create a shareable link");
                return null;
            }
        }

        [HttpGet]
        public List<ShareLink> GetShareableLinks(int nodeId)
        {
            var result = new List<ShareLink>();

            var content = _contentService.GetById(nodeId);
            if (content.EditedCultures.Any())
            {
                foreach (var editedCulture in content.EditedCultures)
                {
                    var cultureInfo = new CultureInfo(editedCulture);
                    result.Add(new ShareLink(cultureInfo.DisplayName, GenerateShareLink(nodeId, editedCulture)));
                }
            }
            return result;
        }

        private string GenerateShareLink(int nodeId, string culture = null)
        {
            var privateEncryptionKey = _sharePreviewSettingsService.GetSettings().PrivateKey;

            var latestNodeVersion = _contentService.GetVersionIds(nodeId, 1).FirstOrDefault();

            var objToEncrypt = new SharePreviewContext()
            {
                NodeId = nodeId,
                NewestVersionId = latestNodeVersion,
                Culture = culture
            };

            var encrypted = TPEncryptHelper.EncryptString(JsonConvert.SerializeObject(objToEncrypt), privateEncryptionKey);

            return string.Format("/umbraco/sharepreview/index/{0}", encrypted);
        }
    }
}
