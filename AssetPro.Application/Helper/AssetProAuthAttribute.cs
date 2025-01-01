using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BloodDonation.Application.Helper
{
    public class BloodDonationAuthAttribute : TypeFilterAttribute
    {
        public BloodDonationAuthAttribute() : base(typeof(BloodDonationAuthorizeFilter))
        {
            Arguments = new object[] { };
        }
    }
}
