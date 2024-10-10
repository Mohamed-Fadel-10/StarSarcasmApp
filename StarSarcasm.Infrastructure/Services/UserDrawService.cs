using Microsoft.EntityFrameworkCore;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Interfaces;
using StarSarcasm.Domain.Entities;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class UserDrawService : IUserDrawService
    {
        private readonly Context _context;

        public UserDrawService(Context context)
        {
            _context = context;
        }

        public async Task<List<UserDrawDTO>> GetAllAsync()
        {
            List<UserDrawDTO> dto = new();
            var userDraws = await _context.UsersDraws.ToListAsync();

            if (userDraws.Any())
            {
                foreach (var userDraw in userDraws)
                {
                    var draw = new UserDrawDTO()
                    {
                        Id = userDraw.Id,
                        IsWinner = userDraw.IsWinner,
                    };
                    dto.Add(draw);
                }
            }
            return dto;
        }

        public async Task AddAsync(Draw draw, UserDTO user)
        {
            var userDraw = new UsersDraws()
            {
                DrawId = draw.Id,
                UserId = user.Id,
            };

            await _context.UsersDraws.AddAsync(userDraw);
            await _context.SaveChangesAsync();
        }
    }
}
