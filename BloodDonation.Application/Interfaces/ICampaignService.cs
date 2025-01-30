using System.Collections.Generic;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces
{
    public interface ICampaignService
    {
        PayloadResponse Create(CampaignVm campaignData);
        PayloadResponse Update(CampaignVm campaignData);
        PayloadResponse Delete(string id);
        CampaignVm Get(string id);
        object GetAll(int pageNo, int pageSize);
        object GetRunningAndUpcomingCampaign(int pageNo, int pageSize);
    }
}