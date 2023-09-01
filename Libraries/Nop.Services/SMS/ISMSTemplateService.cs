using System.Collections.Generic;
using Nop.Core.Domain.SMS;

namespace Nop.Services.SMS
{
    /// <summary>
    /// SMS template service
    /// </summary>
    public partial interface ISMSTemplateService
    {
        /// <summary>
        /// Delete a message template
        /// </summary>
        /// <param name="messageTemplate">SMS template</param>
        void DeleteSMSTemplate(SMSTemplate messageTemplate);

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="messageTemplate">SMS template</param>
        void InsertSMSTemplate(SMSTemplate messageTemplate);

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="messageTemplate">SMS template</param>
        void UpdateSMSTemplate(SMSTemplate messageTemplate);

        /// <summary>
        /// Gets a message template by identifier
        /// </summary>
        /// <param name="messageTemplateId">SMS template identifier</param>
        /// <returns>SMS template</returns>
        SMSTemplate GetSMSTemplateById(int messageTemplateId);

        /// <summary>
        /// Gets a message template by name
        /// </summary>
        /// <param name="messageTemplateName">SMS template name</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>SMS template</returns>
        SMSTemplate GetSMSTemplateByName(string messageTemplateName, int storeId);

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <returns>SMS template list</returns>
        IList<SMSTemplate> GetAllSMSTemplates(int storeId);

        /// <summary>
        /// Create a copy of message template with all depended data
        /// </summary>
        /// <param name="messageTemplate">SMS template</param>
        /// <returns>SMS template copy</returns>
        SMSTemplate CopySMSTemplate(SMSTemplate messageTemplate);
    }
}
