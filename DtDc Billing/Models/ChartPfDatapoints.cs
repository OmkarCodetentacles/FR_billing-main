using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DtDc_Billing.Models
{
    [DataContract]
    public class ChartPfDatapoints
    {
        public ChartPfDatapoints(string label, double ? y)
        {
            this.Label = label;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }

}
