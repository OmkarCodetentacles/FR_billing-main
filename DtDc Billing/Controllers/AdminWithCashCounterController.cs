using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using DtDc_Billing.CustomModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using Microsoft.SqlServer.Management.Common;
using System.Configuration;
using System.Text;
using System.Net.Mail;

namespace DtDc_Billing.Controllers
{
    
    public class AdminWithCashCounterController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Admin
        [SessionUserModule]
        public ActionResult AdminLogin(string ReturnUrl)
        {


            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        public ActionResult IndexView()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult AdminLogin(AdminLogin login, string ReturnUrl, EmpLogin emplogin)
        {

          
            var obj = db.getLogin(login.UserName, login.Password,"").Select(x => new registration { registrationId = x.registrationId, userName = x.username , Pfcode=x.Pfcode }).FirstOrDefault();

            var objUser = db.Users.Where(a => a.Email.ToLower().Equals(emplogin.UserName.ToLower()) && a.Password_U.Equals(login.Password) && a.Usertype.Contains("CashCounter")).FirstOrDefault();
            
            var objBilling = db.Users.Where(a => a.Email.Equals(emplogin.UserName) && a.Password_U.Equals(login.Password) && a.Usertype.Contains("Billing")).FirstOrDefault();

            var types = db.Users.Where(m => m.Email == emplogin.UserName).Select(m => m.Usertype).FirstOrDefault();

           
            TempData["DataCheck"] = obj;
           

            if (types != null)
            {

                string[] split = types.Split(',');

                int Count = split.Count();

                Session["CountUserType"] = Count;
            }
           // var firmlist = db.FirmDetails.ToList();
            // var FirmDetail = new List<FirmDetail>();
            if (obj != null)
            {
                Session["Admin"] = obj.registrationId.ToString();
                Session["UserName"] = obj.userName.ToString();
                Session["PFCode"] = obj.Pfcode.ToString();
                //Session["firmlist"] = firmlist;
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
                    return RedirectToAction("Index", "Home");
                }


            }

            else if (objUser != null)
            {
                Session["EmpId"] = objUser.User_Id.ToString();
                Session["pfCode"] = objUser.PF_Code.ToString();
                Session["EmpName"] = objUser.Name.ToString();


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
                    return RedirectToAction("Printreceipt", "CashBooking");
                }



            }
            else if (objBilling != null)
            {
                Session["PfID"] = objBilling.PF_Code.ToString();
                Session["EmpId"] = objBilling.User_Id;
                Session["UserName"] = objBilling.Name;

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

            if (Convert.ToInt32(Session["CountUserType"]) > 1)
            {
                if (Session["UserType"].ToString() == "CashCounter")
                {
                    Session["EmpId"] = objUser.User_Id.ToString();
                    Session["pfCode"] = objUser.PF_Code.ToString();
                    Session["EmpName"] = objUser.Name.ToString();


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
                        return RedirectToAction("Printreceipt", "CashBooking");
                    }
                }

                if (Session["UserType"].ToString() == "Billing")
                {
                    Session["PfID"] = objBilling.PF_Code.ToString();
                    Session["EmpId"] = objBilling.User_Id;
                    Session["UserName"] = objBilling.Name;

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
            }
            
            else
            {
                //ModelState.AddModelError("LoginAuth", "Username or Password Is Incorrect");
                ModelState.AddModelError("LoginAuth", "Username or Password Is Incorrect or Please Do The Registration First");

            }

            //return View(emplogin);
            return View();
        }

        public ActionResult AdminChangePass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminChangePass(string currentpass, string newpass, string Token)
        {
            var obj = db.Admins.Select(m => m.Username).FirstOrDefault();
            if (obj != null)
            {



            }

            return View();
        }

        public ActionResult NewpasswordSave()
        {

            using (MailMessage mm = new MailMessage("codetentacles@gmail.com", "nileshveer17@gmail.com"))
            {

                mm.Subject = "Token Verification for Change Password";
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[6];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var Tokne = new String(stringChars);

                string Bodytext = "<html><body>Your Verification Token is -" + Tokne + " </body></html>";

                mm.IsBodyHtml = true;



                mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                // mm.Body = Bodytext;
                mm.Body = Bodytext;

                //Add Byte array as Attachment.

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                credentials.UserName = "codetentacles@gmail.com";
                credentials.Password = "Codeadmin";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = credentials;
                smtp.Port = 587;
                smtp.Send(mm);

                Admin admin = db.Admins.FirstOrDefault();
                if (admin != null)
                {
                    admin.Token = Tokne;
                    db.Admins.Attach(admin);
                    db.Entry(admin).Property(x => x.Token).IsModified = true;
                    db.SaveChanges();
                }


                ViewBag.sendmail = "Mail send to Your Emailid";
            }
            return Json("chamara", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult NewpasswordSave(string currentpass, string newpass, string Token)
        {
            var obj = db.Admins.Where(m => m.Token == Token).FirstOrDefault();
            Admin admin = db.Admins.FirstOrDefault();
            if (obj != null)
            {
                admin.A_Password = newpass;
                db.Admins.Attach(admin);
                db.Entry(admin).Property(x => x.A_Password).IsModified = true;
                db.SaveChanges();
                ViewBag.changepass = "Password Change Successfullly";
            }
            return RedirectToAction("AdminChangePass", "Admin");
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

                            //var consno= workSheet.Cells[rowIterator, 2].Value.ToString().Trim() ?? null;
                            //var custid= (workSheet?.Cells[rowIterator, 3]?.Value?.ToString());

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
                                        transaction.AdminEmp = 000;
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

                    TempData["Success"] = "Excel File With Other PF Uploaded SuccessFully";
                }
            }


            return RedirectToAction("importFromExcel");
        }


        [HttpGet]
        public ActionResult AddNewimporFromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewimporFromExcel(HttpPostedFileBase httpPostedFileBase)
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

                            //string bdate = DateTime.ParseExact(abc.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                            tran.booking_date = Convert.ToDateTime(abc);

