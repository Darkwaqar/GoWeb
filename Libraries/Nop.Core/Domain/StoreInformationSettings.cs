﻿using Nop.Core.Configuration;

namespace Nop.Core.Domain
{
    public class StoreInformationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether "powered by nopCommerce" text should be displayed.
        /// Please find more info at http://www.nopcommerce.com/copyrightremoval.aspx
        /// </summary>
        public bool HidePoweredByNopCommerce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether store is closed
        /// </summary>
        public bool StoreClosed { get; set; }

        /// <summary>
        /// Gets or sets a picture identifier of the logo. If 0, then the default one will be used
        /// </summary>
        public int LogoPictureId { get; set; }

        /// <summary>
        /// Gets or sets a default store theme
        /// </summary>
        public string DefaultStoreTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select a theme
        /// </summary>
        public bool AllowCustomerToSelectTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed in public store (used for debugging)
        /// </summary>
        public bool DisplayMiniProfilerInPublicStore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed only for users with access to the admin area
        /// </summary>
        public bool DisplayMiniProfilerForAdminOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should display warnings about the new EU cookie law
        /// </summary>
        public bool DisplayEuCookieLawWarning { get; set; }

        /// <summary>
        /// Gets or sets a value of Facebook page URL of the site
        /// </summary>
        public string FacebookLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Twitter page URL of the site
        /// </summary>
        public string TwitterLink { get; set; }

        /// <summary>
        /// Gets or sets a value of YouTube channel URL of the site
        /// </summary>
        public string YoutubeLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Google+ page URL of the site
        /// </summary>
        public string GooglePlusLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Instagram page URL of the site
        /// </summary>
        public string InstagramLink { get; set; }

        /// <summary>
        /// Gets or sets a value of LinkedIn page URL of the site
        /// </summary>
        public string LinkedInLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Pinterest page URL of the site
        /// </summary>
        public string PinterestLink { get; set; }


        /// <summary>
        /// Gets or sets a value of Play Store page URL of the application
        /// </summary>
        public string PlayStoreLink { get; set; }


        /// <summary>
        /// Gets or sets a value of App Store page URL of the application
        /// </summary>
        public string AppStoreLink { get; set; }
    }
}
