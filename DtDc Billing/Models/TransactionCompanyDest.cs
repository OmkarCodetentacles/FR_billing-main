using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class TransactionCompanyDest
    {
        public long T_id { get; set; }
        public string Customer_Id { get; set; }
        public Nullable<System.DateTime> booking_date { get; set; }
        public string Consignment_no { get; set; }
        public string Mode { get; set; }
        public string Weight_t { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Company_id { get; set; }
        public string Pincode { get; set; }
        public string Pf_Code { get; set; }
        public Nullable<int> Quanntity { get; set; }
        public string Type_t { get; set; }
        public string Insurance { get; set; }
        public string Claimamount { get; set; }
        public string Percentage { get; set; }
        public string calinsuranceamount { get; set; }
        public string topay { get; set; }
        public Nullable<double> codAmount { get; set; }
        public string consignee { get; set; }
        public string consigner { get; set; }
        public string cod { get; set; }
        public Nullable<double> TopayAmount { get; set; }
        public Nullable<double> Topaycharges { get; set; }
        public Nullable<double> codcharges { get; set; }
        public Nullable<double> codtotalamount { get; set; }
        public Nullable<double> dtdcamount { get; set; }
        public string status_t { get; set; }
        public Nullable<double> rateperkg { get; set; }
        public Nullable<double> docketcharege { get; set; }
        public Nullable<double> fovcharge { get; set; }
        public Nullable<double> loadingcharge { get; set; }
        public Nullable<double> odocharge { get; set; }
        public Nullable<double> Risksurcharge { get; set; }
        public Nullable<long> Invoice_No { get; set; }
        public Nullable<double> BillAmount { get; set; }
        public string tembookingdate { get; set; }
        public Nullable<double> Actual_weight { get; set; }
        public Nullable<double> chargable_weight { get; set; }
        public Nullable<int> AdminEmp { get; set; }
        public Nullable<double> diff_weight { get; set; }
        public string Name { get; set; }
        public string tempdelivereddate { get; set; }
        public string receivedby { get; set; }
        public string remarks { get; set; }
        public string tempdeliveredtime { get; set; }

        public string Company_Address { get; set; }
        public string Email { get; set; }
        public Nullable<long> Phone { get; set; }

        public Nullable<double> Fuel_Sur_Charge { get; set; }
        [Display(Name = "Topay Charge")]
        public Nullable<double> Topay_Charge { get; set; }
        [Display(Name = "Cod Charge")]
        public Nullable<double> Cod_Charge { get; set; }
        [Display(Name = "Gec Fuel Sur Charge")]
        public Nullable<double> Gec_Fuel_Sur_Charge { get; set; }

        [Display(Name = "Royalty Charges")]
        public Nullable<double> Royalty_Charges { get; set; }
        [Display(Name = "D")]
        public Nullable<double> D_Docket { get; set; }
        [Display(Name = "P")]
        public Nullable<double> P_Docket { get; set; }
        [Display(Name = "E")]
        public Nullable<double> E_Docket { get; set; }
        [Display(Name = "V")]
        public Nullable<double> V_Docket { get; set; }
        [Display(Name = "I")]
        public Nullable<double> I_Docket { get; set; }
        [Display(Name = "N")]
        public Nullable<double> N_Docket { get; set; }
        public string Gst_No { get; set; }

        public double ? CnoteCharges { get; set; }

        public double? RoyalCharges { get; set; }

        public double? ServiceCharges { get; set; }

        public double? FscAmt { get; set; }

        public double? Cgst { get; set; }
        public double? Sgst { get; set; }
        public double? Igst { get; set; }

        public double? CNote_cost { get; set; }

        public string Fr_Gst_No { get; set; }


    }
}