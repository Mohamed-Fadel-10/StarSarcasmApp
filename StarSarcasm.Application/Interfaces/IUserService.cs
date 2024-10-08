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
    public interface IUserService
    {
        public  Task<List<UserDTO>> GetAll();
        public Task<ResponseModel> Profile(string id);
    }
}
