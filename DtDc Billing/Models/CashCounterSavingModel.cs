using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class CashCounterSavingModel
    {


        public long Saving_ID { get; set; }

        [Required(ErrorMessage = "Please Enter Amount")]  //@"^[0-9]+(\.[0-9])$"
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Amount")]
        public Nullable<float> Saving_amount { get; set; }
        public string Pf_Code { get; set; }

        [Required(ErrorMessage = "Please Enter Reason")]
        public string Rason { get; set; }
        public Nullable<System.DateTime> Datetime_Sav { get; set; }
        public Nullable<long> User_Id { get; set; }

        public virtual User User { get; set; }
        public virtual Franchisee Franchisee { get; set; }
    }
}