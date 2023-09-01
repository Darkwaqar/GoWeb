using Microsoft.AspNet.SignalR;

namespace Nop.Web.Infrastructure
{
    public class ChatHub : Hub
    {
        public void Send(int ToCustomerId ,int FromCustomerId)
        {
            Clients.All.addNewMessageToPage(ToCustomerId, FromCustomerId);
        }
    }
}