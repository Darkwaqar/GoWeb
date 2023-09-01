using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Fcm
{
    public class FcmStatisticsModel : BaseNopModel
    {
        public int NumberOfDevices { get; set; }

        public int NumberOfAndroidDevices { get; set; }

        public int NumberOfIosDevices { get; set; }

        public int NumberOfWebDevices { get; set; }
    }
}