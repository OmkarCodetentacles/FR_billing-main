using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class SenderPhoneFromReceiptdetailsModel
    {
        public string sender_phone { get; set; }

        public string Sender { get; set; }

        public string Sender_Email { get; set; }

        public string SenderAddress { get; set; }

        public string SenderCompany { get; set; }

        public string SenderCity { get; set; }

        public string SenderState { get; set; }

        public string SenderPincode { get; set; }
    }
}