using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class AdminLogin
    {

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Please Enter Username")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Please Enter PFCode")]
        //[Display(Name = "PFCode")]
        //public string PFCode { get; set; }


    }
}