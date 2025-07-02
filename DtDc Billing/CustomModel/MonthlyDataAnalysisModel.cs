using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class MonthlyDataAnalysisModel
    {
        public string PFCode { get; set; }
        public int InvoiceCount { get; set; }

        public decimal TotalInvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public string LastMonth { get; set; }
        public decimal CashAmount { get; set; }
        public string FranchiseName { get; set; }
        public string OwnerName { get; set; }
        public string EmailId { get; set; }
    }
}