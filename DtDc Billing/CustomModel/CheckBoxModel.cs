using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class functionalityModel
    {
        //Value of checkbox 
        public int Value { get; set; }
        //description of checkbox 
        public string Text { get; set; }
        //whether the checkbox is selected or not
        public bool IsChecked { get; set; }
    }
    public class functionalityList
    {
        //use CheckBoxModel class as list 
        public List<functionalityModel> fun { get; set; }
    }
}