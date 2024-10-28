using DtDc_Billing.Entity_FR;
using DtDc_Billing.CustomModel;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Drawing;
using ZXing;
using DocumentFormat.OpenXml;
using System.Transactions;
using System.Windows.Forms;

namespace DtDc_Billing.Controllers

{
    [SessionAdmin]  
    public class StationeryController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
              

        // GET: Stationery
        public ActionResult Add()
        {
            return View();
        }

            
        [HttpPost]
        public ActionResult Add(StationaryModel stationary, string Submit)
        {
            string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


            if (ModelState.IsValid)
            {

                string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

                var checkDuplicate = db.Stationaries.Where(x => x.startno == stationary.startno && x.endno == stationary.endno).FirstOrDefault();
                if(checkDuplicate != null)
                {
                    TempData["duplicateError"] = "This series already exist";
                    return View(stationary);
                }
                var dataFid = (from d in db.Franchisees
                              where d.PF_Code == strpf
                               select d.F_Id).FirstOrDefault();

                Stationary St = new Stationary();

                St.Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString();
                St.startno = stationary.startno;
                St.endno = stationary.endno;
                St.noofbooks = stationary.noofbooks;
                St.noofleafs = stationary.noofleafs;
                St.Invoiceno = stationary.Invoiceno;
                St.Status = 0;
                string invdate = DateTime.ParseExact(stationary.temprecdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                St.Expiry_Date = Convert.ToDateTime(invdate);

                St.tempExpiry_Date = stationary.tempExpiry_Date;
                St.temprecdate = stationary.temprecdate;
                St.recieptdate = stationary.recieptdate;
                St.fid = Convert.ToInt32(dataFid);
                db.Stationaries.Add(St);
                db.SaveChanges();        

                ViewBag.Message = "Stationary Added SuccessFully";
                ModelState.Clear();

                if (Submit == "Print")
                {
                    List<string> series = GenerateSeries(stationary.startno, stationary.endno);

                    var getRD = series.ToList();
                    /////////generate barcode//////////////////

                    foreach (var getsingle in getRD)
                    {
                        ////////////////test print reciept////////////////////

                        string imageName = getsingle + "." + ImageType.Png;
                        string imagePath = "/BarcodeImages/" + imageName;
                        

                        //    string baseUrl = "https://frbilling.com/";     // create dynamic url rahter than static url of the frbiling
                        string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                           Request.ApplicationPath.TrimEnd('/');
                        //  Uri imageUri = new Uri(new Uri(baseUrl), imagePath);
                        //  string imageServerPath = imageUri.AbsoluteUri;

                        //string imageServerPath = Request.Url.GetLeftPart(UriPartial.Authority) + imagePath;
                        // string imageServerPath = "http://frbilling.com/BarcodeImages/" + imagePath;

                        string imageServerPath = Server.MapPath("~" + imagePath);


                        // Usage 
                        string data1 = getsingle; // Your barcode data
                        Image barcodeImage = GenerateBarcode(data1);

                        // Save the barcode image
                        // string imageServerPath = Server.MapPath("~" + imagePath);
                        barcodeImage.Save(imageServerPath, System.Drawing.Imaging.ImageFormat.Png);

                        // Dispose of the image
                        barcodeImage.Dispose();

                        var getRecipt = db.BarcodeAndPaths.Where(x => x.ConsignmentNo == getsingle).FirstOrDefault();
                        if (getRecipt == null)
                        {
                            BarcodeAndPath addBarcode = new BarcodeAndPath();
                            addBarcode.ConsignmentNo = getsingle;
                            addBarcode.FilePath = baseUrl + imagePath;
                            db.BarcodeAndPaths.Add(addBarcode);
                            db.SaveChanges();
                        }
                        else
                        {
                            getRecipt.FilePath = baseUrl + imagePath;
                            db.SaveChanges();
                        }
                        /////////generete barcode//////////////////
                    }
                    LocalReport lr = new LocalReport();

                    var getFinalList = db.BarcodeAndPaths.Where(x => series.Contains(x.ConsignmentNo)).ToList();

                    //  var getRD = db.Receipt_details.Where(x => x.Receipt_Id > 37509).ToList();

                    ////////////////////////////logic of 16 barcode on one page///////////////////////////////////////////////

                    // Example 1
                    int total = getFinalList.Count();

                    // Calculate the result and round up to the nearest integer
                    int result1 = (int)Math.Ceiling((double)total / 16);

                    // Multiply the rounded result by 8
                    int getFirstList = result1 * 8;
                    int secondList = (total - getFirstList);

                    var Recieptdetails1 = getFinalList.Take(getFirstList).ToList();

                    var Recieptdetails2 = getFinalList.Select(x => new { ConsignmentNo = x.ConsignmentNo, FilePath = x.FilePath }).Skip(getFirstList).Take(secondList).ToList();


                    ////////////////////////////logic of 16 barcode on one page/////////////////////////////////////////////
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintBarCodeMultiple.rdlc");

                    // Set the ReportPath property
                    lr.ReportPath = path;

                    ReportDataSource rd = new ReportDataSource("BarcodeMain", Recieptdetails1);

                    ReportDataSource rd2 = new ReportDataSource("BarCode", Recieptdetails2);

                    lr.DataSources.Add(rd);
                    lr.DataSources.Add(rd2);

                    lr.EnableExternalImages = true;

                    string reportType = "PDF";
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
                    (
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExte,
                        out streams,
                        out warnings
                    );

                    var namefile = "Barcode-" + stationary.startno.ToString() +"-"+ stationary.endno.ToString();

                    string savePath = Server.MapPath("~/PrintMulConsignmentPDF/" + namefile + ".pdf");

                    using (FileStream stream = new FileStream(savePath, FileMode.Create))
                    {
                        stream.Write(renderByte, 0, renderByte.Length);
                    }

                    var pdfFileName = namefile + ".pdf";

                    if (!string.IsNullOrEmpty(pdfFileName))
                    {
                        // Redirect to a new action that will open the PDF in a new tab
                        return RedirectToAction("OpenPdfInNewTab", new { pdfFileName });
                    }

                }
                return View();

            }

          
            return View(stationary);
        }



        public ActionResult Issue()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Issue(StationaryIssueModel issueModel)
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            if (ModelState.IsValid)
            {
                Issue issue = new Issue();

                issue.Pf_code = pfcode;

                issue.startno = issueModel.startno;
                issue.endno = issueModel.endno;
                issue.noofleafs = issueModel.noofleafs;
                issue.Inssuedate = issueModel.Inssuedate;
                issue.Comapny_Id = issueModel.Comapny_Id;
                issue.EmployeeName = issueModel.EmployeeName;

                db.Issues.Add(issue);
                db.SaveChanges();
                ViewBag.Message = "Issue Added SuccessFully";

                ModelState.Clear();

                return View();
            }

            return View(issueModel);
        }


