using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DtDc_Billing.Models;

namespace DtDc_Billing.CustomModel
{
    public class dashboardDataModel
    {
        public int expiredStationaryCount { get; set; }
        public int openConCount { get; set; }
        public int unSignPincode { get; set; }
        public int invalidCon { get; set; }

        public double sumOfBillingCurrentMonth { get; set; }
        public double countofbillingcurrentmonth { get; set; }
        public Nullable<double> avgOfBillingCount { get; set; }
        public Nullable<double> SumOfBillingCurrentDay { get; set; }
        public Nullable<double> CountOfBillingCurrentDay { get; set; }

        public double sumOfCashcounterCurrentMonth { get; set; }
        public double countofCashcountercurrentmonth { get; set; }
        public Nullable<double> avgOfCashcounterCount { get; set; }
        public Nullable<double> SumOfCashcounterCurrentDay { get; set; }
        public Nullable<double> CountOfCashcounterCurrentDay { get; set; }


        public double todayExp { get; set; }
        public double monthexp { get; set; }
        public List<DestinationModel> DestinationList { get; set; }
        
        
    }
}