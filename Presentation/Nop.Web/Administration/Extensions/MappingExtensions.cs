using Nop.Admin.Models.Affiliates;
using Nop.Admin.Models.Appointments;
using Nop.Admin.Models.Banner;
using Nop.Admin.Models.Blogs;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Cms;
using Nop.Admin.Models.Common;
using Nop.Admin.Models.Contact;
using Nop.Admin.Models.Customers;
using Nop.Admin.Models.Directory;
using Nop.Admin.Models.Discounts;
using Nop.Admin.Models.ExternalAuthentication;
using Nop.Admin.Models.Fcm;
using Nop.Admin.Models.Forums;
using Nop.Admin.Models.Localization;
using Nop.Admin.Models.Logging;
using Nop.Admin.Models.Magazines;
using Nop.Admin.Models.Messages;
using Nop.Admin.Models.News;
using Nop.Admin.Models.Orders;
using Nop.Admin.Models.Payments;
using Nop.Admin.Models.Plugins;
using Nop.Admin.Models.Polls;
using Nop.Admin.Models.Settings;
using Nop.Admin.Models.Shipping;
using Nop.Admin.Models.ShopTheLook;
using Nop.Admin.Models.SMS;
using Nop.Admin.Models.Stores;
using Nop.Admin.Models.Tax;
using Nop.Admin.Models.Templates;
using Nop.Admin.Models.Topics;
using Nop.Admin.Models.Vendors;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Appointments;
using Nop.Core.Domain.Banners;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Contact;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Fcm;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Interactive;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Magazines;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.PushNotifications;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.ShopTheLook;
using Nop.Core.Domain.SMS;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Web.Framework.Security.Captcha;
using System;
using System.Linq;

