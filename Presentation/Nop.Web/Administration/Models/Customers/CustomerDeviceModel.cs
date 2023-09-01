using Nop.Admin.Models.Common;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class CustomerDeviceModel : BaseNopModel
    {
        public int CustomerId { get; set; }

        public DeviceModel Device { get; set; }
    }
}