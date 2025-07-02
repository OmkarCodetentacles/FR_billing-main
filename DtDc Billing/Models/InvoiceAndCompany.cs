using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class InvoiceAndCompany
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
        public Nullable<double> netamount { get; set; }
        
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
        
        public string Tempdatefrom { get; set; }
        
        public string TempdateTo { get; set; }
        
        public string tempInvoicedate { get; set; }
        
        public string Address { get; set; }
        public string Invoice_Lable { get; set; }
        public string Total_Lable { get; set; }
        public string Royalti_Lable { get; set; }
        public string Docket_Lable { get; set; }

        [Display(Name = "Company Name")]
        public string Company_Name { get; set; }

        [Display(Name = "Gst No")]
        public string Gst_No { get; set; }

        public string Fr_Gst_No { get; set; }


        public double CgstPer { get; set; }
        public double SgstPer { get; set; }
        public double IgstPer { get; set; }

        public double ? CgstAmt { get; set; }
        public double ? SgstAmt { get; set; }
        public double ? IgstAmt { get; set; }





    }
}