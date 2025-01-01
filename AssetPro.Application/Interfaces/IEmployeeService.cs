using AssetPro.Application.ViewModels;
using System.Collections.Generic;

namespace AssetPro.Application.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeVm> GetAll();

        EmployeeVm GetById(string id);

        PayloadResponse Create(EmployeeVm model);

        PayloadResponse Update(string id, EmployeeVm model);
    }
}