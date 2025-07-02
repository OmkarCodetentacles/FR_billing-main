using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class TransationCountModel
    {
        
        public string Customer_Id { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Pf_Code { get; set; }
        public string TempdateTo { get; set; }
        public Nullable<System.DateTime> booking_date { get; set; }
        public string month { get; set; }
     

    }
}