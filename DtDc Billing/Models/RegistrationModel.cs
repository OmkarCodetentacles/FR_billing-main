using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static DtDc_Billing.Models.NoSpacesAttribute;

namespace DtDc_Billing.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]

    public class NoSpacesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string stringValue = value.ToString();

                // Check for spaces at the beginning or end
                if (stringValue.Contains(" "))
                {
                    return new ValidationResult("Username or Password cannot Contain space");
                }
            }

            return ValidationResult.Success;
        }
    }

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
        [NoSpaces(ErrorMessage = "User Name cannot contain space")]
        [RegularExpression(@"^.{4,}$", ErrorMessage = "User Name must be greater than or equal 4 characters.")]
        public string userName { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        [NoSpaces(ErrorMessage = "Password cannot contain space")]
        [Compare("password")]
        public string confirmPassword { get; set; }

        public Nullable<bool> isPaid { get; set; }

        [Required]
        public string mobileNo { get; set; }

        public string address { get; set; }

        public string referralCode { get; set; }
        public string referral { get; set; }

        public Nullable<bool> isUserNameExist { get; set; }

        public Nullable<bool> isEmailConfirmed { get; set; }
        public string emailOTP { get; set; }
    }
}
