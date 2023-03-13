using DtDc_Billing.CustomModel;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace DtDc_Billing.Controllers
{

    //[SessionAdmin]
    [SessionUserModule]
   
    public class ReportsController : Controller
    {
        db_a71c08_elitetokenEntities db = new db_a71c08_elitetokenEntities();
        // GET: Reports

        [SessionAdmin]
        public ActionResult ReceiptReports()
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            List<Receipt_details> rd = db.Receipt_details.Where(m => m.Pf_Code == pfcode).OrderByDescending(m => m.Receipt_Id).ToList();
            ViewBag.totalAmt = (from emp in rd
                                select emp.Charges_Total).Sum();
            return View(rd);

        }

        [SessionAdmin]
        public ActionResult WalletReports()
        {
            return View(db.WalletPoints.OrderBy(m => m.Wallet_Id).ToList());
        }


        [SessionAdmin]
        public ActionResult billedunbilled()
        {
            
            List<Transaction> list = new List<Transaction>();
            //ViewBag.PfCode = new SelectList(db.Franchisees.Where(d=>d.PF_Code==strpfcode), "PF_Code", "PF_Code");
            var list1 = new List<SelectListItem>
             {
        new SelectListItem{ Text="Billed", Value = "Billed" , Selected = true},
        new SelectListItem{ Text="Unbilled", Value = "Unbilled" },
        new SelectListItem{ Text="Both", Value = "Both" },
              };


            ViewData["status"] = list1;
            return View(list);
        }

        [SessionAdmin]
        [HttpPost]
        public ActionResult billedunbilled(string Fromdatetime, string ToDatetime, string PfCode, string status, string Submit)
        {
            PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            //ViewBag.PfCode = new SelectList(db.Franchisees.Where(d=>d.PF_Code== PfCode), "PF_Code", "PF_Code", PfCode);

            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }

            List<Transaction> transactions;

            if (status == "Billed")
            {
                transactions =
                   db.Transactions.Where(m =>
                  (m.Customer_Id != null && m.Customer_Id != "") && (m.Pf_Code == PfCode)
                       ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date && m.status_t != "0" && m.status_t != null).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();
            }
            else if (status == "Unbilled")
            {
                transactions =
                  db.Transactions.Where(m =>
                 (m.Customer_Id != null && m.Customer_Id != "") && (m.Pf_Code == PfCode)
                      ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date && (m.status_t == "0" || m.status_t == null)).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();
            }
            else
            {
                transactions =
                 db.Transactions.Where(m =>
                (m.Customer_Id != null && m.Customer_Id != "") && (m.Pf_Code == PfCode)
                     ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

            }



            if (Submit == "Export to Excel")
            {
                if (status == "Billed")
                {

                    var import = db.Transactions.Where(m =>
                (m.Customer_Id != null && m.Customer_Id != "") && (m.Pf_Code == PfCode || PfCode == "")
                    ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date && m.status_t != "0" && m.status_t != null).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Pf_Code, x.Consignment_no, Weight = x.Actual_weight, x.Pincode, x.Amount, x.tembookingdate, x.Customer_Id }).ToList();
                    ExportToExcelAll.ExportToExcelAdmin(import);
                }

                else if (status == "Unbilled")
                {
                    var import = db.Transactions.Where(m =>
              (m.Customer_Id != null && m.Customer_Id != "") && (m.Pf_Code == PfCode || PfCode == "")
                  ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date && (m.status_t == "0" || m.status_t == null)).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Pf_Code, x.Consignment_no, Weight = x.Actual_weight, x.Pincode, x.Amount, x.tembookingdate, x.Customer_Id }).ToList();
                    ExportToExcelAll.ExportToExcelAdmin(import);
                }
                else
                {

                    var import = db.Transactions.Where(m =>
                (m.Customer_Id != null && m.Customer_Id != "") && (m.Pf_Code == PfCode || PfCode == "")
                    ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Pf_Code, x.Consignment_no, Weight = x.Actual_weight, x.Pincode, x.Amount, x.tembookingdate, x.Customer_Id }).ToList();
                    ExportToExcelAll.ExportToExcelAdmin(import);
                }

            }


            return View(transactions);
        }

        [SessionAdmin]
        public ActionResult SaleReports()
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var rc = db.getReceiptDetails(PfCode).Select(x => new Receipt_details
            {

                Consignment_No = x.Consignment_No,
                Destination = x.Destination,
                sender_phone = x.sender_phone,
                SenderCity = x.SenderCity,
                SenderPincode = x.SenderPincode,
                Reciepents_phone = x.Reciepents_phone,
                Reciepents = x.Reciepents,
                ReciepentsPincode = x.ReciepentsPincode,
                Pf_Code = x.Pf_Code,
                Datetime_Cons = x.Datetime_Cons,
                Charges_Total = x.Charges_Total,         
            }).ToList();

            // ViewBag.Employees = new SelectList(db.Users.Take(0), "Name", "Name");


            //List<Receipt_details> rc = new List<Receipt_details>();

            ViewBag.sum = (from emp in db.Receipt_details
                           where emp.Pf_Code== PfCode
                           select emp.Charges_Total).Sum();

            return View(rc);
        }

        [SessionAdmin]
        [HttpPost]
        public ActionResult SaleReports(string Employees, string ToDatetime, string Fromdatetime, string Submit)
        {


          string PfCode= Request.Cookies["Cookies"]["AdminValue"].ToString();

            if (Employees == null)
            {
                Employees = "";
            }

            List<Receipt_details> rc = new List<Receipt_details>();

            rc = db.Receipt_details.ToList();

            //ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);

            ViewBag.Employees = Employees;//new SelectList(db.Users, "Name", "Name", Employees);


            ViewBag.Fromdatetime = Fromdatetime;
            ViewBag.ToDatetime = ToDatetime;



            if (Fromdatetime == "")
            {
                if (Employees == "")
                {

                    rc = db.Receipt_details.ToList();
                }
                else if (Employees == "")
                {
                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode

                          select m).ToList();
                }
                else if (Employees != "")
                {
                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode


                          select m).ToList();
                }
                else
                {
                    var compdata = (from c in db.Companies
                                    where c.Company_Name == Employees
                                    select new { c.Company_Name }).FirstOrDefault();

                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode
                          && compdata.Company_Name == Employees

                          select m).ToList();
                }
            }
            else if (ToDatetime == "")
            {
                if (PfCode == "" && Employees == "")
                {

                    rc = db.Receipt_details.

                    ToList();
                }
                else if (PfCode != "" && Employees == "")
                {
                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode

                          select m).ToList();
                }
                else if (Employees != "" && PfCode == "")
                {
                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode


                          select m).ToList();
                }
                else
                {
                    var compdata = (from c in db.Companies
                                    where c.Company_Name == Employees
                                    select new { c.Company_Name }).FirstOrDefault();

                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode
                          && compdata.Company_Name == Employees

                          select m).ToList();
                }
            }
            else
            {


                ViewBag.selectedemp = Employees;


                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                DateTime fromdate = Convert.ToDateTime(bdatefrom);
                DateTime todate = Convert.ToDateTime(bdateto);


                if (Employees == "")
                {

                    rc = db.Receipt_details.Where(m => m.Datetime_Cons != null && DbFunctions.TruncateTime(m.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Cons) <= DbFunctions.TruncateTime(todate)).ToList();

                    //.ToList()
                    //.Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                    //.ToList();

                }

               
                else
                {
                    var compdata = (from c in db.Companies
                                   where c.Company_Name == Employees
                                   select new { c.Company_Name }).FirstOrDefault(); 

                    rc = (from m in db.Receipt_details

                          where m.Pf_Code == PfCode
                          && compdata.Company_Name == Employees
                          && m.Datetime_Cons != null
                          select m).ToList()
                           .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                              .ToList();
                }





                ViewBag.sum = (from emp in rc
                               where emp.Pf_Code == PfCode
                               select emp.Charges_Total).Sum();

            }

            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(rc);
            }
            return View(rc);
        }

        [SessionAdmin]
        public ActionResult UniqueReport()
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            List<Receipt_details> rc = new List<Receipt_details>();

            ViewBag.sum = (from emp in db.Receipt_details
                           where emp.Pf_Code == PfCode
                           select emp.Charges_Total).Sum();

            return View(rc);
        }

        [SessionAdmin]
        [HttpPost]
        public ActionResult UniqueReport(string PfCode, string ToDatetime, string Fromdatetime, string Submit)
        {
            List<Receipt_details> rc = new List<Receipt_details>();

            rc = db.Receipt_details.ToList();



            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);

            ViewBag.Fromdatetime = Fromdatetime;
            ViewBag.ToDatetime = ToDatetime;







            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);


            if (PfCode == "")
            {


                rc = db.Receipt_details.Where(m => m.Datetime_Cons != null)
                          .ToList()
                          .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                          .ToList();
            }
            else
            {
                rc = (from m in db.Receipt_details
                      where m.Pf_Code == PfCode && m.Datetime_Cons != null
                      select m).ToList()
                     .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                     .ToList();
            }






            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(rc);
            }


            return View(rc);
        }

        [SessionAdmin]
        public ActionResult GetUserList()
        {
            string Pfcode= Request.Cookies["Cookies"]["AdminValue"].ToString();
            db.Configuration.ProxyCreationEnabled = false;

            List<User> lstuser = new List<User>();

            lstuser = db.Users.Where(m => m.PF_Code == Pfcode).ToList();

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            string result = javaScriptSerializer.Serialize(lstuser);

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [SessionTimeout]
        public ActionResult DailyReport()
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

            List<Receipt_details> rc = db.Receipt_details.Where(m => m.Datetime_Cons.Value.Day == localTime.Day
            && m.Datetime_Cons.Value.Month == localTime.Month
            && m.Datetime_Cons.Value.Year == localTime.Year
            && m.Pf_Code == pfcode
            ).ToList();



            ViewBag.sum = (from emp in rc

                           select emp.Paid_Amount).Sum();


            ViewBag.Expense = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == localTime.Day
              && m.Datetime_Exp.Value.Month == localTime.Month
              && m.Datetime_Exp.Value.Year == localTime.Year
              && m.Pf_Code == pfcode
            ).ToList();


            ViewBag.expenseCount = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == localTime.Day
               && m.Datetime_Exp.Value.Month == localTime.Month
               && m.Datetime_Exp.Value.Year == localTime.Year
               && m.Pf_Code == pfcode
            ).Select(m => m.Amount).Sum();



            ViewBag.Payment = db.Payments.Where(m => m.Datetime_Pay.Value.Day == localTime.Day
            && m.Datetime_Pay.Value.Month == localTime.Month
            && m.Datetime_Pay.Value.Year == localTime.Year
            && m.Pf_Code == pfcode
          ).ToList();

            ViewBag.PaymentCount = db.Payments.Where(m => m.Datetime_Pay.Value.Day == localTime.Day
         && m.Datetime_Pay.Value.Month == localTime.Month
         && m.Datetime_Pay.Value.Year == localTime.Year
         && m.Pf_Code == pfcode
       ).Select(m => m.amount).Sum();




            ViewBag.Savings = db.Savings.Where(m => m.Datetime_Sav.Value.Day == localTime.Day
          && m.Datetime_Sav.Value.Month == localTime.Month
          && m.Datetime_Sav.Value.Year == localTime.Year
          && m.Pf_Code == pfcode
        ).ToList();


            ViewBag.Savingscount = db.Savings.Where(m => m.Datetime_Sav.Value.Day == localTime.Day
       && m.Datetime_Sav.Value.Month == localTime.Month
       && m.Datetime_Sav.Value.Year == localTime.Year
       && m.Pf_Code == pfcode
     ).Select(m => m.Saving_amount).Sum();





            return View(rc);

        }

        [HttpPost]
        public ActionResult DailyReport(string searcheddate, string Submit)
        {


            DateTime? dateTime;

            if (searcheddate == "")
            {
                dateTime = DateTime.Now;
                ViewBag.date = String.Format("{0:dd/MM/yyyy}", dateTime);
            }
            else
            {
                dateTime = Convert.ToDateTime(searcheddate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                ViewBag.date = searcheddate;
            }

            if (Submit == "Export to Excel")
            {
                ExportToExcel(dateTime);
            }

            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            List<Receipt_details> rc = db.Receipt_details.Where(m => m.Datetime_Cons.Value.Day == dateTime.Value.Day
            && m.Datetime_Cons.Value.Month == dateTime.Value.Month
            && m.Datetime_Cons.Value.Year == dateTime.Value.Year
            && m.Pf_Code == pfcode
            ).ToList();




            ViewBag.sum = (from emp in rc

                           select emp.Charges_Total).Sum();


            ViewBag.Expense = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == dateTime.Value.Day
             && m.Datetime_Exp.Value.Month == dateTime.Value.Month
             && m.Datetime_Exp.Value.Year == dateTime.Value.Year
             && m.Pf_Code == pfcode
           ).ToList();

            ViewBag.expenseCount = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == dateTime.Value.Day
            && m.Datetime_Exp.Value.Month == dateTime.Value.Month
            && m.Datetime_Exp.Value.Year == dateTime.Value.Year
            && m.Pf_Code == pfcode
         ).Select(m => m.Amount).Sum();



            ViewBag.Payment = db.Payments.Where(m => m.Datetime_Pay.Value.Day == dateTime.Value.Day
            && m.Datetime_Pay.Value.Month == dateTime.Value.Month
            && m.Datetime_Pay.Value.Year == dateTime.Value.Year
            && m.Pf_Code == pfcode
          ).ToList();

            ViewBag.PaymentCount = db.Payments.Where(m => m.Datetime_Pay.Value.Day == dateTime.Value.Day
     && m.Datetime_Pay.Value.Month == dateTime.Value.Month
     && m.Datetime_Pay.Value.Year == dateTime.Value.Year
     && m.Pf_Code == pfcode
   ).Select(m => m.amount).Sum();


            ViewBag.Savings = db.Savings.Where(m => m.Datetime_Sav.Value.Day == dateTime.Value.Day
          && m.Datetime_Sav.Value.Month == dateTime.Value.Month
          && m.Datetime_Sav.Value.Year == dateTime.Value.Year
          && m.Pf_Code == pfcode
        ).ToList();

            ViewBag.Savingscount = db.Savings.Where(m => m.Datetime_Sav.Value.Day == dateTime.Value.Day
   && m.Datetime_Sav.Value.Month == dateTime.Value.Month
   && m.Datetime_Sav.Value.Year == dateTime.Value.Year
   && m.Pf_Code == pfcode
 ).Select(m => m.Saving_amount).Sum();

            return View(rc);
        }


        public ActionResult AdminDailyReport(string searcheddate, string pfcode)
        {
            ViewBag.PfCode = pfcode;//new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);

            DateTime? dateTime = DateTime.Now;

            if (searcheddate == "")
            {
                dateTime = DateTime.Now;
                ViewBag.date = String.Format("{0:dd/MM/yyyy}", dateTime);
            }
            else
            {
                dateTime = Convert.ToDateTime(searcheddate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                ViewBag.date = searcheddate;
            }



            //string pfcode = Session["pfCode"].ToString();

            List<Receipt_details> rc = db.Receipt_details.Where(m => m.Datetime_Cons.Value.Day == dateTime.Value.Day
            && m.Datetime_Cons.Value.Month == dateTime.Value.Month
            && m.Datetime_Cons.Value.Year == dateTime.Value.Year
            && m.Pf_Code == pfcode
            ).ToList();



            var sum = (from emp in rc

                       select emp.Credit_Amount).Sum();

            ViewBag.bycard = (from card in rc
                              where card.Credit == "card"
                              select card.Credit_Amount).Sum();
            ViewBag.bycheque = (from cheque in rc
                                where cheque.Credit == "cheque"
                                select cheque.Credit_Amount).Sum();
            ViewBag.bycredit = (from credit in rc
                                where credit.Credit == "credit"
                                select credit.Credit_Amount).Sum();
            ViewBag.bycash = (from cash in rc
                              where cash.Credit == "cash"
                              select cash.Credit_Amount).Sum();
            ViewBag.byother = (from other in rc
                               where other.Credit == "other"
                               select other.Credit_Amount).Sum();


            ViewBag.Expense = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == dateTime.Value.Day
             && m.Datetime_Exp.Value.Month == dateTime.Value.Month
             && m.Datetime_Exp.Value.Year == dateTime.Value.Year
             && m.Pf_Code == pfcode
           ).ToList();

            ViewBag.expenseCount = (db.Expenses.Where(m => m.Datetime_Exp.Value.Day == dateTime.Value.Day
            && m.Datetime_Exp.Value.Month == dateTime.Value.Month
            && m.Datetime_Exp.Value.Year == dateTime.Value.Year
            && m.Pf_Code == pfcode
         ).Select(m => m.Amount).Sum() ?? 0);



            ViewBag.Payment = db.Payments.Where(m => m.Datetime_Pay.Value.Day == dateTime.Value.Day
            && m.Datetime_Pay.Value.Month == dateTime.Value.Month
            && m.Datetime_Pay.Value.Year == dateTime.Value.Year
            && m.Pf_Code == pfcode
          ).ToList();

            ViewBag.PaymentCount = (db.Payments.Where(m => m.Datetime_Pay.Value.Day == dateTime.Value.Day
     && m.Datetime_Pay.Value.Month == dateTime.Value.Month
     && m.Datetime_Pay.Value.Year == dateTime.Value.Year
     && m.Pf_Code == pfcode
   ).Select(m => m.amount).Sum() ?? 0);


            ViewBag.Savings = db.Savings.Where(m => m.Datetime_Sav.Value.Day == dateTime.Value.Day
          && m.Datetime_Sav.Value.Month == dateTime.Value.Month
          && m.Datetime_Sav.Value.Year == dateTime.Value.Year
          && m.Pf_Code == pfcode
        ).ToList();

            ViewBag.Savingscount = (db.Savings.Where(m => m.Datetime_Sav.Value.Day == dateTime.Value.Day
   && m.Datetime_Sav.Value.Month == dateTime.Value.Month
   && m.Datetime_Sav.Value.Year == dateTime.Value.Year
   && m.Pf_Code == pfcode
 ).Select(m => m.Saving_amount).Sum() ?? 0);

            if (ViewBag.expenseCount != null && ViewBag.PaymentCount != null)
            {
                ViewBag.sum = ((sum + ViewBag.PaymentCount) - (ViewBag.expenseCount));

            }

            if (ViewBag.expenseCount == null && ViewBag.PaymentCount == null)
            {
                ViewBag.sum = sum;
            }

            if (ViewBag.PaymentCount == null && ViewBag.expenseCount != null)
            {
                ViewBag.sum = (sum - ViewBag.expenseCount);
            }
            if (ViewBag.expenseCount == null && ViewBag.PaymentCount != null)
            {
                ViewBag.sum = (sum + ViewBag.PaymentCount);
            }

            return View(rc);

        }

        [SessionAdmin]
        [HttpPost]
        public ActionResult AdminDailyReport(string searcheddate, string pfcode, string Submit)
        {
            ViewBag.PfCode = pfcode;//new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);

            DateTime? dateTime;

            if (searcheddate == "")
            {
                dateTime = DateTime.Now;
                ViewBag.date = String.Format("{0:dd/MM/yyyy}", dateTime);
            }
            else
            {
                dateTime = Convert.ToDateTime(searcheddate, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                ViewBag.date = searcheddate;
            }

            if (Submit == "Export to Excel")
            {
                ExportToExcel(dateTime);
            }

            //string pfcode = Session["pfCode"].ToString();

            List<Receipt_details> rc = db.Receipt_details.Where(m => m.Datetime_Cons.Value.Day == dateTime.Value.Day
            && m.Datetime_Cons.Value.Month == dateTime.Value.Month
            && m.Datetime_Cons.Value.Year == dateTime.Value.Year
            && m.Pf_Code == pfcode
            ).ToList();




            var sum = (from emp in rc

                       select emp.Credit_Amount).Sum();

            ViewBag.bycard = (from card in rc
                              where card.Credit == "card"
                              select card.Credit_Amount).Sum();
            ViewBag.bycheque = (from cheque in rc
                                where cheque.Credit == "cheque"
                                select cheque.Credit_Amount).Sum();
            ViewBag.bycredit = (from credit in rc
                                where credit.Credit == "credit"
                                select credit.Credit_Amount).Sum();
            ViewBag.bycash = (from cash in rc
                              where cash.Credit == "cash"
                              select cash.Credit_Amount).Sum();
            ViewBag.byother = (from other in rc
                               where other.Credit == "other"
                               select other.Credit_Amount).Sum();

            ViewBag.Expense = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == dateTime.Value.Day
             && m.Datetime_Exp.Value.Month == dateTime.Value.Month
             && m.Datetime_Exp.Value.Year == dateTime.Value.Year
             && m.Pf_Code == pfcode
           ).ToList();

            ViewBag.expenseCount = (db.Expenses.Where(m => m.Datetime_Exp.Value.Day == dateTime.Value.Day
            && m.Datetime_Exp.Value.Month == dateTime.Value.Month
            && m.Datetime_Exp.Value.Year == dateTime.Value.Year
            && m.Pf_Code == pfcode
         ).Select(m => m.Amount).Sum() ?? 0);



            ViewBag.Payment = db.Payments.Where(m => m.Datetime_Pay.Value.Day == dateTime.Value.Day
            && m.Datetime_Pay.Value.Month == dateTime.Value.Month
            && m.Datetime_Pay.Value.Year == dateTime.Value.Year
            && m.Pf_Code == pfcode
          ).ToList();

            ViewBag.PaymentCount = (db.Payments.Where(m => m.Datetime_Pay.Value.Day == dateTime.Value.Day
     && m.Datetime_Pay.Value.Month == dateTime.Value.Month
     && m.Datetime_Pay.Value.Year == dateTime.Value.Year
     && m.Pf_Code == pfcode
   ).Select(m => m.amount).Sum() ?? 0);


            ViewBag.Savings = db.Savings.Where(m => m.Datetime_Sav.Value.Day == dateTime.Value.Day
          && m.Datetime_Sav.Value.Month == dateTime.Value.Month
          && m.Datetime_Sav.Value.Year == dateTime.Value.Year
          && m.Pf_Code == pfcode
        ).ToList();

            ViewBag.Savingscount = (db.Savings.Where(m => m.Datetime_Sav.Value.Day == dateTime.Value.Day
   && m.Datetime_Sav.Value.Month == dateTime.Value.Month
   && m.Datetime_Sav.Value.Year == dateTime.Value.Year
   && m.Pf_Code == pfcode
 ).Select(m => m.Saving_amount).Sum() ?? 0);

            if (ViewBag.expenseCount != null && ViewBag.PaymentCount != null)
            {
                ViewBag.sum = ((sum + ViewBag.PaymentCount) - (ViewBag.expenseCount));

            }

            if (ViewBag.expenseCount == null && ViewBag.PaymentCount == null)
            {
                ViewBag.sum = sum;
            }

            if (ViewBag.PaymentCount == null && ViewBag.expenseCount != null)
            {
                ViewBag.sum = (sum - ViewBag.expenseCount);
            }
            if (ViewBag.expenseCount == null && ViewBag.PaymentCount != null)
            {
                ViewBag.sum = (sum + ViewBag.PaymentCount);
            }

            return View(rc);
        }

        [SessionAdmin]
        public ActionResult BulkBooking()
        {

            return View();
        }

        [SessionAdmin]
        public ActionResult PfReport()
        {
            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            return View(Pfsum);
        }




        [SessionAdmin]
        [HttpPost]
        public ActionResult PfReport(string ToDatetime, string Fromdatetime)
        {

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();


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


                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                DateTime fromdate = Convert.ToDateTime(bdatefrom);
                DateTime todate = Convert.ToDateTime(bdateto);





                //Pfsum =(from student in db.Franchisees                        
                //       group student by student.PF_Code into studentGroup
                //       select new DisplayPFSum
                //       {
                //          PfCode = studentGroup.Key,
                //          Sum =
                //               ((from od in db.Receipt_details
                //                where od.Pf_Code == studentGroup.Key && od.Datetime_Cons != null
                //                &&  (od.Datetime_Cons >= fromdate && od.Datetime_Cons <= todate)
                //                 select od.Charges_Total).Sum()) ?? 0
                //                                         }).ToList();


                Pfsum = (from student in db.Franchisees
                         group student by student.PF_Code into studentGroup
                         select new DisplayPFSum
                         {
                             PfCode = studentGroup.Key,
                             Sum =
                                 (from od in db.Receipt_details
                                  where od.Pf_Code == studentGroup.Key && od.Datetime_Cons != null
                                  select od).ToList()
                                 .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                                 .Sum(m => m.Charges_Total) ?? 0


                         }).ToList();
            }

            return View(Pfsum);
        }

        [SessionAdmin]
        [HttpGet]
        public ActionResult PfReportDaily(string Fromdatetime = null, string ToDatetime = null)
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();
            DateTime? fromdate = null;
            DateTime? todate = null;
            if (Fromdatetime != null && ToDatetime != null)
            {
                ViewBag.fromdate = Fromdatetime;
                ViewBag.todate = ToDatetime;
            }
            else
            {
                ViewBag.todaydate = GetLocalTime.GetDateTime();
                DateTime? EnteredDate;
                EnteredDate = DateTime.Now;
                Fromdatetime = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
                ToDatetime = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
            }






            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;




            Pfsum = (from student in db.Franchisees
                     group student by student.PF_Code into studentGroup

                     select new DisplayPFSum
                     {

                         PfCode = studentGroup.Key,
                         Sum =
                             ((from od in db.Receipt_details
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)

                               select od.Charges_Total).Sum()) ?? 0,
                         Count =
                             ((from od in db.Receipt_details
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)

                               select od.Charges_Total).Count()),
                         Branchname = (from od in db.Franchisees
                                       where od.PF_Code == studentGroup.Key

                                       select od.BranchName).FirstOrDefault()
                     }).Where(d=>d.PfCode==PfCode).ToList();
            return View(Pfsum);
        }


        [HttpPost]
        public ActionResult PfReportDaily(string Fromdatetime, string ToDatetime, string Submit)
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            DateTime? fromdate = null;
            DateTime? todate = null;



            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;




            Pfsum = (from student in db.Franchisees
                     group student by student.PF_Code into studentGroup

                     select new DisplayPFSum
                     {

                         PfCode = studentGroup.Key,
                         Sum =
                             ((from od in db.Receipt_details
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)

                               select od.Charges_Total).Sum()) ?? 0,
                         Branchname = (from od in db.Franchisees
                                       where od.PF_Code == studentGroup.Key

                                       select od.BranchName).FirstOrDefault()
                     }).Where(d=>d.PfCode==PfCode).ToList();

            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(Pfsum.Select(x => new { PFCode = x.PfCode, x.Branchname, x.Sum }));
            }


            return View(Pfsum);
        }


        [HttpGet]
        public ActionResult Todaysale(string Fromdatetime = null, string ToDatetime = null)
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();
            DateTime? fromdate = null;
            DateTime? todate = null;
            if (Fromdatetime != null && ToDatetime != null)
            {
                ViewBag.fromdate = Fromdatetime;
                ViewBag.todate = ToDatetime;
            }
            else
            {
                ViewBag.todaydate = GetLocalTime.GetDateTime();
                DateTime? EnteredDate;
                EnteredDate = DateTime.Now;
                Fromdatetime = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
                ToDatetime = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
            }






            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;




            Pfsum = (from student in db.Franchisees
                     group student by student.PF_Code into studentGroup
     
                     select new DisplayPFSum
                     {

                         PfCode = studentGroup.Key,
                         Sum =
                             ((from od in db.TransactionViews
                               where od.Pf_Code == studentGroup.Key
                               && od.booking_date >= fromdate
                               && od.booking_date <= todate
                               && od.Customer_Id != null
                               && (!od.Customer_Id.StartsWith("Cash"))
                               && od.Customer_Id != "BASIC_TS"
                               && od.Pf_Code != null
                               select new { od.Amount, od.Risksurcharge, od.loadingcharge }).Sum(m => m.Amount + (m.Risksurcharge ?? 0) + (m.loadingcharge ?? 0))) ?? 0,
                         Count =
                             ((from od in db.TransactionViews
                               where od.Pf_Code == studentGroup.Key
                               && od.booking_date >= fromdate
                               && od.booking_date <= todate
                                && od.Customer_Id != null
                                && (!od.Customer_Id.StartsWith("Cash"))
                                && od.Customer_Id != "BASIC_TS"
                                && od.Pf_Code != null
                               select od.Amount).Count()),
                         Branchname = (from od in db.Franchisees
                                       where od.PF_Code == PfCode

                                       select od.Franchisee_Name).FirstOrDefault()
                     }).Where(d=>d.PfCode== PfCode).ToList();
            return View(Pfsum);
        }


        [HttpPost]
        public ActionResult Todaysale(string Fromdatetime, string ToDatetime, string Submit)
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            DateTime? fromdate = null;
            DateTime? todate = null;



            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;




            Pfsum = (from student in db.Franchisees
                     group student by student.PF_Code into studentGroup

                     select new DisplayPFSum
                     {

                         PfCode = studentGroup.Key,
                         Sum =
                             ((from od in db.TransactionViews
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.booking_date) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.booking_date) <= DbFunctions.TruncateTime(todate)
                               && od.Customer_Id != null
                               && (!od.Customer_Id.StartsWith("Cash"))
                               && od.Customer_Id != "BASIC_TS"
                               && od.Pf_Code != null
                               select new { od.Amount, od.Risksurcharge, od.loadingcharge }).Sum(m => m.Amount + (m.Risksurcharge ?? 0) + (m.loadingcharge ?? 0))) ?? 0,
                         Count =
                             ((from od in db.TransactionViews
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.booking_date) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.booking_date) <= DbFunctions.TruncateTime(todate)
                               && od.Customer_Id != null
                               && (!od.Customer_Id.StartsWith("Cash"))
                               && od.Customer_Id != "BASIC_TS"
                               && od.Pf_Code != null
                               select od.Amount).Count()),
                         Branchname = (from od in db.Franchisees
                                       where od.PF_Code == PfCode

                                       select od.BranchName).FirstOrDefault()
                     }).Where(d=>d.PfCode== PfCode).ToList();

            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(Pfsum.Select(x => new { PFCode = x.PfCode, x.Branchname, x.Sum }));
            }


            return View(Pfsum);
        }



        [HttpGet]
        public ActionResult Cashtotalsale(string Fromdatetime = null, string ToDatetime = null)
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();
            DateTime? fromdate = null;
            DateTime? todate = null;
            if (Fromdatetime != null && ToDatetime != null)
            {
                ViewBag.fromdate = Fromdatetime;
                ViewBag.todate = ToDatetime;
            }
            else
            {
                ViewBag.todaydate = GetLocalTime.GetDateTime();
                DateTime? EnteredDate;
                EnteredDate = DateTime.Now;
                Fromdatetime = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
                ToDatetime = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
            }






            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;




            Pfsum = (from student in db.Franchisees
                     group student by student.PF_Code into studentGroup

                     select new DisplayPFSum
                     {

                         PfCode = studentGroup.Key,

                         Sumcash =
                             ((from od in db.Receipt_details
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)
                               && od.Pf_Code != null
                               select od.Charges_Total).Sum()) ?? 0,
                         Countcash =
                             ((from od in db.Receipt_details
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)
                               && od.Pf_Code != null
                               select od.Charges_Total).Count()),

                         Sum =
                             ((from od in db.TransactionViews
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.booking_date) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.booking_date) <= DbFunctions.TruncateTime(todate)
                               && od.Customer_Id != null
                               && (!od.Customer_Id.StartsWith("Cash"))
                               && od.Customer_Id != "BASIC_TS"
                               && od.Pf_Code != null
                               select new { od.Amount, od.Risksurcharge, od.loadingcharge }).Sum(m => m.Amount + (m.Risksurcharge ?? 0) + (m.loadingcharge ?? 0))) ?? 0,
                         Count =
                             ((from od in db.TransactionViews
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.booking_date) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.booking_date) <= DbFunctions.TruncateTime(todate)
                               && od.Customer_Id != null
                               && (!od.Customer_Id.StartsWith("Cash"))
                               && od.Customer_Id != "BASIC_TS"
                               && od.Pf_Code != null
                               select od.Amount).Count()),
                         Branchname = (from od in db.Franchisees
                                       where od.PF_Code == studentGroup.Key

                                       select od.BranchName).FirstOrDefault()
                     }).Where(d=>d.PfCode== PfCode).ToList();
            return View(Pfsum);
        }


        [HttpPost]
        public ActionResult Cashtotalsale(string Fromdatetime, string ToDatetime, string Submit)
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            DateTime? fromdate = null;
            DateTime? todate = null;



            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;




            Pfsum = (from student in db.Franchisees
                     group student by student.PF_Code into studentGroup

                     select new DisplayPFSum
                     {

                         PfCode = studentGroup.Key,
                         Sumcash =
                             ((from od in db.Receipt_details
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)
                               && od.Pf_Code != null
                               select od.Charges_Total).Sum()) ?? 0,
                         Countcash =
                             ((from od in db.Receipt_details
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)
                               && od.Pf_Code != null
                               select od.Charges_Total).Count()),
                         Sum =
                             ((from od in db.TransactionViews
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.booking_date) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.booking_date) <= DbFunctions.TruncateTime(todate)
                               && od.Customer_Id != null
                               && (!od.Customer_Id.StartsWith("Cash"))
                               && od.Customer_Id != "BASIC_TS"
                               && od.Pf_Code != null
                               select new { od.Amount, od.Risksurcharge, od.loadingcharge }).Sum(m => m.Amount + (m.Risksurcharge ?? 0) + (m.loadingcharge ?? 0))) ?? 0,
                         Count =
                             ((from od in db.TransactionViews
                               where od.Pf_Code == studentGroup.Key
                               && DbFunctions.TruncateTime(od.booking_date) >= DbFunctions.TruncateTime(fromdate)
                               && DbFunctions.TruncateTime(od.booking_date) <= DbFunctions.TruncateTime(todate)
                               && od.Customer_Id != null
                               && (!od.Customer_Id.StartsWith("Cash"))
                               && od.Customer_Id != "BASIC_TS"
                               && od.Pf_Code != null
                               select od.Amount).Count()),
                         Branchname = (from od in db.Franchisees
                                       where od.PF_Code == studentGroup.Key

                                       select od.BranchName).FirstOrDefault()
                     }).Where(d=>d.PfCode==PfCode).ToList();

            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(Pfsum.Select(x => new { PFCode = x.PfCode, x.Branchname, x.Sum }));
            }


            return View(Pfsum);
        }




        public void ExportToExcel(DateTime? dateTime)
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var consignments = (from m in db.Receipt_details
                                where m.Pf_Code == pfcode
                               && m.Datetime_Cons.Value.Day == dateTime.Value.Day
             && m.Datetime_Cons.Value.Month == dateTime.Value.Month
             && m.Datetime_Cons.Value.Year == dateTime.Value.Year
                                select m).ToList();


            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Consignment No\",\"Service Type\",\"Shipment Type\",\"Insuance Amount\",\"Risk Surcharge\",\"Weight\",\"Length\",\"Width\",\"Height\",\"Sender Pincode\",\"Sender Name\",\"Sender Phone\",\"Sender Address Line 1\",\"Sender Address Line 2\",\"Sender City\",\"Sender State\",\"Receiver Pincode\",\"Receiver name\",\"Receiver Phone\",\"Receiver Address Line 1\",\"Receiver Address Line 2\",\"Receiver City\",\"Receiver State\"");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Exported_Consignments.csv");
            Response.ContentType = "text/csv";

            string Shipmenttype = "";
            string Servicetype = "";
            foreach (var e in consignments)
            {
                if (e.Shipmenttype == "N")
                {
                    Shipmenttype = "NON-DOCUMENT";
                }
                else
                {
                    Shipmenttype = "DOCUMENT";
                }
                if (e.Consignment_No.StartsWith("P") || e.Consignment_No.StartsWith("N"))
                {
                    Servicetype = "STANDARD";
                }
                else if (e.Consignment_No.StartsWith("V") || e.Consignment_No.StartsWith("I"))
                {
                    Servicetype = "PREMIUM";
                }
                else if (e.Consignment_No.StartsWith("E"))
                {
                    Servicetype = "PRIME TIME PLUS";
                }
                else if (e.Consignment_No.StartsWith("G"))
                {
                    Servicetype = "GROUND";
                }
                else if (e.Consignment_No.StartsWith("D"))
                {
                    Servicetype = "STANDARD";
                }



                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\",\"{21}\",\"{22}\"",

                                           e.Consignment_No,
                                           Servicetype,
                                           Shipmenttype,
                                           e.Total_Amount,

                                           e.Insurance,
                                           e.Actual_Weight,
                                           e.Shipment_Length,
                                           e.Shipment_Breadth,
                                           e.Shipment_Heigth,
                                           e.SenderPincode,
                                           e.Sender,
                                           e.sender_phone,
                                           e.SenderAddress,
                                           "", //SenderAddress2
                                           e.SenderCity,
                                           e.SenderState,
                                           e.ReciepentsPincode,
                                          e.Reciepents,
                                           e.Reciepents_phone,
                                          e.ReciepentsAddress,
                                           "",//ReciepentsAddress2 =
                                           e.ReciepentsCity,
                                           e.ReciepentsState




                                           ));
            }

            Response.Write(sw.ToString());

            Response.End();


        }

        public void ExportToExcelAdmin(List<Receipt_details> rc)
        {
            //string pfcode = Session["pfCode"].ToString();

            var cons = rc;

            var gv = new GridView();
            gv.DataSource = cons;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ConsignmentExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

        }




        public ActionResult Chart()
        {
            var cons = (from p in db.Franchisees
                        join c in db.Receipt_details on p.PF_Code equals c.Pf_Code into j1
                        from j2 in j1

                        group j2 by p.PF_Code into grouped
                        select new DisplayPFSum { PfCode = grouped.Key, Sum = grouped.Sum(t => t.Charges_Amount) }).ToList();

            List<ChartPfDatapoints> dataPoints = new List<ChartPfDatapoints>();

            foreach (var i in cons)
            {
                ChartPfDatapoints data = new ChartPfDatapoints(i.PfCode, i.Sum);
                dataPoints.Add(data);
            }



            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View();
        }

        [HttpPost]
        public ActionResult Chart(string ToDatetime, string Fromdatetime)
        {
            List<ChartPfDatapoints> dataPoints = new List<ChartPfDatapoints>();

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


                var cons = (from p in db.Franchisees
                            join c in db.Receipt_details on p.PF_Code equals c.Pf_Code into j1

                            from j2 in j1
                            where j2.Datetime_Cons.Value.Day >= fromdate.Value.Day
                                     && j2.Datetime_Cons.Value.Year >= fromdate.Value.Year
                                     && j2.Datetime_Cons.Value.Month >= fromdate.Value.Month

                                   && j2.Datetime_Cons.Value.Day <= todate.Value.Day
                                   && j2.Datetime_Cons.Value.Month <= todate.Value.Month
                                   && j2.Datetime_Cons.Value.Year <= todate.Value.Year
                            group j2 by p.PF_Code into grouped
                            select new DisplayPFSum { PfCode = grouped.Key, Sum = grouped.Sum(t => t.Charges_Amount) }).ToList();



                foreach (var i in cons)
                {
                    ChartPfDatapoints data = new ChartPfDatapoints(i.PfCode, i.Sum);
                    dataPoints.Add(data);
                }

            }



            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View();
        }


        public ActionResult DayWiseCharts()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");

            ViewBag.Months = new SelectList(Enumerable.Range(1, 12).Select(x =>
        new SelectListItem()
        {
            Text = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[x - 1] + " (" + x + ")",
            Value = x.ToString()
        }), "Value", "Text");



            ViewBag.Years = new SelectList(Enumerable.Range(DateTime.Today.Year, 20).Select(x =>

               new SelectListItem()
               {
                   Text = x.ToString(),
                   Value = x.ToString()
               }), "Value", "Text");




            return View();
        }




        public ActionResult PercantagePIChart()
        {
            var cons = (from p in db.Franchisees
                        join c in db.Receipt_details on p.PF_Code equals c.Pf_Code into j1
                        from j2 in j1

                        group j2 by p.PF_Code into grouped
                        select new DisplayPFSum { PfCode = grouped.Key, Sum = grouped.Sum(t => t.Charges_Amount) }).ToList();

            List<ChartPfDatapoints> dataPoints = new List<ChartPfDatapoints>();

            var amtsum = cons.Sum(m => m.Sum);

            foreach (var i in cons)
            {
                double? percentage = (100 / amtsum) * i.Sum;

                ChartPfDatapoints data = new ChartPfDatapoints(i.PfCode, System.Math.Round(Convert.ToDouble(percentage), 2));
                dataPoints.Add(data);
            }



            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            ViewBag.totalSaleAmount = amtsum;

            return View();

        }

        [HttpPost]
        public ActionResult PercantagePIChart(string ToDatetime, string Fromdatetime)
        {
            List<ChartPfDatapoints> dataPoints = new List<ChartPfDatapoints>();

            ViewBag.totalSaleAmount = 0;

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


                var cons = (from p in db.Franchisees
                            join c in db.Receipt_details on p.PF_Code equals c.Pf_Code into j1

                            from j2 in j1
                            where j2.Datetime_Cons.Value.Day >= fromdate.Value.Day
                                     && j2.Datetime_Cons.Value.Year >= fromdate.Value.Year
                                     && j2.Datetime_Cons.Value.Month >= fromdate.Value.Month

                                   && j2.Datetime_Cons.Value.Day <= todate.Value.Day
                                   && j2.Datetime_Cons.Value.Month <= todate.Value.Month
                                   && j2.Datetime_Cons.Value.Year <= todate.Value.Year
                            group j2 by p.PF_Code into grouped
                            select new DisplayPFSum { PfCode = grouped.Key, Sum = grouped.Sum(t => t.Charges_Amount) }).ToList();


                var amtsum = cons.Sum(m => m.Sum);

                foreach (var i in cons)
                {
                    double? percentage = (100 / amtsum) * i.Sum;



                    ChartPfDatapoints data = new ChartPfDatapoints(i.PfCode, System.Math.Round(Convert.ToDouble(percentage), 2));
                    dataPoints.Add(data);
                }

                ViewBag.totalSaleAmount = amtsum;

            }



            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);



            return View();
        }

        public ContentResult JSON(string Fromdatetime, string ToDatetime, string pfCode)
        {




            //     ViewBag.Months = new SelectList(Enumerable.Range(1, 12).Select(x =>
            //new SelectListItem()
            //{
            //    Text = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[x - 1] + " (" + x + ")",
            //    Value = x.ToString()
            //}), "Value", "Text", Months);



            //     ViewBag.Years = new SelectList(Enumerable.Range(DateTime.Today.Year, 20).Select(x =>

            //        new SelectListItem()
            //        {
            //            Text = x.ToString(),
            //            Value = x.ToString()
            //        }), "Value", "Text", Years);

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfCode);


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "" && Fromdatetime != null)
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now.AddYears(-10);
            }

            if (ToDatetime != "" && ToDatetime != null)
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now.AddYears(-10);
            }



            List<ChartPFDay> dataPoints = new List<ChartPFDay>();




            //dataPoints.Add(new ChartPFDay(1513449000000, 4.3));
            //dataPoints.Add(new ChartPFDay(1513621800000, 4.36));

            var cons = (from p in db.Franchisees
                        join c in db.Receipt_details on p.PF_Code equals c.Pf_Code into j1

                        from j2 in j1
                        where (DbFunctions.TruncateTime(j2.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(j2.Datetime_Cons) <= DbFunctions.TruncateTime(todate))
                          && (j2.Pf_Code == pfCode || pfCode == "")
                        let dt = j2.Datetime_Cons
                        group j2 by new { y = dt.Value.Year, m = dt.Value.Month, d = dt.Value.Day } into grouped
                        select new { PfCode = grouped.Key, Sum = grouped.Sum(t => t.Charges_Amount) }).ToList();

            foreach (var i in cons)
            {
                // DateTime value = new DateTime(i.PfCode.y, i.PfCode.m, i.PfCode.d);

                var baseDate = new DateTime(1970, 01, 01);
                var toDate = new DateTime(i.PfCode.y, i.PfCode.m, i.PfCode.d);
                var numberOfSeconds = toDate.Subtract(baseDate).TotalSeconds;


                ChartPFDay data = new ChartPFDay(numberOfSeconds, i.Sum);
                dataPoints.Add(data);
            }


            JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            return Content(JsonConvert.SerializeObject(dataPoints, _jsonSetting), "application/json");
        }



        [HttpPost]
        public ActionResult WalletReportsAdmin(string demo)
        {

            List<WalletPointModel> list =
       (from student in db.WalletPoints
        select new
        {
            MobileNo = student.MobileNo,
            Wallet_Money = student.Wallet_Money,
            Datetime_Wa = student.Datetime_Wa,
            Redeemed = (from od in db.Receipt_details
                        where od.sender_phone == student.MobileNo
                        select od.Discount).Sum(),

            Name = (from od in db.Receipt_details
                    where od.sender_phone == student.MobileNo
                    select od.Sender).FirstOrDefault(),

            PFCode = (from od in db.Receipt_details
                      where od.sender_phone == student.MobileNo
                      select od.Pf_Code).FirstOrDefault(),

        }).AsEnumerable().Select(x => new WalletPointModel
        {
            MobileNo = x.MobileNo,
            Wallet_Money = x.Wallet_Money,
            Datetime_Wa = x.Datetime_Wa,
            Redeemed = x.Redeemed ?? 0,
            Name = x.Name,
            PFCode = x.PFCode
        }).ToList();



            ExportToExcelWallet(list);
            return View(list);
        }

        [HttpGet]
        public ActionResult WalletReportsAdmin()
        {

            List<WalletPointModel> list =
       (from student in db.WalletPoints
        select new
        {
            MobileNo = student.MobileNo,
            Wallet_Money = student.Wallet_Money,
            Datetime_Wa = student.Datetime_Wa,
            Redeemed = (from od in db.Receipt_details
                        where od.sender_phone == student.MobileNo
                        select od.Discount).Sum(),

            Name = (from od in db.Receipt_details
                    where od.sender_phone == student.MobileNo
                    select od.Sender).FirstOrDefault(),

            PFCode = (from od in db.Receipt_details
                      where od.sender_phone == student.MobileNo
                      select od.Pf_Code).FirstOrDefault(),

        }).AsEnumerable().Select(x => new WalletPointModel
        {
            MobileNo = x.MobileNo,
            Wallet_Money = x.Wallet_Money,
            Datetime_Wa = x.Datetime_Wa,
            Redeemed = x.Redeemed ?? 0,
            Name = x.Name,
            PFCode = x.PFCode
        }).ToList();




            return View(list);
        }


        public void ExportToExcelWallet(List<WalletPointModel> rc)
        {
            //string pfcode = Session["pfCode"].ToString();

            var cons = rc;

            var gv = new GridView();
            gv.DataSource = cons;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Wallet.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

        }



        [HttpGet]
        public ActionResult PfWiseReport()
        {


            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");


            List<TransactionView> list = new List<TransactionView>();

            return View(list);
        }

        [HttpPost]
        public ActionResult PfWiseReport(string PfCode, string Fromdatetime, string ToDatetime)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }





            List<TransactionView> transactions =
                db.TransactionViews.Where(m =>
               (m.Pf_Code == PfCode)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date)
                           .ToList();





            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            return View(transactions);



        }

        public ActionResult Topayreport()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            // List<TransactionView> transactions = db.TransactionViews.Where(m => m.topay == "yes").ToList();
            var obj = (from t in db.TransactionViews
                       join ad in db.addtopayamounts on t.Consignment_no equals ad.consinmentno into adt
                       from ad in adt.DefaultIfEmpty()
                       where t.topay=="yes"
                       select new
                       {
                           Customer_Id = t.Customer_Id,
                           Consignment_no = t.Consignment_no,
                           booking_date = t.booking_date,
                           Pincode = t.Pincode,
                           Name = t.Name,
                           TopayAmount = t.TopayAmount,
                           Topaycharges = t.Topaycharges,
                           status_t = ad != null ? "Paid" : "unpaid"
                                                 }).AsEnumerable().Select(x=> new TransactionView {
                                                     Customer_Id = x.Customer_Id,
                                                     Consignment_no = x.Consignment_no,
                                                     booking_date = x.booking_date,
                                                     Pincode = x.Pincode,
                                                     Name = x.Name,
                                                     TopayAmount = x.TopayAmount,
                                                     Topaycharges = x.Topaycharges,
                                                      status_t =x.status_t
                                                 }).ToList();


            return View(obj);
        }

        [HttpPost]
        public ActionResult Topayreport(string PfCode, string ToDatetime, string Fromdatetime)
        {
            //  List<TransactionView> transactions = new List<TransactionView>();

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);
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


                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                DateTime fromdate = Convert.ToDateTime(bdatefrom);
                DateTime todate = Convert.ToDateTime(bdateto);

                //transactions = (from m in db.TransactionViews
                //                where m.topay == "yes"
                //                select m).ToList()
                //                .Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).ToList();

                if (PfCode != null && PfCode != "")
                {
                    var obj = (from t in db.TransactionViews
                               join ad in db.addtopayamounts on t.Consignment_no equals ad.consinmentno into adt
                               from ad in adt.DefaultIfEmpty()
                               where t.topay == "yes" && t.Pf_Code == PfCode
                               select new
                               {
                                   Customer_Id = t.Customer_Id,
                                   Consignment_no = t.Consignment_no,
                                   booking_date = t.booking_date,
                                   Pincode = t.Pincode,
                                   Name = t.Name,
                                   TopayAmount = t.TopayAmount,
                                   Topaycharges = t.Topaycharges,
                                   status_t = "Paid"
                               }).AsEnumerable().Select(x => new TransactionView
                               {
                                   Customer_Id = x.Customer_Id,
                                   Consignment_no = x.Consignment_no,
                                   booking_date = x.booking_date,
                                   Pincode = x.Pincode,
                                   Name = x.Name,
                                   TopayAmount = x.TopayAmount,
                                   Topaycharges = x.Topaycharges,
                                   status_t = "Paid"
                               }).Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).ToList();
                    return View(obj);

                }
                else
                {
                    var obj = (from t in db.TransactionViews
                               join ad in db.addtopayamounts on t.Consignment_no equals ad.consinmentno into adt
                               from ad in adt.DefaultIfEmpty()
                               where t.topay == "yes"
                               select new
                               {
                                   Customer_Id = t.Customer_Id,
                                   Consignment_no = t.Consignment_no,
                                   booking_date = t.booking_date,
                                   Pincode = t.Pincode,
                                   Name = t.Name,
                                   TopayAmount = t.TopayAmount,
                                   Topaycharges = t.Topaycharges,
                                   status_t = "Paid"
                               }).AsEnumerable().Select(x => new TransactionView
                               {
                                   Customer_Id = x.Customer_Id,
                                   Consignment_no = x.Consignment_no,
                                   booking_date = x.booking_date,
                                   Pincode = x.Pincode,
                                   Name = x.Name,
                                   TopayAmount = x.TopayAmount,
                                   Topaycharges = x.Topaycharges,
                                   status_t = "Paid"
                               }).Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).ToList();
                    return View(obj);
                }
            }
            return View();
        }



        public ActionResult Codreport()
        {
            //   List<TransactionView> transactions = db.TransactionViews.Where(m => m.cod == "yes").ToList();

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            var obj = (from t in db.TransactionViews
                       join ad in db.addcodamounts on t.Consignment_no equals ad.consinment_no into adt
                       from ad in adt.DefaultIfEmpty()
                       where t.cod=="yes"
                       select new
                       {
                           Customer_Id = t.Customer_Id,
                           Consignment_no = t.Consignment_no,
                           booking_date = t.booking_date,
                           Pincode = t.Pincode,
                           Name = t.Name,
                           codAmount = t.codAmount,
                           codcharges = t.codcharges,
                           status_t = ad != null ? "Paid" : "unpaid"
                       }).AsEnumerable().Select(x => new TransactionView
                       {
                           Customer_Id = x.Customer_Id,
                           Consignment_no = x.Consignment_no,
                           booking_date = x.booking_date,
                           Pincode = x.Pincode,
                           Name = x.Name,
                           codAmount = x.codAmount,
                           codcharges = x.codcharges,
                           status_t = x.status_t
                       }).ToList();

            return View(obj);
        }

        [HttpPost]
        public ActionResult Codreport(string PfCode, string ToDatetime, string Fromdatetime)
        {
            // List<TransactionView> transactions = new List<TransactionView>();

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);
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


                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                DateTime fromdate = Convert.ToDateTime(bdatefrom);
                DateTime todate = Convert.ToDateTime(bdateto);

                //transactions = (from m in db.TransactionViews
                //                where m.cod == "yes"
                //                select m).ToList()
                //                .Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0 ).ToList();
                if (PfCode != null && PfCode != "")
                {
                    var obj = (from t in db.TransactionViews
                               join ad in db.addcodamounts on t.Consignment_no equals ad.consinment_no into adt
                               from ad in adt.DefaultIfEmpty()
                               where t.cod == "yes" && t.Pf_Code == PfCode
                               select new
                               {
                                   Customer_Id = t.Customer_Id,
                                   Consignment_no = t.Consignment_no,
                                   booking_date = t.booking_date,
                                   Pincode = t.Pincode,
                                   Name = t.Name,
                                   TopayAmount = t.TopayAmount,
                                   Topaycharges = t.Topaycharges,
                                   status_t = ad != null ? "Paid" : "unpaid"
                               }).AsEnumerable().Select(x => new TransactionView
                               {
                                   Customer_Id = x.Customer_Id,
                                   Consignment_no = x.Consignment_no,
                                   booking_date = x.booking_date,
                                   Pincode = x.Pincode,
                                   Name = x.Name,
                                   TopayAmount = x.TopayAmount,
                                   Topaycharges = x.Topaycharges,
                                   status_t = x.status_t
                               }).Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).ToList();
                    return View(obj);
                }
                else
                {
                    var obj = (from t in db.TransactionViews
                               join ad in db.addcodamounts on t.Consignment_no equals ad.consinment_no into adt
                               from ad in adt.DefaultIfEmpty()
                               where t.cod == "yes" && t.Pf_Code == PfCode
                               select new
                               {
                                   Customer_Id = t.Customer_Id,
                                   Consignment_no = t.Consignment_no,
                                   booking_date = t.booking_date,
                                   Pincode = t.Pincode,
                                   Name = t.Name,
                                   TopayAmount = t.TopayAmount,
                                   Topaycharges = t.Topaycharges,
                                   status_t = ad != null ? "Paid" : "unpaid"
                               }).AsEnumerable().Select(x => new TransactionView
                               {
                                   Customer_Id = x.Customer_Id,
                                   Consignment_no = x.Consignment_no,
                                   booking_date = x.booking_date,
                                   Pincode = x.Pincode,
                                   Name = x.Name,
                                   TopayAmount = x.TopayAmount,
                                   Topaycharges = x.Topaycharges,
                                   status_t = x.status_t
                               }).Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).ToList();
                    return View(obj);
                }

                   

            }

            return View();
        }

        [HttpGet]
        public ActionResult SaleReportBeforeInvoice()
        {


            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");


            List<TransactionView> list = new List<TransactionView>();

            return View(list);
        }

        [HttpPost]
        public ActionResult SaleReportBeforeInvoice(string PfCode, string Fromdatetime, string ToDatetime)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }







            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            Pfsum = (from student in db.Franchisees
                     group student by student.PF_Code into studentGroup
                     select new DisplayPFSum
                     {
                         PfCode = studentGroup.Key,
                         Sum = db.TransactionViews.Where(m => (m.Pf_Code == PfCode)).AsEnumerable().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date)
                           .Select(m => m.Amount + m.BillAmount).Sum()



                     }).ToList();




            //}).ToList();


            ////Branchname = (from od in db.Franchisees
            ////                             where od.PF_Code == studentGroup.Key
            ////                             select od.BranchName).FirstOrDefault()

            //    ViewBag.totalamt = transactions.Sum(b => b.Amount);

            //    return View(transactions);



            //}

            return View();


        }

        public ActionResult creditorsreport()
        {
            //ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");

            //ViewBag.Employees = new SelectList(db.Users.Take(0), "Name", "Name");

            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();//Session["pfCode"].ToString();

            var rc = db.getReceiptDetails(PfCode).Select(x => new Receipt_details
            {

                Consignment_No = x.Consignment_No,
                Destination = x.Destination,
                sender_phone = x.sender_phone,
                SenderCity = x.SenderCity,
                SenderPincode = x.SenderPincode,
                Reciepents_phone = x.Reciepents_phone,
                Reciepents = x.Reciepents,
                ReciepentsPincode = x.ReciepentsPincode,
                Pf_Code = x.Pf_Code,
                Datetime_Cons = x.Datetime_Cons,
                Charges_Total = x.Charges_Total,
                Paid_Amount=x.Paid_Amount
            }).ToList();

           // List<Receipt_details> rc = new List<Receipt_details>();

            ViewBag.sum = (from emp in db.Receipt_details
                           where emp.Pf_Code==PfCode
                           select emp.Charges_Total).Sum();

            return View(rc);
        }


        [HttpPost]
        public ActionResult creditorsreport(string Employees, string ToDatetime, string Fromdatetime, string Submit)
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            if (Employees == null)
            {
                Employees = "";
            }

            List<Receipt_details> rc = new List<Receipt_details>();

            rc = db.Receipt_details.ToList();

            // ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);

            ViewBag.Employees = Employees;//new SelectList(db.Users, "Name", "Name", Employees);


            ViewBag.Fromdatetime = Fromdatetime;
            ViewBag.ToDatetime = ToDatetime;


            {


                ViewBag.selectedemp = Employees;


                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                DateTime fromdate = Convert.ToDateTime(bdatefrom);
                DateTime todate = Convert.ToDateTime(bdateto);


                //if (Employees == "")
                //{

                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode && m.Datetime_Cons != null
                          select m).ToList()
                        .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                            .ToList();
                    //.ToList()
                    //.Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                    //.ToList();

               // }

                //else if (PfComployees == "")
                //{
                //    rc = (from m in db.Receipt_details
                //          where m.Pf_Code == PfCode && m.Datetime_Cons != null
                //          select m).ToList()
                //           .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                //              .ToList();


                //}
                //else if (Employees != "" )
                //{
                //    rc = (from m in db.Receipt_details
                //          where m.Pf_Code == PfCode && m.Datetime_Cons != null
                //          select m).ToList()
                //          .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && x.Paid_Amount < x.Charges_Total && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                //              .ToList();
                //}
                //else
                //{
                //    var compdata = (from c in db.Companies
                //                    where c.Company_Name == Employees
                //                    select new { c.Company_Name }).FirstOrDefault();

                //    rc = (from m in db.Receipt_details
                //          where m.Pf_Code == PfCode
                //          && compdata.Company_Name == Employees
                //          && m.Datetime_Cons != null
                //          select m).ToList()
                //           .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && x.Paid_Amount < x.Charges_Total && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                //              .ToList();
                //}





                ViewBag.sum = (from emp in rc

                               select emp.Charges_Total).Sum();
                rc = rc.OrderBy(m => m.Datetime_Cons).ToList();

            }

            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(rc);
            }
            return View(rc);
        }


        public ActionResult DailySaleReport(string pfcode, string Fromdate = null, string Todate = null)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);

            DateTime? dateTime = DateTime.Now;

            DateTime? fromdate = null;
            DateTime? todate = null;
            if (Fromdate != null && Todate != null)
            {
                ViewBag.fromdate = Fromdate;
                ViewBag.todate = Todate;
            }
            else
            {
                ViewBag.todaydate = GetLocalTime.GetDateTime();
                DateTime? EnteredDate;
                EnteredDate = DateTime.Now;
                Fromdate = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
                Todate = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
            }


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(Todate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdate;
            ViewBag.todate = Todate;

            //string pfcode = Session["pfCode"].ToString();

            //List<Receipt_details> rc = db.Receipt_details.Where(m => m.Datetime_Cons.Value.Day == dateTime.Value.Day
            //&& m.Datetime_Cons.Value.Month == dateTime.Value.Month
            //&& m.Datetime_Cons.Value.Year == dateTime.Value.Year
            //&& m.Pf_Code == pfcode
            //).ToList();

            List<Receipt_details> rc = (from od in db.Receipt_details
                                        where od.Pf_Code == pfcode
                                        && DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
                                        && DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)
                                        select od).ToList();
            //from od in db.Receipt_details
            //where od.Pf_Code == studentGroup.Key
            //&& DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
            //&& DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)

            //select od.Charges_Total).Sum()) ?? 0,

            var sum = (from emp in rc

                       select emp.Paid_Amount).Sum();

            ViewBag.bycard = (from card in rc
                              where card.Credit == "card"
                              select card.Paid_Amount).Sum();
            ViewBag.bycheque = (from cheque in rc
                                where cheque.Credit == "cheque"
                                select cheque.Paid_Amount).Sum();
            ViewBag.bycredit = (from credit in rc
                                where credit.Credit == "credit"
                                select credit.Paid_Amount).Sum();
            ViewBag.bycash = (from cash in rc
                              where cash.Credit == "cash"
                              select cash.Paid_Amount).Sum();
            ViewBag.byother = (from other in rc
                               where other.Credit == "other"
                               select other.Paid_Amount).Sum();


            // ViewBag.Expense = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == dateTime.Value.Day
            //  && m.Datetime_Exp.Value.Month == dateTime.Value.Month
            //  && m.Datetime_Exp.Value.Year == dateTime.Value.Year
            //  && m.Pf_Code == pfcode
            //).ToList();

            ViewBag.Expense = (from e in db.Expenses
                               where e.Pf_Code == pfcode
                                && DbFunctions.TruncateTime(e.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate)
                                       && DbFunctions.TruncateTime(e.Datetime_Exp) <= DbFunctions.TruncateTime(todate)
                               select e).ToList();


            //   ViewBag.expenseCount = (db.Expenses.Where(m => m.Datetime_Exp.Value.Day == dateTime.Value.Day
            //   && m.Datetime_Exp.Value.Month == dateTime.Value.Month
            //   && m.Datetime_Exp.Value.Year == dateTime.Value.Year
            //   && m.Pf_Code == pfcode
            //).Select(m => m.Amount).Sum()?? 0);

            ViewBag.expenseCount = ((from ec in db.Expenses
                                     where ec.Pf_Code == pfcode
                                      && DbFunctions.TruncateTime(ec.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate)
                                             && DbFunctions.TruncateTime(ec.Datetime_Exp) <= DbFunctions.TruncateTime(todate)
                                     select ec.Amount).Sum() ?? 0);


            //  ViewBag.Payment = db.Payments.Where(m => m.Datetime_Pay.Value.Day == dateTime.Value.Day
            //  && m.Datetime_Pay.Value.Month == dateTime.Value.Month
            //  && m.Datetime_Pay.Value.Year == dateTime.Value.Year
            //  && m.Pf_Code == pfcode
            //).ToList();

            ViewBag.Payment = (from p in db.Payments
                               where p.Pf_Code == pfcode
                                && DbFunctions.TruncateTime(p.Datetime_Pay) >= DbFunctions.TruncateTime(fromdate)
                                       && DbFunctions.TruncateTime(p.Datetime_Pay) <= DbFunctions.TruncateTime(todate)
                               select p).ToList();


            //         ViewBag.PaymentCount = (db.Payments.Where(m => m.Datetime_Pay.Value.Day == dateTime.Value.Day
            //  && m.Datetime_Pay.Value.Month == dateTime.Value.Month
            //  && m.Datetime_Pay.Value.Year == dateTime.Value.Year
            //  && m.Pf_Code == pfcode
            //).Select(m => m.amount).Sum()?? 0);

            ViewBag.PaymentCount = ((from pc in db.Payments
                                     where pc.Pf_Code == pfcode
                                      && DbFunctions.TruncateTime(pc.Datetime_Pay) >= DbFunctions.TruncateTime(fromdate)
                                             && DbFunctions.TruncateTime(pc.Datetime_Pay) <= DbFunctions.TruncateTime(todate)
                                     select pc.amount).Sum() ?? 0);


            //    ViewBag.Savings = db.Savings.Where(m => m.Datetime_Sav.Value.Day == dateTime.Value.Day
            //  && m.Datetime_Sav.Value.Month == dateTime.Value.Month
            //  && m.Datetime_Sav.Value.Year == dateTime.Value.Year
            //  && m.Pf_Code == pfcode
            //).ToList();

            ViewBag.Savings = (from s in db.Savings
                               where s.Pf_Code == pfcode
                                && DbFunctions.TruncateTime(s.Datetime_Sav) >= DbFunctions.TruncateTime(fromdate)
                                       && DbFunctions.TruncateTime(s.Datetime_Sav) <= DbFunctions.TruncateTime(todate)
                               select s).ToList();


            //           ViewBag.Savingscount = (db.Savings.Where(m => m.Datetime_Sav.Value.Day == dateTime.Value.Day
            //  && m.Datetime_Sav.Value.Month == dateTime.Value.Month
            //  && m.Datetime_Sav.Value.Year == dateTime.Value.Year
            //  && m.Pf_Code == pfcode
            //).Select(m => m.Saving_amount).Sum()??0);

            ViewBag.Savingscount = ((from sc in db.Savings
                                     where sc.Pf_Code == pfcode
                                      && DbFunctions.TruncateTime(sc.Datetime_Sav) >= DbFunctions.TruncateTime(fromdate)
                                             && DbFunctions.TruncateTime(sc.Datetime_Sav) <= DbFunctions.TruncateTime(todate)
                                     select sc.Saving_amount).Sum() ?? 0);

            if (ViewBag.expenseCount != null && ViewBag.PaymentCount != null)
            {
                ViewBag.sum = ((sum + ViewBag.PaymentCount) - (ViewBag.expenseCount));

            }

            if (ViewBag.expenseCount == null && ViewBag.PaymentCount == null)
            {
                ViewBag.sum = sum;
            }

            if (ViewBag.PaymentCount == null && ViewBag.expenseCount != null)
            {
                ViewBag.sum = (sum - ViewBag.expenseCount);
            }
            if (ViewBag.expenseCount == null && ViewBag.PaymentCount != null)
            {
                ViewBag.sum = (sum + ViewBag.PaymentCount);
            }

            return View(rc);
        }




        [HttpPost]
        public ActionResult DailySaleReport(string Fromdate, string Todate, string pfcode, string Submit)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);

            DateTime? dateTime = DateTime.Now;

            DateTime? fromdate = null;
            DateTime? todate = null;
            if (Fromdate != null && Todate != null)
            {
                ViewBag.fromdate = Fromdate;
                ViewBag.todate = Todate;
            }
            else
            {
                ViewBag.todaydate = GetLocalTime.GetDateTime();
                DateTime? EnteredDate;
                EnteredDate = DateTime.Now;
                Fromdate = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
                Todate = GetLocalTime.GetDateTime().ToString("dd-MM-yyyy");
            }


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(Todate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdate;
            ViewBag.todate = Todate;



            //string pfcode = Session["pfCode"].ToString();

            List<Receipt_details> rc = (from od in db.Receipt_details
                                        where od.Pf_Code == pfcode
                                        && DbFunctions.TruncateTime(od.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate)
                                        && DbFunctions.TruncateTime(od.Datetime_Cons) <= DbFunctions.TruncateTime(todate)
                                        select od).ToList();




            var sum = (from emp in rc

                       select emp.Paid_Amount).Sum();

            ViewBag.bycard = (from card in rc
                              where card.Credit == "card"
                              select card.Paid_Amount).Sum();
            ViewBag.bycheque = (from cheque in rc
                                where cheque.Credit == "cheque"
                                select cheque.Paid_Amount).Sum();
            ViewBag.bycredit = (from credit in rc
                                where credit.Credit == "credit"
                                select credit.Paid_Amount).Sum();
            ViewBag.bycash = (from cash in rc
                              where cash.Credit == "cash"
                              select cash.Paid_Amount).Sum();
            ViewBag.byother = (from other in rc
                               where other.Credit == "other"
                               select other.Paid_Amount).Sum();



            ViewBag.Expense = (from e in db.Expenses
                               where e.Pf_Code == pfcode
                                && DbFunctions.TruncateTime(e.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate)
                                       && DbFunctions.TruncateTime(e.Datetime_Exp) <= DbFunctions.TruncateTime(todate)
                               select e).ToList();


            ViewBag.expenseCount = ((from ec in db.Expenses
                                     where ec.Pf_Code == pfcode
                                      && DbFunctions.TruncateTime(ec.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate)
                                             && DbFunctions.TruncateTime(ec.Datetime_Exp) <= DbFunctions.TruncateTime(todate)
                                     select ec.Amount).Sum() ?? 0);


            ViewBag.Payment = (from p in db.Payments
                               where p.Pf_Code == pfcode
                                && DbFunctions.TruncateTime(p.Datetime_Pay) >= DbFunctions.TruncateTime(fromdate)
                                       && DbFunctions.TruncateTime(p.Datetime_Pay) <= DbFunctions.TruncateTime(todate)
                               select p).ToList();


            ViewBag.PaymentCount = ((from pc in db.Payments
                                     where pc.Pf_Code == pfcode
                                      && DbFunctions.TruncateTime(pc.Datetime_Pay) >= DbFunctions.TruncateTime(fromdate)
                                             && DbFunctions.TruncateTime(pc.Datetime_Pay) <= DbFunctions.TruncateTime(todate)
                                     select pc.amount).Sum() ?? 0);


            ViewBag.Savings = (from s in db.Savings
                               where s.Pf_Code == pfcode
                                && DbFunctions.TruncateTime(s.Datetime_Sav) >= DbFunctions.TruncateTime(fromdate)
                                       && DbFunctions.TruncateTime(s.Datetime_Sav) <= DbFunctions.TruncateTime(todate)
                               select s).ToList();



            ViewBag.Savingscount = ((from sc in db.Savings
                                     where sc.Pf_Code == pfcode
                                      && DbFunctions.TruncateTime(sc.Datetime_Sav) >= DbFunctions.TruncateTime(fromdate)
                                             && DbFunctions.TruncateTime(sc.Datetime_Sav) <= DbFunctions.TruncateTime(todate)
                                     select sc.Saving_amount).Sum() ?? 0);

            if (ViewBag.expenseCount != null && ViewBag.PaymentCount != null)
            {
                ViewBag.sum = ((sum + ViewBag.PaymentCount) - (ViewBag.expenseCount));

            }

            if (ViewBag.expenseCount == null && ViewBag.PaymentCount == null)
            {
                ViewBag.sum = sum;
            }

            if (ViewBag.PaymentCount == null && ViewBag.expenseCount != null)
            {
                ViewBag.sum = (sum - ViewBag.expenseCount);
            }
            if (ViewBag.expenseCount == null && ViewBag.PaymentCount != null)
            {
                ViewBag.sum = (sum + ViewBag.PaymentCount);
            }


            return View(rc);
        }

        public ActionResult StepLine()
        {
            try
            {
                ViewBag.DataPoints = JsonConvert.SerializeObject(db.Invoices.Select(m=> new { m.netamount, month= SqlFunctions.DatePart("month",m.invoicedate)+"-"+SqlFunctions.DatePart("year", m.invoicedate) }).GroupBy(o => new
                {
                   o.month
                   
                }).ToList(), _jsonSetting);


                return View();
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                return View("Error");
            }
            catch (System.Data.SqlClient.SqlException)
            {
                return View("Error");
            }

        }

        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
    }
}
