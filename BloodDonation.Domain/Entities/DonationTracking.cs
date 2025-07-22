namespace BloodDonation.Domain.Entities;

public class DonationTracking : Entity
{
    public string DonorId { get; set; }
    public string DonationDate { get; set; }
    public string ReceiverName { get; set; }
    public string MobileNumber { get; set; }
}