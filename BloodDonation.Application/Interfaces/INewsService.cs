using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces;

public interface INewsService
{
    PayloadResponse Create(NewsVm newsData);
    PayloadResponse Update(NewsVm newsData);
    PayloadResponse Delete(string id);
    News Get(string id);
    object GetAll(int pageNo, int pageSize);
}