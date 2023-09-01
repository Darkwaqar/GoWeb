using Nop.Core.Domain.Appointments;
using System.Collections.Generic;

namespace Nop.Services.Appointments
{
    /// <summary>
    /// Appointment attribute parser interface
    /// </summary>
    public partial interface IAppointmentAttributeParser
    {
        /// <summary>
        /// Gets selected Appointment attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected Appointment attributes</returns>
        IList<AppointmentAttribute> ParseAppointmentAttributes(string attributesXml);

        /// <summary>
        /// Get Appointment attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Appointment attribute values</returns>
        IList<AppointmentAttributeValue> ParseAppointmentAttributeValues(string attributesXml);

        /// <summary>
        /// Gets selected Appointment attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="AppointmentAttributeId">Appointment attribute identifier</param>
        /// <returns>Appointment attribute value</returns>
        IList<string> ParseValues(string attributesXml, int AppointmentAttributeId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ca">Appointment attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddAppointmentAttribute(string attributesXml, AppointmentAttribute ca, string value);

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="attribute">Appointment attribute</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>Result</returns>
        bool? IsConditionMet(AppointmentAttribute attribute, string selectedAttributesXml);

        /// <summary>
        /// Remove an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attribute">Appointment attribute</param>
        /// <returns>Updated result (XML format)</returns>
        string RemoveAppointmentAttribute(string attributesXml, AppointmentAttribute attribute);
    }
}
