using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class LiveDataModel
    {
        public long TotalNoOfInvoice { get; set; }
        public double TotalInvoiceAmount { get; set; }
        public long TotalUser { get; set; }
        public long TotalConsignmentBooked { get; set; }
    }
}