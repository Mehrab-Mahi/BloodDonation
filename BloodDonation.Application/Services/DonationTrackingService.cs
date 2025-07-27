using System;
using System.Collections.Generic;
using System.Linq;
using BloodDonation.Application.Interfaces;
using BloodDonation.Application.ViewModels;
using BloodDonation.Application.ViewModels.DonationTracking;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Interfaces;

namespace BloodDonation.Application.Services;

public class DonationTrackingService : IDonationTrackingService
{
    private readonly IRepository<DonationTracking>  _donationTrackingRepository;
    private readonly ILoggedInUserService _loggedInUserService;
    private readonly IBaseRepository _baseRepository;

    public DonationTrackingService(IRepository<DonationTracking> donationTrackingRepository,
        ILoggedInUserService loggedInUserService,
        IBaseRepository baseRepository)
    {
        _donationTrackingRepository = donationTrackingRepository;
        _loggedInUserService = loggedInUserService;
        _baseRepository = baseRepository;
    }

    public PayloadResponse Upsert(DonationTrackingUpsertRequest donationTrackingData)
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

            if (donationTrackingData == null)
            {
                _donationTrackingRepository.Insert(new DonationTracking
                {
                    DonorId = currentUser.Id,
                    DonationDate = DateTime.Now,
                });
                _donationTrackingRepository.SaveChanges();
            }
            else
            {
                _donationTrackingRepository.Insert(new DonationTracking
                {
                    DonorId = currentUser.Id,
                    DonationDate = donationTrackingData.DonationDate ?? DateTime.Now,
                    ReceiverName = donationTrackingData.ReceiverName,
                    MobileNumber = donationTrackingData.MobileNumber
                });
                _donationTrackingRepository.SaveChanges();
            }

            return new PayloadResponse
            {
                IsSuccess = true,
                Message = "Donation tracking data saved successfully."
            };
        }
        catch (Exception ex)
        {
            return new PayloadResponse
            {
                IsSuccess = false,
                Message = $"An error occurred while saving donation tracking data: {ex.Message}"
            };
        }
    }

    public object GetHighestDonorList(DateTime startTime, DateTime endTime, int pageNo = 1, int pageSize = 10)
    {
        var totalCount = GetTotalCountOfDonation(startTime, endTime);
        var donorData = GetDonorsData(startTime, endTime, pageNo, pageSize);

        return new
        {
            totalCount,
            data = donorData
        };
    }

    public List<DonationTracking> GetDonationDetail(string donorId, int pageNo, int pageSize)
    {
        var donationDetail = _donationTrackingRepository.GetAll()
            .Where(dt => dt.DonorId == donorId)
            .OrderByDescending(dt => dt.CreateTime)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return donationDetail;
    }

    private List<DonationTrackingData> GetDonorsData(DateTime startTime, DateTime endTime, int pageNo, int pageSize)
    {
        var query = $@"SELECT 
                        u.FullName,
                        u.MobileNumber,
                        u.DateOfBirth,
                        u.BloodGroup,
                        u.LastDonationTime,
                        dt.DonorId,
                        COUNT(dt.Id) AS DonationCount
                    FROM 
                        DonationTrackings dt
                    INNER JOIN 
                        Users u ON dt.DonorId = u.Id
                    where dt.DonationDate >= '{startTime}' and dt.DonationDate <='{endTime}'
                    GROUP BY 
                        u.FullName, 
                        u.MobileNumber, 
                        u.DateOfBirth, 
                        u.BloodGroup, 
                        u.LastDonationTime,
                        dt.Id
                    ORDER BY 
                        DonationCount DESC,
                        MAX(dt.CreateTime) DESC
                    OFFSET 
                        ({pageNo} - 1) * {pageSize} ROWS
                    FETCH NEXT 
                        {pageSize} ROWS ONLY;
                    ";

        var data = _baseRepository.Query<DonationTrackingData>(query).ToList();

        return data;
    }

    private int GetTotalCountOfDonation(DateTime startTime, DateTime endTime)
    {
        var query = @$"
                    with data as (
                    SELECT 
                        u.FullName,
                        u.MobileNumber,
                        u.DateOfBirth,
                        u.BloodGroup,
                        u.LastDonationTime,
                        dt.Id as DonorId,
                        COUNT(dt.Id) AS DonationCount
                    FROM 
                        DonationTrackings dt
                    INNER JOIN 
                        Users u ON dt.DonorId = u.Id
                    where dt.DonationDate >= '{startTime}' and dt.DonationDate <='{endTime}'
                    GROUP BY 
                        u.FullName, 
                        u.MobileNumber, 
                        u.DateOfBirth, 
                        u.BloodGroup, 
                        u.LastDonationTime,
                        dt.Id)
                    select count(*) as count
                    from data
                    ";

        var data = _baseRepository.QuerySingleOrDefault<int>(query);

        return data;
    }
}