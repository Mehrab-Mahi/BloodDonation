using BloodDonation.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace BloodDonation.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetRootPath()
        {
            return _webHostEnvironment.ContentRootPath;
        }

        public void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void SaveFile(string filePath, IFormFile campaignBanner)
        {
            if (campaignBanner.Length <= 0) return;
            using MemoryStream ms = new();
            campaignBanner.CopyTo(ms);
            var fileBytes = ms.ToArray();
            File.WriteAllBytes(filePath, fileBytes);
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
