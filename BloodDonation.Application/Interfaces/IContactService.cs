using System.Collections.Generic;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces
{
    public interface IContactService
    {
        PayloadResponse Create(Contact contactData);
        List<ContactVm> GetAll(string contactType, int pageNo, int pageSize);
        PayloadResponse ReadContact(string id);
        ContactVm Get(string id);
    }
}