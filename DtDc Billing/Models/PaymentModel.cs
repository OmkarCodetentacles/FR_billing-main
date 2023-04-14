using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class PaymentModel
    {
        public string Customer_Id { get; set; }
        public string invoiceno { get; set; }
        public Nullable<System.DateTime> invoicedate { get; set; }
        public Nullable<double> total { get; set; }
        public Nullable<double> fullsurchargetax { get; set; }
        public Nullable<double> fullsurchargetaxtotal { get; set; }
        public Nullable<double> servicetax { get; set; }
        public Nullable<double> servicetaxtotal { get; set; }
        public Nullable<double> othercharge { get; set; }
        public Nullable<double> netamount { get; set; }    
        public Nullable<double> paid { get; set; }
        public string discount { get; set; }
        public double Balance { get; set; }
        public Nullable<long> Firm_Id { get; set; }
        public string tempInvoicedate { get; set; }
        public Nullable<double> Royalty_charges { get; set; }
        public Nullable<double> Docket_charges { get; set; }

    }
}