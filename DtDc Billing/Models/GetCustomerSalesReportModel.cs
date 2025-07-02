using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class GetCustomerSalesReportModel
    {
        public string Company_Id { get; set; }
        public string Company_Name { get; set; }
        public string Pf_code { get; set; }
        public string LastMonth { get; set; }
        public string PrevMonth { get; set; }
        public double LastMonthSales { get; set; }
        public double PrevMonthSales { get; set; }
        public double SalesDifference { get; set; }
        public Nullable<double> PercentageChange { get; set; }
        public string ChangeIndicator { get; set; }
    }
}