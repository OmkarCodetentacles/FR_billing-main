using DtDc_Billing.CustomModel;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Invoice = DtDc_Billing.Entity_FR.Invoice;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    //   [SessionUserModule]
    public class BillingReportsController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: BillingReports
        public ActionResult DatewiseReport()
        {
            List<TransactionView> list = new List<TransactionView>();

            return View(list);
        }

        [HttpPost]
        public ActionResult DatewiseReport(string Fromdatetime, string ToDatetime, string Submit)
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;


            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);

            ViewBag.fromdate = Fromdatetime;



            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.todate = ToDatetime;





            List<TransactionView> transactions =
                db.TransactionViews.Where(m => m.Customer_Id != null && m.Customer_Id != "").ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                           .ToList();





            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(transactions);
            }

            return View(transactions);
        }


        [HttpGet]
        public ActionResult PfWiseReport()
        {


            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");


            List<TransactionCompanyDest> list = new List<TransactionCompanyDest>();

            return View(list);
        }

        [HttpPost]
        public ActionResult PfWiseReport(string PfCode, string Fromdatetime, string ToDatetime, string Submit)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;


            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);

            ViewBag.fromdate = Fromdatetime;


            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.todate = ToDatetime;



            var list = (from t in db.TransactionViews
                        join c in db.Companies
                        on t.Customer_Id equals c.Company_Id
                        join f in db.Franchisees
                        on t.Pf_Code equals f.PF_Code
                        where (t.Pf_Code == PfCode || PfCode == "") &&
                         t.Customer_Id != null &&
                         DbFunctions.TruncateTime(t.booking_date) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(t.booking_date) <= DbFunctions.TruncateTime(todate)


                        select new
                        {
                            Consignment_no = t.Consignment_no,
                            bookingdate = t.tembookingdate,
                            Pf_Code = t.Pf_Code,
                            Customer_Id = t.Customer_Id,
                            Gst_No = c.Gst_No,
                            chargable_weight = t.chargable_weight,
                            Mode = t.Mode,
                            Type_t = t.Type_t,
                            Name = t.Name,
                            Pincode = t.Pincode,
                            BillAmount = t.BillAmount ?? 0,
                            Amount = t.Amount,
                            Risksurcharge = t.Risksurcharge ?? 0,
                            CnoteCharges = (t.Consignment_no.StartsWith("D") ? c.D_Docket : t.Consignment_no.StartsWith("P") ? c.P_Docket : t.Consignment_no.StartsWith("E") ? c.E_Docket : t.Consignment_no.StartsWith("I") ? c.I_Docket : t.Consignment_no.StartsWith("V") ? c.V_Docket : t.Consignment_no.StartsWith("N") ? c.N_Docket : 0) ?? 0,
                            RoyalCharges = ((t.Amount * c.Royalty_Charges) / 100) ?? 0,
                            ServiceCharges = 0,
                            Subtotal = (t.Amount ?? 0) + (t.Risksurcharge ?? 0) + (((t.Amount * c.Royalty_Charges) / 100) ?? 0) + ((t.Consignment_no.StartsWith("D") ? c.D_Docket : t.Consignment_no.StartsWith("P") ? c.P_Docket : t.Consignment_no.StartsWith("E") ? c.E_Docket : t.Consignment_no.StartsWith("I") ? c.I_Docket : t.Consignment_no.StartsWith("V") ? c.V_Docket : t.Consignment_no.StartsWith("N") ? c.N_Docket : 0) ?? 0),
                            Fuel_Sur_Charge = c.Fuel_Sur_Charge ?? 0,
                            FscAmt = (t.Amount + (t.Risksurcharge ?? 0) + (t.Consignment_no.StartsWith("D") ? c.D_Docket ?? 0 : t.Consignment_no.StartsWith("P") ? c.P_Docket ?? 0 : t.Consignment_no.StartsWith("E") ? c.E_Docket ?? 0 : t.Consignment_no.StartsWith("I") ? c.I_Docket ?? 0 : t.Consignment_no.StartsWith("V") ? c.V_Docket ?? 0 : t.Consignment_no.StartsWith("N") ? c.N_Docket ?? 0 : 0) + (((t.Amount * (c.Royalty_Charges ?? 0)) / 100) ?? 0)) * (c.Fuel_Sur_Charge / 100) ?? 0,
                            Taxable = (t.Amount ?? 0) +
                            (t.Risksurcharge ?? 0) +
                            (((t.Amount * c.Royalty_Charges) / 100) ?? 0) +
                            ((t.Consignment_no.StartsWith("D") ? c.D_Docket : t.Consignment_no.StartsWith("P") ? c.P_Docket : t.Consignment_no.StartsWith("E") ? c.E_Docket : t.Consignment_no.StartsWith("I") ? c.I_Docket : t.Consignment_no.StartsWith("V") ? c.V_Docket : t.Consignment_no.StartsWith("N") ? c.N_Docket : 0) ?? 0) +
                            ((t.Amount + (t.Risksurcharge ?? 0) + (t.Consignment_no.StartsWith("D") ? c.D_Docket ?? 0 : t.Consignment_no.StartsWith("P") ? c.P_Docket ?? 0 : t.Consignment_no.StartsWith("E") ? c.E_Docket ?? 0 : t.Consignment_no.StartsWith("I") ? c.I_Docket ?? 0 : t.Consignment_no.StartsWith("V") ? c.V_Docket ?? 0 : t.Consignment_no.StartsWith("N") ? c.N_Docket ?? 0 : 0) + (((t.Amount * (c.Royalty_Charges ?? 0)) / 100) ?? 0)) * (c.Fuel_Sur_Charge / 100) ?? 0),

                            Cgst = (c.Gst_No == null || c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2)) ? ((t.Amount ?? 0) + (t.Risksurcharge ?? 0) + (((t.Amount * c.Royalty_Charges) / 100) ?? 0) + ((t.Amount + (t.Risksurcharge ?? 0)) + (t.Consignment_no.StartsWith("D") ? c.D_Docket ?? 0 : t.Consignment_no.StartsWith("P") ? c.P_Docket ?? 0 : t.Consignment_no.StartsWith("E") ? c.E_Docket ?? 0 : t.Consignment_no.StartsWith("I") ? c.I_Docket ?? 0 : t.Consignment_no.StartsWith("V") ? c.V_Docket ?? 0 : t.Consignment_no.StartsWith("N") ? c.N_Docket ?? 0 : 0) + (((t.Amount * (c.Royalty_Charges ?? 0)) / 100) ?? 0)) * (c.Fuel_Sur_Charge / 100) ?? 0 + (t.Consignment_no.StartsWith("D") ? c.D_Docket : t.Consignment_no.StartsWith("P") ? c.P_Docket : t.Consignment_no.StartsWith("E") ? c.E_Docket : t.Consignment_no.StartsWith("I") ? c.I_Docket : t.Consignment_no.StartsWith("V") ? c.V_Docket : t.Consignment_no.StartsWith("N") ? c.N_Docket : 0) ?? 0) * 0.09 : 0,
                            Sgst = (c.Gst_No == null || c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2)) ? ((t.Amount ?? 0) + (t.Risksurcharge ?? 0) + (((t.Amount * c.Royalty_Charges) / 100) ?? 0) + ((t.Amount + (t.Risksurcharge ?? 0)) + (t.Consignment_no.StartsWith("D") ? c.D_Docket ?? 0 : t.Consignment_no.StartsWith("P") ? c.P_Docket ?? 0 : t.Consignment_no.StartsWith("E") ? c.E_Docket ?? 0 : t.Consignment_no.StartsWith("I") ? c.I_Docket ?? 0 : t.Consignment_no.StartsWith("V") ? c.V_Docket ?? 0 : t.Consignment_no.StartsWith("N") ? c.N_Docket ?? 0 : 0) + (((t.Amount * (c.Royalty_Charges ?? 0)) / 100) ?? 0)) * (c.Fuel_Sur_Charge / 100) ?? 0 + (t.Consignment_no.StartsWith("D") ? c.D_Docket : t.Consignment_no.StartsWith("P") ? c.P_Docket : t.Consignment_no.StartsWith("E") ? c.E_Docket : t.Consignment_no.StartsWith("I") ? c.I_Docket : t.Consignment_no.StartsWith("V") ? c.V_Docket : t.Consignment_no.StartsWith("N") ? c.N_Docket : 0) ?? 0) * 0.09 : 0,
                            Igst = (c.Gst_No != null && c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2)) ? ((t.Amount ?? 0) + (t.Risksurcharge ?? 0) + (((t.Amount * c.Royalty_Charges) / 100) ?? 0) + ((t.Amount + (t.Risksurcharge ?? 0)) + (t.Consignment_no.StartsWith("D") ? c.D_Docket ?? 0 : t.Consignment_no.StartsWith("P") ? c.P_Docket ?? 0 : t.Consignment_no.StartsWith("E") ? c.E_Docket ?? 0 : t.Consignment_no.StartsWith("I") ? c.I_Docket ?? 0 : t.Consignment_no.StartsWith("V") ? c.V_Docket ?? 0 : t.Consignment_no.StartsWith("N") ? c.N_Docket ?? 0 : 0) + (((t.Amount * (c.Royalty_Charges ?? 0)) / 100) ?? 0)) * (c.Fuel_Sur_Charge / 100) ?? 0 + (t.Consignment_no.StartsWith("D") ? c.D_Docket : t.Consignment_no.StartsWith("P") ? c.P_Docket : t.Consignment_no.StartsWith("E") ? c.E_Docket : t.Consignment_no.StartsWith("I") ? c.I_Docket : t.Consignment_no.StartsWith("V") ? c.V_Docket : t.Consignment_no.StartsWith("N") ? c.N_Docket : 0) ?? 0) * 0.18 : 0,
                            GrandTotal = (
                            (
                            (t.Amount ?? 0) +
                            (t.Risksurcharge ?? 0) +
                            (((t.Amount * c.Royalty_Charges) / 100) ?? 0) +
                            ((t.Consignment_no.StartsWith("D") ? c.D_Docket : t.Consignment_no.StartsWith("P") ? c.P_Docket : t.Consignment_no.StartsWith("E") ? c.E_Docket : t.Consignment_no.StartsWith("I") ? c.I_Docket : t.Consignment_no.StartsWith("V") ? c.V_Docket : t.Consignment_no.StartsWith("N") ? c.N_Docket : 0) ?? 0) +
                            ((t.Amount + (t.Risksurcharge ?? 0) + (t.Consignment_no.StartsWith("D") ? c.D_Docket ?? 0 : t.Consignment_no.StartsWith("P") ? c.P_Docket ?? 0 : t.Consignment_no.StartsWith("E") ? c.E_Docket ?? 0 : t.Consignment_no.StartsWith("I") ? c.I_Docket ?? 0 : t.Consignment_no.StartsWith("V") ? c.V_Docket ?? 0 : t.Consignment_no.StartsWith("N") ? c.N_Docket ?? 0 : 0) + (((t.Amount * (c.Royalty_Charges ?? 0)) / 100) ?? 0)) * (c.Fuel_Sur_Charge / 100) ?? 0)
                            ) +
                           (((t.Amount ?? 0) + (t.Risksurcharge ?? 0) + (((t.Amount * c.Royalty_Charges) / 100) ?? 0) + ((t.Amount + (t.Risksurcharge ?? 0)) + (t.Consignment_no.StartsWith("D") ? c.D_Docket ?? 0 : t.Consignment_no.StartsWith("P") ? c.P_Docket ?? 0 : t.Consignment_no.StartsWith("E") ? c.E_Docket ?? 0 : t.Consignment_no.StartsWith("I") ? c.I_Docket ?? 0 : t.Consignment_no.StartsWith("V") ? c.V_Docket ?? 0 : t.Consignment_no.StartsWith("N") ? c.N_Docket ?? 0 : 0) + (((t.Amount * (c.Royalty_Charges ?? 0)) / 100) ?? 0)) * (c.Fuel_Sur_Charge / 100) ?? 0 + (t.Consignment_no.StartsWith("D") ? c.D_Docket : t.Consignment_no.StartsWith("P") ? c.P_Docket : t.Consignment_no.StartsWith("E") ? c.E_Docket : t.Consignment_no.StartsWith("I") ? c.I_Docket : t.Consignment_no.StartsWith("V") ? c.V_Docket : t.Consignment_no.StartsWith("N") ? c.N_Docket : 0) ?? 0) * 0.18)


                            ),
                            CNote_cost = 0,
                            dtdcamount = t.dtdcamount,

                        }).ToList();








            ExportToExcelAll.ExportToExcelAdmin(list);



            ViewBag.totalamt = list.Sum(b => b.Amount);

            return View(list);



        }

        [HttpGet]
        [PageTitle("SaleReportBeforeInvoice")]
        public ActionResult SaleReportBeforeInvoice()
        {

            //it is pfcode based 
            //   ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");


            List<DisplayPFSum> list = new List<DisplayPFSum>();

            return View(list);
        }

        [HttpPost]
        public ActionResult SaleReportBeforeInvoice(string Fromdatetime, string ToDatetime, string Submit)
        {
            //  ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);
            var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;


            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);

            ViewBag.fromdate = Fromdatetime;


            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.todate = ToDatetime;



            List<DisplayPFSum> Pfsum = new List<DisplayPFSum>();



            Pfsum = (from student in db.Transactions
                     join ab in db.Companies on
                     student.Customer_Id equals ab.Company_Id
                     where (ab.Pf_code == PfCode && student.Pf_Code == PfCode)
                  && student.Customer_Id != null
                  && (student.IsGSTConsignment == null || student.IsGSTConsignment == false)
                        && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(student.Consignment_no)
                            && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(student.Consignment_no)
                     group student by student.Customer_Id into studentGroup

                     select new DisplayPFSum
                     {
                         CustomerId = studentGroup.FirstOrDefault().Customer_Id,
                         CustomerName = (from comp in db.Companies
                                         where comp.Company_Id == studentGroup.FirstOrDefault().Customer_Id
                                         select comp.Company_Name
                                       ).FirstOrDefault(),
                         Sum = db.TransactionViews.Where(m =>
               (m.Customer_Id == studentGroup.Key)
                     && m.status_t == null
                                && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)
                                && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)
                    ).ToList().Where(m => DbFunctions.TruncateTime(m.booking_date) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.booking_date) <= DbFunctions.TruncateTime(todate))
                           .Sum(m => m.Amount + (m.Risksurcharge ?? 0) + (m.loadingcharge ?? 0)) ?? 0,

                         Branchname = db.TransactionViews.Where(m =>
                 (m.Customer_Id == studentGroup.Key)
                       && m.status_t == null
                                && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)
                                && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)
                    ).ToList().Where(m => DbFunctions.TruncateTime(m.booking_date) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.booking_date) <= DbFunctions.TruncateTime(todate))
                           .Count().ToString(),
                         Count = studentGroup.Select(x => x.Consignment_no).Count() != null ? studentGroup.Select(x => x.Consignment_no).Count() : 0,

                     }

                    ).ToList();
            //var pfsumData = (from student in db.Transactions
            //                join ab in db.Companies on student.Customer_Id equals ab.Company_Id
            //                 where ab.Pf_code == PfCode && student.Pf_Code == PfCode
            //                    && student.Customer_Id != null
            //                    && student.status_t!=null
            //                    && (student.IsGSTConsignment == null || student.IsGSTConsignment == false)
            //                    && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(student.Consignment_no)
            //                    && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(student.Consignment_no)
            //                 select new
            //                 {
            //                     CustomerId = student.Customer_Id,
            //                     CustomerName = (from comp in db.Companies
            //                                     where comp.Company_Id == student.Customer_Id
            //                                     select comp.Company_Name).FirstOrDefault(),
            //                    Amount=student.codAmount,
            //                     Risksurcharge=student.Risksurcharge,
            //                     loadingcharge=student.loadingcharge,
            //                     Consignment_no=student.Consignment_no,

            //                 }).Distinct().ToList();

            // Now perform the calculation after retrieving the base data
            // Pfsum = pfsumData.Select(data => new DisplayPFSum
            //{
            //    CustomerId = data.CustomerId,
            //    CustomerName = data.CustomerName,
            //     Sum = db.TransactionViews.Where(m =>
            //   (m.Customer_Id == studentGroup.Key)
            //         && (m.IsGSTConsignment == null || m.IsGSTConsignment == false)
            //                    && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)
            //                    && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)
            //        ).ToList().Where(m => DbFunctions.TruncateTime(m.booking_date) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.booking_date) <= DbFunctions.TruncateTime(todate))
            //               .Sum(m => m.Amount + (m.Risksurcharge ?? 0) + (m.loadingcharge ?? 0)) ?? 0,

            //                  Branchname = db.TransactionViews.Where(m =>
            //          (m.Customer_Id == studentGroup.Key)
            //                && (m.IsGSTConsignment == null || m.IsGSTConsignment == false)
            //                         && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)
            //                         && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)
            //             ).ToList().Where(m => DbFunctions.TruncateTime(m.booking_date) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.booking_date) <= DbFunctions.TruncateTime(todate))
            //                    .Count().ToString(),
            //}).ToList();


            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(Pfsum.Select(m => new { CustomerId = m.CustomerId, Count = m.Count, Total = m.Sum }));
            }
