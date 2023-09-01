using FluentValidation.Attributes;
using Nop.Admin.Validators.Interactive;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Admin.Models.Messages
{
    [Validator(typeof(InteractiveFormValidator))]
    public partial class InteractiveFormModel : BaseNopEntityModel, ILocalizedModel<InteractiveFormLocalizedModel>
    {
        public InteractiveFormModel()
        {
            Locales = new List<InteractiveFormLocalizedModel>();
            this.AvailableEmailAccounts = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.AvailableTokens")]
        public string AvailableTokens { get; set; }
        public IList<SelectListItem> AvailableEmailAccounts { get; set; }

        public IList<InteractiveFormLocalizedModel> Locales { get; set; }

    }

    public partial class InteractiveFormLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Fields.Body")]

        public string Body { get; set; }

    }

}