using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Appointment
{
    public class CustomerProductAppointmentModel : BaseNopModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSeName { get; set; }
        public string Title { get; set; }
        public string AppointmentText { get; set; }
        public string ReplyText { get; set; }
        public int Rating { get; set; }
        public string WrittenOnStr { get; set; }
        public string ApprovalStatus { get; set; }
    }

    public class CustomerProductAppointmentsModel : BaseNopModel
    {
        public CustomerProductAppointmentsModel()
        {
            ProductAppointments = new List<CustomerProductAppointmentModel>();
        }

        public IList<CustomerProductAppointmentModel> ProductAppointments { get; set; }
        public PagerModel PagerModel { get; set; }

        #region Nested class

        /// <summary>
        /// Class that has only page for route value. Used for (My Account) My Product Appointments pagination
        /// </summary>
        public partial class CustomerProductAppointmentsRouteValues : IRouteValues
        {
            public int page { get; set; }
        }

        #endregion
    }
}