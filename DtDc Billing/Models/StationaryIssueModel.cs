using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class StationaryIssueModel
    {
        public int Issue_id { get; set; }

        [Required(ErrorMessage = "Please Enter Start No")]
        public string startno { get; set; }

        [Required(ErrorMessage = "Please Enter End No")]
        public string endno { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than 0.")]
        public Nullable<int> noofleafs { get; set; }

        
        public string Inssuedate { get; set; }

        [Required(ErrorMessage = "Please Enter Comapny")]
         public string Comapny_Id { get; set; }

        [Required(ErrorMessage = "Please Enter Employee Name")]
        public string EmployeeName { get; set; }

        public string Pf_code { get; set; }


    }
}