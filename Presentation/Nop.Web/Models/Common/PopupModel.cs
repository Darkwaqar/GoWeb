using Nop.Core.Domain.Messages;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public class PopupModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public int CustomerActionId { get; set; }
        public PopupType PopupType { get; set; }
    }
}