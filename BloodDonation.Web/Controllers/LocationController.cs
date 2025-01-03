using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    [Route("api/location")]
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [AllowAnonymous]
        [HttpGet("GetByParentId/{parentId}")]
        public IActionResult GetLocationByParentId(string parentId)
        {
            var locations = _locationService.GetLocationByParentId(parentId);

            return Ok(locations);
        }

        [BloodDonationAuth]
        [HttpPost("create")]
        public IActionResult Create([FromBody] LocationVm locationData)
        {
            var locations = _locationService.Create(locationData);

            return Ok(locations);
        }
        
        [BloodDonationAuth]
        [HttpPut("update")]
        public IActionResult Update([FromBody] LocationVm locationData)
        {
            var locations = _locationService.Update(locationData);

            return Ok(locations);
        }

        [BloodDonationAuth]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(string id)
        {
            var locations = _locationService.Delete(id);

            return Ok(locations);
        }
        
        [BloodDonationAuth]
        [HttpGet("get/{id}")]
        public IActionResult Get(string id)
        {
            var locations = _locationService.Get(id);

            return Ok(locations);
        }
    }
}
