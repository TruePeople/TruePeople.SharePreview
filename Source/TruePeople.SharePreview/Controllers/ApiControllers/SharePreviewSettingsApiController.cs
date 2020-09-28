using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TruePeople.SharePreview.Helpers;
using TruePeople.SharePreview.Models;
using TruePeople.SharePreview.Services;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;

namespace TruePeople.SharePreview.Controllers.ApiControllers
{
    public class ShareablePreviewSettingsApiController : UmbracoAuthorizedApiController
    {
        private readonly SharePreviewSettingsService _shareablePreviewSettingsService;

        public ShareablePreviewSettingsApiController(SharePreviewSettingsService shareablePreviewSettingsService)
        {
            _shareablePreviewSettingsService = shareablePreviewSettingsService;
        }

        [HttpGet]
        public ShareSettings GetSettings()
        {
            return _shareablePreviewSettingsService.GetSettings();
        }

        [HttpPost]
        public bool SaveSettings(ShareSettings settings)
        {
            if (_shareablePreviewSettingsService.UpdateSettings(settings))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
