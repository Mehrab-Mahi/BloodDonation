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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

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
            var response = _userService.Update(model);
            return Ok(new {data = response});
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
        [HttpGet("getUnapprovedVolunteer")]
        public IActionResult GetUnapprovedVolunteer(int pageNo = 1, int pageSize = 10)
        {
            var data = _userService.GetUnapprovedVolunteer(pageNo, pageSize);
            return Ok(data);
        }
        
        [BloodDonationAuth]
        [HttpGet("getApprovedVolunteer")]
        public IActionResult GetAllApprovedVolunteer(int pageNo = 1, int pageSize = 10)
        {
            var data = _userService.GetApprovedVolunteer(pageNo, pageSize);
            return Ok(data);
        }

        [BloodDonationAuth]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(string id)
        {
            var response = _userService.DeleteUser(id);
            return Ok(new {data = response});
        }


        [BloodDonationAuth]
        [HttpPost("getall")]
        public IActionResult GetAll([FromBody] UserFilter userFilter)
        {
            var data = _userService.GetAll(userFilter);
            return Ok(data);
        }
        
        [BloodDonationAuth]
        [HttpPost("getApprovedDonor")]
        public IActionResult GetApprovedDonor([FromBody] DonorFilter donorFilter)
        {
            var data = _userService.GetApprovedDonor(donorFilter);
            return Ok(data);
        }
        
        [BloodDonationAuth]
        [HttpPost("getUnapprovedDonor")]
        public IActionResult GetUnapprovedDonor([FromBody] DonorFilter donorFilter)
        {
            var data = _userService.GetUnapprovedDonor(donorFilter);
            return Ok(data);
        }
        
        [BloodDonationAuth]
        [HttpGet("getAllAdmin")]
        public IActionResult GetAllAdmin(int pageNo = 1, int pageSize = 10)
        {
            var data = _userService.GetAllAdmin(pageNo, pageSize);
            return Ok(data);
        }
        
        [BloodDonationAuth]
        [HttpGet("getPermittedDonors")]
        public IActionResult GetPermittedDonors(int pageNo = 1, int pageSize = 10)
        {
            var data = _userService.GetPermittedDonors(pageNo, pageSize);
            return Ok(data);
        }
        
        [AllowAnonymous]
        [HttpGet("getOfficialLeaders")]
        public IActionResult GetOfficialLeaders()
        {
            var data = _userService.GetOfficialLeaders();
            return Ok(data);
        }
        
        [AllowAnonymous]
        [HttpGet("getScoutLeaders")]
        public IActionResult GetScoutLeaders(int pageNo = 1, int pageSize = 10)
        {
            var data = _userService.GetScoutLeaders(pageNo, pageSize);
            return Ok(data);
        }

        [BloodDonationAuth]
        [HttpPost("donorRegistration")]
        public IActionResult DonorRegistration([FromForm] UserCreationVm model)
        {
            var data = _userService.Insert(model);
            return Ok(new { data });
        }

        [BloodDonationAuth]
        [HttpGet("getbyid/{id}")]
        public IActionResult GetById(string id)
        {
            var userData = _userService.GetById(id);
            return Ok(new { data = userData });
        }
    }
}