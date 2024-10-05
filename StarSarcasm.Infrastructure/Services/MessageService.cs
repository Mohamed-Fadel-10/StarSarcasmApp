using Microsoft.EntityFrameworkCore;
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

        public MessageService(Context context, FirebaseNotificationService firebaseNotificationService)
        {
            _context = context;
        }

        public async Task<List<Message>> GetRandomMessages(int count)
        {
            var messages = await _context.Messages
                .OrderBy(r => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
            return messages;
        }

        private async Task<ResponseModel> SendMessagesToUsers(List<Message> messages,bool IsSubscribed)
        {
            var users = await _context.Users
                .Where(u=>u.IsSubscribed == IsSubscribed)
                .ToListAsync();

            foreach (var user in users)
            {
                foreach (var message in messages)
                {
                    await _context.UsersMessages
                        .AddAsync(new UsersMessages
                        {
                            MessageId = message.Id,
                            UserId = user.Id 
                        });
                   
                }      
            }
            await _context.SaveChangesAsync();
            return new ResponseModel { IsSuccess = true, Message = "Messages Sent Successfully" };
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
                um => um.UserMessages.Id,
                m => m.Id,
                (um, m) => new { UserMessages = um, Message = m }).ToListAsync();

            var userMessages= messages
                              .Where(u=>u.UserMessages.User.Id == userId)
                              .Select(m=> new MessageDTO
                              {
                                  Id=m.Message.Id,
                                  Title=m.Message.Title,
                                  Content=m.Message.Content,
                              })
                              .ToList();

            return userMessages.Any()? userMessages : new List<MessageDTO>();
        }

    }
}
