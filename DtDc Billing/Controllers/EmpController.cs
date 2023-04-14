using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace EmpBilling.Controllers
{
    [SessionUserModule]
    public class EmpController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Emp

        public ActionResult EmpLogin(string ReturnUrl)
        {

            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }



        [HttpPost]
        public ActionResult EmpLogin(DtDc_Billing.Models.Login login, string ReturnUrl)
        {

            var obj = db.Users.Where(a => a.Email.Equals(login.Username) && a.Password_U.Equals(login.Password) && a.Usertype.Contains("Billing")).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (obj != null)
                {
                    Session["PfID"] = obj.PF_Code.ToString();
                    Session["EmpId"] = obj.User_Id;
                    Session["UserName"] = obj.Name;

                    string decodedUrl = "";
                    if (!string.IsNullOrEmpty(ReturnUrl))
                        decodedUrl = Server.UrlDecode(ReturnUrl);

                    //Login logic...

                    if (Url.IsLocalUrl(decodedUrl))
                    {
                        return Redirect(decodedUrl);
                    }
                    else
                    {
                        return RedirectToAction("CompanyList", "Booking");
                    }


                }
                else
                {
                    ModelState.AddModelError("LoginAuth", "Username or password is incorrect");
                }
            }
            return View();
        }


        public ActionResult importTextFile()
        {

            return View();
        }

        //[HttpPost]
        //public async Task<ActionResult> importTextFile(HttpPostedFileBase ImportText)
        //{
        //    string filePath = string.Empty;

        //    if (ImportText != null)
        //    {
        //        string path = Server.MapPath("~/UploadsText/");

        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        filePath = path + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + Path.GetFileName(ImportText.FileName);
        //        string extension = Path.GetExtension(ImportText.FileName);
        //        ImportText.SaveAs(filePath);




        //        Task.Run(() => InsertRecords(filePath, ImportText.FileName));

        //    }



        //    TempData["Upload"] = "File Uploaded Successfully!";


        //    return RedirectToAction("ConsignMent", "Booking");

        //}


        #region new uploadtext
        [HttpPost]
        public ActionResult importTextFile(HttpPostedFileBase ImportText)
        {
            string filePath = string.Empty;

            if (ImportText != null)
            {
                //string path = Server.MapPath("~/UploadsText/");

                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}

                //filePath = path + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + Path.GetFileName(ImportText.FileName);
                //string extension = Path.GetExtension(ImportText.FileName);
                //ImportText.SaveAs(filePath);




                //   Task.Run(() => InsertRecords(filePath, ImportText.FileName));


               

                System.IO.StreamReader myReader = new System.IO.StreamReader(ImportText.InputStream);
                string csvData = myReader.ReadToEnd();
                //Read the contents of CSV file.
                //   string csvData = System.IO.File.ReadAllText(filePath);

                //Execute a loop over the rows.
                int i = 0;
                foreach (string row in csvData.Split('\n'))
                {
                    i++;
                    if (i <= 2)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(row))
                    {

                        string[] values = row.Split('"');


                        Transaction tr = new Transaction();

                        string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};
                        string bdate = DateTime.ParseExact(values[10].Trim('\''), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");



                        tr.Consignment_no = values[1].Trim('\'').Trim();
                        tr.Pf_Code = values[3].Trim('\'');
                        tr.Actual_weight = Convert.ToDouble(values[4].Trim('\''));
                        tr.Mode = values[5].Trim('\'');
                        tr.Quanntity = Convert.ToInt16(values[8].Trim('\''));
                        tr.Pincode = values[9].Trim('\'');
                        tr.booking_date = Convert.ToDateTime(bdate);
                        tr.tembookingdate = values[10].Trim('\'');
                        tr.dtdcamount = Convert.ToDouble(values[11].Trim('\''));
                        tr.chargable_weight = Convert.ToDouble(values[4].Trim('\''));
                        tr.diff_weight = Convert.ToDouble(values[4].Trim('\''));
                        tr.topay = "no";
                        tr.cod = "no";
                        tr.Insurance = "no";
                        tr.Type_t = values[16].Trim('\'');


                        Transaction insertupdate = db.Transactions.Where(m => m.Consignment_no == tr.Consignment_no).FirstOrDefault();





                        if (insertupdate == null)
                        {
                            // db.Entry(insertupdate).State = EntityState.Detached;

                            db.Transactions.Add(tr);
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

                        }
                        else
                        {
                            insertupdate.Pf_Code = values[3].Trim('\'');
                            insertupdate.dtdcamount = Convert.ToDouble(values[11].Trim('\''));
                            insertupdate.diff_weight = Convert.ToDouble(values[4].Trim('\''));
                            insertupdate.Consignment_no = insertupdate.Consignment_no.Trim();

                            db.Entry(insertupdate).State = EntityState.Modified;

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

                            // db.SaveChanges();

                        }




                    }


                }




            }





            TempData["Upload"] = "File Uploaded Successfully!";

            return View();

           




            //TempData["Upload"] = "File Uploaded Successfully!";

            //return RedirectToAction("ConsignMent", "Booking");

        }
 #endregion

        public void InsertRecords(string filePath, string Filename)
        {
            List<Transaction> Tranjaction = new List<Transaction>();



            //Read the contents of CSV file.
            string csvData = System.IO.File.ReadAllText(filePath);

            //Execute a loop over the rows.
            int i = 0;
            foreach (string row in csvData.Split('\n'))
            {
                i++;
                if (i <= 2)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(row))
                {

                    string[] values = row.Split('"');


                    Transaction tr = new Transaction();

                    string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};
                    string bdate = DateTime.ParseExact(values[10].Trim('\''), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");



                    tr.Consignment_no = values[1].Trim('\''); ;
                    tr.Pf_Code = values[3].Trim('\'');
                    tr.Actual_weight = Convert.ToDouble(values[4].Trim('\''));
                    tr.Mode = values[5].Trim('\'');
                    tr.Quanntity = Convert.ToInt16(values[8].Trim('\''));
                    tr.Pincode = values[9].Trim('\'');
                    tr.booking_date = Convert.ToDateTime(bdate);
                    tr.tembookingdate = values[10].Trim('\'');
                    tr.dtdcamount = Convert.ToDouble(values[11].Trim('\''));
                    tr.chargable_weight = Convert.ToDouble(values[4].Trim('\''));
                    tr.diff_weight = Convert.ToDouble(values[4].Trim('\''));
                    tr.topay = "no";
                    tr.cod = "no";
                    tr.Insurance = "no";
                    tr.Type_t = values[16].Trim('\'');


                    Transaction insertupdate = db.Transactions.Where(m => m.Consignment_no == tr.Consignment_no).FirstOrDefault();





                    if (insertupdate == null)
                    {
                        // db.Entry(insertupdate).State = EntityState.Detached;

                        db.Transactions.Add(tr);
                        db.SaveChanges();

                    }
                    else
                    {

                        insertupdate.dtdcamount = Convert.ToDouble(values[11].Trim('\''));
                        insertupdate.diff_weight = Convert.ToDouble(values[4].Trim('\''));

                        db.Entry(insertupdate).State = EntityState.Modified;
                        db.SaveChanges();

                    }




                }


            }

            Notification nt = new Notification();

            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            nt.dateN = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            nt.Message = Filename + "File Uploaded Successfully From Branch";
            nt.Status = false;

            db.Notifications.Add(nt);
            db.SaveChanges();

        }



        public JsonResult CustomerDetails(string CustomerId)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var suggestions = (from s in db.Companies
                               where s.Company_Id == CustomerId
                               select s).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("EmpLogin", "Emp");

        }


        [HttpGet]
        public ActionResult importFromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult importFromExcel(HttpPostedFileBase httpPostedFileBase)
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



                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var tran = new Transaction();

                            tran.Consignment_no = (workSheet?.Cells[rowIterator, 2]?.Value?.ToString().Trim());
                            tran.Customer_Id = (workSheet?.Cells[rowIterator, 3]?.Value?.ToString());

                            if (tran.Consignment_no != null || tran.Customer_Id != null)
                            {

                                Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no).FirstOrDefault();

                                var validcomp = db.Companies.Where(m => m.Company_Id == tran.Customer_Id).FirstOrDefault();

                                if (transaction != null)
                                {

                                    CalculateAmount ca = new CalculateAmount();
                                    double? amt = 0;
                                    if (transaction.Pincode != null && transaction.Pincode != "NULL " && validcomp != null)
                                    {
                                        amt = ca.CalulateAmt(transaction.Consignment_no, tran.Customer_Id, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                                        transaction.Amount = amt;


                                        transaction.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                        transaction.AdminEmp = Convert.ToInt32(Session["EmpId"]);
                                    }

                                    transaction.Customer_Id = tran.Customer_Id;
                                    db.Entry(transaction).State = EntityState.Modified;
                                    db.SaveChanges();

                                }
                            }

                        }
                    }

                    ViewBag.Success = "Excel File Uploaded SuccessFully";
                }
            }


            return View();
        }



        [HttpGet]
        public ActionResult importFromExcelWhole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult importFromExcelWhole(HttpPostedFileBase httpPostedFileBase)
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


                          //  tran.Consignment_no = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();
                            tran.chargable_weight = Convert.ToDouble(workSheet.Cells[rowIterator, 3].Value);
                          //  tran.Customer_Id = workSheet.Cells[rowIterator, 4].Value.ToString();
                            //tran.Insurance = workSheet.Cells[rowIterator, 5].Value.ToString();
                            tran.loadingcharge = Convert.ToDouble(workSheet.Cells[rowIterator, 8].Value);

                            insuranceamt = Convert.ToDouble(workSheet.Cells[rowIterator, 5].Value);
                            FOVamt = Convert.ToDouble(workSheet.Cells[rowIterator, 6].Value);
                            fovper = Convert.ToDouble(workSheet.Cells[rowIterator, 7].Value);

                            tran.Consignment_no = (workSheet?.Cells[rowIterator, 2]?.Value?.ToString().Trim());
                            tran.Customer_Id = (workSheet?.Cells[rowIterator, 4]?.Value?.ToString());

                            if (tran.Consignment_no != null || tran.Customer_Id != null)
                            {
                                Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no).FirstOrDefault();

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
                                        if (company.Insurance != null && company.Insurance > 0)
                                            transaction.Risksurcharge = Math.Round((transaction.BillAmount ?? 0) * ((company.Insurance / 100) ?? 0), 2);
                                        if (company.Minimum_Risk_Charge > transaction.Risksurcharge)
                                            transaction.Risksurcharge = company.Minimum_Risk_Charge;
                                    }
                                    else if (FOVamt > 0 && transaction.Type_t == "N" && validcomp != null)
                                    {
                                        transaction.Insurance = "no";
                                        transaction.BillAmount = FOVamt;
                                        transaction.Percentage = fovper.ToString();
                                        transaction.Risksurcharge = Math.Round((transaction.BillAmount ?? 0) * (fovper/100), 2);
                                        if (company.Minimum_Risk_Charge > transaction.Risksurcharge)
                                            transaction.Risksurcharge = company.Minimum_Risk_Charge;
                                    }


                                    transaction.AdminEmp = Convert.ToInt32(Session["EmpId"]);

                                    db.Entry(transaction).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                        }
                    }

                    TempData["Success"] = "Excel File With Other PF Uploaded SuccessFully";
                }
            }


            return RedirectToAction("importFromExcel");
        }
    }
    }