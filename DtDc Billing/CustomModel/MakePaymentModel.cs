using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{

    public class MakePaymentModel
    {
        [Required]
        public string PaymentType { get; set; }
        [Required]
        public Nullable<double> Amount { get; set; }
        [Required]
        public Nullable<double> TdsAmount { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public string InvoiceNo { get; set; }
        public string ChequeNo { get; set; }
        public string TransactionId { get; set; }
        [Required]
        public Nullable<System.DateTime> tempinserteddate { get; set; }
        public Nullable<long> FirmId { get; set; }
        public string PfCode { get; set; }

        public string CompanyName { get; set; }


        public List<PaymentModel> paymentsList { get; set; }
    }
}
