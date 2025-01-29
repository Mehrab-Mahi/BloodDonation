using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces
{
    public interface ILoggedInUserService
    {
        User GetLoggedInUser();
    }
}