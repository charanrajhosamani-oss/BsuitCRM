
namespace BSuit.Infrastructure.Files
{
    public interface IFileTransferService
    {
        // FTP
        Task UploadFtpAsync(string localPath, string remotePath);
        Task DownloadFtpAsync(string remotePath, string localPath);
        Task DeleteFtpAsync(string remotePath);

        // SFTP
        Task UploadSftpAsync(string localPath, string remotePath);
        Task DownloadSftpAsync(string remotePath, string localPath);
        Task DeleteSftpAsync(string remotePath);
    }
}
