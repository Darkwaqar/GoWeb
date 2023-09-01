using Nop.Core;
using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.TelenorEasyPay.Services
{
    public interface IEasyPayPaymentStatusService
    {

        /// <summary>
        /// Inserts a EasyPay PaymentStatus
        /// </summary>
        /// <param name="easyPayPaymentStatus">EasyPay PaymentStatus</param>
        void InsertEasyPayPaymentStatus(EasyPayPaymentStatus easyPayPaymentStatus);

        /// <summary>
        /// Updates a EasyPay PaymentStatus
        /// </summary>
        /// <param name="easyPayPaymentStatus">EasyPay PaymentStatus</param>
        void UpdateEasyPayPaymentStatus(EasyPayPaymentStatus easyPayPaymentStatus);

        /// <summary>
        /// Deleted a EasyPay PaymentStatus
        /// </summary>
        /// <param name="easyPayPaymentStatus">EasyPay PaymentStatus</param>
        void DeleteEasyPayPaymentStatus(EasyPayPaymentStatus easyPayPaymentStatus);

        /// <summary>
        /// Deleted a EasyPay PaymentStatuss
        /// </summary>
        /// <param name="easyPayPaymentStatuss">EasyPay PaymentStatuss</param>
        void DeleteEasyPayPaymentStatuss(IList<EasyPayPaymentStatus> easyPayPaymentStatuss);

        /// <summary>
        /// Gets a EasyPay PaymentStatus by identifier
        /// </summary>
        /// <param name="easyPayPaymentStatusId">EasyPay PaymentStatus identifier</param>
        /// <returns>EasyPay PaymentStatus</returns>
        EasyPayPaymentStatus GetEasyPayPaymentStatusById(int easyPayPaymentStatusId);

        /// <summary>
        /// Get EasyPay PaymentStatuss by identifiers
        /// </summary>
        /// <param name="easyPayPaymentStatusIds">EasyPay PaymentStatus identifiers</param>
        /// <returns>EasyPay PaymentStatuss</returns>
        IList<EasyPayPaymentStatus> GetEasyPayPaymentStatussByIds(int[] easyPayPaymentStatusIds);

        /// <summary>
        /// Search EasyPay PaymentStatuss
        /// </summary>
        /// <param name="SearchName">Search by name</param>
        /// <param name="Showhidden">Is Action status Active  ;false to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>EasyPay PaymentStatuss</returns>
        IPagedList<EasyPayPaymentStatus> SearchEasyPayPaymentStatus(string AccountNumber = null, string Msisdn = null, string TransactionId = null, string TransactionStatus = "0",
            string PaymentToken = null, string ResponseCode = null, string PaymentMethod = "0",
            decimal TransactionAmount = 0,
            int OrderId = 0, int StoreId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);


        /// <summary>
        /// Gets all EasyPay PaymentStatus
        /// </summary>
        /// <returns>EasyPayPaymentStatus</returns>
        IList<EasyPayPaymentStatus> GetAllEasyPayPaymentStatus();
    }
}
