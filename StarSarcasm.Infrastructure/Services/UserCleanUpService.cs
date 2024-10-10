using Microsoft.EntityFrameworkCore;
using StarSarcasm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class UserCleanUpService
    {
        private readonly Context _context;

        public UserCleanUpService(Context context)
        {
            _context = context;
        }

        public async Task CleanUpUnconfirmedUsersAsync()
        {

                var timeThreshold = DateTime.Now.AddHours(-48);
                var unConfirmedUsers = await _context.Users
                    .Where(u => !u.EmailConfirmed && u.RegisteredAt < timeThreshold)
                    .ToListAsync();

                if (unConfirmedUsers.Any())
                {
                    _context.Users.RemoveRange(unConfirmedUsers);
                    await _context.SaveChangesAsync();
                }
            }
    }
}
