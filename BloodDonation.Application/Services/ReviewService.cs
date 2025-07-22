using System;
using System.Linq;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Application.ViewModels.Review;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly ILoggedInUserService _loggedInUserService;

    public ReviewService(IRepository<Review> reviewRepository,
        ILoggedInUserService loggedInUserService)
    {
        _reviewRepository = reviewRepository;
        _loggedInUserService = loggedInUserService;
    }

    public PayloadResponse Post(PostReviewDto postReview)
    {
        try
        {
            var currentUser = _loggedInUserService.GetLoggedInUser();

            if (currentUser == null)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            if (postReview == null || string.IsNullOrEmpty(postReview.ReviewMessage))
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    Message = "Review message cannot be empty."
                };
            }

            var review = new Review
            {
                PostedBy = currentUser.Id,
                ReviewMessage = postReview.ReviewMessage,
                IsApproved = false
            };

            _reviewRepository.Insert(review);
            _reviewRepository.SaveChanges();

            return new PayloadResponse
            {
                IsSuccess = true,
                Message = "Review posted successfully."
            };
        }
        catch(Exception ex)
        {
            return new PayloadResponse
            {
                IsSuccess = false,
                Message = $"An error occurred while posting the review: {ex.Message}"
            };
        }
    }

    public object GetApprovedReviews(int pageNo, int pageSize)
    {
        var allApprovedReviews = _reviewRepository.GetAll()
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreateTime);

        var approvedReviews = allApprovedReviews
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.ReviewOwner)
            .ToList();

        return new
        {
            data = approvedReviews,
            rowCount = allApprovedReviews.Count()
        };
    }

    public object GetUnapprovedReviews(int pageNo, int pageSize)
    {
        var allUnapprovedReviews = _reviewRepository.GetAll()
            .Where(r => !r.IsApproved)
            .OrderByDescending(r => r.CreateTime);

        var unapprovedReviews = allUnapprovedReviews
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.ReviewOwner)
            .ToList();

        return new
        {
            data = unapprovedReviews,
            rowCount = allUnapprovedReviews.Count()
        };
    }

    public PayloadResponse Approve(ReviewApprovalRequest approvalRequest)
    {
        try
        {
            var review = _reviewRepository.GetConditional(r => r.Id == approvalRequest.ReviewId);

            if (review == null)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    Message = "Review not found."
                };
            }

            review.IsApproved = true;

            _reviewRepository.Update(review);
            _reviewRepository.SaveChanges();

            return new PayloadResponse
            {
                IsSuccess = true,
                Message = "Review approved successfully."
            };
        }
        catch (Exception ex)
        {
            return new PayloadResponse
            {
                IsSuccess = false,
                Message = $"An error occurred while approving the review: {ex.Message}"
            };
        }
    }

    public PayloadResponse Disapprove(ReviewApprovalRequest approvalRequest)
    {
        try
        {
            var review = _reviewRepository.GetConditional(r => r.Id == approvalRequest.ReviewId);

            if (review == null)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    Message = "Review not found."
                };
            }

            review.IsApproved = false;

            _reviewRepository.Update(review);
            _reviewRepository.SaveChanges();

            return new PayloadResponse
            {
                IsSuccess = true,
                Message = "Review disapproved successfully."
            };
        }
        catch (Exception ex)
        {
            return new PayloadResponse
            {
                IsSuccess = false,
                Message = $"An error occurred while disapproving the review: {ex.Message}"
            };
        }
    }

    public PayloadResponse Delete(string id)
    {
        try
        {
            var review = _reviewRepository.GetConditional(r => r.Id == id);

            if (review == null)
            {
                return new PayloadResponse
                {
                    IsSuccess = false,
                    Message = "Review not found."
                };
            }

            _reviewRepository.Delete(review);
            _reviewRepository.SaveChanges();

            return new PayloadResponse
            {
                IsSuccess = true,
                Message = "Review deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new PayloadResponse
            {
                IsSuccess = false,
                Message = $"An error occurred while deleting the review: {ex.Message}"
            };
        }
    }
}