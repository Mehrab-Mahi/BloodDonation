using System.Collections.Generic;
using BloodDonation.Application.ViewModels;

namespace BloodDonation.Application.Interfaces
{
    public interface IBloodBankService
    {
        object GetBloodBankData(BloodBankFilter filter);
        DashboardDataVm GetDashboardData();
    }
}