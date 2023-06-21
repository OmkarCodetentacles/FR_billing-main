using DtDc_Billing.Controllers;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
                            tran.Customer_Id = (workSheet?.Cells[rowIterator, 3]?.Value?.ToString());

                            if (tran.Consignment_no != null || tran.Customer_Id != null)
                            {


                                Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no).FirstOrDefault();

                                var validcomp = db.Companies.Where(m => m.Company_Id == tran.Customer_Id && m.Pf_code == getPfcode).FirstOrDefault();

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
            return "1";
        }


        public string Import3Async(HttpPostedFileBase httpPostedFileBase, string PfCode)
        {
            var damageResult = Task.Run(() => asyncAddNewimporFromExcel(httpPostedFileBase, PfCode));

            return damageResult.ToString();
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

                    //string[] formats = { "dd-MM-yyyy" };
                    string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy", "MM/dd/yyyy" };

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



                            tran.Consignment_no = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();

                            tran.chargable_weight = Convert.ToDouble(workSheet.Cells[rowIterator, 3].Value);
                            tran.Mode = workSheet.Cells[rowIterator, 4].Value.ToString();
                            tran.compaddress = (workSheet?.Cells[rowIterator, 5]?.Value?.ToString());
                            tran.Quanntity = Convert.ToInt16(workSheet.Cells[rowIterator, 6].Value);
                            tran.Pincode = workSheet.Cells[rowIterator, 7].Value.ToString();
                            DateTime abc = Convert.ToDateTime(workSheet.Cells[rowIterator, 8].Value.ToString());

                            
                            tran.booking_date = Convert.ToDateTime(abc);
                            tran.tembookingdate = abc.ToString();

                            tran.Type_t = workSheet.Cells[rowIterator, 9].Value.ToString();
                            tran.Customer_Id = workSheet.Cells[rowIterator, 10].Value.ToString();


                            Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no && m.Pf_Code == getPfcode).FirstOrDefault();

                            if (transaction != null)
                            {

                                CalculateAmount ca = new CalculateAmount();

                                double? amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);

                                transaction.Amount = amt;
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

                                double? amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);

                                tran.Amount = amt;
                                tran.Customer_Id = tran.Customer_Id;

                                tran.Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                tran.AdminEmp = 000;



                                db.Transactions.Add(tran);
                                db.SaveChanges();
                            }

                        }
                    }

                }
            }
            return "1";
        }
    }
}