using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StarSarcasm.Application.Interfaces.IFileUploadService
{
    public interface IFileUploadService
    {
        public Task<string> SaveFileAsync(IFormFile file, string folderName = "general");
        public string GetFileUrl(string path);
    }
}
