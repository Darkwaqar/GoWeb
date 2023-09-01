using FluentValidation.Attributes;
using Nop.Admin.Validators.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;


namespace Nop.Admin.Models.Customers
{
    [Validator(typeof(CustomerTagValidator))]
    public partial class CustomerTagModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Customers.CustomerTags.Fields.Name")]
        public string Name { get; set; }
    }
}