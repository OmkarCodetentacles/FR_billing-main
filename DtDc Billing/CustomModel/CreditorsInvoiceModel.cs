using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class CreditorsInvoiceModel
    {


        public string invoiceno { get; set; }
        public Nullable<System.DateTime> invoicedate { get; set; }
        public Nullable<System.DateTime> periodfrom { get; set; }
        public Nullable<System.DateTime> periodto { get; set; }
        public Nullable<double> total { get; set; }
        public Nullable<double> fullsurchargetax { get; set; }
        public Nullable<double> fullsurchargetaxtotal { get; set; }
        public Nullable<double> servicetax { get; set; }
        public Nullable<double> servicetaxtotal { get; set; }

        public Nullable<double> netamount { get; set; }
        public string Customer_Id { get; set; }
        public string CustomerName { get; set; }
        public Nullable<double> paid { get; set; }
       
        public Nullable<double> discountamount { get; set; }
        

        public Nullable<double> TdsAmount { get; set; }
        public Nullable<double> TotalAmount { get; set; }
    }
}