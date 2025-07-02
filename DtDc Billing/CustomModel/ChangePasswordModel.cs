using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    

    public class ChangePasswordModel
    {
        [Required]
        public string oldpass { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string newpass { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("newpass", ErrorMessage = "The new password and confirmation password do not match.")]
        public string confirmpass { get; set; }
        public string pfocde { get; set; }
        //[Required]
        //public string Token { get; set; }
    }
}