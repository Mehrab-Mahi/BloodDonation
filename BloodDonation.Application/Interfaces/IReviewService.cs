using BloodDonation.Application.ViewModels;
using BloodDonation.Application.ViewModels.Review;

namespace BloodDonation.Application.Interfaces;

public interface IReviewService
{
    PayloadResponse Post(PostReviewDto postReview);
    object GetApprovedReviews(int pageNo, int pageSize);
    object GetUnapprovedReviews(int pageNo, int pageSize);
    PayloadResponse Approve(ReviewApprovalRequest approvalRequest);
    PayloadResponse Disapprove(ReviewApprovalRequest approvalRequest);
    PayloadResponse Delete(string id);
}