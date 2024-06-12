using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class CreditNoteModel
    {

       
            public int Cr_id { get; set; }
            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]


            public Nullable<double> Cr_Amount { get; set; }
            public Nullable<System.DateTime> cr_date { get; set; }
            public string Invoiceno { get; set; }
            public string Creditnoteno { get; set; }
            public Nullable<long> Firm_Id { get; set; }
            public string Pfcode { get; set; }
            [Required(ErrorMessage = "Date must be Required")]

            public Nullable<System.DateTime> tempch_date { get; set; }
            public string FormattedTempCrInsertedDate
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