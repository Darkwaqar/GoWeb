﻿using Nop.Core.Domain.Vendors;
using System.Collections.Generic;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor attribute service
    /// </summary>
    public partial interface IVendorAttributeService
    {
        #region Vendor attributes

        /// <summary>
        /// Deletes a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        void DeleteVendorAttribute(VendorAttribute vendorAttribute);

        /// <summary>
        /// Gets all Vendor attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Vendor attributes</returns>
        IList<VendorAttribute> GetAllVendorAttributes(int storeId = 0, bool ignorAcl = false);

        /// <summary>
        /// Gets a Vendor attribute 
        /// </summary>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>Vendor attribute</returns>
        VendorAttribute GetVendorAttributeById(int vendorAttributeId);

        /// <summary>
        /// Inserts a Vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        void InsertVendorAttribute(VendorAttribute vendorAttribute);

        /// <summary>
        /// Updates the Vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        void UpdateVendorAttribute(VendorAttribute vendorAttribute);

        #endregion

        #region Vendor attribute values

        /// <summary>
        /// Deletes a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        void DeleteVendorAttributeValue(VendorAttributeValue vendorAttributeValue);

        /// <summary>
        /// Gets vendor attribute values by vendor attribute identifier
        /// </summary>
        /// <param name="vendorAttributeId">The vendor attribute identifier</param>
        /// <returns>Vendor attribute values</returns>
        IList<VendorAttributeValue> GetVendorAttributeValues(int vendorAttributeId);

        /// <summary>
        /// Gets a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValueId">Vendor attribute value identifier</param>
        /// <returns>Vendor attribute value</returns>
        VendorAttributeValue GetVendorAttributeValueById(int vendorAttributeValueId);

        /// <summary>
        /// Inserts a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        void InsertVendorAttributeValue(VendorAttributeValue vendorAttributeValue);

        /// <summary>
        /// Updates the vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        void UpdateVendorAttributeValue(VendorAttributeValue vendorAttributeValue);

        #endregion
    }
}
