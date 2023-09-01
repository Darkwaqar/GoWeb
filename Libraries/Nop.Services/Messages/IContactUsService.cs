using Nop.Core;
using Nop.Core.Domain.Contact;
using System;
using System.Collections.Generic;

namespace Nop.Services.Messages
{
    /// <summary>
    /// ContactUs interface
    /// </summary>
    public partial interface IContactUsService
    {
        /// <summary>
        /// Deletes a contactus item
        /// </summary>
        /// <param name="contactus">ContactUs item</param>
        void DeleteContactUs(ContactUs contactus);

        /// <summary>
        /// Update a contactus item
        /// </summary>
        /// <param name="contactus">ContactUs item</param>
        void UpdateContactUs(ContactUs contactus);

        /// <summary>
        /// Deletes a contactus items
        /// </summary>
        /// <param name="contactus">ContactUs items</param>
        void DeleteContactUss(IList<ContactUs> contactuss);

        /// <summary>
        /// Clears table
        /// </summary>
        void ClearTable();

        /// <summary>
        /// Gets all contactUs items
        /// </summary>
        /// <param name="fromUtc">ContactUs item creation from; null to load all records</param>
        /// <param name="toUtc">ContactUs item creation to; null to load all records</param>
        /// <param name="email">email</param>
        /// <param name="vendorId">vendorId; null to load all records</param>
        /// <param name="customerId">customerId; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>ContactUs items</returns>
        IPagedList<ContactUs> GetAllContactUs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string email = "", int vendorId = 0, int customerId = 0, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a contactus item
        /// </summary>
        /// <param name="contactUsId">ContactUs item identifier</param>
        /// <returns>ContactUs item</returns>
        ContactUs GetContactUsById(int contactUsId);


        /// <summary>
        /// Get contactus by identifiers
        /// </summary>
        /// <param name="contactusIds">ContactUs item identifiers</param>
        /// <returns>ContactUs items</returns>
        IList<ContactUs> GetContactUsByIds(int[] contactUsIds);

        /// <summary>
        /// Inserts a contactus item
        /// </summary>
        /// <param name="contactus">ContactUs</param>
        /// <returns>A contactus item</returns>
        void InsertContactUs(ContactUs contactus);

    }
}
