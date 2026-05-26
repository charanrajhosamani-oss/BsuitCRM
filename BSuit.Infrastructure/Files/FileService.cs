using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.Files
{
    public class FileService : IFileService
    {
        public async Task<string> SaveAsync(IFormFile file, string folder)
        {
            var path = Path.Combine("wwwroot", folder);
            Directory.CreateDirectory(path);

            var filePath = Path.Combine(path, file.FileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return filePath;
        }

        public async Task<byte[]> ReadAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }
    }
}
