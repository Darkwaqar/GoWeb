using Nop.Core;
using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.TelenorEasyPay.Services
{
    public interface IEasyPayResponseService
    {

        /// <summary>
        /// Get OTC Payments.
        /// </summary>
        /// <param name="record">Payment of last couple of days</param>
        List<EasyPayResponse> GetOTCPayments(int paymentOflastDays);


        /// <summary>
        /// Inserts a easyPayResponse
        /// </summary>
        /// <param name="easyPayResponse">EasyPay Response</param>
        void InsertEasyPayResponse(EasyPayResponse easyPayResponse);

        /// <summary>
        /// Updates a easyPayResponse
        /// </summary>
        /// <param name="easyPayResponse">EasyPay Response</param>
        void UpdateEasyPayResponse(EasyPayResponse easyPayResponse);

        /// <summary>
        /// Deleted a easyPayResponse
        /// </summary>
        /// <param name="easyPayResponse">EasyPay Response</param>
        void DeleteEasyPayResponse(EasyPayResponse easyPayResponse);

        /// <summary>
        /// Deleted a easyPayResponses
        /// </summary>
        /// <param name="easyPayResponses">EasyPay Responses</param>
        void DeleteEasyPayResponses(IList<EasyPayResponse> easyPayResponses);

        /// <summary>
        /// Gets a easyPayResponse by identifier
        /// </summary>
        /// <param name="easyPayResponseId">EasyPay Response identifier</param>
        /// <returns>EasyPay Response</returns>
        EasyPayResponse GetEasyPayResponseById(int easyPayResponseId);

        /// <summary>
        /// Get easyPayResponses by identifiers
        /// </summary>
        /// <param name="easyPayResponseIds">easyPayResponse identifiers</param>
        /// <returns>EasyPay Responses</returns>
        IList<EasyPayResponse> GetEasyPayResponsesByIds(int[] easyPayResponseIds);

        /// <summary>
        /// Search easyPayResponses
        /// </summary>
        /// <param name="SearchName">Search by name</param>
        /// <param name="Showhidden">Is Action status Active  ;false to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>EasyPay Responses</returns>
        IPagedList<EasyPayResponse> SearchEasyPayResponses( bool showOtc = false, int pageIndex = 0, int pageSize = int.MaxValue);


        /// <summary>
        /// Gets all easyPayResponse
        /// </summary>
        /// <returns>EasyPayResponse</returns>
        IList<EasyPayResponse> GetAllEasyPayResponse();
    }
}
