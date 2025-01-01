using System.Threading.Tasks;

namespace BloodDonation.Application.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string content, string email);
    }
}