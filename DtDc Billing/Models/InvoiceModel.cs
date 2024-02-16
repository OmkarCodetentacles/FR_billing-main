using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class InvoiceModel
    {
        public int IN_Id { get; set; }
        public string invoiceno { get; set; }
        
        public Nullable<System.DateTime> invoicedate { get; set; }
      
        public Nullable<System.DateTime> periodfrom { get; set; }

        public Nullable<System.DateTime> periodto { get; set; }
        public Nullable<double> total { get; set; }
        public Nullable<double> fullsurchargetax { get; set; }
        public Nullable<double> fullsurchargetaxtotal { get; set; }
        public Nullable<double> servicetax { get; set; }
        public Nullable<double> servicetaxtotal { get; set; }
        public Nullable<double> othercharge { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Net amount must be greater than 0.")]

        public Nullable<double> netamount { get; set; }
        [Required]
        public string Customer_Id { get; set; }
        public Nullable<int> fid { get; set; }
        public string annyear { get; set; }
        public Nullable<double> paid { get; set; }
        public string status { get; set; }
        public string discount { get; set; }
        public Nullable<double> discountper { get; set; }
        public Nullable<double> discountamount { get; set; }
        public Nullable<double> servicecharges { get; set; }
        public Nullable<double> Royalty_charges { get; set; }
        public Nullable<double> Docket_charges { get; set; }
        [Required(ErrorMessage = "Date Required")]
        public string Tempdatefrom { get; set; }
        [Required(ErrorMessage ="Date Required")]
        public string TempdateTo { get; set; }
        [Required(ErrorMessage = "Date Required")]
        public string tempInvoicedate { get; set; }
        public string Address { get; set; }
        public string Invoice_Lable { get; set; }
        public string Total_Lable { get; set; }
        public string Royalti_Lable { get; set; }
        public string Docket_Lable { get; set; }
        public Nullable<double> Amount4 { get; set; }
        public string Amount4_Lable { get; set; }
        public Nullable<long> Firm_Id { get; set; }
        public string Pfcode { get; set; }
        public int totalCount { get; set; }


    }
}