;


            return View(Pfsum);


        }


        public ActionResult PfwiseCreditorsReport()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            List<Invoice> inc = new List<Invoice>();
            return View(inc);
        }

        [HttpPost]
        public ActionResult PfwiseCreditorsReport(string PfCode, string Fromdatetime, string ToDatetime, string status, string Submit)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);
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


            List<Invoice> collectionAmount = new List<Invoice>();

            if (PfCode != null && PfCode != "")
            {
                if (status == "Paid")
                {
                    collectionAmount = (from u in db.Invoices.AsEnumerable()
                                        join c in db.Companies
                                        on u.Customer_Id equals c.Company_Id
                                        where c.Pf_code == PfCode
                                        && u.isDelete == false
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
                                        join c in db.Companies
                                        on u.Customer_Id equals c.Company_Id
                                        where c.Pf_code == PfCode
                                        && u.isDelete == false
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
                                            paid = u.paid ?? 0,
                                            discountamount = u.netamount - (u.paid ?? 0)

                                        }).
                                           ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0 && (x.discountamount > 0 || x.paid == null))
                                               .ToList();  // Discount Amount Is Temporary Column for Checking Balance

                }
                else
                {
                    collectionAmount = (from u in db.Invoices.AsEnumerable()
                                        join c in db.Companies
                                        on u.Customer_Id equals c.Company_Id
                                        where c.Pf_code == PfCode
                                        && u.isDelete == false
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
                                            paid = u.paid ?? 0,
                                            discountamount = u.netamount - (u.paid ?? 0)

                                        }).
                           ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0)
                               .ToList();

                }
            }
            else
            {
                if (status == "Paid")
                {
                    collectionAmount = (from u in db.Invoices.AsEnumerable()
                                        join c in db.Companies
                                        on u.Customer_Id equals c.Company_Id
                                        where u.isDelete == false
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
                                        join c in db.Companies
                                        on u.Customer_Id equals c.Company_Id
                                        where u.isDelete == false
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
                                            paid = u.paid ?? 0,
                                            discountamount = u.netamount - (u.paid ?? 0)

                                        }).
                                           ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0 && (x.discountamount > 0 || x.paid == null))
                                               .ToList();  // Discount Amount Is Temporary Column for Checking Balance

                }
                else
                {
                    collectionAmount = (from u in db.Invoices.AsEnumerable()
                                        join c in db.Companies
                                        on u.Customer_Id equals c.Company_Id
                                        where u.isDelete == false
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
                                            paid = u.paid ?? 0,
                                            discountamount = u.netamount - (u.paid ?? 0)

                                        }).
                           ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0)
                               .ToList();

                }
            }
            if (Submit == "Export to Excel")
            {

                ExportToExcelAll.ExportToExcelAdmin(collectionAmount.Select(m => new { m.invoiceno, m.Customer_Id, m.invoicedate, m.netamount, m.paid, Balance = m.netamount - m.paid }));
            }



            return View(collectionAmount);

        }

        [PageTitle("CreditorsReport")]
        public ActionResult CreditorsReport()
        {
            List<CreditorsInvoiceModel> inc = new List<CreditorsInvoiceModel>();
            // List<Invoice> inc=new List<Invoice>();
            return View(inc);
        }


        [HttpPost]
        public ActionResult CreditorsReport(string Fromdatetime, string ToDatetime, string Custid, string status,string invoicetype, string Submit)
        {

            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
            Request.ApplicationPath.TrimEnd('/');
            DateTime? fromdate = null;
            DateTime? todate = null;

            if (Submit == "Send mail")
            {
                if (Custid == null || Custid == "")
                {
                    ViewBag.fromdate = Fromdatetime;
                    ViewBag.todate = ToDatetime;
                    ViewBag.select = status;
                    ModelState.AddModelError("CustError", "Customer Id is Required");
                    return View();
                }
            }
            ViewBag.select = status;
            ViewBag.selectType = invoicetype;

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

            //List<Invoice> obj = new List<Invoice>();

            List<CreditorsInvoiceModel> newObj = new List<CreditorsInvoiceModel>();
            List<CreditorsInvoiceModel> newObj1 = new List<CreditorsInvoiceModel>();

            // List<Invoice> collectionAmount = new List<Invoice>();
            string customerid = Custid != null ? Custid : null;
            string pfcode = PfCode;

            if (invoicetype == "GST" || invoicetype == "Both")
            {
                if (status == "Paid")
                {

                    newObj = db.getCreditorsInvoiceWithTDSAmount(fromdate, todate, customerid, pfcode).Select(x => new CreditorsInvoiceModel
                    {
                        invoicedate = x.invoicedate,
                        invoiceno = x.invoiceno,
                        periodfrom = x.periodfrom,
                        periodto = x.periodto,
                        total = x.total,
                        fullsurchargetax = x.fullsurchargetax,
                        fullsurchargetaxtotal = x.fullsurchargetaxtotal,
                        servicetax = x.servicetax,
                        servicetaxtotal = x.servicetaxtotal,
                        Customer_Id = x.Customer_Id,
                        CustomerName = db.Companies.Where(m => m.Company_Id == x.Customer_Id).Select(m => m.Company_Name).FirstOrDefault(),

                        netamount = x.netamount,
                        paid = x.paid ?? 0,
                        discountper = x.discountper ?? 0,
                        discountamount = x.discountamount ?? 0,
                        balanceamount = Math.Round((double)x.netamount - (x.paid ?? 0)),
                        TdsAmount = x.TdsAmount ?? 0,
                        TotalAmount = x.TotalAmount ?? 0,
                        InvoiceType = "GST"

                    }).OrderBy(x => x.invoicedate).ToList().Where(x => x.balanceamount <= 0).ToList();


                }

                else if (status == "Unpaid")
                {
                    
                    newObj = db.getCreditorsInvoiceWithTDSAmount(fromdate, todate, customerid, pfcode).Select(x => new CreditorsInvoiceModel
                    {
                        invoicedate = x.invoicedate,
                        invoiceno = x.invoiceno,
                        periodfrom = x.periodfrom,
                        periodto = x.periodto,
                        total = x.total,
                        fullsurchargetax = x.fullsurchargetax,
                        fullsurchargetaxtotal = x.fullsurchargetaxtotal,
                        servicetax = x.servicetax,
                        servicetaxtotal = x.servicetaxtotal,
                        Customer_Id = x.Customer_Id,
                        CustomerName = db.Companies.Where(m => m.Company_Id == x.Customer_Id).Select(m => m.Company_Name).FirstOrDefault(),
                        netamount = x.netamount,
                        paid = x.paid ?? 0,
                        discountper = x.discountper ?? 0,
                        discountamount = x.discountamount ?? 0,
                        balanceamount = Math.Round((double)x.netamount - (x.paid ?? 0)),
                        TdsAmount = x.TdsAmount ?? 0,
                        TotalAmount = x.TotalAmount ?? 0,
                        InvoiceType = "GST"

                    }).OrderBy(x => x.invoicedate).ToList().Where(x => x.balanceamount > 0 || x.paid == null).ToList();


                }
                else
                {
                    
                    newObj = db.getCreditorsInvoiceWithTDSAmount(fromdate, todate, customerid, pfcode)
                        .Select(x => new CreditorsInvoiceModel
                        {
                            invoicedate = x.invoicedate,
                            invoiceno = x.invoiceno,
                            periodfrom = x.periodfrom,
                            periodto = x.periodto,
                            total = x.total,
                            fullsurchargetax = x.fullsurchargetax,
                            fullsurchargetaxtotal = x.fullsurchargetaxtotal,
                            servicetax = x.servicetax,
                            servicetaxtotal = x.servicetaxtotal,
                            Customer_Id = x.Customer_Id,
                            CustomerName = db.Companies.Where(m => m.Company_Id == x.Customer_Id).Select(m => m.Company_Name).FirstOrDefault(),
                            discountper = x.discountper ?? 0,
                            discountamount = x.discountamount ?? 0,
                            netamount = x.netamount ?? 0,
                            paid = x.paid ?? 0,
                            balanceamount = Math.Round((double)x.netamount - (x.paid ?? 0)),
                            TdsAmount = x.TdsAmount ?? 0,
                            TotalAmount = x.TotalAmount ?? 0,
                            InvoiceType = "GST"
                        }).OrderBy(x => x.invoicedate).ToList();
                }

            }
            if(invoicetype == "NonGST" || invoicetype == "Both")
            {
                newObj1 = (from item in db.GSTInvoices
                               where item.Pfcode == pfcode && (customerid == "" || item.Customer_Id.Contains(customerid)) &&
                               (DbFunctions.TruncateTime(item.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(item.invoicedate) <= DbFunctions.TruncateTime(todate))
                               select new CreditorsInvoiceModel
                               {
                                   invoicedate = item.invoicedate,
                                   invoiceno = item.invoiceno,
                                   periodfrom = item.periodfrom,
                                   periodto = item.periodto,
                                   total = item.total,
                                   fullsurchargetax = item.fullsurchargetax,
                                   fullsurchargetaxtotal = item.fullsurchargetaxtotal,
                                   servicetax = item.servicetax,
                                   servicetaxtotal = item.servicetaxtotal,
                                   Customer_Id = item.Customer_Id,
                                   CustomerName = db.Companies.Where(m => m.Company_Id == item.Customer_Id).Select(m => m.Company_Name).FirstOrDefault(),
                                   discountper = item.discountper ?? 0,
                                   discountamount = item.discountamount ?? 0,
                                   netamount = item.netamount ?? 0,
                                   paid = item.paid ?? 0,
                                   balanceamount = Math.Round((double)item.netamount - (item.paid ?? 0)),
                                   TdsAmount = 0,
                                   TotalAmount = item.FinalNetAmount ?? 0,
                                   InvoiceType = "Non-GST"
                               }).ToList();
            }
            if (invoicetype == "Both")
            {  
                newObj.AddRange(newObj1);
            }
            if (Submit == "Export to Excel")
            {
               
                var data = newObj.Select(x => new
                {
                    InvoiceDate = x.invoicedate.Value.ToString("dd-MM-yyyy"),
                    InvoiceNo = x.invoiceno,
                    PeriodFrom = x.periodfrom.Value.ToString("dd-MM-yyyy"),
                    PeriodTo = x.periodto.Value.ToString("dd-MM-yyyy"),
                    Total = x.total,
                    FullSurchargeTax = x.fullsurchargetax,
                    FullSurChargeTotal = x.fullsurchargetaxtotal,
                    ServiceTax = x.servicetax,
                    ServiceTaxTotal = x.servicetaxtotal,
                    DiscountPer = x.discountper,
                    DiscountAmount = x.discountamount,
                    CustomerId = x.Customer_Id,
                    NetAmount = x.netamount ?? 0,
                    Paid = x.paid ?? 0,

                    Balance = x.balanceamount,
                    //   TDSAmount = x.TotalAmount ?? 0,
                    //  TotalAmount = x.TotalAmount ?? 0

                }).OrderBy(x=>x.InvoiceDate).ToList();

                if (newObj.Count() <= 0 || newObj == null)
                {
                    ViewBag.Nodata = "No Data Found";
                }
                else
                {
                    ExportToExcelAll.ExportToExcelAdmin(data);

                }
            }


            if (Submit == "Print Without GST")
            {
                newObj = db.getCreditorsInvoiceWithTDSWithoutGSTAmount(fromdate, todate, customerid, pfcode).Select(x => new CreditorsInvoiceModel
                {
                    invoicedate = x.invoicedate,
                    invoiceno = x.invoiceno,
                    periodfrom = x.periodfrom,
                    periodto = x.periodto,
                    total = x.total,
                    fullsurchargetax = x.fullsurchargetax,
                    fullsurchargetaxtotal = x.fullsurchargetaxtotal,
                    servicetax = x.servicetax,
                    servicetaxtotal = x.servicetaxtotal,
                    Customer_Id = x.Customer_Id,
                    CustomerName = db.Companies.Where(m => m.Company_Id == x.Customer_Id).Select(m => m.Company_Name).FirstOrDefault(),

                    netamount = x.netamount,
                    paid = x.paid ?? 0,
                    discountper = x.discountper ?? 0,
                    discountamount = x.discountamount ?? 0,
                    balanceamount = Math.Round((double)x.netamount - (x.paid ?? 0)),
                    TdsAmount = x.TdsAmount ?? 0,
                    TotalAmount = x.TotalAmount ?? 0

                }).OrderBy(x => x.invoicedate).ToList();

                var DataSet1 = newObj.Where(x => customerid == x.Customer_Id).OrderBy(x => x.invoicedate).ToList();
                if (DataSet1.Count() > 0)
                {
                    var DataSet2 = db.Companies.Where(m => m.Company_Id == Custid).ToList();
                    var pfcode1 = DataSet2.FirstOrDefault().Pf_code;
                    var DataSet3 = db.Franchisees.Where(m => m.PF_Code == pfcode1).ToList();
                    DataSet3.FirstOrDefault().LogoFilePath = (DataSet3.FirstOrDefault().LogoFilePath == null || DataSet3.FirstOrDefault().LogoFilePath == "") ? baseurl + "/assets/Dtdclogo.png" : DataSet3.FirstOrDefault().LogoFilePath;

                    LocalReport lr = new LocalReport();

                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PaymentOutstanding.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }


                    ReportDataSource rd1 = new ReportDataSource("DataSet3", DataSet3);
                    ReportDataSource rd2 = new ReportDataSource("DataSet1", DataSet1);
                    ReportDataSource rd3 = new ReportDataSource("DataSet2", DataSet2);
                    ReportDataSource rd4 = new ReportDataSource("DataSet4", DataSet1);

                    lr.DataSources.Add(rd1);
                    lr.DataSources.Add(rd2);
                    lr.DataSources.Add(rd3);
                    lr.DataSources.Add(rd4);
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


                    string savePath = Server.MapPath("~/PDF/" + Custid + "-PaymentOutstanding.pdf");
                    using (FileStream stream = new FileStream(savePath, FileMode.Create))
                    {
                        stream.Write(renderByte, 0, renderByte.Length);
                    }
                    return File(renderByte, mimeType);
                }

            }

            if (Submit == "Print" || Submit == "Send mail")
            {
                if (Custid != null && Custid != "")
                {
                    var DataSet1 = newObj.Where(x => customerid == x.Customer_Id).OrderBy(x => x.invoicedate).ToList();
                    if (DataSet1.Count() > 0)
                    {
                        var DataSet2 = db.Companies.Where(m => m.Company_Id == Custid).ToList();
                        var pfcode1 = DataSet2.FirstOrDefault().Pf_code;
                        var DataSet3 = db.Franchisees.Where(m => m.PF_Code == pfcode1).ToList();//Remove static url https://frbilling.com
                        DataSet3.FirstOrDefault().LogoFilePath = (DataSet3.FirstOrDefault().LogoFilePath == null || DataSet3.FirstOrDefault().LogoFilePath == "") ? baseurl + "/assets/Dtdclogo.png" : DataSet3.FirstOrDefault().LogoFilePath;

                        LocalReport lr = new LocalReport();

                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PaymentOutstanding.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }


                        ReportDataSource rd1 = new ReportDataSource("DataSet3", DataSet3);
                        ReportDataSource rd2 = new ReportDataSource("DataSet1", DataSet1);
                        ReportDataSource rd3 = new ReportDataSource("DataSet2", DataSet2);
                        ReportDataSource rd4 = new ReportDataSource("DataSet4", DataSet1);

                        lr.DataSources.Add(rd1);
                        lr.DataSources.Add(rd2);
                        lr.DataSources.Add(rd3);
                        lr.DataSources.Add(rd4);
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


                        string savePath = Server.MapPath("~/PDF/" + Custid + "-PaymentOutstanding.pdf");
                        using (FileStream stream = new FileStream(savePath, FileMode.Create))
                        {
                            stream.Write(renderByte, 0, renderByte.Length);
                        }



                        if (Submit == "Send mail")
                        {
                            try
                            {
                                if (DataSet2.FirstOrDefault().Email != null || DataSet2.FirstOrDefault().Email != "")
                                {
                                    MemoryStream memoryStream = new MemoryStream(renderByte);



                                    using (MailMessage mm = new MailMessage(DataSet3.FirstOrDefault().Sendermail, DataSet2.FirstOrDefault().Email))
                                    {
                                        mm.Subject = "Payment Outstanding from " + Fromdatetime + " to " + ToDatetime;

                                        string Bodytext = "<html><body>Please Find Attachment</body></html>";
                                        Attachment attachment = new Attachment(memoryStream, "PaymentOutstanding.pdf");

                                        mm.IsBodyHtml = true;



                                        mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                                        AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                                        // mm.Body = Bodytext;
                                        mm.Body = Bodytext;

                                        //Add Byte array as Attachment.

                                        mm.Attachments.Add(attachment);

                                        SmtpClient smtp = new SmtpClient();
                                        smtp.Host = "smtp.gmail.com";
                                        smtp.EnableSsl = true;
                                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                                        credentials.UserName = DataSet3.FirstOrDefault().Sendermail;
                                        credentials.Password = DataSet3.FirstOrDefault().password;
                                        smtp.UseDefaultCredentials = true;
                                        smtp.Credentials = credentials;
                                        smtp.Port = 587;
                                        smtp.Send(mm);
                                    }
                                }
                                {
                                    TempData["error"] = "Select Company";
                                    return View(newObj);

                                }
                            }
                            catch (Exception ex)
                            {

                                TempData["error"] = "Something Went Wrong To send the E-mail,Please Check Your Franchisee E-Mail Id and Company E-Mail Id";
                                return View(newObj);
                            }

                        }

                        return File(renderByte, mimeType);
                    }
                    else
                    {
                        ViewBag.Nodata = "No Data Found";
                    }

                }
                else
                {
                    var DataSet1 = newObj.OrderBy(x => x.invoiceno).ToList();
                    if (DataSet1.Count() > 0)
                    {
                        LocalReport lr = new LocalReport();

                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PaymentOutstandingWComppany.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }


                        ReportDataSource rd2 = new ReportDataSource("DataSet1", DataSet1);
                        ReportDataSource rd4 = new ReportDataSource("DataSet4", DataSet1);

                        lr.DataSources.Add(rd4);
                        lr.DataSources.Add(rd2);

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


                        string savePath = Server.MapPath("~/PDF/" + Custid + "-PaymentOutstanding.pdf");
                        using (FileStream stream = new FileStream(savePath, FileMode.Create))
                        {
                            stream.Write(renderByte, 0, renderByte.Length);
                        }





                        return File(renderByte, mimeType);

                    }
                    else
                    {
                        TempData["error"] = "Dat Not Found";
                    }
                }
            }

            ViewBag.NetAmountSum = newObj.Distinct().Select(x => new { x.netamount, x.invoiceno }).GroupBy(x => x.invoiceno).Sum(x => x.FirstOrDefault().netamount);
            ViewBag.Balance = newObj.Distinct().Select(x => new { x.balanceamount, x.invoiceno }).GroupBy(x => x.invoiceno).Sum(x => x.FirstOrDefault().balanceamount);
            ViewBag.Paid = newObj.Distinct().Select(x => new { x.paid, x.invoiceno }).GroupBy(x => x.invoiceno).Sum(x => x.FirstOrDefault().paid);
            return View(newObj);
        }


        [PageTitle("BusinessAnalysis")]
        public ActionResult BusinessAnalysis()
        {
            List<TransactionView> list = new List<TransactionView>();

            return View(list);

        }

        [HttpPost]
        public ActionResult BusinessAnalysis(string Fromdatetime, string ToDatetime, string Custid)
        {
            var Pfcode = CommonFunctions.getSessionPfcode();

            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

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

            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }



            List<TransactionView> transactions =
                db.TransactionViews.Where(m =>
               (string.IsNullOrEmpty(Custid) || m.Customer_Id == Custid)
               && m.Pf_Code == pfcode
               && m.booking_date.HasValue && m.booking_date.Value >= fromdate.Value && m.booking_date.Value <= todate.Value

                    ).ToList().OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                           .ToList();
            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            return View(transactions);
        }
        [HttpGet]
        public ActionResult EmployeeWiseConsigmentReport()
        {
            var st = db.Issues.ToList();

            List<string> str = new List<string>();


            foreach (var j in st)
            {

                int counter = 0;

                char stch = j.startno[0];
                char Endch = j.endno[0];

                long startConsignment = Convert.ToInt64(j.startno.Substring(1));
                long EndConsignment = Convert.ToInt64(j.endno.Substring(1));



                for (long i = startConsignment; i <= EndConsignment; i++)
                {
                    string updateconsignment = stch + i.ToString();


                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment && m.isDelete == false).FirstOrDefault();


                    if (transaction != null && transaction.Customer_Id != null && transaction.Customer_Id.Length > 1)
                    {
                        counter++;
                    }


                }


                str.Add(counter.ToString());
                counter = 0;




            }

            ViewBag.str = str.ToArray();

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View(st);

        }


        [HttpPost]
        public ActionResult EmployeeWiseConsigmentReport(string PfCode)
        {
            var st = db.Issues.Where(m => m.Pf_code == PfCode || PfCode == "").ToList();

            List<string> str = new List<string>();


            foreach (var j in st)
            {

                int counter = 0;

                char stch = j.startno[0];
                char Endch = j.endno[0];

                long startConsignment = Convert.ToInt64(j.startno.Substring(1));
                long EndConsignment = Convert.ToInt64(j.endno.Substring(1));



                for (long i = startConsignment; i <= EndConsignment; i++)
                {
                    string updateconsignment = stch + i.ToString();


                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment && m.isDelete == false).FirstOrDefault();


                    if (transaction != null && transaction.Customer_Id != null && transaction.Customer_Id.Length > 1)
                    {
                        counter++;
                    }


                }


                str.Add(counter.ToString());
                counter = 0;




            }

            ViewBag.str = str.ToArray();

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);
            return View(st);

        }


        public ActionResult MemberShipreport(string ToDatetime, string Fromdatetime, string Submit, string pfcode = "")
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);


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






            var tmpItem = (from item in db.wallet_History
                           where item.PF_Code == pfcode || pfcode == "" && (DbFunctions.TruncateTime(item.datetime) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(item.datetime) <= DbFunctions.TruncateTime(todate))
                           group item by item.mobile_no into g
                           select new WalletReport
                           {

                               Total_Wallet_Money = g.Where(item => item.H_Status == "Added").Select(m => m.Amount).Sum(),
                               Total_Redeemed = g.Where(item => item.H_Status == "Redeemed").Select(m => m.Amount).Sum() ?? 0,
                               No_Of_Bookings = g.Count(),
                               // Balance = (g.Where(item => item.H_Status == "Added").Select(m => m.Amount).Sum()  - g.Where(item => item.H_Status == "Redeemed").Select(m => m.Amount).Sum() ?? 0),
                               Mobile_No = g.Key,
                               Name = g.Select(m => m.PF_Code).FirstOrDefault(),

                               //Amount = g.Sum(item => item.Amount), <-- we can also do like that


                           }).ToList();


            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(tmpItem);
            }

            return View(tmpItem);
        }

        [HttpGet]
        public ActionResult MembershipPfWiseReport(string ToDatetime, string Fromdatetime, string pfcode = "")
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


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



            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);

            var tmpItem = (from item in db.wallet_History
                           where item.PF_Code == pfcode || pfcode == "" && (DbFunctions.TruncateTime(item.datetime) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(item.datetime) <= DbFunctions.TruncateTime(todate))
                           group item by item.mobile_no into g

                           select new WalletReport
                           {

                               Total_Wallet_Money = g.Where(item => item.H_Status == "Added").Select(m => m.Amount).Sum(),
                               Total_Redeemed = g.Where(item => item.H_Status == "Redeemed").Select(m => m.Amount).Sum() ?? 0,
                               No_Of_Bookings = g.Count(),
                               // Balance = (g.Where(item => item.H_Status == "Added").Select(m => m.Amount).Sum()  - g.Where(item => item.H_Status == "Redeemed").Select(m => m.Amount).Sum() ?? 0),
                               Mobile_No = g.Key,
                               Name = g.Select(m => m.PF_Code).FirstOrDefault(),

                               //Amount = g.Sum(item => item.Amount), <-- we can also do like that


                           }).ToList();

            return View(tmpItem);
        }

        [HttpGet]
        public ActionResult PFwisemembershipsummary(string ToDatetime, string Fromdatetime, string pfcode, string Submit)
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", pfcode);

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





            var tmpItem = (from item in db.wallet_History
                           where item.PF_Code == pfcode || pfcode == "" && (DbFunctions.TruncateTime(item.datetime) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(item.datetime) <= DbFunctions.TruncateTime(todate))
                           group item by item.PF_Code into g
                           select new WalletReport
                           {

                               Total_Wallet_Money = g.Where(item => item.H_Status == "Added").Select(m => m.Amount).Sum(),
                               Total_Redeemed = g.Where(item => item.H_Status == "Redeemed").Select(m => m.Amount).Sum() ?? 0,
                               No_Of_Bookings = g.Count(),
                               // Balance = (g.Where(item => item.H_Status == "Added").Select(m => m.Amount).Sum()  - g.Where(item => item.H_Status == "Redeemed").Select(m => m.Amount).Sum() ?? 0),
                               Mobile_No = g.Key,
                               Name = g.Select(m => m.PF_Code).FirstOrDefault(),

                               //Amount = g.Sum(item => item.Amount), <-- we can also do like that


                           }).ToList();


            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(tmpItem);
            }
            return View(tmpItem);
        }

        public ActionResult Destinations()
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }


            var list = (from user in db.Transactions
                        where !db.Destinations.Any(f => f.Pincode == user.Pincode)
                        && user.Pf_Code == pfcode
                        && user.isDelete == false
                        select user).ToList();

            return View(list);
        }

        public ActionResult InvalidConsignment()
        {
            //string pfcode = Session["pfCode"].ToString();
            string pfcode = Request.Cookies["Cookies"]["pfCode"].ToString();
            var list = (from user in db.Transactions
                        where !db.Companies.Any(f => f.Company_Id == user.Customer_Id) && user.Customer_Id != null
                        && user.Pf_Code == pfcode
                        && user.isDelete == false
                        select user).ToList();

            return View(list);
        }

        public ActionResult ViewAllDestinationReport(string Fromdatetime, string ToDatetime, string PfCode = "")
        {
            //PfCode=Session["pfCode"].ToString();
            PfCode = Request.Cookies["Cookies"]["pfCode"].ToString();

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            //ViewBag.PfCode = Session["pfCode"].ToString();//new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);
            ViewBag.PfCode = Request.Cookies["Cookies"]["pfCode"].ToString();
            if (Fromdatetime != null && ToDatetime != null)
            {

                DateTime? fromdate;
                DateTime? todate;


                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;



                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;






                var results = (from p in db.Receipt_details
                               where p.Pf_Code == PfCode || PfCode == ""
                               && ((DbFunctions.TruncateTime(p.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) || Fromdatetime == null) && (DbFunctions.TruncateTime(p.Datetime_Cons) <= DbFunctions.TruncateTime(todate) || ToDatetime == null))
                               group p by p.Destination into g
                               orderby g.Count() descending
                               select new ConsignmentCount
                               {

                                   Destination = g.Key,
                                   Count = g.Count()
                               });
                return View(results);
            }
            else
            {
                var results = (from p in db.Receipt_details
                               where p.Pf_Code == PfCode || PfCode == ""
                               group p by p.Destination into g
                               orderby g.Count() descending
                               select new ConsignmentCount
                               {

                                   Destination = g.Key,
                                   Count = g.Count()
                               });
                return View(results);
            }




        }

        public ActionResult ViewAllProductReport(string Fromdatetime, string ToDatetime, string PfCode = "")
        {
            //PfCode = Session["pfCode"].ToString();
            PfCode = Request.Cookies["Cookies"]["pfCode"].ToString();

            List<ConsignmentCount> Consignmentcount = new List<ConsignmentCount>();

            ViewBag.PfCode = PfCode;//new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);
            if (Fromdatetime != null && ToDatetime != null)
            {

                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};




                DateTime? fromdate;
                DateTime? todate;


                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;



                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;



                ConsignmentCount consptp = new ConsignmentCount();

                consptp.Destination = "PTP";
                consptp.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("E") && (m.Pf_Code == PfCode || PfCode == "") && (DbFunctions.TruncateTime(m.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) || Fromdatetime == null) && (DbFunctions.TruncateTime(m.Datetime_Cons) <= DbFunctions.TruncateTime(todate) || ToDatetime == null)).Count();

                Consignmentcount.Add(consptp);

                ConsignmentCount consPlus = new ConsignmentCount();

                consPlus.Destination = "Plus";
                consPlus.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("V") && (m.Pf_Code == PfCode || PfCode == "") && (DbFunctions.TruncateTime(m.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) || Fromdatetime == null) && (DbFunctions.TruncateTime(m.Datetime_Cons) <= DbFunctions.TruncateTime(todate) || ToDatetime == null)).Count();

                Consignmentcount.Add(consPlus);


                ConsignmentCount consInternational = new ConsignmentCount();

                consInternational.Destination = "International";
                consInternational.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("N") && (m.Pf_Code == PfCode || PfCode == "") && (DbFunctions.TruncateTime(m.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) || Fromdatetime == null) && (DbFunctions.TruncateTime(m.Datetime_Cons) <= DbFunctions.TruncateTime(todate) || ToDatetime == null)).Count();

                Consignmentcount.Add(consInternational);


                ConsignmentCount consDox = new ConsignmentCount();

                consDox.Destination = "Standart";
                consDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("P") && (m.Pf_Code == PfCode || PfCode == "") && (DbFunctions.TruncateTime(m.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) || Fromdatetime == null) && (DbFunctions.TruncateTime(m.Datetime_Cons) <= DbFunctions.TruncateTime(todate) || ToDatetime == null)).Count();

                Consignmentcount.Add(consDox);


                ConsignmentCount consNonDox = new ConsignmentCount();

                consNonDox.Destination = "Non Dox";
                consNonDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("D") && (m.Pf_Code == PfCode || PfCode == "") && (DbFunctions.TruncateTime(m.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) || Fromdatetime == null) && (DbFunctions.TruncateTime(m.Datetime_Cons) <= DbFunctions.TruncateTime(todate) || ToDatetime == null)).Count();


                ConsignmentCount consNonVas = new ConsignmentCount();

                consNonVas.Destination = "VAS";
                consNonVas.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("I") && (m.Pf_Code == PfCode || PfCode == "") && (DbFunctions.TruncateTime(m.Datetime_Cons) >= DbFunctions.TruncateTime(fromdate) || Fromdatetime == null) && (DbFunctions.TruncateTime(m.Datetime_Cons) <= DbFunctions.TruncateTime(todate) || ToDatetime == null)).Count();

                Consignmentcount.Add(consNonVas);



                Consignmentcount.Add(consNonDox);

                return View(Consignmentcount);
            }
            else
            {
                ConsignmentCount consptp = new ConsignmentCount();

                consptp.Destination = "PTP";
                consptp.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("E")).Count();

                Consignmentcount.Add(consptp);

                ConsignmentCount consPlus = new ConsignmentCount();

                consPlus.Destination = "Plus";
                consPlus.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("V")).Count();

                Consignmentcount.Add(consPlus);


                ConsignmentCount consInternational = new ConsignmentCount();

                consInternational.Destination = "International";
                consInternational.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("N")).Count();

                Consignmentcount.Add(consInternational);


                ConsignmentCount consDox = new ConsignmentCount();

                consDox.Destination = "Standard";
                consDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("P")).Count();

                Consignmentcount.Add(consDox);


                ConsignmentCount consNonDox = new ConsignmentCount();

                consNonDox.Destination = "Non Dox";
                consNonDox.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("D")).Count();

                Consignmentcount.Add(consNonDox);


                ConsignmentCount consNonVas = new ConsignmentCount();

                consNonVas.Destination = "VAS";
                consNonVas.Count = db.Receipt_details.Where(m => m.Consignment_No.StartsWith("I")).Count();

                Consignmentcount.Add(consNonVas);

                return View(Consignmentcount);
            }

        }

        [HttpGet]
        public ActionResult PfwiseTaxReport()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            List<InvoiceAndCompany> list = new List<InvoiceAndCompany>();

            return View(list);

        }

        [HttpPost]
        public ActionResult PfwiseTaxReport(string PfCode, string ToDatetime, string Fromdatetime, string Submit, string Tallyexcel)
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;


            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);

            ViewBag.fromdate = Fromdatetime;


            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.todate = ToDatetime;

            if (PfCode != null && PfCode != "")
            {
                List<InvoiceAndCompany> list =
                list = (from i in db.Invoices
                        join c in db.Companies
                        on i.Customer_Id equals c.Company_Id
                        join f in db.Franchisees
                        on c.Pf_code equals f.PF_Code

                        where
                            (c.Pf_code == PfCode) &&
                            i.isDelete == false &&
                            DbFunctions.TruncateTime(i.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(i.invoicedate) <= DbFunctions.TruncateTime(todate)


                        select new InvoiceAndCompany
                        {
                            invoiceno = i.invoiceno,
                            invoicedate = i.invoicedate,
                            periodfrom = i.periodfrom,
                            periodto = i.periodto,
                            total = i.total,
                            fullsurchargetax = i.fullsurchargetax,
                            fullsurchargetaxtotal = i.fullsurchargetaxtotal,
                            servicetax = i.servicetax,
                            servicetaxtotal = i.servicetaxtotal,
                            othercharge = i.othercharge,
                            netamount = i.netamount,
                            Customer_Id = i.Customer_Id,
                            fid = i.fid,
                            servicecharges = i.servicecharges,
                            Royalty_charges = i.Royalty_charges,
                            Docket_charges = i.Docket_charges,
                            Tempdatefrom = i.Tempdatefrom,
                            TempdateTo = i.TempdateTo,
                            tempInvoicedate = i.tempInvoicedate,
                            Company_Name = c.Company_Name,
                            Gst_No = c.Gst_No,
                            Fr_Gst_No = f.GstNo,
                            CgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                            SgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                            IgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0) : 0) : 0,

                            CgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                            SgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                            IgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0) : 0) : 0,

                        }).ToList();



                if (Submit == "Export to Excel")
                {
                    var list1 = (from i in db.Invoices
                                 join c in db.Companies
                                 on i.Customer_Id equals c.Company_Id
                                 join f in db.Franchisees
                                 on c.Pf_code equals f.PF_Code

                                 where
                                     (c.Pf_code == PfCode) &&
                                     i.isDelete == false &&
                                     DbFunctions.TruncateTime(i.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(i.invoicedate) <= DbFunctions.TruncateTime(todate)


                                 select new
                                 {
                                     invoiceno = i.invoiceno,
                                     invoicedate = i.tempInvoicedate,
                                     periodfrom = i.Tempdatefrom,
                                     periodto = i.TempdateTo,
                                     total = i.total,
                                     fullsurchargetax = i.fullsurchargetax,
                                     fullsurchargetaxtotal = i.fullsurchargetaxtotal,
                                     //servicetax = i.servicetax,
                                     //servicetaxtotal = i.servicetaxtotal,
                                     //othercharge = i.othercharge,
                                     netamount = i.netamount,
                                     Customer_Id = i.Customer_Id,
                                     // fid = i.fid,
                                     // servicecharges = i.servicecharges,
                                     Royalty_charges = i.Royalty_charges,
                                     Docket_charges = i.Docket_charges,

                                     Company_Name = c.Company_Name,
                                     Gst_No = c.Gst_No,
                                     // Fr_Gst_No = f.GstNo,
                                     CgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                     SgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                     IgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0) : 0) : 0,

                                     CgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                                     SgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                                     IgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0) : 0) : 0,

                                 }).ToList();
                    if (list1.Count() <= 0 || list1 == null)
                    {
                        ViewBag.Nodata = "No Data Found";
                    }
                    else
                    {
                        ExportToExcelAll.ExportToExcelAdmin(list1);

                    }
                }

                if (Tallyexcel == "Tally excel")
                {
                    var list1 = (from i in db.Invoices
                                 join c in db.Companies
                                 on i.Customer_Id equals c.Company_Id
                                 join f in db.Franchisees
                                 on c.Pf_code equals f.PF_Code

                                 where
                                     (c.Pf_code == PfCode) &&
                                     i.isDelete == false &&
                                     DbFunctions.TruncateTime(i.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(i.invoicedate) <= DbFunctions.TruncateTime(todate)
                                 select new
                                 {
                                     Vch_No = i.invoiceno,
                                     Vch_Type = "Sales",
                                     Date = i.tempInvoicedate,

                                     Reference_No = i.invoiceno,
                                     Party_Name = i.Customer_Id,
                                     Ledger_Group = "Sundry Debtors",
                                     Registration_Type = "Regular",
                                     GstNo = c.Gst_No,
                                     Country = "India",
                                     State = "Maharashtra",
                                     Pincode = "400013",
                                     Address_1 = c.Company_Address,
                                     Address_2 = "",
                                     Address_3 = "",
                                     Sales_Ledger = "Advertising Service",
                                     Amt = i.discountamount > 0 ? i.total + i.fullsurchargetaxtotal + i.Royalty_charges + i.Docket_charges + i.discountamount : i.total + i.fullsurchargetaxtotal + i.Royalty_charges,
                                     Additional_Ledger = "Discount",
                                     Amount = i.discountamount > 0 ? "-" + i.discountamount.ToString() : null,
                                     CGST_Ledger = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                     //CGST_Amt = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0,
                                     CGST_Amt = Math.Round((double)(i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0), 2),

                                     SGST_Ledger = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                     // SGST_Amt = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0,
                                     SGST_Amt = Math.Round((double)(i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0), 2),
                                     IGST_Ledger = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0) : 0) : 0,
                                     //  IGST_Amt = c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetax) : 0,
                                     IGST_Amt = Math.Round((double)(i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0) : 0) : 0), 2),
                                     CESS_Ledger = "",
                                     //Round_off=0,

                                     Total = Math.Round((double)i.netamount, 2),
                                     Narration = "COURIER CHARGES MONTH FROM " + fromdate + " TO " + todate,
                                     TALLYIMPORTSTATUS = "",


                                 }).ToList();
                    if (list1.Count() <= 0 || list1 == null)
                    {
                        ViewBag.Nodata = "No Data Found";

                    }
                    else
                    {
                        ExportToExcelAll.ExportToExcelAdmin(list1);

                    }
                }

                return View(list);
            }
            else
            {
                List<InvoiceAndCompany> list =
                list = (from i in db.Invoices
                        join c in db.Companies
                        on i.Customer_Id equals c.Company_Id
                        join f in db.Franchisees
                        on c.Pf_code equals f.PF_Code

                        where
                        i.isDelete == false &&
                            DbFunctions.TruncateTime(i.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(i.invoicedate) <= DbFunctions.TruncateTime(todate)


                        select new InvoiceAndCompany
                        {
                            invoiceno = i.invoiceno,
                            invoicedate = i.invoicedate,
                            periodfrom = i.periodfrom,
                            periodto = i.periodto,
                            total = i.total,
                            fullsurchargetax = i.fullsurchargetax,
                            fullsurchargetaxtotal = i.fullsurchargetaxtotal,
                            servicetax = i.servicetax,
                            servicetaxtotal = i.servicetaxtotal,
                            othercharge = i.othercharge,
                            netamount = i.netamount,
                            Customer_Id = i.Customer_Id,
                            fid = i.fid,
                            servicecharges = i.servicecharges,
                            Royalty_charges = i.Royalty_charges,
                            Docket_charges = i.Docket_charges,
                            Tempdatefrom = i.Tempdatefrom,
                            TempdateTo = i.TempdateTo,
                            tempInvoicedate = i.tempInvoicedate,
                            Company_Name = c.Company_Name,
                            Gst_No = c.Gst_No,
                            Fr_Gst_No = f.GstNo,
                            CgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                            SgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                            IgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0) : 0) : 0,

                            CgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                            SgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                            IgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0) : 0) : 0,


                        }).ToList();



                if (Submit == "Export to Excel")
                {
                    var list1 = (from i in db.Invoices
                                 join c in db.Companies
                                 on i.Customer_Id equals c.Company_Id
                                 join f in db.Franchisees
                                 on c.Pf_code equals f.PF_Code

                                 where
                                 i.isDelete == false &&
                                     DbFunctions.TruncateTime(i.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(i.invoicedate) <= DbFunctions.TruncateTime(todate)


                                 select new
                                 {
                                     invoiceno = i.invoiceno,
                                     invoicedate = i.tempInvoicedate,
                                     periodfrom = i.Tempdatefrom,
                                     periodto = i.TempdateTo,
                                     total = i.total,
                                     fullsurchargetax = i.fullsurchargetax,
                                     fullsurchargetaxtotal = i.fullsurchargetaxtotal,
                                     //servicetax = i.servicetax,
                                     //servicetaxtotal = i.servicetaxtotal,
                                     //othercharge = i.othercharge,
                                     netamount = i.netamount,
                                     Customer_Id = i.Customer_Id,
                                     // fid = i.fid,
                                     // servicecharges = i.servicecharges,
                                     Royalty_charges = i.Royalty_charges,
                                     Docket_charges = i.Docket_charges,

                                     Company_Name = c.Company_Name,
                                     Gst_No = c.Gst_No,
                                     // Fr_Gst_No = f.GstNo,
                                     CgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                     SgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                     IgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0) : 0) : 0,

                                     CgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                                     SgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                                     IgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0) : 0) : 0,


                                 }).ToList();
                    if (list1.Count() <= 0 || list1 == null)
                    {
                        ViewBag.Nodata = "No Data Found";
                    }
                    else
                    {
                        ExportToExcelAll.ExportToExcelAdmin(list1);

                    }
                }


                if (Tallyexcel == "Tally excel")
                {
                    var list1 = (from i in db.Invoices
                                 join c in db.Companies
                                 on i.Customer_Id equals c.Company_Id
                                 join f in db.Franchisees
                                 on c.Pf_code equals f.PF_Code

                                 where
                                 i.isDelete == false &&
                                     DbFunctions.TruncateTime(i.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(i.invoicedate) <= DbFunctions.TruncateTime(todate)

                                 select new
                                 {
                                     Vch_No = i.invoiceno,
                                     Vch_Type = "Sales",
                                     Date = i.tempInvoicedate,

                                     Reference_No = i.invoiceno,
                                     Party_Name = i.Customer_Id,
                                     Ledger_Group = "Sundry Debtors",
                                     Registration_Type = "Regular",
                                     GstNo = c.Gst_No,
                                     Country = "India",
                                     State = "Maharashtra",
                                     Pincode = "400013",
                                     Address_1 = c.Company_Address,
                                     Address_2 = "",
                                     Address_3 = "",
                                     Sales_Ledger = "Advertising Service",
                                     Amt = i.discountamount > 0 ? i.total + i.fullsurchargetaxtotal + i.Royalty_charges + i.Docket_charges + i.discountamount : i.total + i.fullsurchargetaxtotal + i.Royalty_charges,
                                     Additional_Ledger = "Discount",
                                     Amount = i.discountamount > 0 ? "-" + i.discountamount.ToString() : null,
                                     CGST_Ledger = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                     //CGST_Amt = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0,
                                     CGST_Amt = Math.Round((double)(i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0), 2),

                                     SGST_Ledger = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                     // SGST_Amt = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0,
                                     SGST_Amt = Math.Round((double)(i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0), 2),
                                     IGST_Ledger = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0) : 0) : 0,
                                     //  IGST_Amt = c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetax) : 0,
                                     IGST_Amt = Math.Round((double)(i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0) : 0) : 0), 2),
                                     CESS_Ledger = "",
                                     //Round_off=0,

                                     Total = Math.Round((double)i.netamount, 2),
                                     Narration = "COURIER CHARGES MONTH FROM " + fromdate + " TO " + todate,
                                     TALLYIMPORTSTATUS = "",


                                 }).ToList();
                    if (list1.Count() <= 0 || list1 == null)
                    {
                        ViewBag.Nodata = "No Data Found";

                    }
                    else
                    {
                        ExportToExcelAll.ExportToExcelAdmin(list1);

                    }
                }


                return View(list);
            }
        }

        [HttpGet]
        [PageTitle("TaxReport")]
        public ActionResult TaxReport()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            List<InvoiceAndCompany> list = new List<InvoiceAndCompany>();

            return View(list);
        }
        [HttpPost]
        public ActionResult TaxReport(string ToDatetime, string Fromdatetime, string Custid, string Submit, string Tallyexcel)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;


            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);

            ViewBag.fromdate = Fromdatetime;


            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.todate = ToDatetime;


            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }

            List<InvoiceAndCompany> list =
            list = (from i in db.Invoices
                    join c in db.Companies
                    on i.Customer_Id equals c.Company_Id
                    join f in db.Franchisees
                    on c.Pf_code equals f.PF_Code

                    where
                        (i.Customer_Id == Custid || Custid == "") &&
                        i.Pfcode == strpf &&
                        i.isDelete == false &&
                        DbFunctions.TruncateTime(i.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(i.invoicedate) <= DbFunctions.TruncateTime(todate)


                    select new InvoiceAndCompany
                    {
                        invoiceno = i.invoiceno,
                        invoicedate = i.invoicedate,
                        periodfrom = i.periodfrom,
                        periodto = i.periodto,
                        total = i.total,
                        fullsurchargetax = i.fullsurchargetax,
                        fullsurchargetaxtotal = i.fullsurchargetaxtotal != null ? Math.Round((double)i.fullsurchargetaxtotal) : 0,
                        servicetax = i.servicetax,
                        servicetaxtotal = i.servicetaxtotal != null ? Math.Round((double)i.servicetaxtotal) : 0,
                        othercharge = i.othercharge != null ? Math.Round((double)i.othercharge) : 0,
                        netamount = Math.Round((double)i.netamount),
                        Customer_Id = i.Customer_Id,
                        fid = i.fid,
                        servicecharges = i.servicecharges != null ? Math.Round((double)i.servicecharges) : 0,
                        Royalty_charges = i.Royalty_charges != null ? Math.Round((double)i.Royalty_charges) : 0,
                        Docket_charges = i.Docket_charges != null ? Math.Round((double)i.Docket_charges) : 0,
                        discount = i.discount != null ? i.discount : "0",
                        discountamount = i.discountamount != null ? Math.Round((double)i.discountamount) : 0,
                        Tempdatefrom = i.Tempdatefrom,
                        TempdateTo = i.TempdateTo,
                        tempInvoicedate = i.tempInvoicedate,
                        Company_Name = c.Company_Name,
                        Gst_No = c.Gst_No,
                        Fr_Gst_No = f.GstNo,
                        CgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                        SgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                        IgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0) : 0) : 0,

                        CgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                        SgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                        IgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0) : 0) : 0,

                    }).ToList();



            if (Submit == "Export to Excel")
            {
                var list1 = (from i in db.Invoices
                             join c in db.Companies
                             on i.Customer_Id equals c.Company_Id
                             join f in db.Franchisees
                             on c.Pf_code equals f.PF_Code

                             where
                                 (i.Customer_Id == Custid || Custid == "") &&
                                  i.Pfcode == strpf &&
                                  i.isDelete == false &&
                                 DbFunctions.TruncateTime(i.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(i.invoicedate) <= DbFunctions.TruncateTime(todate)


                             select new
                             {
                                 invoiceno = i.invoiceno,
                                 invoicedate = i.tempInvoicedate,
                                 periodfrom = i.Tempdatefrom,
                                 periodto = i.TempdateTo,
                                 total = i.total,
                                 fullsurchargetax = i.fullsurchargetax,
                                 fullsurchargetaxtotal = i.fullsurchargetaxtotal,
                                 //servicetax = i.servicetax,
                                 //servicetaxtotal = i.servicetaxtotal,
                                 //othercharge = i.othercharge,
                                 DiscountPer = i.discountper,
                                 DiscountAmt = i.discountamount,
                                 netamount = i.netamount,
                                 Customer_Id = i.Customer_Id,
                                 // fid = i.fid,
                                 // servicecharges = i.servicecharges,
                                 Royalty_charges = i.Royalty_charges,
                                 Docket_charges = i.Docket_charges,

                                 Company_Name = c.Company_Name,
                                 Gst_No = c.Gst_No,
                                 // Fr_Gst_No = f.GstNo,
                                 //CgstPer = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0,
                                 //SgstPer = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0,
                                 //IgstPer = c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0,

                                 //CgstAmt = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0,
                                 //SgstAmt = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0,
                                 //IgstAmt = c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0,

                                 CgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                 SgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                 IgstPer = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0) : 0) : 0,

                                 CgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                                 SgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0,
                                 IgstAmt = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0) : 0) : 0,

                             }).ToList();
                if (list1.Count() <= 0 || list1 == null)
                {
                    ViewBag.Nodata = "No Data Found";

                }
                else
                {
                    ExportToExcelAll.ExportToExcelAdmin(list1);

                }
            }

            if (Tallyexcel == "Tally excel")
            {
                string frmdate = Fromdatetime;
                string todate1 = ToDatetime;
                var list2 = (from i in db.Invoices
                             join c in db.Companies
                             on i.Customer_Id equals c.Company_Id
                             join f in db.Franchisees
                             on c.Pf_code equals f.PF_Code

                             where
                                 (i.Customer_Id == Custid || Custid == "") &&
                                  i.Pfcode == strpf &&
                                  i.isDelete == false &&
                                 DbFunctions.TruncateTime(i.invoicedate) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(i.invoicedate) <= DbFunctions.TruncateTime(todate)


                             select new
                             {
                                 Vch_No = i.invoiceno,
                                 Vch_Type = "Sales",
                                 Date = i.tempInvoicedate,

                                 Reference_No = i.invoiceno,
                                 Party_Name = i.Customer_Id,
                                 Ledger_Group = "Sundry Debtors",
                                 Registration_Type = "Regular",
                                 GstNo = c.Gst_No,
                                 Country = "India",
                                 State = "Maharashtra",
                                 Pincode = "400013",
                                 Address_1 = c.Company_Address,
                                 Address_2 = "",
                                 Address_3 = "",
                                 Sales_Ledger = "Advertising Service",
                                 Amt = i.discountamount > 0 ? i.total + i.fullsurchargetaxtotal + i.Royalty_charges + i.Docket_charges + i.discountamount : i.total + i.fullsurchargetaxtotal + i.Royalty_charges,
                                 Additional_Ledger = "Discount",
                                 Amount = i.discountamount > 0 ? i.discountamount : null,
                                 CGST_Ledger = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                 //CGST_Amt = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0,
                                 CGST_Amt = Math.Round((double)(i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0), 2),

                                 SGST_Ledger = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? 9 : 0) : 9) : 0,
                                 // SGST_Amt = c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0,
                                 SGST_Amt = Math.Round((double)(i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) == f.GstNo.Substring(0, 2) ? (i.servicetaxtotal / 2) : 0) : (i.servicetaxtotal / 2)) : 0), 2),
                                 IGST_Ledger = i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? 18 : 0) : 0) : 0,
                                 //  IGST_Amt = c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetax) : 0,
                                 IGST_Amt = Math.Round((double)(i.servicetax > 0 ? (c.Gst_No.Length > 1 ? (c.Gst_No.Substring(0, 2) != f.GstNo.Substring(0, 2) ? (i.servicetaxtotal) : 0) : 0) : 0), 2),
                                 CESS_Ledger = "",
                                 //Round_off=0,

                                 Total = Math.Round((double)i.netamount, 2),
                                 Narration = "COURIER CHARGES MONTH FROM " + frmdate + " TO " + todate1,
                                 TALLYIMPORTSTATUS = "",


                             }).ToList();



                if (list2.Count() <= 0 || list2 == null)
                {
                    ViewBag.Nodata = "No Data Found";

                }
                else
                {
                    ExportToExcelAll.ExportToExcelAdmin(list2);

                }

            }




            return View(list);
        }


        public JsonResult RemainingConsignments(string startno, string endno)
        {


            List<string> Consignments = new List<string>();


            char stch = startno[0];
            char Endch = endno[0];

            long startConsignment = Convert.ToInt64(startno.Substring(1));
            long EndConsignment = Convert.ToInt64(endno.Substring(1));



            for (long i = startConsignment; i <= EndConsignment; i++)
            {
                string updateconsignment = stch + i.ToString();


                Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment && m.isDelete == false).FirstOrDefault();


                if (transaction != null && transaction.Customer_Id != null && transaction.Customer_Id.Length > 1)
                {
                    Consignments.Add(transaction.Consignment_no);
                }


            }

            return Json(Consignments, JsonRequestBehavior.AllowGet);

        }



    }
}