using Microsoft.EntityFrameworkCore;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Response;
using StarSarcasm.Domain.Enums;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class ChatMessageService: IChatMessageService
    {
        private readonly Context _context;
        public ChatMessageService(Context context)
        {
              _context = context;
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
                    Type= (int)message.Type,
                };
                chatMessages.Add(Message);
            }
            return messages.Any() ? new ResponseModel { IsSuccess = true, StatusCode = 200, Model = chatMessages } :
                new ResponseModel { IsSuccess = false, StatusCode = 200, Message = "No Messages For This Chat" };
        }
    }
}
