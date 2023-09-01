using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Validators.Common;

namespace Nop.Web.Models.Appointment
{
    [Validator(typeof(AppointmentValidator))]
    public partial class AppointmentModel : BaseNopModel
    {
        public AppointmentModel()
        {
            AppointmentAttributes = new List<AppointmentAttributeModel>();
        }

        [AllowHtml]
        [NopResourceDisplayName("Appointment.Email")]
        public string Email { get; set; }
        [AllowHtml]
        [NopResourceDisplayName("Appointment.Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        [NopResourceDisplayName("Appointment.Enquiry")]
        public string Enquiry { get; set; }

        [NopResourceDisplayName("Appointment.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }

        public string AppointmentAttributeInfo { get; set; }
        public string AppointmentAttributeXml { get; set; }

        public IList<AppointmentAttributeModel> AppointmentAttributes { get; set; }


        public partial class AppointmentAttributeModel : BaseNopModel
        {
            public AppointmentAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<AppointmentAttributeValueModel>();
            }
            public int Id { get; set; }

            public string Name { get; set; }

            public string DefaultValue { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }

            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<AppointmentAttributeValueModel> Values { get; set; }
            
        }

        public partial class AppointmentAttributeValueModel : BaseNopModel
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public int DisplayOrder { get; set; }

            public string ColorSquaresRgb { get; set; }

            public bool IsPreSelected { get; set; }
           
        }
    }
}