using AssetPro.Application.ViewModels;
using System.Collections.Generic;

namespace AssetPro.Application.Interfaces
{
    public interface IDesignationService
    {
        IEnumerable<DesignationVm> GetAll();
        DesignationVm GetById(string id);
        PayloadResponse Create(DesignationVm model);
        PayloadResponse Update(string id, DesignationVm model);
    }
}