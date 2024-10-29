using Microsoft.AspNetCore.Http;
using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Response;
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
        public Task<ResponseModel> GetActiveDrawAsync();
        public Task<ResponseModel> AddAsync(DrawDTO dto);
        public Task<ResponseModel> RandomDrawWinner(int drawId);
        public Task<ResponseModel> UpdateAsync(int id, DrawDTO dto);
        public Task<ResponseModel> DeleteAsync(int id);
        public Task<ResponseModel> GetAllSubscribers(int id);
        public Task<ResponseModel> GetLastFourDraws();
        public Task<ResponseModel> GetAll();

    }
}
