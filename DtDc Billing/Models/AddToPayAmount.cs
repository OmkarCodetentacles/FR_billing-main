using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class AddToPayAmount
    {

        
            public int ad_id { get; set; }
            public string sapno { get; set; }
            [Required]
            public string consinmentno { get; set; }
            public string date { get; set; }
            [Required]
            public string Invoiceno { get; set; }
            public string pfcode { get; set; }
        
    }
}