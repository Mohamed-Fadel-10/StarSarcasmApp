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
        public  Task<List<UserDTO>> GetAllSubscribers();
        public Task<ResponseModel> Profile(string id);
        public Task<ResponseModel> RemoveUser(string id);
        public Task<ResponseModel> UsersWithZodiac(int zodiacNum);
        public Task<ResponseModel> UpdateAsync(string id,UserDTO userDTO);
    }
}
