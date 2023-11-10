using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class RemainingModel
    {
        public int S_id { get; set; }
        public Nullable<System.DateTime> recieptdate { get; set; }
        public string startno { get; set; }
        public string endno { get; set; }
        public Nullable<int> noofleafs { get; set; }
        public Nullable<int> noofbooks { get; set; }
        public Nullable<System.DateTime> Expiry_Date { get; set; }
        public Nullable<int> fid { get; set; }
        public string Invoiceno { get; set; }
        public string temprecdate { get; set; }
        public string tempExpiry_Date { get; set; }
        public string Pf_code { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<long> Firm_Id { get; set; }
        public int totalCount { get; set; }
       // public Nullable<long> Pf_code { get; set; }



    }
}