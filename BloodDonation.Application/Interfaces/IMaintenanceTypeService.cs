﻿using BloodDonation.Application.ViewModels;
using System.Collections.Generic;

namespace BloodDonation.Application.Interfaces
{
    public interface IMaintenanceTypeService
    {
        IEnumerable<MaintenanceTypeVm> GetAll();
        PayloadResponse Create(MaintenanceTypeVm model);
        PayloadResponse Update(string id, MaintenanceTypeVm model);
        MaintenanceTypeVm GetById(string id);
    }
}