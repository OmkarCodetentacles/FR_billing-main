using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class AddNotification
    {
      
        [Required(ErrorMessage = "Please Enter Message")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Please Enter Date")]
        public Nullable<System.DateTime> dateN { get; set; }

        [Required(ErrorMessage = "Please Enter Path")]
        public string url_path { get; set; }

        public string description { get; set; }




    }
}