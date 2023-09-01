using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Customer;

namespace Nop.Web.Models.Order
{
    [Validator(typeof(AddOrderNoteValidator))]
    public partial class AddOrderNoteModel : BaseNopModel
    {
        public int OrderId { get; set; }
        public string Note { get; set; }
    }
}