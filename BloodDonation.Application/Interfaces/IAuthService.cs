﻿using BloodDonation.Application.ViewModels;
using System.Collections.Generic;

namespace BloodDonation.Application.Interfaces
{
    public interface IAuthService
    {
        PayloadResponse Authenticate(AuthRequest model);

        UserAuthVm ValidateToken(string authToken);

        UserAuthVm GetCurrentUser();

        List<AccessControlVm> GetUserMenu(string id);
        UserTypeResponse UserType(AuthRequest model);
    }
}