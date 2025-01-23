using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    [Route("api/bloodbank")]
    public class BloodBankController : Controller
    {
        private readonly IBloodBankService _bloodBankService;

        public BloodBankController(IBloodBankService bloodBankService)
        {
            _bloodBankService = bloodBankService;
        }

        [AllowAnonymous]
        [HttpPost("getbloodbankdata")]
        public IActionResult GetBloodBankData(BloodBankFilter filter)
        {
            var data = _bloodBankService.GetBloodBankData(filter);
            return Ok(new {data});
        }

        [AllowAnonymous]
        [HttpGet("getdashboarddata")]
        public IActionResult GetDashboardData()
        {
            var data = _bloodBankService.GetDashboardData();
            return Ok(new {data});
        }
    }
}
