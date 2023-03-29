using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Web.Mvc;



namespace _1640WebApp.Hubs
{
    
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string username, string message)
        {
            await Clients.All.SendAsync("RecieveMessage", username, message);
        }

        public Task SendMessagetoGroup(string receiver, string message)
        {
            return Clients.Group(receiver).SendAsync("RecieveMessage", Context.User.Identity.Name, message);
        }
    }
}
