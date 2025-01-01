using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssetPro.Application.Helper
{
    public class AssetProAuthAttribute : TypeFilterAttribute
    {
        public AssetProAuthAttribute() : base(typeof(AssetProAuthorizeFilter))
        {
            Arguments = new object[] { };
        }
    }
}
