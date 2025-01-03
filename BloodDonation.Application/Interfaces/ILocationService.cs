using System.Collections.Generic;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces
{
    public interface ILocationService
    {
        List<Location> GetLocationByParentId(string parentId);
        PayloadResponse Create(LocationVm locationData);
        PayloadResponse Update(LocationVm locationData);
        PayloadResponse Delete(string id);
        Location Get(string id);
    }
}