
using System.Collections.Generic;

namespace Nop.Web.Models.PrivateMessages
{
    public partial class PrivateMessageIndexModel
    {
        public PrivateMessageIndexModel()
        {
            InboxModel = new List<PrivateMessageModel>();
            ConversationsModel = new List<PrivateMessageModel>();
        }
        public List<PrivateMessageModel> InboxModel {get;set ;}
        public List<PrivateMessageModel> ConversationsModel { get;set ;}

        public int NumberOfUnreadMessage { get;  set; }
        public int FromCustomerId { get; internal set; }

        public int InboxPage { get; set; }
        public int SentItemsPage { get; set; }
        public bool SentItemsTabSelected { get; set; }

        public int ToCustomerId { get; set; }

        public string Message { get; set; }
        public string Inbox { get;  set; }
        public string Conversations { get;  set; }

        public bool IsGuest { get; set; }
    }
}