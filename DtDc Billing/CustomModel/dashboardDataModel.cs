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

        public int complaintCount { get; set; }

        public double sumOfBilling { get; set; }

        public int countOfBilling { get; set; }

        public double avgOfBillingSum { get; set; }

        public double sumOfBillingCurrentMonth { get; set; }

        public double countofbillingcurrentmonth { get; set; }

        public double todayExp { get; set; }

        public double monthexp { get; set; }

        public List<Notification> notificationsList { get; set; }

        public List<UserModuleList> UserModuleList { get; set; }

        public List<DestinationModel> DestinationList { get; set; }
     
        public   List<ConsignmentCount> Consignmentcount = new List<ConsignmentCount>();

        //public IEnumerable<DestinationModel> dest { get; set; }
        
        public Nullable<int> complaintcount { get; set; }
        
        public Nullable<double> avgOfBillingCount { get; set; }
        public Nullable<double> SumOfBillingCurrentDay { get; set; }
        public Nullable<double> CountOfBillingCurrentDay { get; set; }
        

    }
}