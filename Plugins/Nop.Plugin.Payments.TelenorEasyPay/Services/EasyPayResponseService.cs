using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Payments.TelenorEasyPay.Services
{
    public class EasyPayResponseService : IEasyPayResponseService
    {
        private readonly IRepository<EasyPayResponse> _easyPayRepository;
        private readonly IEventPublisher _eventPublisher;

        public EasyPayResponseService(IRepository<EasyPayResponse> easyPayRepository,
            IEventPublisher eventPublisher)
        {
            _easyPayRepository = easyPayRepository;
            _eventPublisher = eventPublisher;
        }


        public List<EasyPayResponse> GetOTCPayments(int paymentOflastDays)
        {
            DateTime previusDate = DateTime.UtcNow.AddDays(paymentOflastDays * -1);

            var query = from e in _easyPayRepository.Table
                        where e.IsOtcPayment &&
                        e.UTCTime >= previusDate
                        select e;

            var easyPayResponse = query.ToList();

            return easyPayResponse;
        }



        /// <summary>
        /// Inserts a EasyPay Response 
        /// </summary>
        /// <param name="easyPayResponse">EasyPay Response</param>        
        public virtual void InsertEasyPayResponse(EasyPayResponse easyPayResponse)
        {
            if (easyPayResponse == null)
                throw new ArgumentNullException("easyPayResponse");

            _easyPayRepository.Insert(easyPayResponse);

            //event notification
            _eventPublisher.EntityInserted(easyPayResponse);
        }

        /// <summary>
        /// Updates a EasyPay Response 
        /// </summary>
        /// <param name="easyPayResponse">EasyPay Response</param>
        public virtual void UpdateEasyPayResponse(EasyPayResponse easyPayResponse)
        {
            if (easyPayResponse == null)
                throw new ArgumentNullException("easyPayResponse");

            _easyPayRepository.Update(easyPayResponse);

            //event notification
            _eventPublisher.EntityUpdated(easyPayResponse);
        }

        /// <summary>
        /// Deleted a EasyPay Response 
        /// </summary>
        /// <param name="easyPayResponse">EasyPay Response</param>
        public virtual void DeleteEasyPayResponse(EasyPayResponse easyPayResponse)
        {
            if (easyPayResponse == null)
                throw new ArgumentNullException("easyPayResponse");

            _easyPayRepository.Delete(easyPayResponse);

            //event notification
            _eventPublisher.EntityDeleted(easyPayResponse);
        }

        /// <summary>
        /// Deleted a EasyPay Response s
        /// </summary>
        /// <param name="easyPayResponses">EasyPay Responses</param>
        public virtual void DeleteEasyPayResponses(IList<EasyPayResponse> easyPayResponses)
        {
            if (easyPayResponses == null)
                throw new ArgumentNullException("easyPayResponses");

            _easyPayRepository.Delete(easyPayResponses);

            //event notification
            foreach (var easyPayResponse in easyPayResponses)
            {
                _eventPublisher.EntityDeleted(easyPayResponse);
            }
        }

        /// <summary>
        /// Gets a EasyPay Response  by identifier
        /// </summary>
        /// <param name="easyPayResponseId">EasyPay Response identifier</param>
        /// <returns>EasyPay Response</returns>
        public virtual EasyPayResponse GetEasyPayResponseById(int easyPayResponseId)
        {
            if (easyPayResponseId == 0)
                return null;

            return _easyPayRepository.GetById(easyPayResponseId);

        }

        /// <summary>
        /// Get EasyPay Response s by identifiers
        /// </summary>
        /// <param name="easyPayResponseIds">EasyPay Response  identifiers</param>
        /// <returns>EasyPay Responses</returns>
        public virtual IList<EasyPayResponse> GetEasyPayResponsesByIds(int[] easyPayResponseIds)
        {
            if (easyPayResponseIds == null || easyPayResponseIds.Length == 0)
                return new List<EasyPayResponse>();

            var query = from qe in _easyPayRepository.Table
                        where easyPayResponseIds.Contains(qe.Id)
                        select qe;
            var easyPayResponses = query.ToList();
            //sort by passed identifiers
            var sortedEasyPayResponses = new List<EasyPayResponse>();
            foreach (int id in easyPayResponseIds)
            {
                var easyPayResponse = easyPayResponses.Find(x => x.Id == id);
                if (easyPayResponse != null)
                    sortedEasyPayResponses.Add(easyPayResponse);
            }
            return sortedEasyPayResponses;
        }

        /// <summary>
        /// Search fcmactions
        /// </summary>
        /// <param name="SearchName">Search by name</param>
        /// <param name="ShowHidden">Is Action status Active  ;false to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>EasyPay Responses</returns>
        /// <returns>Fcm item list</returns>
        public virtual IPagedList<EasyPayResponse> SearchEasyPayResponses(bool showOTC = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _easyPayRepository.Table;
            if (!showOTC)
                query = query.Where(v => v.IsOtcPayment);

            var easyPayResponses = new PagedList<EasyPayResponse>(query, pageIndex, pageSize);
            return easyPayResponses;
        }


        /// <summary>
        /// Gets all fcmapplication
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>EasyPayResponse</returns>
        public IList<EasyPayResponse> GetAllEasyPayResponse()
        {
            var query = _easyPayRepository.Table;
            var applications = query.ToList();
            return applications;
        }


    }
}
