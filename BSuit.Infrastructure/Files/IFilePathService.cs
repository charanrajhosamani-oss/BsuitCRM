
namespace BSuit.Infrastructure.Files
{
    public interface IFilePathService
    {
        string ToPhysicalPath(string virtualPath);
        string ToVirtualPath(string physicalPath);

        string ToAbsoluteUrl(string virtualPath);
        string PhysicalToAbsoluteUrl(string physicalPath);

        string GetImageUrl(string virtualPath, string defaultImage = "/images/no-image.png");

        bool FileExists(string virtualPath);
    }
}
