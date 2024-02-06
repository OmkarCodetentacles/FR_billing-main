using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class NewRegisterUser
    {
        public long registrationId { get; set; }
        public string Pfcode { get; set; }
        public string franchiseName { get; set; }
        public string emailId { get; set; }
        public Nullable<System.DateTime> dateTime { get; set; }
        public string ownerName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public Nullable<bool> isPaid { get; set; }
        public string referralCode { get; set; }
        public string mobileNo { get; set; }

        public int DaysSinceRegistration { get; set; }

    }
}