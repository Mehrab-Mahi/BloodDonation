using System.Collections.Generic;
using BloodDonation.Application.ViewModels;

namespace BloodDonation.Application.Interfaces
{
    public interface IBloodBankService
    {
        List<BloodBankDonorDataVm> GetBloodBankData(BloodBankFilter filter);
        DashboardDataVm GetDashboardData();
    }
}