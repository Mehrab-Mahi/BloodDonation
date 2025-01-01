using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodDonation.Domain.Enums
{
    public enum DbConnection
    {
        BloodDonationConnection_Local,
        BloodDonationConnection_Live
    }

    public enum PaymentType
    {
        One_Off,
        Bonus,
        Royality
    }
}
