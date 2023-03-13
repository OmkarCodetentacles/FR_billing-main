using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class AddlogoModel
    {

        [Required(ErrorMessage = "Please Select File")]
       // [RegularExpression(@"(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))+(.jpeg|.JPEG|.jpg|.JPG|.gif|.GIF|.png|.PNG)$", ErrorMessage = "Only Image files allowed.")]
        public HttpPostedFileBase file { get; set; }
    }
}