using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public static class GetLocalTime
    {
        public static DateTime  GetDateTime()
        {

            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

            return localTime;
        }

    }
}