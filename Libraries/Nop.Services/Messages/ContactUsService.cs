using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Contact;
using Nop.Data;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Messages
{
    /// <summary>
    /// ContactUs interface
    /// </summary>
    public partial class ContactUsService : IContactUsService
    {

        private readonly IRepository<ContactUs> _contactusRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;

        public ContactUsService(
            IRepository<ContactUs> contactusRepository,
            IEventPublisher eventPublisher,
            IDbContext dbContext,
            IDataProvider dataProvider,
            CommonSettings commonSettings)
        {
            this._contactusRepository = contactusRepository;
            this._eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._commonSettings = commonSettings;
        }
        /// <summary>
        /// Deletes a contactus item
        /// </summary>
        /// <param name="contactus">ContactUs item</param>
        public virtual void DeleteContactUs(ContactUs contactus)
        {
            if (contactus == null)
                throw new ArgumentNullException("contactus");

            _contactusRepository.Delete(contactus);

            //event notification
            _eventPublisher.EntityDeleted(contactus);

        }

        /// <summary>
        /// Update a contactus item
        /// </summary>
        /// <param name="contactus">ContactUs item</param>
        public virtual void UpdateContactUs(ContactUs contactus)
        {
            if (contactus == null)
                throw new ArgumentNullException(nameof(contactus));

            _contactusRepository.Update(contactus);

            //event notification
            _eventPublisher.EntityUpdated(contactus);
        }

        /// <summary>
        /// Deletes a contactus items
        /// </summary>
        /// <param name="contactus">ContactUs items</param>
        public virtual void DeleteContactUss(IList<ContactUs> contactuss)
        {
            if (contactuss == null)
                throw new ArgumentNullException("contactuss");

            _contactusRepository.Delete(contactuss);
        }

        /// <summary>
        /// Clears table
        /// </summary>
        public virtual void ClearTable()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //although it's not a stored procedure we use it to ensure that a database supports them
                //we cannot wait until EF team has it implemented - http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/suggestions/1015357-batch-cud-support


                //do all databases support "Truncate command"?
                string logTableName = _dbContext.GetTableName<ContactUs>();
                _dbContext.ExecuteSqlCommand(String.Format("TRUNCATE TABLE [{0}]", logTableName));
            }
            else
            {
                var contactusForms = _contactusRepository.Table.ToList();
                foreach (var contactusItem in contactusForms)
                    _contactusRepository.Delete(contactusItem);
            }
        }

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
        public virtual IPagedList<ContactUs> GetAllContactUs(DateTime? fromUtc = null, DateTime? toUtc = null,
            string email = "", int vendorId = 0, int customerId = 0, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {

            var query = _contactusRepository.Table;
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (vendorId != 0)
                query = query.Where(l => vendorId == l.VendorId);
            if (customerId != 0)
                query = query.Where(l => customerId == l.CustomerId);
            if (storeId != 0)
                query = query.Where(l => storeId == l.StoreId);
            if (!String.IsNullOrEmpty(email))
                query = query.Where(l => l.Email.Contains(email));


            query = query.OrderByDescending(x => x.CreatedOnUtc);
            var contactus = new PagedList<ContactUs>(query, pageIndex, pageSize);
            return contactus;
        }

        /// <summary>
        /// Gets a contactus item
        /// </summary>
        /// <param name="contactUsId">ContactUs item identifier</param>
        /// <returns>ContactUs item</returns>
        public virtual ContactUs GetContactUsById(int contactUsId)
        {
            return _contactusRepository.GetById(contactUsId);
        }


        /// <summary>
        /// Get contactus by identifiers
        /// </summary>
        /// <param name="contactusIds">ContactUs item identifiers</param>
        /// <returns>ContactUs items</returns>
        public virtual IList<ContactUs> GetContactUsByIds(int[] contactUsIds)
        {
            if (contactUsIds == null || contactUsIds.Length == 0)
                return new List<ContactUs>();

            var query = from l in _contactusRepository.Table
                        where contactUsIds.Contains(l.Id)
                        select l;
            var logItems = query.ToList();
            //sort by passed identifiers
            var sortedLogItems = new List<ContactUs>();
            foreach (int id in contactUsIds)
            {
                var log = logItems.Find(x => x.Id == id);
                if (log != null)
                    sortedLogItems.Add(log);
            }
            return sortedLogItems;
        }

        /// <summary>
        /// Inserts a contactus item
        /// </summary>
        /// <param name="contactus">ContactUs</param>
        /// <returns>A contactus item</returns>
        public virtual void InsertContactUs(ContactUs contactus)
        {
            if (contactus == null)
                throw new ArgumentNullException("contactus");

            _contactusRepository.Insert(contactus);

            //event notification
            _eventPublisher.EntityInserted(contactus);

        }

    }
}
