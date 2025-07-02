using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class TransactionModel
    {
        public long T_id { get; set; }
        [Required]
        public string Customer_Id { get; set; }
        public Nullable<System.DateTime> booking_date { get; set; }
        [Required]
        public string Consignment_no { get; set; }
        [Required]
        public string Pincode { get; set; }
        [Required]
        public string Mode { get; set; }
        public string Weight_t { get; set; }
        [Required]
        public string Amount { get; set; }
        public string Company_id { get; set; }
        public string Pf_Code { get; set; }
        public Nullable<int> Quanntity { get; set; }
        public string Type_t { get; set; }
        public string Insurance { get; set; }
        public string Claimamount { get; set; }
        public string Percentage { get; set; }
        public string calinsuranceamount { get; set; }
        public string remark { get; set; }
        public string topay { get; set; }
        public Nullable<double> codAmount { get; set; }
        public string consignee { get; set; }
        public string consigner { get; set; }
        public string cod { get; set; }
        public Nullable<double> TopayAmount { get; set; }
        public Nullable<double> Topaycharges { get; set; }
        public Nullable<double> Actual_weight { get; set; }
        public Nullable<double> codcharges { get; set; }
        public Nullable<double> codtotalamount { get; set; }
        public Nullable<double> dtdcamount { get; set; }
        public string chargable_weight { get; set; }
        public string status_t { get; set; }
        public Nullable<double> rateperkg { get; set; }
        public Nullable<double> docketcharege { get; set; }
        public Nullable<double> fovcharge { get; set; }
        public Nullable<double> loadingcharge { get; set; }
        public Nullable<double> odocharge { get; set; }
        public Nullable<double> Risksurcharge { get; set; }
        public Nullable<long> Invoice_No { get; set; }
        public Nullable<double> BillAmount { get; set; }
        [Required]
        public string tembookingdate { get; set; }

    }
}