using StarSarcasm.Application.DTOs;
using StarSarcasm.Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.Interfaces
{
    public interface IPostservice
    {
        public Task<ResponseModel> AddPost(PostDTO model);
    }
}
