using BloodDonation.Application.ViewModels;
using System.Collections.Generic;

namespace BloodDonation.Application.Interfaces
{
    public interface IDepartmentService
    {
        IEnumerable<DepartmentVm> GetAll();
        PayloadResponse Create(DepartmentVm model);
        PayloadResponse Update(string id, DepartmentVm model);
        DepartmentVm GetById(string id);
    }
}