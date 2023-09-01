using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Admin.Validators.SMS;

namespace Nop.Admin.Models.SMS
{
    [Validator(typeof(NumberAccountValidator))]
    public partial class NumberAccountModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.NumberAccounts.Fields.Number")]
        [AllowHtml]
        public string Number { get; set; }

        [NopResourceDisplayName("Admin.Configuration.NumberAccounts.Fields.DisplayName")]
        [AllowHtml]
        public string DisplayName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.NumberAccounts.Fields.AccountSid")]
        [AllowHtml]
        public string AccountSid { get; set; }

        [NopResourceDisplayName("Admin.Configuration.NumberAccounts.Fields.AuthToken")]
        [AllowHtml]
        public string AuthToken { get; set; }

        [NopResourceDisplayName("Admin.Configuration.NumberAccounts.Fields.IsDefaultNumberAccount")]
        public bool IsDefaultNumberAccount { get; set; }

        [NopResourceDisplayName("Admin.Configuration.NumberAccounts.Fields.SendTestSMSTo")]
        [AllowHtml]
        public string SendTestSMSTo { get; set; }

    }
}