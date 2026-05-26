
namespace BSuit.Infrastructure.Graphics
{
    public interface IImageService
    {
        Task ResizeAsync(Stream input, string outputPath, int width, int height);

        Task<List<string>> ResizeMultipleAsync(Stream input, string folderPath, List<(int w, int h)> sizes);

        Task GenerateFaviconAsync(Stream input, string outputPath);

        Task<string> ToBase64Async(Stream input);
        Task<byte[]> FromBase64Async(string base64);

        Task SaveAsync(Stream input, string path);
    }
}
