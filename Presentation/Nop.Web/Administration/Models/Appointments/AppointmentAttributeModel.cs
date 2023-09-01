using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Admin.Validators.Appointments;

namespace Nop.Admin.Models.Appointments
{
    [Validator(typeof(AppointmentAttributeValidator))]
    public partial class AppointmentAttributeModel : BaseNopEntityModel, ILocalizedModel<AppointmentAttributeLocalizedModel>
    {
        public AppointmentAttributeModel()
        {
            Locales = new List<AppointmentAttributeLocalizedModel>();

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.TextPrompt")]
        [AllowHtml]
        public string TextPrompt { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.AttributeControlType")]
        [AllowHtml]
        public string AttributeControlTypeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.MinLength")]
        [UIHint("Int32Nullable")]
        public int? ValidationMinLength { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.MaxLength")]
        [UIHint("Int32Nullable")]
        public int? ValidationMaxLength { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.FileAllowedExtensions")]
        public string ValidationFileAllowedExtensions { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.FileMaximumSize")]
        [UIHint("Int32Nullable")]
        public int? ValidationFileMaximumSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.DefaultValue")]
        public string DefaultValue { get; set; }

        public IList<AppointmentAttributeLocalizedModel> Locales { get; set; }

        //condition
        public bool ConditionAllowed { get; set; }
        public ConditionModel ConditionModel { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        //ACL (customer roles)
        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.AclCustomerRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }
    }

    public partial class ConditionModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Condition.EnableCondition")]
        public bool EnableCondition { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Condition.Attributes")]
        public int SelectedAttributeId { get; set; }

        public IList<AttributeConditionModel> ConditionAttributes { get; set; }
    }
    public partial class AttributeConditionModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<SelectListItem> Values { get; set; }

        public string SelectedValueId { get; set; }
    }
    public partial class AppointmentAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.AppointmentAttributes.Fields.TextPrompt")]
        [AllowHtml]
        public string TextPrompt { get; set; }

    }
}