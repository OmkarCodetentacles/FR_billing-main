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

namespace DtDc_Billing.Controllers
{
    [SessionAdminold]
   [SessionUserModule]
  
    public class HomeController : Controller
    {
        private db_a71c08_elitetokenEntities db = new db_a71c08_elitetokenEntities();
        //sLogger logger = LogManager.GetCurrentClassLogger();
        //[OutputCache(CacheProfile = "Cachefast")]   
        public ActionResult Index()
        {
            DateTime? dateTime;
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            //dashboardDataModel data = new dashboardDataModel();
            ViewBag.PfCode = PfCode;
            dateTime = DateTime.Now;
            ViewBag.date = String.Format("{0:dd/MM/yyyy}", dateTime);
            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

            var obj = db.dashboardData(localTime, PfCode).Select(x => new dashboardDataModel
            {
                expiredStationaryCount = x.expiredStationaryCount ?? 0,
                openConCount = x.openConCount ?? 0,
                unSignPincode = x.unSignPincode ?? 0,
                invalidCon = x.invalidCon ?? 0,
                complaintCount = x.complaintcount ?? 0,
                sumOfBilling = x.sumOfBilling ?? 0,
                countOfBilling = x.countOfBilling ?? 0,
                avgOfBillingSum = x.avgOfBillingSum ?? 0,
                sumOfBillingCurrentMonth = x.sumOfBillingCurrentMonth ?? 0,
                countofbillingcurrentmonth = x.countofbillingcurrentmonth ?? 0,
                todayExp = x.todayExp ?? 0,
                monthexp = x.monthexp ?? 0

            }).FirstOrDefault();
            
            //try
            //{
            //    int x = 0;
            //    int y = 5;
            //    int z = y / x;
            //}
            //catch (Exception ex)
            //{
            //    logger.ErrorException("Error occured in Home controller Index Action", ex);

            //}
            // ViewBag.stationary = db.ExpiredStationaries.Count();
            //ViewBag.openconcount = db.TransactionViews.Where(m => (m.Customer_Id == "" || m.Customer_Id == null) && (m.Pf_Code == "" || m.Pf_Code != null)).Count();
            //ViewBag.unsignpinc = (from user in db.Transactions
            //                      where !db.Destinations.Any(f => f.Pincode == user.Pincode)
            //                      select user.Pincode).Distinct().ToList().Count();
            //ViewBag.invalidcon = (from user in db.Transactions
            //                      where !db.Companies.Any(f => f.Company_Id == user.Customer_Id) && user.Customer_Id != null
            //                      select user).Count();



            //  double d = ((from od in db.TransactionViews
            //               where (od.booking_date.Value.Day == localTime.Day)
            //             && (od.booking_date.Value.Month == localTime.Month)
            //             && (od.booking_date.Value.Year == localTime.Year)
            //             && od.Customer_Id != null && (!od.Customer_Id.StartsWith("Cash")) && od.Customer_Id != "BASIC_TS" && od.Pf_Code != null
            //               select new { od.Amount, od.Risksurcharge, od.loadingcharge }).Sum(m => (m.Amount) + (m.Risksurcharge ?? 0) + (m.loadingcharge ?? 0))) ?? 0;

            ////  ViewBag.sumofbilling = d.ToString("##");
            //  ViewBag.sumofbilling = Math.Round(d);
            //  ViewBag.countofbilling = ((from od in db.TransactionViews
            //                             where (od.booking_date.Value.Day == localTime.Day)
            //                           && (od.booking_date.Value.Month == localTime.Month)
            //                           && (od.booking_date.Value.Year == localTime.Year)
            //                           && od.Customer_Id != null && (!od.Customer_Id.StartsWith("Cash")) && od.Customer_Id != "BASIC_TS"
            //                             select od.Amount).Count());


            //  double avgsum = db.TransactionViews.Select(m => new { m.Customer_Id, m.Amount, m.Risksurcharge, m.loadingcharge, m.booking_date, m.Pf_Code, month = SqlFunctions.DatePart("month", m.booking_date) + "-" + SqlFunctions.DatePart("year", m.booking_date) }).Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null).GroupBy(m => m.month).Average(m => m.Sum(x => (x.Amount + (x.loadingcharge ?? 0) + (x.Risksurcharge ?? 0)))) ?? 0;
            double ? data = db.TransactionViews.Select(m => new
            {
                Customer_Id = m.Customer_Id,
                Amount = m.Amount ?? 0,
                Pf_Code = m.Pf_Code,
                booking_date = m.booking_date,
                month = SqlFunctions.DatePart("month", m.booking_date) + "-" + SqlFunctions.DatePart("year", m.booking_date)
            }).Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null && m.Pf_Code == PfCode).GroupBy(m => m.month).Average(m => (double?)m.Count());

