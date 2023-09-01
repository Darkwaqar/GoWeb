using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Fcm
{
    public partial class DeviceDistanceModel : BaseNopModel
    {
        public int Id { get; set; }

        public decimal Latitute { get; set; }

        public decimal Longitude { get; set; }

        public double Distance { get; set; }
    }
}