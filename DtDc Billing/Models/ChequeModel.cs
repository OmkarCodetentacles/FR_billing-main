using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class ChequeModel
    {

        
            public int Cheque_id { get; set; }
            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]


            public Nullable<double> C_Amount { get; set; }
            public Nullable<System.DateTime> ch_date { get; set; }
            public string bank_name { get; set; }
            public string branch_Name { get; set; }
            public Nullable<double> totalAmount { get; set; }
            public string Invoiceno { get; set; }
            public Nullable<double> Tds_amount { get; set; }
            public Nullable<long> Firm_Id { get; set; }
            public string Pfcode { get; set; }
            public Nullable<System.DateTime> tempch_date { get; set; }
            public string FormattedTempChInsertedDate
            {
                get
                {
                    return tempch_date?.ToString("dd-MM-yyyy");
                }
            }


            public virtual FirmDetail FirmDetail { get; set; }
            public virtual FirmDetail FirmDetail1 { get; set; }
        
    }
}