using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using System.Collections.Generic;

namespace BloodDonation.Application.Interfaces
{
    public interface IUserService
    {
        User Get(AuthRequest model);

        User GetById(string id);

        PayloadResponse Update(UserCreationVm User);
        object GetAll(UserFilter userFilter);
        PayloadResponse Insert(UserCreationVm model);
        public bool Delete(string id, string table);
        UserTypeResponse GetUserTypeByPhoneNumberAndDob(AuthRequest model);
        PayloadResponse ApproveUser(string id);
        object GetUnapprovedVolunteer(int pageNo, int pageSize);
        object GetApprovedVolunteer(int pageNo, int pageSize);
        PayloadResponse DisapproveUser(string id);
        PayloadResponse DeleteUser(string id);
        object GetApprovedDonor(DonorFilter donorFilter);
        object GetUnapprovedDonor(DonorFilter donorFilter);
        object GetAllAdmin(int pageNo, int pageSize);
        object GetPermittedDonors(int pageNo, int pageSize);
    }
}