using Microsoft.AspNetCore.Mvc;
using TruePeople.SharePreview.Models;
using TruePeople.SharePreview.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace TruePeople.SharePreview.Controllers.ApiControllers
{
    public class ShareablePreviewSettingsApiController : UmbracoAuthorizedApiController
    {
        private readonly ISharePreviewSettingsService _shareablePreviewSettingsService;

        public ShareablePreviewSettingsApiController(ISharePreviewSettingsService shareablePreviewSettingsService)
        {
            _shareablePreviewSettingsService = shareablePreviewSettingsService;
        }

        [HttpGet]
        public ShareablePreviewSettings GetSettings()
        {
            return _shareablePreviewSettingsService.GetSettings();
        }

        [HttpPost]
        public bool SaveSettings(ShareablePreviewSettings settings)
        {
            return _shareablePreviewSettingsService.UpdateSettings(settings);
        }
    }
}
