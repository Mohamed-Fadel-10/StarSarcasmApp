using Microsoft.AspNetCore.Http;
using StarSarcasm.Application.Interfaces.IFileUploadService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services.FileUploadService
{
    public class FileUploadService: IFileUploadService
    {
        private readonly string _uploadsFolder;

        public FileUploadService()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "StarSarcasm.Infrastructure"));
            _uploadsFolder = Path.Combine(projectRoot, "Uploads", "images");

            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return uniqueFileName; 
            }
            return string.Empty; 
        }


        public string GetFilePath(string fileName)
        {
            return Path.Combine(_uploadsFolder, fileName);
        }
    }
}
