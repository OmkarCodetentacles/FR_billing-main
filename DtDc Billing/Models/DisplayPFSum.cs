using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class DisplayPFSum
    {
        
        public string PfCode { get; set; }

        public string PfCodetemp { get; set; }

        public double? Sum { get; set; }

        public double? Count { get; set; }

        public double? Sumcash { get; set; }

        public double? Countcash { get; set; }

        public string Branchname { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<DateTime> fromdate { get; set; }
        public Nullable<DateTime> Todate { get; set; }
    }
}