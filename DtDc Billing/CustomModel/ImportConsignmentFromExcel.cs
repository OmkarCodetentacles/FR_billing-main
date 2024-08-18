using DocumentFormat.OpenXml.Spreadsheet;
using DtDc_Billing.Controllers;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Util;

namespace DtDc_Billing.CustomModel
{
    public class ImportConsignmentFromExcel
    {
        public static db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        public string Import1Async(HttpPostedFileBase httpPostedFileBase,string PfCode)
        {
            var damageResult =  Task.Run(() => asyncImportFromExcel(httpPostedFileBase, PfCode));

            return damageResult.ToString();
        }
        public static async Task<string> asyncImportFromExcel(HttpPostedFileBase httpPostedFileBase,string PfCode)
        {
            if (httpPostedFileBase != null)
            {
                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    #region getting cookies pf code
;
                    var getPfcode = PfCode;

                    #endregion

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var tran = new Transaction();

                            //var consno= workSheet.Cells[rowIterator, 2].Value.ToString().Trim() ?? null;
                            //var custid= (workSheet?.Cells[rowIterator, 3]?.Value?.ToString());

                            tran.Consignment_no = (workSheet?.Cells[rowIterator, 2]?.Value?.ToString().Trim());
                            tran.Customer_Id = (workSheet?.Cells[rowIterator, 3]?.Value?.ToString().Trim());
                             
                            if (tran.Consignment_no != null || tran.Customer_Id != null)
                            {


                                Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no).FirstOrDefault();

                                var validcomp = db.Companies.Where(m => m.Company_Id == tran.Customer_Id && m.Pf_code == getPfcode).FirstOrDefault();
                                var Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id && m.Pf_code == getPfcode).Select(m => m.Pf_code).FirstOrDefault();

                                if (Pf_Code != null)
                                {
                                    if (transaction != null)
                                    {

                                        CalculateAmount ca = new CalculateAmount();
                                        double? amt = 0;
                                        if (transaction.Pincode != null && transaction.Pincode != "NULL " && validcomp != null)
                                        {
                                            amt = ca.CalulateAmt(transaction.Consignment_no, tran.Customer_Id, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                                            transaction.Amount = amt;


                                            transaction.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                            transaction.AdminEmp = 000;
                                        }

                                        transaction.Customer_Id = tran.Customer_Id;
                                        transaction.Pf_Code = getPfcode;
                                        db.Entry(transaction).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }

                        }
                    }

                   // ViewBag.Success = "Excel File Uploaded SuccessFully";
                }
            }
            return "1";
        }


        public string Import2Async(HttpPostedFileBase httpPostedFileBase, string PfCode)
        {
            var damageResult = Task.Run(() => asyncImportFromExcelWhole(httpPostedFileBase, PfCode));

            return damageResult.ToString();
        }
        public static async Task<string> asyncImportFromExcelWhole(HttpPostedFileBase httpPostedFileBase,string PfCode)
        {
            if (httpPostedFileBase != null)
            {
                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy","d/M/yyyy", "dd MMM yyyy"};

                    #region getting cookies pf code

                    BookingController admin = new BookingController();
                    var getPfcode = PfCode;

                    #endregion
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var tran = new Transaction();
                            double insuranceamt = 0;
                            double FOVamt = 0, fovper = 0;


                            // tran.Consignment_no = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();

                            tran.Consignment_no = (workSheet?.Cells[rowIterator, 2]?.Value?.ToString().Trim());
                            tran.Customer_Id = (workSheet?.Cells[rowIterator, 3]?.Value?.ToString());
                            tran.chargable_weight = Convert.ToDouble(workSheet.Cells[rowIterator, 4].Value);
                            //  tran.Customer_Id = workSheet.Cells[rowIterator, 4].Value.ToString();
                            //tran.Insurance = workSheet.Cells[rowIterator, 5].Value.ToString();

                            insuranceamt = Convert.ToDouble(workSheet.Cells[rowIterator, 5].Value);
                            FOVamt = Convert.ToDouble(workSheet.Cells[rowIterator, 6].Value);
                            fovper = Convert.ToDouble(workSheet.Cells[rowIterator, 7].Value);
                            tran.loadingcharge = Convert.ToDouble(workSheet.Cells[rowIterator, 8].Value);



                            if (tran.Consignment_no != null || tran.Customer_Id != null)
                            {
                                Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no && m.Pf_Code == getPfcode).FirstOrDefault();
                                var Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id && m.Pf_code == getPfcode).Select(m => m.Pf_code).FirstOrDefault();

                                if (Pf_Code != null)
                                {
                                    if (transaction != null)
                                    {

                                        CalculateAmount ca = new CalculateAmount();
                                        var validcomp = db.Companies.Where(m => m.Company_Id == tran.Customer_Id).FirstOrDefault();

                                        var company = db.Companies.Where(m => m.Company_Id == tran.Customer_Id).Select(m => new { m.Pf_code, m.Minimum_Risk_Charge, m.Insurance }).FirstOrDefault();
                                        if (transaction.Pincode != null && transaction.Pincode != "NULL " && validcomp != null)
                                        {
                                            double? amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, transaction.Pincode, transaction.Mode, Convert.ToDouble(tran.chargable_weight), transaction.Type_t);

                                            transaction.Amount = amt;
                                            transaction.chargable_weight = tran.chargable_weight;
                                            transaction.Insurance = "no";

                                            transaction.Pf_Code = company.Pf_code;
                                        }
                                        transaction.Customer_Id = tran.Customer_Id;

                                        transaction.Consignment_no = tran.Consignment_no.Trim();




                                        if (insuranceamt > 0 && transaction.Type_t == "N" && validcomp != null)
                                        {
                                            transaction.Insurance = "yes";
                                            transaction.BillAmount = insuranceamt;
                                            transaction.Percentage = company.Insurance.ToString();
                                            transaction.Risksurcharge = Math.Round((transaction.BillAmount ?? 0) * (company.Insurance ?? 0), 2);
                                            if (company.Minimum_Risk_Charge > transaction.Risksurcharge)
                                                transaction.Risksurcharge = company.Minimum_Risk_Charge;
                                        }
                                        else if (FOVamt > 0 && transaction.Type_t == "N" && validcomp != null)
                                        {
                                            transaction.Insurance = "no";
                                            transaction.BillAmount = FOVamt;
                                            transaction.Percentage = fovper.ToString();
                                            transaction.Risksurcharge = Math.Round((transaction.BillAmount ?? 0) * fovper, 2);
                                            if (company.Minimum_Risk_Charge > transaction.Risksurcharge)
                                                transaction.Risksurcharge = company.Minimum_Risk_Charge;
                                        }


                                        transaction.AdminEmp = 000;

                                        db.Entry(transaction).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }

                            }
                        }
                    }

                }
            }
            return "1";
        }


        public string Import3Async(HttpPostedFileBase httpPostedFileBase, string PfCode)
        {
            try
            {
                var damageResult = Task.Run(() => asyncAddNewimporFromExcel(httpPostedFileBase, PfCode));

                return damageResult.ToString();
            }
            catch (Exception ex)
            {
                throw new RedirectException(ex.Message);
            }
        }
       
        public static async Task<string> asyncAddNewimporFromExcel(HttpPostedFileBase httpPostedFileBase,string PfCode)
        {
           

            if (httpPostedFileBase != null)
            {


                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                  
                   // string[] formats = { "dd-MM-yyyy","dd-MMM-yyyy", "yyyy-MM-dd",
                   //"dd-MM-yyyy", "M/d/yyyy","d/M/yyyy", "dd MMM yyyy","MM-dd-yyyy","M-d-yyyy","dd/MM/yyyy","d-M-yyyy","d-MM-yyyy","d/MM/yyyy" ,"dd/M/yyyy","M/d/yyyy h:mm:ss tt","d/M/yyyy h:mm:ss tt","MM/dd/yyyy h:mm:ss tt","dd/MM/yyyy h:mm:ss tt","M/dd/yyyy h:mm:ss tt","d/MM/yyyy h:mm:ss tt","MM/d/yyyy h:mm:ss tt","dd/M/yyyy h:mm:ss tt"};

                     string[] formats = { "dd/MM/yyyy","dd-MM-yyyy", "dd-MMM-yyyy", "d/MM/yyyy","d-MM-yyyy","dd/M/yyyy","dd-M-yyyy" /*"d /M/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss tt" ,"d-M-yyyy h:mm:ss tt", "dd-MM-yyyy h:mm:ss tt"*//*"M/d/yyyy h:mm:ss tt", "MM/dd/yyyy h:mm:ss tt"*/ };
                    #region getting cookies pf code

                    BookingController admin = new BookingController();
                    var getPfcode = PfCode;

                    #endregion

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var tran = new Transaction();

                            try
                            {
                                if (workSheet.Cells[rowIterator, 8]?.Value?.ToString()!=null)
                            {
                                tran.Consignment_no = workSheet.Cells[rowIterator, 2]?.Value?.ToString().Trim();
                                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no && m.Pf_Code == getPfcode).FirstOrDefault();



                                    //       tran.chargable_weight =  Convert.ToDouble(workSheet.Cells[rowIterator, 3]?.Value);
                                    tran.chargable_weight = workSheet.Cells[rowIterator, 3]?.Value != null
                                          ? Convert.ToDouble(workSheet.Cells[rowIterator, 3]?.Value)
                                          : transaction.chargable_weight;

                                    // tran.Mode = workSheet.Cells[rowIterator, 4]?.Value?.ToString().ToUpper();

                                    tran.Mode = !string.IsNullOrEmpty(workSheet.Cells[rowIterator, 4]?.Value?.ToString().ToUpper())
                                       ? workSheet.Cells[rowIterator, 4]?.Value?.ToString().ToUpper()
                                       : transaction.Mode;

                                    //tran.compaddress = (workSheet?.Cells[rowIterator, 5]?.Value?.ToString());
                                    tran.compaddress = !string.IsNullOrEmpty(workSheet.Cells[rowIterator, 5]?.Value?.ToString())
                                    ? workSheet.Cells[rowIterator, 5]?.Value?.ToString()
                                    : transaction.compaddress;

                                    //  tran.Quanntity = Convert.ToInt16(workSheet.Cells[rowIterator, 6]?.Value);
                                    tran.Quanntity = workSheet.Cells[rowIterator, 6]?.Value != null
                                     ? Convert.ToInt16(workSheet.Cells[rowIterator, 6]?.Value)
                                     : transaction.Quanntity;

                                    //  tran.Pincode = workSheet.Cells[rowIterator, 7]?.Value?.ToString();
                                    tran.Pincode = !string.IsNullOrEmpty(workSheet.Cells[rowIterator, 7]?.Value?.ToString())
              ? workSheet.Cells[rowIterator, 7]?.Value?.ToString()
              : transaction.Pincode;

                                    string dateString = workSheet.Cells[rowIterator, 8]?.Value?.ToString();
                                    DateTime dateTime;
                                    ////  tran.tembookingdate = tran.booking_date.Value.ToString("dd-MM-yyyy");
                                    object cellValue = workSheet.Cells[rowIterator, 8]?.Value; // Assuming the date is in the 8th column (column H)


                                    if (!string.IsNullOrEmpty(dateString))
                                    {
                                        if (cellValue != null && cellValue is DateTime)
                                        {
                                            DateTime excelDate = (DateTime)cellValue;
                                            tran.booking_date = excelDate;
                                            tran.tembookingdate = excelDate.ToString("dd-MM-yyyy"); // If needed, store formatted date
                                        }

                                        // Check if the dateString can be parsed as a double (Excel serial date number)
                                        else if (double.TryParse(dateString, out double excelDateNumber))
                                        {
                                            dateTime = DateTime.FromOADate(excelDateNumber);
                                            string formattedDate = DateTime.FromOADate(excelDateNumber).ToString("MM/dd/yyyy");

                                            // Convert the formatted date string back to DateTime
                                            DateTime formattedDateTime = DateTime.ParseExact(formattedDate, "MM/dd/yyyy", null);

                                            // Set the booking date
                                            tran.booking_date = formattedDateTime;

                                            // Set the tembookingdate
                                            tran.tembookingdate = formattedDateTime.ToString("dd-MM-yyyy");
                                        }
                                        else
                                        {
                                            // parse the date string with the specified format
                                            if (DateTime.TryParseExact(dateString, formats, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
                                            {
                                                // Convert the DateTime object to the Excel date number
                                                double excelDateNumber1 = parsedDateTime.ToOADate();

                                                // Format the Excel date number as MM/dd/yyyy
                                                string formattedDate = DateTime.FromOADate(excelDateNumber1).ToString("MM/dd/yyyy");

                                                // Convert the formatted date string back to DateTime
                                                DateTime formattedDateTime = DateTime.ParseExact(formattedDate, "MM/dd/yyyy", null);

                                                // Set the booking date
                                                tran.booking_date = formattedDateTime;

                                                // Set the tembookingdate
                                                tran.tembookingdate = formattedDateTime.ToString("dd-MM-yyyy");
                                            }
                                            else
                                            {
                                                if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                                                {
                                                    // Convert the DateTime object to the Excel date number
                                                    double excelDateNumberd = dateTime.ToOADate();

                                                    // Format the Excel date number as MM/dd/yyyy
                                                    string formattedDate = DateTime.FromOADate(excelDateNumberd).ToString("MM/dd/yyyy");

                                                    // Convert the formatted date string back to DateTime
                                                    DateTime formattedDateTime = DateTime.ParseExact(formattedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                                                    // Set the booking date
                                                    tran.booking_date = formattedDateTime;

                                                    // Set the tembookingdate
                                                    tran.tembookingdate = formattedDateTime.ToString("dd-MM-yyyy");
                                                }

                                            }
                                        }


                                    }



                                    // tran.Type_t = workSheet.Cells[rowIterator, 9]?.Value?.ToString();
                                    tran.Type_t = !string.IsNullOrEmpty(workSheet.Cells[rowIterator, 9]?.Value?.ToString())
                                     ? workSheet.Cells[rowIterator, 9]?.Value?.ToString()
                                     : transaction.Type_t;

                                    //  tran.Customer_Id = workSheet.Cells[rowIterator, 10]?.Value?.ToString();

                                    tran.Customer_Id = !string.IsNullOrEmpty(workSheet.Cells[rowIterator, 10]?.Value?.ToString())
                                    ? workSheet.Cells[rowIterator, 10]?.Value?.ToString()
                                    : transaction.Customer_Id;

                                    // double? loadingChargeValue = workSheet?.Cells[rowIterator, 11]?.Value as double?;
                                   // tran.loadingcharge = loadingChargeValue ?? 0.0;
                                    tran.loadingcharge = workSheet.Cells[rowIterator, 11]?.Value is double loadingChargeValue
                                        ? loadingChargeValue
                                        : transaction.loadingcharge;

                                    //tran.Receiver = workSheet.Cells[rowIterator, 12]?.Value?.ToString();
                                    tran.Receiver = !string.IsNullOrEmpty(workSheet.Cells[rowIterator, 12]?.Value?.ToString())
                                    ? workSheet.Cells[rowIterator, 12]?.Value?.ToString()
                                    : transaction.Receiver;

                                    //double?  amount= workSheet?.Cells[rowIterator, 13]?.Value as double?;
                                    //tran.Amount = amount ?? 0.0;
                                    tran.Amount = workSheet.Cells[rowIterator, 13]?.Value is double amount
                                           ? amount
                                           : tran.Amount;

                                    var Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id && m.Pf_code==getPfcode).Select(m => m.Pf_code).FirstOrDefault();

                                if (Pf_Code!=null)
                                {

                                    if (transaction != null)
                                     {

                                         CalculateAmount ca = new CalculateAmount();
                                            double? amt = 0;
                                            if (tran.Amount==null || tran.Amount==0.0)
                                            {
                                               amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);
                                                transaction.Amount =Math.Round( (double)amt);

                                            }
                                            else
                                            {
                                                transaction.Amount = tran.Amount;
                                            }

                                            transaction.Customer_Id = tran.Customer_Id;

                                        transaction.Consignment_no = tran.Consignment_no.Trim();
                                        transaction.chargable_weight = tran.chargable_weight;
                                        transaction.Mode = tran.Mode;
                                        transaction.compaddress = tran.compaddress;
                                        transaction.Quanntity = tran.Quanntity;
                                        transaction.Pincode = tran.Pincode;
                                        transaction.booking_date = tran.booking_date;
                                        transaction.Type_t = tran.Type_t;
                                        transaction.tembookingdate = tran.tembookingdate;
                                        transaction.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                        transaction.AdminEmp = 000;



                                        db.Entry(transaction).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        CalculateAmount ca = new CalculateAmount();
                                            if(tran.Amount == null || tran.Amount == 0)
                                            {
                                                double? amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);

                                                tran.Amount = Math.Round((double)amt);
                                            }
                                       tran.Amount = Convert.ToDouble(tran.Amount);
                                        tran.Customer_Id = tran.Customer_Id;

                                        tran.Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                        tran.AdminEmp = 000;


                                        db.Transactions.Add(tran);
                                            db.SaveChanges();
                                      
                                    }

                                }
                            }

                            }
                            catch (Exception ex)
                            {
                                throw new RedirectException(ex.Message);
                            }
                        }
                    }

                }
            }
            return "1";
        }

        public string Import4Async(HttpPostedFileBase httpPostedFileBase, string PfCode)
        {
            try
            {
                var damageResult = Task.Run(() => asyncAddNewDTDCimporFromExcel(httpPostedFileBase, PfCode));

                return damageResult.ToString();
            }
            catch (Exception ex)
            {
                throw new RedirectException(ex.Message);
            }
        }

        public static async Task<string> asyncAddNewDTDCimporFromExcel(HttpPostedFileBase httpPostedFileBase, string PfCode)
        {


            if (httpPostedFileBase != null)
            {


                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {

                   
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));


                    // string[] formats = { "dd-MM-yyyy","dd-MMM-yyyy", "yyyy-MM-dd",
                    //"dd-MM-yyyy", "M/d/yyyy","d/M/yyyy", "dd MMM yyyy","MM-dd-yyyy","M-d-yyyy","dd/MM/yyyy","d-M-yyyy","d-MM-yyyy","d/MM/yyyy" ,"dd/M/yyyy","M/d/yyyy h:mm:ss tt","d/M/yyyy h:mm:ss tt","MM/dd/yyyy h:mm:ss tt","dd/MM/yyyy h:mm:ss tt","M/dd/yyyy h:mm:ss tt","d/MM/yyyy h:mm:ss tt","MM/d/yyyy h:mm:ss tt","dd/M/yyyy h:mm:ss tt"};

                    string[] formats = { "dd/MM/yyyy", "dd-MM-yyyy", "dd-MMM-yyyy", "d/MM/yyyy", "d-MM-yyyy", "dd/M/yyyy", "dd-M-yyyy" /*"d /M/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss tt" ,"d-M-yyyy h:mm:ss tt", "dd-MM-yyyy h:mm:ss tt"*//*"M/d/yyyy h:mm:ss tt", "MM/dd/yyyy h:mm:ss tt"*/ };
                    #region getting cookies pf code

                    BookingController admin = new BookingController();
                    var getPfcode = PfCode;

                    #endregion

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var tran = new Transaction();

                            try
                            {
                               
                                    tran.Consignment_no = workSheet.Cells[rowIterator, 2]?.Value?.ToString().Trim();
                                    tran.Pf_Code=workSheet.Cells[rowIterator, 4]?.Value.ToString().Trim();
                                    tran.Actual_weight = Convert.ToDouble(workSheet.Cells[rowIterator, 5]?.Value);
                                    tran.Mode = workSheet.Cells[rowIterator, 6]?.Value?.ToString().Trim();
                                    tran.Quanntity = Convert.ToInt16(workSheet.Cells[rowIterator, 9]?.Value);
                                     tran.Pincode = workSheet.Cells[rowIterator, 10]?.Value?.ToString();
                                   tran.dtdcamount = Convert.ToDouble(workSheet.Cells[rowIterator, 12]?.Value);   
                                    tran.chargable_weight = Convert.ToDouble(workSheet.Cells[rowIterator, 5]?.Value);
                                    tran.compaddress = (workSheet?.Cells[rowIterator, 5]?.Value?.ToString());
                                    tran.topay = "no";
                                    tran.cod = "no";
 
                                  tran.Type_t = workSheet.Cells[rowIterator, 17]?.Value?.ToString();
                                tran.BillAmount = Convert.ToDouble(workSheet.Cells[rowIterator, 22]?.Value);
                                if (tran.BillAmount == 0.00)
                                {
                                    tran.Insurance = "nocoverage";
                                }
                                else
                                {
                                    tran.Insurance = "ownerrisk";
                                }


                               // tran.Customer_Id = workSheet.Cells[rowIterator, 10]?.Value?.ToString();
                               
                                double? amount = workSheet?.Cells[rowIterator, 13]?.Value as double?;
                                tran.Amount = amount ?? 0.0;

                                string dateString = workSheet.Cells[rowIterator, 11]?.Value?.ToString();

                                    DateTime dateTime;

                                    //// parse the date string with the specified format
                                    //if (datetime.tryparseexact(datestring, formats, null, system.globalization.datetimestyles.none, out datetime))
                                    //{
                                    //    // convert the datetime object to the excel date number
                                    //    double exceldatenumber = datetime.tooadate();

                                    //    // format the excel date number as mm/dd/yyyy
                                    //    string formatteddate = datetime.fromoadate(exceldatenumber).tostring("mm/dd/yyyy");

                                    //    // convert the formatted date string back to datetime
                                    //    datetime formatteddatetime = datetime.parseexact(formatteddate, "mm/dd/yyyy", null);

                                    //    // set the booking date
                                    //    tran.booking_date = formatteddatetime;
                                    //   // tran.tembookingdate = datetime.tostring("dd-mm-yyyy");
                                    //    // set the tembookingdate
                                    //    tran.tembookingdate = formatteddatetime.tostring("dd-mm-yyyy");
                                    //}


                                    ////  tran.tembookingdate = tran.booking_date.Value.ToString("dd-MM-yyyy");
                                    object cellValue = workSheet.Cells[rowIterator, 8]?.Value; // Assuming the date is in the 8th column (column H)

                                    if (cellValue != null && cellValue is DateTime)
                                    {
                                        DateTime excelDate = (DateTime)cellValue;
                                        tran.booking_date = excelDate;
                                        tran.tembookingdate = excelDate.ToString("dd-MM-yyyy"); // If needed, store formatted date
                                    }

                                    // Check if the dateString can be parsed as a double (Excel serial date number)
                                    else if (double.TryParse(dateString, out double excelDateNumber))
                                    {
                                        dateTime = DateTime.FromOADate(excelDateNumber);
                                        string formattedDate = DateTime.FromOADate(excelDateNumber).ToString("MM/dd/yyyy");

                                        // Convert the formatted date string back to DateTime
                                        DateTime formattedDateTime = DateTime.ParseExact(formattedDate, "MM/dd/yyyy", null);

                                        // Set the booking date
                                        tran.booking_date = formattedDateTime;

                                        // Set the tembookingdate
                                        tran.tembookingdate = formattedDateTime.ToString("dd-MM-yyyy");
                                    }
                                    else
                                    {
                                        // parse the date string with the specified format
                                        if (DateTime.TryParseExact(dateString, formats, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
                                        {
                                            // Convert the DateTime object to the Excel date number
                                            double excelDateNumber1 = parsedDateTime.ToOADate();

                                            // Format the Excel date number as MM/dd/yyyy
                                            string formattedDate = DateTime.FromOADate(excelDateNumber1).ToString("MM/dd/yyyy");

                                            // Convert the formatted date string back to DateTime
                                            DateTime formattedDateTime = DateTime.ParseExact(formattedDate, "MM/dd/yyyy", null);

                                            // Set the booking date
                                            tran.booking_date = formattedDateTime;

                                            // Set the tembookingdate
                                            tran.tembookingdate = formattedDateTime.ToString("dd-MM-yyyy");
                                        }
                                    }



                                   
                                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no && m.Pf_Code == getPfcode).FirstOrDefault();
                                    var Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id && m.Pf_code == getPfcode).Select(m => m.Pf_code).FirstOrDefault();

                                    if (Pf_Code != null)
                                    {

                                        if (transaction != null)
                                        {

                                            CalculateAmount ca = new CalculateAmount();
                                            double? amt = 0;
                                            if (tran.Amount == null || tran.Amount == 0.0)
                                            {
                                                amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);
                                                transaction.Amount = Math.Round((double)amt);

                                            }
                                            else
                                            {
                                                transaction.Amount = tran.Amount;
                                            }

                                            transaction.Customer_Id = tran.Customer_Id;

                                            transaction.Consignment_no = tran.Consignment_no.Trim();
                                            transaction.chargable_weight = tran.chargable_weight;
                                            transaction.Mode = tran.Mode;
                                            transaction.compaddress = tran.compaddress;
                                            transaction.Quanntity = tran.Quanntity;
                                            transaction.Pincode = tran.Pincode;
                                            transaction.booking_date = tran.booking_date;
                                            transaction.Type_t = tran.Type_t;
                                            transaction.tembookingdate = tran.tembookingdate;
                                            transaction.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                            transaction.AdminEmp = 000;



                                            db.Entry(transaction).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            CalculateAmount ca = new CalculateAmount();
                                            if (tran.Amount == null || tran.Amount == 0)
                                            {
                                                double? amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);

                                                tran.Amount = Math.Round((double)amt);
                                            }
                                            tran.Amount = Convert.ToDouble(tran.Amount);
                                            tran.Customer_Id = tran.Customer_Id;

                                            tran.Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                            tran.AdminEmp = 000;


                                            db.Transactions.Add(tran);
                                            db.SaveChanges();

                                        }

                                    }
                                

                            }
                            catch (Exception ex)
                            {
                                throw new RedirectException(ex.Message);
                            }
                        }
                    }

                }
            }
            return "1";
        }

    }
}