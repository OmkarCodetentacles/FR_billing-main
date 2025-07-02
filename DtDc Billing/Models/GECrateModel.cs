using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DtDc_Billing.Models
{
    public class GECrateModel
    {
        public long GECrateId { get; set; }
        public Nullable<double> Slab1 { get; set; }
        public Nullable<double> Slab2 { get; set; }
        public Nullable<double> Slab3 { get; set; }
        public Nullable<double> Slab4 { get; set; }
        public Nullable<double> Uptosl1 { get; set; }
        public Nullable<double> Uptosl2 { get; set; }
        public Nullable<double> Uptosl3 { get; set; }
        public Nullable<double> Uptosl4 { get; set; }
        public string Company_id { get; set; }
        public Nullable<int> NoOfSlab { get; set; }
        public Nullable<int> Sector_Id { get; set; }
        public string PFcode { get; set; }

        public Sector Sector { get; set; }
        public string SectorName { get; set; }
    }
    public class GECrateDTO
    {
        public long GECrateId { get; set; }
        public Nullable<double> Slab1 { get; set; }
        public Nullable<double> Slab2 { get; set; }
        public Nullable<double> Slab3 { get; set; }
        public Nullable<double> Slab4 { get; set; }
        public Nullable<double> Uptosl1 { get; set; }
        public Nullable<double> Uptosl2 { get; set; }
        public Nullable<double> Uptosl3 { get; set; }
        public Nullable<double> Uptosl4 { get; set; }
        public Nullable<int> NoOfSlab { get; set; }
        public string Company_id { get; set; }
        public string PFcode { get; set; }

    }

}