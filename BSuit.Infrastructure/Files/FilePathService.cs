using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BSuit.Infrastructure.Files
{
    public class FilePathService : IFilePathService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;

        public FilePathService(
            IWebHostEnvironment env,
            IHttpContextAccessor http)
        {
            _env = env;
            _http = http;
        }

        // ✅ Virtual → Physical
        public string ToPhysicalPath(string virtualPath)
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
                return null;

            virtualPath = virtualPath.TrimStart('~').TrimStart('/');

            return Path.Combine(_env.WebRootPath, virtualPath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        }

        // ✅ Physical → Virtual
        public string ToVirtualPath(string physicalPath)
        {
            if (string.IsNullOrWhiteSpace(physicalPath))
                return null;

            var webRoot = _env.WebRootPath;

            if (!physicalPath.StartsWith(webRoot))
                return physicalPath;

            var relative = physicalPath.Substring(webRoot.Length)
                .Replace("\\", "/");

            return "/" + relative.TrimStart('/');
        }

        // ✅ Virtual → Full URL
        public string ToAbsoluteUrl(string virtualPath)
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
                return null;

            var request = _http.HttpContext?.Request;

            if (request == null)
                return virtualPath;

            var baseUrl = $"{request.Scheme}://{request.Host}";

            return baseUrl + virtualPath;
        }

        // ✅ Physical → Full URL
        public string PhysicalToAbsoluteUrl(string physicalPath)
        {
            var virtualPath = ToVirtualPath(physicalPath);
            return ToAbsoluteUrl(virtualPath);
        }

        // ✅ Default Image Fallback
        public string GetImageUrl(string virtualPath, string defaultImage = "/images/no-image.png")
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
                return ToAbsoluteUrl(defaultImage);

            var physical = ToPhysicalPath(virtualPath);

            if (!File.Exists(physical))
                return ToAbsoluteUrl(defaultImage);

            return ToAbsoluteUrl(virtualPath);
        }

        // ✅ Check file exists
        public bool FileExists(string virtualPath)
        {
            var physical = ToPhysicalPath(virtualPath);
            return File.Exists(physical);
        }
    }
}
