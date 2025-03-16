using System;

namespace BloodDonation.Application.ViewModels
{
    public class BloodBankDonorDataVm
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string BloodGroup { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public int BloodDonationCount { get; set; }
        public DateTime? LastDonationTime { get; set; }
        public int LastDonationDayCount { get; set; }

    }
}
