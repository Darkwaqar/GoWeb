using Google.Authenticator;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Messages;
using System;

namespace Nop.Services.Authentication
{
    public class TwoFactorAuthenticationService : ITwoFactorAuthenticationService
    {
        private readonly IStoreContext _storeContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IGenericAttributeService _genericAttributeService;
        private TwoFactorAuthenticator _twoFactorAuthentication;

        public TwoFactorAuthenticationService(
            IStoreContext storeContext,
            IWorkflowMessageService workflowMessageService,
            IGenericAttributeService genericAttributeService)
        {
            this._storeContext = storeContext;
            this._workflowMessageService = workflowMessageService;
            this._genericAttributeService = genericAttributeService;
            this._twoFactorAuthentication = new TwoFactorAuthenticator();
        }

        public virtual bool AuthenticateTwoFactor(string secretKey, string token, Customer customer, TwoFactorAuthenticationType twoFactorAuthenticationType)
        {
            switch (twoFactorAuthenticationType)
            {
                case TwoFactorAuthenticationType.AppVerification:
                    return _twoFactorAuthentication.ValidateTwoFactorPIN(secretKey, token.Trim());

                case TwoFactorAuthenticationType.EmailVerification:
                    var customertoken = customer.GetAttribute<string>(SystemCustomerAttributeNames.TwoFactorValidCode);
                    if (customertoken != token.Trim())
                        return false;
                    var validuntil = customer.GetAttribute<DateTime>(SystemCustomerAttributeNames.TwoFactorCodeValidUntil);
                    if (validuntil < DateTime.UtcNow)
                        return false;

                    return true;
                case TwoFactorAuthenticationType.SMSVerification:
                    var smsVerificationService = EngineContext.Current.Resolve<ISMSVerificationService>();
                    return  smsVerificationService.Authenticate(secretKey, token.Trim(), customer);
                default:
                    return false;
            }
        }

        public virtual TwoFactorCodeSetup GenerateCodeSetup(string secretKey, Customer customer, Language language, TwoFactorAuthenticationType twoFactorAuthenticationType)
        {
            var model = new TwoFactorCodeSetup();

            switch (twoFactorAuthenticationType)
            {
                case TwoFactorAuthenticationType.AppVerification:
                    var setupInfo = _twoFactorAuthentication.GenerateSetupCode(_storeContext.CurrentStore.CompanyName, customer.Email, secretKey, false, 3);
                    model.CustomValues.Add("QrCodeImageUrl", setupInfo.QrCodeSetupImageUrl);
                    model.CustomValues.Add("ManualEntryQrCode", setupInfo.ManualEntryKey);
                    break;

                case TwoFactorAuthenticationType.EmailVerification:
                    var token = PrepareRandomCode();
                     _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TwoFactorValidCode, token);
                     _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TwoFactorCodeValidUntil, DateTime.UtcNow.AddMinutes(30));
                    model.CustomValues.Add("Token", token);
                     _workflowMessageService.SendCustomerEmailTokenValidationMessage(customer, language.Id);
                    break;

                case TwoFactorAuthenticationType.SMSVerification:
                    var smsVerificationService = EngineContext.Current.Resolve<ISMSVerificationService>();
                    model =  smsVerificationService.GenerateCode(secretKey, customer, language);
                    break;

                default:
                    break;
            }

            return model;
        }

        private string PrepareRandomCode()
        {
            Random generator = new Random();
            return generator.Next(0, 999999).ToString("D6");
        }
    }
}
