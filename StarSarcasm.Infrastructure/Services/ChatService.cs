using Microsoft.EntityFrameworkCore;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Application.Response;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class ChatService: IChatService
    {
        private readonly Context _context;
        public ChatService(Context context)
        {
            _context =context;
        }
        public async Task<ResponseModel> GetUserChats(string id)
        {
            var chats = await _context.UsersChats
                .Include(uc => uc.Chat)
                .Include(uc => uc.Sender) 
                .Include(uc => uc.Receiver) 
                .Where(uc => uc.User1 == id || uc.User2 == id)
                .ToListAsync();

            if (chats.Count == 0)
            {
                return new ResponseModel
                {
                    StatusCode = 400,
                    Message = "لا توجد محادثات قمت باجرائها حتى الان",
                    IsSuccess = false,
                };
            }

            var chatsDTO = new List<UserChatDTO>();
            foreach (var chat in chats)
            {
                var receiver = chat.User1 == id ? chat.Receiver : chat.Sender;

                var item = new UserChatDTO
                {
                    ChatId = chat.Chat.Id,
                    ChatName = chat.Chat.Name,
                    ReceiverDate = (DateTime)(receiver?.BirthDate),
                    ReceiverId = receiver?.Id,
                    FcmToken =receiver.FcmToken,
                    Longitude=receiver.Longitude,
                    Latitude=receiver.Latitude,
                };

                chatsDTO.Add(item);
            }

            return new ResponseModel
            {
                StatusCode = 200,
                Message = "Done",
                IsSuccess = true,
                Model = chatsDTO
            };
        }


    }
}
