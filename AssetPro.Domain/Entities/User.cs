namespace BloodDonation.Domain.Entities
{
    public class User : Entity
    {
        public bool IsSuperAdmin { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
        public string ImageUrl { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; } = true;
        public string RoleId { get; set; }
    }
}