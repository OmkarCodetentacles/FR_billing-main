using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class ContactUsModel
    {
        [Required]
        public string name { get; set; }

        public string surname { get; set; }

        [Required]
        public string email { get; set; }

        public string website { get; set; }
    }

    public class newsletters
    {
        [Required]
        public string email { get; set; }
    }
}