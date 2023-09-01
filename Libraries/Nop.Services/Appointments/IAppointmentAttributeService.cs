using Nop.Core.Domain.Appointments;
using System.Collections.Generic;

namespace Nop.Services.Appointments
{
    /// <summary>
    /// Appointment attribute service
    /// </summary>
    public partial interface IAppointmentAttributeService
    {
        #region Appointment attributes

        /// <summary>
        /// Deletes a Appointment attribute
        /// </summary>
        /// <param name="AppointmentAttribute">Appointment attribute</param>
        void DeleteAppointmentAttribute(AppointmentAttribute AppointmentAttribute);

        /// <summary>
        /// Gets all Appointment attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Appointment attributes</returns>
        IList<AppointmentAttribute> GetAllAppointmentAttributes(int storeId = 0, bool ignorAcl = false);

        /// <summary>
        /// Gets a Appointment attribute 
        /// </summary>
        /// <param name="AppointmentAttributeId">Appointment attribute identifier</param>
        /// <returns>Appointment attribute</returns>
        AppointmentAttribute GetAppointmentAttributeById(int AppointmentAttributeId);

        /// <summary>
        /// Inserts a Appointment attribute
        /// </summary>
        /// <param name="AppointmentAttribute">Appointment attribute</param>
        void InsertAppointmentAttribute(AppointmentAttribute AppointmentAttribute);

        /// <summary>
        /// Updates the Appointment attribute
        /// </summary>
        /// <param name="AppointmentAttribute">Appointment attribute</param>
        void UpdateAppointmentAttribute(AppointmentAttribute AppointmentAttribute);

        #endregion

        #region Appointment attribute values

        /// <summary>
        /// Deletes a Appointment attribute value
        /// </summary>
        /// <param name="AppointmentAttributeValue">Appointment attribute value</param>
        void DeleteAppointmentAttributeValue(AppointmentAttributeValue AppointmentAttributeValue);

        /// <summary>
        /// Gets Appointment attribute values by Appointment attribute identifier
        /// </summary>
        /// <param name="AppointmentAttributeId">The Appointment attribute identifier</param>
        /// <returns>Appointment attribute values</returns>
        IList<AppointmentAttributeValue> GetAppointmentAttributeValues(int AppointmentAttributeId);

        /// <summary>
        /// Gets a Appointment attribute value
        /// </summary>
        /// <param name="AppointmentAttributeValueId">Appointment attribute value identifier</param>
        /// <returns>Appointment attribute value</returns>
        AppointmentAttributeValue GetAppointmentAttributeValueById(int AppointmentAttributeValueId);

        /// <summary>
        /// Inserts a Appointment attribute value
        /// </summary>
        /// <param name="AppointmentAttributeValue">Appointment attribute value</param>
        void InsertAppointmentAttributeValue(AppointmentAttributeValue AppointmentAttributeValue);

        /// <summary>
        /// Updates the Appointment attribute value
        /// </summary>
        /// <param name="AppointmentAttributeValue">Appointment attribute value</param>
        void UpdateAppointmentAttributeValue(AppointmentAttributeValue AppointmentAttributeValue);

        #endregion
    }
}
