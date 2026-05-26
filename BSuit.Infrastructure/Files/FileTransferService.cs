using System.Net;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;

namespace BSuit.Infrastructure.Files
{
   

    public class FileTransferService : IFileTransferService
    {
        private readonly IConfiguration _config;

        public FileTransferService(IConfiguration config)
        {
            _config = config;
        }

        #region FTP

        public async Task UploadFtpAsync(string localPath, string remotePath)
        {
            var request = CreateFtpRequest(remotePath, WebRequestMethods.Ftp.UploadFile);

            var fileBytes = await File.ReadAllBytesAsync(localPath);

            using var stream = await request.GetRequestStreamAsync();
            await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
        }

        public async Task DownloadFtpAsync(string remotePath, string localPath)
        {
            var request = CreateFtpRequest(remotePath, WebRequestMethods.Ftp.DownloadFile);

            using var response = (FtpWebResponse)await request.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using var fs = new FileStream(localPath, FileMode.Create);

            await stream.CopyToAsync(fs);
        }

        public async Task DeleteFtpAsync(string remotePath)
        {
            var request = CreateFtpRequest(remotePath, WebRequestMethods.Ftp.DeleteFile);

            using var response = (FtpWebResponse)await request.GetResponseAsync();
        }

        private FtpWebRequest CreateFtpRequest(string path, string method)
        {
            var host = _config["FtpSettings:Host"];
            var user = _config["FtpSettings:Username"];
            var pass = _config["FtpSettings:Password"];

            var uri = new Uri($"ftp://{host}/{path}");

            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.Credentials = new NetworkCredential(user, pass);
            request.UseBinary = true;
            request.KeepAlive = false;

            return request;
        }

        #endregion

        #region SFTP

        public Task UploadSftpAsync(string localPath, string remotePath)
        {
            using var client = CreateSftpClient();

            client.Connect();

            using var fs = File.OpenRead(localPath);
            client.UploadFile(fs, remotePath, true);

            client.Disconnect();

            return Task.CompletedTask;
        }

        public Task DownloadSftpAsync(string remotePath, string localPath)
        {
            using var client = CreateSftpClient();

            client.Connect();

            using var fs = File.Create(localPath);
            client.DownloadFile(remotePath, fs);

            client.Disconnect();

            return Task.CompletedTask;
        }

        public Task DeleteSftpAsync(string remotePath)
        {
            using var client = CreateSftpClient();

            client.Connect();
            client.DeleteFile(remotePath);
            client.Disconnect();

            return Task.CompletedTask;
        }

        private SftpClient CreateSftpClient()
        {
            return new SftpClient(
                _config["SftpSettings:Host"],
                int.Parse(_config["SftpSettings:Port"]),
                _config["SftpSettings:Username"],
                _config["SftpSettings:Password"]);
        }

        #endregion
    }
}
