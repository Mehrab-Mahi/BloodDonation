using System;

namespace BloodDonation.Application.ViewModels.DonationTracking;

public class DonationTrackingUpsertRequest
{
    public DateTime? DonationDate { get; set; }
    public string ReceiverName { get; set; }
    public string MobileNumber { get; set; }
}