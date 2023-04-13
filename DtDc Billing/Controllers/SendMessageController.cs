using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    //[SessionTimeout]
    //[SessionUserModule]
    public class SendMessageController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: SendMessage
        public ActionResult Pickupmsg()
        {
            ViewBag.pickuptable = db.Sendmessages.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pickupmsg(SendMsgModel sendmessage)
        {
            string strpf = Session["pfCode"].ToString();

            //  string phone = db.Receipt_details.Where(m => m.Consignment_No == sendmessage.Consignment_No).Select(m => m.sender_phone).FirstOrDefault();

            if (ModelState.IsValid)
            {
                
                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

              
                Sendmessage sm = new Sendmessage();

                sm.Consignment_No = sendmessage.Consignment_No;
                sm.Mobile_No = sendmessage.Mobile_No;
                sm.PF_Code = Session["pfCode"].ToString();
                sm.User_Id = 0;//Convert.ToInt64(Session["EmpId"]);
                sm.datetime_msg = localTime;



                db.Sendmessages.Add(sm);
                db.SaveChanges();

                sendmsg(sendmessage.Mobile_No, sendmessage.Consignment_No);
                TempData["Success"] = "sent";
                return View();
            }

            ViewBag.pickuptable = db.Sendmessages.Where(d=>d.PF_Code== strpf).ToList();
            return View();
        }


        public ActionResult SendBulkmsg()
        {
            string strpf = Session["pfCode"].ToString();

            long User_Id = Convert.ToInt64(Session["EmpId"]);

            List<Receipt_details> list = db.Receipt_details.Where(m =>  m.User_Id == User_Id && m.Pf_Code== strpf).ToList();

            return View(list);
        }

        [HttpPost]
        public ActionResult SendBulkmsg(string Consignname, string ToDatetime,string Fromdatetime,string submitbtn,string message)
        {
            List<Receipt_details> list = new List<Receipt_details>();
            if (Fromdatetime == "")
            {
                ModelState.AddModelError("Fromdateeror", "Please select Date");
            }
            else if (ToDatetime == "")
            {
                ModelState.AddModelError("Todateeror", "Please select Date");
            }
            else
            {

                ViewBag.Fromdatetime = Fromdatetime;
                ViewBag.ToDatetime = ToDatetime;

                DateTime? todate = Convert.ToDateTime(ToDatetime,
        System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                DateTime? fromdate = Convert.ToDateTime(Fromdatetime,
        System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                long User_Id = Convert.ToInt64(Session["EmpId"]);

                 list = db.Receipt_details.Where(m => m.Consignment_No.StartsWith(Consignname) && m.User_Id == User_Id
                && m.Datetime_Cons.Value.Day >= fromdate.Value.Day
                && m.Datetime_Cons.Value.Year >= fromdate.Value.Year
                && m.Datetime_Cons.Value.Month >= fromdate.Value.Month

              && m.Datetime_Cons.Value.Day <= todate.Value.Day
              && m.Datetime_Cons.Value.Month <= todate.Value.Month
              && m.Datetime_Cons.Value.Year <= todate.Value.Year

                ).ToList();



                if (submitbtn == "msagsend")
                {
                    foreach (var i in list)
                    {
                        sendmsginbulk(i.sender_phone, message);
                    }
                }
                return View(list);
            }


            return View(list);

        }



        public void sendmsg(string phone,string Consignmentno )
        {
            string pfcode = Session["pfCode"].ToString();
            Franchisee branchname = db.Franchisees.Where(m => m.PF_Code == pfcode).FirstOrDefault();


            string url = "http://smsc.biz/httpapi/send?username=santoshmaske99@outlook.com&password=97sai22&sender_id=DTDCFR&route=T&phonenumber=" + phone+"&message=Dear%20sir%20your%20shipment%20%"+Consignmentno+"%20is%20pending%20at%20DTDC%20BRANCH%20pls%20collect%20from%20%"+branchname.BranchName+"%20office%20pls%20NOTE%20it%20will%20go%20back%20in%203%20days%20pls%20call%20%"+branchname.ContactNo+"";


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:56851/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(url).Result;            

        }


        public void sendmsginbulk(string phone,string message)
        {
            string urmmsg = HttpUtility.UrlEncode(message);

            string pfcode = Session["pfCode"].ToString();
            Franchisee branchname = db.Franchisees.Where(m => m.PF_Code == pfcode).FirstOrDefault();


            string url = "http://sms.googlymart.com/httpapi/send?username=santoshmaske99@outlook.com&password=97sai22&sender_id=PROMOTIONAL&route=P&phonenumber=" + phone+"&message="+urmmsg+"";


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:56851/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(url).Result;

        }



    }
}