using System.ComponentModel.DataAnnotations.Schema;

namespace BloodDonation.Domain.Entities;

public class Review : Entity
{
    public string ReviewMessage { get; set; }
    public bool IsApproved { get; set; }
    public string PostedBy { get; set; }
    [ForeignKey("PostedBy")]
    public User ReviewOwner { get; set; }
}