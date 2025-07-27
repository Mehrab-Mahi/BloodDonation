using System;
using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels.DonationTracking;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers;

[Route("api/donationTracking")]
public class DonationTrackingController : Controller
{
    private readonly IDonationTrackingService _donationTrackingService;

    public DonationTrackingController(IDonationTrackingService donationTrackingService)
    {
        _donationTrackingService = donationTrackingService;
    }

    [BloodDonationAuth]
    [HttpPost("Upsert")]
    public IActionResult Upsert([FromBody] DonationTrackingUpsertRequest donationTrackingData)
    {
        var response = _donationTrackingService.Upsert(donationTrackingData);
        return Ok(new { response });
    }
    
    [BloodDonationAuth]
    [HttpGet("GetHighestDonorList")]
    public IActionResult GetHighestDonorList(DateTime startTime, DateTime endTime, int pageNo = 1, int pageSize = 10) 
    {
        var data = _donationTrackingService.GetHighestDonorList(startTime, endTime, pageNo, pageSize);
        return Ok(new { data });
    }
    
    [BloodDonationAuth]
    [HttpGet("GetDonationDetail")]
    public IActionResult GetDonationDetail(string donorId, int pageNo = 1, int pageSize = 10) 
    {
        var data = _donationTrackingService.GetDonationDetail(donorId, pageNo, pageSize);
        return Ok(new { data });
    }
}