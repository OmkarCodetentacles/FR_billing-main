using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public partial class Point
    {
        public int amount { get; set; }
        public Nullable<int> y { get; set; }
    }
}