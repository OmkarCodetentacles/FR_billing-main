using DtDc_Billing.Models;
using DtDc_Billing.Entity_FR;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmpBilling.Controllers
{
    [SessionTimeout]
    [SessionUserModule]
    public class BillingInvoiceController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Invoice

        public ActionResult ViewInvoice(string Fromdatetime = null, string ToDatetime = null)
        {
            var pf = Session["pfCode"].ToString();
            //var pf = Session["PfID"].ToString();
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
            

            string[] formats = {
                   "dd-MM-yyyy"};
            

            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);

 
            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;

            List<Invoice> abbbak = (from s in db.Companies
                                    join sa in db.Invoices on s.Company_Id equals sa.Customer_Id
                                    where s.Pf_code == pf && sa.Total_Lable == null || sa.Total_Lable.Length == 0
                                    where s.Franchisee.Firm_Id == sa.Firm_Id
                                    select sa).OrderBy(m=>m.IN_Id).ToList();
            return View(abbbak);
            //return View(db.Invoices.Where(m => m.Total_Lable == null || m.Total_Lable.Length == 0).ToList());
        }

        [HttpPost]
        public ActionResult ViewInvoice(string Fromdatetime, string ToDatetime, string Submit)
        {
            //var pf = Session["PfID"].ToString();
            var pf = Session["pfCode"].ToString();

            DateTime? fromdate = null;
            DateTime? todate = null;



            string[] formats = {
                   "dd-MM-yyyy"};



            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            fromdate = Convert.ToDateTime(bdatefrom);




            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            todate = Convert.ToDateTime(bdateto);
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;

            List<Invoice> abbbak = (from s in db.Companies
                                    join sa in db.Invoices on s.Company_Id equals sa.Customer_Id
                                    where s.Pf_code == pf && sa.Total_Lable == null || sa.Total_Lable.Length == 0
                                    where s.Franchisee.Firm_Id == sa.Firm_Id
                                    select sa).ToList().Where(x => DateTime.Compare(x.invoicedate.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.invoicedate.Value.Date, todate.Value.Date) <= 0).OrderBy(m => m.IN_Id).ToList();
            return View(abbbak);
            //return View(db.Invoices.Where(m => m.Total_Lable == null || m.Total_Lable.Length == 0).ToList());
        }


        [HttpGet]
        public ActionResult ReportPrinterMethod(string myParameter)
        {
            {  

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id)
                           .ToList().
                           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                      .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/Rdlc"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/Rdlc"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/Rdlc"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }





                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);



                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

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

                return File(renderByte, mimeType);
            }

        }

        public ActionResult Download(long id)
        {

            var invoice = db.Invoices.Where(m => m.IN_Id == id).FirstOrDefault();

            string companyname = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Company_Id).FirstOrDefault().ToString();

            string savePath = "http://admin.veerexpress.in/PDF/" + invoice.Firm_Id + "-" + invoice.invoiceno.Replace("/", "-") + ".pdf";

            return Redirect(savePath);

        }
    }
}