                            //string tempdate = DateTime.ParseExact(abc.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MM-yyyy");
                            tran.tembookingdate = abc.ToString();
                            tran.Type_t = workSheet.Cells[rowIterator, 9].Value.ToString();
                            tran.Customer_Id = workSheet.Cells[rowIterator, 10].Value.ToString();


                            Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no).FirstOrDefault();

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

                    TempData["Successnew"] = "Excel File With New Added SuccessFully";
                }
            }


            return RedirectToAction("importFromExcel");
        }


        public ActionResult importTextFile()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> importTextFile(HttpPostedFileBase ImportText)
        {
            string filePath = string.Empty;

            if (ImportText != null)
            {
                string path = Server.MapPath("~/UploadsText/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + Path.GetFileName(ImportText.FileName);
                string extension = Path.GetExtension(ImportText.FileName);
                ImportText.SaveAs(filePath);




                Task.Run(() => InsertRecords(filePath, ImportText.FileName));

            }





            TempData["Upload"] = "File Uploaded Successfully!";

            return RedirectToAction("ConsignMent", "Booking");
        }


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

            //Notification nt = new Notification();

            //TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            //nt.dateN = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            //nt.Message = Filename + "File Uploaded Successfully From Branch";
            //nt.Status = false;

            //db.Notifications.Add(nt);
            //db.SaveChanges();

        }



        [HttpPost]
        public async Task<ActionResult> InternationalimportTextFile(HttpPostedFileBase ImportText)
        {

            string filePath = string.Empty;

            if (ImportText != null)
            {
                string path = Server.MapPath("~/UploadsText/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + Path.GetFileName(ImportText.FileName);
                string extension = Path.GetExtension(ImportText.FileName);
                ImportText.SaveAs(filePath);




                Task.Run(() => InsertRecords(filePath, ImportText.FileName));

            }




            TempData["Upload"] = "File Uploaded Successfully!";

            return RedirectToAction("ConsignMent", "Booking");

        }



        public ActionResult DeliveryFile()
        {

            return View();
        }

        [HttpPost]
        public ActionResult DeliveryFile(HttpPostedFileBase ImportText)
        {


            List<deliverydata> Tranjaction = new List<deliverydata>();
            string filePath = string.Empty;

            if (ImportText != null)
            {
                string path = Server.MapPath("~/Uploadsdelivery/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(ImportText.FileName);
                string extension = Path.GetExtension(ImportText.FileName);
                ImportText.SaveAs(filePath);

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


                        deliverydata tr = new deliverydata();

                        string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};
                        // string bdate = DateTime.ParseExact(values[10].Trim('\''), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");



                        tr.consinmentno = values[0].Trim('\'').Trim();
                        tr.tempdelivereddate = values[1].Trim('\'');
                        tr.tempdeliveredtime = values[2].Trim('\'');
                        tr.receivedby = values[3].Trim('\'');
                        tr.remarks = values[4].Trim('\'');


                        deliverydata dr = db.deliverydatas.Where(m => m.consinmentno == tr.consinmentno).FirstOrDefault();

                        if (dr == null)
                        {
                            db.deliverydatas.Add(tr);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(dr).State = EntityState.Detached;

                            tr.d_id = dr.d_id;
                            db.Entry(tr).State = EntityState.Modified;
                            db.SaveChanges();
                        }






                    }


                }
            }

            ViewBag.Message = "File Uploaded SuccessFully";

            return View();
        }

        [SessionAdmin]
        public ActionResult CreateUser()
        {

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "CashCounter", Value = "CashCounter" });

            items.Add(new SelectListItem { Text = "Billing", Value = "Billing" });

            ViewBag.Usertype = items;


            var categories = db.Franchisees.Select(c => c.PF_Code).ToList();
            ViewBag.Categories = new MultiSelectList(categories, "PF_Code");


            ViewBag.PF_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "PF_Code");

          
            List<SelectListItem> items1 = new List<SelectListItem>();
           
            items1.Add(new SelectListItem { Text = "Stationary", Value = "Stationary".ToString() });

            items1.Add(new SelectListItem { Text = "RateMaster", Value = "RateMaster".ToString() });

            items1.Add(new SelectListItem { Text = "Booking", Value = "Booking".ToString() });

            items1.Add(new SelectListItem { Text = "Invoice", Value = "Invoice".ToString() });
            
            items1.Add(new SelectListItem { Text = "Payment", Value = "Payment".ToString() });

            items1.Add(new SelectListItem { Text = "Track", Value = "Track".ToString() });

            items1.Add(new SelectListItem { Text = "Daily Expenses", Value = "DailyExpenses".ToString() });

            items1.Add(new SelectListItem { Text = "Reports", Value = "Reports".ToString() });

            items1.Add(new SelectListItem { Text = "Send Message", Value = "SendMessage".ToString() });

            ViewBag.ModuletypeCash = items1;



            List<SelectListItem> itemsBilling = new List<SelectListItem>();

            itemsBilling.Add(new SelectListItem { Text = "Stationary", Value = "Stationary".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "RateMaster", Value = "RateMaster".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Booking", Value = "Booking".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Invoice", Value = "Invoice".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Payment", Value = "Payment".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Track", Value = "Track".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Daily Expenses", Value = "DailyExpenses".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Reports", Value = "Reports".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Send Message", Value = "SendMessage".ToString() });


            ViewBag.ModuletypeBilling = itemsBilling;

            return View();

        }
        [SessionAdmin]
        [HttpPost]
        public ActionResult CreateUser(User user, string[] Usertype, string[] ModuletypeCash,string[] ModuletypeBilling)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "CashCounter", Value = "CashCounter" });

            items.Add(new SelectListItem { Text = "Billing", Value = "Billing" });

          
            
            
            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "Stationary", Value = "Stationary".ToString() });

            items1.Add(new SelectListItem { Text = "RateMaster", Value = "RateMaster".ToString() });

            items1.Add(new SelectListItem { Text = "Booking", Value = "Booking".ToString() });

            items1.Add(new SelectListItem { Text = "Invoice", Value = "Invoice".ToString() });

            items1.Add(new SelectListItem { Text = "Payment", Value = "Payment".ToString() });

            items1.Add(new SelectListItem { Text = "Track", Value = "Track".ToString() });

            items1.Add(new SelectListItem { Text = "Daily Expenses", Value = "DailyExpenses".ToString() });

            items1.Add(new SelectListItem { Text = "Reports", Value = "Reports".ToString() });

            items1.Add(new SelectListItem { Text = "Send Message", Value = "SendMessage".ToString() });




            List<SelectListItem> itemsBilling = new List<SelectListItem>();

            itemsBilling.Add(new SelectListItem { Text = "Stationary", Value = "Stationary".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "RateMaster", Value = "RateMaster".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Booking", Value = "Booking".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Invoice", Value = "Invoice".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Payment", Value = "Payment".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Track", Value = "Track".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Daily Expenses", Value = "DailyExpenses".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Reports", Value = "Reports".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Send Message", Value = "SendMessage".ToString() });



            if (ModelState.IsValid)
            {


                var result = string.Join(",", Usertype);
                user.Usertype = result;

                db.Users.Add(user);
                db.SaveChanges();

                //CheckBoxModel CheckBox = new CheckBoxModel();


                //CheckBox.Name = "TEST";
                // CheckBox.Id = 2;
                // CheckBox.IsSelected = false;

                // ViewBag.Moduletype = CheckBox;
                string str = "",strbilling="";

                string[] split = user.Usertype.Split(',');

                if (ModuletypeCash.Count() !=null)
                {
                    for (int i = 0; i < ModuletypeCash.Count();)
                    {
                        str = ModuletypeCash[i];
    
                            var data = (from d in db.UserModuleLists
                                        where d.ModuleName == str
                                        && d.UserName == user.Name
                                        && d.Usertype == "CashCounter"
                                        select d).ToList();

                            if (data.Count() == 0)
                            {
                                UserModuleList userm = new UserModuleList();

                                userm.ModuleName = ModuletypeCash[i];
                                userm.PF_Code = user.PF_Code;
                                userm.User_Id = user.User_Id;
                                userm.UserName = user.Name;
                                userm.Usertype = "CashCounter";
                                db.UserModuleLists.Add(userm);
                                db.SaveChanges();
                            }
                            

                        i++;
                    }
                }

                if (ModuletypeBilling.Count() !=null)
                {
                    for (int i = 0; i < ModuletypeBilling.Count();)
                    {
                        strbilling = ModuletypeBilling[i];

                        
                            var data = (from d in db.UserModuleLists
                                        where d.ModuleName == strbilling
                                        && d.UserName == user.Name
                                        && d.Usertype == "Billing"
                                        select d).ToList();

                            if (data.Count() == 0)
                            {
                                UserModuleList userm = new UserModuleList();

                                userm.ModuleName = ModuletypeBilling[i];
                                userm.PF_Code = user.PF_Code;
                                userm.User_Id = user.User_Id;
                                userm.UserName = user.Name;
                                userm.Usertype = "Billing";
                                db.UserModuleLists.Add(userm);
                                db.SaveChanges();
                            }
                           

                        i++;
                    }
                }

                //////////Alert Afte Success///
                ViewBag.Success = " Added Successfully...!!!";
                ////////////////////////////////////////
                ViewBag.PF_Code = user.PF_Code;//new SelectList(db.Franchisees, "PF_Code", "PF_Code", user.PF_Code);
                ViewBag.Usertype = items;
                ViewBag.ModuletypeCash = items1;
                ViewBag.ModuletypeBilling = itemsBilling;
                ModelState.Clear();

                return View(new User());
            }

            ViewBag.PF_Code = user.PF_Code;//new SelectList(db.Franchisees, "PF_Code", "PF_Code", user.PF_Code);
            ViewBag.Usertype = items;


          
             

                return View(user);

        }

        [SessionAdmin]
        public ActionResult EditUser(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "CashCounter", Value = "CashCounter" });

            items.Add(new SelectListItem { Text = "Billing", Value = "Billing" });



            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "Stationary", Value = "Stationary".ToString() });

            items1.Add(new SelectListItem { Text = "RateMaster", Value = "RateMaster".ToString() });

            items1.Add(new SelectListItem { Text = "Booking", Value = "Booking".ToString() });

            items1.Add(new SelectListItem { Text = "Invoice", Value = "Invoice".ToString() });

            items1.Add(new SelectListItem { Text = "Payment", Value = "Payment".ToString() });

            items1.Add(new SelectListItem { Text = "Track", Value = "Track".ToString() });

            items1.Add(new SelectListItem { Text = "Daily Expenses", Value = "DailyExpenses".ToString() });

            items1.Add(new SelectListItem { Text = "Reports", Value = "Reports".ToString() });

            items1.Add(new SelectListItem { Text = "Send Message", Value = "SendMessage".ToString() });



            List<SelectListItem> itemsBilling = new List<SelectListItem>();

            itemsBilling.Add(new SelectListItem { Text = "Stationary", Value = "Stationary".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "RateMaster", Value = "RateMaster".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Booking", Value = "Booking".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Invoice", Value = "Invoice".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Payment", Value = "Payment".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Track", Value = "Track".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Daily Expenses", Value = "DailyExpenses".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Reports", Value = "Reports".ToString() });

            itemsBilling.Add(new SelectListItem { Text = "Send Message", Value = "SendMessage".ToString() });


            var types = db.Users.Where(m => m.User_Id == id).Select(m => m.Usertype).FirstOrDefault();
            string[] split = types.Split(',');

            foreach (var item in items)
            {
                if (split.Contains(item.Value))
                {
                    item.Selected = true;

                }
            }

            var modulelist = db.UserModuleLists.Where(m => m.User_Id == id && m.Usertype=="CashCounter").Select(m => m.ModuleName).ToList();
            
            foreach (var item in items1)
            {
                foreach(var list in modulelist)
                {
                    if (list == item.Value)
                    {
                        item.Selected = true;

                    }
                }
                
            }

            var modulelistBilling = db.UserModuleLists.Where(m => m.User_Id == id && m.Usertype == "Billing").Select(m => m.ModuleName).ToList();

            foreach (var item in itemsBilling)
            {
                foreach (var list in modulelistBilling)
                {
                    if (list == item.Value)
                    {
                        item.Selected = true;

                    }
                }

            }
            ViewBag.Usertype = items;

            ViewBag.ModuletypeCash = items1;
            ViewBag.ModuletypeBilling = itemsBilling;


            ViewBag.PF_Code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", user.PF_Code);
            return View(user);
        }

        // POST: demo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "User_Id,Name,Email,Contact_no,PF_Code,Password_U,Usertype,Datetime_User")] User user, string[] Usertype, string[] ModuletypeCash, string[] ModuletypeBilling)
        {
            if (ModelState.IsValid)
            {
                var result = string.Join(",", Usertype);
                user.Usertype = result;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                string[] split = user.Usertype.Split(',');

             
                var data1 = (from d in db.UserModuleLists
                             where  d.UserName == user.Name
                             && d.Usertype == "Billing"
                               && d.User_Id == user.User_Id
                             select d).ToList();

                IEnumerable<UserModuleList> UserModuleLists = db.UserModuleLists.Where(x => x.UserName == user.Name && x.Usertype == "CashCounter" && x.User_Id == user.User_Id).ToList();
                db.UserModuleLists.RemoveRange(UserModuleLists);
                db.SaveChanges();

                IEnumerable<UserModuleList> UserModuleListsBill = db.UserModuleLists.Where(x => x.UserName == user.Name && x.Usertype == "Billing" && x.User_Id == user.User_Id).ToList();
                db.UserModuleLists.RemoveRange(UserModuleListsBill);
                db.SaveChanges();

                if (ModuletypeCash != null)
                {

                    for (int i = 0; i < ModuletypeCash.Count();)
                    {
                        UserModuleList userm = new UserModuleList();

                        userm.ModuleName = ModuletypeCash[i];
                        userm.PF_Code = user.PF_Code;
                        userm.User_Id = user.User_Id;
                        userm.UserName = user.Name;
                        userm.Usertype = "CashCounter";
                        db.UserModuleLists.Add(userm);
                        db.SaveChanges();
                        i++;
                    }
                }

                if (ModuletypeBilling != null)
                {
                    for (int i = 0; i < ModuletypeBilling.Count();)
                    {
                        UserModuleList userm = new UserModuleList();

                        userm.ModuleName = ModuletypeBilling[i];
                        userm.PF_Code = user.PF_Code;
                        userm.User_Id = user.User_Id;
                        userm.UserName = user.Name;
                        userm.Usertype = "Billing";
                        db.UserModuleLists.Add(userm);
                        db.SaveChanges();
                        i++;
                    }
                }



                return RedirectToAction("UserList");
            }

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "CashCounter", Value = "CashCounter" });

            items.Add(new SelectListItem { Text = "Billing", Value = "Billing" });

           

            ViewBag.PF_Code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", user.PF_Code);
            return View(user);
        }


        [SessionAdmin]
        public ActionResult AddFranchisee()
        {
            ViewBag.Firm_Id = new SelectList(db.FirmDetails, "Firm_Id", "Firm_Name");

            return View();
        }


        [SessionAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFranchisee(Franchisee franchisee)
        {

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }


            if (ModelState.IsValid)
            {
                db.Franchisees.Add(franchisee);

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
                //  db.SaveChanges();



                //Adding Eantries To the Sector Table
                var sectornamelist = db.sectorNames.ToList();

                var pfcode = (from u in db.Franchisees
                              where u.PF_Code == franchisee.PF_Code
                              select u).FirstOrDefault();
                if (pfcode != null)
                {
                    foreach (var i in sectornamelist)
                    {
                        Sector sn = new Sector();

                        sn.Pf_code = pfcode.PF_Code;
                        sn.Sector_Name = i.sname;



                        sn.CashD = true;
                        sn.CashN = true;
                        sn.BillD = true;
                        sn.BillN = true;


                        if (sn.Sector_Name == "Local")
                        {
                            sn.Priority = 1;
                            sn.Pincode_values = "400001-400610,400615-400706,400710-401203,401205-402209";

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = true;
                            sn.BillN = true;

                        }
                        else if (sn.Sector_Name == "Maharashtra")
                        {

                            sn.CashD = true;
                            sn.CashN = false;
                            sn.BillD = true;
                            sn.BillN = false;

                            sn.Priority = 2;
                            sn.Pincode_values = "400000-403000,404000-450000";
                        }


                        else if (sn.Sector_Name == "Western Zone")
                        {
                            sn.Priority = 3;
                            sn.Pincode_values = "400000-450000,360000-400000,450000-490000";

                            sn.CashD = false;
                            sn.CashN = true;
                            sn.BillD = false;
                            sn.BillN = true;

                        }

                        else if (sn.Sector_Name == "Metro")
                        {
                            sn.Priority = 4;
                            sn.Pincode_values = "180000-200000";

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = true;
                            sn.BillN = true;

                        }



                        else if (sn.Sector_Name == "North East Sector")
                        {
                            sn.Priority = 5;
                            sn.Pincode_values = "780000-800000,170000-180000";

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = true;
                            sn.BillN = true;

                        }



                        else if (sn.Sector_Name == "Rest of India")
                        {
                            sn.Priority = 6;
                            sn.Pincode_values = "000000";

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = true;
                            sn.BillN = true;

                        }
                        else
                        {
                            sn.Pincode_values = null;
                        }




                        db.Sectors.Add(sn);

                        db.SaveChanges();

                    }
                }
                //////////////////////////////////////////////

                ///Adding Eantries To New Company For Cash Counter ///               




                var Companyid = "Cash_" + franchisee.PF_Code;


                var secotrs = db.Sectors.Where(m => m.Pf_code == franchisee.PF_Code).ToList();

                Company cm = new Company();
                cm.Company_Id = Companyid;
                cm.Pf_code = franchisee.PF_Code;
                cm.Phone = 1234567890;
                cm.Company_Address = franchisee.F_Address;
                cm.Company_Name = Companyid;
                cm.Email = Companyid + "@gmail.com";
                db.Companies.Add(cm);



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

                var basiccompid = "BASIC_TS";

                var basicrec = db.Ratems.Where(m => m.Company_id == "BASIC_TS").FirstOrDefault();



                if (basicrec == null)
                {
                    Company bs = new Company();
                    bs.Company_Id = basiccompid;
                    bs.Pf_code = null;
                    bs.Phone = 1234567890;
                    bs.Company_Address = franchisee.F_Address;
                    bs.Company_Name = "BASIC_TS";
                    bs.Email = "Email@gmail.com";
                    db.Companies.Add(bs);
                    db.SaveChanges();

                    int j = 0;

                    foreach (var i in secotrs)
                    {
                        Ratem dox = new Ratem();
                        Nondox ndox = new Nondox();
                        express_cargo cs = new express_cargo();

                        dox.Company_id = basiccompid;
                        dox.Sector_Id = i.Sector_Id;
                        dox.NoOfSlab = 2;

                        dox.slab1 = 1;
                        dox.slab2 = 1;
                        dox.slab3 = 1;
                        dox.slab4 = 1;

                        dox.Uptosl1 = 1;
                        dox.Uptosl2 = 1;
                        dox.Uptosl3 = 1;
                        dox.Uptosl4 = 1;

                        ndox.Company_id = basiccompid;
                        ndox.Sector_Id = i.Sector_Id;
                        ndox.NoOfSlabN = 2;
                        ndox.NoOfSlabS = 2;

                        ndox.Aslab1 = 1;
                        ndox.Aslab2 = 1;
                        ndox.Aslab3 = 1;
                        ndox.Aslab4 = 1;


                        ndox.Sslab1 = 1;
                        ndox.Sslab2 = 1;
                        ndox.Sslab3 = 1;
                        ndox.Sslab4 = 1;

                        ndox.AUptosl1 = 1;
                        ndox.AUptosl2 = 1;
                        ndox.AUptosl3 = 1;
                        ndox.AUptosl4 = 1;

                        ndox.SUptosl1 = 1;
                        ndox.SUptosl2 = 1;
                        ndox.SUptosl3 = 1;
                        ndox.SUptosl4 = 1;


                        cs.Company_id = basiccompid;
                        cs.Sector_Id = i.Sector_Id;

                        cs.Exslab1 = 1;
                        cs.Exslab2 = 1;

                        db.Ratems.Add(dox);
                        db.Nondoxes.Add(ndox);
                        db.express_cargo.Add(cs);

                        j++;

                    }

                    int p = 0;

                    for (int i = 0; i < 5; i++)
                    {

                        dtdcPlu dtplu = new dtdcPlu();
                        Dtdc_Ptp stptp = new Dtdc_Ptp();

                        if (i == 0)
                        {
                            dtplu.destination = "City Plus";
                            stptp.dest = "City";
                        }
                        else if (i == 1)
                        {
                            dtplu.destination = "Zonal Plus/Blue";
                            stptp.dest = "Zonal";

                        }
                        else if (i == 2)
                        {
                            dtplu.destination = "Metro Plus/Blue";
                            stptp.dest = "Metro";
                        }
                        else if (i == 3)
                        {
                            dtplu.destination = "National Plus/Blue";
                            stptp.dest = "National";
                        }
                        else if (i == 4)
                        {
                            dtplu.destination = "Regional Plus";
                            stptp.dest = "Regional";
                        }

                        dtplu.Company_id = basiccompid;

                        dtplu.Upto500gm = 1;
                        dtplu.U10to25kg = 1;
                        dtplu.U25to50 = 1;
                        dtplu.U50to100 = 1;
                        dtplu.add100kg = 1;
                        dtplu.Add500gm = 1;


                        stptp.Company_id = basiccompid;
                        stptp.PUpto500gm = 1;
                        stptp.PAdd500gm = 1;
                        stptp.PU10to25kg = 1;
                        stptp.PU25to50 = 1;
                        stptp.Padd100kg = 1;
                        stptp.PU50to100 = 1;

                        stptp.P2Upto500gm = 1;
                        stptp.P2Add500gm = 1;
                        stptp.P2U10to25kg = 1;
                        stptp.P2U25to50 = 1;
                        stptp.P2add100kg = 1;
                        stptp.P2U50to100 = 1;

                        db.dtdcPlus.Add(dtplu);
                        db.Dtdc_Ptp.Add(stptp);

                        p++;

                    }

                }




                foreach (var i in secotrs)
                {
                    Ratem dox = new Ratem();
                    Nondox ndox = new Nondox();
                    express_cargo cs = new express_cargo();

                    dox.Company_id = Companyid;
                    dox.Sector_Id = i.Sector_Id;
                    dox.NoOfSlab = 2;
                    //dox.CashCounter = true;

                    ndox.Company_id = Companyid;
                    ndox.Sector_Id = i.Sector_Id;
                    ndox.NoOfSlabN = 2;
                    ndox.NoOfSlabS = 2;
                    // ndox.CashCounterNon = true;


                    cs.Company_id = Companyid;
                    cs.Sector_Id = i.Sector_Id;

                    // cs.CashCounterExpr = true;

                    db.Ratems.Add(dox);
                    db.Nondoxes.Add(ndox);
                    db.express_cargo.Add(cs);


                }

                for (int i = 0; i < 5; i++)
                {
                    dtdcPlu dtplu = new dtdcPlu();
                    Dtdc_Ptp stptp = new Dtdc_Ptp();

                    if (i == 0)
                    {
                        dtplu.destination = "City Plus";
                        stptp.dest = "City";
                    }
                    else if (i == 1)
                    {
                        dtplu.destination = "Zonal Plus/Blue";
                        stptp.dest = "Zonal";

                    }
                    else if (i == 2)
                    {
                        dtplu.destination = "Metro Plus/Blue";
                        stptp.dest = "Metro";
                    }
                    else if (i == 3)
                    {
                        dtplu.destination = "National Plus/Blue";
                        stptp.dest = "National";
                    }
                    else if (i == 4)
                    {
                        dtplu.destination = "Regional Plus";
                        stptp.dest = "Regional";
                    }

                    dtplu.Company_id = Companyid;
                    // dtplu.CashCounterPlus = true;
                    stptp.Company_id = Companyid;


                    db.dtdcPlus.Add(dtplu);
                    db.Dtdc_Ptp.Add(stptp);

                }

                db.SaveChanges();

                /////////////////////////////////////////////////////
                //////////Alert Afte Success///
                TempData["Success1"] = " Added Successfully...!!!";
                ////////////////////////////////////////
                ModelState.Clear();

                return RedirectToAction("Add_SectorPin", new { PfCode = franchisee.PF_Code });
            }


            ViewBag.Firm_Id = new SelectList(db.FirmDetails, "Firm_Id", "Firm_Name", franchisee.Firm_Id);

            return View(franchisee);

        }

        [SessionAdmin]
        public ActionResult Add_SectorPin(string PfCode)
        {
            string Pf = PfCode; /*Session["PfID"].ToString();*/



            List<Sector> st = (from u in db.Sectors
                               where u.Pf_code == Pf
                               select u).ToList();
            ViewBag.pfcode = PfCode;//stored in hidden format on the view

           
            return View(st);
        }



        [SessionAdmin]
        [HttpPost]
        public ActionResult Add_SectorPin(registration franchisee,FormCollection fc)
        {
            //Adding Eantries To the Sector Table
            var sectornamelist = db.sectorNames.ToList();

            var code = (from u in db.registrations
                          where u.Pfcode == franchisee.Pfcode
                          select u).FirstOrDefault();

            var datasector = (from d in db.Sectors
                             where d.Pf_code == franchisee.Pfcode
                             select d);

            if (datasector != null)
            {

                var sectoridarray = fc.GetValues("item.Sector_Id");
                var pincodearayy = fc.GetValues("item.Pincode_values");


                  for (int i = 0; i < sectoridarray.Count(); i++)
                  {

                     Sector str = db.Sectors.Find(Convert.ToInt16(sectoridarray[i]));

                     if (pincodearayy[i] == "")
                     {
                          pincodearayy[i] = null;
                     }


                       str.Pincode_values = pincodearayy[i];
                       db.Entry(str).State = EntityState.Modified;

                  }

                     int result = pincodearayy.Count(s => s == null);

                    if (result > 0)
                    {
                       ModelState.AddModelError("PinError", "All Fields Are Compulsary");

                       List<Sector> stt = (from u in db.Sectors
                                            where u.Pf_code == franchisee.Pfcode
                                            && u.Pincode_values == null
                                           select u).ToList();
                    ViewBag.DataSector = stt;
                    return View(stt);
                   }
                  else
                   {
                        db.SaveChanges();
                        TempData["Success"] = "Sectors Added Successfully!";
                   }


                List<Sector> secct = (from u in db.Sectors
                                   where u.Pf_code == franchisee.Pfcode

                                      select u).ToList();
                ViewBag.DataSector = secct;
                return View(secct);

            }
            else
            {
                if (code != null)
                {
                    foreach (var i in sectornamelist)
                    {
                        Sector sn = new Sector();

                        sn.Pf_code = code.Pfcode;
                        sn.Sector_Name = i.sname;



                        sn.CashD = true;
                        sn.CashN = true;
                        sn.BillD = true;
                        sn.BillN = true;


                        if (sn.Sector_Name == "Local")
                        {
                            sn.Priority = 1;
                            sn.Pincode_values = "400001-400610,400615-400706,400710-401203,401205-402209";

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = true;
                            sn.BillN = true;

                        }
                        else if (sn.Sector_Name == "Maharashtra")
                        {

                            sn.CashD = true;
                            sn.CashN = false;
                            sn.BillD = true;
                            sn.BillN = false;

                            sn.Priority = 2;
                            sn.Pincode_values = "400000-403000,404000-450000";
                        }


                        else if (sn.Sector_Name == "Western Zone")
                        {
                            sn.Priority = 3;
                            sn.Pincode_values = "400000-450000,360000-400000,450000-490000";

                            sn.CashD = false;
                            sn.CashN = true;
                            sn.BillD = false;
                            sn.BillN = true;

                        }

                        else if (sn.Sector_Name == "Metro")
                        {
                            sn.Priority = 4;
                            sn.Pincode_values = "180000-200000";

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = true;
                            sn.BillN = true;

                        }



                        else if (sn.Sector_Name == "North East Sector")
                        {
                            sn.Priority = 5;
                            sn.Pincode_values = "780000-800000,170000-180000";

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = true;
                            sn.BillN = true;

                        }



                        else if (sn.Sector_Name == "Rest of India")
                        {
                            sn.Priority = 6;
                            sn.Pincode_values = "000000";

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = true;
                            sn.BillN = true;

                        }
                        else
                        {
                            sn.Pincode_values = null;
                        }




                        db.Sectors.Add(sn);

                        db.SaveChanges();

                    }
                }
            }
            List<Sector> st = (from u in db.Sectors
                               where u.Pf_code == franchisee.Pfcode
                               select u).ToList();
            ViewBag.pfcode = franchisee.Pfcode;//stored in hidden format on the view
            ViewBag.DataSector = st;

            return View(st);
           //return View();

            //////////////////////////////////////////////

        }
        //public ActionResult Add_SectorPin(FormCollection fc, string pfcode)
        //{
        //    string Pf = pfcode;

        //    ViewBag.pfcode = pfcode;//stored in hidden format on the view if All fields not filled

        //    var sectoridarray = fc.GetValues("item.Sector_Id");
        //    var pincodearayy = fc.GetValues("item.Pincode_values");


        //    for (int i = 0; i < sectoridarray.Count(); i++)
        //    {

        //        Sector str = db.Sectors.Find(Convert.ToInt16(sectoridarray[i]));

        //        if (pincodearayy[i] == "")
        //        {
        //            pincodearayy[i] = null;
        //        }


        //        str.Pincode_values = pincodearayy[i];
        //        db.Entry(str).State = EntityState.Modified;

        //    }

        //    int result = pincodearayy.Count(s => s == null);

        //    if (result > 0)
        //    {
        //        ModelState.AddModelError("PinError", "All Fields Are Compulsary");

        //        List<Sector> stt = (from u in db.Sectors
        //                            where u.Pf_code == Pf
        //                            && u.Pincode_values == null
        //                            select u).ToList();
        //        return View(stt);
        //    }
        //    else
        //    {
        //        db.SaveChanges();
        //        TempData["Success"] = "Sectors Added Successfully!";
        //    }


        //    List<Sector> st = (from u in db.Sectors
        //                       where u.Pf_code == Pf

        //                       select u).ToList();

        //    return View(st);
        //}


        [SessionAdmin]
        public ActionResult Add_SectorPinEdit(string PfCode)
        {
            string Pf = PfCode; /*Session["PfID"].ToString();*/



            List<Sector> st = (from u in db.Sectors
                               where u.Pf_code == Pf
                               orderby u.Priority
                               select u).ToList();
            ViewBag.pfcode = PfCode;//stored in hidden format on the view
            ViewBag.DataSector = st;

            return View("Add_SectorPin", st);
        }


       

        public ActionResult Edit(int registrationID)
        {

            if (registrationID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Franchisee franchisee = db.Franchisees.Find(PfCode);
            registration reg = db.registrations.Find(registrationID);
            //Franchisee franchisee = db.Franchisees.Find(registrationID);
            // ViewBag.FirmDetails = new SelectList(db.FirmDetails, "Firm_Id", "Firm_Name", franchisee.FirmDetail.Firm_Id);
            if (reg == null)
            {
                return HttpNotFound();
            }

           // ViewBag.Firm_Id = new SelectList(db.FirmDetails, "Firm_Id", "Firm_Name", franchisee.FirmDetail.Firm_Id);
            return View(reg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(registration reg,Franchisee franchisee)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(reg).State = EntityState.Modified;
                franchisee.PF_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();
                db.Entry(franchisee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("FranchiseeList");
            }
            return View(reg);
        }


        public ActionResult ImportCsv()
        {
            return View();
        }

        public ActionResult FranchiseeList()
        {
            long stradmin = Convert.ToInt64(Session["Admin"]);
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var data = (from d in db.registrations
                       where d.Pfcode == strpf
                       && d.registrationId == stradmin
                       select d).ToList();
            return View(data.ToList());
            //return View(db.Franchisees.ToList());
        }


        public ActionResult UserList()
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var datauser = (from d in db.Users
                            where d.PF_Code == strpf
                            select d).ToList();

            return View(datauser.ToList());
        }

        public ActionResult Destinationlist()
        {
            return View(db.Destinations.ToList());
        }

        [SessionAdmin]
        public ActionResult Consignmentlist(string id)
        {

            return View(db.TransactionViews.Take(100).ToList());
        }


        [SessionAdmin]
        public ActionResult CompanyList(string id)
        {

            return View(db.Companies.Take(100));
        }




        #region Edit Consignments


        public ActionResult EditCons(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Receipt_details receipt_details = db.Receipt_details.Find(id);

            if (receipt_details == null)
            {
                return HttpNotFound();
            }

            ViewBag.Pf_Code = new SelectList(db.Franchisees, "PF_Code", "F_Address", receipt_details.Pf_Code);
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "Name", receipt_details.User_Id);

            return View(receipt_details);
        }

        // POST: Receipt_details/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCons([Bind(Include = "Receipt_Id,Consignment_No,Destination,sender_phone,Sender_Email,Sender,SenderCompany,SenderAddress,SenderCity,SenderState,SenderPincode,Reciepents_phone,Reciepents_Email,Reciepents,ReciepentCompany,ReciepentsAddress,ReciepentsCity,ReciepentsState,ReciepentsPincode,Shipmenttype,Shipment_Length,Shipment_Quantity,Shipment_Breadth,Shipment_Heigth,DivideBy,TotalNo,Actual_Weight,volumetric_Weight,DescriptionContent1,DescriptionContent2,DescriptionContent3,Amount1,Amount2,Amount3,Total_Amount,Insurance,Insuance_Percentage,Insuance_Amount,Charges_Amount,Charges_Service,Risk_Surcharge,Service_Tax,Charges_Total,Cash,Credit,Credit_Amount,secure_Pack,Passport,OfficeSunday,Shipment_Mode,Addition_charge,Addition_Lable,Discount,Pf_Code,User_Id,Datetime_Cons,Paid_Amount")] Receipt_details receipt_details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receipt_details).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Pf_Code = new SelectList(db.Franchisees, "PF_Code", "F_Address", receipt_details.Pf_Code);
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "Name", receipt_details.User_Id);
            return View(receipt_details);
        }

        #endregion



        public ActionResult DeleteCons()
        {

            return View();
        }
        [HttpPost]
        public ActionResult DeleteCons(string id)
        {
            var validcons = db.Receipt_details.Where(p => p.Consignment_No == id).FirstOrDefault();
            if (validcons != null)
            {
                Receipt_details receipt = db.Receipt_details.Where(m => m.Consignment_No == id).FirstOrDefault();
                db.Receipt_details.Remove(receipt);
                db.SaveChanges();
            }
            else
            {
                TempData["fail"] = "Invalid Consignment";
                return View();
            }
            TempData["Success"] = "Consignment Deleted SuccessFully";
            return RedirectToAction("Consignmentlist");
        }



        public ActionResult DeleteCompapy(string id)
        {

            List<Dtdc_Ptp> dtdc_Ptps = db.Dtdc_Ptp.Where(m => m.Company_id == id).ToList();
            List<dtdcPlu> dtdcPlu = db.dtdcPlus.Where(m => m.Company_id == id).ToList();
            List<express_cargo> express_cargo = db.express_cargo.Where(m => m.Company_id == id).ToList();
            List<Nondox> Nondox = db.Nondoxes.Where(m => m.Company_id == id).ToList();
            List<Ratem> Ratem = db.Ratems.Where(m => m.Company_id == id).ToList();
            List<Priority> pra = db.Priorities.Where(m => m.Company_id == id).ToList();
            Company tran = db.Companies.Where(m => m.Company_Id == id).FirstOrDefault();

            foreach (var i in dtdc_Ptps)
            {
                db.Dtdc_Ptp.Remove(i);
            }
            foreach (var i in dtdcPlu)
            {
                db.dtdcPlus.Remove(i);
            }
            foreach (var i in express_cargo)
            {
                db.express_cargo.Remove(i);
            }
            foreach (var i in Nondox)
            {
                db.Nondoxes.Remove(i);
            }
            foreach (var i in Ratem)
            {
                db.Ratems.Remove(i);
            }
            foreach (var i in pra)
            {
                db.Priorities.Remove(i);
            }
            db.Companies.Remove(tran);

            db.SaveChanges();
            TempData["Success"] = "Company Deleted SuccessFully";
            return RedirectToAction("EditCompanyRateMaster", "RateMaster");
        }


        public ActionResult WalletHistory(string phone)
        {
            List<wallet_History> wallet_History = db.wallet_History.Where(m => m.mobile_no == phone).ToList();
            return View(wallet_History);
        }


        public ActionResult DeleteConsignment()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DeleteConsignment(string Consignment_no)
        {
            var validcons = db.Transactions.Where(p => p.Consignment_no == Consignment_no).FirstOrDefault();
            if (validcons != null)
            {
                Transaction tran = db.Transactions.Where(m => m.Consignment_no == Consignment_no).FirstOrDefault();
                db.Transactions.Remove(tran);
                db.SaveChanges();
            }
            else
            {
                TempData["fail"] = "Invalid Consignment";
                return View();
            }
            TempData["Success"] = "Consignment Deleted SuccessFully";
            return View();
        }

        public ActionResult ExpensesList(string ToDatetime, string Fromdatetime)
        {
            ViewBag.Pf_Code = Session["pfCode"].ToString();//new SelectList(db.Franchisees, "Pf_Code", "Pf_Code");


            var Cat = new List<SelectListItem>
    {
                   new SelectListItem{ Text="Select All", Value = "" },
        new SelectListItem{ Text="Load Connecting exp 1st and 2nd", Value = "Load Connecting exp 1st and 2nd" },
        new SelectListItem{ Text="Load connecting exp - Night load", Value = "Load connecting exp - Night load" },
        new SelectListItem{ Text="Pick up expenses", Value = "Pick up expenses"},
        new SelectListItem{ Text="Patpedhi Deposit", Value = "Patpedhi Deposit"},
        new SelectListItem{ Text="Salary Advance", Value = "Salary Advance"},
        new SelectListItem{ Text="Office Expenses", Value = "Office Expenses"},
        new SelectListItem{ Text="Fuel Exp", Value = "Fuel Exp"},
        new SelectListItem{ Text="Tea and refreshments exp", Value = "Tea and refreshments exp"},
        new SelectListItem{ Text="Packing Expenses", Value = "Packing Expenses"},
        new SelectListItem{ Text="Others", Value = "Others"},
    };

            ViewData["Category"] = Cat;

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            var obj = new List<Expense>();
            if (ToDatetime != null && Fromdatetime != null)
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                DateTime fromdate = Convert.ToDateTime(bdatefrom);
                DateTime todate = Convert.ToDateTime(bdateto);

                ViewBag.Fromdatetime = Fromdatetime;
                ViewBag.ToDatetime = ToDatetime;

                obj = db.Expenses.Where(m => DbFunctions.TruncateTime(m.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Exp) <= DbFunctions.TruncateTime(todate)).ToList();
            }
            else
            {
                obj = db.Expenses.ToList();
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult ExpensesList(string Pf_Code, string Category, string ToDatetime, string Fromdatetime, string Submit)
        {
            ViewBag.Fromdatetime = Fromdatetime;
            ViewBag.ToDatetime = ToDatetime;
            Pf_Code = Session["pfCode"].ToString();
            ViewBag.Pf_Code = Pf_Code; //new SelectList(db.Franchisees, "Pf_Code", "Pf_Code");


            var Cat = new List<SelectListItem>
    {
        new SelectListItem{ Text="Select All", Value = "" },
        new SelectListItem{ Text="Load Connecting exp 1st and 2nd", Value = "Load Connecting exp 1st and 2nd" },
        new SelectListItem{ Text="Load connecting exp - Night load", Value = "Load connecting exp - Night load" },
        new SelectListItem{ Text="Pick up expenses", Value = "Pick up expenses"},
        new SelectListItem{ Text="Patpedhi Deposit", Value = "Patpedhi Deposit"},
        new SelectListItem{ Text="Salary Advance", Value = "Salary Advance"},
        new SelectListItem{ Text="Office Expenses", Value = "Office Expenses"},
        new SelectListItem{ Text="Fuel Exp", Value = "Fuel Exp"},
        new SelectListItem{ Text="Tea and refreshments exp", Value = "Tea and refreshments exp"},
        new SelectListItem{ Text="Packing Expenses", Value = "Packing Expenses"},
        new SelectListItem{ Text="Others", Value = "Others"},
    };

            ViewData["Category"] = Cat;


           

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

            string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);


            List<Expense> list = new List<Expense>();
            if ((Pf_Code != null && Pf_Code != "") && (Category != null && Category != ""))
            {
                list = db.Expenses.Where(m => m.Pf_Code == Pf_Code && m.Category == Category && DbFunctions.TruncateTime(m.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Exp) <= DbFunctions.TruncateTime(todate)).ToList();
            }
            else if ((Pf_Code != null && Pf_Code != "") || (Category == null && Category == ""))
            {
                list = db.Expenses.Where(m => m.Pf_Code == Pf_Code && DbFunctions.TruncateTime(m.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Exp) <= DbFunctions.TruncateTime(todate)).ToList();
            }
            else if ((Pf_Code == null && Pf_Code == "") || (Category != null && Category != ""))
            {
                list = db.Expenses.Where(m => m.Category == Category && DbFunctions.TruncateTime(m.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Exp) <= DbFunctions.TruncateTime(todate)).ToList();
            }
            else if((Pf_Code == null && Pf_Code == "") || (Category == null && Category == "") || (ToDatetime != "" && Fromdatetime != null))
            {
                list = db.Expenses.Where(m=> DbFunctions.TruncateTime(m.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Exp) <= DbFunctions.TruncateTime(todate)).ToList();
            }
            else
            {
                list = db.Expenses.ToList();
            }


            if (Submit == "Export to Excel")
            {
                ExportToExcelAll.ExportToExcelAdmin(list);
            }
            return View(list);
        }


        public ActionResult EditExpenses(long? id)
        {

            ViewBag.Pf_Code = Session["pfCode"].ToString();//new SelectList(db.Franchisees, "Pf_Code", "Pf_Code");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditExpenses(Expense expense)
        {
            if (ModelState.IsValid)
            {

                db.Entry(expense).State = EntityState.Modified;
                db.SaveChanges();
                TempData["updated"] = "Updated successfully";
                return RedirectToAction("ExpensesList");
            }


            return View(expense);
        }


        public ActionResult DeleteExpenses(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            db.Expenses.Remove(expense);
            db.SaveChanges();
            TempData["delete"] = "Deleted successfully";
            return RedirectToAction("ExpensesList");
        }





        public ActionResult LogOut()
        {


            // Microsoft.SqlServer.Management.Smo.Backup backup = new Microsoft.SqlServer.Management.Smo.Backup();
            // //Set type of backup to be performed to database
            // backup.Action = Microsoft.SqlServer.Management.Smo.BackupActionType.Database;
            // backup.BackupSetDescription = "BackupDataBase description";
            // //Set the name used to identify a particular backup set.
            // backup.BackupSetName = "Backup";
            // //specify the name of the database to back up
            // backup.Database = "DtdcBilling";
            // backup.Initialize = true;
            // backup.Checksum = true;
            // //Set it to true to have the process continue even after checksum error.
            // backup.ContinueAfterError = true;
            // //Set the backup expiry date.
            // backup.ExpirationDate = DateTime.Now.AddDays(3);
            // //truncate the database log as part of the backup operation.
            // backup.LogTruncation = Microsoft.SqlServer.Management.Smo.BackupTruncateLogType.Truncate;



            // Microsoft.SqlServer.Management.Smo.BackupDeviceItem deviceItem = new Microsoft.SqlServer.Management.Smo.BackupDeviceItem(
            //                     "E:\\DtdcBilling1.Bak",
            //                     Microsoft.SqlServer.Management.Smo.DeviceType.File);
            // backup.Devices.Add(deviceItem);

            //     ServerConnection connection = new ServerConnection(@"43.255.152.26");

            // // Log in using SQL authentication
            // connection.LoginSecure = false;
            // connection.Login = "DtdcBilling";
            // connection.Password = "Billing@123";
            // Microsoft.SqlServer.Management.Smo.Server sqlServer = new Microsoft.SqlServer.Management.Smo.Server(connection);


            ////start the back up operation

            // backup.SqlBackup(sqlServer);


            //SqlConnection con = new SqlConnection();
            //SqlCommand sqlcmd = new SqlCommand();
            //SqlDataAdapter da = new SqlDataAdapter();


            //con.ConnectionString = @"Data Source=43.255.152.26;Initial Catalog=DtdcBilling;User id=DtdcBilling;Password=Billing@123";



            //string backupDIR = Server.MapPath("~/Content/");

            //if (!System.IO.Directory.Exists(Server.MapPath(backupDIR)))
            //{
            //    System.IO.Directory.CreateDirectory(Server.MapPath(backupDIR));
            //}
            //try
            //{
            //    con.Open();
            //    sqlcmd = new SqlCommand("backup database DtdcBilling to disk='" + backupDIR + "//" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".Bak'", con);
            //    sqlcmd.ExecuteNonQuery();
            //    con.Close();

            //}
            //catch (Exception ex)
            //{
            //    con.Close();
            //}

            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Data Source=43.255.152.26;Initial Catalog=DtdcBilling;User id=DtdcBilling;Password=Billing@123"].ConnectionString);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = con;
            //cmd.CommandText = "BACKUP DATABASE MyDB TO DISK = 'E:\\DB.bak'";
            //con.Open();
            //cmd.ExecuteNonQuery();
            //con.Close();




            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
                               //return RedirectToAction("Adminlogin", "Admin");
            string SubPath = "http://codetentacles-005-site2.htempurl.com";
            return Redirect(SubPath);
           

        }
    }
}
