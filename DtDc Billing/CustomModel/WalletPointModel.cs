using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class WalletPointModel
    {
        public long Wallet_Id { get; set; }
        public string MobileNo { get; set; }
        public Nullable<float> Wallet_Money { get; set; }
        public Nullable<System.DateTime> Datetime_Wa { get; set; }
        public float Redeemed { get; set; }
        public string Name { get; set; }
        public string PFCode { get; set; }
    }
}