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
    public class UserService : IUserService
    {
        private readonly Context _context;

        public UserService(Context context)
        {
            _context = context;
        }

        public async Task<List<UserDTO>> GetAll()
        {
            var dto=new List<UserDTO>();
            var users= await _context.Users.ToListAsync();

            if(!users.Any())
            {
                return new List<UserDTO>();
            }

            foreach (var user in users)
            {
                var userDto=new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FcmToken = user.FcmToken,
                    IsSubscribed = user.IsSubscribed,
                };
                dto.Add(userDto);
            }

            return dto;

        }
    }
}
