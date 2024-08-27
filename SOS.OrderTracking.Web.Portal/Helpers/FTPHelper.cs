using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Portal.Helpers
{
    public interface IFTPUploadService
    {
        Task<string> UploadFileAsync(byte[] fileContents, string remoteFileName);
    }

    public class FTPUploadService : IFTPUploadService
    {
        private readonly string _ftpServer;
        private readonly string _username;
        private readonly string _password;

        public FTPUploadService(string ftpServer, string username, string password)
        {
            _ftpServer = ftpServer;
            _username = username;
            _password = password;
        }

        public async Task<string> UploadFileAsync(byte[] fileContents, string fileName)
        {
            // Create the request to upload the file
            var request = (FtpWebRequest)WebRequest.Create(new Uri($"{_ftpServer}/upload/Consignment/Receipt/{fileName}"));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_username, _password);
            request.ContentLength = fileContents.Length;

            // Write the file contents to the request stream
            using (var requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(fileContents, 0, fileContents.Length);
            }

            // Get the response from the server
            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != FtpStatusCode.ClosingData)
                {
                    throw new Exception($"Error uploading file: {response.StatusDescription}");
                }
            }
            return Path.Combine("upload", "Consignment", "Receipt", fileName);
            
        }
    }

}