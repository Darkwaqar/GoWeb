using System;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Appointments;

namespace Nop.Admin.Models.Appointments
{
    [Validator(typeof(ProductAppointmentValidator))]
    public partial class ProductAppointmentModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.ProductAppointment.Fields.Store")]
        public string StoreName { get; set; }
        [NopResourceDisplayName("Admin.Catalog.ProductAppointment.Fields.Product")]
        public int ProductId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.ProductAppointment.Fields.Product")]
        public string ProductName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductAppointment.Fields.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.ProductAppointment.Fields.Customer")]
        public string CustomerInfo { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.ReplyText")]
        public string ReplyText { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Admin.Catalog.ProductAppointment.Fields.AppointmentText")]
        public string AppointmentText { get; set; }
        
        [NopResourceDisplayName("Admin.Catalog.ProductAppointment.Fields.IsApproved")]
        public bool IsApproved { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductAppointment.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        //vendor
        public bool IsLoggedInAsVendor { get; set; }
    }
}