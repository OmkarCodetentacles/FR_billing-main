using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.CustomModel;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;

namespace DtDc_Billing.Controllers
{
    [SessionUserModule]
    public class demoController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();


        public ActionResult AddNotification()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddNotification(notificationModel notificationModel)
        {
            Notification notification = new Notification();
            
            if (ModelState.IsValid)
            {
                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                DateTime? fromdate;


                string bdatefrom = DateTime.ParseExact(notificationModel.tempdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);
                notification.dateN = fromdate;
                notification.Message = notificationModel.Message;
                notification.url_path = notificationModel.url_path;
                notification.description = notificationModel.description;
                db.Notifications.Add(notification);
                db.SaveChanges();

            }
            TempData["add"] = "Added succesfully";
            return View();
        }

        [HttpGet]
        public ActionResult importFromExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> importFromExcel(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase != null)
            {
                HttpPostedFileBase file = httpPostedFileBase;

                await GizmosAsync(httpPostedFileBase);


            }


            return View();
        }

        public async Task<ActionResult> GizmosAsync(HttpPostedFileBase httpPostedFileBase)
        {
            HttpPostedFileBase file = httpPostedFileBase;

            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = file.FileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];
                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));



                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        var tran = new Destination();

                        tran.Pincode = workSheet.Cells[rowIterator, 2].Value.ToString();
                        tran.Name = workSheet.Cells[rowIterator, 3].Value.ToString();
                        tran.State_ = workSheet.Cells[rowIterator, 4].Value.ToString();


                        var abc = db.Destinations.Where(m => m.Pincode == tran.Pincode).FirstOrDefault();

                        if (abc != null)
                        {
                            tran.Dest_Id = abc.Dest_Id;
                            db.Entry(abc).State = EntityState.Detached;
                            db.Entry(tran).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Destinations.Add(tran);
                            db.SaveChanges();
                        }

                    }
                }

                ViewBag.Success = "Excel File Uploaded SuccessFully";
            }

            return View("Gizmos");
        }


        public ActionResult index()
        {

            var list = db.singleinvoiceconsignments.ToList();

            Invoice invoie = db.Invoices.Where(m => m.invoiceno == "INV/18-19/807").FirstOrDefault();

            var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoie.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no))
              .ToList().
              Where(x => DateTime.Compare(x.booking_date.Value.Date, invoie.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoie.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
         .ToList();

            var dataset1 = db.TransactionViews.Where(m => m.Customer_Id == invoie.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                Where(x => DateTime.Compare(x.booking_date.Value.Date, invoie.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoie.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                              .ToList();

            return View();
        }





        //public ActionResult sendMailInvalidInvoice()
        //{

        //    int i = 387;

        //    for (i = 387; i <= 413; i++)
        //    {
        //        Invoice invoiceno = db.Invoices.Where(m => m.invoiceno == "INV/18-19/" + i).FirstOrDefault();

        //        Company company = db.Companies.Where(m => m.Company_Id == invoiceno.Customer_Id).FirstOrDefault();


        //        //using (MailMessage mm = new MailMessage("billingpawanhansexpress@gmail.com", "codetentacles@gmail.com"))
        //        using (MailMessage mm = new MailMessage("billingpawanhansexpress@gmail.com", company.Email))
        //        {
        //            mm.Subject = "Regarding Invoice";

        //            string Bodytext = "<html><body>Please ignore this month Bill we will send you Correct bill within next 4 days.<br />Thanks & Regards, <br /> AtkExpress</body></html>";






        //            mm.IsBodyHtml = true;



        //            mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

        //            AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
        //            // mm.Body = Bodytext;
        //            mm.Body = Bodytext;

        //            //mm.Headers.Add("Message-Id", "billingpawanhansexpress@gmail.com");


        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Host = "relay-hosting.secureserver.net";
        //            // smtp.EnableSsl = true;
        //            //mm.Priority = MailPriority.High;                
        //            mm.ReplyToList.Add("billingpawanhansexpress@gmail.com");
        //            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
        //            credentials.UserName = "billingpawanhansexpress@gmail.com";
        //            credentials.Password = "lalghati123";
        //            smtp.UseDefaultCredentials = false;
        //            smtp.Credentials = credentials;
        //            smtp.Port = 25;
        //            smtp.Send(mm);


        //        }


        //    }



        //    return View();
        //}



        public ActionResult sendMail413to535()
        {

            int i = 413;

            for (i = 413; i <= 509; i++)
            {
                Invoice invoiceno = db.Invoices.Where(m => m.invoiceno == "INV/18-19/" + i).FirstOrDefault();

                Company company = db.Companies.Where(m => m.Company_Id == invoiceno.Customer_Id).FirstOrDefault();


                //using (MailMessage mm = new MailMessage("billingpawanhansexpress@gmail.com", "codetentacles@gmail.com"))

                SendMailInvoiceMultiple(invoiceno);

            }



            return View();
        }


        public void SendMailInvoiceMultiple(Invoice invoice)
        {
            LocalReport lr = new LocalReport();

            string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

            var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no))
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

           
                MemoryStream memoryStream = new MemoryStream(renderByte);
            using (MailMessage mm = new MailMessage("billingpawanhansexpress@gmail.com", dataset4.FirstOrDefault().Email))
            //using (MailMessage mm = new MailMessage("billingpawanhansexpress@gmail.com", "codetentacles@gmail.com"))
            {
                    mm.Subject = "Invoice";

                    string Bodytext = "<html><body>Please Find Attachment</body></html>";
                    Attachment attachment = new Attachment(memoryStream, invoice.invoiceno +".pdf");
                    //Add Byte array as Attachment.




                    mm.IsBodyHtml = true;



                    mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                    AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                    // mm.Body = Bodytext;
                    mm.Body = Bodytext;

                    mm.Attachments.Add(attachment);
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "relay-hosting.secureserver.net";
                    // smtp.EnableSsl = true;
                    mm.ReplyToList.Add("billingpawanhansexpress@gmail.com");
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                    credentials.UserName = "billingpawanhansexpress@gmail.com";
                    credentials.Password = "lalghati123";
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = credentials;
                    smtp.Port = 25;
                    smtp.Send(mm);
                }



            string path111 = Server.MapPath("~/Content/Invoices.txt");

           

                using (StreamWriter sw = (System.IO.File.Exists(path111)) ? System.IO.File.AppendText(path111) : System.IO.File.CreateText(path111))
                {

                    sw.WriteLine("Invoice no : " + " "  + invoice.invoiceno.ToString() +" "+ "Send To " + dataset4.FirstOrDefault().Email);

                }
            





        }

        //public ActionResult index()
        //{
        //    int i = 101;

        //    string path = Server.MapPath("~/Content/Invoices.txt");

        //    for (i=0;i<=100;i++)
        //    {               

        //        using (StreamWriter sw = (System.IO.File.Exists(path)) ? System.IO.File.AppendText(path) : System.IO.File.CreateText(path))
        //        {

        //            sw.WriteLine(i.ToString() + "Hello");

        //        }
        //    }


            


          


        //    return View();
        //}

        //public ActionResult SendMail()
        //{

        //    Invoice invoice = db.Invoices.OrderBy(m=>m.IN_Id).Skip(50).Take(1).FirstOrDefault();

        //    string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


        //    LocalReport lr = new LocalReport();







        //    var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id)
        //                  .ToList().
        //                  Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
        //             .ToList();


        //    var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

        //    var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

        //    var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

        //    string clientGst = dataset4.FirstOrDefault().Gst_No;
        //    string frgst = dataset2.FirstOrDefault().GstNo;


        //    if (clientGst != null && clientGst.Length > 4)
        //    {
        //        if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
        //        {
        //            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

        //            if (System.IO.File.Exists(path))
        //            {
        //                lr.ReportPath = path;
        //            }

        //        }
        //        else
        //        {
        //            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

        //            if (System.IO.File.Exists(path))
        //            {
        //                lr.ReportPath = path;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

        //        if (System.IO.File.Exists(path))
        //        {
        //            lr.ReportPath = path;
        //        }
        //    }




        //    ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
        //    ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
        //    ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
        //    ReportDataSource rd3 = new ReportDataSource("comp", dataset4);



        //    lr.DataSources.Add(rd);
        //    lr.DataSources.Add(rd1);
        //    lr.DataSources.Add(rd2);
        //    lr.DataSources.Add(rd3);

        //    string reportType = "pdf";
        //    string mimeType;
        //    string encoding;
        //    string fileNameExte;

        //    string deviceInfo =
        //        "<DeviceInfo>" +
        //        "<OutputFormat>" + "pdf" + "</OutputFormat>" +
        //        "<PageHeight>11in</PageHeight>" +
        //       "<Margintop>0.1in</Margintop>" +
        //         "<Marginleft>0.1in</Marginleft>" +
        //          "<Marginright>0.1in</Marginright>" +
        //           "<Marginbottom>0.5in</Marginbottom>" +
        //           "</DeviceInfo>";

        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderByte;


        //    renderByte = lr.Render
        //  (reportType,
        //  deviceInfo,
        //  out mimeType,
        //  out encoding,
        //  out fileNameExte,
        //  out streams,
        //  out warnings
        //  );


        //    ViewBag.pdf = false;

        //    //if (submit == "Generate")
        //    //{
        //    ViewBag.pdf = true;
        //    ViewBag.invoiceno = invoice.invoiceno;
        //    //}


        //    MemoryStream memoryStream = new MemoryStream(renderByte);
        //    using (MailMessage mm = new MailMessage("billingpawanhansexpress@gmail.com", "codetentacles@gmail.com"))
        //    {
        //        mm.Subject = "Invoice";

        //        string Bodytext = "<html><body>Please Find Attachment</body></html>";
        //        Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");
        //        //Add Byte array as Attachment.




        //        mm.IsBodyHtml = true;



        //        mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

        //        AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
        //        // mm.Body = Bodytext;
        //        mm.Body = Bodytext;

        //        //mm.Headers.Add("Message-Id", "billingpawanhansexpress@gmail.com");

        //        mm.Attachments.Add(attachment);
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "relay-hosting.secureserver.net";
        //        // smtp.EnableSsl = true;
        //        //mm.Priority = MailPriority.High;                
        //        mm.ReplyToList.Add("billingpawanhansexpress@gmail.com");
        //        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
        //        credentials.UserName = "billingpawanhansexpress@gmail.com";
        //        credentials.Password = "lalghati123";
        //        smtp.UseDefaultCredentials = false;
        //        smtp.Credentials = credentials;
        //        smtp.Port = 25;
        //        smtp.Send(mm);


        //    }


        //    //const string SERVER = "relay-hosting.secureserver.net";

        //    //MailMessage mm = new System.Web.Mail.MailMessage();

        //    //mm.From = "billingpawanhansexpress@gmail.com";
        //    //mm.To = "codetentacles@gmail.com";
        //    //mm.Subject = "Invoice";
        //    //mm.BodyFormat = MailFormat.Html; // enumeration
        //    //mm.Priority = MailPriority.High; // enumeration

        //    //mm.Body = "Sent at: " + DateTime.Now;
        //    //SmtpMail.SmtpServer = SERVER;
        //    //SmtpMail.Send(mm);
        //    //mm = null;

        //    return View();
        //}

        // GET: demo
        //public ActionResult Index()
        //{
        //    //List<string> list = new List<string>();


        //    //list = db.Sectors.Select(m => m.Pf_code).Distinct().ToList();



        //    //foreach (var i in list)
        //    //{

        //    //    Sector sn = new Sector();

        //    //    sn.Pf_code = i;
        //    //    sn.Sector_Name = "Mumbai";
        //    //    sn.Pincode_values = "400001-400610,400615-400706,400710-401203,401205-402209";

        //    //    db.Sectors.Add(sn);
        //    //    db.SaveChanges();
        //    //}


        //    IEnumerable<Sector> Sector = db.Sectors.AsEnumerable();


        //    foreach (Sector i in Sector.ToList())
        //    {

        //        i.CashD = true;
        //        i.CashN = true;
        //        i.BillD = true;
        //        i.BillN = true;

        //        if (i.Sector_Name == "Within City")
        //        {

        //            i.Priority = 1;

        //        }
        //        else if (i.Sector_Name == "Mumbai")
        //        {
        //            i.CashD = false;
        //            i.CashN = false;
        //            i.BillD = true;
        //            i.BillN = false;

        //            i.Priority = 2;

        //        }
        //        else if (i.Sector_Name == "Within Zone")
        //        {
        //            i.CashD = true;
        //            i.CashN = true;
        //            i.BillD = false;
        //            i.BillN = false;

        //            i.Priority = 3;

        //        }
        //        else if (i.Sector_Name == "Within State")
        //        {
        //            i.Priority = 4;

        //        }
        //        else if (i.Sector_Name == "Metro")
        //        {
        //            i.Priority = 5;

        //        }
        //        else if (i.Sector_Name == "Estern And Non Estern")
        //        {
        //            i.Priority = 6;

        //        }
        //        else if (i.Sector_Name == "Jammu And Kashmir")
        //        {
        //            i.Priority = 7;

        //        }
        //        else if (i.Sector_Name == "Rest Of India")
        //        {
        //            i.Priority = 8;


        //        }





        //        db.Entry(i).State = EntityState.Modified;
        //        db.SaveChanges();




        //    }


        //    //List<string> list = new List<string>();


        //    //list = db.Ratems.Select(m => m.Company_id).Distinct().ToList();



        //    //foreach (var i in list)
        //    //{
        //    //    string pfcode = db.Companies.Where(m => m.Company_Id == i).Select(m => m.Pf_code).FirstOrDefault();

        //    //    int sectid = db.Sectors.Where(m => m.Pf_code == pfcode && m.Sector_Name == "Mumbai").Select(m => m.Sector_Id).FirstOrDefault();





        //    //List<string> list = new List<string>();


        //    //list = db.Ratems.Select(m => m.Company_id).Distinct().ToList();


        //    //    Ratem ratem = db.Ratems.Where(m=>m.Sector_Id ==sectid).FirstOrDefault();
        //    //    Nondox nondox = db.Nondoxes.Where(m => m.Sector_Id == sectid).FirstOrDefault();
        //    //    express_cargo express = db.express_cargo.Where(m => m.Sector_Id == sectid).FirstOrDefault();


        //    //    db.Ratems.Remove(ratem);
        //    //    db.Nondoxes.Remove(nondox);
        //    //    db.express_cargo.Remove(express);

        //    //    db.SaveChanges();

        //    //}



        //    //foreach (var i in list)
        //    //{
        //    //    string pfcode = db.Companies.Where(m => m.Company_Id == i).Select(m => m.Pf_code).FirstOrDefault();

        //    //    int sectid = db.Sectors.Where(m => m.Pf_code == pfcode && m.Sector_Name == "Mumbai").Select(m => m.Sector_Id).FirstOrDefault();



        //    //    Ratem dox = new Ratem();
        //    //    Nondox ndox = new Nondox();
        //    //    express_cargo cs = new express_cargo();




        //    //    dox.Company_id = i;
        //    //    dox.Sector_Id = sectid;






        //    //    dox.NoOfSlab = db.Ratems.Where(m => m.Company_id == i).Select(m => m.NoOfSlab).FirstOrDefault();

        //    //    dox.slab1 = 50;
        //    //    dox.slab2 = 60;
        //    //    dox.slab3 = 70;
        //    //    dox.slab4 = 80;

        //    //    dox.Uptosl1 = 0.25;
        //    //    dox.Uptosl2 = 0.5;
        //    //    dox.Uptosl3 = 0.5;
        //    //    dox.Uptosl4 = 0.5;




        //    //    ndox.Company_id = i;
        //    //    ndox.Sector_Id = sectid;
        //    //    ndox.NoOfSlabN = db.Nondoxes.Where(m => m.Company_id == i).Select(m => m.NoOfSlabN).FirstOrDefault();
        //    //    ndox.NoOfSlabS = db.Nondoxes.Where(m => m.Company_id == i).Select(m => m.NoOfSlabS).FirstOrDefault();



        //    //    ndox.Aslab1 = 50;
        //    //    ndox.Aslab2 = 60;
        //    //    ndox.Aslab3 = 70;
        //    //    ndox.Aslab4 = 80;

        //    //    ndox.Sslab1 = 50;
        //    //    ndox.Sslab2 = 60;
        //    //    ndox.Sslab3 = 70;
        //    //    ndox.Sslab4 = 80;

        //    //    ndox.AUptosl1 = 0.5;
        //    //    ndox.AUptosl2 = 0.5;
        //    //    ndox.AUptosl3 = 0.5;
        //    //    ndox.AUptosl4 = 0.5;

        //    //    ndox.SUptosl1 = 0.5;
        //    //    ndox.SUptosl2 = 0.5;
        //    //    ndox.SUptosl3 = 0.5;
        //    //    ndox.SUptosl4 = 0.5;



        //    //    cs.Company_id = i;
        //    //    cs.Sector_Id = sectid;

        //    //    cs.Exslab1 = 50;
        //    //    cs.Exslab2 = 100;


        //    //    db.express_cargo.Add(cs);
        //    //    db.Nondoxes.Add(ndox);
        //    //    db.Ratems.Add(dox);
        //    //    db.SaveChanges();
        //    //}





        //    return View();
        //}

        // GET: demo/Details/5

        public ActionResult updateinv()
        {
            List<Invoice> invlist = db.Invoices.Where(m => m.Total_Lable == null || m.Total_Lable.Length == 0).ToList();

            foreach (Invoice i in invlist)
            {


                SavepdInvoice(i.invoiceno);

                //i.Invoice_Lable = AmountTowords.changeToWords(i.netamount.ToString());

                //if(i.discount == null)
                //{
                //    i.discount = "no";
                //}

                //db.Entry(i).State = EntityState.Modified;
                //db.SaveChanges();

            }

            

            return View();
        }


        //public ActionResult SendMailAllInvoices()
        //{
        //    List<Invoice> invoices = db.Invoices.Where(m => m.Total_Lable == null || m.Total_Lable.Length == 0).ToList();

        //    List<string> list = new List<string>();
        //    foreach (var i in  invoices)
        //    {
        //        Invoice invoice = i;

        //        string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


        //        LocalReport lr = new LocalReport();







        //        var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id)
        //                      .ToList().
        //                      Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
        //                 .ToList();


        //        var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

        //        var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

        //        var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

        //        string clientGst = dataset4.FirstOrDefault().Gst_No;
        //        string frgst = dataset2.FirstOrDefault().GstNo;


        //        if (clientGst != null && clientGst.Length > 4)
        //        {
        //            if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
        //            {
        //                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

        //                if (System.IO.File.Exists(path))
        //                {
        //                    lr.ReportPath = path;
        //                }

        //            }
        //            else
        //            {
        //                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

        //                if (System.IO.File.Exists(path))
        //                {
        //                    lr.ReportPath = path;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

        //            if (System.IO.File.Exists(path))
        //            {
        //                lr.ReportPath = path;
        //            }
        //        }




        //        ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
        //        ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
        //        ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
        //        ReportDataSource rd3 = new ReportDataSource("comp", dataset4);



        //        lr.DataSources.Add(rd);
        //        lr.DataSources.Add(rd1);
        //        lr.DataSources.Add(rd2);
        //        lr.DataSources.Add(rd3);

        //        string reportType = "pdf";
        //        string mimeType;
        //        string encoding;
        //        string fileNameExte;

        //        string deviceInfo =
        //            "<DeviceInfo>" +
        //            "<OutputFormat>" + "pdf" + "</OutputFormat>" +
        //            "<PageHeight>11in</PageHeight>" +
        //           "<Margintop>0.1in</Margintop>" +
        //             "<Marginleft>0.1in</Marginleft>" +
        //              "<Marginright>0.1in</Marginright>" +
        //               "<Marginbottom>0.5in</Marginbottom>" +
        //               "</DeviceInfo>";

        //        Warning[] warnings;
        //        string[] streams;
        //        byte[] renderByte;


        //        renderByte = lr.Render
        //      (reportType,
        //      deviceInfo,
        //      out mimeType,
        //      out encoding,
        //      out fileNameExte,
        //      out streams,
        //      out warnings
        //      );


        //        ViewBag.pdf = false;

        //        //if (submit == "Generate")
        //        //{
        //        ViewBag.pdf = true;
        //        ViewBag.invoiceno = invoice.invoiceno;
        //        //}
              

        //            MemoryStream memoryStream = new MemoryStream(renderByte);
        //        using (MailMessage mm = new MailMessage("billingpawanhansexpress@gmail.com", dataset4.FirstOrDefault().Email))
        //        //using (MailMessage mm = new MailMessage("billingpawanhansexpress@gmail.com", "codetentacles@gmail.com"))
        //        {
        //                mm.Subject = "Invoice";

        //                string Bodytext = "<html><body>Please Find Attachment</body></html>";
        //                Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");
        //                //Add Byte array as Attachment.




        //                mm.IsBodyHtml = true;



        //                mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

        //                AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
        //                // mm.Body = Bodytext;
        //                mm.Body = Bodytext;

        //                mm.Attachments.Add(attachment);
        //                SmtpClient smtp = new SmtpClient();
        //                smtp.Host = "relay-hosting.secureserver.net";
        //                // smtp.EnableSsl = true;
        //                mm.ReplyToList.Add("billingpawanhansexpress@gmail.com");
        //                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
        //                credentials.UserName = "billingpawanhansexpress@gmail.com";
        //                credentials.Password = "lalghati123";
        //                smtp.UseDefaultCredentials = false;
        //                smtp.Credentials = credentials;
        //                smtp.Port = 25;
        //                smtp.Send(mm);
        //            }

        //        string pathtext = Server.MapPath("~/Content/Invoices.txt");

        //        using (StreamWriter sw = (System.IO.File.Exists(pathtext)) ? System.IO.File.AppendText(pathtext) : System.IO.File.CreateText(pathtext))
        //        {

        //            sw.WriteLine(invoice.invoiceno + " " +"To" + " " + dataset4.FirstOrDefault().Email);

        //        }
        //    }

        //    return View();
        //}
        
        public void SavepdInvoice(string myParameter)
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



                string savePath = Server.MapPath("~/PDF/" +dataset4.FirstOrDefault().Company_Name + " " + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }

                

            }

        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cash cash = db.Cashes.Find(id);
            if (cash == null)
            {
                return HttpNotFound();
            }
            return View(cash);
        }

        // GET: demo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: demo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Cash_id,Amount,inserteddate,tempinserteddate,Invoiceno")] Cash cash)
        {
            if (ModelState.IsValid)
            {
                db.Cashes.Add(cash);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cash);
        }

        // GET: demo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cash cash = db.Cashes.Find(id);
            if (cash == null)
            {
                return HttpNotFound();
            }
            return View(cash);
        }

        // POST: demo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Cash_id,Amount,inserteddate,tempinserteddate,Invoiceno")] Cash cash)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cash).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cash);
        }

        // GET: demo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cash cash = db.Cashes.Find(id);
            if (cash == null)
            {
                return HttpNotFound();
            }
            return View(cash);
        }

        // POST: demo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cash cash = db.Cashes.Find(id);
            db.Cashes.Remove(cash);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        } 


        public ActionResult list()
        {
            List<Cash> list = new List<Cash>();
            return View(list);
        }
    }
}
