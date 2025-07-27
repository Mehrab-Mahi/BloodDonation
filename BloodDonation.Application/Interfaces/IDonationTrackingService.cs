using System;
using System.Collections.Generic;
using BloodDonation.Application.ViewModels;
using BloodDonation.Application.ViewModels.DonationTracking;
using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces;

public interface IDonationTrackingService
{
    PayloadResponse Upsert(DonationTrackingUpsertRequest donationTrackingData);
    object GetHighestDonorList(DateTime startTime, DateTime endTime, int pageNo = 1, int pageSize = 10);
    List<DonationTracking> GetDonationDetail(string donorId, int pageNo, int pageSize);
}