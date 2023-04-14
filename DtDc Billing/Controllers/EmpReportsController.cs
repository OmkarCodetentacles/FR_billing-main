using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace EmpBilling.Controllers
{
    [SessionUserModule]
    public class EmpReportsController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        public ActionResult CreditorsReport()
        {
            List<Invoice> inc = new List<Invoice>();

            return View(inc);
        }


        [HttpPost]
        public ActionResult CreditorsReport(string Fromdatetime, string ToDatetime, string Custid, string status, string Submit)
        {
            DateTime? fromdate = null;
            DateTime? todate = null;

             
            ViewBag.select = status;

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;


            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }


            List<Invoice> collectionAmount = new List<Invoice>();

            if (status == "Paid")
            {
                collectionAmount = (from u in db.Invoices.AsEnumerable()
                                    select new Invoice
                                    {
                                        invoicedate = u.invoicedate,
                                        invoiceno = u.invoiceno,
                                        periodfrom = u.periodfrom,
                                        periodto = u.periodto,
                                        total = u.total,
                                        fullsurchargetax = u.fullsurchargetax,
                                        fullsurchargetaxtotal = u.fullsurchargetaxtotal,
                                        servicetax = u.servicetax,
                                        servicetaxtotal = u.servicetaxtotal,
                                        Customer_Id = u.Customer_Id,
                                        netamount = u.netamount,
                                        paid = u.paid,
                                        discountamount = u.netamount - u.paid

                                    }).
                                      ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0 && x.discountamount <= 0 && (x.Customer_Id == Custid || Custid == ""))
                                          .ToList();  // Discount Amount Is Temporary Column for Checking Balance  // Discount Amount Is Temporary Column for Checking Balance
            }
            else if (status == "Unpaid")
            {
                collectionAmount = (from u in db.Invoices.AsEnumerable()
                                    select new Invoice
                                    {
                                        invoicedate = u.invoicedate,
                                        invoiceno = u.invoiceno,
                                        periodfrom = u.periodfrom,
                                        periodto = u.periodto,
                                        total = u.total,
                                        fullsurchargetax = u.fullsurchargetax,
                                        fullsurchargetaxtotal = u.fullsurchargetaxtotal,
                                        servicetax = u.servicetax,
                                        servicetaxtotal = u.servicetaxtotal,
                                        Customer_Id = u.Customer_Id,
                                        netamount = u.netamount,
                                        paid = u.paid,
                                        discountamount = u.netamount - u.paid

                                    }).
                                       ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0 && (x.discountamount > 0 || x.paid == null) && (x.Customer_Id == Custid || Custid == ""))
                                           .ToList();  // Discount Amount Is Temporary Column for Checking Balance






            }
            else
            {


                collectionAmount = (from u in db.Invoices.AsEnumerable()
                                    select new Invoice
                                    {
                                        invoicedate = u.invoicedate,
                                        invoiceno = u.invoiceno,
                                        periodfrom = u.periodfrom,
                                        periodto = u.periodto,
                                        total = u.total,
                                        fullsurchargetax = u.fullsurchargetax,
                                        fullsurchargetaxtotal = u.fullsurchargetaxtotal,
                                        servicetax = u.servicetax,
                                        servicetaxtotal = u.servicetaxtotal,
                                        Customer_Id = u.Customer_Id,
                                        netamount = u.netamount,
                                        paid = u.paid,
                                        discountamount = u.netamount - u.paid

                                    }).
                          ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0 && (x.Customer_Id == Custid || Custid == ""))
                              .ToList();

            }



            //if (Submit == "Export to Excel")
            //{
            //    ExportToExcelAll.ExportToExcelAdmin(collectionAmount);
            //}



            return View(collectionAmount);
        }

        public ActionResult InvalidConsignment()
        {

            string pfcode = Session["pfCode"].ToString();


            string b = pfcode.Substring(2, pfcode.Length - 2);
            string prcode = "PR" + "" + b;

            var list = (from user in db.Transactions
                        where !db.Companies.Any(f => f.Company_Id == user.Customer_Id) && user.Customer_Id != null && (user.Pf_Code==pfcode || user.Pf_Code == prcode)
                        select user).ToList();

            return View(list);
        }

        public ActionResult Destinations()
        {
            string pfcode = Session["pfCode"].ToString();
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }

            string b = pfcode.Substring(2, pfcode.Length - 2);
            string prcode = "PR" + "" + b;


            var list = (from user in db.Transactions
                        where !db.Destinations.Any(f => f.Pincode == user.Pincode) && (user.Pf_Code == pfcode || user.Pf_Code == prcode)
                        select user).ToList();

            return View(list);
        }


    }
}