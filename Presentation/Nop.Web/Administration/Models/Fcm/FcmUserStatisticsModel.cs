using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Fcm
{
    public class FcmUserStatisticsModel : BaseNopModel
    {
        public int NumberOfApplications { get; set; }

        public int NumberOfNotificationSend { get; set; }

        public int NumberOfActions { get; set; }

        public int NumberOfLastMonth { get; set; }
    }
}