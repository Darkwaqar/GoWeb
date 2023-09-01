using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Vendors;
using Nop.Web.Models.Catalog;
using Nop.Web.Extensions;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Seo;
using System.Web.Mvc;
using System.Collections.Generic;
using Nop.Services.Vendors;
using System.Globalization;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Common;
using Nop.Services.Common;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the vendor model factory
    /// </summary>
    public partial class VendorModelFactory : IVendorModelFactory
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CommonSettings _commonSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Constructors

        public VendorModelFactory(CaptchaSettings captchaSettings,
            CommonSettings commonSettings,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings,
            IStoreContext storeContext)
        {
            _captchaSettings = captchaSettings;
            _commonSettings = commonSettings;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _vendorAttributeParser = vendorAttributeParser;
            _vendorAttributeService = vendorAttributeService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _vendorSettings = vendorSettings;
            this._storeContext = storeContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the apply vendor model
        /// </summary>
        /// <param name="model">The apply vendor model</param>
        /// <param name="validateVendor">Whether to validate that the customer is already a vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>The apply vendor model</returns>
        public virtual ApplyVendorModel PrepareApplyVendorModel(ApplyVendorModel model, bool validateVendor, bool excludeProperties, string vendorAttributesXml)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (validateVendor && _workContext.CurrentCustomer.VendorId > 0)
            {
                //already applied for vendor account
                model.DisableFormInput = true;
                model.Result = _localizationService.GetResource("Vendors.ApplyAccount.AlreadyApplied");
            }

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnApplyVendorPage;
            model.TermsOfServiceEnabled = _vendorSettings.TermsOfServiceEnabled;
            model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentCustomer.Email;
            }

            //vendor attributes
            model.VendorAttributes = PrepareVendorAttributeModel(vendorAttributesXml);

            return model;
        }

        /// <summary>
        /// Prepare the vendor info model
        /// </summary>
        /// <param name="model">Vendor info model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Vendor info model</returns>
        public virtual VendorInfoModel PrepareVendorInfoModel(VendorInfoModel model, bool excludeProperties, string overriddenVendorAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var vendor = _workContext.CurrentVendor;
            if (!excludeProperties)
            {
                model.Description = vendor.Description;
                model.Email = vendor.Email;
                model.Name = vendor.Name;
            }

            var picture = _pictureService.GetPictureById(vendor.PictureId);
            var pictureSize = _mediaSettings.AvatarPictureSize;
            model.PictureUrl = picture != null ? _pictureService.GetPictureUrl(picture, pictureSize) : string.Empty;

            //vendor attributes
            if (string.IsNullOrEmpty(overriddenVendorAttributesXml))
                overriddenVendorAttributesXml = vendor.GetAttribute<string>(SystemVendorAttributeNames.VendorAttributes);
            model.VendorAttributes = PrepareVendorAttributeModel(overriddenVendorAttributesXml);

            return model;
        }

        /// <summary>
        /// Prepare the PublicReviewEdit model
        /// </summary>
        /// <param name="model">PublicReviewEdit model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Vendor info model</returns>
        public virtual PublicReviewEditModel PreparePublicReviewEditModel(PublicReviewEditModel model, Product product, Order order,
            Vendor vendor, Customer customer, IOrderedEnumerable<ProductReview> customerProductReviews, VendorReview customerVendorReview)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (customerProductReviews.Any())
            {
                var pr = customerProductReviews.First();
                model.AddProductReviewModel = new AddProductReviewModel()
                {

                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    Rating = pr.Rating,
                };
            }
            if (customerVendorReview != null)
            {
                model.VendorReviewListModel = customerVendorReview.ToListModel(_pictureService, product, vendor, order);
            }

            model.VendorReviewListModel.CustomerName = customer.FormatUserName();

            model.VendorName = vendor.Name;
            model.ProductName = product.Name;
            model.ProductSeName = product.GetSeName();
            model.VendorSeName = vendor.GetSeName();

            if (product.ProductPictures.Count > 0)
                model.ProductImageUrl = _pictureService.GetPictureUrl(product.ProductPictures.First().Picture);

            if (vendor != null && vendor.mpLogo > 0)
            {
                model.VendorImageUrl = _pictureService.GetPictureUrl(vendor.mpLogo);
            }

            model.VendorReviewListModel.ProductId = product.Id;
            model.VendorReviewListModel.OrderId = order.Id;
            return model;
        }

        #endregion

        #region VendorAttibute
        public virtual string ParseVendorAttributes(FormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var checkoutAttributes = _vendorAttributeService.GetAllVendorAttributes(_storeContext.CurrentStore.Id);
            foreach (var attribute in checkoutAttributes)
            {
                string controlId = string.Format("vendor_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                          attribute, selectedAttributeId.ToString());
                                }


                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                    }
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _vendorAttributeService.GetVendorAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = EngineContext.Current.Resolve<IDownloadService>().GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //save checkout attributes
            //validate conditional attributes (if specified)
            foreach (var attribute in checkoutAttributes)
            {
                var conditionMet = _vendorAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                    attributesXml = _vendorAttributeParser.RemoveVendorAttribute(attributesXml, attribute);
            }

            return attributesXml;
        }

        public virtual IList<string> GetVendorAttributesWarnings(string vendorAttributesXml)
        {
            var warnings = new List<string>();

            //selected attributes
            var attributes1 = _vendorAttributeParser.ParseVendorAttributes(vendorAttributesXml);

            //existing checkout attributes
            var attributes2 = _vendorAttributeService.GetAllVendorAttributes(_storeContext.CurrentStore.Id);
            foreach (var a2 in attributes2)
            {
                if (a2.IsRequired)
                {
                    bool found = false;
                    //selected checkout attributes
                    foreach (var a1 in attributes1)
                    {
                        if (a1.Id == a2.Id)
                        {
                            var attributeValuesStr = _vendorAttributeParser.ParseValues(vendorAttributesXml, a1.Id);
                            foreach (string str1 in attributeValuesStr)
                                if (!String.IsNullOrEmpty(str1.Trim()))
                                {
                                    found = true;
                                    break;
                                }
                        }
                    }

                    //if not found
                    if (!found)
                    {
                        if (!string.IsNullOrEmpty(a2.GetLocalized(a => a.TextPrompt, _workContext.WorkingLanguage.Id)))
                            warnings.Add(a2.GetLocalized(a => a.TextPrompt, _workContext.WorkingLanguage.Id));
                        else
                            warnings.Add(string.Format(_localizationService.GetResource("VendorUs.SelectAttribute"), a2.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id)));
                    }
                }
            }

            //now validation rules

            //minimum length
            foreach (var ca in attributes2)
            {
                if (ca.ValidationMinLength.HasValue)
                {
                    if (ca.AttributeControlType == AttributeControlType.TextBox ||
                        ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = _vendorAttributeParser.ParseValues(vendorAttributesXml, ca.Id);
                        var enteredText = valuesStr.FirstOrDefault();
                        int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (ca.ValidationMinLength.Value > enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("VendorUs.TextboxMinimumLength"), ca.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), ca.ValidationMinLength.Value));
                        }
                    }
                }

                //maximum length
                if (ca.ValidationMaxLength.HasValue)
                {
                    if (ca.AttributeControlType == AttributeControlType.TextBox ||
                        ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = _vendorAttributeParser.ParseValues(vendorAttributesXml, ca.Id);
                        var enteredText = valuesStr.FirstOrDefault();
                        int enteredTextLength = String.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (ca.ValidationMaxLength.Value < enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("VendorUs.TextboxMaximumLength"), ca.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), ca.ValidationMaxLength.Value));
                        }
                    }
                }
            }

            return warnings;
        }

        public virtual IList<VendorAttributeModel> PrepareVendorAttributeModel(string selectedVendorAttributes)
        {
            var model = new List<VendorAttributeModel>();

            var vendorAttributes = _vendorAttributeService.GetAllVendorAttributes(_storeContext.CurrentStore.Id);
            foreach (var attribute in vendorAttributes)
            {
                var attributeModel = new VendorAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name, _workContext.WorkingLanguage.Id),
                    TextPrompt = attribute.GetLocalized(x => x.TextPrompt, _workContext.WorkingLanguage.Id),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = attribute.DefaultValue
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = attribute.VendorAttributeValues;
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new VendorAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name, _workContext.WorkingLanguage.Id),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                            DisplayOrder = attributeValue.DisplayOrder,
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            if (!String.IsNullOrEmpty(selectedVendorAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _vendorAttributeParser.ParseVendorAttributeValues(selectedVendorAttributes);
                                foreach (var attributeValue in selectedValues)
                                    if (attributeModel.Id == attributeValue.VendorAttributeId)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedVendorAttributes))
                            {
                                var enteredText = _vendorAttributeParser.ParseValues(selectedVendorAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //keep in mind my that the code below works only in the current culture
                            var selectedDateStr = _vendorAttributeParser.ParseValues(selectedVendorAttributes, attribute.Id);
                            if (selectedDateStr.Any())
                            {
                                DateTime selectedDate;
                                if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                       DateTimeStyles.None, out selectedDate))
                                {
                                    //successfully parsed
                                    attributeModel.SelectedDay = selectedDate.Day;
                                    attributeModel.SelectedMonth = selectedDate.Month;
                                    attributeModel.SelectedYear = selectedDate.Year;
                                }
                            }

                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            if (!String.IsNullOrEmpty(selectedVendorAttributes))
                            {
                                var downloadGuidStr = _vendorAttributeParser.ParseValues(selectedVendorAttributes, attribute.Id).FirstOrDefault();
                                Guid downloadGuid;
                                Guid.TryParse(downloadGuidStr, out downloadGuid);
                                var download = EngineContext.Current.Resolve<IDownloadService>().GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                    attributeModel.DefaultValue = download.DownloadGuid.ToString();
                            }
                        }
                        break;
                    default:
                        break;
                }

                model.Add(attributeModel);
            }

            return model;
        }
        #endregion
    }
}
