using System.Collections.Generic;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces
{
    public interface ICampaignService
    {
        PayloadResponse Create(CampaignVm campaignData);
        PayloadResponse Update(string id, CampaignVm campaignData);
        PayloadResponse Delete(string id);
        CampaignVm Get(string id);
        List<Campaign> GetAll(int pageNo, int pageSize);
    }
}