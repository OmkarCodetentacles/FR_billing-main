using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class CashCounterPaymentModel
    {

        public long Pay_ID { get; set; }

        [Required(ErrorMessage = "Please Enter Consignment No")]
        public string Consignment_No { get; set; }

        [Required(ErrorMessage = "Please Enter Amount")]  //@"^[0-9]+(\.[0-9])$"
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Amount")]
        public Nullable<float> amount { get; set; }
        public string Pf_Code { get; set; }

        [Required(ErrorMessage = "Please Enter Description")]
        public string Description_ { get; set; }
        public Nullable<System.DateTime> Datetime_Pay { get; set; }
        public Nullable<long> User_Id { get; set; }

        public virtual User User { get; set; }
        public virtual Franchisee Franchisee { get; set; }
    }
}