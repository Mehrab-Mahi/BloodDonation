using BloodDonation.Application.Helper;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.Web.Controllers;

[Route("api/review")]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [BloodDonationAuth]
    [HttpPost("post")]
    public IActionResult Post([FromBody] PostReviewDto postReview)
    {
        var response = _reviewService.Post(postReview);
        return Ok(new { data = response });
    }
    
    [AllowAnonymous]
    [HttpGet("approvedReviews")]
    public IActionResult GetApprovedReviews(int pageNo = 1, int pageSize = 10)
    {
        var response = _reviewService.GetApprovedReviews(pageNo, pageSize);
        return Ok(new { data = response });
    }
    
    [BloodDonationAuth]
    [HttpGet("unapprovedReviews")]
    public IActionResult GetUnapprovedReviews(int pageNo = 1, int pageSize = 10)
    {
        var response = _reviewService.GetUnapprovedReviews(pageNo, pageSize);
        return Ok(new { data = response });
    }

    [BloodDonationAuth]
    [HttpPost("Approve")]
    public IActionResult Approve([FromBody] ReviewApprovalRequest approvalRequest)
    {
        var response = _reviewService.Approve(approvalRequest);
        return Ok(new { data = response });
    }
    
    [BloodDonationAuth]
    [HttpPost("Disapprove")]
    public IActionResult Disapprove([FromBody] ReviewApprovalRequest approvalRequest)
    {
        var response = _reviewService.Disapprove(approvalRequest);
        return Ok(new { data = response });
    }

    [BloodDonationAuth]
    [HttpDelete("delete/{id}")]
    public IActionResult Delete(string id)
    {
        var response = _reviewService.Delete(id);
        return Ok(new { data = response });
    }
}