using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [AllowAnonymous]
        [HttpPost("registration")]
        public IActionResult Create([FromForm] UserCreationVm model)
        {
            var data = _userService.Insert(model);
            return Ok(new { data });
        }

        [BloodDonationAuth]
        [HttpPut("update")]
        public IActionResult Update([FromForm] UserCreationVm model)
        {
            var data = _userService.Update(model);
            return Ok(data);
        }
        
        [BloodDonationAuth]
        [HttpPost("approvevolunteer")]
        public IActionResult ApproveUser([FromBody] UserApproval userApproval)
        {
            var data = _userService.ApproveUser(userApproval.Id);
            return Ok(new { data });
        }
        
        [BloodDonationAuth]
        [HttpPost("disapprovevolunteer")]
        public IActionResult DisapproveUser([FromBody] UserApproval userApproval)
        {
            var data = _userService.DisapproveUser(userApproval.Id);
            return Ok(new { data });
        }

        [BloodDonationAuth]
        [HttpGet("unapprovedVolunteer")]
        public IActionResult GetUnapprovedUser()
        {
            var data = _userService.GetUnapprovedUser();
            return Ok(new { data });
        }
        
        [BloodDonationAuth]
        [HttpGet("getallapprovedvolunteer")]
        public IActionResult GetAllApprovedVolunteer(int pageNo = 1, int pageSize = 10)
        {
            var data = _userService.GetAllApprovedVolunteer(pageNo, pageSize);
            return Ok(new { data });
        }

        [BloodDonationAuth]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(string id)
        {
            var data = _userService.DeleteUser(id);
            return Ok(new { data });
        }


        [BloodDonationAuth]
        [HttpPost("getall")]
        public IActionResult GetAll([FromBody] UserFilter userFilter)
        {
            var data = _userService.GetAll(userFilter);
            return Ok(data);
        }

        //[BloodDonationAuth]
        //[HttpGet("getbyid/{id}")]
        //public IActionResult GetById(string id)
        //{
        //    var list = _userService.GetById(id);
        //    return Ok(new { data = list });
        //}
        //[BloodDonationAuth]
        //[HttpGet("getusermenu/{id}")]
        //public IActionResult GetUserMenu(string id)
        //{
        //    var data = _authService.GetUserMenu(id);
        //    return Ok(data);
        //}
    }
}