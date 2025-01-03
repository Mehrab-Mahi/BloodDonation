﻿using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public IActionResult Create([FromBody] UserCreationVm model)
        {
            var data = _userService.Insert(model);
            return Ok(new { data });
        }

        [BloodDonationAuth]
        [HttpPut("update/{id}")]
        public IActionResult Update(string id, [FromBody] UserVm model)
        {
            var data = _userService.Update(id, model);
            return Ok(new { data });
        }

        [BloodDonationAuth]
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var list = _userService.GetAll();
            return Ok(new { data = list });
        }

        [BloodDonationAuth]
        [HttpGet("getbyid/{id}")]
        public IActionResult GetById(string id)
        {
            var list = _userService.GetById(id);
            return Ok(new { data = list });
        }
        [BloodDonationAuth]
        [HttpGet("getusermenu/{id}")]
        public IActionResult GetUserMenu(string id)
        {
            var data = _authService.GetUserMenu(id);
            return Ok(data);
        }
    }
}