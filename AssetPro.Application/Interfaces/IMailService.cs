using System.Threading.Tasks;

namespace AssetPro.Application.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string content, string email);
    }
}