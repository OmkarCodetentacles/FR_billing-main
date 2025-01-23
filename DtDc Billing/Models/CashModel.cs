using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class CashModel
    {

        public int Cash_id { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]

        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> inserteddate { get; set; }
        public string custId { get; set; }
        public string Invoiceno { get; set; }
        public string invoiceList { get; set; }
        public string SelectedInvoices { get; set; }
        public string invoiceType { get; set; }
        public Nullable<double> C_Tds_Amount { get; set; }
        public Nullable<double> C_Total_Amount { get; set; }
        public Nullable<long> Firm_Id { get; set; }
        public string Pfcode { get; set; }
        [Required(ErrorMessage = "Date must be Required")]

        public Nullable<System.DateTime> tempinserteddate { get; set; }
        public Nullable<System.DateTime> tempch_date { get; set; }
        public Nullable<double> Balance { get; set; }
        public Nullable<double> creditTotalAmt { get; set; }
        public Nullable<double> selectedTotal { get; set; }

        public string FormattedTempInsertedDate
        {
            get
            {
                return tempinserteddate?.ToString("dd-MM-yyyy");
            }
        }

        public virtual FirmDetail FirmDetail { get; set; }
        public virtual FirmDetail FirmDetail1 { get; set; }

    }


    public class MulInvoice
    {
        public string InvoiceNo { get; set; }
        public double Amount { get; set; }
    }
}