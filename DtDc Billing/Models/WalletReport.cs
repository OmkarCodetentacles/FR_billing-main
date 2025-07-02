using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class WalletReport
    {
        public double ? Total_Wallet_Money { get; set; }
        public double ?  Total_Redeemed { get; set; }
        public int No_Of_Bookings { get; set; }
        public double ? Balance { get; set; }
        public string Mobile_No { get; set; }
        public string Name { get; set; }
        [System.ComponentModel.DisplayName("MF_Code")]
        public string Pfcode { get; set; }

    }
}