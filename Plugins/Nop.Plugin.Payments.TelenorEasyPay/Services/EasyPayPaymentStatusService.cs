using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Payments.TelenorEasyPay.Services
{
    public class EasyPayPaymentStatusService : IEasyPayPaymentStatusService
    {
        private readonly IRepository<EasyPayPaymentStatus> _EasyPayPaymentStatusRepository;
        private readonly IEventPublisher _eventPublisher;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="easyPayPaymentStatusRepository">EasyPay PaymentStatus repository</param>
        /// <param name="eventPublisher">Event published</param>

        public EasyPayPaymentStatusService(IRepository<EasyPayPaymentStatus> easyPayRepository,
            IEventPublisher eventPublisher)
        {
            _EasyPayPaymentStatusRepository = easyPayRepository;
            _eventPublisher = eventPublisher;
        }



        /// <summary>
        /// Inserts a EasyPay PaymentStatus 
        /// </summary>
        /// <param name="easyPayPaymentStatus">EasyPay PaymentStatus</param>        
        public virtual void InsertEasyPayPaymentStatus(EasyPayPaymentStatus easyPayPaymentStatus)
        {
            if (easyPayPaymentStatus == null)
                throw new ArgumentNullException("easyPayPaymentStatus");

            _EasyPayPaymentStatusRepository.Insert(easyPayPaymentStatus);

            //event notification
            _eventPublisher.EntityInserted(easyPayPaymentStatus);
        }

        /// <summary>
        /// Updates a EasyPay PaymentStatus 
        /// </summary>
        /// <param name="easyPayPaymentStatus">EasyPay PaymentStatus</param>
        public virtual void UpdateEasyPayPaymentStatus(EasyPayPaymentStatus easyPayPaymentStatus)
        {
            if (easyPayPaymentStatus == null)
                throw new ArgumentNullException("easyPayPaymentStatus");

            _EasyPayPaymentStatusRepository.Update(easyPayPaymentStatus);

            //event notification
            _eventPublisher.EntityUpdated(easyPayPaymentStatus);
        }

        /// <summary>
        /// Deleted a EasyPay PaymentStatus 
        /// </summary>
        /// <param name="easyPayPaymentStatus">EasyPay PaymentStatus</param>
        public virtual void DeleteEasyPayPaymentStatus(EasyPayPaymentStatus easyPayPaymentStatus)
        {
            if (easyPayPaymentStatus == null)
                throw new ArgumentNullException("easyPayPaymentStatus");

            _EasyPayPaymentStatusRepository.Delete(easyPayPaymentStatus);

            //event notification
            _eventPublisher.EntityDeleted(easyPayPaymentStatus);
        }

        /// <summary>
        /// Deleted a EasyPay PaymentStatus s
        /// </summary>
        /// <param name="easyPayPaymentStatuss">EasyPay PaymentStatuss</param>
        public virtual void DeleteEasyPayPaymentStatuss(IList<EasyPayPaymentStatus> easyPayPaymentStatuss)
        {
            if (easyPayPaymentStatuss == null)
                throw new ArgumentNullException("easyPayPaymentStatuss");

            _EasyPayPaymentStatusRepository.Delete(easyPayPaymentStatuss);

            //event notification
            foreach (var easyPayPaymentStatus in easyPayPaymentStatuss)
            {
                _eventPublisher.EntityDeleted(easyPayPaymentStatus);
            }
        }

        /// <summary>
        /// Gets a EasyPay PaymentStatus  by identifier
        /// </summary>
        /// <param name="easyPayPaymentStatusId">EasyPay PaymentStatus identifier</param>
        /// <returns>EasyPay PaymentStatus</returns>
        public virtual EasyPayPaymentStatus GetEasyPayPaymentStatusById(int easyPayPaymentStatusId)
        {
            if (easyPayPaymentStatusId == 0)
                return null;

            return _EasyPayPaymentStatusRepository.GetById(easyPayPaymentStatusId);

        }

        /// <summary>
        /// Get EasyPay PaymentStatus s by identifiers
        /// </summary>
        /// <param name="easyPayPaymentStatusIds">EasyPay PaymentStatus  identifiers</param>
        /// <returns>EasyPay PaymentStatuss</returns>
        public virtual IList<EasyPayPaymentStatus> GetEasyPayPaymentStatussByIds(int[] easyPayPaymentStatusIds)
        {
            if (easyPayPaymentStatusIds == null || easyPayPaymentStatusIds.Length == 0)
                return new List<EasyPayPaymentStatus>();

            var query = from qe in _EasyPayPaymentStatusRepository.Table
                        where easyPayPaymentStatusIds.Contains(qe.Id)
                        select qe;
            var easyPayPaymentStatuss = query.ToList();
            //sort by passed identifiers
            var sortedEasyPayPaymentStatuss = new List<EasyPayPaymentStatus>();
            foreach (int id in easyPayPaymentStatusIds)
            {
                var easyPayPaymentStatus = easyPayPaymentStatuss.Find(x => x.Id == id);
                if (easyPayPaymentStatus != null)
                    sortedEasyPayPaymentStatuss.Add(easyPayPaymentStatus);
            }
            return sortedEasyPayPaymentStatuss;
        }

        /// <summary>
        /// Search fcmactions
        /// </summary>
        /// <param name="SearchName">Search by name</param>
        /// <param name="ShowHidden">Is Action status Active  ;false to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>EasyPay PaymentStatuss</returns>
        /// <returns>Fcm item list</returns>
        public virtual IPagedList<EasyPayPaymentStatus> SearchEasyPayPaymentStatus(string AccountNumber = null, string Msisdn = null, string TransactionId = null, string TransactionStatus = "0",
            string PaymentToken = null, string ResponseCode = null, string PaymentMethod = "0",
            decimal TransactionAmount = 0,
            int OrderId = 0, int StoreId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _EasyPayPaymentStatusRepository.Table;
            if (!String.IsNullOrEmpty(AccountNumber))
                query = query.Where(qe => qe.AccountNumber.Contains(AccountNumber));
            if (!String.IsNullOrEmpty(Msisdn))
                query = query.Where(qe => qe.Msisdn.Contains(Msisdn));
            if (OrderId > 0)
                query = query.Where(qe => qe.OrderId == OrderId);
            if (StoreId > 0)
                query = query.Where(qe => qe.StoreId == StoreId);
            if (!PaymentMethod.Equals("0"))
                query = query.Where(qe => qe.PaymentMethod.Contains(PaymentMethod));
            if (!String.IsNullOrEmpty(PaymentToken))
                query = query.Where(qe => qe.PaymentToken.Contains(PaymentToken));
            if (!String.IsNullOrEmpty(ResponseCode))
                query = query.Where(qe => qe.ResponseCode.Contains(ResponseCode));
            if (TransactionAmount > 0)
                query = query.Where(qe => qe.TransactionAmount == TransactionAmount);
            if (!String.IsNullOrEmpty(TransactionId))
                query = query.Where(qe => qe.TransactionId == TransactionId);
            if (!TransactionStatus.Equals("0"))
                query = query.Where(qe => qe.TransactionStatus == TransactionStatus);

            query = query.OrderByDescending(qe => qe.UTCTime);
            var easyPayPaymentStatuss = new PagedList<EasyPayPaymentStatus>(query, pageIndex, pageSize);
            return easyPayPaymentStatuss;
        }


        /// <summary>
        /// Gets all fcmapplication
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>EasyPayPaymentStatus</returns>
        public IList<EasyPayPaymentStatus> GetAllEasyPayPaymentStatus()
        {
            var query = _EasyPayPaymentStatusRepository.Table;
            var applications = query.ToList();
            return applications;
        }
    }
}
