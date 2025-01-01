using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Application.ViewModels
{
    public class AuthRequest
    {
        [Required]
        [Display(Name = "UserName")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public AuthResponse(string token)
        {
            Token = token;
        }
    }

    public class AccessControlVm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ParentId { get; set; }
        public string Url { get; set; }
        public string MenuId { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }
        public List<AccessControlVm> Child { get; set; }
        public AccessControlVm()
        {
            Child = new List<AccessControlVm>();
        }
    }
}
