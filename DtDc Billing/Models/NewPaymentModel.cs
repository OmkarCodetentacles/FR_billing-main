using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class NewPaymentModel
    {

        public int Payment_Id { get; set; }
        public string Payment_Mode { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<decimal> Tds_Amount { get; set; }
        public Nullable<decimal> Total_Amount { get; set; }
        public string Pfcode { get; set; }
        public System.DateTime Payment_Date { get; set; }
        public string Bank_Name { get; set; }
        public string Branch_Name { get; set; }
        public string Transaction_Id { get; set; }
        public string CheckNo { get; set; }
        public string CreditNoteNo { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }

        public string temppaymentdate { get; set; }


    }
}