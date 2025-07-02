using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class AddModuleList
    {
        public int Value { get; set; }
        
        public string Text { get; set; }
       
        public bool IsChecked { get; set; }
    }

    public class Modulelist
    {
        //use CheckBoxModel class as list 
        public List<AddModuleList> Moduleli { get; set; }
    }
}