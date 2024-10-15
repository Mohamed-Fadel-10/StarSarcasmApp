using Microsoft.EntityFrameworkCore;
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
            var chats = await _context.UsersChats.Join(_context.Chat,
                uc => uc.ChatId,
                c => c.Id,
                (uc, c) => new { UsersChate = uc, Chat = c })
                .Where(u=>(u.UsersChate.User1==id)|| (u.UsersChate.User2 == id))
                .Select(c => c.Chat)
                .ToListAsync();
            if(chats.Count == 0)
            {
                return new ResponseModel
                {
                    StatusCode = 400,
                    Message = "لا توجد محادثات قمت باجرائها حتى الان",
                    IsSuccess = false,
                };
            }
            return new ResponseModel
            {
                StatusCode = 200,
                Message = "Done",
                IsSuccess = true,
                Model=chats
            };
        }
    }
}
