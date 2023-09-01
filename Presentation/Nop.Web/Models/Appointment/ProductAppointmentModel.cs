using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Validators.Appointments;

namespace Nop.Web.Models.Appointment
{
    [Validator(typeof(ProductAppointmentsValidator))]
    public partial class ProductAppointmentsModel : BaseNopModel
    {
        public ProductAppointmentsModel()
        {
            AddProductAppointment = new AddProductAppointmentModel();
        }
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public AddProductAppointmentModel AddProductAppointment { get; set; }
    }

    public partial class ProductAppointmentModel : BaseNopModel
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public bool AllowViewingProfiles { get; set; }

        public string Title { get; set; }

        public string AppointmentText { get; set; }

        public string ReplyText { get; set; }

        public string WrittenOnStr { get; set; }
    }

    public partial class AddProductAppointmentModel : BaseNopModel
    {

        [AllowHtml]
        [NopResourceDisplayName("Appointments.Fields.AppointmentText")]
        public string AppointmentText { get; set; }

        [NopResourceDisplayName("Appointments.Fields.Rating")]
        public int Rating { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool CanCurrentCustomerLeaveAppointment { get; set; }
        public bool SuccessfullyAdded { get; set; }
        public string Result { get; set; }
    }
}