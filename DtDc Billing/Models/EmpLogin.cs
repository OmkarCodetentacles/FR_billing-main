using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class EmpLogin
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Please Enter Username")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
    }
}