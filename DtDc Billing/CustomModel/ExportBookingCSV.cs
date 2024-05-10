using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class ExportBookingCSV
    {

        public int SrNo { get; set; }   
        public string ConsignmentNo { get; set; }
        public double? ChargableWeight { get; set; }
        public string Mode { get; set; }

        public string CompanyAddres { get; set; }
        public int Quanntity { get; set; }
        public string Pincode { get; set; }

        public string BookingDate { get; set; }

        public string Type { get; set;}
        public string CustomerId { get; set;}
        public float otherchanges { get; set;}
        public string Receiver { get; set;}
    }
}