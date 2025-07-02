using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class SendMsgModel
    {

        public long M_ID { get; set; }
        public string Consignment_No { get; set; }
        public string Mobile_No { get; set; }
        public Nullable<System.DateTime> datetime_msg { get; set; }
        public Nullable<long> User_Id { get; set; }
        public string PF_Code { get; set; }

        public virtual User User { get; set; }
        public virtual Franchisee Franchisee { get; set; }
    }
}