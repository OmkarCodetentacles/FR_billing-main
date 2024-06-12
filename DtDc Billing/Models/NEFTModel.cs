using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class NEFTModel
    {

            public int Neft_id { get; set; }
            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]

            public Nullable<double> NeftAmount { get; set; }
            public Nullable<System.DateTime> neftdate { get; set; }
            public string Invoiceno { get; set; }
            public string Transaction_Id { get; set; }
            public Nullable<double> N_Tds_Amount { get; set; }
            public Nullable<double> N_Total_Amount { get; set; }
            public Nullable<long> Firm_Id { get; set; }
            public string Pfcode { get; set; }
            [Required(ErrorMessage = "Date must be Required")]
            public string FormattedTempNEFTInsertedDate
            {
                get
                {
                    return tempneftdate?.ToString("dd-MM-yyyy");
                }
            }

            public Nullable<System.DateTime> tempneftdate { get; set; }

            public virtual FirmDetail FirmDetail { get; set; }
            public virtual FirmDetail FirmDetail1 { get; set; }
        
    }
}