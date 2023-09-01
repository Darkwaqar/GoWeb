using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;

namespace Nop.Services.Authentication
{
    public interface ITwoFactorAuthenticationService
    {
        bool AuthenticateTwoFactor(string secretKey, string token, Customer customer, TwoFactorAuthenticationType twoFactorAuthenticationType);

        TwoFactorCodeSetup GenerateCodeSetup(string secretKey, Customer customer, Language language, TwoFactorAuthenticationType twoFactorAuthenticationType);
    }
}
