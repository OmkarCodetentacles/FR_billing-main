using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class PaymentTrack
    {
        public int id { get; set; }

        public Nullable<double> Amount { get; set; }

        public Nullable<System.DateTime> tempinserteddate { get; set; }
       
        public string Invoiceno { get; set; }

        public string Modeofpayment { get; set; }

        public Nullable<System.DateTime> invoicedate { get; set; }

        public string Customer_Id { get; set; }

        public Nullable<double> netamount { get; set; }

        public string remark { get; set; }

        public Nullable<long> Firmid { get; set; }
        public double CalculatedAmount
        {
            get { return Math.Round((netamount ?? 0) - (Amount ?? 0),2); }
        }
    }
}