        [HttpGet]
        public ActionResult Remaining(string PfCode=null, string RemainingType=null)
        {
            if (PfCode == null)
            {
                 PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            }
            List<RemainingModel> list = new List<RemainingModel>();

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "All", Value = "All" });

            items.Add(new SelectListItem { Text = "Remaining", Value = "Remaining" });

            items.Add(new SelectListItem { Text = "RemainingDone", Value = "RemainingDone" });

            ViewBag.RemainingType = items;
    
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.PfCode = new SelectList(db.Franchisees.Where(d=>d.PF_Code == pfcode), "PF_Code", "PF_Code");
            //ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            //return View(st);

            var obj = db.getRemaining(PfCode).Select(x => new RemainingModel
            {

                S_id = x.S_id,
                startno = x.startno,
                endno = x.endno,
                Expiry_Date = x.Expiry_Date,
                temprecdate = x.temprecdate,
                totalCount = x.totalCOUNTER ?? 0

            }).OrderByDescending(x=>x.temprecdate).ToList();


            ViewBag.type = RemainingType;

            if (obj != null)
            {
                return View(obj);
            }
            return View(list);
          
        }


        //[HttpPost]
        //public ActionResult Remaining(string PfCode, string RemainingType)
        //{


        //    //var st = db.Stationaries.Where(m => m.Pf_code == PfCode || PfCode == "").ToList();

        //    List<string> str = new List<string>();

        //    PfCode= Request.Cookies["Cookies"]["AdminValue"].ToString();

        //    ViewBag.PfCode = new SelectList(db.Franchisees.Where(d => d.PF_Code == PfCode), "PF_Code", "PF_Code");

        //    List<SelectListItem> items = new List<SelectListItem>();

        //    items.Add(new SelectListItem { Text = "All", Value = "All" });

        //    items.Add(new SelectListItem { Text = "Remaining", Value = "Remaining" });

        //    items.Add(new SelectListItem { Text = "RemainingDone", Value = "RemainingDone" });

        //    ViewBag.RemainingType = items;

        //    //if (PfCode == "")
        //    //{
        //    //    var obj = db.getRemainingAll().Select(x => new RemainingModel
        //    //    {


        //    //        startno = x.startno,
        //    //        endno = x.endno,
        //    //        Expiry_Date = x.Expiry_Date,
        //    //        temprecdate = x.temprecdate,
        //    //        totalCount = x.totalCOUNTER ?? 0

        //    //    }).ToList();



        //    //    ViewBag.type = RemainingType;

        //    //    return View(obj);
        //    //}
        //    //else
        //    //{
        //        var obj = db.getRemaining(PfCode).Select(x => new RemainingModel
        //        {

        //            S_id=x.S_id,
        //            startno = x.startno,
        //            endno = x.endno,
        //            Expiry_Date = x.Expiry_Date,
        //            temprecdate = x.temprecdate,
        //            totalCount = x.totalCOUNTER ?? 0

        //        }).ToList();


        //        ViewBag.type = RemainingType;
        //        return View(obj);


        //    //}

        //    //return View();



        //    }


        public JsonResult RemainingConsignments(string startno, string endno)
        {

            List<BarcodeAndPath> Consignmentswithbarcode = new List<BarcodeAndPath>();
            List<string> Consignments = new List<string>();

            //char stch = startno[0];
            //char Endch = endno[0];

            //long startConsignment = Convert.ToInt64(startno.Substring(1));
            //long EndConsignment = Convert.ToInt64(endno.Substring(1));



            //for (long i = startConsignment; i <= EndConsignment; i++)
            //{
            //    string updateconsignment = stch + i.ToString();


            //    var transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment && m.isDelete==false).FirstOrDefault();



            //    if (transaction == null || transaction.Customer_Id == null || transaction.Customer_Id.Length == 0)
            //    {
            //        Consignments.Add(updateconsignment);
            //    }

            //}

            string stch = startno[0].ToString();
            string Endch = endno[0].ToString();
            long startConsignment;
            long EndConsignment;
            if (startno.ToLower().StartsWith("7x") || endno.ToLower().StartsWith("7d"))
            {
                stch = startno.Substring(0, 2);
                Endch = endno.Substring(0, 2);
                startConsignment = Convert.ToInt64(startno.Substring(2));
                EndConsignment = Convert.ToInt64(endno.Substring(2));
            }
            else
            {
                startConsignment = Convert.ToInt64(startno.Substring(1));
                EndConsignment = Convert.ToInt64(endno.Substring(1));
            }

            //long startConsignment = Convert.ToInt64(startno.Substring(1));
            //long EndConsignment = Convert.ToInt64(endno.Substring(1));





            for (long i = startConsignment; i <= EndConsignment; i++)
            {
                string updateconsignment = stch + i.ToString();

                DtDc_Billing.Entity_FR.Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();

                if (transaction == null || transaction.Customer_Id == null || transaction.Customer_Id.Length == 0)
                {
                    Consignments.Add(updateconsignment);
                }

            }
     //       foreach (var getConsignement in Consignments)
     //       {
     //           //////if barcode is not generated/////////
     //           string imageName = getConsignement + "." + ImageType.Png;
     //           string imagePath = "/BarcodeImages/" + imageName;
              

     //           //string baseUrl = "https://frbilling.com/"; use Dynamic url rather than static url
     //           string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
     //Request.ApplicationPath.TrimEnd('/');
     //           string imageServerPath = Server.MapPath("~" + imagePath);


     //           // Usage 
     //           string data1 = getConsignement; // Your barcode data
     //           Image barcodeImage = GenerateBarcode(data1);

     //           // Save the barcode image
     //           // string imageServerPath = Server.MapPath("~" + imagePath);
     //           barcodeImage.Save(imageServerPath, System.Drawing.Imaging.ImageFormat.Png);

     //           // Dispose of the image
     //           barcodeImage.Dispose();

     //           var getRecipt = db.BarcodeAndPaths.Where(x => x.ConsignmentNo == getConsignement).FirstOrDefault();
     //           if (getRecipt == null)
     //           {
     //               BarcodeAndPath addBarcode = new BarcodeAndPath();
     //               addBarcode.ConsignmentNo = getConsignement;
     //               addBarcode.FilePath = baseUrl + imagePath;
     //               db.BarcodeAndPaths.Add(addBarcode);
     //               db.SaveChanges();
     //           }
     //           else
     //           {
     //               getRecipt.FilePath = baseUrl + imagePath;
     //               db.SaveChanges();
     //           }
     //           //////if barcode is not generated end/////////



     //           BarcodeAndPath addSingle = new BarcodeAndPath();
     //           addSingle.ConsignmentNo = getConsignement;
     //           addSingle.FilePath = baseUrl + imagePath;
     //           Consignmentswithbarcode.Add(addSingle);

     //       }

            return Json(Consignments, JsonRequestBehavior.AllowGet);

        }


        public ActionResult IsseueRemaining()
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


                    var transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment && m.isDelete==false).FirstOrDefault();


                    if (transaction != null && transaction.Customer_Id != null && transaction.Customer_Id.Length > 1)
                    {
                        counter++;
                    }


                }


                str.Add(counter.ToString());
                counter = 0;




            }

            ViewBag.str = str.ToArray();

            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View(st);

        }

        public ActionResult Employeeautocomplete()
        {


            var entity = db.Issues.
Select(e => new
{
    e.EmployeeName
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditStationary(long id)
        {
            Stationary stationary = db.Stationaries.Find(id);

            if (stationary == null)
            {
                return HttpNotFound();
            }
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
            return View(stationary);


        }


        [HttpPost]
        public ActionResult EditStationary(StationaryModel stationary)
        {

            string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

            if (ModelState.IsValid)
            {


                var dataFid = (from d in db.Franchisees
                               where d.PF_Code == Request.Cookies["Cookies"]["AdminValue"].ToString()
                               select d.Firm_Id).FirstOrDefault();

                Stationary St = new Stationary();

                St.Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString();
                St.startno = stationary.startno;
                St.endno = stationary.endno;
                St.noofbooks = stationary.noofbooks;
                St.noofleafs = stationary.noofleafs;
                St.Invoiceno = stationary.Invoiceno;
                St.Status = 0;
                string invdate = DateTime.ParseExact(stationary.temprecdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                St.Expiry_Date = Convert.ToDateTime(invdate);

                St.tempExpiry_Date = stationary.tempExpiry_Date;
                St.temprecdate = stationary.temprecdate;
                St.recieptdate = stationary.recieptdate;
                St.fid = Convert.ToInt32(dataFid);             
                db.SaveChanges();

                // db.Entry(stationary).State = System.Data.Entity.EntityState.Modified;

                ViewBag.Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString(); //new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
                ViewBag.Message = "Stationary Updated SuccessFully";
                ModelState.Clear();
                return View();

            }

            ViewBag.Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString(); // new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
            return View(stationary);


        }

        [HttpGet]
        public ActionResult DeleteStationary(int stationary_id, string type, bool isDelete = false)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            if (isDelete)
            {
                var checkStationaryNo = db.Stationaries.Where(x => x.S_id == stationary_id && x.Pf_code == strpf).FirstOrDefault();
                if (checkStationaryNo == null)
                {
                    TempData["error"] = "Invalid Stationary No";

                }
                else
                {
                    db.Stationaries.Remove(checkStationaryNo);
                    db.SaveChanges();
                    TempData["success"] = " Stationary Delete successfully";
                }
               
            }
            return RedirectToAction("Remaining", new { PfCode = strpf, RemainingType=type });
        
        }

        public ActionResult BulkBarcodePrint()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BulkBarcodePrint(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase == null)
            {
                TempData["error"] = "Upload excel file first";
                return View();
            }

            var pdfFileName = PrintMultipleBarCode(httpPostedFileBase);

            if (!string.IsNullOrEmpty(pdfFileName))
            {
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
     Request.ApplicationPath.TrimEnd('/') + "/";
                string filePath = baseUrl + "PrintMulConsignmentPDF/" + pdfFileName;

                ViewBag.file = filePath;
            }

            return View();
        }


        public string PrintMultipleBarCode(HttpPostedFileBase httpPostedFileBase)
        {
            // Initialize a list to store Consignment_no values
            List<string> consignmentNumbers = new List<string>();

            if (httpPostedFileBase != null)
            {

                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    // Set the LicenseContext property
                   // ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial

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

                            var Consignment_no = (workSheet?.Cells[rowIterator, 1]?.Value?.ToString().Trim());
                            if (!string.IsNullOrEmpty(Consignment_no))
                            {
                                // Add Consignment_no to the list
                                consignmentNumbers.Add(Consignment_no);
                            }

                        }
                    }
                }
            }

            // Use LINQ to retrieve data from the database based on the Consignment_no values
            var getRD = consignmentNumbers.ToList();
            /////////generate barcode//////////////////

            foreach (var getsingle in getRD)
            {
                ////////////////test print reciept////////////////////

                string imageName = getsingle + "." + ImageType.Png;
                string imagePath = "/BarcodeImages/" + imageName;
              

             //   string baseUrl = "https://frbilling.com/";
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
       Request.ApplicationPath.TrimEnd('/');
                //  Uri imageUri = new Uri(new Uri(baseUrl), imagePath);
                //  string imageServerPath = imageUri.AbsoluteUri;

                //string imageServerPath = Request.Url.GetLeftPart(UriPartial.Authority) + imagePath;
                // string imageServerPath = "http://frbilling.com/BarcodeImages/" + imagePath;

                string imageServerPath = Server.MapPath("~" + imagePath);


                // Usage 
                string data1 = getsingle; // Your barcode data
                Image barcodeImage = GenerateBarcode(data1);

                // Save the barcode image
                // string imageServerPath = Server.MapPath("~" + imagePath);
                barcodeImage.Save(imageServerPath, System.Drawing.Imaging.ImageFormat.Png);

                // Dispose of the image
                barcodeImage.Dispose();

                var getRecipt = db.BarcodeAndPaths.Where(x => x.ConsignmentNo == getsingle).FirstOrDefault();
                if (getRecipt == null)
                {
                    BarcodeAndPath addBarcode = new BarcodeAndPath();
                    addBarcode.ConsignmentNo = getsingle;
                    addBarcode.FilePath = baseUrl + imagePath;
                    db.BarcodeAndPaths.Add(addBarcode);
                    db.SaveChanges();
                }
                else
                {
                    getRecipt.FilePath = baseUrl + imagePath;
                    db.SaveChanges();
                }
                /////////generete barcode//////////////////
            }
            LocalReport lr = new LocalReport();

            var getFinalList = db.BarcodeAndPaths.Where(x => consignmentNumbers.Contains(x.ConsignmentNo)).ToList();

            //  var getRD = db.Receipt_details.Where(x => x.Receipt_Id > 37509).ToList();

            ////////////////////////////logic of 16 barcode on one page///////////////////////////////////////////////

            // Example 1
            int total = getFinalList.Count();

            // Calculate the result and round up to the nearest integer
            int result1 = (int)Math.Ceiling((double)total / 16);

            // Multiply the rounded result by 8
            int getFirstList = result1 * 8;
            int secondList = (total - getFirstList);

            var Recieptdetails1 = getFinalList.Take(getFirstList).ToList();

            var Recieptdetails2 = getFinalList.Select(x => new { ConsignmentNo = x.ConsignmentNo, FilePath = x.FilePath }).Skip(getFirstList).Take(secondList).ToList();


            ////////////////////////////logic of 16 barcode on one page/////////////////////////////////////////////
            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintBarCodeMultiple.rdlc");

            // Set the ReportPath property
            lr.ReportPath = path;

            ReportDataSource rd = new ReportDataSource("BarcodeMain", Recieptdetails1);

            ReportDataSource rd2 = new ReportDataSource("BarCode", Recieptdetails2);

            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd2);

            lr.EnableExternalImages = true;

            string reportType = "PDF";
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
            (
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExte,
                out streams,
                out warnings
            );

            var namefile = "BulkBarcodes-" + DateTime.Now.Ticks;

            string savePath = Server.MapPath("~/PrintMulConsignmentPDF/" + namefile + ".pdf");

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                stream.Write(renderByte, 0, renderByte.Length);
            }

            return namefile + ".pdf";

        }


        public Image GenerateBarcode(string data)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 100,
                    Width = 300,
                    PureBarcode = true
                }
            };

            return writer.Write(data);
        }

        public ActionResult OpenPdfInNewTab(string pdfFileName)
        {
            if (!string.IsNullOrEmpty(pdfFileName))
            {
                // Get the full path to the generated PDF file
                string filePath = Server.MapPath("~/PrintMulConsignmentPDF/" + pdfFileName);

                // Return the PDF file with the appropriate content type
                return File(filePath, "application/pdf", pdfFileName);
            }

            // Handle the situation where the PDF file name is not provided
            TempData["error"] = "PDF file not found";
            return View();
        }


        static List<string> GenerateSeries(string startValue, string endValue)
        {
            List<string> series = new List<string>();
            var slength = startValue.Length;
            var elength=endValue.Length;    
            // Extract the prefix and numeric part from the start and end values
            string prefix = "";
            int startNumber = ExtractNumericPart(startValue, out prefix);
            int endNumber = ExtractNumericPart(endValue, out _);

            // Generate the series
            for (int i = startNumber; i <= endNumber; i++)
            {
                var paddedNumber = i.ToString().PadLeft(slength -i.ToString().Length, '0');

                var finalvalue = prefix + paddedNumber;
                series.Add(finalvalue);
            }

            return series;
        }

        static int ExtractNumericPart(string value, out string prefix)
        {
            int numericPartStartIndex = 0;

            // Find the index where the numeric part starts
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsDigit(value[i]))
                {
                    numericPartStartIndex = i;
                    break;
                }
            }

            // Extract the prefix
            prefix = value.Substring(0, numericPartStartIndex);

            // Extract the numeric part and parse it
            int numericPart = int.Parse(value.Substring(numericPartStartIndex));

            return numericPart;
        }

    }
}