namespace Nop.Admin.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }
        
        #region Category

        public static CategoryModel ToModel(this Category entity)
        {
            return entity.MapTo<Category, CategoryModel>();
        }

        public static Category ToEntity(this CategoryModel model)
        {
            return model.MapTo<CategoryModel, Category>();
        }

        public static Category ToEntity(this CategoryModel model, Category destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Manufacturer

        public static ManufacturerModel ToModel(this Manufacturer entity)
        {
            return entity.MapTo<Manufacturer, ManufacturerModel>();
        }

        public static Manufacturer ToEntity(this ManufacturerModel model)
        {
            return model.MapTo<ManufacturerModel, Manufacturer>();
        }

        public static Manufacturer ToEntity(this ManufacturerModel model, Manufacturer destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Vendor

        public static VendorModel ToModel(this Vendor entity)
        {
            return entity.MapTo<Vendor, VendorModel>();
        }

        public static Vendor ToEntity(this VendorModel model)
        {
            return model.MapTo<VendorModel, Vendor>();
        }

        public static Vendor ToEntity(this VendorModel model, Vendor destination)
        {
            return model.MapTo(destination);
        }


        #endregion

        #region Vendor attributes

        //attributes
        public static VendorAttributeModel ToModel(this VendorAttribute entity)
        {
            return entity.MapTo<VendorAttribute, VendorAttributeModel>();
        }

        public static VendorAttribute ToEntity(this VendorAttributeModel model)
        {
            return model.MapTo<VendorAttributeModel, VendorAttribute>();
        }

        public static VendorAttribute ToEntity(this VendorAttributeModel model, VendorAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region MobileAppSetting

        public static MobileAppSettingModel ToModel(this MobileAppSetting entity)
        {
            return entity.MapTo<MobileAppSetting, MobileAppSettingModel>();
        }

        public static MobileAppSetting ToEntity(this MobileAppSettingModel model)
        {
            return model.MapTo<MobileAppSettingModel, MobileAppSetting>();
        }
        public static MobileAppSetting ToEntity(this MobileAppSettingModel model, MobileAppSetting destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region SocialLinks

        public static VendorPaymentInfoModel ToModel(this VendorPaymentInfo entity)
        {
            return entity.MapTo<VendorPaymentInfo, VendorPaymentInfoModel>();
        }

        public static VendorPaymentInfo ToEntity(this VendorPaymentInfoModel model)
        {
            return model.MapTo<VendorPaymentInfoModel, VendorPaymentInfo>();
        }
        public static VendorPaymentInfo ToEntity(this VendorPaymentInfoModel model, VendorPaymentInfo destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region VendorPaymentInfo

        public static SocialLinksModel ToModel(this SocialLinks entity)
        {
            return entity.MapTo<SocialLinks, SocialLinksModel>();
        }

        public static SocialLinks ToEntity(this SocialLinksModel model)
        {
            return model.MapTo<SocialLinksModel, SocialLinks>();
        }
        public static SocialLinks ToEntity(this SocialLinksModel model, SocialLinks destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region VendorPayout

        public static VendorPayoutModel ToModel(this VendorPayout entity)
        {
            return entity.MapTo<VendorPayout, VendorPayoutModel>();
        }

        public static VendorPayout ToEntity(this VendorPayoutModel model)
        {
            return model.MapTo<VendorPayoutModel, VendorPayout>();
        }

        public static VendorPayout ToEntity(this VendorPayoutModel model, VendorPayout destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region VendorReview

        public static VendorReviewModel ToModel(this VendorReview entity)
        {
            return entity.MapTo<VendorReview, VendorReviewModel>();
        }

        public static VendorReview ToEntity(this VendorReviewModel model)
        {
            return model.MapTo<VendorReviewModel, VendorReview>();
        }

        public static VendorReview ToEntity(this VendorReviewModel model, VendorReview destination)
        {
            return model.MapTo(destination);
        }


        #endregion

        #region Products

        public static ProductModel ToModel(this Product entity)
        {
            return entity.MapTo<Product, ProductModel>();
        }

        public static Product ToEntity(this ProductModel model)
        {
            return model.MapTo<ProductModel, Product>();
        }

        public static Product ToEntity(this ProductModel model, Product destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Product attributes

        public static ProductAttributeModel ToModel(this ProductAttribute entity)
        {
            return entity.MapTo<ProductAttribute, ProductAttributeModel>();
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model)
        {
            return model.MapTo<ProductAttributeModel, ProductAttribute>();
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model, ProductAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Specification attributes

        //attributes
        public static SpecificationAttributeModel ToModel(this SpecificationAttribute entity)
        {
            return entity.MapTo<SpecificationAttribute, SpecificationAttributeModel>();
        }

        public static SpecificationAttribute ToEntity(this SpecificationAttributeModel model)
        {
            return model.MapTo<SpecificationAttributeModel, SpecificationAttribute>();
        }

        public static SpecificationAttribute ToEntity(this SpecificationAttributeModel model, SpecificationAttribute destination)
        {
            return model.MapTo(destination);
        }

        //attribute options
        public static SpecificationAttributeOptionModel ToModel(this SpecificationAttributeOption entity)
        {
            return entity.MapTo<SpecificationAttributeOption, SpecificationAttributeOptionModel>();
        }

        public static SpecificationAttributeOption ToEntity(this SpecificationAttributeOptionModel model)
        {
            return model.MapTo<SpecificationAttributeOptionModel, SpecificationAttributeOption>();
        }

        public static SpecificationAttributeOption ToEntity(this SpecificationAttributeOptionModel model, SpecificationAttributeOption destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region Checkout attributes

        //attributes
        public static CheckoutAttributeModel ToModel(this CheckoutAttribute entity)
        {
            return entity.MapTo<CheckoutAttribute, CheckoutAttributeModel>();
        }

        public static CheckoutAttribute ToEntity(this CheckoutAttributeModel model)
        {
            return model.MapTo<CheckoutAttributeModel, CheckoutAttribute>();
        }

        public static CheckoutAttribute ToEntity(this CheckoutAttributeModel model, CheckoutAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Contact attributes

        //attributes
        public static ContactAttributeModel ToModel(this ContactAttribute entity)
        {
            return entity.MapTo<ContactAttribute, ContactAttributeModel>();
        }

        public static ContactAttribute ToEntity(this ContactAttributeModel model)
        {
            return model.MapTo<ContactAttributeModel, ContactAttribute>();
        }

        public static ContactAttribute ToEntity(this ContactAttributeModel model, ContactAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Appointment attributes

        //attributes
        public static AppointmentAttributeModel ToModel(this AppointmentAttribute entity)
        {
            return entity.MapTo<AppointmentAttribute, AppointmentAttributeModel>();
        }

        public static AppointmentAttribute ToEntity(this AppointmentAttributeModel model)
        {
            return model.MapTo<AppointmentAttributeModel, AppointmentAttribute>();
        }

        public static AppointmentAttribute ToEntity(this AppointmentAttributeModel model, AppointmentAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Customer attributes

        //attributes
        public static CustomerAttributeModel ToModel(this CustomerAttribute entity)
        {
            return entity.MapTo<CustomerAttribute, CustomerAttributeModel>();
        }

        public static CustomerAttribute ToEntity(this CustomerAttributeModel model)
        {
            return model.MapTo<CustomerAttributeModel, CustomerAttribute>();
        }

        public static CustomerAttribute ToEntity(this CustomerAttributeModel model, CustomerAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Address attributes

        //attributes
        public static AddressAttributeModel ToModel(this AddressAttribute entity)
        {
            return entity.MapTo<AddressAttribute, AddressAttributeModel>();
        }

        public static AddressAttribute ToEntity(this AddressAttributeModel model)
        {
            return model.MapTo<AddressAttributeModel, AddressAttribute>();
        }

        public static AddressAttribute ToEntity(this AddressAttributeModel model, AddressAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Languages

        public static LanguageModel ToModel(this Language entity)
        {
            return entity.MapTo<Language, LanguageModel>();
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return model.MapTo<LanguageModel, Language>();
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return model.MapTo(destination);
        }
        
        #endregion

        #region Email account

        public static EmailAccountModel ToModel(this EmailAccount entity)
        {
            return entity.MapTo<EmailAccount, EmailAccountModel>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model)
        {
            return model.MapTo<EmailAccountModel, EmailAccount>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model, EmailAccount destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Number account

        public static NumberAccountModel ToModel(this NumberAccount entity)
        {
            return entity.MapTo<NumberAccount, NumberAccountModel>();
        }

        public static NumberAccount ToEntity(this NumberAccountModel model)
        {
            return model.MapTo<NumberAccountModel, NumberAccount>();
        }

        public static NumberAccount ToEntity(this NumberAccountModel model, NumberAccount destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Message templates

        public static MessageTemplateModel ToModel(this MessageTemplate entity)
        {
            return entity.MapTo<MessageTemplate, MessageTemplateModel>();
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model)
        {
            return model.MapTo<MessageTemplateModel, MessageTemplate>();
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model, MessageTemplate destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region SMS templates

        public static SMSTemplateModel ToModel(this SMSTemplate entity)
        {
            return entity.MapTo<SMSTemplate, SMSTemplateModel>();
        }

        public static SMSTemplate ToEntity(this SMSTemplateModel model)
        {
            return model.MapTo<SMSTemplateModel, SMSTemplate>();
        }

        public static SMSTemplate ToEntity(this SMSTemplateModel model, SMSTemplate destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Contact form

        public static ContactFormModel ToModel(this ContactUs entity)
        {
            return entity.MapTo<ContactUs, ContactFormModel>();
        }

        public static ContactUs ToEntity(this ContactFormModel model)
        {
            return model.MapTo<ContactFormModel, ContactUs>();
        }

        public static ContactUs ToEntity(this ContactFormModel model, ContactUs destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return entity.MapTo<QueuedEmail, QueuedEmailModel>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            return model.MapTo<QueuedEmailModel, QueuedEmail>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Queued SMS

        public static QueuedSMSModel ToModel(this QueuedSMS entity)
        {
            return entity.MapTo<QueuedSMS, QueuedSMSModel>();
        }

        public static QueuedSMS ToEntity(this QueuedSMSModel model)
        {
            return model.MapTo<QueuedSMSModel, QueuedSMS>();
        }

        public static QueuedSMS ToEntity(this QueuedSMSModel model, QueuedSMS destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Banner

        public static BannerModel ToModel(this Banner entity)
        {
            return entity.MapTo<Banner, BannerModel>();
        }

        public static Banner ToEntity(this BannerModel model)
        {
            return model.MapTo<BannerModel, Banner>();
        }

        public static Banner ToEntity(this BannerModel model, Banner destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Interactive form

        public static InteractiveFormModel ToModel(this InteractiveForm entity)
        {
            return entity.MapTo<InteractiveForm, InteractiveFormModel>();
        }

        public static InteractiveForm ToEntity(this InteractiveFormModel model)
        {
            return model.MapTo<InteractiveFormModel, InteractiveForm>();
        }

        public static InteractiveForm ToEntity(this InteractiveFormModel model, InteractiveForm destination)
        {
            return model.MapTo(destination);
        }


        public static InteractiveFormAttributeModel ToModel(this InteractiveFormAttribute entity)
        {
            return entity.MapTo<InteractiveFormAttribute, InteractiveFormAttributeModel>();
        }

        public static InteractiveFormAttribute ToEntity(this InteractiveFormAttributeModel model)
        {
            return model.MapTo<InteractiveFormAttributeModel, InteractiveFormAttribute>();
        }

        public static InteractiveFormAttribute ToEntity(this InteractiveFormAttributeModel model, InteractiveFormAttribute destination)
        {
            return model.MapTo(destination);
        }


        public static InteractiveFormAttributeValueModel ToModel(this InteractiveFormAttributeValue entity)
        {
            return entity.MapTo<InteractiveFormAttributeValue, InteractiveFormAttributeValueModel>();
        }

        public static InteractiveFormAttributeValue ToEntity(this InteractiveFormAttributeValueModel model)
        {
            return model.MapTo<InteractiveFormAttributeValueModel, InteractiveFormAttributeValue>();
        }

        public static InteractiveFormAttributeValue ToEntity(this InteractiveFormAttributeValueModel model, InteractiveFormAttributeValue destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Queued fcm

        public static QueuedFcmModel ToModel(this QueuedFcm entity)
        {
            return entity.MapTo<QueuedFcm, QueuedFcmModel>();
        }

        public static QueuedFcm ToEntity(this QueuedFcmModel model)
        {
            return model.MapTo<QueuedFcmModel, QueuedFcm>();
        }

        public static QueuedFcm ToEntity(this QueuedFcmModel model, QueuedFcm destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Campaigns

        public static CampaignModel ToModel(this Campaign entity)
        {
            return entity.MapTo<Campaign, CampaignModel>();
        }

        public static Campaign ToEntity(this CampaignModel model)
        {
            return model.MapTo<CampaignModel, Campaign>();
        }

        public static Campaign ToEntity(this CampaignModel model, Campaign destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Topics

        public static TopicModel ToModel(this Topic entity)
        {
            return entity.MapTo<Topic, TopicModel>();
        }

        public static Topic ToEntity(this TopicModel model)
        {
            return model.MapTo<TopicModel, Topic>();
        }

        public static Topic ToEntity(this TopicModel model, Topic destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Log

        public static LogModel ToModel(this Log entity)
        {
            return entity.MapTo<Log, LogModel>();
        }

        public static Log ToEntity(this LogModel model)
        {
            return model.MapTo<LogModel, Log>();
        }

        public static Log ToEntity(this LogModel model, Log destination)
        {
            return model.MapTo(destination);
        }

        public static ActivityLogTypeModel ToModel(this ActivityLogType entity)
        {
            return entity.MapTo<ActivityLogType, ActivityLogTypeModel>();
        }

        public static ActivityLogModel ToModel(this ActivityLog entity)
        {
            return entity.MapTo<ActivityLog, ActivityLogModel>();
        }

        #endregion
        
        #region Currencies

        public static CurrencyModel ToModel(this Currency entity)
        {
            return entity.MapTo<Currency, CurrencyModel>();
        }

        public static Currency ToEntity(this CurrencyModel model)
        {
            return model.MapTo<CurrencyModel, Currency>();
        }

        public static Currency ToEntity(this CurrencyModel model, Currency destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region Measure weights

        public static MeasureWeightModel ToModel(this MeasureWeight entity)
        {
            return entity.MapTo<MeasureWeight, MeasureWeightModel>();
        }

        public static MeasureWeight ToEntity(this MeasureWeightModel model)
        {
            return model.MapTo<MeasureWeightModel, MeasureWeight>();
        }

        public static MeasureWeight ToEntity(this MeasureWeightModel model, MeasureWeight destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Measure dimension

        public static MeasureDimensionModel ToModel(this MeasureDimension entity)
        {
            return entity.MapTo<MeasureDimension, MeasureDimensionModel>();
        }

        public static MeasureDimension ToEntity(this MeasureDimensionModel model)
        {
            return model.MapTo<MeasureDimensionModel, MeasureDimension>();
        }

        public static MeasureDimension ToEntity(this MeasureDimensionModel model, MeasureDimension destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Tax providers

        public static TaxProviderModel ToModel(this ITaxProvider entity)
        {
            return entity.MapTo<ITaxProvider, TaxProviderModel>();
        }

        #endregion

        #region Tax categories

        public static TaxCategoryModel ToModel(this TaxCategory entity)
        {
            return entity.MapTo<TaxCategory, TaxCategoryModel>();
        }

        public static TaxCategory ToEntity(this TaxCategoryModel model)
        {
            return model.MapTo<TaxCategoryModel, TaxCategory>();
        }

        public static TaxCategory ToEntity(this TaxCategoryModel model, TaxCategory destination)
        {
            return model.MapTo(destination);
        }

        #endregion
        
        #region Shipping rate computation method

        public static ShippingRateComputationMethodModel ToModel(this IShippingRateComputationMethod entity)
        {
            return entity.MapTo<IShippingRateComputationMethod, ShippingRateComputationMethodModel>();
        }

        #endregion

        #region Pickup point providers

        public static PickupPointProviderModel ToModel(this IPickupPointProvider entity)
        {
            return entity.MapTo<IPickupPointProvider, PickupPointProviderModel>();
        }

        #endregion

        #region Shipping methods

        public static ShippingMethodModel ToModel(this ShippingMethod entity)
        {
            return entity.MapTo<ShippingMethod, ShippingMethodModel>();
        }

        public static ShippingMethod ToEntity(this ShippingMethodModel model)
        {
            return model.MapTo<ShippingMethodModel, ShippingMethod>();
        }

        public static ShippingMethod ToEntity(this ShippingMethodModel model, ShippingMethod destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Delivery dates

        public static DeliveryDateModel ToModel(this DeliveryDate entity)
        {
            return entity.MapTo<DeliveryDate, DeliveryDateModel>();
        }

        public static DeliveryDate ToEntity(this DeliveryDateModel model)
        {
            return model.MapTo<DeliveryDateModel, DeliveryDate>();
        }

        public static DeliveryDate ToEntity(this DeliveryDateModel model, DeliveryDate destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Product availability ranges

        public static ProductAvailabilityRangeModel ToModel(this ProductAvailabilityRange entity)
        {
            return entity.MapTo<ProductAvailabilityRange, ProductAvailabilityRangeModel>();
        }

        public static ProductAvailabilityRange ToEntity(this ProductAvailabilityRangeModel model)
        {
            return model.MapTo<ProductAvailabilityRangeModel, ProductAvailabilityRange>();
        }

        public static ProductAvailabilityRange ToEntity(this ProductAvailabilityRangeModel model, ProductAvailabilityRange destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Payment methods

        public static PaymentMethodModel ToModel(this IPaymentMethod entity)
        {
            return entity.MapTo<IPaymentMethod, PaymentMethodModel>();
        }

        #endregion

        #region External authentication methods

        public static AuthenticationMethodModel ToModel(this IExternalAuthenticationMethod entity)
        {
            return entity.MapTo<IExternalAuthenticationMethod, AuthenticationMethodModel>();
        }

        #endregion
        
        #region Widgets

        public static WidgetModel ToModel(this IWidgetPlugin entity)
        {
            return entity.MapTo<IWidgetPlugin, WidgetModel>();
        }

        #endregion

        #region Address

        public static AddressModel ToModel(this Address entity)
        {
            return entity.MapTo<Address, AddressModel>();
        }

        public static Address ToEntity(this AddressModel model)
        {
            return model.MapTo<AddressModel, Address>();
        }

        public static Address ToEntity(this AddressModel model, Address destination)
        {
            return model.MapTo(destination);
        }

        public static void PrepareCustomAddressAttributes(this AddressModel model,
            Address address,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeParser addressAttributeParser)
        {
            //this method is very similar to the same one in Nop.Web project
            if (addressAttributeService == null)
                throw new ArgumentNullException("addressAttributeService");

            if (addressAttributeParser == null)
                throw new ArgumentNullException("addressAttributeParser");

            var attributes = addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressModel.AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = addressAttributeService.GetAddressAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressModel.AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = address != null ? address.CustomAttributes : null;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!String.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = addressAttributeParser.ParseAddressAttributeValues(selectedAddressAttributes);
                                foreach (var attributeValue in selectedValues)
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
                            if (!String.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                var enteredText = addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }

        #endregion

        #region Device
        public static DeviceModel ToModel(this Device entity)
        {
            return entity.MapTo<Device, DeviceModel>();
        }

        public static Device ToEntity(this DeviceModel model)
        {
            return model.MapTo<DeviceModel, Device>();
        }

        public static Device ToEntity(this DeviceModel model, Device destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Notification
        public static NotificationModel ToModel(this Notification entity)
        {
            return entity.MapTo<Notification, NotificationModel>();
        }

        public static Notification ToEntity(this NotificationModel model)
        {
            return model.MapTo<NotificationModel, Notification>();
        }

        public static Notification ToEntity(this NotificationModel model, Notification destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Appointment
        public static ProductAppointmentModel ToModel(this ProductAppointment entity)
        {
            return entity.MapTo<ProductAppointment,ProductAppointmentModel>();
        }

        public static ProductAppointment ToEntity(this ProductAppointmentModel model)
        {
            return model.MapTo<ProductAppointmentModel, ProductAppointment>();
        }

        public static ProductAppointment ToEntity(this ProductAppointmentModel model, ProductAppointment destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Magazine
        public static MagazineModel ToModel(this Magazine entity)
        {
            return entity.MapTo<Magazine, MagazineModel>();
        }

        public static Magazine ToEntity(this MagazineModel model)
        {
            return model.MapTo<MagazineModel, Magazine>();
        }

        public static Magazine ToEntity(this MagazineModel model, Magazine destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region ShopTheLook
        public static PointerModel ToModel(this Pointer entity)
        {
            return entity.MapTo<Pointer, PointerModel>();
        }

        public static Pointer ToEntity(this PointerModel model)
        {
            return model.MapTo<PointerModel, Pointer>();
        }

        public static Pointer ToEntity(this PointerModel model, Pointer destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region FcmApplication
        public static FcmApplicationModel ToModel(this FcmApplication entity)
        {
            return entity.MapTo<FcmApplication, FcmApplicationModel>();
        }

        public static FcmApplication ToEntity(this FcmApplicationModel model)
        {
            return model.MapTo<FcmApplicationModel, FcmApplication>();
        }

        public static FcmApplication ToEntity(this FcmApplicationModel model, FcmApplication destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region FcmAction
        public static FcmActionModel ToModel(this FcmAction entity)
        {
            return entity.MapTo<FcmAction, FcmActionModel>();
        }

        public static FcmAction ToEntity(this FcmActionModel model)
        {
            return model.MapTo<FcmActionModel, FcmAction>();
        }

        public static FcmAction ToEntity(this FcmActionModel model, FcmAction destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region NewsLetter subscriptions

        public static NewsLetterSubscriptionModel ToModel(this NewsLetterSubscription entity)
        {
            return entity.MapTo<NewsLetterSubscription, NewsLetterSubscriptionModel>();
        }

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionModel model)
        {
            return model.MapTo<NewsLetterSubscriptionModel, NewsLetterSubscription>();
        }

        public static NewsLetterSubscription ToEntity(this NewsLetterSubscriptionModel model, NewsLetterSubscription destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Discounts

        public static DiscountModel ToModel(this Discount entity)
        {
            return entity.MapTo<Discount, DiscountModel>();
        }

        public static Discount ToEntity(this DiscountModel model)
        {
            return model.MapTo<DiscountModel, Discount>();
        }

        public static Discount ToEntity(this DiscountModel model, Discount destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Forums

        //forum groups
        public static ForumGroupModel ToModel(this ForumGroup entity)
        {
            return entity.MapTo<ForumGroup, ForumGroupModel>();
        }

        public static ForumGroup ToEntity(this ForumGroupModel model)
        {
            return model.MapTo<ForumGroupModel, ForumGroup>();
        }

        public static ForumGroup ToEntity(this ForumGroupModel model, ForumGroup destination)
        {
            return model.MapTo(destination);
        }
        //forums
        public static ForumModel ToModel(this Forum entity)
        {
            return entity.MapTo<Forum, ForumModel>();
        }

        public static Forum ToEntity(this ForumModel model)
        {
            return model.MapTo<ForumModel, Forum>();
        }

        public static Forum ToEntity(this ForumModel model, Forum destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region Blog

        //blog posts
        public static BlogPostModel ToModel(this BlogPost entity)
        {
            return entity.MapTo<BlogPost, BlogPostModel>();
        }

        public static BlogPost ToEntity(this BlogPostModel model)
        {
            return model.MapTo<BlogPostModel, BlogPost>();
        }

        public static BlogPost ToEntity(this BlogPostModel model, BlogPost destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region News

        //news items
        public static NewsItemModel ToModel(this NewsItem entity)
        {
            return entity.MapTo<NewsItem, NewsItemModel>();
        }

        public static NewsItem ToEntity(this NewsItemModel model)
        {
            return model.MapTo<NewsItemModel, NewsItem>();
        }

        public static NewsItem ToEntity(this NewsItemModel model, NewsItem destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region PollCategory

        public static PollCategoryModel ToModel(this PollCategory entity)
        {
            return entity.MapTo<PollCategory, PollCategoryModel>();
        }

        public static PollCategory ToEntity(this PollCategoryModel model)
        {
            return model.MapTo<PollCategoryModel, PollCategory>();
        }

        public static PollCategory ToEntity(this PollCategoryModel model, PollCategory destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Polls

        //news items
        public static PollModel ToModel(this Poll entity)
        {
            return entity.MapTo<Poll, PollModel>();
        }

        public static Poll ToEntity(this PollModel model)
        {
            return model.MapTo<PollModel, Poll>();
        }

        public static Poll ToEntity(this PollModel model, Poll destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Customer roles

        //customer roles
        public static CustomerRoleModel ToModel(this CustomerRole entity)
        {
            return entity.MapTo<CustomerRole, CustomerRoleModel>();
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model)
        {
            return model.MapTo<CustomerRoleModel, CustomerRole>();
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model, CustomerRole destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Customer Tag

        //customer tags
        public static CustomerTagModel ToModel(this CustomerTag entity)
        {
            return entity.MapTo<CustomerTag, CustomerTagModel>();
        }

        public static CustomerTag ToEntity(this CustomerTagModel model)
        {
            return model.MapTo<CustomerTagModel, CustomerTag>();
        }

        public static CustomerTag ToEntity(this CustomerTagModel model, CustomerTag destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Customer Action

        public static CustomerActionModel ToModel(this CustomerAction entity)
        {
            return entity.MapTo<CustomerAction, CustomerActionModel>();
        }

        public static CustomerAction ToEntity(this CustomerActionModel model)
        {
            return model.MapTo<CustomerActionModel, CustomerAction>();
        }

        public static CustomerAction ToEntity(this CustomerActionModel model, CustomerAction destination)
        {
            return model.MapTo(destination);
        }

        public static CustomerActionConditionModel ToModel(this CustomerAction.ActionCondition entity)
        {
            return entity.MapTo<CustomerAction.ActionCondition, CustomerActionConditionModel>();
        }

        public static CustomerAction.ActionCondition ToEntity(this CustomerActionConditionModel model)
        {
            return model.MapTo<CustomerActionConditionModel, CustomerAction.ActionCondition>();
        }

        public static CustomerAction.ActionCondition ToEntity(this CustomerActionConditionModel model, CustomerAction.ActionCondition destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Customer Action Type

        public static CustomerActionTypeModel ToModel(this CustomerActionType entity)
        {
            return entity.MapTo<CustomerActionType, CustomerActionTypeModel>();
        }

        public static CustomerActionType ToEntity(this CustomerActionTypeModel model)
        {
            return model.MapTo<CustomerActionTypeModel, CustomerActionType>();
        }

        public static CustomerActionType ToEntity(this CustomerActionTypeModel model, CustomerActionType destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Customer Reminder

        public static CustomerReminderModel ToModel(this CustomerReminder entity)
        {
            return entity.MapTo<CustomerReminder, CustomerReminderModel>();
        }

        public static CustomerReminder ToEntity(this CustomerReminderModel model)
        {
            return model.MapTo<CustomerReminderModel, CustomerReminder>();
        }

        public static CustomerReminder ToEntity(this CustomerReminderModel model, CustomerReminder destination)
        {
            return model.MapTo(destination);
        }

        //customer action
        public static CustomerReminderModel.ReminderLevelModel ToModel(this CustomerReminder.ReminderLevel entity)
        {
            return entity.MapTo<CustomerReminder.ReminderLevel, CustomerReminderModel.ReminderLevelModel>();
        }

        public static CustomerReminder.ReminderLevel ToEntity(this CustomerReminderModel.ReminderLevelModel model)
        {
            return model.MapTo<CustomerReminderModel.ReminderLevelModel, CustomerReminder.ReminderLevel>();
        }

        public static CustomerReminder.ReminderLevel ToEntity(this CustomerReminderModel.ReminderLevelModel model, CustomerReminder.ReminderLevel destination)
        {
            return model.MapTo(destination);
        }

        public static CustomerReminderModel.ConditionModel ToModel(this CustomerReminder.ReminderCondition entity)
        {
            return entity.MapTo<CustomerReminder.ReminderCondition, CustomerReminderModel.ConditionModel>();
        }

        public static CustomerReminder.ReminderCondition ToEntity(this CustomerReminderModel.ConditionModel model)
        {
            return model.MapTo<CustomerReminderModel.ConditionModel, CustomerReminder.ReminderCondition>();
        }

        public static CustomerReminder.ReminderCondition ToEntity(this CustomerReminderModel.ConditionModel model, CustomerReminder.ReminderCondition destination)
        {
            return model.MapTo(destination);
        }


        #endregion

        #region Gift Cards

        public static GiftCardModel ToModel(this GiftCard entity)
        {
            return entity.MapTo<GiftCard, GiftCardModel>();
        }

        public static GiftCard ToEntity(this GiftCardModel model)
        {
            return model.MapTo<GiftCardModel, GiftCard>();
        }

        public static GiftCard ToEntity(this GiftCardModel model, GiftCard destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Coupons

        public static CouponModel ToModel(this Coupon entity)
        {
            return entity.MapTo<Coupon, CouponModel>();
        }

        public static Coupon ToEntity(this CouponModel model)
        {
            return model.MapTo<CouponModel, Coupon>();
        }

        public static Coupon ToEntity(this CouponModel model, Coupon destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Countries / states

        public static CountryModel ToModel(this Country entity)
        {
            return entity.MapTo<Country, CountryModel>();
        }

        public static Country ToEntity(this CountryModel model)
        {
            return model.MapTo<CountryModel, Country>();
        }

        public static Country ToEntity(this CountryModel model, Country destination)
        {
            return model.MapTo(destination);
        }

        public static StateProvinceModel ToModel(this StateProvince entity)
        {
            return entity.MapTo<StateProvince, StateProvinceModel>();
        }

        public static StateProvince ToEntity(this StateProvinceModel model)
        {
            return model.MapTo<StateProvinceModel, StateProvince>();
        }

        public static StateProvince ToEntity(this StateProvinceModel model, StateProvince destination)
        {
            return model.MapTo(destination);
        }


        #endregion

        #region Settings

        public static TaxSettingsModel ToModel(this TaxSettings entity)
        {
            return entity.MapTo<TaxSettings, TaxSettingsModel>();
        }
        public static TaxSettings ToEntity(this TaxSettingsModel model, TaxSettings destination)
        {
            return model.MapTo(destination);
        }


        public static ShippingSettingsModel ToModel(this ShippingSettings entity)
        {
            return entity.MapTo<ShippingSettings, ShippingSettingsModel>();
        }
        public static ShippingSettings ToEntity(this ShippingSettingsModel model, ShippingSettings destination)
        {
            return model.MapTo(destination);
        }


        public static ForumSettingsModel ToModel(this ForumSettings entity)
        {
            return entity.MapTo<ForumSettings, ForumSettingsModel>();
        }
        public static ForumSettings ToEntity(this ForumSettingsModel model, ForumSettings destination)
        {
            return model.MapTo(destination);
        }


        public static BlogSettingsModel ToModel(this BlogSettings entity)
        {
            return entity.MapTo<BlogSettings, BlogSettingsModel>();
        }
        public static BlogSettings ToEntity(this BlogSettingsModel model, BlogSettings destination)
        {
            return model.MapTo(destination);
        }


        public static VendorSettingsModel ToModel(this VendorSettings entity)
        {
            return entity.MapTo<VendorSettings, VendorSettingsModel>();
        }
        public static VendorSettings ToEntity(this VendorSettingsModel model, VendorSettings destination)
        {
            return model.MapTo(destination);
        }


        public static NewsSettingsModel ToModel(this NewsSettings entity)
        {
            return entity.MapTo<NewsSettings, NewsSettingsModel>();
        }
        public static NewsSettings ToEntity(this NewsSettingsModel model, NewsSettings destination)
        {
            return model.MapTo(destination);
        }


        public static CatalogSettingsModel ToModel(this CatalogSettings entity)
        {
            return entity.MapTo<CatalogSettings, CatalogSettingsModel>();
        }
        public static CatalogSettings ToEntity(this CatalogSettingsModel model, CatalogSettings destination)
        {
            return model.MapTo(destination);
        }


        public static RewardPointsSettingsModel ToModel(this RewardPointsSettings entity)
        {
            return entity.MapTo<RewardPointsSettings, RewardPointsSettingsModel>();
        }
        public static RewardPointsSettings ToEntity(this RewardPointsSettingsModel model, RewardPointsSettings destination)
        {
            return model.MapTo(destination);
        }


        public static OrderSettingsModel ToModel(this OrderSettings entity)
        {
            return entity.MapTo<OrderSettings, OrderSettingsModel>();
        }
        public static OrderSettings ToEntity(this OrderSettingsModel model, OrderSettings destination)
        {
            return model.MapTo(destination);
        }


        public static ShoppingCartSettingsModel ToModel(this ShoppingCartSettings entity)
        {
            return entity.MapTo<ShoppingCartSettings, ShoppingCartSettingsModel>();
        }
        public static ShoppingCartSettings ToEntity(this ShoppingCartSettingsModel model, ShoppingCartSettings destination)
        {
            return model.MapTo(destination);
        }


        public static MediaSettingsModel ToModel(this MediaSettings entity)
        {
            return entity.MapTo<MediaSettings, MediaSettingsModel>();
        }
        public static MediaSettings ToEntity(this MediaSettingsModel model, MediaSettings destination)
        {
            return model.MapTo(destination);
        }


        public static GdprSettingsModel ToModel(this GdprSettings entity)
        {
            return entity.MapTo<GdprSettings, GdprSettingsModel>();
        }
        public static GdprSettings ToEntity(this GdprSettingsModel model, GdprSettings destination)
        {
            return model.MapTo(destination);
        }

        #region PushNotifications

        public static PushNotificationsSettingsModel ToModel(this PushNotificationsSettings entity)
        {
            return entity.MapTo<PushNotificationsSettings, PushNotificationsSettingsModel>();
        }
        public static PushNotificationsSettings ToEntity(this PushNotificationsSettingsModel model, PushNotificationsSettings destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        //customer/user settings
        public static CustomerUserSettingsModel.CustomerSettingsModel ToModel(this CustomerSettings entity)
        {
            return entity.MapTo<CustomerSettings, CustomerUserSettingsModel.CustomerSettingsModel>();
        }
        public static CustomerSettings ToEntity(this CustomerUserSettingsModel.CustomerSettingsModel model, CustomerSettings destination)
        {
            return model.MapTo(destination);
        }
        public static CustomerUserSettingsModel.AddressSettingsModel ToModel(this AddressSettings entity)
        {
            return entity.MapTo<AddressSettings, CustomerUserSettingsModel.AddressSettingsModel>();
        }
        public static AddressSettings ToEntity(this CustomerUserSettingsModel.AddressSettingsModel model, AddressSettings destination)
        {
            return model.MapTo(destination);
        }



        //general (captcha) settings
        public static GeneralCommonSettingsModel.CaptchaSettingsModel ToModel(this CaptchaSettings entity)
        {
            return entity.MapTo<CaptchaSettings, GeneralCommonSettingsModel.CaptchaSettingsModel>();
        }
        public static CaptchaSettings ToEntity(this GeneralCommonSettingsModel.CaptchaSettingsModel model, CaptchaSettings destination)
        {
            return model.MapTo(destination);
        }

        //product editor settings
        public static ProductEditorSettingsModel ToModel(this ProductEditorSettings entity)
        {
            return entity.MapTo<ProductEditorSettings, ProductEditorSettingsModel>();
        }
        public static ProductEditorSettings ToEntity(this ProductEditorSettingsModel model, ProductEditorSettings destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region Plugins

        public static PluginModel ToModel(this PluginDescriptor entity)
        {
            return entity.MapTo<PluginDescriptor, PluginModel>();
        }

        #endregion

        #region Stores

        public static StoreModel ToModel(this Store entity)
        {
            return entity.MapTo<Store, StoreModel>();
        }

        public static Store ToEntity(this StoreModel model)
        {
            return model.MapTo<StoreModel, Store>();
        }

        public static Store ToEntity(this StoreModel model, Store destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Templates

        public static CategoryTemplateModel ToModel(this CategoryTemplate entity)
        {
            return entity.MapTo<CategoryTemplate, CategoryTemplateModel>();
        }

        public static CategoryTemplate ToEntity(this CategoryTemplateModel model)
        {
            return model.MapTo<CategoryTemplateModel, CategoryTemplate>();
        }

        public static CategoryTemplate ToEntity(this CategoryTemplateModel model, CategoryTemplate destination)
        {
            return model.MapTo(destination);
        }


        public static ManufacturerTemplateModel ToModel(this ManufacturerTemplate entity)
        {
            return entity.MapTo<ManufacturerTemplate, ManufacturerTemplateModel>();
        }

        public static ManufacturerTemplate ToEntity(this ManufacturerTemplateModel model)
        {
            return model.MapTo<ManufacturerTemplateModel, ManufacturerTemplate>();
        }

        public static ManufacturerTemplate ToEntity(this ManufacturerTemplateModel model, ManufacturerTemplate destination)
        {
            return model.MapTo(destination);
        }


        public static ProductTemplateModel ToModel(this ProductTemplate entity)
        {
            return entity.MapTo<ProductTemplate, ProductTemplateModel>();
        }

        public static ProductTemplate ToEntity(this ProductTemplateModel model)
        {
            return model.MapTo<ProductTemplateModel, ProductTemplate>();
        }

        public static ProductTemplate ToEntity(this ProductTemplateModel model, ProductTemplate destination)
        {
            return model.MapTo(destination);
        }



        public static TopicTemplateModel ToModel(this TopicTemplate entity)
        {
            return entity.MapTo<TopicTemplate, TopicTemplateModel>();
        }

        public static TopicTemplate ToEntity(this TopicTemplateModel model)
        {
            return model.MapTo<TopicTemplateModel, TopicTemplate>();
        }

        public static TopicTemplate ToEntity(this TopicTemplateModel model, TopicTemplate destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Return request reason

        public static ReturnRequestReasonModel ToModel(this ReturnRequestReason entity)
        {
            return entity.MapTo<ReturnRequestReason, ReturnRequestReasonModel>();
        }

        public static ReturnRequestReason ToEntity(this ReturnRequestReasonModel model)
        {
            return model.MapTo<ReturnRequestReasonModel, ReturnRequestReason>();
        }

        public static ReturnRequestReason ToEntity(this ReturnRequestReasonModel model, ReturnRequestReason destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Return request action

        public static ReturnRequestActionModel ToModel(this ReturnRequestAction entity)
        {
            return entity.MapTo<ReturnRequestAction, ReturnRequestActionModel>();
        }

        public static ReturnRequestAction ToEntity(this ReturnRequestActionModel model)
        {
            return model.MapTo<ReturnRequestActionModel, ReturnRequestAction>();
        }

        public static ReturnRequestAction ToEntity(this ReturnRequestActionModel model, ReturnRequestAction destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Gdpr Consent

        public static GdprConsentModel ToModel(this GdprConsent entity)
        {
            return entity.MapTo<GdprConsent, GdprConsentModel>();
        }

        public static GdprConsent ToEntity(this GdprConsentModel model)
        {
            return model.MapTo<GdprConsentModel, GdprConsent>();
        }

        public static GdprConsent ToEntity(this GdprConsentModel model, GdprConsent destination)
        {
            return model.MapTo(destination);
        }

        #endregion

    }
}