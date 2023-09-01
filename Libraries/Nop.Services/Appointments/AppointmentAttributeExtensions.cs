using Nop.Core.Domain.Appointments;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Appointments
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class AppointmentAttributeExtensions
    {

        /// <summary>
        /// Gets a value indicating whether this Appointment attribute should have values
        /// </summary>
        /// <param name="AppointmentAttribute">Appointment attribute</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this AppointmentAttribute AppointmentAttribute)
        {
            if (AppointmentAttribute == null)
                return false;

            if (AppointmentAttribute.AttributeControlType == AttributeControlType.TextBox ||
                AppointmentAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                AppointmentAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                AppointmentAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute controle types support values
            return true;
        }

        /// <summary>
        /// A value indicating whether this Appointment attribute can be used as condition for some other attribute
        /// </summary>
        /// <param name="AppointmentAttribute">Appointment attribute</param>
        /// <returns>Result</returns>
        public static bool CanBeUsedAsCondition(this AppointmentAttribute AppointmentAttribute)
        {
            if (AppointmentAttribute == null)
                return false;

            if (AppointmentAttribute.AttributeControlType == AttributeControlType.ReadonlyCheckboxes ||
                AppointmentAttribute.AttributeControlType == AttributeControlType.TextBox ||
                AppointmentAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                AppointmentAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                AppointmentAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute controle types support it
            return true;
        }
    }
}
