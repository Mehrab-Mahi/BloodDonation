using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    [Route("api/campaign")]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;
        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [BloodDonationAuth]
        [HttpPost("create")]
        public IActionResult Create([FromForm] CampaignVm campaignData)
        {
            var response = _campaignService.Create(campaignData);

            return Ok(new {data = response});
        }

        [BloodDonationAuth]
        [HttpPut("update")]
        public IActionResult Update([FromForm] CampaignVm campaignData)
        {
            var response = _campaignService.Update(campaignData);

            return Ok(new {data = response});
        }

        [BloodDonationAuth]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(string id)
        {
            var response = _campaignService.Delete(id);

            return Ok(new {data = response});
        }

        [AllowAnonymous]
        [HttpGet("get/{id}")]
        public IActionResult Get(string id)
        {
            var response = _campaignService.Get(id);

            return Ok(new  {data = response});
        }

        [AllowAnonymous]
        [HttpGet("getall")]
        public IActionResult GetAll(int pageNo = 1, int pageSize = 10)
        {
            var response = _campaignService.GetAll(pageNo, pageSize);
            
            return Ok(response);
        }
        
        [AllowAnonymous]
        [HttpGet("getRunningAndUpcomingCampaign")]
        public IActionResult GetRunningAndUpcomingCampaign(int pageNo = 1, int pageSize = 10)
        {
            var response = _campaignService.GetRunningAndUpcomingCampaign(pageNo, pageSize);
            
            return Ok(response);
        }

        [BloodDonationAuth]
        [HttpGet("getVolunteerPermittedCampaigns")]
        public IActionResult GetVolunteerPermittedCampaigns(int pageNo = 1, int pageSize = 10)
        {
            var response = _campaignService.GetVolunteerPermittedCampaigns(pageNo, pageSize);

            return Ok(response);
        }
    }
}
