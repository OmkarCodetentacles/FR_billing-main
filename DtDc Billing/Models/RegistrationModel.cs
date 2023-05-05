using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class RegistrationModel
    {
        [Required]
        public string Pfcode { get; set; }
        
        public string franchiseName { get; set; }
        [Required]
        [MaxLength(50)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string emailId { get; set; }
        public Nullable<System.DateTime> dateTime { get; set; }
        
        public string ownerName { get; set; }

        [Required]
        [RegularExpression(@"^(?:.*[a-z]){4,}$", ErrorMessage = "user Name must be greater than or equal 4 characters.")]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        [Compare("password")]
        public string confirmPassword { get; set; }

        public Nullable<bool> isPaid { get; set; }

        [Required]
        public string mobileNo { get; set; }
        
        public string address { get; set; }

        public string referralCode { get; set; }
        public string referral { get; set; }

        public bool isUserNameExist { get; set; }


    }
}