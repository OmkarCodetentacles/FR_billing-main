using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Newtonsoft.Json;

namespace DtDc_Billing.Controllers
{
    [SessionAdminold]
    [SessionUserModule]
    public class AtkApiController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        // GET: AtkApi

        public JsonResult Datewise(string Date)
        {


            DateTime? EnteredDate;


            ViewBag.Date = Date;



            if (Date == "" || Date == null)
            {
                EnteredDate = GetLocalTime.GetDateTime();
            }
            else
            {


                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                string bdate = DateTime.ParseExact(Date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                EnteredDate = Convert.ToDateTime(bdate);
            }


            Apiclass apiclass = new Apiclass();

            db.Configuration.ProxyCreationEnabled = false;


            apiclass.Amount = ((from od in db.Receipt_details
                                where (od.Datetime_Cons.Value.Day == EnteredDate.Value.Day)
                              && (od.Datetime_Cons.Value.Month == EnteredDate.Value.Month)
                              && (od.Datetime_Cons.Value.Year == EnteredDate.Value.Year)

                                select od.Charges_Total).Sum()) ?? 0;

            apiclass.Count = ((from od in db.Receipt_details
                               where (od.Datetime_Cons.Value.Day == EnteredDate.Value.Day)
                             && (od.Datetime_Cons.Value.Month == EnteredDate.Value.Month)
                             && (od.Datetime_Cons.Value.Year == EnteredDate.Value.Year)
                               select od)).Count();


            return Json(apiclass, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult Consignment()
        //{
        //    string PfCode = Session["pfCode"].ToString();

        //    List<ConsignmentCount> Consignmentcount = new List<ConsignmentCount>();

        //    ConsignmentCount consptp = new ConsignmentCount();

        //    consptp.Destination = "PTP";
        //    consptp.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("E") && m.Pf_Code == PfCode).Count();

        //    Consignmentcount.Add(consptp);

        //    ConsignmentCount consPlus = new ConsignmentCount();

        //    consPlus.Destination = "Plus";
        //    consPlus.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("V") && m.Pf_Code == PfCode).Count();

        //    Consignmentcount.Add(consPlus);


        //    ConsignmentCount consInternational = new ConsignmentCount();

        //    consInternational.Destination = "International";
        //    consInternational.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("N") && m.Pf_Code == PfCode).Count();

        //    Consignmentcount.Add(consInternational);


        //    ConsignmentCount consDox = new ConsignmentCount();

        //    consDox.Destination = "Standard";
        //    consDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("P") && m.Pf_Code == PfCode).Count();

        //    Consignmentcount.Add(consDox);


        //    ConsignmentCount consNonDox = new ConsignmentCount();

        //    consNonDox.Destination = "Non Dox";
        //    consNonDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("D") && m.Pf_Code == PfCode).Count();

        //    Consignmentcount.Add(consNonDox);


        //    ConsignmentCount consNonVas = new ConsignmentCount();

        //    consNonVas.Destination = "VAS";
        //    consNonVas.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("I") && m.Pf_Code == PfCode).Count();

        //    Consignmentcount.Add(consNonVas);

        //    ViewBag.Consignmentcount = Consignmentcount.Count();

        //    return Json(Consignmentcount, JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult Destination()
        //{
        //    //string PfCode = Session["pfCode"].ToString();

        //    var results = db.destinationCount(Session["pfCode"].ToString()).Select(x => new
        //    {

        //        Destination = x.Destination,
        //        Count = x.DestCount
        //    }).ToList();

        //    ViewBag.DestinationCount = Convert.ToInt32(results.Count());

        //    return Json(results.Take(20), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult LastSevenDays()
        {
            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            DateTime? EnteredDate;

            Apiclass apiclass = new Apiclass();

            db.Configuration.ProxyCreationEnabled = false;


            apiclass.Amount = db.Receipt_details.Where(m => m.Datetime_Cons != null)
                              .ToList().
                             AsEnumerable().Where(m => m.Datetime_Cons.Value.Date >= DateTime.Now.AddDays(-7).Date).Sum(m => m.Charges_Total) ?? 0;

            apiclass.Count = db.Receipt_details.Where(m => m.Datetime_Cons != null)
                              .ToList().
                             AsEnumerable().Where(m => m.Datetime_Cons.Value.Date >= DateTime.Now.AddDays(-7).Date).Count();


            return Json(apiclass, JsonRequestBehavior.AllowGet);

        }

        public JsonResult PfReportDaily(string Date)
        {
            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            DateTime? EnteredDate;

            db.Configuration.ProxyCreationEnabled = false;
            ViewBag.Date = Date;



            if (Date == "" || Date == null)
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
                         Branchname = (from od in db.Franchisees
                                       where od.PF_Code == studentGroup.Key

                                       select od.BranchName).FirstOrDefault()
                     }).ToList();
            return Json(Pfsum, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PercantagePIChart(string Fromdatetime, string ToDatetime)
        {
            List<ChartPfDatapoints> dataPoints = new List<ChartPfDatapoints>();

            db.Configuration.ProxyCreationEnabled = false;

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

            return Json(dataPoints, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Chart(string ToDatetime, string Fromdatetime)
        {
            List<ChartPfDatapoints> dataPoints = new List<ChartPfDatapoints>();

           
                

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
                            select new  { Label = grouped.Key, Y = grouped.Sum(t => t.Charges_Total),Count=grouped.Count() }).ToList();



               

            

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return Json(cons, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JSON(int? Months, int? Years, string pfCode)
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

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            return Json(dataPoints, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Today()
        {


            DateTime? EnteredDate = DateTime.Now;




            Apiclass apiclass = new Apiclass();

            db.Configuration.ProxyCreationEnabled = false;


            apiclass.Amount = ((from od in db.Receipt_details
                                where (od.Datetime_Cons.Value.Day == EnteredDate.Value.Day)
                              && (od.Datetime_Cons.Value.Month == EnteredDate.Value.Month)
                              && (od.Datetime_Cons.Value.Year == EnteredDate.Value.Year)

                                select od.Charges_Total).Sum()) ?? 0;

            apiclass.Count = ((from od in db.Receipt_details
                               where (od.Datetime_Cons.Value.Day == EnteredDate.Value.Day)
                             && (od.Datetime_Cons.Value.Month == EnteredDate.Value.Month)
                             && (od.Datetime_Cons.Value.Year == EnteredDate.Value.Year)
                               select od)).Count();


            return Json(apiclass, JsonRequestBehavior.AllowGet);

        }

       
        public JsonResult Creditors(string Fromdatetime, string ToDatetime, string Custid, string status)
        {

            DateTime? fromdate = null;
            DateTime? todate = null;

            ViewBag.select = status;

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            if (Fromdatetime != "")
            {

                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = null;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = null;
            }
            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }

            db.Configuration.ProxyCreationEnabled = false;

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
                                      ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0 && x.discountamount <= 0)
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
                                       ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0 && (x.discountamount > 0 || x.paid == null))
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
                          ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0)
                              .ToList();

            }


            return Json(collectionAmount.Take(20), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ThisMonth()
        {
            string PfCode = Session["pfCode"].ToString();

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();

            DateTime? EnteredDate;

            Apiclass apiclass = new Apiclass();

            db.Configuration.ProxyCreationEnabled = false;

            apiclass.Amount = db.Receipt_details.Where(m => m.Datetime_Cons != null && m.Pf_Code== PfCode && SqlFunctions.DatePart("month", m.Datetime_Cons) == DateTime.Now.Month && SqlFunctions.DatePart("year", m.Datetime_Cons) == DateTime.Now.Year).Sum(m => m.Charges_Total) ?? 0;

            apiclass.Count = db.Receipt_details.Where(m => m.Datetime_Cons != null && m.Pf_Code == PfCode && SqlFunctions.DatePart("month", m.Datetime_Cons) == DateTime.Now.Month && SqlFunctions.DatePart("year", m.Datetime_Cons) == DateTime.Now.Year).Count();





            return Json(apiclass, JsonRequestBehavior.AllowGet);

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





            db.Configuration.ProxyCreationEnabled = false;

            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();







            Pfsum = (from student in db.TransactionViews
                     where student.Pf_Code == PfCode
                     && student.Customer_Id != null
                     group student by student.Customer_Id into studentGroup
                     select new DisplayPFSum
                     {
                         PfCode = studentGroup.Key,
                         Sum = db.TransactionViews.Where(m =>
               (m.Customer_Id == studentGroup.Key)
                    ).ToList().Where(m => DbFunctions.TruncateTime(m.booking_date) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.booking_date) <= DbFunctions.TruncateTime(todate))
                           .Sum(m => m.Amount + (m.Risksurcharge ?? 0))
                     }
                     ).ToList();



            ;


            return Json(Pfsum.Take(20), JsonRequestBehavior.AllowGet);


        }

        

        //public JsonResult DatewiseLastSenven(string Date)
        //{


        //    DateTime? EnteredDate;


        //    ViewBag.Date = Date;



        //    if (Date == "" || Date == null)
        //    {
        //        EnteredDate = DateTime.Now;
        //    }
        //    else
        //    {


        //        string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
        //           "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

        //        string bdate = DateTime.ParseExact(Date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


        //        EnteredDate = Convert.ToDateTime(bdate);
        //    }


        //   List<Apiclass> apiclasslist = new List<Apiclass>();

        //    Apiclass apiclass = new Apiclass();

        //    db.Configuration.ProxyCreationEnabled = false;


        //    apiclass.Amount = ((from od in db.Receipt_details
        //                        where (od.Datetime_Cons.Value.Day == EnteredDate.Value.Day)
        //                      && (od.Datetime_Cons.Value.Month == EnteredDate.Value.Month)
        //                      && (od.Datetime_Cons.Value.Year == EnteredDate.Value.Year)

        //                        select od.Charges_Total).Sum()) ?? 0;

        //    apiclass.Count = ((from od in db.Receipt_details
        //                       where (od.Datetime_Cons.Value.Day == EnteredDate.Value.Day)
        //                     && (od.Datetime_Cons.Value.Month == EnteredDate.Value.Month)
        //                     && (od.Datetime_Cons.Value.Year == EnteredDate.Value.Year)
        //                       select od)).Count();

        //    apiclasslist.Add(apiclass);


        //    List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();



        //    Apiclass apiclass1 = new Apiclass();

        //    db.Configuration.ProxyCreationEnabled = false;


        //    apiclass1.Amount = db.Receipt_details.Where(m => m.Datetime_Cons != null)
        //                      .ToList().
        //                     AsEnumerable().Where(m => m.Datetime_Cons.Value.Date >= DateTime.Now.AddDays(-7).Date).Sum(m => m.Charges_Total) ?? 0;

        //    apiclass1.Count = db.Receipt_details.Where(m => m.Datetime_Cons != null)
        //                      .ToList().
        //                     AsEnumerable().Where(m => m.Datetime_Cons.Value.Date >= DateTime.Now.AddDays(-7).Date).Count();

        //    apiclasslist.Add(apiclass1);

        //    return Json(apiclasslist, JsonRequestBehavior.AllowGet);

        //}


        public JsonResult pfcode()
        {
            var link = db.Franchisees.Select(m => m.PF_Code).ToList();
            return Json(link, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Destinationpfwise(string pfcode = "")
        {

            var results = (from p in db.Receipt_details
                           where p.Pf_Code == pfcode || pfcode == ""
                           group p by p.Destination into g
                           orderby g.Count() descending
                           select new
                           {

                               Destination = g.Key,
                               Count = g.Count()
                           }).Take(20);


            return Json(results.Take(20), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ViewAllProductReportPF(string PfCode = "")
        {
            List<ConsignmentCount> Consignmentcount = new List<ConsignmentCount>();

            ConsignmentCount consptp = new ConsignmentCount();

            consptp.Destination = "PTP";
            consptp.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("E") && (m.Pf_Code == PfCode || PfCode == "")).Count();

            Consignmentcount.Add(consptp);

            ConsignmentCount consPlus = new ConsignmentCount();

            consPlus.Destination = "Plus";
            consPlus.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("V") && (m.Pf_Code == PfCode || PfCode == "")).Count();

            Consignmentcount.Add(consPlus);


            ConsignmentCount consInternational = new ConsignmentCount();

            consInternational.Destination = "International";
            consInternational.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("N") && (m.Pf_Code == PfCode || PfCode == "")).Count();

            Consignmentcount.Add(consInternational);


            ConsignmentCount consDox = new ConsignmentCount();

            consDox.Destination = "Standart";
            consDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("P") && (m.Pf_Code == PfCode || PfCode == "")).Count();

            Consignmentcount.Add(consDox);


            ConsignmentCount consNonDox = new ConsignmentCount();

            consNonDox.Destination = "Non Dox";
            consNonDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("D") && (m.Pf_Code == PfCode || PfCode == "")).Count();

            Consignmentcount.Add(consNonDox);

            ConsignmentCount consNonVas = new ConsignmentCount();

            consNonVas.Destination = "VAS";
            consNonVas.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("I") && (m.Pf_Code == PfCode || PfCode == "")).Count();

            Consignmentcount.Add(consNonVas);

            return Json(Consignmentcount, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult OutstandingApi(string Fromdatetime, string ToDatetime, string PfCode = "")
        {

            

            DateTime? fromdate = null;
            DateTime? todate = null;


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy","d-M-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            if(Fromdatetime == null || Fromdatetime=="")
            {
                fromdate = DateTime.Now.AddYears(-10);
            }
            else
            { 
            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);
            }

            if(ToDatetime==null || ToDatetime=="")
            {
                todate = GetLocalTime.GetDateTime();
            }
            else
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
            }



            var collectionAmount = (from u in db.Invoices
                                    join b in db.Companies
                                    on u.Customer_Id equals b.Company_Id
                                    where (b.Pf_code == PfCode|| PfCode=="")&&(DbFunctions.TruncateTime(u.invoicedate) >= DbFunctions.TruncateTime(fromdate)) && (DbFunctions.TruncateTime(u.invoicedate) <= DbFunctions.TruncateTime(todate))
                                    group u by u.Customer_Id into g
                                select new
                                {
                                    CompanyId = g.Key,
                                    Total = g.Sum(m => m.netamount),
                                    paid = g.Sum(m => m.paid) ?? 0,
                                    Balance = (g.Sum(m=>m.netamount)??0) - (g.Sum(m=>m.paid)??0)
                                }).OrderByDescending(m=>m.Balance).
                      ToList();

            
            
            

            return Json(collectionAmount, JsonRequestBehavior.AllowGet);
        }



        public JsonResult AdminDailyReport(string searcheddate= "", string pfcode= "")
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);

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

