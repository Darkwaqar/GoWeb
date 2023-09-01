using Nop.Admin.Models.Customers;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using FluentValidation;

namespace Nop.Admin.Validators.Customers
{
    public class CustomerReminderValidator : BaseNopValidator<CustomerReminderModel>
    {
        public CustomerReminderValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerReminder.Fields.Name.Required"));
            SetDatabaseValidationRules<CustomerReminderModel>(dbContext);
        }
    }
    public class CustomerReminderLevelValidator : BaseNopValidator<CustomerReminderModel.ReminderLevelModel>
    {
        public CustomerReminderLevelValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerReminder.Level.Fields.Name.Required"));
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerReminder.Level.Fields.Subject.Required"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerReminder.Level.Fields.Body.Required"));
            RuleFor(x => x.Hour + x.Day + x.Minutes).NotEqual(0).WithMessage(localizationService.GetResource("Admin.Customers.CustomerReminder.Level.Fields.DayHourMin.Required"));
            SetDatabaseValidationRules<CustomerReminderModel.ReminderLevelModel>(dbContext);
        }
    }
}