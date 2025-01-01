using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetPro.Domain.Enums
{
    public enum DbConnection
    {
        AssetProConnection_Local,
        AssetProConnection_Live
    }

    public enum PaymentType
    {
        One_Off,
        Bonus,
        Royality
    }
}
