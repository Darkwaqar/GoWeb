using FluentValidation.Attributes;
using Nop.Admin.Validators.Interactive;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Messages
{
    [Validator(typeof(InteractiveFormAttributeValidator))]
    public partial class InteractiveFormAttributeModel : BaseNopEntityModel, ILocalizedModel<InteractiveFormAttributeLocalizedModel>
    {
        public InteractiveFormAttributeModel()
        {
            Locales = new List<InteractiveFormAttributeLocalizedModel>();
        }
        public int InteractiveFormId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.SystemName")]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.RegexValidation")]
        public string RegexValidation { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.Style")]
        public string Style { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.Class")]
        public string Class { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.TextPrompt")]
        [AllowHtml]
        public string TextPrompt { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.AttributeControlType")]
        [AllowHtml]
        public string AttributeControlTypeName { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.MinLength")]
        [UIHint("Int32Nullable")]
        public int? ValidationMinLength { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.MaxLength")]
        [UIHint("Int32Nullable")]
        public int? ValidationMaxLength { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.FileAllowedExtensions")]
        public string ValidationFileAllowedExtensions { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.FileMaximumSize")]
        [UIHint("Int32Nullable")]
        public int? ValidationFileMaximumSize { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.DefaultValue")]
        public string DefaultValue { get; set; }

        public IList<InteractiveFormAttributeLocalizedModel> Locales { get; set; }

    }

    public partial class InteractiveFormAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.InteractiveForms.Attribute.Fields.Name")]
        public string Name { get; set; }

    }
}