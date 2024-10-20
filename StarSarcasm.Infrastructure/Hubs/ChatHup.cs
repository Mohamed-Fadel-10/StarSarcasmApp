using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Domain.Enums;
using StarSarcasm.Infrastructure.Data;

namespace StarSarcasm.Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Context _context;

        private static Dictionary<string, string> connectedUsers = new Dictionary<string, string>();

        public ChatHub(Context context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext().Request.Query["userId"].ToString();
            var connectionId = Context.ConnectionId;

            if (!string.IsNullOrEmpty(userId))
            {
                if (!connectedUsers.ContainsKey(userId))
                {
                    connectedUsers.Add(userId, connectionId);
                }
                else
                {
                    connectedUsers[userId] = connectionId;
                }

               // Console.WriteLine($"User {userId} connected with connection ID: {connectionId}");
               // await Clients.Caller.SendAsync("ReceiveMessage", "System", $"You are connected as {userId}");

                var unreadMessages = await _context.ChatMessages
                    .Where(m => m.ReciverId == userId && !m.IsReaded)
                    .ToListAsync();

                foreach (var unreadMessage in unreadMessages)
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", unreadMessage.SenderId, unreadMessage.Content,unreadMessage.SendAt);
                    unreadMessage.IsReaded = true;
                }

                await _context.SaveChangesAsync();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            var userId = connectedUsers.FirstOrDefault(x => x.Value == connectionId).Key;

            if (!string.IsNullOrEmpty(userId))
            {
                connectedUsers.Remove(userId); 
                Console.WriteLine($"User {userId} disconnected.");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageDTO model)
        {
            if (model == null || string.IsNullOrEmpty(model.SenderId) || string.IsNullOrEmpty(model.ReciverId))
            {
                throw new ArgumentException("Invalid message data.");
            }

            var chat = await _context.UsersChats
                .Where(uc => (uc.User1 == model.SenderId && uc.User2 == model.ReciverId) ||
                             (uc.User1 == model.ReciverId && uc.User2 == model.SenderId))
                .Select(uc => uc.Chat)
                .FirstOrDefaultAsync();

            if (chat == null)
            {
                var reciverName = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.ReciverId);
                chat = new Chat
                {
                   Id = $"{model.SenderId}-{model.ReciverId}",
                    Name = reciverName.Name,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false,
                };

                await _context.Chat.AddAsync(chat);
                await _context.SaveChangesAsync();

                var usersChat = new UsersChats
                {
                    ChatId = chat.Id,
                    User1 = model.SenderId,
                    User2 = model.ReciverId,
                };

                await _context.UsersChats.AddAsync(usersChat);
                await _context.SaveChangesAsync();
            }

            var message = new ChatMessages
            {
                Type = (MessageType)model.Type,
                Content = model.Content,
                IsDeleted = false,
                IsModified = false,
                IsReaded = false,
                SendAt = DateTime.UtcNow,
                ChatId = chat.Id,
                SenderId = model.SenderId,
                ReciverId = model.ReciverId
            };

            try
            {
                await _context.ChatMessages.AddAsync(message);
                await _context.SaveChangesAsync();

                if (connectedUsers.ContainsKey(model.ReciverId))
                {
                    var receiverConnectionId = connectedUsers[model.ReciverId];
                    await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", model.SenderId, model.Content,message.SendAt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the message.", ex);
            }
        }
    }
}
