using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using System.Collections.Generic;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor service interface
    /// </summary>
    public partial interface IVendorService
    {

        #region Vendor

        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor</returns>
        Vendor GetVendorById(int vendorId);

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        void DeleteVendor(Vendor vendor);

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        IPagedList<Vendor> GetAllVendorsLimitedToStore(string name = "", int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);


        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        IPagedList<Vendor> GetAllVendors(string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        void InsertVendor(Vendor vendor);

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        void UpdateVendor(Vendor vendor);



        /// <summary>
        /// Gets a vendor note note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>Vendor note</returns>
        VendorNote GetVendorNoteById(int vendorNoteId);

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        void DeleteVendorNote(VendorNote vendorNote);

        #endregion

        #region Extended Vendor
        IPagedList<Vendor> GetAllFeaturedVendors(int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<Vendor> SearchVendors(string search = "",
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);


        /// <summary>
        /// Gets a vendor by Category identifier
        /// </summary>
        /// <param name="categoryId">Vendor identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        IPagedList<Vendor> GetVendorByCategoryID( int categoryId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);



        #region VendorBanner

        /// <summary>
        /// Gets a vendor banners by vendor identifier
        /// </summary>
        /// <param name="vendorId">The vendor identifier</param>
        /// <returns>Product pictures</returns>
        IList<VendorBanner> GetVendorBannersByVendorId(int vendorId);


        /// <summary>
        /// Updates a vendor banner
        /// </summary>
        /// <param name="vendorBanner">vendor banner</param>
        void UpdateVendorBanner(VendorBanner vendorBanner);

        void InsertVendorBanner(VendorBanner vendorBanner);

        void DeleteVendorBanner(VendorBanner vendorBanner);

        VendorBanner GetVendorBannerById(int vendorbannerId);

        #endregion

        #region MobileAppSettings
        MobileAppSetting GetMobileAppSetting(int VendorId);

        void InsertMobileAppSetting(MobileAppSetting MobileAppSetting);

        void UpdateMobileAppSetting(MobileAppSetting MobileAppSetting);

        #endregion

        #region Socials
        SocialLinks GetSocialLinks(int VendorId);

        void InsertSocialLinks(SocialLinks SocialLinks);

        void UpdateSocialLinks(SocialLinks SocialLinks);

        #endregion

        #region Paymentsinfo

        VendorPaymentInfo GetVendorPaymentInfo(int VendorId);

        void InsertVendorPaymentInfo(VendorPaymentInfo VendorPaymentInfo);

        void UpdateVendorPaymentInfo(VendorPaymentInfo VendorPaymentInfo);

        #endregion

        #region Payouts
        IPagedList<VendorPayout> GetVendorPayouts(int VendorId, PayoutStatus? PayoutStatus, int PageNumber = 1, int PageSize = int.MaxValue);

        VendorPayout GetVendorPayout(int VendorPayoutId);

        IList<VendorPayout> GetVendorPayoutsByOrder(int OrderId);

        void SaveVendorPayout(VendorPayout VendorPayout);

        void DeleteVendorPayout(VendorPayout VendorPayout);

        #endregion

        #region Review

        VendorReview GetVendorReview(int VendorReviewId);

        VendorReview GetVendorReview(int VendorId, int CustomerId, int OrderId, int ProductId);

        IPagedList<VendorReview> GetVendorReviews(int? VendorId, int? CustomerId, bool? IsApproved, int PageNumber = 1, int PageSize = int.MaxValue);

        void SaveVendorReview(VendorReview VendorReview);

        void DeleteVendorReview(VendorReview VendorReview);

        bool IsVendorReviewDone(int VendorId, int CustomerId, int OrderId, int ProductId);

        bool CanCustomerReviewVendor(int VendorId, int CustomerId, Order Order);

        Dictionary<Order, List<Product>> GetProductsWithPendingReviews(IList<Order> Orders, int CustomerId);

       
        #endregion

        #endregion
    }
}