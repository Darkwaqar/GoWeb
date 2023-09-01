using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;

namespace Nop.Services.Authentication
{
    public interface ISMSVerificationService
    {
        bool Authenticate(string secretKey, string token, Customer customer);
        TwoFactorCodeSetup GenerateCode(string secretKey, Customer customer, Language language);
    }
}