            db.Configuration.ProxyCreationEnabled = false;

            //List<Receipt_details> rc = db.Receipt_details.Where(m => m.Datetime_Cons.Value.Day == dateTime.Value.Day
            //&& m.Datetime_Cons.Value.Month == dateTime.Value.Month
            //&& m.Datetime_Cons.Value.Year == dateTime.Value.Year
            //&& m.Pf_Code == pfcode
            //).ToList();

            var rc = (from od in db.Receipt_details
                       where (od.Datetime_Cons.Value.Day == dateTime.Value.Day)
                     && (od.Datetime_Cons.Value.Month == dateTime.Value.Month)
                     && (od.Datetime_Cons.Value.Year == dateTime.Value.Year)
                       select 
                       od).ToList();


            //return View(rc);
            return Json(rc, JsonRequestBehavior.AllowGet);

           
        }




        public JsonResult DatewiseAdminDailyReport(string Fromdatetime, string ToDatetime, string pfcode = "")
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                fromdate = Convert.ToDateTime(Fromdatetime, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
               // fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                todate = Convert.ToDateTime(ToDatetime, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
              //  todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }


            //string pfcode = Session["pfCode"].ToString();

            db.Configuration.ProxyCreationEnabled = false;

            //List<Receipt_details> rc = db.Receipt_details.Where(m => m.Datetime_Cons.Value.Day == dateTime.Value.Day
            //&& m.Datetime_Cons.Value.Month == dateTime.Value.Month
            //&& m.Datetime_Cons.Value.Year == dateTime.Value.Year
            //&& m.Pf_Code == pfcode
            //).ToList();

            var rc = (from od in db.Receipt_details
                      select
                      od).Where(m => m.Datetime_Cons != null && DbFunctions.TruncateTime(m.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Cons) <= DbFunctions.TruncateTime(todate)).ToList();

            //.Where(m => m.Datetime_Cons.Value.Date >= fromdate.Value.Date && m.Datetime_Cons.Value.Date <= todate.Value.Date).ToList();

            //   .Where(m => m.Datetime_Cons != null && DbFunctions.TruncateTime(m.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Cons) <= DbFunctions.TruncateTime(todate)).ToList();
            //return View(rc);
            return Json(rc, JsonRequestBehavior.AllowGet);


        }

       

    }
}
