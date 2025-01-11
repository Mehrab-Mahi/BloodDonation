using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers
{
    [Route("api/contact")]
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public IActionResult Create([FromBody] Contact contactData)
        {
            var response = _contactService.Create(contactData);
            return Ok(response);
        }
        
        [BloodDonationAuth]
        [HttpGet("getall")]
        public IActionResult GetAll(string contactType, int pageNo, int pageSize)
        {
            var response = _contactService.GetAll(contactType, pageNo, pageSize);
            return Ok(response);
        }
        
        [BloodDonationAuth]
        [HttpPost("read")]
        public IActionResult Read([FromBody] NoticeReadVm noticeReadData)
        {
            var response = _contactService.ReadContact(noticeReadData.Id);
            return Ok(response);
        }

        [BloodDonationAuth]
        [HttpGet("get")]
        public IActionResult Get(string id)
        {
            var response = _contactService.Get(id);
            return Ok(response);
        }
    }
}
