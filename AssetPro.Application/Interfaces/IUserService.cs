﻿using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using System.Collections.Generic;

namespace BloodDonation.Application.Interfaces
{
    public interface IUserService
    {
        User Get(string email);

        User GetById(string id);

        PayloadResponse Update(string data, UserVm User);

        List<UserVm> GetAll();

        PayloadResponse Insert(UserCreationVm model);

        public bool Delete(string id, string table);
    }
}