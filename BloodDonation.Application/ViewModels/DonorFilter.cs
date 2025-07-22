namespace BloodDonation.Application.ViewModels;

public class DonorFilter
{
    public string BloodGroup { get; set; }
    public string Upazila { get; set; }
    public string Union { get; set; }
    public string BloodDonationStatus { get; set; }
    public int? StartAge { get; set; }
    public int? EndAge { get; set; }
    public int? PageNo { get; set; }
    public int? PageSize { get; set; }
    public string Gender { get; set; }
    public string SearchQuery { get; set; }
}