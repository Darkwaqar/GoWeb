﻿using System;
using System.Text;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Html;
using Nop.Services.Localization;
using Nop.Services.Media;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Contact attribute helper
    /// </summary>
    public partial class ContactAttributeFormatter : IContactAttributeFormatter
    {
        private readonly IWorkContext _workContext;
        private readonly IContactAttributeService _checkoutAttributeService;
        private readonly IContactAttributeParser _checkoutAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;

        public ContactAttributeFormatter(IWorkContext workContext,
            IContactAttributeService checkoutAttributeService,
            IContactAttributeParser checkoutAttributeParser,
            IDownloadService downloadService,
            IWebHelper webHelper)
        {
            this._workContext = workContext;
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._downloadService = downloadService;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributes(string attributesXml)
        {
            var customer = _workContext.CurrentCustomer;
            return FormatAttributes(attributesXml, customer);
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributes(string attributesXml,
            Customer customer,
            string serapator = "<br />",
            bool htmlEncode = true,
            bool renderPrices = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();

            var attributes = _checkoutAttributeParser.ParseContactAttributes(attributesXml);
            for (int i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var valuesStr = _checkoutAttributeParser.ParseValues(attributesXml, attribute.Id);
                for (int j = 0; j < valuesStr.Count; j++)
                {
                    string valueStr = valuesStr[j];
                    string formattedAttribute = "";
                    if (!attribute.ShouldHaveValues())
                    {
                        //no values
                        if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            //multiline textbox
                            var attributeName = attribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id);
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = HttpUtility.HtmlEncode(attributeName);
                            formattedAttribute = string.Format("{0}: {1}", attributeName, HtmlHelper.FormatText(valueStr, false, true, false, false, false, false));
                            //we never encode multiline textbox input
                        }
                        else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                        {
                            //file upload
                            Guid downloadGuid;
                            Guid.TryParse(valueStr, out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                                string attributeText;
                                var fileName = string.Format("{0}{1}",
                                    download.Filename ?? download.DownloadGuid.ToString(),
                                    download.Extension);
                                //encode (if required)
                                if (htmlEncode)
                                    fileName = HttpUtility.HtmlEncode(fileName);
                                if (allowHyperlinks)
                                {
                                    //hyperlinks are allowed
                                    var downloadLink = string.Format("{0}download/getfileupload/?downloadId={1}", _webHelper.GetStoreLocation(false), download.DownloadGuid);
                                    attributeText = string.Format("<a href=\"{0}\" class=\"fileuploadattribute\">{1}</a>", downloadLink, fileName);
                                }
                                else
                                {
                                    //hyperlinks aren't allowed
                                    attributeText = fileName;
                                }
                                var attributeName = attribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id);
                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = HttpUtility.HtmlEncode(attributeName);
                                formattedAttribute = string.Format("{0}: {1}", attributeName, attributeText);
                            }
                        }
                        else
                        {
                            //other attributes (textbox, datepicker)
                            formattedAttribute = string.Format("{0}: {1}", attribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), valueStr);
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = HttpUtility.HtmlEncode(formattedAttribute);
                        }
                    }
                    else
                    {
                        int attributeValueId;
                        if (int.TryParse(valueStr, out attributeValueId))
                        {
                            var attributeValue = _checkoutAttributeService.GetContactAttributeValueById(attributeValueId);
                            if (attributeValue != null)
                            {
                                formattedAttribute = string.Format("{0}: {1}", attribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), attributeValue.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id));
                            }
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = HttpUtility.HtmlEncode(formattedAttribute);
                        }
                    }

                    if (!String.IsNullOrEmpty(formattedAttribute))
                    {
                        if (i != 0 || j != 0)
                            result.Append(serapator);
                        result.Append(formattedAttribute);
                    }
                }
            }

            return result.ToString();
        }
    }
}