            //  ViewBag.avgofbillingsum = avgsum.ToString("##");
            if (data != null)
            {
                double avgofbillingcount = (double)data;
                ViewBag.avgofbillingcount = avgofbillingcount.ToString("##");
            }
            else
            {
                ViewBag.avgofbillingcount = 0;
            }
          
          //  double sumofbillingcurrentmonthd = db.TransactionViews.Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null && SqlFunctions.DatePart("month", m.booking_date) == DateTime.Now.Month && SqlFunctions.DatePart("year", m.booking_date) == DateTime.Now.Year).Sum(m => (m.Amount + (m.loadingcharge ?? 0) + (m.Risksurcharge ?? 0))) ?? 0;
          //  ViewBag.sumofbillingcurrentmonth = sumofbillingcurrentmonthd.ToString("##");

          //  ViewBag.countofbillingcurrentmonth = db.TransactionViews.Where(m => m.Customer_Id != null && (!m.Customer_Id.StartsWith("cash")) && m.Customer_Id != "BASIC_TS" && m.Pf_Code != null && SqlFunctions.DatePart("month", m.booking_date) == DateTime.Now.Month && SqlFunctions.DatePart("year", m.booking_date) == DateTime.Now.Year).Count();

          //  DateTime date = DateTime.Now;
            ViewBag.firstDayOfMonth = new DateTime(serverTime.Year, serverTime.Month, 1).ToString("dd-MM-yyyy");
            ViewBag.currentday = DateTime.Now.ToString("dd-MM-yyyy");

            //DateTime abc = DateTime.Now;

            //var todayexp =  ((from e in db.Expenses
            //                          where DbFunctions.TruncateTime(e.Datetime_Exp) >= DbFunctions.TruncateTime(abc)
            //                                  && DbFunctions.TruncateTime(e.Datetime_Exp) <= DbFunctions.TruncateTime(abc)
            //                          select e.Amount).Sum() ?? 0);

            //ViewBag.todayexp = todayexp;  


            //var monthexp = db.Expenses.Where(m => SqlFunctions.DatePart("month", m.Datetime_Exp) == DateTime.Now.Month && SqlFunctions.DatePart("year", m.Datetime_Exp) == DateTime.Now.Year).Sum(m=>m.Amount) ?? 0;
            //ViewBag.monthexp = monthexp;

            //string Pf = "abcd"; /*Session["PfID"].ToString();*/

            //int PfCount = db.Sectors.Where(m => m.Pf_code == Pf).Count();

            ViewBag.firstDayOfMonth = new DateTime(serverTime.Year, serverTime.Month, 1).ToString("dd-MM-yyyy");
            ViewBag.currentday = DateTime.Now.ToString("dd-MM-yyyy");
            ViewBag.complaintcount = obj.complaintCount;
            ViewBag.sumofbilling = obj.sumOfBilling;
            ViewBag.sumofbillingcurrentmonth = obj.sumOfBillingCurrentMonth.ToString("##");
            ViewBag.avgofbillingsum = obj.avgOfBillingSum.ToString("##");
            ViewBag.countofbilling = obj.countOfBilling;
            ViewBag.countofbillingcurrentmonth = obj.countofbillingcurrentmonth;
            ViewBag.todayexp = obj.todayExp;
            ViewBag.monthexp = obj.monthexp;

            ViewBag.openconcount = obj.openConCount;
            ViewBag.unsignpinc = obj.unSignPincode;
            ViewBag.invalidcon = obj.invalidCon;
            ViewBag.stationary = obj.expiredStationaryCount;

            //if (PfCount < 6)
            //{
            //    ViewBag.RedirectSector = true;
            //}

            // ViewBag.complaintcount = db.Complaints.Where(x => x.C_Status == "Not Resolved").Count();

            ViewBag.notification = db.Notifications.ToList().OrderByDescending(m => m.dateN).Take(4);


            obj.notificationsList = db.getNotification().Select(x => new Notification
            {
                N_ID = x.N_ID,
                Message = x.Message,
                description = x.description,
                dateN = x.dateN ?? serverTime,
                url_path = x.url_path,
                Status = x.Status

            }).Where(d => d.dateN >= DateTime.Now.Date).ToList();

