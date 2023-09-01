using Nop.Core.Domain.Interactive;
using System.Collections.Generic;

namespace Nop.Services.Messages
{
    /// <summary>
    /// InteractiveForm attribute service
    /// </summary>
    public partial interface IInteractiveFormAttributeService
    {
        #region InteractiveForm attributes

        /// <summary>
        /// Deletes a interactiveForm attribute
        /// </summary>
        /// <param name="interactiveFormAttribute">InteractiveForm attribute</param>
        void DeleteInteractiveFormAttribute(InteractiveFormAttribute interactiveFormAttribute);

        /// <summary>
        /// Gets all InteractiveForm attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>InteractiveForm attributes</returns>
        IList<InteractiveFormAttribute> GetAllInteractiveFormAttributes(int InteractiveFormId = 0);

        /// <summary>
        /// Gets a InteractiveForm attribute 
        /// </summary>
        /// <param name="interactiveFormAttributeId">InteractiveForm attribute identifier</param>
        /// <returns>InteractiveForm attribute</returns>
        InteractiveFormAttribute GetInteractiveFormAttributeById(int interactiveFormAttributeId);

        /// <summary>
        /// Inserts a InteractiveForm attribute
        /// </summary>
        /// <param name="interactiveFormAttribute">InteractiveForm attribute</param>
        void InsertInteractiveFormAttribute(InteractiveFormAttribute interactiveFormAttribute);

        /// <summary>
        /// Updates the InteractiveForm attribute
        /// </summary>
        /// <param name="interactiveFormAttribute">InteractiveForm attribute</param>
        void UpdateInteractiveFormAttribute(InteractiveFormAttribute interactiveFormAttribute);

        #endregion

        #region InteractiveForm attribute values

        /// <summary>
        /// Deletes a interactiveForm attribute value
        /// </summary>
        /// <param name="interactiveFormAttributeValue">InteractiveForm attribute value</param>
        void DeleteInteractiveFormAttributeValue(InteractiveFormAttributeValue interactiveFormAttributeValue);

        /// <summary>
        /// Gets interactiveForm attribute values by interactiveForm attribute identifier
        /// </summary>
        /// <param name="interactiveFormAttributeId">The interactiveForm attribute identifier</param>
        /// <returns>InteractiveForm attribute values</returns>
        IList<InteractiveFormAttributeValue> GetInteractiveFormAttributeValues(int interactiveFormAttributeId);

        /// <summary>
        /// Gets a interactiveForm attribute value
        /// </summary>
        /// <param name="interactiveFormAttributeValueId">InteractiveForm attribute value identifier</param>
        /// <returns>InteractiveForm attribute value</returns>
        InteractiveFormAttributeValue GetInteractiveFormAttributeValueById(int interactiveFormAttributeValueId);

        /// <summary>
        /// Inserts a interactiveForm attribute value
        /// </summary>
        /// <param name="interactiveFormAttributeValue">InteractiveForm attribute value</param>
        void InsertInteractiveFormAttributeValue(InteractiveFormAttributeValue interactiveFormAttributeValue);

        /// <summary>
        /// Updates the interactiveForm attribute value
        /// </summary>
        /// <param name="interactiveFormAttributeValue">InteractiveForm attribute value</param>
        void UpdateInteractiveFormAttributeValue(InteractiveFormAttributeValue interactiveFormAttributeValue);

        #endregion
    }
}
