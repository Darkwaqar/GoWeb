using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.SMS;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.SMS
{
    [Validator(typeof(SMSTemplateValidator))]
    public partial class SMSTemplateModel : BaseNopEntityModel, ILocalizedModel<SMSTemplateLocalizedModel>
    {
        public SMSTemplateModel()
        {
            Locales = new List<SMSTemplateLocalizedModel>();
            AvailableNumberAccounts = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }


        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.BccNumberAddresses")]
        [AllowHtml]
        public string BccNumberAddresses { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.IsActive")]
        [AllowHtml]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.DelayBeforeSend")]
        [UIHint("Int32Nullable")]
        public int? DelayBeforeSend { get; set; }
        public int DelayPeriodId { get; set; }

        public bool HasAttachedDownload { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.AttachedDownload")]
        [UIHint("Download")]
        public int AttachedDownloadId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.NumberAccount")]
        public int NumberAccountId { get; set; }
        public IList<SelectListItem> AvailableNumberAccounts { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        //comma-separated list of stores used on the list page
        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.LimitedToStores")]
        public string ListOfStores { get; set; }



        public IList<SMSTemplateLocalizedModel> Locales { get; set; }
    }

    public partial class SMSTemplateLocalizedModel : ILocalizedModelLocal
    {
        public SMSTemplateLocalizedModel()
        {
            AvailableNumberAccounts = new List<SelectListItem>();
        }
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.BccNumberAddresses")]
        [AllowHtml]
        public string BccNumberAddresses { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Fields.NumberAccount")]
        public int NumberAccountId { get; set; }
        public IList<SelectListItem> AvailableNumberAccounts { get; set; }
    }
}