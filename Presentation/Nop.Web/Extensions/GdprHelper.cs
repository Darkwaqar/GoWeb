using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Gdpr;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Models.Customer;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Web.Extensions
{
    public static class GdprHelper
    {
        public static void LogGdpr(this FormCollection form, Customer customer, CustomerInfoModel oldCustomerInfoModel,
          CustomerInfoModel newCustomerInfoModel, IGdprService gdrpService,
          GdprSettings gdprSettings, IWorkContext workContext, ILocalizationService localizationService,
          ICountryService countryService, IStateProvinceService stateProvinceService, ILogger logger)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            try
            {
                //consents
                var consents = gdrpService.GetAllConsents().Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var previousConsentValue = gdrpService.IsConsentAccepted(consent.Id, workContext.CurrentCustomer.Id);
                    var controlId = $"consent{consent.Id}";
                    var cbConsent = form[controlId];
                    if (!String.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                    {
                        //agree
                        if (!previousConsentValue.HasValue || !previousConsentValue.Value)
                        {
                            gdrpService.InsertLog(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                        }
                    }
                    else
                    {
                        //disagree
                        if (!previousConsentValue.HasValue || previousConsentValue.Value)
                        {
                            gdrpService.InsertLog(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                        }
                    }
                }

                //newsletter subscriptions
                if (gdprSettings.LogNewsletterConsent)
                {
                    if (oldCustomerInfoModel.Newsletter && !newCustomerInfoModel.Newsletter)
                        gdrpService.InsertLog(customer, 0, GdprRequestType.ConsentDisagree, localizationService.GetResource("Gdpr.Consent.Newsletter"));
                    if (!oldCustomerInfoModel.Newsletter && newCustomerInfoModel.Newsletter)
                        gdrpService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, localizationService.GetResource("Gdpr.Consent.Newsletter"));
                }

                //user profile changes
                if (!gdprSettings.LogUserProfileChanges)
                    return;

                if (oldCustomerInfoModel.Gender != newCustomerInfoModel.Gender)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.Gender")} = {newCustomerInfoModel.Gender}");

                if (oldCustomerInfoModel.FirstName != newCustomerInfoModel.FirstName)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.FirstName")} = {newCustomerInfoModel.FirstName}");

                if (oldCustomerInfoModel.LastName != newCustomerInfoModel.LastName)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.LastName")} = {newCustomerInfoModel.LastName}");

                if (oldCustomerInfoModel.ParseDateOfBirth() != newCustomerInfoModel.ParseDateOfBirth())
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.DateOfBirth")} = {newCustomerInfoModel.ParseDateOfBirth()}");

                if (oldCustomerInfoModel.Email != newCustomerInfoModel.Email)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.Email")} = {newCustomerInfoModel.Email}");

                if (oldCustomerInfoModel.Company != newCustomerInfoModel.Company)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.Company")} = {newCustomerInfoModel.Company}");

                if (oldCustomerInfoModel.StreetAddress != newCustomerInfoModel.StreetAddress)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.StreetAddress")} = {newCustomerInfoModel.StreetAddress}");

                if (oldCustomerInfoModel.StreetAddress2 != newCustomerInfoModel.StreetAddress2)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.StreetAddress2")} = {newCustomerInfoModel.StreetAddress2}");

                if (oldCustomerInfoModel.ZipPostalCode != newCustomerInfoModel.ZipPostalCode)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.ZipPostalCode")} = {newCustomerInfoModel.ZipPostalCode}");

                if (oldCustomerInfoModel.City != newCustomerInfoModel.City)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.City")} = {newCustomerInfoModel.City}");

                if (oldCustomerInfoModel.County != newCustomerInfoModel.County)
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.County")} = {newCustomerInfoModel.County}");

                if (oldCustomerInfoModel.CountryId != newCustomerInfoModel.CountryId)
                {
                    var countryName = countryService.GetCountryById(newCustomerInfoModel.CountryId)?.Name;
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.Country")} = {countryName}");
                }

                if (oldCustomerInfoModel.StateProvinceId != newCustomerInfoModel.StateProvinceId)
                {
                    var stateProvinceName = stateProvinceService.GetStateProvinceById(newCustomerInfoModel.StateProvinceId)?.Name;
                    gdrpService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{localizationService.GetResource("Account.Fields.StateProvince")} = {stateProvinceName}");
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception.Message, exception, customer);
            }
        }
    }
}