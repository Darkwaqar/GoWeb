using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Services.Events;
using Nop.Core.Domain.SMS;

namespace Nop.Services.SMS
{
    public partial class NumberAccountService : INumberAccountService
    {
        private readonly IRepository<NumberAccount> _numberAccountRepository;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="numberAccountRepository">Number account repository</param>
        /// <param name="eventPublisher">Event published</param>
        public NumberAccountService(IRepository<NumberAccount> numberAccountRepository,
            IEventPublisher eventPublisher)
        {
            this._numberAccountRepository = numberAccountRepository;
            this._eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Inserts an number account
        /// </summary>
        /// <param name="numberAccount">Number account</param>
        public virtual void InsertNumberAccount(NumberAccount numberAccount)
        {
            if (numberAccount == null)
                throw new ArgumentNullException("numberAccount");

            numberAccount.Number = CommonHelper.EnsureNotNull(numberAccount.Number);
            numberAccount.DisplayName = CommonHelper.EnsureNotNull(numberAccount.DisplayName);
            numberAccount.AccountSid = CommonHelper.EnsureNotNull(numberAccount.AccountSid);
            numberAccount.AuthToken = CommonHelper.EnsureNotNull(numberAccount.AuthToken);


            numberAccount.Number = numberAccount.Number.Trim();
            numberAccount.DisplayName = numberAccount.DisplayName.Trim();
            numberAccount.AccountSid = numberAccount.AccountSid.Trim();
            numberAccount.AuthToken = numberAccount.AuthToken.Trim();

            numberAccount.Number = CommonHelper.EnsureMaximumLength(numberAccount.Number, 255);
            numberAccount.DisplayName = CommonHelper.EnsureMaximumLength(numberAccount.DisplayName, 255);
            numberAccount.AccountSid = CommonHelper.EnsureMaximumLength(numberAccount.AccountSid, 255);
            numberAccount.AuthToken = CommonHelper.EnsureMaximumLength(numberAccount.AuthToken, 255);
           

            _numberAccountRepository.Insert(numberAccount);

            //event notification
            _eventPublisher.EntityInserted(numberAccount);
        }

        /// <summary>
        /// Updates an number account
        /// </summary>
        /// <param name="numberAccount">Number account</param>
        public virtual void UpdateNumberAccount(NumberAccount numberAccount)
        {
            if (numberAccount == null)
                throw new ArgumentNullException("numberAccount");

            numberAccount.Number = CommonHelper.EnsureNotNull(numberAccount.Number);
            numberAccount.DisplayName = CommonHelper.EnsureNotNull(numberAccount.DisplayName);
            numberAccount.AccountSid = CommonHelper.EnsureNotNull(numberAccount.AccountSid);
            numberAccount.AuthToken = CommonHelper.EnsureNotNull(numberAccount.AuthToken);

            numberAccount.Number = numberAccount.Number.Trim();
            numberAccount.DisplayName = numberAccount.DisplayName.Trim();
            numberAccount.AccountSid = numberAccount.AccountSid.Trim();
            numberAccount.AuthToken = numberAccount.AuthToken.Trim();

            numberAccount.Number = CommonHelper.EnsureMaximumLength(numberAccount.Number, 255);
            numberAccount.DisplayName = CommonHelper.EnsureMaximumLength(numberAccount.DisplayName, 255);
            numberAccount.AccountSid = CommonHelper.EnsureMaximumLength(numberAccount.AccountSid, 255);
            numberAccount.AuthToken = CommonHelper.EnsureMaximumLength(numberAccount.AuthToken, 255);

            _numberAccountRepository.Update(numberAccount);

            //event notification
            _eventPublisher.EntityUpdated(numberAccount);
        }

        /// <summary>
        /// Deletes an number account
        /// </summary>
        /// <param name="numberAccount">Number account</param>
        public virtual void DeleteNumberAccount(NumberAccount numberAccount)
        {
            if (numberAccount == null)
                throw new ArgumentNullException("numberAccount");

            if (GetAllNumberAccounts().Count == 1)
                throw new NopException("You cannot delete this number account. At least one account is required.");

            _numberAccountRepository.Delete(numberAccount);

            //event notification
            _eventPublisher.EntityDeleted(numberAccount);
        }

        /// <summary>
        /// Gets an number account by identifier
        /// </summary>
        /// <param name="numberAccountId">The number account identifier</param>
        /// <returns>Number account</returns>
        public virtual NumberAccount GetNumberAccountById(int numberAccountId)
        {
            if (numberAccountId == 0)
                return null;

            return _numberAccountRepository.GetById(numberAccountId);
        }

        /// <summary>
        /// Gets all number accounts
        /// </summary>
        /// <returns>Number accounts list</returns>
        public virtual IList<NumberAccount> GetAllNumberAccounts()
        {
            var query = from ea in _numberAccountRepository.Table
                        orderby ea.Id
                        select ea;
            var numberAccounts = query.ToList();
            return numberAccounts;
        }
    }
}
