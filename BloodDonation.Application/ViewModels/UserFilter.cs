namespace BloodDonation.Application.ViewModels
{
    public class UserFilter
    {
        public string BloodGroup { get; set; }
        public string Upazila { get; set; }
        public string Union { get; set; }
        public string UserType { get; set; }
        public string BloodDonationStatus { get; set; }
        public int StartAge { get; set; } = 18;
        public int EndAge { get; set; } = 62;
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
