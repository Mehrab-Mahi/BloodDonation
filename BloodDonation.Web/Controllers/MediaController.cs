using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    [Route("api/media")]
    public class MediaController : Controller
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [BloodDonationAuth]
        [HttpPut("uploadcampaignmedia")]
        public IActionResult UploadCampaignMedia([FromForm] MediaVm mediaData)
        {
            var response = _mediaService.UploadCampaignMedia(mediaData);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("getcampaignmedia")]
        public IActionResult GetCampaignMedia([FromBody] MediaDataSizeVm mediaDataSize)
        {
            var response = _mediaService.GetCampaignMedia(mediaDataSize);
            return Ok(response);
        }

        [BloodDonationAuth]
        [HttpDelete("deletecampaignmedia")]
        public IActionResult DeleteCampaignMedia([FromBody] MediaDeleteVm mediaDeleteData)
        {
            var response = _mediaService.DeleteCampaignMedia(mediaDeleteData);
            return Ok(response);
        }
    }
}
