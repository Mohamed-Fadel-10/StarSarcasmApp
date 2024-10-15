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

        public ChatHub(Context context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"User connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"User disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            Console.WriteLine($"User {Context.ConnectionId} joined chat group {chatId}");
        }

        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
            Console.WriteLine($"User {Context.ConnectionId} left chat group {chatId}");
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

                var groupName = chat.Id.ToString();  
                await Clients.Group(groupName).SendAsync("ReceiveMessage", message.Content);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the message.", ex);
            }
        }
    }
}
