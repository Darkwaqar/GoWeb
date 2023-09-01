using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public partial class PushNotificationsSettingsModel : BaseNopModel
    {
        #region Ctor
        public PushNotificationsSettingsModel()
        {
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.PushNotifications.AllowGuestNotifications")]
        public bool AllowGuestNotifications { get; set; }
        public bool AllowGuestNotifications_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.PushNotifications.AuthDomain")]
        public string AuthDomain { get; set; }
        public bool AuthDomain_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.PushNotifications.DatabaseUrl")]
        public string DatabaseUrl { get; set; }
        public bool DatabaseUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.PushNotifications.ProjectId")]
        public string ProjectId { get; set; }
        public bool ProjectId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.PushNotifications.PushApiKey")]
        public string PublicApiKey { get; set; }
        public bool PublicApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.PushNotifications.SenderId")]
        public string SenderId { get; set; }
        public bool SenderId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.PushNotifications.StorageBucket")]
        public string StorageBucket { get; set; }
        public bool StorageBucket_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.PushNotifications.PrivateApiKey")]
        public string PrivateApiKey { get; set; }
        public bool PrivateApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.PushNotifications.NotificationsEnabled")]
        public bool Enabled { get; set; }
        public bool Enabled_OverrideForStore { get; set; }

        #endregion
    }
}