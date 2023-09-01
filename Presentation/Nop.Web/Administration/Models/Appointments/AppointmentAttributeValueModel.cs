using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Nop.Admin.Validators.Appointments;

namespace Nop.Admin.Models.Appointments
{
    [Validator(typeof(AppointmentAttributeValueValidator))]
    public partial class AppointmentAttributeValueModel : BaseNopEntityModel, ILocalizedModel<AppointmentAttributeValueLocalizedModel>
    {
        public AppointmentAttributeValueModel()
        {
            Locales = new List<AppointmentAttributeValueLocalizedModel>();
        }

        public int AppointmentAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Values.Fields.ColorSquaresRgb")]
        [AllowHtml]
        public string ColorSquaresRgb { get; set; }
        public bool DisplayColorSquaresRgb { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<AppointmentAttributeValueLocalizedModel> Locales { get; set; }
    }

    public partial class AppointmentAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}