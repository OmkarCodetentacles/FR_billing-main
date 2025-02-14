using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class GetCustomerCreditModel
    {
        public string CustomerId { get;set; }
        public double TotalCreditPayment { get;set; }
        public double LastBalanceAmount { get;set; }
    }
}