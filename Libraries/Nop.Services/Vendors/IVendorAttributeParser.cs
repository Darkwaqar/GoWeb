using Nop.Core.Domain.Vendors;
using System.Collections.Generic;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor attribute parser interface
    /// </summary>
    public partial interface IVendorAttributeParser
    {
        /// <summary>
        /// Gets selected vendor attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected vendor attributes</returns>
        IList<VendorAttribute> ParseVendorAttributes(string attributesXml);

        /// <summary>
        /// Get vendor attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Vendor attribute values</returns>
        IList<VendorAttributeValue> ParseVendorAttributeValues(string attributesXml);

        /// <summary>
        /// Gets selected vendor attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>Vendor attribute value</returns>
        IList<string> ParseValues(string attributesXml, int vendorAttributeId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ca">Vendor attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddVendorAttribute(string attributesXml, VendorAttribute ca, string value);

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="attribute">Vendor attribute</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>Result</returns>
        bool? IsConditionMet(VendorAttribute attribute, string selectedAttributesXml);

        /// <summary>
        /// Remove an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attribute">vendor attribute</param>
        /// <returns>Updated result (XML format)</returns>
        string RemoveVendorAttribute(string attributesXml, VendorAttribute attribute);
    }
}
