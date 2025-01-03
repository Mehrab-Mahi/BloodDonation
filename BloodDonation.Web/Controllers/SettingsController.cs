﻿using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    [BloodDonationAuth]
    public class SettingsController : Controller
    {
        private readonly ICommonService _commonService;
        private readonly IAuthService _authService;

        public SettingsController(ICommonService commonService, IAuthService authService)
        {
            _commonService = commonService;
            _authService = authService;
        }
       

        [HttpPost]
        public IActionResult DeleteEntity(DeleteEntityViewModel model)
        {
            if (model.Table == "User")
            {
                var currentUser = _authService.GetCurrentUser();
                if (currentUser.Id == model.Id)
                {
                    return Json(new { status = false, message = "You can't delete Yourself." });
                }
                else
                {
                    var status = _commonService.Delete(model.Id, model.Table);
                    return Json(new { status = status, message = "Entity has been deleted successfully" });
                }
            }
            else
            {
                var status = _commonService.Delete(model.Id, model.Table);
                return Json(new { status = status, message = "Entity has been deleted successfully" });
            }
        }
    }
}