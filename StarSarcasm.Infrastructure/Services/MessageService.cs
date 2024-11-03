using Microsoft.EntityFrameworkCore;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.DTOs.Message;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class MessageService:IMessageService
    {
        private readonly Context _context;
        private readonly FirebaseNotificationService _firebaseNotificationService;

        public MessageService(Context context, FirebaseNotificationService firebaseNotificationService)
        {
            _context = context;
            _firebaseNotificationService = firebaseNotificationService;
        }

        public async Task<List<Message>> GetRandomMessages(int count)
        {
            var userMessagesIds = await _context
                .UsersMessages
                .Select(um => um.MessageId)
                .ToListAsync();

            var messages = await _context.Messages
                .Where(m => !userMessagesIds.Contains(m.Id)) 
                .OrderBy(r => Guid.NewGuid())
                .Take(count) 
                .ToListAsync();

            return messages;
        }


        private async Task<ResponseModel> SendMessagesToUsers(List<Message> messages, bool IsSubscribed)
        {
            var users = await _context.Users.Include(u=>u.RefreshTokens)
                .Where(u => u.IsSubscribed == IsSubscribed)
                .ToListAsync();

            foreach (var user in users)
            {
                var isLoggedIn = user.RefreshTokens!.Any(r => r.IsActive);
				if (!isLoggedIn)
                {
                    //not create or send message if he is not logged in
                    continue;
                }

                foreach (var message in messages)
                {
                    await _context.UsersMessages.AddAsync(new UsersMessages
                    {
                        MessageId = message.Id,
                        UserId = user.Id,
                        SendAt = DateTime.Now,
                    });

                    if (!string.IsNullOrEmpty(user.FcmToken))
                    {
                        await _firebaseNotificationService.SendNotificationAsync(
                            user.FcmToken,
                            "New Message",
                            message.Content
                        );
                    }
                }
            }

            await _context.SaveChangesAsync();
            return new ResponseModel { IsSuccess = true, Message = "Messages Sent and Notifications Sent Successfully" };
        }

        public async Task SendMessagesToSubscribedUsers()
        {
            var messages = await GetRandomMessages(1);
            await SendMessagesToUsers(messages, true);
        }
        public async Task SendMessagesToUnSubscribedUsers()
        {
            var messages = await GetRandomMessages(1);
            await SendMessagesToUsers(messages, false);
        }


        public async Task<List<MessageDTO>> GetUserMessages(string userId)
        {
            var messages = await _context.UsersMessages
                .Join(_context.Users,
                um => um.UserId,
                u => u.Id,
                (um, u) => new { UserMessages = um, User = u })
                .Join(_context.Messages,
                um => um.UserMessages.MessageId,
                m => m.Id,
                (um, m) => new { UserMessages = um, Message = m }).ToListAsync();

            var userMessages = messages
                              .Where(u => u.UserMessages.User.Id == userId)
                              .Select(m => new MessageDTO
                              {
                                  Id = m.Message.Id,
                                  Title = m.Message.Title,
                                  Content = m.Message.Content,
                                  SandedAt=m.UserMessages.UserMessages.SendAt,
                              })
                              .ToList();

            return userMessages.Any() ? userMessages : new List<MessageDTO>();
        }

        public async Task<ResponseModel> GetMessagesForChat(string chatId)
        {
            var messages = await _context.ChatMessages
                .Where(c => c.ChatId == chatId)
                .ToListAsync();
            var chatMessages = new List<ChatMessageDTO>();

            foreach (var message in messages)
            {
                var Message = new ChatMessageDTO()
                {
                    Id = message.Id,
                    Content = message.Content,
                    ChatId = message.ChatId,
                    ReciverId = message.ReciverId,
                    SendAt = message.SendAt,
                    SenderId = message.SenderId,
                };
                chatMessages.Add(Message);
            }
            return messages.Any() ? new ResponseModel { IsSuccess = true, StatusCode = 200, Model = chatMessages } :
                new ResponseModel { IsSuccess = false, StatusCode = 404, Message = "No Messages For This Chat" };
        }

    }
}
