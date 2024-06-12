using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class AddCodAmount
    {

      
            public int am_id { get; set; }
            [Required(ErrorMessage = "Check Number must be Required")]

            public string chequeno { get; set; }
            public string bank_name { get; set; }
            public string branch { get; set; }
            [Required]

            public string consinment_no { get; set; }
            public string Invoiceno { get; set; }
            public string pfcode { get; set; }
        
    }
}