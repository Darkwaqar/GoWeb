﻿using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    public class CommonSettings : ISettings
    {
        public CommonSettings()
        {
            SitemapCustomUrls = new List<string>();
            IgnoreLogWordlist = new List<string>();
        }
        /// <summary>
        /// Gets or sets a value indicating whether the contacts form should store in Database
        /// </summary>
        public bool StoreInDatabaseContactUsForm { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Vendor Account form should store in Database
        /// </summary>
        public bool StoreInDatabaseVendorAccountForm { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Appointment form should store in Database
        /// </summary>
        public bool StoreInDatabaseAppointmentForm { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the contacts form should have "Subject"
        /// </summary>
        public bool SubjectFieldOnContactUsForm { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the contacts form should use system email
        /// </summary>
        public bool UseSystemEmailForContactUsForm { get; set; }

        /// Gets or sets a value indicating whether the Appointment form should have "Subject"
        /// </summary>
        public bool SubjectFieldOnAppointmentForm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether stored procedures are enabled (should be used if possible)
        /// </summary>
        public bool UseStoredProceduresIfSupported { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use stored procedure (if supported) for loading categories (it's much faster in admin area with a large number of categories than the LINQ implementation)
        /// </summary>
        public bool UseStoredProcedureForLoadingCategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sitemap is enabled
        /// </summary>
        public bool SitemapEnabled { get; set; }

        /// <summary>
        /// Gets or sets the page size for sitemap
        /// </summary>
        public int SitemapPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include blog posts to sitemap
        /// </summary>
        public bool SitemapIncludeBlogPosts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include categories to sitemap
        /// </summary>
        public bool SitemapIncludeCategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include manufacturers to sitemap
        /// </summary>
        public bool SitemapIncludeManufacturers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include news to sitemap
        /// </summary>
        public bool SitemapIncludeNews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include products to sitemap
        /// </summary>
        public bool SitemapIncludeProducts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include product tags to sitemap
        /// </summary>
        public bool SitemapIncludeProductTags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include topics to sitemap
        /// </summary>
        public bool SitemapIncludeTopics { get; set; }
        /// <summary>
        /// A list of custom URLs to be added to sitemap.xml (include page names only)
        /// </summary>
        public List<string> SitemapCustomUrls { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display a warning if java-script is disabled
        /// </summary>
        public bool DisplayJavaScriptDisabledWarning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether full-text search is supported
        /// </summary>
        public bool UseFullTextSearch { get; set; }

        /// <summary>
        /// Gets or sets a Full-Text search mode
        /// </summary>
        public FulltextSearchMode FullTextMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 404 errors (page or file not found) should be logged
        /// </summary>
        public bool Log404Errors { get; set; }

        /// <summary>
        /// Gets or sets a breadcrumb delimiter used on the site
        /// </summary>
        public string BreadcrumbDelimiter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should render <meta http-equiv="X-UA-Compatible" content="IE=edge"/> tag
        /// </summary>
        public bool RenderXuaCompatible { get; set; }

        /// <summary>
        /// Gets or sets a value of "X-UA-Compatible" META tag
        /// </summary>
        public string XuaCompatibleValue { get; set; }

        /// <summary>
        /// Gets or sets ignore words (phrases) to be ignored when logging errors/messages
        /// </summary>
        public List<string> IgnoreLogWordlist { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether links generated by BBCode Editor should be opened in a new window
        /// </summary>
        public bool BbcodeEditorOpenLinksInNewWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "accept terms of service" links should be open in popup window. If disabled, then they'll be open on a new page.
        /// </summary>
        public bool PopupForTermsOfServiceLinks { get; set; }
    }
}