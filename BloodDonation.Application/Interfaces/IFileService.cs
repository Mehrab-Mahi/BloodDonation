using Microsoft.AspNetCore.Http;

namespace BloodDonation.Application.Interfaces
{
    public interface IFileService
    {
        string GetRootPath();
        void CreateDirectoryIfNotExists(string path);
        void SaveFile(string filePath, IFormFile campaignBanner);
        void DeleteFile(string filePath);
    }
}