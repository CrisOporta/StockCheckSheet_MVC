using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StockCheckSheet.DataAccess.Data;
using StockCheckSheet.Models;
using System.Security.Claims;

namespace StockCheckSheet.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            var messages = _db.ChatMessages.OrderBy(m => m.Timestamp).ToList();

            foreach (var message in messages)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", message.Sender, message.Message);
            }

            await base.OnConnectedAsync();
        }


        public async Task SendMessageToAll(string user, string message)
        {
            var chatMessage = new ChatMessage
            {
                Sender = user,
                Receiver = null,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            _db.ChatMessages.Add(chatMessage);
            await _db.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        [Authorize]
        public async Task SendMessageToReceiver(string sender, string receiver, string message)
        {
            var receiverId = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == receiver.ToLower())?.Id;

            var claimsIdentity = (ClaimsIdentity)Context.User.Identity;
            var senderId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!string.IsNullOrEmpty(receiverId))
            {
                var chatMessage = new ChatMessage
                {
                    Sender = sender,
                    Receiver = receiver,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };
                _db.ChatMessages.Add(chatMessage);
                await _db.SaveChangesAsync();

                await Clients.User(receiverId).SendAsync("ReceiveMessage", sender, message);
                await Clients.User(senderId).SendAsync("ReceiveMessage", sender, message);
            }
        }
    }
}
