using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class FranchiseeModel
    {

        public int F_Id { get; set; }
        public string PF_Code { get; set; }

        [Required(ErrorMessage = "Please Enter Franchisee Address")]
        public string F_Address { get; set; }

        [Required(ErrorMessage = "Please Enter Owner Name")]
        public string OwnerName { get; set; }

        public string BranchName { get; set; }

        [Required(ErrorMessage = "Please Enter GST No")]
        public string GstNo { get; set; }

        [Required(ErrorMessage = "Please Enter Franchisee Name")]
        public string Franchisee_Name { get; set; }

        public string ContactNo { get; set; }
        public string Branch_Area { get; set; }
        public Nullable<System.DateTime> Datetime_Fr { get; set; }

        [Required(ErrorMessage = "Please Enter PAN No")]
        public string Pan_No { get; set; }    
        
        public string Sendermail { get; set; }
        public string password { get; set; }
        public string AccountName { get; set; }
        public string Bankname { get; set; }
        public string Accountno { get; set; }
        public string IFSCcode { get; set; }
        public string Branch { get; set; }
        public string Accounttype { get; set; }

        [Required(ErrorMessage = "Please Enter Invoice Start Prefix")]
        public string InvoiceStart { get; set; }

    }
}