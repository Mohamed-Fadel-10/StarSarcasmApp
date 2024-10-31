using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Domain.Enums;
using StarSarcasm.Infrastructure.Data;

namespace StarSarcasm.Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static Dictionary<string, string> connectedUsers = new Dictionary<string, string>();

        public ChatHub(Context context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
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
                    await Clients.Caller.SendAsync("ReceiveMessage", unreadMessage.SenderId, unreadMessage.Content,unreadMessage.SendAt,unreadMessage.Type);
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

        #region ChatHup Text Only
        //public async Task SendMessage(SendMessageDTO model)
        //{
        //    if (model == null || string.IsNullOrEmpty(model.SenderId) || string.IsNullOrEmpty(model.ReciverId))
        //    {
        //        throw new ArgumentException("Invalid message data.");
        //    }

        //    var chat = await _context.UsersChats
        //        .Where(uc => (uc.User1 == model.SenderId && uc.User2 == model.ReciverId) ||
        //                     (uc.User1 == model.ReciverId && uc.User2 == model.SenderId))
        //        .Select(uc => uc.Chat)
        //        .FirstOrDefaultAsync();

        //    if (chat == null)
        //    {
        //        var reciverName = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.ReciverId);
        //        chat = new Chat
        //        {
        //           Id = $"{model.SenderId}-{model.ReciverId}",
        //            Name = reciverName.Name,
        //            CreatedAt = DateTime.Now,
        //            IsDeleted = false,
        //        };

        //        await _context.Chat.AddAsync(chat);
        //        await _context.SaveChangesAsync();

        //        var usersChat = new UsersChats
        //        {
        //            ChatId = chat.Id,
        //            User1 = model.SenderId,
        //            User2 = model.ReciverId,
        //        };

        //        await _context.UsersChats.AddAsync(usersChat);
        //        await _context.SaveChangesAsync();
        //    }

        //    var message = new ChatMessages
        //    {
        //        Type = (MessageType)model.Type,
        //        Content = model.Content,
        //        IsDeleted = false,
        //        IsModified = false,
        //        IsReaded = false,
        //        SendAt = DateTime.UtcNow,
        //        ChatId = chat.Id,
        //        SenderId = model.SenderId,
        //        ReciverId = model.ReciverId
        //    };

        //    try
        //    {
        //        await _context.ChatMessages.AddAsync(message);
        //        await _context.SaveChangesAsync();

        //        if (connectedUsers.ContainsKey(model.ReciverId))
        //        {
        //            var receiverConnectionId = connectedUsers[model.ReciverId];
        //            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", model.SenderId, model.Content,message.SendAt);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while saving the message.", ex);
        //    }
        //} 
        #endregion

 
        public async Task<ResponseModel> SendMessage(SendMessageDTO model)
        {
            if (model == null || string.IsNullOrEmpty(model.SenderId) || string.IsNullOrEmpty(model.ReciverId))
            {
                return new ResponseModel { IsSuccess=false,Message="Invalid Model Data For Message"};
            }

            var chat = await _context.UsersChats
                .Where(uc => (uc.User1 == model.SenderId && uc.User2 == model.ReciverId) ||
                             (uc.User1 == model.ReciverId && uc.User2 == model.SenderId))
                .Select(uc => uc.Chat)
                .FirstOrDefaultAsync();

            if (chat == null)
            {
                var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.SenderId);
                var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.ReciverId);

                if (sender == null || receiver == null)
                {
                    return  new ResponseModel { IsSuccess = false, Message = "Sender or receiver not found." };
                }

                chat = new Chat
                {
                    Id = $"{model.SenderId}%{model.ReciverId}",
                    SenderChatName = sender.Name,
                    ReciverChatName = receiver.Name,
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

            if (model.Type == 1 && !string.IsNullOrEmpty(model.VoiceFile))
            {
                var uploadResult = await SaveVoiceFileAsync(model);
                if (!uploadResult.IsSuccess)
                {
                    return uploadResult; 
                }
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

            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            if (connectedUsers.ContainsKey(model.ReciverId))
            {
                var receiverConnectionId = connectedUsers[model.ReciverId];
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", model.SenderId, model.Content, message.SendAt, message.Type);
            }

            return new ResponseModel { IsSuccess = true, Message = "Message Sent Successfully" };
        }

        private async Task<ResponseModel> SaveVoiceFileAsync(SendMessageDTO model)
        {
            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "voices");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + ".mp3";
                var filePath = Path.Combine(uploadsFolder, fileName);

                var base64Data = model.VoiceFile.Substring(model.VoiceFile.IndexOf(',') + 1);
                var fileBytes = Convert.FromBase64String(base64Data);

                await File.WriteAllBytesAsync(filePath, fileBytes);

                var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}";
                model.Content = $"{baseUrl}/voices/{fileName}";

                return new ResponseModel { IsSuccess = true, Message = "Voice Stored Successfully" };
            }
            catch
            {
                return new ResponseModel { IsSuccess = false, Message = "An error occurred while uploading the voice file." };
            }
        }



    }
}
