using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Nop.Admin.Validators.Contact;

namespace Nop.Admin.Models.Contact
{
    [Validator(typeof(ContactAttributeValueValidator))]
    public partial class ContactAttributeValueModel : BaseNopEntityModel, ILocalizedModel<ContactAttributeValueLocalizedModel>
    {
        public ContactAttributeValueModel()
        {
            Locales = new List<ContactAttributeValueLocalizedModel>();
        }

        public int ContactAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.ContactAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.ContactAttributes.Values.Fields.ColorSquaresRgb")]
        [AllowHtml]
        public string ColorSquaresRgb { get; set; }
        public bool DisplayColorSquaresRgb { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.ContactAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.ContactAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<ContactAttributeValueLocalizedModel> Locales { get; set; }
    }

    public partial class ContactAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.ContactAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}