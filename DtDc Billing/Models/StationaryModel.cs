using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class StationaryModel
    {

        public int S_id { get; set; }

       
        public Nullable<System.DateTime> recieptdate { get; set; }

        [Required(ErrorMessage = "Please Enter Start No")]
        public string startno { get; set; }

        [Required(ErrorMessage = "Please Enter End No")]
        public string endno { get; set; }

        [Required(ErrorMessage = "Please Enter noofleafs")]
        [Range(1, int.MaxValue, ErrorMessage = "No of Leafs  value must be greater than 0.")]

        public Nullable<int> noofleafs { get; set; }

        [Required(ErrorMessage = "Please Enter noofbooks")]
        [Range(1, int.MaxValue, ErrorMessage = "No of Books value  must be greater than 0.")]
        public Nullable<int> noofbooks { get; set; }

        public Nullable<System.DateTime> Expiry_Date { get; set; }
        public Nullable<int> fid { get; set; }
        public string Invoiceno { get; set; }
        public string temprecdate { get; set; }
        public string tempExpiry_Date { get; set; }
        public string Pf_code { get; set; }
        public Nullable<int> Status { get; set; }


    }
}