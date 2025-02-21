using DtDc_Billing.CustomModel;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Razorpay.Api;
using Microsoft.Reporting.WebForms;
using System.Net;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System.Text.RegularExpressions;
using System.Text;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using static DtDc_Billing.invo;

namespace DtDc_Billing.Controllers
{
    [SessionAdminold]
 //  [SessionUserModule]
  
    public class HomeController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
       
        [PageTitle("Home Index")]
        public ActionResult Index()
        {
            DateTime? dateTime;
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.PfCode = PfCode;
            dateTime = DateTime.Now;
            ViewBag.date = String.Format("{0:dd/MM/yyyy}", dateTime);
            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

            var obj = db.dashboardData(localTime, PfCode).Select(x => new dashboardDataModel
            {
                openConCount = x.openConCount ?? 0,
                unSignPincode = x.unSignPincode ?? 0,

                sumOfBillingCurrentMonth = x.sumOfBillingCurrentMonth ?? 0,
                countofbillingcurrentmonth = x.countofbillingcurrentmonth ?? 0,
                SumOfBillingCurrentDay = x.SumOfBillingCurrentDay ??0,
                CountOfBillingCurrentDay= x.CountOfBillingCurrentDay ??0,

                sumOfCashcounterCurrentMonth = x.sumOfCashcounterCurrentMonth ?? 0,
                countofCashcountercurrentmonth = x.countofCashcountercurrentmonth ?? 0,
                SumOfCashcounterCurrentDay = x.SumOfCashcounterCurrentDay ?? 0,
                CountOfCashcounterCurrentDay = x.CountOfCashcounterCurrentDay ?? 0,

                todayExp = x.todayExp ?? 0,
                monthexp = x.monthexp ?? 0

            }).FirstOrDefault();

          
           
            ViewBag.ShowModal = TempData["ShowModal "];
           
            DateTime After30days = localTime.AddDays(30);

            var Date = (from d in db.registrations
                        where d.Pfcode == PfCode
                        select new
                        {
                            d.dateTime,
                            d.paymentDate,
                            d.subscriptionForInDays,
                            d.userName,
                            d.mobileNo

                        }).FirstOrDefault();

            
            DateTime currentDate = localTime;

            System.DateTime newDate = Date.paymentDate.Value.AddDays(Date.subscriptionForInDays ?? 0);
            TimeSpan date_difference = newDate - currentDate;
            DateTime before15days = newDate.AddDays(-15);
            DateTime before1day = newDate.AddDays(-1);
            DateTime  before30days= newDate.AddDays(-30);
            DateTime before10days=newDate.AddDays(-10);
            
            var mobileno = Date.mobileNo;

            ViewBag.subscriptionExpiredOnDate = newDate.ToString("dd/MM/yyyy");
            ViewBag.subscriptionExpiredOnDays = (newDate - currentDate).Days;
            

            if (currentDate >= before10days && currentDate < newDate)
            {
                ViewBag.ExpiryMessage = "Your subscription is expiring on " + newDate + ". Please Renew to continue enjoying our services.";


            }
            if (currentDate == before30days)
            {

                var message = "🔔 **Renewal Reminder * * 🔔\r\n\r\n" +
                    "Hello[Recipient's Name],\r\n\r\n" +
                    "I hope this message finds you well. 👋 Your subscription is expiring on Tomorrow. " +
                    "📆 To continue enjoying our fantastic services, please consider Renewing your subscription." +
                    "\r\n\r\n🔄 **Renew Now * * 🔄\r\n\r\n" +
                    "We appreciate your continued support!" +
                    " If you have any questions or need assistance, feel free to reach out." +
                    " \'91+9209764995'\r\n\r\n" +
                    "Thank you!\r\n\r\n[" + Date.userName + "]";
                SendWhatsappMessage sw = new SendWhatsappMessage();
                Task<string> whatsappmessage = sw.sendWhatsappMessage(mobileno, message);

            }
            else if(currentDate==before15days)
            {
                var message = "🔔 **Renewal Reminder * * 🔔\r\n\r\n" +
                  "Hello[Recipient's Name],\r\n\r\n" +
                  "I hope this message finds you well. 👋 Your subscription is expiring on Tomorrow. " +
                  "📆 To continue enjoying our fantastic services, please consider Renewing your subscription." +
                  "\r\n\r\n🔄 **Renew Now * * 🔄\r\n\r\n" +
                  "We appreciate your continued support!" +
                  " If you have any questions or need assistance, feel free to reach out." +
                  " \'91+9209764995'\r\n\r\n" +
                  "Thank you!\r\n\r\n[" + Date.userName + "]";
                SendWhatsappMessage sw = new SendWhatsappMessage();
                Task<string> whatsappmessage = sw.sendWhatsappMessage(mobileno, message);
            }
            else if (currentDate>=before10days && currentDate<=before1day)
            {
                var message = "🔔 **Renewal Reminder * * 🔔\r\n\r\n" +
                "Hello[Recipient's Name],\r\n\r\n" +
                "I hope this message finds you well. 👋 Your subscription is expiring on Tomorrow. " +
                "📆 To continue enjoying our fantastic services, please consider Renewing your subscription." +
                "\r\n\r\n🔄 **Renew Now * * 🔄\r\n\r\n" +
                "We appreciate your continued support!" +
                " If you have any questions or need assistance, feel free to reach out." +
                " \'91+9209764995'\r\n\r\n" +
                "Thank you!\r\n\r\n[" + Date.userName + "]";
                SendWhatsappMessage sw = new SendWhatsappMessage();
                Task<string> whatsappmessage = sw.sendWhatsappMessage(mobileno, message);

            }


          //  Fetch the necessary data without using AddDays in the query
            var invoiceData = (from inv in db.Invoices
                               join comp in db.Companies
                               on inv.Customer_Id equals comp.Company_Id
                               where inv.Pfcode.Equals(PfCode) && comp.Pf_code.Equals(PfCode)
                               && inv.invoicedate!=null
                               && inv.isDelete == false
                               select new
                               {
                                   DueDaydata = comp.DueDays ?? 0,
                                   Company_Iddata = inv.Customer_Id,
                                   InvoiceDate = inv.invoicedate,
                                   InvoiceNo=inv.invoiceno
                               }).ToList();

          //  Perform the date calculations in memory
             var Duedaysdata= (from d in invoiceData
                                 select new DueDaysModel
                                 {
                                     Date=d.InvoiceDate.Value.AddDays(d.DueDaydata),
                                     DueDays=(d.InvoiceDate.Value.AddDays(d.DueDaydata).Date-DateTime.Now.Date).Days,
                                     Company_Id=d.Company_Iddata,
                                     InvoiceNo=d.InvoiceNo  
                                 
                                 }).OrderBy(d=> d.DueDays).ToList();  
            
            var Duedysexpires = Duedaysdata.Where(x=>x.DueDays>0 && x.DueDays<5).Count();

            ViewBag.DueDaysExpire = Duedysexpires;
          
            return View(obj);
        }
        
