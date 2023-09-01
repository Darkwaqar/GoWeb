using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Order;

namespace Nop.Web.Validators.Customer
{
    public class AddOrderNoteValidator : BaseNopValidator<AddOrderNoteModel>
    {
        public AddOrderNoteValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Note).NotEmpty().WithMessage(localizationService.GetResource("OrderNote.Fields.Title.Required"));
        }
    }
}