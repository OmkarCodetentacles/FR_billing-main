using Microsoft.SqlServer.Management.Sdk.Sfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public static class CommonFunctions
    {
        public static string getSessionPfcode()
        {
            string pfCode = "";

            if (HttpContext.Current.Request.Cookies["Cookies"]["AdminValue"] != null)
            {
                pfCode = HttpContext.Current.Request.Cookies["Cookies"]["AdminValue"].ToString();
            }
            return pfCode;
        }

    }
}