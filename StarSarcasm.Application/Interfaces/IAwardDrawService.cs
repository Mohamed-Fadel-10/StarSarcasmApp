using StarSarcasm.Application.DTOs;
using StarSarcasm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.Interfaces
{
    public interface IAwardDrawService
    {
        Task<Draw> GetActiveDrawAsync();
        Task<Draw> AddAsync(AwardDrawDTO dto);
    }
}
