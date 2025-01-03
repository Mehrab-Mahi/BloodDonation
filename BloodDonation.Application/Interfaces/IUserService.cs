using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using System.Collections.Generic;

namespace BloodDonation.Application.Interfaces
{
    public interface IUserService
    {
        User Get(AuthRequest model);

        User GetById(string id);

        PayloadResponse Update(string data, UserCreationVm User);
        List<UserVm> GetAll();
        PayloadResponse Insert(UserCreationVm model);
        public bool Delete(string id, string table);
        UserTypeResponse GetUserTypeByPhoneNumberAndDob(AuthRequest model);
        PayloadResponse ApproveUser(string id);
        List<UserCreationVm> GetUnapprovedUser();
        List<UserCreationVm> GetAllApprovedVolunteer(int pageNo, int pageSize);
        PayloadResponse DisapproveUser(string id);
    }
}