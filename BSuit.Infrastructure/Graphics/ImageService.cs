using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace BSuit.Infrastructure.Graphics
{
   
    public class ImageService : IImageService
    {
        // ✅ Resize Single
        public async Task ResizeAsync(Stream input, string outputPath, int width, int height)
        {
            using var image = await Image.LoadAsync(input);

            image.Mutate(x => x.Resize(width, height));

            await image.SaveAsync(outputPath);
        }

        // ✅ Resize Multiple (Thumbnail + Medium + Large)
        public async Task<List<string>> ResizeMultipleAsync(
            Stream input,
            string folderPath,
            List<(int w, int h)> sizes)
        {
            var paths = new List<string>();

            using var image = await Image.LoadAsync(input);

            foreach (var size in sizes)
            {
                var clone = image.Clone(x => x.Resize(size.w, size.h));

                var fileName = $"{Guid.NewGuid()}_{size.w}x{size.h}.png";
                var fullPath = Path.Combine(folderPath, fileName);

                await clone.SaveAsync(fullPath);

                paths.Add(fullPath);
            }

            return paths;
        }

        // ✅ Generate Favicon (.ico like sizes)
        public async Task GenerateFaviconAsync(Stream input, string outputPath)
        {
            using var image = await Image.LoadAsync(input);

            var sizes = new[] { 16, 32, 48, 64 };

            using var ms = new MemoryStream();

            foreach (var size in sizes)
            {
                var resized = image.Clone(x => x.Resize(size, size));
                await resized.SaveAsync(ms, new PngEncoder());
            }

            await File.WriteAllBytesAsync(outputPath, ms.ToArray());
        }

        // ✅ Image → Base64
        public async Task<string> ToBase64Async(Stream input)
        {
            using var ms = new MemoryStream();
            await input.CopyToAsync(ms);

            return Convert.ToBase64String(ms.ToArray());
        }

        // ✅ Base64 → Image bytes
        public Task<byte[]> FromBase64Async(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            return Task.FromResult(bytes);
        }

        // ✅ Save original
        public async Task SaveAsync(Stream input, string path)
        {
            using var fs = new FileStream(path, FileMode.Create);
            await input.CopyToAsync(fs);
        }
    }
}
