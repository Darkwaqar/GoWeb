using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Admin.Validators.SMS;

namespace Nop.Admin.Models.SMS
{
    [Validator(typeof(TestSMSTemplateValidator))]
    public partial class TestSMSTemplateModel : BaseNopEntityModel
    {
        public TestSMSTemplateModel()
        {
            Tokens = new List<string>();
        }

        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Test.Tokens")]
        public List<string> Tokens { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.SMSTemplates.Test.SendTo")]
        public string SendTo { get; set; }
    }
}