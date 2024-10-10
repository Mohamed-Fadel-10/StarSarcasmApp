using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
    public class AwardDrawService:IAwardDrawService
    {
        private readonly Context _context;
        private readonly IUserService _userService;
        private readonly IUserDrawService _userDrawService;

        public AwardDrawService(Context context, IUserService userService, IUserDrawService userDrawService)
        {
            _context = context;
            _userService = userService;
            _userDrawService = userDrawService;
        }

        public async Task<Draw> GetActiveDrawAsync()
        {
            var draw = await _context.Draws
                .Include(d=>d.UsersDraws).FirstOrDefaultAsync(d =>
            DateTime.Now >= d.StartAt && DateTime.Now <= d.EndAt);

            if (draw != null)
            {
                draw.SubscribersNumber = draw.UsersDraws.Count;
                return draw;
            }
            return new Draw();
        }

        public async Task<Draw> AddAsync(AwardDrawDTO dto)
        {
            var subscribers = await _userService.GetAllSubscribers();

            var result = await _context.Draws.AddAsync(new Draw
            {
                Name = dto.Name,
                Description = dto.Description,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                ImagePath = dto.ImagePath,
                SubscribersNumber=subscribers.Count,
            });

            await _context.SaveChangesAsync();
            foreach (var subscriber in subscribers)
            {
                await _userDrawService.AddAsync(result.Entity, subscriber);
            }

            return result.Entity;
        }

    }

}
