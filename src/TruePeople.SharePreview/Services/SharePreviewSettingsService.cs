using Microsoft.Extensions.Logging;
using System;
using System.Xml.Serialization;
using TruePeople.SharePreview.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace TruePeople.SharePreview.Services
{
    public class SharePreviewSettingsService : ISharePreviewSettingsService
    {
        private readonly string _settingsCacheKey = "ShareablePreviewSettings";
        private readonly IAppPolicyCache _runtimeCache;
        private readonly ILogger<SharePreviewSettingsService> _logger;
        private readonly IScopeProvider _scopeProvider;

        public SharePreviewSettingsService(
            ILogger<SharePreviewSettingsService> logger,
            IScopeProvider scopeProvider,
            AppCaches appCaches)
        {
            _logger = logger;
            _scopeProvider = scopeProvider;
            _runtimeCache = appCaches.RuntimeCache;
        }

        public ShareablePreviewSettings GetSettings()
        {
            var settings = _runtimeCache.GetCacheItem<ShareablePreviewSettings>(_settingsCacheKey);

            if (settings == null)
            {
                settings = ReadSettings();
                _runtimeCache.InsertCacheItem(_settingsCacheKey, () => settings, DateTime.Now.AddHours(1).TimeOfDay);
            }
            return settings;
        }


        public bool UpdateSettings(ShareablePreviewSettings newSettings)
        {
            if (SetSettings(newSettings))
            {
                _runtimeCache.InsertCacheItem(_settingsCacheKey, () => newSettings, DateTime.Now.AddHours(1).TimeOfDay);
                return true;
            }
            else
            {
                return false;
            }
        }

        private ShareablePreviewSettings ReadSettings()
        {
            try
            {
                using var scope = _scopeProvider.CreateScope();
                var settings = scope.Database.First<ShareablePreviewSettings>("SELECT * FROM ShareablePreviewSettings");
                scope.Complete();
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured whilst reading out the settings.");
                return null;
            }
        }

        private bool SetSettings(ShareablePreviewSettings newSettings)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope();
                scope.Complete();
                return scope.Database.Update(newSettings, newSettings.Id) == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured whilst saving the settings.");
                return false;
            }
        }
    }
}