            var datacount = db.getNotification().Where(d => d.dateN >= DateTime.Now.Date).Count();

            ViewBag.notificationCount = datacount;      
    
            DateTime After30days = serverTime.AddDays(30);

            //List<CompanyExpiryModel> CompanyExpiry = new List<CompanyExpiryModel>();

            var Date = (from d in db.registrations
                             where d.Pfcode == PfCode
                               select new
                             {
                                d.dateTime
                             }).FirstOrDefault();

            string strdate = Convert.ToString(Date.dateTime);

            string[] strarr = strdate.Split(' '); 
            string date = strarr[0];
            DateTime date1 = Convert.ToDateTime(date);
            DateTime After1Year=date1.AddYears(1);
            DateTime before30days = After1Year.AddDays(-30);
            DateTime NowDate = DateTime.Today;
            //DateTime NowDate1 = NowDate.AddDays(8);
            if (After1Year >= NowDate)
            {
                String diff2 = (After1Year - NowDate).TotalDays.ToString();
                ViewBag.ExpiryCompCount = diff2;
            }
            else
            {
                ViewBag.ExpiryCompCount ="";
            }
            ViewBag.After1Year = After1Year;
            ViewBag.NowDate = NowDate;
            ViewBag.before30days = before30days;
            //Backup();
            Session["EndDate"] = After1Year.ToString("dd/MM/yyyy");
            return View(obj);
        }


        public PartialViewResult DestinationAndProductPartial()
        {
            //string PfCode = Session["pfCode"].ToString();

            dashboardDataModel obj = new dashboardDataModel();

            obj.DestinationList = db.destinationCount(Request.Cookies["Cookies"]["AdminValue"].ToString()).Select(x => new DestinationModel
            {
                City = x.Destination,
                Count = x.DestCount ?? 0

            }).Take(20).ToList();

            ViewBag.DestinationCount = Convert.ToInt32(obj.DestinationList.Count());



            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            //List<ConsignmentCount> Consignmentcount = new List<ConsignmentCount>();

            ConsignmentCount consptp = new ConsignmentCount();

            consptp.Destination = "PTP";
            consptp.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("E") && m.Pf_Code == PfCode).Count();

            obj.Consignmentcount.Add(consptp);

            ConsignmentCount consPlus = new ConsignmentCount();

            consPlus.Destination = "Plus";
            consPlus.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("V") && m.Pf_Code == PfCode).Count();

            obj.Consignmentcount.Add(consPlus);


            ConsignmentCount consInternational = new ConsignmentCount();

            consInternational.Destination = "International";
            consInternational.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("N") && m.Pf_Code == PfCode).Count();

            obj.Consignmentcount.Add(consInternational);


            ConsignmentCount consDox = new ConsignmentCount();

            consDox.Destination = "Standard";
            consDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("P") && m.Pf_Code == PfCode).Count();

            obj.Consignmentcount.Add(consDox);


            ConsignmentCount consNonDox = new ConsignmentCount();

            consNonDox.Destination = "Non Dox";
            consNonDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("D") && m.Pf_Code == PfCode).Count();

            obj.Consignmentcount.Add(consNonDox);


            ConsignmentCount consNonVas = new ConsignmentCount();

            consNonVas.Destination = "VAS";
            consNonVas.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("I") && m.Pf_Code == PfCode).Count();

            obj.Consignmentcount.Add(consNonVas);

            ViewBag.Consignmentcount = obj.Consignmentcount.Count();

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
                //string conString = ConfigurationManager.ConnectionStrings["db_a71c08_elitetokenEntities"].ConnectionString;

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
        public ActionResult CreateCompanyPartial(Company company)
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
                var inv1 = db.Invoices.Select(m => new { m.netamount, m.invoicedate, day= SqlFunctions.DatePart("day", m.invoicedate), month = SqlFunctions.DatePart("month", m.invoicedate) + "-" + SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(m => m.month).Select(m => new { netamount = m.Sum(c => c.netamount), day = SqlFunctions.DatePart("day", m.FirstOrDefault().invoicedate), month = SqlFunctions.DatePart("month", m.FirstOrDefault().invoicedate), Year = SqlFunctions.DatePart("year", m.FirstOrDefault().invoicedate), invoicedate = m.FirstOrDefault().invoicedate }).OrderBy(m => m.invoicedate).Where(m => m.invoicedate >= sixMonthsBack && m.invoicedate <= today).Take(6).ToList();

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
    }
}