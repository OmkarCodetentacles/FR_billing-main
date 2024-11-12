using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DtDc_Billing.Models
{
    public class EmailTemplateModel
    {
        public string RecipientName { get; set; }
        public string Status { get; set; }
        public string ActionUrl { get; set; }   

    }
}