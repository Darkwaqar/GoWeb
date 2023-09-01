using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Common;
using Nop.Web.Models.Customer;

namespace Nop.Web.MServices.Models.Common
{
    public partial class SaveAddressModelAPI
    {

        public AddressMobileModel BillingAddress { get; set; }
        public AddressMobileModel ShippingAddress { get; set; }
        public bool SameAsShipping { get; set; }
        public List<AddressMobileModel> Addresses { get; set; }
    
    }
}