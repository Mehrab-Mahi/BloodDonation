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

            return Ok(response);
        }

        [BloodDonationAuth]
        [HttpPut("update/{id}")]
        public IActionResult Update(string id, [FromForm] CampaignVm campaignData)
        {
            var response = _campaignService.Update(id, campaignData);

            return Ok(response);
        }

        [BloodDonationAuth]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(string id)
        {
            var response = _campaignService.Delete(id);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("get/{id}")]
        public IActionResult Get(string id)
        {
            var response = _campaignService.Get(id);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("getall")]
        public IActionResult GetAll(int pageNo, int pageSize)
        {
            var response = _campaignService.GetAll(pageNo, pageSize);

            return Ok(response);
        }
    }
}
