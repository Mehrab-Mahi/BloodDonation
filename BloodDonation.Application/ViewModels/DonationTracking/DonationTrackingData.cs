using System;

namespace BloodDonation.Application.ViewModels.DonationTracking;

public class DonationTrackingData
{
    public string DonorId { get; set; }
    public int DonationCount { get; set; }= 0;
    public string FullName { get; set; }
    public string MobileNumber { get; set; }
    public string DateOfBirth { get; set; }
    public string BloodGroup { get; set; }
    public DateTime LastDonationTime { get; set; }
}