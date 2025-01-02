using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Domain.Entities
{
    public class MenuCrud : Entity
    {
        public string RoleId { get; set; }
        public string AccessControlId { get; set; }
    }
}
