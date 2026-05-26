using Microsoft.AspNetCore.Http;


namespace BSuit.Infrastructure.Files
{
    public interface IFileService
    {
        Task<string> SaveAsync(IFormFile file, string folder);
        Task<byte[]> ReadAsync(string path);
    }
}
