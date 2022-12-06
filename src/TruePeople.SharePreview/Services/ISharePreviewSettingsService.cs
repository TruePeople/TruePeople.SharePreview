using TruePeople.SharePreview.Models;

namespace TruePeople.SharePreview.Services
{
    public interface ISharePreviewSettingsService
    {
        ShareablePreviewSettings GetSettings();
        bool UpdateSettings(ShareablePreviewSettings newSettings);
    }
}