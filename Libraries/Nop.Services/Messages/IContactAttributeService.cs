using Nop.Core.Domain.Contact;
using System.Collections.Generic;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Contact attribute service
    /// </summary>
    public partial interface IContactAttributeService
    {
        #region Contact attributes

        /// <summary>
        /// Deletes a contact attribute
        /// </summary>
        /// <param name="contactAttribute">Contact attribute</param>
        void DeleteContactAttribute(ContactAttribute contactAttribute);

        /// <summary>
        /// Gets all Contact attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Contact attributes</returns>
        IList<ContactAttribute> GetAllContactAttributes(int storeId = 0, bool ignorAcl = false);

        /// <summary>
        /// Gets a Contact attribute 
        /// </summary>
        /// <param name="contactAttributeId">Contact attribute identifier</param>
        /// <returns>Contact attribute</returns>
        ContactAttribute GetContactAttributeById(int contactAttributeId);

        /// <summary>
        /// Inserts a Contact attribute
        /// </summary>
        /// <param name="contactAttribute">Contact attribute</param>
        void InsertContactAttribute(ContactAttribute contactAttribute);

        /// <summary>
        /// Updates the Contact attribute
        /// </summary>
        /// <param name="contactAttribute">Contact attribute</param>
        void UpdateContactAttribute(ContactAttribute contactAttribute);

        #endregion

        #region Contact attribute values

        /// <summary>
        /// Deletes a contact attribute value
        /// </summary>
        /// <param name="contactAttributeValue">Contact attribute value</param>
        void DeleteContactAttributeValue(ContactAttributeValue contactAttributeValue);

        /// <summary>
        /// Gets contact attribute values by contact attribute identifier
        /// </summary>
        /// <param name="contactAttributeId">The contact attribute identifier</param>
        /// <returns>Contact attribute values</returns>
        IList<ContactAttributeValue> GetContactAttributeValues(int contactAttributeId);

        /// <summary>
        /// Gets a contact attribute value
        /// </summary>
        /// <param name="contactAttributeValueId">Contact attribute value identifier</param>
        /// <returns>Contact attribute value</returns>
        ContactAttributeValue GetContactAttributeValueById(int contactAttributeValueId);

        /// <summary>
        /// Inserts a contact attribute value
        /// </summary>
        /// <param name="contactAttributeValue">Contact attribute value</param>
        void InsertContactAttributeValue(ContactAttributeValue contactAttributeValue);

        /// <summary>
        /// Updates the contact attribute value
        /// </summary>
        /// <param name="contactAttributeValue">Contact attribute value</param>
        void UpdateContactAttributeValue(ContactAttributeValue contactAttributeValue);

        #endregion
    }
}
