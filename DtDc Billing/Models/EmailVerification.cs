using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class EmailVerification
    {
        [Required]
        [MaxLength(50)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Please enter correct email")]

        public string Email { get; set; }
        [Required]
        public string PF_Code { get; set; }
        //  public Nullable<bool> isEmailConfirmed { get; set; }
        [Required(ErrorMessage ="Enter Valid OTP")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 characters long")]
        public string EmailOTP { get; set; }
    }
}