        public PartialViewResult GetTop5Customer()
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);

            var data = db.Invoices
                         .Where(x => x.Pfcode == PfCode && x.invoicedate >= threeMonthsAgo && x.isDelete==false)
                         .GroupBy(x => x.Customer_Id)
                         .Select(g => new Top5data {
                             customerId = g.FirstOrDefault().Customer_Id,
                             NetAmount = g.Sum(x => x.netamount)
                         })
                         .OrderByDescending(x => x.NetAmount)
                         .Take(5)
                         .ToList();
            return PartialView(data);   

        }

        public PartialViewResult DestinationAndProductPartial()
        {
           
            dashboardDataModel obj = new dashboardDataModel();

            obj.DestinationList = db.destinationCount(Request.Cookies["Cookies"]["AdminValue"].ToString()).Select(x => new DestinationModel
            {
                City = x.Destination,
                Count = x.DestCount ?? 0

            }).OrderByDescending(x=>x.Count).Take(5).ToList();

            ViewBag.DestinationCount = Convert.ToInt32(obj.DestinationList.Count());

            return PartialView(obj);
        }


        public PartialViewResult NoteConsignmentPartial()
        {
            return PartialView();
        }
            public void BacDb()
        {
            //SqlConnection sqlconn = new SqlConnection(@"Data Source=sql5104.site4now.net;Initial Catalog=db_a71c08_elitetoken; User ID=db_a71c08_elitetoken_admin; Password=Test@123; Connection Timeout=15;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true");
            SqlCommand sqlcmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();

          
            try
            {
                string dbNAme = "db_a71c08_elitetoken";
                string backupDestination = "C:\\BackDB";//Server.MapPath("~/BackUp");

                if (!Directory.Exists(backupDestination))
                {
                    Directory.CreateDirectory(backupDestination);
                }
                string fileName = dbNAme + " of " + DateTime.Now.ToString("yyyy-MM-dd@HH_mm") + ".bak";
                //string conString = ConfigurationManager.ConnectionStrings["db_a92afa_frbillingEntities"].ConnectionString;

                // string conString = @"Server=sql5104.site4now.net;database=db_a71c08_elitetoken;user id=db_a71c08_elitetoken_admin;password=Test@123;Integrated Security=true;Connection Timeout=60;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true";
                string conString = @"Data Source=sql5104.site4now.net;Initial Catalog=db_a71c08_elitetoken; User ID=db_a71c08_elitetoken_admin; Password=Test@123; Connection Timeout=60;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true";

                string query = "BACKUP database " + dbNAme + " to disk='" + backupDestination + "\\" + fileName + "'";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = query;
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteScalar();
                        con.Close();
                    }
                }
                Response.Write("Backup database successfully");
            }
            catch (Exception ex)
            { }

        }

        public void Backup()
        {
            try
            {
                string backlocation = "C://BackDB/";//Server.MapPath("~/BackupFolder/");
                String query = "BACKUP database db_a71c08_elitetoken to disk='" + backlocation + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".Bak'";
                string mycon = @"Data Source=sql5104.site4now.net;Initial Catalog=db_a71c08_elitetoken; User ID=db_a71c08_elitetoken_admin; Password=Test@123; Connection Timeout=60;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=100;Pooling=true";
               SqlConnection con = new SqlConnection(mycon);
               
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = query;
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                Console.WriteLine("Backup of Database Has Been Done Successfully");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error Occured While Creating Backup of Database Error Code" + ex.ToString());
            }
        }

        public ActionResult UserModelPartial()
        {

            return PartialView("UserModelPartial");
        }

        public ActionResult BillingUserModelPartial()
        {

            return PartialView("BillingUserModelPartial");
        }

        public ActionResult NotificationPartial()
        {

            return PartialView("NotificationPartial");
        }


        public ActionResult Company()
        {
           
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        
        public ActionResult CreateCompanyPartial()
        {
         

            return PartialView();
        }

        
        [HttpPost]
        public ActionResult CreateCompanyPartial(DtDc_Billing.Entity_FR.Company company)
        {
            if (!ModelState.IsValid)
            {
                // prepare and populate required data for the input fields
                // . . .

                return PartialView("Createoredit");
            }
            else
            {
                return PartialView(company);
            }
        }


        public ActionResult ajax1()
        {
            return View();
        }

        public JsonResult GetNotifications()
        {

            db.Configuration.ProxyCreationEnabled = false;

            var notifications = db.Notifications.OrderByDescending(m=>m.N_ID).ToList();
            
            return Json(notifications,JsonRequestBehavior.AllowGet);

        }

      
        ////////////////////////////////////////////

        //public ActionResult steplinechart()
        //{

        //    List<steplinechart> dataPoints = new List<steplinechart>();
        //    var inv = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate) , invoicedate= m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Take(12).ToList();

        //    foreach (var i in inv)
        //    {
        //        steplinechart data = new steplinechart(i.netamount, i.month, i.Year,i.day);
        //        dataPoints.Add(data);
        //    }

        //    ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

        //    return View();
        //}

        /// ////////////////////////////////////////////////
        /// 

        public ActionResult Salesstatistics()
        {
            return View();
        }

        public JsonResult PivotData()
        {
            
            var results = (from c in db.Invoices
                           group c by new
                           {
                               month = SqlFunctions.DatePart("month", c.invoicedate) + "-" + SqlFunctions.DatePart("year", c.invoicedate),
                               c.Customer_Id,
                           } into gcs
                           select new
                           {
                               Customer_id = gcs.Key.Customer_Id,
                               month = gcs.Key.month,
                               NetAmount = gcs.Sum(c => c.netamount),
                           }).ToList();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RenewalPanel()
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            userDetailsModel user = new userDetailsModel();

            var data = db.registrations.Where(x => x.Pfcode == PfCode).FirstOrDefault();

            user.name = data.userName;
            user.email = data.emailId;
            user.address = data.address;
            user.mobileNo = data.mobileNo;
            
            ViewBag.franchiseename = data.franchiseName;
            ViewBag.StartDate = data.dateTime.Value.ToString("dd/MM/yyyy");
            //ViewBag.Count = data;

            //ViewBag.name = data.userName;
            //ViewBag.email = data.emailId;
            //ViewBag.address = data.address;
            //ViewBag.mobileNo = data.mobileNo;

            return View(user);
        }

        public ActionResult Pay(string paymentid)
        {
            //try
            //{
            var paymentlog = db.paymentLogs.Where(x => x.paymentLogId == "pay_KMXk1qOGUda3K6").FirstOrDefault();

            if (paymentlog != null)
            {
                ModelState.AddModelError("customErrorPay", "Pfcode already exist");
                //return PartialView("RegistrationPartialView", userDetails);
            }

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var key = "rzp_test_ORKmnTOESzja0p";
            var key_secret = "MIDsnrVFquZJY8MPVSGPbOYs";
            RazorpayClient client = new RazorpayClient(key, key_secret);

            Razorpay.Api.Payment payment = client.Payment.Fetch(paymentid);



            string status = (string)payment["status"];

            var amount = (double)payment["amount"] / 100;
            var description = (string)payment["description"];
            var email = (string)payment["email"];
            var paymentmethod = (string)payment["method"];
            var RenewalPaymentStatus = "1";
            var userdetails = db.registrations.Where(x => x.emailId == email).FirstOrDefault();


            if (status == "authorized")
            {

                var save = db.RenewalpaymentLogSave(userdetails.Pfcode, userdetails.ownerName, amount, userdetails.registrationId, paymentid, status, DateTime.Now, description, RenewalPaymentStatus, DateTime.Now, paymentmethod);


                //var userid = Convert.ToInt64(Session["User"]);

                var Registration = db.registrations.Where(m => m.emailId == email).ToList();
                var PaymentLog = db.paymentLogs.Where(x => x.paymentLogId == paymentid).ToList();
                var grandTotal = (PaymentLog.FirstOrDefault().totalAmount + ((PaymentLog.FirstOrDefault().totalAmount * 18) / 100)).ToString();
                PaymentLog.FirstOrDefault().status = InWords.NumberToWords(grandTotal);
                // var user_id = DataSet2.FirstOrDefault().userid;

                //DataSet2.FirstOrDefault().status = AmountTowords.changeToWords(DataSet1.FirstOrDefault().plan_price.ToString());

                // PaymentLog.FirstOrDefault().totalAmount = (PaymentLog.FirstOrDefault().totalAmount / 100);

                //var email1 = DataSet2.FirstOrDefault().email_id;
                LocalReport lr = new LocalReport();

                string path = Path.Combine(Server.MapPath("~/RDLC"), "Invoice.rdlc");

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                ReportDataSource rd1 = new ReportDataSource("Registration", Registration);
                ReportDataSource rd = new ReportDataSource("PaymentLog", PaymentLog);

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);

                string reportType = "pdf";
                string mimeType;
                string encoding;
                string fileNameExte;

                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                    "<PageHeight>11in</PageHeight>" +
                   "<Margintop>0.1in</Margintop>" +
                     "<Marginleft>0.1in</Marginleft>" +
                      "<Marginright>0.1in</Marginright>" +
                       "<Marginbottom>0.5in</Marginbottom>" +
                       "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderByte;


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );



                MemoryStream memoryStream = new MemoryStream(renderByte);


                string savePath = Server.MapPath("~/PDF/" + Registration.FirstOrDefault().Pfcode + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }


                //using (MailMessage mm = new MailMessage("navlakheprajkta23@gmail.com", "navlakheprajkta23@gmail.com"))
                //{
                //    mm.Subject = "DTDC subscription invoice";

                //    string Bodytext = "<html><body>Please Find Attachment</body></html>";
                //    Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                //    mm.IsBodyHtml = true;



                //    mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                //    AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");

                //    mm.Body = Bodytext;



                //    mm.Attachments.Add(attachment);

                //    SmtpClient smtp = new SmtpClient();
                //    smtp.Host = "smtp.gmail.com";
                //    smtp.EnableSsl = true;
                //    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                //    credentials.UserName = "navlakheprajkta23@gmail.com";
                //    credentials.Password = "ShubhPraje20";
                //    smtp.UseDefaultCredentials = true;
                //    smtp.Credentials = credentials;
                //    smtp.Port = 587;
                //    //smtp.Send(mm);



                //TempData["msg"] = "subscription plan activated, Invoice has been send to your mail address..!!";

                string SubPath = "http://codetentacles-005-site1.htempurl.com/Admin/AdminLogin?isPaymentSuccess=1";
                return Redirect(SubPath);
                //return Json("Success");
                //return RedirectToAction("SubscriptionPanel");
            }
            return RedirectToAction("MakePayment");
            //return Json("");
        }

        public ActionResult Jschart(string pfcode)
        {
            DateTime today = DateTime.Now;
            DateTime sixMonthsBack = today.AddMonths(-1);
            Console.WriteLine(today.ToShortDateString());
            Console.WriteLine(sixMonthsBack.ToShortDateString());

            string Todayda = Convert.ToString(today.Date.ToString("MM-dd-yyyy"));
            string[] Todaydate = Todayda.Split('-');

            //string Todayda = Convert.ToString(today.Date.ToString("MM/dd/yyyy"));
            //string[] Todaydate = Todayda.Split('/');

            string TodayMonth = Todaydate[0];
            string TodayYear = Todaydate[2];

            string da = Convert.ToString(sixMonthsBack.Date.ToString("MM-dd-yyyy"));
            string[] SixMonthBackdate = da.Split('-');

            //string da = Convert.ToString(sixMonthsBack.Date.ToString("MM/dd/yyyy"));
            //string[] SixMonthBackdate = da.Split('/');

            string SixMonthBackMonth = SixMonthBackdate[0];
            string SixMonthBackYear = SixMonthBackdate[2];

            List<steplinechart> dataPoints = new List<steplinechart>();

            //var inv = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate= m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Take(12).ToList();

            if (pfcode == null || pfcode == "")
            {
                var inv1 = db.Invoices.Where(x=>x.isDelete==false).Select(m => new { m.netamount, m.invoicedate, day= SqlFunctions.DatePart("day", m.invoicedate), month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), day = SqlFunctions.DatePart("day", m.FirstOrDefault().invoicedate), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate = m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).Take(6).ToList();

                foreach (var i in inv1)
                {
                    //steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    steplinechart data = new steplinechart(i.netamount, i.month, i.Year,i.day);
                    dataPoints.Add(data);
                }

                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            }
            else
            {
                //var inv1 = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate = m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).ToList();

                var inv1 = (from inv in db.Invoices
                            join c in db.Companies on inv.Customer_Id equals c.Company_Id
                            where c.Pf_code == pfcode
                            && inv.isDelete== false
                            select (new { inv.netamount, inv.invoicedate, c.Pf_code,  month = SqlFunctions.DatePart("day", inv.invoicedate) +"-"+ SqlFunctions.DatePart("month", inv.invoicedate) + "-" + SqlFunctions.DatePart("year", inv.invoicedate) }))
                           .GroupBy(m => m.month).Select(m => new
                           {
                               netamount = m.Sum(c => c.netamount),
                               day = SqlFunctions.DatePart("day", m.FirstOrDefault().invoicedate),
                               month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate),
                               Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate),
                               invoicedate = m.FirstOrDefault().invoicedate
                           }).OrderBy(m => m.invoicedate)
                               .Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).Take(30).ToList();

                foreach (var i in inv1)
                {
                    //steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    steplinechart data = new steplinechart(i.netamount, i.month, i.Year,i.day);
                    dataPoints.Add(data);
                }

                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            }
            return View();
        }

        public ActionResult JschartforCashcounter(string pfcode)
        {
            DateTime today = DateTime.Now;
            DateTime sixMonthsBack = today.AddMonths(-1);
            Console.WriteLine(today.ToShortDateString());
            Console.WriteLine(sixMonthsBack.ToShortDateString());

            string Todayda = Convert.ToString(today.Date.ToString("MM-dd-yyyy"));
            string[] Todaydate = Todayda.Split('-');

            //string Todayda = Convert.ToString(today.Date.ToString("MM/dd/yyyy"));
            //string[] Todaydate = Todayda.Split('/');

            string TodayMonth = Todaydate[0];
            string TodayYear = Todaydate[2];

            string da = Convert.ToString(sixMonthsBack.Date.ToString("MM-dd-yyyy"));
            string[] SixMonthBackdate = da.Split('-');

            //string da = Convert.ToString(sixMonthsBack.Date.ToString("MM/dd/yyyy"));
            //string[] SixMonthBackdate = da.Split('/');

            string SixMonthBackMonth = SixMonthBackdate[0];
            string SixMonthBackYear = SixMonthBackdate[2];

            List<Cashcounterchart> dataPoints = new List<Cashcounterchart>();

            //var inv = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate= m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Take(12).ToList();

                //var inv1 = db.Invoices.Select(m => new { m.netamount, m.invoicedate, month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate = m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).ToList();

                var inv1 = (from inv in db.Receipt_details
                            join c in db.Companies on inv.Pf_Code equals c.Pf_code
                            where c.Pf_code == pfcode
                            
                            select (new { inv.Charges_Amount, inv.Datetime_Cons, c.Pf_code, month = SqlFunctions.DatePart("day", inv.Datetime_Cons) + "-" + SqlFunctions.DatePart("month", inv.Datetime_Cons) + "-" + SqlFunctions.DatePart("year", inv.Datetime_Cons) }))
                           .GroupBy(m => m.month).Select(m => new
                           {
                               netamount = m.Sum(c => c.Charges_Amount),
                               day= SqlFunctions.DatePart("day", m.FirstOrDefault().Datetime_Cons),
                               month = SqlFunctions.DatePart("month", m.FirstOrDefault().Datetime_Cons),
                               Year = SqlFunctions.DatePart("year", m.FirstOrDefault().Datetime_Cons),
                               invoicedate = m.FirstOrDefault().Datetime_Cons
                           }).OrderBy(m => m.invoicedate)
                               .Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).Take(6).ToList();

                foreach (var i in inv1)
                {
                    //steplinechart data = new steplinechart(i.netamount, i.month, i.Year);
                    Cashcounterchart data = new Cashcounterchart(i.netamount, i.month, i.Year,i.day);
                    dataPoints.Add(data);
                }

                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            return View();
        }


        public ActionResult Share()
        {
            return View();
        }
    }

}