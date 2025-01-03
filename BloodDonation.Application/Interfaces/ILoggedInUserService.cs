using BloodDonation.Application.ViewModels;

namespace BloodDonation.Application.Interfaces
{
    public interface ILoggedInUserService
    {
        UserAuthVm GetLoggedInUser();
    }
}