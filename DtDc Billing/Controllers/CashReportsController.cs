using DtDc_Billing.CustomModel;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    [SessionUserModule]
    public class CashReportsController : Controller
    {
        db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Reports
        [SessionTimeout]
        public ActionResult ReceiptReports()
        {
            string pfcode = Session["pfCode"].ToString();

            //List<Receipt_details> rd = db.Receipt_details.Where(m => m.Pf_Code == pfcode).OrderByDescending(m => m.Receipt_Id).ToList();
            List<Receipt_details> rd = new List<Receipt_details>();
            ViewBag.totalAmt = (from emp in rd

                                select emp.Charges_Total).Sum();

            return View(rd);

        }

        [HttpPost]
        public ActionResult ReceiptReports(string ToDatetime, string Fromdatetime, string Submit)
        {



            string pfcode = Session["pfCode"].ToString();

            List<Receipt_details> rd = db.Receipt_details.Where(m => m.Pf_Code == pfcode).OrderByDescending(m => m.Receipt_Id).ToList();


            ///////////////////////////////////////

            ViewBag.Fromdatetime = Fromdatetime;
            ViewBag.ToDatetime = ToDatetime;



            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);


            if (pfcode == "")
            {


                rd = db.Receipt_details.Where(m => m.Datetime_Cons != null)
                          .ToList()
                          .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                          .ToList();

                ViewBag.totalAmt = (from emp in rd

                                    select emp.Charges_Total).Sum();
            }
            else
            {
                rd = (from m in db.Receipt_details
                      where m.Pf_Code == pfcode && m.Datetime_Cons != null
                      select m).ToList()
                     .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                     .ToList();

                ViewBag.totalAmt = (from emp in rd

                                    select emp.Charges_Total).Sum();
            }




            return View(rd);
        }


        [SessionTimeout]
        public ActionResult WalletReports()
        {
            return View(db.WalletPoints.OrderBy(m => m.Wallet_Id).ToList());
        }

        [SessionAdmin]
        public ActionResult SaleReports()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");

            ViewBag.Employees = new SelectList(db.Users.Take(0), "Name", "Name");

            List<Receipt_details> rc = db.Receipt_details.ToList();

            ViewBag.sum = (from emp in db.Receipt_details

                           select emp.Charges_Total).Sum();

            return View(rc);
        }

        [SessionAdmin]
        [HttpPost]
        public ActionResult SaleReports(string PfCode, string Employees, string ToDatetime, string Fromdatetime, string Submit)
        {




            if (Employees == null)
            {
                Employees = "";
            }

            List<Receipt_details> rc = new List<Receipt_details>();

            rc = db.Receipt_details.ToList();

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);

            ViewBag.Employees = new SelectList(db.Users, "Name", "Name", Employees);



            if (Fromdatetime == "")
            {
                if (PfCode == "" && Employees == "")
                {

                    rc = db.Receipt_details.ToList();
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
                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode
                          && m.User.Name == Employees

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
                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode
                          && m.User.Name == Employees

                          select m).ToList();
                }
            }
            else
            {
                ViewBag.Fromdatetime = Fromdatetime;
                ViewBag.ToDatetime = ToDatetime;

                ViewBag.selectedemp = Employees;


                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                DateTime fromdate = Convert.ToDateTime(bdatefrom);
                DateTime todate = Convert.ToDateTime(bdateto);


                if (PfCode == "" && Employees == "")
                {

                    rc = db.Receipt_details.Where(m => m.Datetime_Cons != null)
                              .ToList()
                              .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                              .ToList();

                }

                else if (PfCode != "" && Employees == "")
                {
                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode &&  m.Datetime_Cons != null
                          select m).ToList()
                           .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                              .ToList();


                }
                else if (Employees != "" && PfCode == "")
                {
                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode && m.Datetime_Cons != null
                          select m).ToList()
                          .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                              .ToList();
                }
                else
                {
                    rc = (from m in db.Receipt_details
                          where m.Pf_Code == PfCode
                          && m.User.Name == Employees
                          && m.Datetime_Cons != null
                          select m).ToList()
                           .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate) <= 0)
                              .ToList();
                }





                ViewBag.sum = (from emp in rc

                               select emp.Charges_Total).Sum();

            }

            if (Submit == "Export to Excel")
            {
                ExportToExcelAdmin(rc);
            }
            return View(rc);
        }


        public ActionResult UniqueReport()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            List<Receipt_details> rc = db.Receipt_details.ToList();

            ViewBag.sum = (from emp in db.Receipt_details

                           select emp.Charges_Total).Sum();

            return View(rc);
        }
        [HttpPost]
        public ActionResult UniqueReport(string PfCode, string ToDatetime, string Fromdatetime, string Submit)
        {
            List<Receipt_details> rc = new List<Receipt_details>();

            rc = db.Receipt_details.ToList();

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);


            if (Fromdatetime == "")
            {

                if (PfCode != "")
                {
                    rc = db.Receipt_details.Where(m => m.Pf_Code == PfCode


                   ).ToList();
                }
                else
                {
                    rc = db.Receipt_details.ToList();
                }


            }
            else if (ToDatetime == "")
            {
                if (PfCode != "")
                {
                    rc = db.Receipt_details.Where(m => m.Pf_Code == PfCode


                   ).ToList();
                }
                else
                {
                    rc = db.Receipt_details.ToList();
                }
            }
            else if (PfCode == "")
            {
                if (Submit == "Export to Excel")
                {
                    ExportToExcelAdmin(rc);
                }

                ModelState.AddModelError("Pferror", "Please select PfCode");
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



            }


            if (Submit == "Export to Excel")
            {
                ExportToExcelAdmin(rc);
            }


            return View(rc);
        }

        public ActionResult GetUserList(string Pfcode)
        {
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
            string pfcode = Session["pfCode"].ToString();

            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

            List<Receipt_details> rc = db.Receipt_details.Where(m => m.Datetime_Cons.Value.Day == localTime.Day
            && m.Datetime_Cons.Value.Month == localTime.Month
            && m.Datetime_Cons.Value.Year == localTime.Year
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
            ViewBag.paidamount = sum;

            ViewBag.Expense = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == localTime.Day
              && m.Datetime_Exp.Value.Month == localTime.Month
              && m.Datetime_Exp.Value.Year == localTime.Year
              && m.Pf_Code == pfcode
            ).ToList();


            ViewBag.expenseCount = (db.Expenses.Where(m => m.Datetime_Exp.Value.Day == localTime.Day
               && m.Datetime_Exp.Value.Month == localTime.Month
               && m.Datetime_Exp.Value.Year == localTime.Year
               && m.Pf_Code == pfcode
            ).Select(m => m.Amount).Sum() ?? 0);



            ViewBag.Payment = db.Payments.Where(m => m.Datetime_Pay.Value.Day == localTime.Day
            && m.Datetime_Pay.Value.Month == localTime.Month
            && m.Datetime_Pay.Value.Year == localTime.Year
            && m.Pf_Code == pfcode
          ).ToList();

            ViewBag.PaymentCount = (db.Payments.Where(m => m.Datetime_Pay.Value.Day == localTime.Day
         && m.Datetime_Pay.Value.Month == localTime.Month
         && m.Datetime_Pay.Value.Year == localTime.Year
         && m.Pf_Code == pfcode
       ).Select(m => m.amount).Sum() ?? 0);




            ViewBag.Savings = db.Savings.Where(m => m.Datetime_Sav.Value.Day == localTime.Day
          && m.Datetime_Sav.Value.Month == localTime.Month
          && m.Datetime_Sav.Value.Year == localTime.Year
          && m.Pf_Code == pfcode
        ).ToList();


            ViewBag.Savingscount = (db.Savings.Where(m => m.Datetime_Sav.Value.Day == localTime.Day
       && m.Datetime_Sav.Value.Month == localTime.Month
       && m.Datetime_Sav.Value.Year == localTime.Year
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


            var chargetotal = (from emp in rc

                               select emp.Charges_Total).Sum();

            var creditamount = (from emp in rc

                                select emp.Credit_Amount).Sum();

            ViewBag.openingcreditamount = (chargetotal - creditamount ?? 0);
            return View(rc);

        }
        [SessionTimeout]
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

            string pfcode = Session["pfCode"].ToString();

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

            ViewBag.paidamount = sum;

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


            var chargetotal = (from emp in rc

                               select emp.Charges_Total).Sum();

            var creditamount = (from emp in rc

                                select emp.Credit_Amount).Sum();

            ViewBag.openingcreditamount = (chargetotal - creditamount ?? 0);
            return View(rc);
        }

        public ActionResult AdminDailyReport()
        {
            //string pfcode = Session["pfCode"].ToString();

            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

            List<Receipt_details> rc = db.Receipt_details.Where(m => m.Datetime_Cons.Value.Day == localTime.Day
            && m.Datetime_Cons.Value.Month == localTime.Month
            && m.Datetime_Cons.Value.Year == localTime.Year
            //&& m.Pf_Code == pfcode
            ).ToList();


            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");



            ViewBag.sum = (from emp in rc

                           select emp.Paid_Amount).Sum();


            ViewBag.Expense = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == localTime.Day
              && m.Datetime_Exp.Value.Month == localTime.Month
              && m.Datetime_Exp.Value.Year == localTime.Year
            // && m.Pf_Code == pfcode
            ).ToList();


            ViewBag.expenseCount = db.Expenses.Where(m => m.Datetime_Exp.Value.Day == localTime.Day
               && m.Datetime_Exp.Value.Month == localTime.Month
               && m.Datetime_Exp.Value.Year == localTime.Year
            // && m.Pf_Code == pfcode
            ).Select(m => m.Amount).Sum();



            ViewBag.Payment = db.Payments.Where(m => m.Datetime_Pay.Value.Day == localTime.Day
            && m.Datetime_Pay.Value.Month == localTime.Month
            && m.Datetime_Pay.Value.Year == localTime.Year
          // && m.Pf_Code == pfcode
          ).ToList();

            ViewBag.PaymentCount = db.Payments.Where(m => m.Datetime_Pay.Value.Day == localTime.Day
         && m.Datetime_Pay.Value.Month == localTime.Month
         && m.Datetime_Pay.Value.Year == localTime.Year
       // && m.Pf_Code == pfcode
       ).Select(m => m.amount).Sum();




            ViewBag.Savings = db.Savings.Where(m => m.Datetime_Sav.Value.Day == localTime.Day
          && m.Datetime_Sav.Value.Month == localTime.Month
          && m.Datetime_Sav.Value.Year == localTime.Year
        // && m.Pf_Code == pfcode
        ).ToList();


            ViewBag.Savingscount = db.Savings.Where(m => m.Datetime_Sav.Value.Day == localTime.Day
       && m.Datetime_Sav.Value.Month == localTime.Month
       && m.Datetime_Sav.Value.Year == localTime.Year
     //  && m.Pf_Code == pfcode
     ).Select(m => m.Saving_amount).Sum();





            return View(rc);

        }

        [SessionAdmin]
        [HttpPost]
        public ActionResult AdminDailyReport(string searcheddate, string pfcode, string Submit)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);

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

        public ActionResult BulkBooking()
        {

            return View();
        }

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
                                 .Sum(m=>m.Charges_Total) ?? 0                        


                         }).ToList();












            }











            return View(Pfsum);
        }




        [SessionAdmin]
        [HttpGet]
         public ActionResult PfReportDaily()
         {

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            DateTime? EnteredDate;            
                

                 
            
                EnteredDate = DateTime.Now;
            
          




            Pfsum = (from student in db.Franchisees
                         group student by student.PF_Code into studentGroup
                         select new DisplayPFSum
                         {
                             PfCode = studentGroup.Key,
                             Sum =
                                 ((from od in db.Receipt_details
                                   where od.Pf_Code == studentGroup.Key

                                 && (od.Datetime_Cons.Value.Day == EnteredDate.Value.Day)
                                 && (od.Datetime_Cons.Value.Month == EnteredDate.Value.Month)
                                 && (od.Datetime_Cons.Value.Year == EnteredDate.Value.Year)


                                   select od.Charges_Total).Sum()) ?? 0,

                             Branchname = (from od in db.Franchisees
                                           where od.PF_Code == studentGroup.Key
                                           select od.BranchName).FirstOrDefault()
                         }).ToList();


            











            return View(Pfsum);
        }

        [SessionAdmin]
        [HttpPost]
        public ActionResult PfReportDaily(string Date)
        {

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            DateTime? EnteredDate;


                ViewBag.Date = Date;
                

                 
            if(Date == ""  || Date == null)
            {
                EnteredDate = DateTime.Now;
            }
            else
            {
                

                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                string bdate = DateTime.ParseExact(Date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                EnteredDate = Convert.ToDateTime(bdate);
            }




            Pfsum = (from student in db.Franchisees
                         group student by student.PF_Code into studentGroup
                         select new DisplayPFSum
                         {

                             PfCode = studentGroup.Key,
                             Sum =
                                 ((from od in db.Receipt_details
                                   where od.Pf_Code == studentGroup.Key

                                 && (od.Datetime_Cons.Value.Day == EnteredDate.Value.Day)
                                 && (od.Datetime_Cons.Value.Month == EnteredDate.Value.Month)
                                 && (od.Datetime_Cons.Value.Year == EnteredDate.Value.Year)

                                   select od.Charges_Total).Sum()) ?? 0,
                             Branchname= (from od in db.Franchisees
                                         where od.PF_Code == studentGroup.Key                                   

                                         select od.BranchName).FirstOrDefault()
                         }).ToList();       











            return View(Pfsum);
        }


        public void ExportToExcel(DateTime? dateTime)
        {
            string pfcode = Session["pfCode"].ToString();

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


        public void ExportToExcelWallet(List<WalletPointModel> rc)
        {
            //string pfcode = Session["pfCode"].ToString();

            var cons = rc;

            var gv = new GridView();
            gv.DataSource = cons;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Walletreport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

        }


        [SessionAdmin]
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



        [SessionAdmin]
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

        public ContentResult JSON(int? Months, int? Years, string pfCode)
        {

            if (Months == null)
            {
                Months = System.DateTime.Now.Month;
            }
            if (Years == null)
            {
                Years = System.DateTime.Now.Year;
            }


            ViewBag.Months = new SelectList(Enumerable.Range(1, 12).Select(x =>
       new SelectListItem()
       {
           Text = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[x - 1] + " (" + x + ")",
           Value = x.ToString()
       }), "Value", "Text", Months);



            ViewBag.Years = new SelectList(Enumerable.Range(DateTime.Today.Year, 20).Select(x =>

               new SelectListItem()
               {
                   Text = x.ToString(),
                   Value = x.ToString()
               }), "Value", "Text", Years);



            List<ChartPFDay> dataPoints = new List<ChartPFDay>();




            //dataPoints.Add(new ChartPFDay(1513449000000, 4.3));
            //dataPoints.Add(new ChartPFDay(1513621800000, 4.36));

            var cons = (from p in db.Franchisees
                        join c in db.Receipt_details on p.PF_Code equals c.Pf_Code into j1

                        from j2 in j1
                        where j2.Datetime_Cons.Value.Month == Months
                          && j2.Datetime_Cons.Value.Year == Years
                          && j2.Pf_Code == pfCode
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



        [SessionAdmin]
        public ActionResult WalletReportsAdmin()
        {

            List<WalletPointModel> list =
       (from student in db.WalletPoints       
       select new 
       {
           MobileNo = student.MobileNo,
           Wallet_Money=student.Wallet_Money,
           Datetime_Wa=student.Datetime_Wa,
           Redeemed = (from od in db.Receipt_details                       
                        where od.sender_phone ==student.MobileNo
                        select od.Discount).Sum(),

           Name = (from od in db.Receipt_details
                       where od.sender_phone == student.MobileNo
                       select od.Sender).FirstOrDefault(),

           PFCode= (from od in db.Receipt_details
                    where od.sender_phone == student.MobileNo
                    select od.Pf_Code).FirstOrDefault(),

       }).AsEnumerable().Select(x => new WalletPointModel
       {
           MobileNo = x.MobileNo,
           Wallet_Money = x.Wallet_Money,
           Datetime_Wa = x.Datetime_Wa,
           Redeemed = x.Redeemed ?? 0,
           Name=x.Name,
           PFCode=x.PFCode
       }).ToList(); 




            return View(list);
        }


        [SessionAdmin]
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



    }
}
