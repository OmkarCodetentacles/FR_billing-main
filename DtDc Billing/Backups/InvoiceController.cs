using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    public class InvoiceController : Controller
    {

        private DB_A45FDB_SolutionsEntities db = new DB_A45FDB_SolutionsEntities();
        string invstart = "INV/20-21/";

        // GET: Invoice


        public ActionResult GenerateInvoice(long Firm_Id=6,string Invoiceno = null)
        {


            if (Firm_Id == 6)
            {
                string invstart1 = "IFS/20-21/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));


                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
          else if(Firm_Id == 7)
            {
                string invstart1 = "SHE/20-21/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else if(Firm_Id == 8)
            {
                string invstart1 = "ATE/20-21/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));


                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else
            {
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));


                ViewBag.lastInvoiceno = invstart + "" + (number + 1);
            }

            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();

           
            ViewBag.Firm_Name = new SelectList(db.FirmDetails, "Firm_Id", "Firm_Name", Firm_Id.ToString());

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;



            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno && m.Firm_Id == Firm_Id).FirstOrDefault();

            return View(inv);


        }

       
        public ActionResult getFirm()
        {           
            return Json(db.FirmDetails.Select(x => new
            {
                Firm_Id = x.Firm_Id,
                Firm_Name = x.Firm_Name
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult DpInvoice(long Firm_Id = 6, string Invoiceno = null)
        {

            if (Firm_Id == 6)
            {
                string invstart1 = "IFS/20-21/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else if(Firm_Id == 7)
            {
                string invstart1 = "SHE/20-21/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else if(Firm_Id == 8)
            {
                string invstart1 = "ATE/20-21/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else
            {

                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart + "" + (number + 1);
            }

            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno && m.Firm_Id == Firm_Id).FirstOrDefault();

            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;


            if (Invoiceno != null && Invoiceno.StartsWith("INV/17-18/"))
            {
                return RedirectToAction("GenerateInvoiceLastYear", new { Invoiceno = Invoiceno });
            }
            else
            { 
            return View(inv);
            }
        }
        // GET: Invoice
        public ActionResult ViewInvoice(string invfromdate, string invtodate, long Firm_Id = 6)
        {
            DateTime? fromdate = null;
            DateTime? todate = null;

            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();
            ViewBag.firmid = firm.Firm_Id;
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            if ((invfromdate != null && invfromdate != "") || (invtodate != null && invtodate != ""))
            {
                string bdatefrom = DateTime.ParseExact(invfromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                string bdateto = DateTime.ParseExact(invtodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);

                

                ViewBag.firmname = firm.Firm_Name;
               
                ViewBag.invfromdate = invfromdate;
                ViewBag.invtodate = invtodate;
                return View(db.Invoices.Where(m => (m.Total_Lable == null || m.Total_Lable.Length == 0) && (db.singleinvoiceconsignments.Where(x => x.Invoice_no == m.invoiceno).Select(x => x.Invoice_no).FirstOrDefault() == null ? true : false) && m.Firm_Id == Firm_Id).ToList());

            }
            else
            {
                return View(db.Invoices.Where(m => (m.Total_Lable == null || m.Total_Lable.Length == 0) &&  (db.singleinvoiceconsignments.Where(x=>x.Invoice_no == m.invoiceno).Select(x => x.Invoice_no).FirstOrDefault() == null ? true : false)  && m.Firm_Id == Firm_Id).ToList());
            }
            }

        
        public ActionResult ViewDPInvoice(long Firm_Id= 6)
        {
            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;
            return View(db.Invoices.Where(m => (((m.Total_Lable != null || m.Total_Lable.Length > 0) && m.Firm_Id == Firm_Id))).ToList());
        }


        public ActionResult ViewSingleInvoice(long Firm_Id = 6)
        {

            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;

            var temp = db.singleinvoiceconsignments.Select(m=>m.Invoice_no).ToList();



            var a =(from member in db.Invoices
                    where temp.Contains(member.invoiceno) && member.Firm_Id == Firm_Id
                    select member).ToList();



            return View(a);

        }



        public JsonResult InvoiceTable(string CustomerId, string Tempdatefrom, string TempdateTo)
        {

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);




            db.Configuration.ProxyCreationEnabled = false;

            var Companies = db.TransactionViews.Where(m => m.Customer_Id == CustomerId && !db.singleinvoiceconsignments.Select(b=>b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                              .ToList();



     

            return Json(Companies, JsonRequestBehavior.AllowGet);

        }

        public JsonResult InvoiceDetails(string CustomerId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Companies = db.Companies.Where(m => m.Company_Id == CustomerId).FirstOrDefault();


            return Json(Companies, JsonRequestBehavior.AllowGet);

        }


        public ActionResult CustomerIdAutocomplete(long Firm_Id)
        {


            var entity = db.Companies.Where(m=>m.Franchisee.Firm_Id==Firm_Id).
Select(e => new
{
    e.Company_Id
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveInvoice(Invoice invoice, string submit)
        {
           

            if (invoice.discount == "yes")
            {
                ViewBag.disc = invoice.discount;
            }

            var firm = db.FirmDetails.Where(m => m.Firm_Id == invoice.Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;

            if (ModelState.IsValid)
            {

                string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


                Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno && m.Firm_Id==invoice.Firm_Id).FirstOrDefault();



                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                   


                    invoice.IN_Id = inv.IN_Id;

                    invoice.invoiceno =  invoice.invoiceno;

                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Added SuccessFully";

                    /////////////////// update consignment///////////////////////
                    using (var db = new DB_A45FDB_SolutionsEntities())
                    {
                        var Companies = db.Transactions.Where(m => m.status_t == invoice.invoiceno).ToList();

                        Companies.ForEach(m => m.status_t = "0");
                        db.SaveChanges();


                        Companies = db.Transactions.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                     Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

                        Companies.ForEach(m => m.status_t = invoice.invoiceno);
                        db.SaveChanges();
                    }
                    ///////////////////end of update consignment///////////////////////
                }
                else
                {

                    var invoi = db.Invoices.Where(m => m.tempInvoicedate == invoice.tempInvoicedate && m.Customer_Id == invoice.Customer_Id).FirstOrDefault();

                    if (invoi != null)
                    {
                        ModelState.AddModelError("invoi", "Invoice is already Generated");
                    }

                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);


                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, (long)invoice.Firm_Id);

                    invoice.invoiceno =  invoice.invoiceno;

                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";


                    /////////////////// update consignment///////////////////////
                    using (var db = new DB_A45FDB_SolutionsEntities())
                    {
                        var Companies = db.Transactions.Where(m => m.status_t == invoice.invoiceno).ToList();

                        Companies.ForEach(m => m.status_t = "0");
                        db.SaveChanges();


                        Companies = db.Transactions.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                     Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

                        Companies.ForEach(m => m.status_t = invoice.invoiceno);
                        db.SaveChanges();
                    }
                    ///////////////////end of update consignment///////////////////////

                }

                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                LocalReport lr = new LocalReport();







                //var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no))
                //              .ToList().
                //              Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                //         .ToList();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
             Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                           .ToList();

                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;

                string discount = dataset3.FirstOrDefault().discount;
                if (discount == "no")
                {
                    if (clientGst != null && clientGst.Length > 4)
                    {
                        if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }

                        }
                        else
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }

                else if(discount ==  "yes")
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountPrint.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }


                lr.EnableExternalImages = true;
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);



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

                
                ViewBag.pdf = false;

                //if (submit == "Generate")
                //{
                ViewBag.pdf = true;
                ViewBag.invoiceno = invoice.invoiceno;
                //}
                if (submit == "Email")
                {

                    MemoryStream memoryStream = new MemoryStream(renderByte);
                   


                    using (MailMessage mm = new MailMessage("billingdtdc48@gmail.com", dataset4.FirstOrDefault().Email))
                    {
                        mm.Subject = "Invoice";

                        string Bodytext = "<html><body>Please Find Attachment</body></html>";
                        Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

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
                        credentials.UserName = "billingdtdc48@gmail.com";
                        credentials.Password = "dtdcmf1339";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = credentials;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }

                }






                return PartialView("GenerateInvoicePartial", invoice);

            }
            return PartialView("GenerateInvoicePartial", invoice);
        }



        [HttpPost]
        public ActionResult SaveDpInvoice(Invoice invoice, string submit)
        {


            if (invoice.Total_Lable == null)
            {
                ModelState.AddModelError("Total_Lable", "Label Required");
            }


            var firm = db.FirmDetails.Where(m => m.Firm_Id == invoice.Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;



            if (ModelState.IsValid)
            {

                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno && m.Firm_Id == invoice.Firm_Id).FirstOrDefault();


                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, (long)invoice.Firm_Id);


                    invoice.IN_Id = inv.IN_Id;

                    invoice.invoiceno =  invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.discountper = 0;
                    invoice.discountamount = 0;
                    invoice.discount = "no";
                    invoice.othercharge = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Added SuccessFully";
                }
                else
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, (long)invoice.Firm_Id);

                    invoice.invoiceno =  invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";

                }





                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                LocalReport lr = new LocalReport();







                var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id)
                              .ToList().
                              Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                         .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;
               

                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }




                lr.EnableExternalImages = true;

                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);



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


                //if (submit == "Generate")
                //{
                ViewBag.pdf = true;
                ViewBag.invoiceno = invoice.invoiceno;
                // }


                if (submit == "Email")
                {

                    MemoryStream memoryStream = new MemoryStream(renderByte);

                    using (MailMessage mm = new MailMessage("Mailid@gmail.com", dataset4.FirstOrDefault().Email))
                    {
                        mm.Subject = "Invoice";

                        string Bodytext = "<html><body>Please Find Attachment</body></html>";
                        Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

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
                        credentials.UserName = "Mailid@gmail.com";
                        credentials.Password = "password";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = credentials;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }




                }








                return PartialView("DpInvoicePartial", invoice);

            }
            return PartialView("DpInvoicePartial", invoice);
        }



        [HttpPost]
        public ActionResult SaveInvoiceLastYear(Invoice invoice, string submit)
        {


            if (invoice.Total_Lable == null)
            {
                ModelState.AddModelError("Total_Lable", "Label Required");
            }



            var firm = db.FirmDetails.Where(m => m.Firm_Id == invoice.Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;

            if (ModelState.IsValid)
            {

                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                Invoice inv = db.Invoices.Where(m => m.invoiceno ==  invoice.invoiceno).FirstOrDefault();


                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                   


                    invoice.IN_Id = inv.IN_Id;

                    invoice.invoiceno =  invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.discountper = 0;
                    invoice.discountamount = 0;
                    invoice.discount = "no";
                    invoice.othercharge = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Added SuccessFully";

                    ViewBag.nextinvoice = GetmaxInvoiceno("INV/17-18/", (long)invoice.Firm_Id);
                }
                else
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                    

                    invoice.invoiceno = invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";
                    ViewBag.nextinvoice = GetmaxInvoiceno("INV/17-18/", (long)invoice.Firm_Id);
                }





                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                LocalReport lr = new LocalReport();










                //if (submit == "Generate")
                //{
                ViewBag.pdf = true;
                ViewBag.invoiceno = invoice.invoiceno;
                // }


                








                return PartialView("GenerateInvoiceLastYearPartial", invoice);

            }
            return PartialView("GenerateInvoiceLastYearPartial", invoice);
        }




        [HttpGet]
        public ActionResult ReportPrinterMethod(string myParameter,long firmid)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id==firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                //var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no))
                //           .ToList().
                //           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                //      .ToList();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
          Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                        .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id == firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

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

                lr.EnableExternalImages = true;

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


        [HttpGet]
        public ActionResult DpReportPrinterMethod(string myParameter, long firmid)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id==firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id)
                           .ToList().
                           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0)
                      .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id == firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

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

                lr.EnableExternalImages = true;

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


        public ActionResult MultipleInvoice(long Firm_Id=6)
        {
            ViewBag.Complist = db.Companies.Where(m => !(m.Company_Id.StartsWith("Cash_")) && !(m.Company_Id.StartsWith("BASIC_TS")) && m.Franchisee.Firm_Id== Firm_Id).Select(m => m.Company_Id).ToList();

            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MultipleInvoice(string[] Companies, Invoice invoice, string submit)
        {
            ViewBag.Complist = db.Companies.Where(m => !(m.Company_Id.StartsWith("Cash_")) && !(m.Company_Id.StartsWith("BASIC_TS")) && m.Franchisee.Firm_Id == invoice.Firm_Id ).Select(m => m.Company_Id).ToList();

            


            if (ModelState.IsValid)
            {

              
                    Task.Run(() => MultipleInvoiceAsyncMethod(Companies, invoice, submit));
                
                ViewBag.Success = "All Invoices Generated SuccessFully";
            }


            return View();
        }

        public void MultipleInvoiceAsyncMethod(string[] Companies, Invoice invoice, string submit)
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string invoicedate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);
            DateTime invdate = Convert.ToDateTime(invoicedate);

           
            foreach (var i in Companies)
            {
                var invoi = db.Invoices.Where(m => m.tempInvoicedate == invoice.tempInvoicedate && m.Customer_Id == i).FirstOrDefault();

                if (invoi == null)
                {
                    Company cm = db.Companies.Where(m => m.Company_Id == i).FirstOrDefault();

                    var TrList = db.TransactionViews.Where(m => m.Customer_Id == i).ToList().
                   Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0)
                                     .ToList();


                    Invoice inv = new Invoice();



                    double? AmountTotal = TrList.Sum(m => m.Amount ?? 0);

                    double? RisksurchargeTotal = TrList.Sum(m => m.Risksurcharge ?? 0);

                    double? OtherchargeTotal = TrList.Sum(m => m.loadingcharge ?? 0);

                    inv.total = AmountTotal + RisksurchargeTotal + OtherchargeTotal;

                    inv.fullsurchargetax = cm.Fuel_Sur_Charge ?? 0;

                    inv.periodfrom = fromdate;
                    inv.servicetax = invoice.servicetax;
                    inv.periodto = todate;
                    inv.invoicedate = invdate;
                    inv.Tempdatefrom = invoice.Tempdatefrom;
                    inv.TempdateTo = invoice.TempdateTo;
                    inv.tempInvoicedate = invoice.tempInvoicedate;
                    inv.Address = db.Companies.Where(m => m.Company_Id == i).Select(m => m.Company_Address).FirstOrDefault();
                    inv.Customer_Id = i;

                    inv.fullsurchargetaxtotal = ((inv.total * Convert.ToDouble(cm.Fuel_Sur_Charge)) / 100);

                    string invoiceno = "0";
                 

                    if (invoice.Firm_Id == 6)
                    {
                        String lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == invoice.Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? "INV/19-20/" + 0;
                        int number = Convert.ToInt32(lastInvoiceno.Substring(10));
                        invoiceno = (number + 1).ToString();
                    }
                    else if (invoice.Firm_Id == 7)
                    {
                        String lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == invoice.Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? "INV/19-20/" + 0;
                        int number = Convert.ToInt32(lastInvoiceno.Substring(10));
                        invoiceno = (number + 1).ToString();
                    }
                    else if (invoice.Firm_Id == 8)
                    {
                        String lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == invoice.Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? "INV/19-20/" + 0;
                        int number = Convert.ToInt32(lastInvoiceno.Substring(10));
                        invoiceno = (number + 1).ToString();
                    }
                  
                    else
                    {
                        String lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == invoice.Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? "INV/19-20/" + 0;
                        int number = Convert.ToInt32(lastInvoiceno.Substring(10));
                        invoiceno = (number + 1).ToString();
                    }

                    inv.invoiceno = invstart + (invoiceno);

                    inv.Firm_Id = invoice.Firm_Id;
                    inv.discount = "no";


                    inv.Docket_charges = 0;

                    foreach (var j in TrList)
                    {
                        if (j.Consignment_no.ToLower().StartsWith("d"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.D_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("m"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.P_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("e"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.E_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("v"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.V_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("i"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.I_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("n"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.N_Docket);
                        }

                        else if (j.Consignment_no.ToLower().StartsWith("g"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.G_Docket);
                        }
                    }

                    inv.Royalty_charges = ((inv.total * Convert.ToDouble(cm.Royalty_Charges)) / 100);

                    inv.servicetaxtotal = (((inv.total + inv.fullsurchargetaxtotal + inv.Docket_charges + inv.Royalty_charges) * invoice.servicetax) / 100); //((gst_total * parseFloat("0" + gst)) / 100);
                    inv.netamount = inv.total + inv.Docket_charges + inv.Royalty_charges + inv.servicetaxtotal + inv.fullsurchargetaxtotal;
                    inv.netamount = Math.Round(inv.netamount ?? 0, 0);
                    inv.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    if (inv.netamount > 0)
                    {
                        db.Invoices.Add(inv);
                        try
                        {
                            // Your code...
                            // Could also be before try if you know the exception occurs in SaveChanges

                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }


                        /****************For Billed unbilled ******************/
                        using (var db = new DB_A45FDB_SolutionsEntities())
                        {
                            var Companies1 = db.Transactions.Where(m => m.status_t == inv.invoiceno).ToList();

                            Companies1.ForEach(m => m.status_t = "0");
                            db.SaveChanges();


                            Companies1 = db.Transactions.Where(m => m.Customer_Id == inv.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                         Where(x => DateTime.Compare(x.booking_date.Value.Date, inv.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inv.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

                            Companies1.ForEach(m => m.status_t = inv.invoiceno);
                            db.SaveChanges();
                        }

                        /****************For Billed unbilled ******************/
                        //if (submit == "Email")
                        //{
                        SendMailInvoiceMultiple(inv, submit);
                        // }
                    }
                }


                Notification nt = new Notification();

                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                nt.dateN = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                nt.Message = "From Company Id" + Companies.FirstOrDefault() + "to" + Companies.LastOrDefault() + "Invoices Generated SuccessFully";
                nt.Status = false;

                db.Notifications.Add(nt);
                db.SaveChanges();
            }

        }



        public void SendMailInvoiceMultiple(Invoice invoice, string submit)
        {
            LocalReport lr = new LocalReport();

            string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

            //var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no))
            //                 .ToList().
            //                 Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
            //            .ToList();


            var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                          .ToList();


            var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

            var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno && m.Firm_Id==invoice.Firm_Id);

            var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

            string clientGst = dataset4.FirstOrDefault().Gst_No;
            string frgst = dataset2.FirstOrDefault().GstNo;


            if (clientGst != null && clientGst.Length > 4)
            {
                if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }

                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }
            }
            else
            {
                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
            }




            ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
            ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
            ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
            ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

            lr.EnableExternalImages = true;

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

            if (submit == "Email")
            {
                MemoryStream memoryStream = new MemoryStream(renderByte);

                using (MailMessage mm = new MailMessage("Mailid@gmail.com", dataset4.FirstOrDefault().Email))
                {
                    mm.Subject = "Invoice";

                    string Bodytext = "<html><body>Please Find Attachment</body></html>";
                    Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

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
                    credentials.UserName = "Mailid@gmail.com";
                    credentials.Password = "password";
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = credentials;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }


            }


            string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id + "-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                stream.Write(renderByte, 0, renderByte.Length);
            }



        }

        [HttpGet]
        public string SavepdInvoice(string myParameter , long firmid)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                //var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b=>b.Consignment_no).Contains(m.Consignment_no))
                //           .ToList().
                //           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                //      .ToList();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                          .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id == firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;
                string discount = dataset3.FirstOrDefault().discount;

                if (discount == "no")
                {
                    if (clientGst != null && clientGst.Length > 4)
                    {
                        if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }

                        }
                        else
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }

                }
               
                else if (discount == "yes")
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountPrint.rdlc");

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

                lr.EnableExternalImages = true;

                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                lr.EnableExternalImages = true;

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



                string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id +"-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }

                return dataset3.FirstOrDefault().Firm_Id +"-"+ dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";
                
            }

        }



        [HttpGet]
        public string SavepdDpInvoice(string myParameter, long firmid)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id)
                           .ToList().
                           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0)
                      .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id == firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

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

                lr.EnableExternalImages = true;

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

                //string savePath = Server.MapPath("~/PDF/" + dataset4.FirstOrDefault().Company_Name + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                //using (FileStream stream = new FileStream(savePath, FileMode.Create))
                //{
                //    stream.Write(renderByte, 0, renderByte.Length);
                //}

                //return dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";



                string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id + "-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }

                return dataset3.FirstOrDefault().Firm_Id + "-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

            }

        }

        [HttpGet]
        public ActionResult InvoiceZip(long Firm_Id = 2)
        {

           
            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;
            return View();
        }



        [HttpPost]
        public ActionResult InvoiceZip(int frominv, int toinv , long Firm_id )
        {


            string fileType = "application/octet-stream";



            var outputStream = new MemoryStream();


            using (ZipFile zipFile = new ZipFile())
            {





                for (int i = frominv; i <= toinv; i++)
                {
                     string filePath = Server.MapPath("/PDF/" + Firm_id + "-" + "INV-19-20-" + i + ".pdf");

                    if (System.IO.File.Exists(filePath))
                    {
                        zipFile.AddFile(filePath, "Invoices");
                    }


                }

                Response.ClearContent();
                Response.ClearHeaders();

                //Set zip file name
                Response.AppendHeader("content-disposition", "attachment; filename=Invoices.zip");

                //Save the zip content in output stream
                zipFile.Save(outputStream);
            }

            //Set the cursor to start position
            outputStream.Position = 0;

            //Dispance the stream
            return new FileStreamResult(outputStream, fileType);
        }

        public ActionResult GenerateInvoiceSingle(long Firm_Id = 6, string Invoiceno = null)
        {


            if (Firm_Id == 6)
            {
                string invstart1 = "IFS/20-21/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else if (Firm_Id == 7)
            {
                string invstart1 = "SHE/20-21/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else if (Firm_Id == 8)
            {
                string invstart1 = "ATE/20-21/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else
            {
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));
                ViewBag.lastInvoiceno = invstart +""+ (number + 1);

            }
            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno && m.Firm_Id == Firm_Id).FirstOrDefault();

            if(Invoiceno != null)
            {
                ViewBag.consignmnts = string.Join(",", db.singleinvoiceconsignments.Where(m => m.Invoice_no == Invoiceno).Select(m => m.Consignment_no).ToArray());
            }
            

            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;

            return View(inv);
        }

        [HttpPost]
        public ActionResult SaveSingleInvoice(Invoice invoice, string submit,string consignments)
        {

            ViewBag.consignmnts = consignments;

            if (invoice.discount == "yes")
            {
                ViewBag.disc = invoice.discount;
            }


            var firm = db.FirmDetails.Where(m => m.Firm_Id == invoice.Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;

            if (ModelState.IsValid)
            {

                string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


                Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno && m.Firm_Id  == invoice.Firm_Id).FirstOrDefault();



                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, (long)invoice.Firm_Id);


                    invoice.IN_Id = inv.IN_Id;

                    invoice.invoiceno =  invoice.invoiceno;

                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    inv.Firm_Id = invoice.Firm_Id;
                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Added SuccessFully";
                }
                else
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);


                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, (long)invoice.Firm_Id);

                    invoice.invoiceno =  invoice.invoiceno;

                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";

                }


            

                string[] cons = consignments.Split(',');

                foreach(var i in cons)
                {
                    singleinvoiceconsignment upsc = db.singleinvoiceconsignments.Where(m => m.Consignment_no == i).FirstOrDefault();

                    if(upsc == null)
                    {

                        singleinvoiceconsignment sc = new singleinvoiceconsignment();

                        sc.Consignment_no = i.Trim();
                        sc.Invoice_no = invoice.invoiceno;
                        db.singleinvoiceconsignments.Add(sc);
                        db.SaveChanges();

                    }




                }

                /////////////////// update consignment///////////////////////
                using (var db = new DB_A45FDB_SolutionsEntities())
                {


                    List<string> Companies = db.singleinvoiceconsignments.Where(m => m.Invoice_no == invoice.invoiceno).Select(m => m.Consignment_no).ToList();
                    var transaction = db.Transactions.Where(m => Companies.Contains(m.Consignment_no)).ToList();

                    transaction.ForEach(m => m.status_t = invoice.invoiceno);
                    db.SaveChanges();
                }
                ///////////////////end of update consignment///////////////////////

                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                LocalReport lr = new LocalReport();




                List<TransactionView> dataset = new List<TransactionView>();

                var consigmfromsingle = db.singleinvoiceconsignments.Where(m => m.Invoice_no ==  invoice.invoiceno);

            


                foreach(var c in consigmfromsingle)
                {
                    TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no).FirstOrDefault();
                    dataset.Add(temp);
                }

                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }




                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);


                lr.EnableExternalImages = true;
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


                ViewBag.pdf = false;

                //if (submit == "Generate")
                //{
                ViewBag.pdf = true;
                ViewBag.invoiceno = invoice.invoiceno;
                //}
                if (submit == "Email")
                {

                    MemoryStream memoryStream = new MemoryStream(renderByte);
                    using (MailMessage mm = new MailMessage("billingdtdc48@gmail.com", dataset4.FirstOrDefault().Email))
                    {
                        mm.Subject = "Invoice";

                        string Bodytext = "<html><body>Please Find Attachment</body></html>";
                        Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

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
                        credentials.UserName = "billingdtdc48@gmail.com";
                        credentials.Password = "dtdcmf1339";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = credentials;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }

                }






                return PartialView("GenerateInvoiceSinglePartial", invoice);

            }


            return PartialView("GenerateInvoiceSinglePartial", invoice);
        }

        public JsonResult InvoiceTableSingle(string [] array,string Customerid)
        {

            List<Transaction> Companies = new List<Transaction>();


            db.Configuration.ProxyCreationEnabled = false;
            if (array != null)
            {
                foreach (var i in array.Distinct().ToArray())
                {

                    Transaction tr = db.Transactions.Where(m => m.Consignment_no == i.Trim() && m.Customer_Id == Customerid).FirstOrDefault();

                    if (tr != null)
                    {
                        Companies.Add(tr);
                    }

                }
            }
           
            
            return Json(Companies, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult ReportsinglePrinterMethod(string myParameter, long firmid) //on view call thise method
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();


                List<TransactionView> dataset = new List<TransactionView>();

                var consigmfromsingle = db.singleinvoiceconsignments.Where(m => m.Invoice_no == myParameter);




                foreach (var c in consigmfromsingle)
                {
                    TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no).FirstOrDefault();
                    dataset.Add(temp);
                }




                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id ==firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

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


                lr.EnableExternalImages = true;
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


        [HttpGet]
        public string SavesinglepdInvoice(string myParameter, long firmid)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id ==firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();


                List<TransactionView> dataset = new List<TransactionView>();

                var consigmfromsingle = db.singleinvoiceconsignments.Where(m => m.Invoice_no == myParameter);




                foreach (var c in consigmfromsingle)
                {
                    TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no).FirstOrDefault();
                    if (temp != null)

                        dataset.Add(temp);
                }

               

                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id == firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

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

                lr.EnableExternalImages = true;

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



                string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id +"-"+ dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }

                return dataset3.FirstOrDefault().Firm_Id +"-"+ dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

            }

        }



        public ActionResult GenerateInvoiceLastYear(long Firm_Id = 6, string Invoiceno = null)
        {

            string invstart = "INV/17-18/";

            
            //string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
            //int number = Convert.ToInt32(lastInvoiceno.Substring(10));

            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno).FirstOrDefault();

            //ViewBag.lastInvoiceno = number + 1;
            
            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
            int number = Convert.ToInt32(lastInvoiceno.Substring(10));


            ViewBag.lastInvoiceno = invstart + "" + (number + 1);
        

        var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();


        ViewBag.Firm_Name = new SelectList(db.FirmDetails, "Firm_Id", "Firm_Name", Firm_Id.ToString());

        ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;

            return View(inv);
        }

        public string GetmaxInvoiceno(string invstart, long firmid)
        {

            
            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id== firmid).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;

            int number = Convert.ToInt32(lastInvoiceno.Substring(10));

            string lastInvoice = invstart + "" + (number + 1);

            return lastInvoice;
        }


        public ActionResult Download(long id)
        {

            var invoice = db.Invoices.Where(m => m.IN_Id == id).FirstOrDefault();

            string companyname = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Company_Id).FirstOrDefault().ToString();

            string savePath = "http://admin.infantjesussolutions.in/PDF/" + invoice.Firm_Id+"-"+invoice.invoiceno.Replace("/", "-")+ ".pdf";

            return Redirect(savePath);

        }
    }
}