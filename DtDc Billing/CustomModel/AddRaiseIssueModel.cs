using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class AddRaiseIssueModel
  {
    
    public string RaiseIssue_Description { get; set; }
   

    [Required(ErrorMessage = "Please Enter Raise Issue Name")]
    public string RaiseIssue_Name { get; set; }

    [Required(ErrorMessage = "Please Enter Domain")]
    public string Domain_Name { get; set; }

   
    [Required(ErrorMessage = "Please Select File")]
    public HttpPostedFileBase file { get; set; }
  }
}
