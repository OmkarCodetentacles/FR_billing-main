using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.CustomModel
{
    public class notificationModel
    {
        public int N_ID { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> dateN { get; set; }
        public Nullable<bool> Status { get; set; }
        public string url_path { get; set; }
        public string description { get; set; }

        public string tempdate { get; set; }
    }
}
