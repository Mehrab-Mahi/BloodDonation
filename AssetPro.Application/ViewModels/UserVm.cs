using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetPro.Application.ViewModels
{
    public class UserVm
    {
        public string Id { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string ImageUrl { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
  
    public class UserAuthVm : UserVm
    {
        public bool IsAuthenticate { get; set; }
        public string Name { get; set; }
    }

    public class UserCreationVm : UserVm
    {
        [Required]

        [Display(Name = "Password")]
        public string Password { get; set; }
        public bool IsApproved { get; set; }
    }
}
