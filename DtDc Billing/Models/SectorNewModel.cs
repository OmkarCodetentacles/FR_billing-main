using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DtDc_Billing.Models
{
    public class SectorNewModel
    {
        public int Sector_Id { get; set; }
        public string Sector_Name { get; set; }
        public string Pf_code { get; set; }
        public string Pincode_values { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<bool> CashD { get; set; }
        public Nullable<bool> CashN { get; set; }

        public bool BillD { get; set; }
        
        public bool BillNonAir { get; set; }
        public bool BillNonSur { get; set; }
        
        public bool BillExpCargo { get; set; }
        public bool BillPriority { get; set; }
        
        public bool BillEcomPrio { get; set; }
        public bool BillEcomGE { get; set; }

    }
}