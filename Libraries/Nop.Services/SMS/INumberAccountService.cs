using System.Collections.Generic;
using Nop.Core.Domain.SMS;

namespace Nop.Services.SMS
{
    public partial interface INumberAccountService
    {
        /// <summary>
        /// Inserts an number account
        /// </summary>
        /// <param name="numberAccount">Number account</param>
        void InsertNumberAccount(NumberAccount numberAccount);

        /// <summary>
        /// Updates an number account
        /// </summary>
        /// <param name="numberAccount">Number account</param>
        void UpdateNumberAccount(NumberAccount numberAccount);

        /// <summary>
        /// Deletes an number account
        /// </summary>
        /// <param name="numberAccount">Number account</param>
        void DeleteNumberAccount(NumberAccount numberAccount);

        /// <summary>
        /// Gets an number account by identifier
        /// </summary>
        /// <param name="numberAccountId">The number account identifier</param>
        /// <returns>Number account</returns>
        NumberAccount GetNumberAccountById(int numberAccountId);

        /// <summary>
        /// Gets all number accounts
        /// </summary>
        /// <returns>Number accounts list</returns>
        IList<NumberAccount> GetAllNumberAccounts();
    }
}
