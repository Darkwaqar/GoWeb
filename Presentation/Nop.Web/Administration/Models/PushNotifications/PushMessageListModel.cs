using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Admin.Models.PushNotifications
{
    public partial class PushMessageListModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime SentOn { get; set; }

        public int NumberOfReceivers { get; set; }
    }
}