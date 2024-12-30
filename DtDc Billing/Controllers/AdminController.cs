using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using DtDc_Billing.CustomModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Mail;
using Razorpay.Api;
using Microsoft.Reporting.WebForms;
using System.Text.RegularExpressions;
using static DtDc_Billing.Models.sendEmail;
using Microsoft.SqlServer.Management.Sdk.Differencing;
using Microsoft.Win32;
using System.Net.Http;
using WebGrease.Css.ImageAssemblyAnalysis.LogModel;
using DocumentFormat.OpenXml.Drawing;
using PagedList;
using System.Runtime.Remoting.Messaging;
using SixLabors.ImageSharp;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using ClosedXML;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace DtDc_Billing.Controllers
{
   // [OutputCache(CacheProfile = "Cachefast")]
    public class AdminController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Adminsss


        public ActionResult test()
        {

            var getSummary = db.MonthlyDataAnalysis().ToList().Select(x => new MonthlyDataAnalysisModel
            {
                PFCode = x.PFCode,
                InvoiceCount = x.InvoiceCount ?? 0,
                TotalInvoiceAmount = x.TotalInvoiceAmount ?? 0,
                PaidAmount = x.PaidAmount ?? 0,
                UnpaidAmount = x.UnpaidAmount ?? 0,
                FranchiseName = x.FranchiseName,
                OwnerName = x.OwnerName,
                // EmailId = x.EmailId,
                LastMonth = x.LastMonth,
                CashAmount = x.CashAmount ?? 0
            }).FirstOrDefault();

            return View(getSummary);
        }

      //  [SessionUserModule]
        public ActionResult AdminLogin(string ReturnUrl)
        {

            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }


        public ActionResult LiveData()
        {
            var getData = db.getLiveData().Select(x => new LiveDataModel
            {
                TotalNoOfInvoice = x.TotalNoOfInvoice ?? 0,
                TotalInvoiceAmount = x.TotalInvoiceAmount ?? 0,
                TotalUser = x.TotalUser ?? 0,
                TotalConsignmentBooked = x.TotalConsignmentBooked ?? 0

            }).FirstOrDefault();
            return PartialView("LiveData", getData);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult TermsCondition()
        {
            return View();
        }
        public ActionResult RefundandCancellationPolicy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ContactUs(ContactUsModel contact)
        {
            if (ModelState.IsValid)
            {
                string subject = "Contact Us";

                //Base class for sending email  
                MailMessage _mailmsg = new MailMessage();

                //Make TRUE because our body text is html  
                _mailmsg.IsBodyHtml = true;

                //Set From Email ID  
                _mailmsg.From = new MailAddress("frbillingsoftware@gmail.com");

                //Set To Email ID  
                _mailmsg.To.Add(contact.email);

                //Set Subject  
                _mailmsg.Subject = subject;

                //Set Body Text of Email   
                _mailmsg.Body = contact.website;


                //Now set your SMTP   
                SmtpClient _smtp = new SmtpClient();

                //Set HOST server SMTP detail  
                _smtp.Host = "smtp.gmail.com";

                //Set PORT number of SMTP  
                _smtp.Port = 587;

                //Set SSL --> True / False  
                _smtp.EnableSsl = true;
                _smtp.UseDefaultCredentials = false;
                //Set Sender UserEmailID, Password  
                NetworkCredential _network = new NetworkCredential("frbillingsoftware@gmail.com", "rqaynjbevkygswkx");
                _smtp.Credentials = _network;

                //Send Method will send your MailMessage create above.  
                _smtp.Send(_mailmsg);
                TempData["success"] = "Mail has been send successfully!!";
            }

            return PartialView("ContactUsPartialView", contact);
        }

        public ActionResult newsletters(newsletters newsletters)
        {
            if (ModelState.IsValid)
            {
                string subject = "Contact Us";

                //Base class for sending email  
                MailMessage _mailmsg = new MailMessage();

                //Make TRUE because our body text is html  
                _mailmsg.IsBodyHtml = true;

                //Set From Email ID  
                _mailmsg.From = new MailAddress("frbillingsoftware@gmail.com");

                //Set To Email ID  
                _mailmsg.To.Add(newsletters.email);

                //Set Subject  
                _mailmsg.Subject = subject;

                //Set Body Text of Email   
                _mailmsg.Body = "mail id from newsletters " + newsletters.email;


                //Now set your SMTP   
                SmtpClient _smtp = new SmtpClient();

                //Set HOST server SMTP detail  
                _smtp.Host = "smtp.gmail.com";

                //Set PORT number of SMTP  
                _smtp.Port = 587;

                //Set SSL --> True / False  
                _smtp.EnableSsl = true;
                _smtp.UseDefaultCredentials = false;
                //Set Sender UserEmailID, Password  
                NetworkCredential _network = new NetworkCredential("frbillingsoftware@gmail.com", "rqaynjbevkygswkx");
                _smtp.Credentials = _network;

                //Send Method will send your MailMessage create above.  
                _smtp.Send(_mailmsg);
                TempData["success"] = "Mail sended!!";
            }
            return PartialView("newslettersPartialView", newsletters);
        }

        [HttpPost]
        public ActionResult AdminLogin(AdminLogin login, string ReturnUrl)
        {

            if (ModelState.IsValid)
            {
                var ObjData = db.getLogin(login.UserName.Trim(), login.Password.Trim(), "").Select(x => new registration { registrationId = x.registrationId, userName = x.username, Pfcode = x.Pfcode, referralCode = x.referralCode, franchiseName =x.franchiseName, emailId =x.emailId, dateTime =x.dateTime , ownerName =x.ownerName, isPaid =x.isPaid , mobileNo =x.mobileNo, address =x.address, IsRenewal=x.IsRenewal, IsRenewalEmail =x.IsRenewalEmail , IsRenewalEmailDate =x.IsRenewalEmailDate , subscriptionForInDays =x.subscriptionForInDays , paymentDate =x.paymentDate ,FirstTimeLoginTime=x.FirstTimeLoginTime,LoginCount=x.LoginCount,password=x.password}).FirstOrDefault();

                if (ObjData != null)
                {
                    if (!ObjData.isPaid ?? false)
                    {
                        ModelState.AddModelError("LoginAuth", "Your account is not activated. Please feel free to contact us at +91 9209764995..");
                        return View();
                    }


                    if(ObjData.LoginCount==null || !(ObjData.LoginCount> 4))
                    {
                        if (ObjData.FirstTimeLoginTime == null)
                        {
                            var logindata = db.registrations.Where(x => x.userName == ObjData.userName && x.password == ObjData.password).FirstOrDefault();
                            logindata.FirstTimeLoginTime = DateTime.Now;
                            logindata.FirstTimeLoginTime = DateTime.Now.Date; // Initialize last login date
                            logindata.LoginCount = 1; // Initialize login count

                            db.Entry(logindata).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["ShowModal "] = true;
                        }
                        else
                        {
                            var updatedLogintime = ObjData.FirstTimeLoginTime.Value;
                            var timeDifference =DateTime.Now - updatedLogintime;


                            if (timeDifference.Hours < 24)
                            {
                                if (ObjData.LoginCount <2)
                                {
                                    ObjData.LoginCount += 1;
                                    db.Entry(ObjData).State = EntityState.Modified;
                                    db.SaveChanges();

                                    TempData["ShowModal "] = true;
                                }
                                else
                                {
                                    TempData["ShowModal "] = false;
                                }
                            }
                            else if (timeDifference.Hours>24 &&ObjData.LoginCount>2 &&  timeDifference.Hours < 48)
                            {
                                if (ObjData.LoginCount < 4)
                                {
                                    ObjData.LoginCount += 1;
                                    db.Entry(ObjData).State = EntityState.Modified;
                                    db.SaveChanges();
                                    TempData["ShowModal "] = true;
                                }
                                else
                                {
                                    TempData["ShowModal "] = false;
                                }
                            }
                        }


                    }
                    else
                    {
                        TempData["ShowModal "] = false;
                    }

                    //Emal Verification Code Before Login check email is confirmed

                    //if (ObjData.isEmailConfirmed == false)
                    //{
                    //    return RedirectToAction("VerifyEmail", "Admin", new { pfcode = ObjData.Pfcode });
                    //}



                    //if (ObjData.Pfcode == "1")
                    //{
                    //     return Redirect("~/Home/Index");
                    //  //  return RedirectToAction("Index", "Home");
                    //}


                    DateTime currentDate = DateTime.Now;

                    System.DateTime newDate = ObjData.paymentDate.Value.AddDays(ObjData.subscriptionForInDays ?? 0);
                    TimeSpan date_difference = newDate - currentDate;
                    //int totalDaysDifference = date_difference.Days;
                   
                    

                    DateTime After1Year;
                    DateTime ExpiryDate;
                    var renewalstatuscheck = (from d in db.paymentLogs
                                              where d.Pfcode == ObjData.Pfcode.ToString()
                                              select new { d.RenewalStatus }).FirstOrDefault();

                    if (renewalstatuscheck.RenewalStatus == "1")
                    {
                        var Date = (from d in db.paymentLogs
                                    where d.Pfcode == ObjData.Pfcode
                                    select new
                                    {
                                        d.RenewalDate
                                    }).FirstOrDefault();

                        string strdate = Convert.ToString(Date.RenewalDate);
                        string[] strarr = strdate.Split(' ');
                        string date = strarr[0];
                        DateTime date1 = Convert.ToDateTime(date);

                        After1Year = date1.AddYears(1);
                        //DateTime edate=ObjData.subscriptionForInDays
                       
                    }
                    else
                    {
                        var Date = (from d in db.registrations
                                    where d.Pfcode == ObjData.Pfcode
                                    select new
                                    {
                                        d.dateTime
                                    }).FirstOrDefault();

                        string strdate = Convert.ToString(Date.dateTime);
                        string[] strarr = strdate.Split(' ');
                        string date = strarr[0];
                        DateTime date1 = Convert.ToDateTime(date);
                        After1Year = date1.AddYears(1);

                    }
                    var firmlist = db.FirmDetails.ToList();

                    Session["After1Year"] = After1Year;

                    if (ObjData != null)
                    {
                        if (newDate <= DateTime.Now.Date)
                        {

                            return RedirectToAction("ExpiredDate", "Admin");
                        }

                        else
                        {
                            if ((newDate - DateTime.Now).TotalDays < 30)
                            {
                                var updation = db.registrations.Where(x => x.userName == ObjData.userName && x.password == ObjData.password).FirstOrDefault();

                                if (ObjData.IsRenewalEmailDate != DateTime.Now.Date)
                                {
                                    updation.IsRenewalEmail = "0";
                                    updation.password = login.Password;
                                    db.Entry(updation).State = EntityState.Modified;
                                    db.SaveChanges();
                                }


                            }

                        }

                        //Set Cookies For Some Time


                       // var ObjData = db.registrations.Where(x => x.Pfcode == pfcode).FirstOrDefault();


                        HttpCookie cookie = new HttpCookie("Cookies");
                        cookie["AdminValue"] = ObjData.Pfcode.ToString();
                        cookie["UserValue"] = ObjData.userName.ToString();
                        cookie["Admin"] = ObjData.registrationId.ToString();
                        cookie["UserName"] = ObjData.userName.ToString();
                        cookie["pfCode"] = ObjData.Pfcode.ToString();
                        cookie["CurrentYear"] = DateTime.Now.Year.ToString();
                        cookie["YearStart"] = DateTime.Now.AddYears(-2).Year.ToString();
                        cookie["datayear"] = "financialYear";
                        cookie.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(cookie);



                        cookie["referalCode"] = ObjData.referralCode.ToString();
                        
                        cookie.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(cookie);



                        int customTimeout = 1440;
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                                          1,
                         ObjData.registrationId.ToString(),
                          DateTime.Now,
                         DateTime.Now.AddMinutes(customTimeout),  // Expiration time
                          false,
                            ObjData.userName
                        );
                        string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        authCookie.Expires = ticket.Expiration;
                        Response.Cookies.Add(authCookie);

                        var objaccessPAge = (from d in db.AdminAccessPages
                                             where d.Pfcode == ObjData.Pfcode.ToString()
                                             select new { d.Accesspage }).FirstOrDefault();

                        var renewalstatus = (from d in db.paymentLogs
                                             where d.Pfcode == ObjData.Pfcode.ToString()
                                             select new { d.RenewalStatus }).FirstOrDefault();

                        if (objaccessPAge != null)
                        {
                            Session["AccessPage"] = objaccessPAge.Accesspage.ToString();
                        }
                        else
                        {
                            Session["AccessPage"] = "0";

                        }
                        ///Set Cookies For Some Time
                        return RedirectToAction("Index", "Home");



                        //return RedirectToAction("VerifyLogin", new { pfcode=ObjData.Pfcode });





                        // return RedirectToAction("AnotherAction");
                        //Session["Admin"] = ObjData.registrationId.ToString();
                        //Session["UserName"] = ObjData.userName.ToString();
                        //Session["PFCode"] = ObjData.Pfcode.ToString();
                        // Session["firmlist"] = firmlist;
                        //string decodedUrl = "";

                        //HttpCookie cookie = new HttpCookie("Cookies");
                        //cookie["AdminValue"] = ObjData.Pfcode.ToString();
                        //cookie["UserValue"] = ObjData.userName.ToString();
                        //cookie["Admin"] = ObjData.registrationId.ToString();
                        //cookie["UserName"] = ObjData.userName.ToString();
                        //cookie["pfCode"] = ObjData.Pfcode.ToString();
                        //cookie.Expires = DateTime.Now.AddDays(1);
                        //Response.Cookies.Add(cookie);



                        //cookie["referalCode"] = ObjData.referralCode.ToString();
                        //cookie.Expires = DateTime.Now.AddDays(1);
                        //Response.Cookies.Add(cookie);



                        //int customTimeout = 1440;
                        //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        //                  1,
                        // ObjData.registrationId.ToString(),
                        //  DateTime.Now,
                        // DateTime.Now.AddMinutes(customTimeout),  // Expiration time
                        //  false,
                        //    ObjData.userName
                        //);
                        //string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                        //HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        //authCookie.Expires = ticket.Expiration;
                        //Response.Cookies.Add(authCookie);

                        //var objaccessPAge = (from d in db.AdminAccessPages
                        //                     where d.Pfcode == ObjData.Pfcode.ToString()
                        //                     select new { d.Accesspage }).FirstOrDefault();

                        //var renewalstatus = (from d in db.paymentLogs
                        //                     where d.Pfcode == ObjData.Pfcode.ToString()
                        //                     select new { d.RenewalStatus }).FirstOrDefault();

                        //if (objaccessPAge != null)
                        //{
                        //    Session["AccessPage"] = objaccessPAge.Accesspage.ToString();
                        //}
                        //else
                        //{
                        //    Session["AccessPage"] = "0";

                        //}
                        //if (!string.IsNullOrEmpty(ReturnUrl))
                        //    decodedUrl = Server.UrlDecode(ReturnUrl);

                        ////Login logic...

                        //if (Url.IsLocalUrl(decodedUrl))
                        //{
                        //    return Redirect(decodedUrl);
                        //}
                        //else
                        //{
                        //    //if (currentDate > newDate)
                        //    //{

                        //    //    ModelState.AddModelError("freedome", "Free demo of 30 days is expired");
                        //    //    return View();
                        //    //}
                        //    //if (newDate <= DateTime.Now.Date)
                        //    //{
                        //    //    ModelState.AddModelError("LoginAuth", "Your Subscription is Expired");
                        //    //    return RedirectToAction("Index", "Admin");
                        //    //}

                        //    return RedirectToAction("Index", "Home");
                        //}
                    }

                }

                else
                {
                    //ModelState.AddModelError("LoginAuth", "Username or Password Is Incorrect");
                    ModelState.AddModelError("LoginAuth", "Username or Password Is Incorrect or Please Do The Registration First");

                }
            }
           
                return View();
    
        }
        public JsonResult UpdateYearStart(string year)
        {
            //cookie["CurrentYear"] = DateTime.Now.Year.ToString();
            HttpCookie cookie = Request.Cookies["Cookies"];
            if (cookie != null)
            {
               if(year== "financialYear")
                {
                    cookie["YearStart"] = DateTime.Now.AddYears(-1).Year.ToString();
                    cookie["CurrentYear"] = DateTime.Now.Year.ToString();

                }
               else if(year== "FinancialYearwithLast")
                {
                    cookie["YearStart"] = DateTime.Now.AddYears(-2).Year.ToString();
                    cookie["CurrentYear"] = DateTime.Now.Year.ToString();
                }
               else if(year== "FinancialWithLast2year")
                {
                    cookie["YearStart"] = DateTime.Now.AddYears(-3).Year.ToString();
                    cookie["CurrentYear"] = DateTime.Now.Year.ToString();
                }
               else if(year== "All")
                {
                    cookie["YearStart"] = "0";
                    cookie["CurrentYear"] ="0";
                }
                cookie["datayear"]=year;

                cookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(cookie);

                return Json(new { success = true, yearStart =/* cookie["YearStart"]*/year }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, yearStart = /*cookie["YearStart"]*/year }, JsonRequestBehavior.AllowGet);

        }
        public string CountIncreaseofLogin()
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var login = db.registrations.Where(x => x.Pfcode == strpfcode).FirstOrDefault();
            if (login != null)
            {
                login.LoginCount = Convert.ToInt32(login.LoginCount ?? 0 + 1);
                db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();

                return "Increase";
            }
            return "Something Went Wrong";

        }
        public bool SetCookies(string pfcode)
        {
            var ObjData = db.registrations.Where(x=>x.Pfcode==pfcode).FirstOrDefault(); 


            HttpCookie cookie = new HttpCookie("Cookies");
            cookie["AdminValue"] = ObjData.Pfcode.ToString();
            cookie["UserValue"] = ObjData.userName.ToString();
            cookie["Admin"] = ObjData.registrationId.ToString();
            cookie["UserName"] = ObjData.userName.ToString();
            cookie["pfCode"] = ObjData.Pfcode.ToString();
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);



            cookie["referalCode"] = ObjData.referralCode.ToString();
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);



            int customTimeout = 1440;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                              1,
             ObjData.registrationId.ToString(),
              DateTime.Now,
             DateTime.Now.AddMinutes(customTimeout),  // Expiration time
              false,
                ObjData.userName
            );
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            authCookie.Expires = ticket.Expiration;
            Response.Cookies.Add(authCookie);

            var objaccessPAge = (from d in db.AdminAccessPages
                                 where d.Pfcode == ObjData.Pfcode.ToString()
                                 select new { d.Accesspage }).FirstOrDefault();

            var renewalstatus = (from d in db.paymentLogs
                                 where d.Pfcode == ObjData.Pfcode.ToString()
                                 select new { d.RenewalStatus }).FirstOrDefault();

            if (objaccessPAge != null)
            {
                Session["AccessPage"] = objaccessPAge.Accesspage.ToString();
            }
            else
            {
                Session["AccessPage"] = "0";

            }
            return true;
        }
        public ActionResult ExpiredDate()
        {
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

        //public ActionResult NewpasswordSave()
        //{

        //    using (MailMessage mm = new MailMessage("codetentacles@gmail.com", "nileshveer17@gmail.com"))
        //    {

        //        mm.Subject = "Token Verification for Change Password";
        //        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //        var stringChars = new char[6];
        //        var random = new Random();

        //        for (int i = 0; i < stringChars.Length; i++)
        //        {
        //            stringChars[i] = chars[random.Next(chars.Length)];
        //        }

        //        var Tokne = new String(stringChars);

        //        string Bodytext = "<html><body>Your Verification Token is -" + "<strong>"+Tokne+"<strong>" + " </body></html>";

        //        mm.IsBodyHtml = true;



        //        mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

        //        AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
        //        // mm.Body = Bodytext;
        //        mm.Body = Bodytext;

        //        //Add Byte array as Attachment.

        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.EnableSsl = true;
        //        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
        //        credentials.UserName = "codetentacles@gmail.com";
        //        credentials.Password = "Codeadmin";
        //        smtp.UseDefaultCredentials = true;
        //        smtp.Credentials = credentials;
        //        smtp.Port = 587;
        //        smtp.Send(mm);

        //        Admin admin = db.Admins.FirstOrDefault();
        //        if (admin != null)
        //        {
        //            admin.Token = Tokne;
        //            db.Admins.Attach(admin);
        //            db.Entry(admin).Property(x => x.Token).IsModified = true;
        //            db.SaveChanges();
        //        }


        //        ViewBag.sendmail = "Mail send to Your Emailid";
        //    }
        //    return Json("chamara", JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult NewpasswordSave(string currentpass, string newpass, string Token)
        //{
        //    var obj = db.Admins.Where(m => m.Token == Token).FirstOrDefault();
        //    Admin admin = db.Admins.FirstOrDefault();
        //    if (obj != null)
        //    {
        //        admin.A_Password = newpass;
        //        db.Admins.Attach(admin);
        //        db.Entry(admin).Property(x => x.A_Password).IsModified = true;
        //        db.SaveChanges();
        //        ViewBag.changepass = "Password Change Successfullly";
        //    }
        //    return RedirectToAction("AdminChangePass", "Admin");
        //}


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

                filePath = path + System.IO.Path.GetFileName(ImportText.FileName);
                string extension = System.IO.Path.GetExtension(ImportText.FileName);
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
        public ActionResult CreateUser(User user, string[] Usertype, string[] ModuletypeCash, string[] ModuletypeBilling)
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
                string str = "", strbilling = "";

                string[] split = user.Usertype.Split(',');

                if (ModuletypeCash.Count() != null)
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

                if (ModuletypeBilling.Count() != null)
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

            var modulelist = db.UserModuleLists.Where(m => m.User_Id == id && m.Usertype == "CashCounter").Select(m => m.ModuleName).ToList();

            foreach (var item in items1)
            {
                foreach (var list in modulelist)
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
                             where d.UserName == user.Name
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

        //public ActionResult NewRegisterClient()
        //{
        //    var register=db.registrations.ToList();
        //    DateTime currentdate = DateTime.Now;

        //    //System.DateTime newDate = register.Value.AddDays(ObjData.subscriptionForInDays ?? 0);




        //    List<NewRegisterUser> rg = (from registration in register
        //                               let expirationDate = registration.paymentDate.HasValue ? registration.paymentDate.Value.AddDays(registration.subscriptionForInDays ?? 0) : (DateTime?)null

        //                                select new NewRegisterUser
        //                             {
        //                                 registrationId = registration.registrationId,
        //                                 Pfcode=registration.Pfcode,
        //                                 franchiseName=registration.franchiseName,
        //                                 emailId=registration.emailId,
        //                                 mobileNo=registration.mobileNo,
        //                                 dateTime=registration.dateTime.Value.ToString("dd/MM/yyyy"),
        //                                 userName=registration.userName,    
        //                                 password=registration.password,
        //                                 DaysSinceRegistration=(currentdate-registration.dateTime.Value).Days,
        //                                 subscriptionfordays=register.Select(x=>x.subscriptionForInDays).FirstOrDefault(),
        //                                    ExpireDate = expirationDate.HasValue ? expirationDate.Value: null,
        //                                   // ExpiredDays = expirationDate.HasValue ? (int?)(expirationDate.Value - currentdate).Days : (int?)null,

        //                                })
        //                                 .OrderByDescending(x => x.DaysSinceRegistration) // Sort in descending order

        //                             .ToList();


        //    return View(rg);
        //}

        public ActionResult UpdateRemark(long registrationId, string remark)
        {
            var getData = db.registrations.Where(x => x.registrationId == registrationId).FirstOrDefault();

            if (getData != null)
            {
                getData.Remark = remark;
                db.Entry(getData).State = EntityState.Modified;

                db.SaveChanges();

            }

            TempData["update"] = "Update successfully";
            return RedirectToAction("NewRegisterClient", "Admin");
        }



        [HttpGet]
        public ActionResult NewRegisterClient()
        {
            DateTime currentdate = DateTime.Now;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;

            // Retrieve data from database
            var registrations = db.registrations.ToList(); // Assuming registrations is your DbSet

            //ViewBag.remarks = new SelectList(registrations, "Remark", "Remark");
            // Project the data into NewRegisterUser objects
            var rg = registrations.Select(x => new NewRegisterUser
            {
                registrationId = x.registrationId,
                Pfcode = x.Pfcode,
                franchiseName = x.franchiseName,
                emailId = x.emailId,
                mobileNo = x.mobileNo,
                dateTime = x.dateTime.HasValue ? x.dateTime.Value.ToString("dd/MM/yyyy") : null,
                userName = x.userName,
                password = x.password,
                isPaid = x.isPaid,
                DaysSinceRegistration = (currentdate - (x.dateTime ?? currentdate)).Days,

                subscriptionfordays = x.subscriptionForInDays ?? 0,
                ExpireDate = x.paymentDate.HasValue ? x.paymentDate.Value.AddDays(x.subscriptionForInDays ?? 0).ToString("dd/MM/yyyy") : null,
                ExpiredDays = x.paymentDate.HasValue ? (x.paymentDate.Value.AddDays(x.subscriptionForInDays ?? 0) - currentdate).Days : 0,
                Remark = x.Remark
            })
            .OrderByDescending(x => x.DaysSinceRegistration)
            .ToList();

            return View(rg);
        }

        [HttpGet]
        public ActionResult RenewSubcriptionExpClient(string Pfcode, int pid)
        {

            if (String.IsNullOrEmpty(Pfcode) && pid != 0)
            {
                TempData["ErrorMessage"] = "Select Payment Subscription";
                return RedirectToAction("NewRegisterClient");
            }
            Pfcode = Pfcode.ToString().ToUpper();

            var register = db.registrations.Where(x => x.Pfcode.ToUpper() == Pfcode).FirstOrDefault();
            var package = db.Packages.Where(x => x.Pid == pid).FirstOrDefault();
            if (register == null)
            {
                TempData["ErrorMessage"] = "User not registered. Please register before activating your account.";

                return RedirectToAction("NewRegisterClient");
            }

            if (register != null)
            {
                register.subscriptionForInDays = package.Subcriptionforindays;
                register.isPaid = package.isPaid;
                register.paymentDate = DateTime.Now;
                register.IsRenewal = "Yes";
                db.Entry(register).State = EntityState.Modified;
                  db.SaveChanges();
            }


            paymentLog paymentdata = new paymentLog();
            var pdata = db.paymentLogs.Where(x => x.Pfcode.ToUpper() == Pfcode).FirstOrDefault();
            if (pdata != null)
            {
                pdata.Pfcode = pdata.Pfcode;
                pdata.ownerName = register.ownerName;
                pdata.totalAmount = package.Amount;
                pdata.registrationId = register.registrationId;
                pdata.status = "authorized";
                pdata.dateTime = DateTime.Now;
                pdata.description = package.Despription;
                pdata.paymentmethod = "Cash".ToUpper();
                pdata.RenewalDate = DateTime.Now;

                db.Entry(pdata).State = EntityState.Modified;
              db.SaveChanges();

                TempData["SuccessMessage"] = "Renewal Subscription Done Successfully!!!";

            }
            return RedirectToAction("NewRegisterClient");

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
                franchisee.IsGECSector = false;
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
        public ActionResult DeleteSingleSector(int sectorId)
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();


            List<Dtdc_Ptp> dtdc_Ptps = db.Dtdc_Ptp.Where(m => m.Sector_Id == sectorId).ToList();
            List<dtdcPlu> dtdcPlu = db.dtdcPlus.Where(m => m.Sector_Id == sectorId).ToList();
            List<express_cargo> express_cargo = db.express_cargo.Where(m => m.Sector_Id == sectorId).ToList();
            List<Nondox> Nondox = db.Nondoxes.Where(m => m.Sector_Id == sectorId).ToList();
            List<Ratem> Ratem = db.Ratems.Where(m => m.Sector_Id == sectorId).ToList();
            List<Priority> pra = db.Priorities.Where(m => m.Sector_Id == sectorId).ToList();
            List<Dtdc_Ecommerce> ecom = db.Dtdc_Ecommerce.Where(m => m.Sector_Id == sectorId).ToList();


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
            foreach (var i in ecom)
            {
                db.Dtdc_Ecommerce.Remove(i);
            }

            db.SaveChanges();

            var getSector = db.Sectors.Where(x => x.Sector_Id == sectorId && x.Pf_code == pfcode && x.BillGecSec==null).FirstOrDefault();

            if (getSector != null)
            {
                db.Sectors.Remove(getSector);
                db.SaveChanges();
            }

            return RedirectToAction("FranchiseeList", new { tab = "2" });
        }

        public ActionResult AddNewSectorSingle(string Sector_Name, string Pincode_values, int Prior)
        {
            string message = "";
            var flag = 0;
            if (Sector_Name == "")
            {
                message = "Sector name required";
                flag = 1;
            }

            if (Pincode_values == "")
            {
                message = "Pincode required";
                flag = 1;
            }

            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var checkExist = db.Sectors.Where(x => x.Sector_Name.ToUpper() == Sector_Name.ToUpper() && x.Pf_code == pfcode ).FirstOrDefault();

            if (checkExist != null)
            {
                message = "Sector already exist";
                flag = 1;
            }

            if (flag == 1)
            {
                TempData["errormsg"] = message;
                return RedirectToAction("FranchiseeList", new { tab = "2" });
            }
            Sector str = new Sector();
            str.Priority = (Prior + 1);
            str.Sector_Name = Sector_Name.ToUpper();
            str.Pincode_values = Pincode_values;
            str.BillD = false;
            str.BillNonAir = false;
            str.BillNonSur = false;

            str.BillExpCargo = false;
            str.BillPriority = false;

            str.BillEcomPrio = false;
            str.BillEcomGE = false;
            str.Pf_code = pfcode;
            try
            {
                db.Sectors.Add(str);
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            message = "Added successfully";

            return RedirectToAction("FranchiseeList", new { tab = "2" });
        }

        [SessionAdmin]
        public ActionResult Add_SectorPin(string PfCode)
        {
            string Pf = PfCode; /*Session["PfID"].ToString();*/

            List<SectorNewModel> st = new List<SectorNewModel>();

            st = (from u in db.Sectors
                  where u.Pf_code == Pf
                  select new SectorNewModel
                  {
                      Sector_Id = u.Sector_Id,
                      Sector_Name = u.Sector_Name,
                      Pf_code = u.Pf_code,
                      Pincode_values = u.Pincode_values,
                      Priority = u.Priority,
                      CashD = u.CashD,
                      CashN = u.CashN,
                      BillD = u.BillD ?? false,
                      BillNonAir = u.BillNonAir ?? false,
                      BillNonSur = u.BillNonSur ?? false,
                      BillExpCargo = u.BillExpCargo ?? false,
                      BillPriority = u.BillPriority ?? false,
                      BillEcomPrio = u.BillEcomPrio ?? false,
                      BillEcomGE = u.BillEcomGE ?? false,
                      BillGecSec=null
                     
                  }).OrderBy(x => x.Priority).ToList();


            ViewBag.pfcode = PfCode;//stored in hidden format on the view


            return View(st);
        }



        [SessionAdmin]
        [HttpPost]
        public ActionResult Add_SectorPin(registration franchisee, FormCollection fc)
        {
            franchisee.Pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var sectorNamearray = fc.GetValues("item.Sector_Name");

            var priorityarray = fc.GetValues("item.Priority");
            // Retrieve the checkbox value from the FormCollection

            // Assuming 'i' is the index you are using in the checkbox names
            int numberOfCheckboxes = sectorNamearray.Count() /* Set the number based on your logic */;
            List<bool> billDValues = new List<bool>();

            List<bool> BillNonAirValues = new List<bool>();
            List<bool> BillNonSurValues = new List<bool>();

            List<bool> BillExpCargoValues = new List<bool>();
            List<bool> BillPriorityValues = new List<bool>();

            List<bool> BillEcomPrioValues = new List<bool>();
            List<bool> BillEcomGEValues = new List<bool>();

            for (int i = 1; i <= numberOfCheckboxes; i++)
            {
                // Construct the dynamic name for the checkbox
                string checkboxName = $"BillD[{i}]";

                // Check if the checkbox with the dynamic name exists in the form collection
                if (fc.AllKeys.Contains(checkboxName))
                {
                    // Retrieve the value (which is "true" or null if not checked)
                    string value = fc[checkboxName];

                    // Convert the string value to the desired data type if needed
                    bool isChecked = !string.IsNullOrEmpty(value) && value.ToLower() == "true";

                    // Add the boolean value to the list
                    billDValues.Add(isChecked);
                }
                else
                {
                    // If the checkbox doesn't exist in the form collection, you may want to handle this case
                    // You can decide the default value or any other action
                    billDValues.Add(false);
                }
            }


            for (int i = 1; i <= numberOfCheckboxes; i++)
            {
                // Construct the dynamic name for the checkbox
                string checkboxNameN = $"BillNonAir[{i}]";

                // Check if the checkbox with the dynamic name exists in the form collection
                if (fc.AllKeys.Contains(checkboxNameN))
                {
                    // Retrieve the value (which is "true" or null if not checked)
                    string value = fc[checkboxNameN];

                    // Convert the string value to the desired data type if needed
                    bool isChecked = !string.IsNullOrEmpty(value) && value.ToLower() == "true";

                    // Add the boolean value to the list
                    BillNonAirValues.Add(isChecked);
                }
                else
                {
                    // If the checkbox doesn't exist in the form collection, you may want to handle this case
                    // You can decide the default value or any other action
                    BillNonAirValues.Add(false);
                }
            }


            for (int i = 1; i <= numberOfCheckboxes; i++)
            {
                // Construct the dynamic name for the checkbox
                string checkboxNameN = $"BillNonSur[{i}]";

                // Check if the checkbox with the dynamic name exists in the form collection
                if (fc.AllKeys.Contains(checkboxNameN))
                {
                    // Retrieve the value (which is "true" or null if not checked)
                    string value = fc[checkboxNameN];

                    // Convert the string value to the desired data type if needed
                    bool isChecked = !string.IsNullOrEmpty(value) && value.ToLower() == "true";

                    // Add the boolean value to the list
                    BillNonSurValues.Add(isChecked);
                }
                else
                {
                    // If the checkbox doesn't exist in the form collection, you may want to handle this case
                    // You can decide the default value or any other action
                    BillNonSurValues.Add(false);
                }
            }


            for (int i = 1; i <= numberOfCheckboxes; i++)
            {
                // Construct the dynamic name for the checkbox
                string checkboxNameN = $"BillExpCargo[{i}]";

                // Check if the checkbox with the dynamic name exists in the form collection
                if (fc.AllKeys.Contains(checkboxNameN))
                {
                    // Retrieve the value (which is "true" or null if not checked)
                    string value = fc[checkboxNameN];

                    // Convert the string value to the desired data type if needed
                    bool isChecked = !string.IsNullOrEmpty(value) && value.ToLower() == "true";

                    // Add the boolean value to the list
                    BillExpCargoValues.Add(isChecked);
                }
                else
                {
                    // If the checkbox doesn't exist in the form collection, you may want to handle this case
                    // You can decide the default value or any other action
                    BillExpCargoValues.Add(false);
                }
            }


            for (int i = 1; i <= numberOfCheckboxes; i++)
            {
                // Construct the dynamic name for the checkbox
                string checkboxNameN = $"BillPriority[{i}]";

                // Check if the checkbox with the dynamic name exists in the form collection
                if (fc.AllKeys.Contains(checkboxNameN))
                {
                    // Retrieve the value (which is "true" or null if not checked)
                    string value = fc[checkboxNameN];

                    // Convert the string value to the desired data type if needed
                    bool isChecked = !string.IsNullOrEmpty(value) && value.ToLower() == "true";

                    // Add the boolean value to the list
                    BillPriorityValues.Add(isChecked);
                }
                else
                {
                    // If the checkbox doesn't exist in the form collection, you may want to handle this case
                    // You can decide the default value or any other action
                    BillPriorityValues.Add(false);
                }
            }


            for (int i = 1; i <= numberOfCheckboxes; i++)
            {
                // Construct the dynamic name for the checkbox
                string checkboxNameN = $"BillEcomPrio[{i}]";

                // Check if the checkbox with the dynamic name exists in the form collection
                if (fc.AllKeys.Contains(checkboxNameN))
                {
                    // Retrieve the value (which is "true" or null if not checked)
                    string value = fc[checkboxNameN];

                    // Convert the string value to the desired data type if needed
                    bool isChecked = !string.IsNullOrEmpty(value) && value.ToLower() == "true";

                    // Add the boolean value to the list
                    BillEcomPrioValues.Add(isChecked);
                }
                else
                {
                    // If the checkbox doesn't exist in the form collection, you may want to handle this case
                    // You can decide the default value or any other action
                    BillEcomPrioValues.Add(false);
                }
            }


            for (int i = 1; i <= numberOfCheckboxes; i++)
            {
                // Construct the dynamic name for the checkbox
                string checkboxNameN = $"BillEcomGE[{i}]";

                // Check if the checkbox with the dynamic name exists in the form collection
                if (fc.AllKeys.Contains(checkboxNameN))
                {
                    // Retrieve the value (which is "true" or null if not checked)
                    string value = fc[checkboxNameN];

                    // Convert the string value to the desired data type if needed
                    bool isChecked = !string.IsNullOrEmpty(value) && value.ToLower() == "true";

                    // Add the boolean value to the list
                    BillEcomGEValues.Add(isChecked);
                }
                else
                {
                    // If the checkbox doesn't exist in the form collection, you may want to handle this case
                    // You can decide the default value or any other action
                    BillEcomGEValues.Add(false);
                }
            }

            var code = (from u in db.registrations
                        where u.Pfcode == franchisee.Pfcode
                        select u).FirstOrDefault();

            for (int i = 0; i < sectorNamearray.Count(); i++)
            {
                if (sectorNamearray[i] == null || sectorNamearray[i] == "")
                {
                    ViewBag.nameRequired = "You cant save null to sector name";
                    return PartialView("Add_SectorPin", code);
                }
            }
            //Adding Eantries To the Sector Table
            var sectornamelist = db.sectorNames.ToList();
            // [RegularExpression("^[0-9]*$", ErrorMessage = "Pincode must be numeric")]

            List<SectorNewModel> datasector = new List<SectorNewModel>();

            datasector = (from u in db.Sectors
                          where u.Pf_code == franchisee.Pfcode
                           && u.BillGecSec == null

                          select new SectorNewModel
                          {
                              Sector_Id = u.Sector_Id,
                              Sector_Name = u.Sector_Name,
                              Pf_code = u.Pf_code,
                              Pincode_values = u.Pincode_values,
                              Priority = u.Priority,
                              BillD = u.BillD ?? false,
                              BillNonAir = u.BillNonAir ?? false,
                              BillNonSur = u.BillNonSur ?? false,
                              BillExpCargo = u.BillExpCargo ?? false,
                              BillPriority = u.BillPriority ?? false,
                              BillEcomPrio = u.BillEcomPrio ?? false,
                              BillEcomGE = u.BillEcomGE ?? false
                          }).ToList();

            if (datasector != null)
            {

                var sectoridarray = fc.GetValues("item.Sector_Id");

                var pincodearayy = fc.GetValues("item.Pincode_values");

                var BillDox = fc.GetValues("item.BillD");

                var BillNonDox = fc.GetValues("item.BillN");


                for (int i = 0; i < sectoridarray.Count(); i++)
                {
                 

                    string[] strarr = pincodearayy[i].Trim().Split(',', '-');

                    for (int j = 0; j < strarr.Count(); j++)
                    {
                        strarr[j] = strarr[j].Trim();

                        if (!Regex.Match(strarr[j], @"^(\d{6})?$").Success)
                        {
                            //TempData["PinError1"] = "Pincode must be numeric!";
                            //ModelState.AddModelError("PinError1", "Pincode must be numeric");
                            //string pfcodef = "";
                            if (franchisee.Pfcode == null)
                            {
                                franchisee.Pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

                            }

                            ViewBag.DataSector = datasector;
                            ViewBag.Message = "Ensure the Pincode is numeric and free from spaces or special characters.";
                            // return View("FranchiseeList", fc);
                            return PartialView("Add_SectorPin", datasector);
                            //return View(fc);
                        }


                        else
                        {
                            Sector str = db.Sectors.Find(Convert.ToInt16(sectoridarray[i]));

                            if (pincodearayy[i] == "")
                            {
                                pincodearayy[i] = null;
                            }

                            str.Priority = Convert.ToInt32(priorityarray[i]);
                            str.Sector_Name = sectorNamearray[i].ToUpper();
                            str.Pincode_values = pincodearayy[i]?.Trim();
                            str.BillD = billDValues[i];
                            str.BillNonAir = BillNonAirValues[i];
                            str.BillNonSur = BillNonSurValues[i];

                            str.BillExpCargo = BillExpCargoValues[i];
                            str.BillPriority = BillPriorityValues[i];

                            str.BillEcomPrio = BillEcomPrioValues[i];
                            str.BillEcomGE = BillEcomGEValues[i];
                            db.Entry(str).State = EntityState.Modified;

                        }
                    }

                }
                int result = pincodearayy.Count(s => s == null);

                if (result > 0)
                {
                    ModelState.AddModelError("PinError", "All Fields Are Compulsary");

                    List<SectorNewModel> stt = datasector.Where(x => x.Pincode_values == null).ToList();
                    ViewBag.DataSector = stt;
                    return View(stt);
                }
                else
                {
                    db.SaveChanges();
                    TempData["Success"] = "Sectors Added Successfully!";
                }


                string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
                List<SectorNewModel> secct1 = (from u in db.Sectors
                                               where u.Pf_code == pfcode
                                                                                      && u.BillGecSec == null

                                               select new SectorNewModel
                                               {
                                                   Sector_Id = u.Sector_Id,
                                                   Sector_Name = u.Sector_Name,
                                                   Pf_code = u.Pf_code,
                                                   Pincode_values = u.Pincode_values,
                                                   Priority = u.Priority,
                                                   BillD = u.BillD ?? false,
                                                   BillNonAir = u.BillNonAir ?? false,
                                                   BillNonSur = u.BillNonSur ?? false,
                                                   BillExpCargo = u.BillExpCargo ?? false,
                                                   BillPriority = u.BillPriority ?? false,
                                                   BillEcomPrio = u.BillEcomPrio ?? false,
                                                   BillEcomGE = u.BillEcomGE ?? false
                                               }).ToList();


                ViewBag.DataSector = secct1;
                return View("Add_SectorPin", secct1);

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

            ViewBag.pfcode = franchisee.Pfcode;//stored in hidden format on the view
            ViewBag.DataSector = datasector;

            return View(datasector);
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
                               && u.BillGecSec==null
                               orderby u.Priority
                               select u).ToList();
            ViewBag.pfcode = PfCode;//stored in hidden format on the view
            ViewBag.DataSector = st;

            return View("Add_SectorPin", st);
        }




        //public ActionResult Edit(string PfCode)
        //{

        //    if (PfCode == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }


        //    var data = (from d in db.Franchisees
        //                where d.PF_Code == PfCode    
        //                select d).FirstOrDefault();

        //    FranchiseeModel Fr = new FranchiseeModel();

        //    Fr.PF_Code = data.PF_Code;
        //    Fr.F_Address = data.F_Address;
        //    Fr.OwnerName = data.OwnerName;
        //    Fr.BranchName = data.BranchName;
        //    Fr.GstNo = data.GstNo;
        //    Fr.Franchisee_Name = data.Franchisee_Name;
        //    Fr.ContactNo = data.ContactNo;
        //    Fr.Branch_Area = data.Branch_Area;
        //    Fr.Datetime_Fr = data.Datetime_Fr;
        //    Fr.Pan_No = data.Pan_No;
        //    Fr.Sendermail = data.Sendermail;
        //    Fr.password = data.password;
        //    Fr.AccountName = data.AccountName;
        //    Fr.Bankname = data.Bankname;
        //    Fr.Accountno = data.Accountno;
        //    Fr.IFSCcode = data.IFSCcode;
        //    Fr.Branch = data.Branch;
        //    Fr.Accounttype = data.Accounttype;
        //    Fr.InvoiceStart = data.InvoiceStart;


        //    if (Fr == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(Fr);
        //}
        //[HttpPost]
        //public ActionResult UploadStamp(FranchiseeModel franchisee)
        //{
        //    Franchisee Fr = new Franchisee();

        //    Fr.PF_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();
        //    var getNewFilePath = "";
        //    if (franchisee.StampFilePath == null)
        //    {
        //        getNewFilePath = db.Franchisees.Where(x => x.PF_Code == Fr.PF_Code).Select(x => x.StampFilePath).FirstOrDefault();
        //    }
        //    Fr.StampFilePath = (franchisee.StampFilePath == null || franchisee.StampFilePath == "") ? getNewFilePath : franchisee.StampFilePath;


        //    db.Entry(Fr).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return Json(new { success = true });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FranchiseeModel franchisee)
        {
            if (ModelState.IsValid)
            {
                var PF_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();
                try
                {
                    Franchisee Fr =db.Franchisees.Where(x => x.PF_Code == PF_Code).FirstOrDefault();

                Fr.F_Address = franchisee.F_Address;
                Fr.OwnerName = franchisee.OwnerName;
                Fr.BranchName = franchisee.BranchName;
                Fr.GstNo = franchisee.GstNo;
                Fr.Franchisee_Name = franchisee.Franchisee_Name;
                Fr.ContactNo = franchisee.ContactNo;
                Fr.Branch_Area = franchisee.Branch_Area;
                Fr.Datetime_Fr = franchisee.Datetime_Fr;
                Fr.Pan_No = franchisee.Pan_No;
                Fr.Sendermail = franchisee.Sendermail;
                Fr.password = franchisee.password;
                Fr.AccountName = franchisee.AccountName;
                Fr.Bankname = franchisee.Bankname;
                Fr.Accountno = franchisee.Accountno;
                Fr.IFSCcode = franchisee.IFSCcode;
                Fr.Branch = franchisee.Branch;
                Fr.Accounttype = franchisee.Accounttype;
                Fr.InvoiceStart = franchisee.InvoiceStart;
                Fr.IsGECSector=franchisee.IsGECSector;
                //var getNewFilePath = "";
                //if (franchisee.StampFilePath == null)
                //{
                //    getNewFilePath = db.Franchisees.Where(x => x.PF_Code == Fr.PF_Code).Select(x => x.StampFilePath).FirstOrDefault();

                //}

                //Fr.StampFilePath = (franchisee.StampFilePath == null || franchisee.StampFilePath == "") ? getNewFilePath : franchisee.StampFilePath;
             

                db.Entry(Fr).State = EntityState.Modified;
                db.SaveChanges();

                var Reg = (from d in db.registrations
                           where d.Pfcode == franchisee.PF_Code
                           select d).FirstOrDefault();

               // Reg.Pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString().ToUpper();
                Reg.address = franchisee.F_Address;
                Reg.ownerName = franchisee.OwnerName;
                Reg.Branch = franchisee.BranchName;
                Reg.GSTNo = franchisee.GstNo;
                Reg.franchiseName = franchisee.Franchisee_Name;
                Reg.mobileNo = franchisee.ContactNo;
                Reg.dateTime = franchisee.Datetime_Fr;
                Reg.Pan_No = franchisee.Pan_No;
                Reg.emailId = franchisee.Sendermail;
                Reg.password = franchisee.password;
                Reg.AccountName = franchisee.AccountName;
                Reg.BankName = franchisee.Bankname;
                Reg.AccountName = franchisee.AccountName;
                Reg.AccountName = franchisee.AccountName;
                Reg.IFSC_Code = franchisee.IFSCcode;
                Reg.Branch = franchisee.Branch;
                Reg.AccountType = franchisee.Accounttype;
              //  Reg.InvoiceStart = franchisee.InvoiceStart.ToString();


                db.Entry(Reg).State = EntityState.Modified;
                db.SaveChanges();
                    if (franchisee.IsGECSector == true)
                    {
                        var gecrate = db.Sectors.Where(m => m.Pf_code == PF_Code && m.BillGecSec==true).ToList();

                        if (gecrate.Count == 0)
                        {
                            string[] sectornamelist = new string[]
                                   {    
                            "CENTRAL I",
                            "CENTRAL II",
                            "EAST I",
                            "EAST II",
                            "NORTH EAST I",
                            "NORTH EAST II",
                            "NORTH EAST III",
                            "NORTH I",
                            "NORTH II",
                            "NORTH III",
                            "SOUTH I",
                            "SOUTH II",
                            "SOUTH III",
                            "WEST I",
                            "WEST II"
                                   };
                            var sector = db.Sectors.Where(m => m.Pf_code ==PF_Code && m.BillGecSec == true).ToList();
                            if (sector.Count == 0)
                            {
                                var p = 1;
                                foreach (var i in sectornamelist)
                                {
                                    Sector sn = new Sector();

                                    sn.Pf_code =PF_Code;
                                    sn.CashD = null;
                                    sn.CashN = null;
                                    sn.BillD = null;
                                    sn.BillN = null;
                                    sn.BillNonAir = null;
                                    sn.BillNonSur = null;
                                    sn.BillEcomGE = null;
                                    sn.BillEcomPrio = null;
                                    sn.Sector_Name = i;
                                    sn.Priority = p;
                                    sn.BillGecSec = true;

                                    db.Sectors.Add(sn);
                                    db.SaveChanges();
                                    p++;

                                }
                            }

                        }



                    }

                }
                catch(Exception ex) {
                
                
                
                }
                TempData["Success"] = "franchisee Updated  Successfully!";
                return PartialView("Edit",franchisee);
               //RedirectToAction("")
            }
            return PartialView("Edit",franchisee);
        }

        public ActionResult AddLogo()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult UploadFile()
        {
            string Pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var myFile = Request.Files[i];
                if (myFile == null)
                {
                    return Json(new {  FailureReason= true });
                }
                string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
                           Request.ApplicationPath.TrimEnd('/');
                if (myFile != null && myFile.ContentLength != 0)
                {
                    var file = Request.Files[0];
                    if (file != null)
                    {
                        var fileName = System.IO.Path.GetFileName(file.FileName);
                        var newFileName = Pfcode;

                        var path = System.IO.Path.Combine(Server.MapPath("~/Stamps"), fileName);
                        // Get the file extension
                        string fileExtension = System.IO.Path.GetExtension(path);
                        string newFilePath = System.IO.Path.Combine(Server.MapPath("~/Stamps"), newFileName + "" + fileExtension); ;
                        // Rename the file
                        //if (System.IO.File.Exists(newFilePath))
                        //{
                        //    System.IO.File.Delete(newFilePath);

                        //}

                        string folderPath = System.IO.Path.Combine(Server.MapPath("~/Stamps")); // Specify the folder path here
                        string imageName = Pfcode; // Specify the image name here

                        // Check if the folder exists
                        if (Directory.Exists(folderPath))
                        {
                            // Get all files in the folder
                            string[] allFiles = Directory.GetFiles(folderPath);

                            // Filter files with the same name
                            var filesWithSameName = allFiles.Where(file123 =>
                                System.IO.Path.GetFileNameWithoutExtension(file123) == imageName);

                            // Delete each file with the same name
                            foreach (string file1 in filesWithSameName)
                            {
                                System.IO.File.Delete(file1);
                                Console.WriteLine($"Deleted: {file1}");
                            }

                            Console.WriteLine($"All files with the name '{imageName}' deleted successfully.");
                        }


                        file.SaveAs(path);


                        string originalFilePath = path;

                        
                       
                        System.IO.File.Move(originalFilePath, newFilePath);


                        // Save file path in database
                        try
                        {

                            var franchises = db.Franchisees.Where(x => x.PF_Code == Pfcode).FirstOrDefault();
                            franchises.StampFilePath = baseurl+"/Stamps/" + newFileName + "" + fileExtension;//create dynamic
                           // franchises.StampFilePath=newFilePath;
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
        TempData["Success"] = "Updated Successfully";
            return Json(new { success = true});
        }

        [HttpPost]
        public ActionResult AddLogo(AddlogoModel logo)
        {

            if(logo.file == null)
            {
                TempData["Error"] = " Image file Not Uploaded!";
                return RedirectToAction("Franchiseelist");
            }
            // Get the file extension in lowercase
            string extension = System.IO.Path.GetExtension(logo.file.FileName)?.ToLower();

            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
            {
                // ModelState.AddModelError("fileerr", "Only Image files allowed.");
                TempData["Error"] = "Only Image files allowed!";
                return RedirectToAction("Franchiseelist");

            }
            else
            {
                string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();
                string _FileName = "";
                string _path = "";
                if (logo.file.ContentLength > 0)
                {
                    _FileName = System.IO.Path.GetFileName(logo.file.FileName);
                    _path = Server.MapPath("~/UploadedLogo/" + _FileName);

                    logo.file.SaveAs(_path);
                }

                var lo = (from d in db.Franchisees
                          where d.PF_Code == strpf
                          select d).FirstOrDefault();
                var LogoFilePath = "https://frbilling.com/UploadedLogo/" + _FileName;


                // lo.LogoFilePath =_path;
                lo.LogoFilePath = LogoFilePath;

                db.Entry(lo).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success1"] = "Logo Added Successfully!";
                return RedirectToAction("Franchiseelist");
            }

           // return View("AddLogo");
           // return RedirectToAction("Franchiseelist");
            //return PartialView(logo);

        }
        public ActionResult UploadQrCode()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddQrCode(AddQrCodeModel qrcode)
        {
            if (qrcode.file== null)
            {
                // ModelState.AddModelError("fileerr", "Only Image files allowed.");
                TempData["Error"] = " Image file Not Upload!";
                return RedirectToAction("Franchiseelist");

            }
            // Get the file extension in lowercase
            string extension = System.IO.Path.GetExtension(qrcode.file.FileName)?.ToLower();
            string baseUrl = Request.Url.Authority + "://";
            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
            {
                // ModelState.AddModelError("fileerr", "Only Image files allowed.");
                TempData["Error"] = "Only Image files allowed!";
                return RedirectToAction("Franchiseelist");
            }
            else
            {
                string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();
                string _FileName = "";
                string _path = "";
                if (qrcode.file.ContentLength > 0)
                {
                    _FileName = strpf+ System.IO.Path.GetExtension(qrcode.file.FileName);
                    string qrpath = Server.MapPath("~/UploadedQrCode/");
                    if (!Directory.Exists(qrpath))
                    {
                        Directory.CreateDirectory(qrpath);
                    }
                    _path = Server.MapPath("~/UploadedQrCode/" + _FileName);
                  
                    qrcode.file.SaveAs(_path);
                }

                var lo = (from d in db.Franchisees
                          where d.PF_Code == strpf
                          select d).FirstOrDefault();
                var QrFilePath = "https://frbilling.com/" +"UploadedQrCode/" + _FileName;


                // lo.LogoFilePath =_path;
                lo.QrCodeImage = QrFilePath;

                db.Entry(lo).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success1"] = "QrCode Added Successfully!";
                return RedirectToAction("Franchiseelist");
            }

         //   return View("UploadQrCode");
            // return RedirectToAction("Franchiseelist");
            //return PartialView(logo);

        }


        public ActionResult ImportCsv()
        {
            return View();
        }

        public ActionResult FranchiseeList(string tab = "1")
        {
            //long stradmin = Convert.ToInt64(Session["Admin"]);
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.pfcode = strpf;
            ViewBag.activateTab = tab;
            if (strpf == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var data = (from d in db.Franchisees
                        where d.PF_Code == strpf
                        select d).FirstOrDefault();

            ViewBag.logoImage = data.LogoFilePath;

            ViewBag.stampImage = data.StampFilePath;
            ViewBag.QrCodeImage= data.QrCodeImage;
            FranchiseeModel Fr = new FranchiseeModel();

            Fr.PF_Code = data.PF_Code;
            Fr.F_Address = data.F_Address;
            Fr.OwnerName = data.OwnerName;
            Fr.BranchName = data.BranchName;
            Fr.GstNo = data.GstNo;
            Fr.Franchisee_Name = data.Franchisee_Name;
            Fr.ContactNo = data.ContactNo;
            Fr.Branch_Area = data.Branch_Area;
            Fr.Datetime_Fr = data.Datetime_Fr;
            Fr.Pan_No = data.Pan_No;
            Fr.Sendermail = data.Sendermail;
            Fr.password = data.password;
            Fr.AccountName = data.AccountName;
            Fr.Bankname = data.Bankname;
            Fr.Accountno = data.Accountno;
            Fr.IFSCcode = data.IFSCcode;
            Fr.Branch = data.Branch;
            Fr.Accounttype = data.Accounttype;  
            Fr.InvoiceStart = data.InvoiceStart;
            Fr.StampFilePath = data.StampFilePath;
            Fr.IsGECSector = data.IsGECSector??false;

            if (Fr == null)
            {
                return HttpNotFound();
            }
            ViewBag.Company = Fr;



            List<SectorNewModel> st = (from u in db.Sectors
                                       where u.Pf_code == strpf
                                       && u.BillGecSec==null
                                       select new SectorNewModel
                                       {
                                           Sector_Id = u.Sector_Id,
                                           Sector_Name = u.Sector_Name,
                                           Pf_code = u.Pf_code,
                                           Pincode_values = u.Pincode_values,
                                           Priority = u.Priority,
                                           BillD = u.BillD ?? false,
                                           BillNonAir = u.BillNonAir ?? false,
                                           BillNonSur = u.BillNonSur ?? false,
                                           BillExpCargo = u.BillExpCargo ?? false,
                                           BillPriority = u.BillPriority ?? false,
                                           BillEcomPrio = u.BillEcomPrio ?? false,
                                           BillEcomGE = u.BillEcomGE ?? false

                                       }).OrderBy(x => x.Priority).ToList();
            ViewBag.pfcode = strpf;//stored in hidden format on the view
            ViewBag.DataSector = st;

            ViewBag.Sectors = st;

            if (data.LogoFilePath != null)
            {
                ViewBag.logoimg = data.LogoFilePath;
            }

            return View();
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


        public ActionResult DeleteCompapy(string companyid)
        {
            var id = companyid.Replace("__", "&").Replace("xdotx",".");
            List<Dtdc_Ptp> dtdc_Ptps = db.Dtdc_Ptp.Where(m => m.Company_id == id).ToList();
            List<dtdcPlu> dtdcPlu = db.dtdcPlus.Where(m => m.Company_id == id).ToList();
            List<express_cargo> express_cargo = db.express_cargo.Where(m => m.Company_id == id).ToList();
            List<Nondox> Nondox = db.Nondoxes.Where(m => m.Company_id == id).ToList();
            List<Ratem> Ratem = db.Ratems.Where(m => m.Company_id == id).ToList();
            List<Priority> pra = db.Priorities.Where(m => m.Company_id == id).ToList();
            Company comp = db.Companies.Where(m => m.Company_Id == id).FirstOrDefault();
            List<Dtdc_Ecommerce> dtdc_Ecom = db.Dtdc_Ecommerce.Where(m => m.Company_id == id).ToList();
            List<GECrate> getGec = db.GECrates.Where(m => m.Company_id == id).ToList();
            List<Transaction> dtdc_tran=db.Transactions.Where(m=>m.Customer_Id.ToUpper()==id.ToUpper()).ToList();
            List<Entity_FR.Invoice> invoice = db.Invoices.Where(m => m.Customer_Id == id).ToList();
            if (dtdc_Ptps.Count>0)
            {
                foreach (var i in dtdc_Ptps)
                {
                    db.Dtdc_Ptp.Remove(i);
                }
            }
          if(dtdcPlu.Count>0)
            {
                foreach (var i in dtdcPlu)
                {
                    db.dtdcPlus.Remove(i);
                }
            }
            if (express_cargo.Count > 0)
            {
                foreach (var i in express_cargo)
                {
                    db.express_cargo.Remove(i);
                }
            }
            if (Nondox.Count > 0)
            {
                foreach (var i in Nondox)
                {
                    db.Nondoxes.Remove(i);
                }
            }
            if (Ratem.Count>0)
            {
                foreach (var i in Ratem)
                {
                    db.Ratems.Remove(i);
                }
            }
            foreach (var i in pra)
            {
                db.Priorities.Remove(i);
            }
           if(dtdc_Ecom.Count > 0)
            {
                foreach (var i in dtdc_Ecom)
                {
                    db.Dtdc_Ecommerce.Remove(i);
                }
            }
            if (getGec.Count > 0)
            {
                foreach (var i in getGec)
                {
                    db.GECrates.Remove(i);
                }
            }
           if(dtdc_tran.Count > 0)
            {
               foreach(var i in dtdc_tran)
                {
                    db.Transactions.Remove(i);  
                }
            }

            if (invoice.Count > 0)
            {
                foreach(var i in invoice)
                {
                    db.Invoices.Remove(i);
                }
            }
            if (comp != null)
            {
                db.Companies.Remove(comp);
            }
           

            db.SaveChanges();
            TempData["Success"] = "Company Deleted SuccessFully";
            return RedirectToAction("EditCompanyRateMaster", "RateMaster");
        }
        [HttpGet]
        public ActionResult CookiesExpires()
        {
            return View();  
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
            var Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var validcons = db.Transactions.Where(p => p.Consignment_no == Consignment_no && p.Pf_Code==Pf_Code).FirstOrDefault();
            if (validcons != null)
            {
                Transaction tran = db.Transactions.Where(m => m.Consignment_no == Consignment_no).FirstOrDefault();
                tran.isDelete= true;
                db.Entry(tran).State=EntityState.Modified;
              //  db.Transactions.Remove(tran);
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
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "Pf_Code", "Pf_Code");
            ViewBag.Pf_Code=pfcode;

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

                obj = db.Expenses.Where(m => DbFunctions.TruncateTime(m.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Exp) <= DbFunctions.TruncateTime(todate) && m.Pf_Code==pfcode).ToList();
            }
            else
            {
                obj = db.Expenses.Where(x=>x.Pf_Code==pfcode).ToList();
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult ExpensesList(string Pf_Code, string Category, string ToDatetime, string Fromdatetime, string Submit)
        {
            ViewBag.Fromdatetime = Fromdatetime;
            ViewBag.ToDatetime = ToDatetime;
            Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();
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
            else if ((Pf_Code == null && Pf_Code == "") || (Category == null && Category == "") || (ToDatetime != "" && Fromdatetime != null))
            {
                list = db.Expenses.Where(m => DbFunctions.TruncateTime(m.Datetime_Exp) >= DbFunctions.TruncateTime(fromdate) && DbFunctions.TruncateTime(m.Datetime_Exp) <= DbFunctions.TruncateTime(todate)).ToList();
            }
            else
            {
                list = db.Expenses.ToList();
            }


            if (Submit == "Export to Excel")
            {
                if (list.Count() > 0)
                {
                    ExportToExcelAll.ExportToExcelAdmin(list.Select(x => new { Amount = x.Amount, Reason = x.Rason, x.Category, DateTime = x.Datetime_Exp != null ? x.Datetime_Exp.Value.ToString("dd/MM/yyyy") : "" }));

                }
            }
            return View(list);
        }


        public ActionResult EditExpenses(long? id)
        {

            ViewBag.Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "Pf_Code", "Pf_Code");


            List<SelectListItem> expe = new List<SelectListItem>();


            expe.Add(new SelectListItem { Text = "Select", Value = "" });
            expe.Add(new SelectListItem { Text = "Load Connecting exp 1st and 2nd", Value = "Load Connecting exp 1st and 2nd" });
            expe.Add(new SelectListItem { Text = "Load connecting exp - Night load", Value = "Load connecting exp - Night load" });
            expe.Add(new SelectListItem { Text = "Pick up expenses", Value = "Pick up expenses" });
            expe.Add(new SelectListItem { Text = "Patpedhi Deposit", Value = "Patpedhi Deposit" });
            expe.Add(new SelectListItem { Text = "Salary Advance", Value = "Salary Advance" });
            expe.Add(new SelectListItem { Text = "Office Expenses", Value = "Office Expenses" });
            expe.Add(new SelectListItem { Text = "Fuel Exp", Value = "Fuel Exp" });
            expe.Add(new SelectListItem { Text = "Tea and refreshments exp", Value = "Tea and refreshments exp" });
            expe.Add(new SelectListItem { Text = "Packing Expenses", Value = "Packing Expenses" });
            expe.Add(new SelectListItem { Text = "Others", Value = "Others" });

            var data = (from d in db.Expenses
                        where d.Exp_ID == id
                        select new { d.Category }).First();


            foreach (var item in expe)
            {

                if (data.Category == item.Value)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }

            }

            ViewData["Category"] = expe;

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





        [SessionAdminold]
        //Not Giving access to User
       // [SessionUserModule]
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


            if (Request.Cookies["Cookies"] != null)
            {
                var c = new HttpCookie("Cookies");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);

                HttpCookie referalCodeCookie = new HttpCookie("referalCode");
                referalCodeCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(referalCodeCookie);

                HttpCookie Adminvalue = new HttpCookie("AdminValue");
                Adminvalue.Expires = DateTime.Now.AddDays(-1);  
                Response.Cookies.Add(Adminvalue);

                HttpCookie UserValue = new HttpCookie("UserValue");
                UserValue.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(UserValue);    

                HttpCookie admin=new HttpCookie("Admin");
                admin.Expires= DateTime.Now.AddDays(-1);    
                Response.Cookies.Add(admin);


                HttpCookie username =new  HttpCookie("UserName");
                username.Expires = DateTime.Now.AddDays(-1);    
                Response.Cookies.Add(username);

                HttpCookie pfcode = new HttpCookie("pfCode");
                pfcode.Expires= DateTime.Now.AddDays(-1);   
                Response.Cookies.Add(pfcode);
            }

            Session.Clear();
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
                               //return RedirectToAction("Adminlogin", "Admin");
                               //string SubPath = "http://codetentacles-005-site1.htempurl.com/";
                               // return Redirect(SubPath);
            return RedirectToAction("Adminlogin", "Admin");


        }

        [HttpGet]
        public JsonResult userNameCheck(string username)
        {
            bool isValid = db.registrations.Where(x => x.userName == username).FirstOrDefault() != null ? false : true;

            return Json(isValid, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Register(string referral = "")
        {
            string testParameter = Request.QueryString["referral"];

            if (referral != "")
            {
                var isValidReferral = db.registrations.Where(x => x.referralCode == referral && x.isPaid == true).FirstOrDefault() != null ? true : false;

                if (isValidReferral)
                {
                    TempData["referralCode"] = referral;
                }
                else
                {
                    referral = "";
                    ModelState.AddModelError("Error", "Invalid referral or not paid yet");
                }

            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Register(RegistrationModel userDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var flag = false;
                    var saveSector = false;
                    var username= db.registrations.Where(x => x.userName.ToUpper() == userDetails.userName.ToUpper()).FirstOrDefault();
                    var Pfcheck = db.registrations.Where(x => x.Pfcode == userDetails.Pfcode).FirstOrDefault();
                    if (username!=null)
                    {
                        flag = true;
                        ModelState.AddModelError("usernameerror", "User name already exist");
                    }

                    if (Pfcheck != null)
                    {
                        flag = true;
                        ModelState.AddModelError("customError", "Pfcode already exist");
                     
                    }

                    if (userDetails.isUserNameExist == false)
                    {
                        flag = true;
                        ModelState.AddModelError("usernameerror", "User name already exist");
                    }


                    if (userDetails.referral != "" && userDetails.referral != null)
                    {
                        var isValidReferral = db.registrations.Where(x => x.referralCode == userDetails.referral && x.isPaid == true).FirstOrDefault() != null ? true : false;

                        if (isValidReferral)
                        {
                            userDetails.referral = userDetails.referral;
                        }
                        else
                        {
                            flag = true;
                            userDetails.referral = "";
                            ModelState.AddModelError("Error", "Invalid referral or not paid yet");
                        }

                    }


                    if (flag)
                    {
                        return View("Register", userDetails);
                    }
                    if (Pfcheck== null)
                    {
                        registration register = new registration();
                   

                       register.Pfcode = userDetails.Pfcode.ToString().ToUpper();
                        register.franchiseName=userDetails.franchiseName; 
                       register.emailId = userDetails.emailId;
                        register.dateTime = DateTime.Now;
                        register.ownerName= userDetails.ownerName;
                        register.userName= userDetails.userName.Trim();
                    register.password  = userDetails.password.Trim();
                    register.mobileNo= userDetails.mobileNo;
                    register.address= userDetails.address;
                    register.referralCode =RandomString(10);
                    register.referralby = userDetails.referral;
                    register.isEmailConfirmed = false;
                    register.emailOTP = "";
                        register.isPaid=false;
                        register.isEmailConfirmed = false;  
                        register.isLoginConfirmed = false;
                        db.registrations.Add(register);
                        db.SaveChanges();
                        //Currenlty Return from Here Because of Mail Sending Problem
                        //return PartialView("~/Views/Shared/AfterRegisterMessage");
                        return PartialView("AfterRegisterMessage");


                    }

                    //var save = db.registrationSave(userDetails.Pfcode.ToUpper(), userDetails.franchiseName, userDetails.emailId, DateTime.Now, userDetails.ownerName, userDetails.userName, userDetails.password, false, userDetails.mobileNo, userDetails.address, RandomString(10), userDetails.referral,userDetails.isEmailConfirmed=false);


                    //if (saveSector)
                    //{
                    //    var savefranchisee = db.FranchiseeSave(userDetails.Pfcode.ToUpper(), userDetails.franchiseName, userDetails.emailId, DateTime.Now, userDetails.ownerName, userDetails.password, userDetails.mobileNo, userDetails.address);

                    //    //Adding Eantries To the Sector Table
                    //    var sectornamelist = db.sectorNames.ToList();

                    //    var pfcode = (from u in db.Franchisees
                    //                  where u.PF_Code == userDetails.Pfcode
                    //                  select u).FirstOrDefault();
                    //Adding Sector Name List wih pincodes
                    //if (pfcode != null)
                    //{
                    //    foreach (var i in sectornamelist)
                    //    {
                    //        Sector sn = new Sector();

                    //        sn.Pf_code = pfcode.PF_Code;
                    //        sn.Sector_Name = i.sname;



                    //        sn.CashD = true;
                    //        sn.CashN = true;
                    //        sn.BillD = true;
                    //        sn.BillN = true;


                    //        if (sn.Sector_Name == "Within city")
                    //        {
                    //            sn.Priority = 1;
                    //            sn.Pincode_values = "400001-400610,400615-400706,400710-401203,401205-402209";

                    //            sn.CashD = true;
                    //            sn.CashN = true;
                    //            sn.BillD = true;
                    //            sn.BillN = true;

                    //        }

                    //        else if (sn.Sector_Name == "Within State")
                    //        {

                    //            sn.CashD = true;
                    //            sn.CashN = false;
                    //            sn.BillD = true;
                    //            sn.BillN = false;

                    //            sn.Priority = 2;
                    //            sn.Pincode_values = "400000-403000,404000-450000";
                    //        }


                    //        else if (sn.Sector_Name == "North East")
                    //        {
                    //            sn.Priority = 3;
                    //            sn.Pincode_values = "400000-450000,360000-400000,450000-490000";

                    //            sn.CashD = false;
                    //            sn.CashN = true;
                    //            sn.BillD = false;
                    //            sn.BillN = true;

                    //        }

                    //        else if (sn.Sector_Name == "Metro")
                    //        {
                    //            sn.Priority = 4;
                    //            sn.Pincode_values = "180000-200000";

                    //            sn.CashD = true;
                    //            sn.CashN = true;
                    //            sn.BillD = true;
                    //            sn.BillN = true;

                    //        }



                    //        else if (sn.Sector_Name == "Jammu and Kashmir")
                    //        {
                    //            sn.Priority = 5;
                    //            sn.Pincode_values = "780000-800000,170000-180000";

                    //            sn.CashD = true;
                    //            sn.CashN = true;
                    //            sn.BillD = true;
                    //            sn.BillN = true;

                    //        }



                    //        else if (sn.Sector_Name == "Rest of India")
                    //        {
                    //            sn.Priority = 6;
                    //            sn.Pincode_values = "000000";

                    //            sn.CashD = true;
                    //            sn.CashN = true;
                    //            sn.BillD = true;
                    //            sn.BillN = true;

                    //        }
                    //        else
                    //        {
                    //            sn.Pincode_values = null;
                    //        }




                    //        db.Sectors.Add(sn);

                    //        db.SaveChanges();

                    //    }
                    //}

                    //Adding Sector Name List
                    //////////////////////////////////////////////
                    //Adding Company
                    //var Companyid = "Cash_" + userDetails.Pfcode;


                    //var secotrs = db.Sectors.Where(m => m.Pf_code == userDetails.Pfcode).ToList();

                    //Company cm = new Company();
                    //cm.Company_Id = Companyid;
                    //cm.Pf_code = userDetails.Pfcode;
                    //cm.Phone = 1234567890;
                    //cm.Company_Address = userDetails.address;
                    //cm.Company_Name = Companyid;
                    //cm.Email = Companyid + "@gmail.com";
                    //db.Companies.Add(cm);
                    //db.SaveChanges();



                    //var basiccompid = "BASIC_TS";

                    //var basicrec = db.Ratems.Where(m => m.Company_id == "BASIC_TS").FirstOrDefault();



                    //if (basicrec == null)
                    //{
                    //    Company bs = new Company();
                    //    bs.Company_Id = basiccompid;
                    //    bs.Pf_code = null;
                    //    bs.Phone = 1234567890;
                    //    bs.Company_Address = userDetails.address;
                    //    bs.Company_Name = "BASIC_TS";
                    //    bs.Email = "Email@gmail.com";
                    //    db.Companies.Add(bs);
                    //    db.SaveChanges();

                    //    int j = 0;

                    //    foreach (var i in secotrs)
                    //    {
                    //        Ratem dox = new Ratem();
                    //        Nondox ndox = new Nondox();
                    //        express_cargo cs = new express_cargo();

                    //        dox.Company_id = basiccompid;
                    //        dox.Sector_Id = i.Sector_Id;
                    //        dox.NoOfSlab = 2;

                    //        dox.slab1 = 1;
                    //        dox.slab2 = 1;
                    //        dox.slab3 = 1;
                    //        dox.slab4 = 1;

                    //        dox.Uptosl1 = 1;
                    //        dox.Uptosl2 = 1;
                    //        dox.Uptosl3 = 1;
                    //        dox.Uptosl4 = 1;

                    //        ndox.Company_id = basiccompid;
                    //        ndox.Sector_Id = i.Sector_Id;
                    //        ndox.NoOfSlabN = 2;
                    //        ndox.NoOfSlabS = 2;

                    //        ndox.Aslab1 = 1;
                    //        ndox.Aslab2 = 1;
                    //        ndox.Aslab3 = 1;
                    //        ndox.Aslab4 = 1;


                    //        ndox.Sslab1 = 1;
                    //        ndox.Sslab2 = 1;
                    //        ndox.Sslab3 = 1;
                    //        ndox.Sslab4 = 1;

                    //        ndox.AUptosl1 = 1;
                    //        ndox.AUptosl2 = 1;
                    //        ndox.AUptosl3 = 1;
                    //        ndox.AUptosl4 = 1;

                    //        ndox.SUptosl1 = 1;
                    //        ndox.SUptosl2 = 1;
                    //        ndox.SUptosl3 = 1;
                    //        ndox.SUptosl4 = 1;


                    //        cs.Company_id = basiccompid;
                    //        cs.Sector_Id = i.Sector_Id;

                    //        cs.Exslab1 = 1;
                    //        cs.Exslab2 = 1;

                    //        db.Ratems.Add(dox);
                    //        db.Nondoxes.Add(ndox);
                    //        db.express_cargo.Add(cs);

                    //        j++;

                    //    }

                    //    int p = 0;

                    //    for (int i = 0; i < 5; i++)
                    //    {

                    //        dtdcPlu dtplu = new dtdcPlu();
                    //        Dtdc_Ptp stptp = new Dtdc_Ptp();

                    //        if (i == 0)
                    //        {
                    //            dtplu.destination = "City Plus";
                    //            stptp.dest = "City";
                    //        }
                    //        else if (i == 1)
                    //        {
                    //            dtplu.destination = "Zonal Plus/Blue";
                    //            stptp.dest = "Zonal";

                    //        }
                    //        else if (i == 2)
                    //        {
                    //            dtplu.destination = "Metro Plus/Blue";
                    //            stptp.dest = "Metro";
                    //        }
                    //        else if (i == 3)
                    //        {
                    //            dtplu.destination = "National Plus/Blue";
                    //            stptp.dest = "National";
                    //        }
                    //        else if (i == 4)
                    //        {
                    //            dtplu.destination = "Regional Plus";
                    //            stptp.dest = "Regional";
                    //        }

                    //        dtplu.Company_id = basiccompid;

                    //        dtplu.Upto500gm = 1;
                    //        dtplu.U10to25kg = 1;
                    //        dtplu.U25to50 = 1;
                    //        dtplu.U50to100 = 1;
                    //        dtplu.add100kg = 1;
                    //        dtplu.Add500gm = 1;


                    //        stptp.Company_id = basiccompid;
                    //        stptp.PUpto500gm = 1;
                    //        stptp.PAdd500gm = 1;
                    //        stptp.PU10to25kg = 1;
                    //        stptp.PU25to50 = 1;
                    //        stptp.Padd100kg = 1;
                    //        stptp.PU50to100 = 1;

                    //        stptp.P2Upto500gm = 1;
                    //        stptp.P2Add500gm = 1;
                    //        stptp.P2U10to25kg = 1;
                    //        stptp.P2U25to50 = 1;
                    //        stptp.P2add100kg = 1;
                    //        stptp.P2U50to100 = 1;

                    //        db.dtdcPlus.Add(dtplu);
                    //        db.Dtdc_Ptp.Add(stptp);

                    //        p++;

                    //    }

                    //}




                    //foreach (var i in secotrs)
                    //{
                    //    Ratem dox = new Ratem();
                    //    Nondox ndox = new Nondox();
                    //    express_cargo cs = new express_cargo();

                    //    dox.Company_id = Companyid;
                    //    dox.Sector_Id = i.Sector_Id;
                    //    dox.NoOfSlab = 2;
                    //    //dox.CashCounter = true;

                    //    ndox.Company_id = Companyid;
                    //    ndox.Sector_Id = i.Sector_Id;
                    //    ndox.NoOfSlabN = 2;
                    //    ndox.NoOfSlabS = 2;
                    //    // ndox.CashCounterNon = true;


                    //    cs.Company_id = Companyid;
                    //    cs.Sector_Id = i.Sector_Id;

                    //    // cs.CashCounterExpr = true;

                    //    db.Ratems.Add(dox);
                    //    db.Nondoxes.Add(ndox);
                    //    db.express_cargo.Add(cs);


                    //}

                    //for (int i = 0; i < 5; i++)
                    //{
                    //    dtdcPlu dtplu = new dtdcPlu();
                    //    Dtdc_Ptp stptp = new Dtdc_Ptp();

                    //    if (i == 0)
                    //    {
                    //        dtplu.destination = "City Plus";
                    //        stptp.dest = "City";
                    //    }
                    //    else if (i == 1)
                    //    {
                    //        dtplu.destination = "Zonal Plus/Blue";
                    //        stptp.dest = "Zonal";

                    //    }
                    //    else if (i == 2)
                    //    {
                    //        dtplu.destination = "Metro Plus/Blue";
                    //        stptp.dest = "Metro";
                    //    }
                    //    else if (i == 3)
                    //    {
                    //        dtplu.destination = "National Plus/Blue";
                    //        stptp.dest = "National";
                    //    }
                    //    else if (i == 4)
                    //    {
                    //        dtplu.destination = "Regional Plus";
                    //        stptp.dest = "Regional";
                    //    }

                    //    dtplu.Company_id = Companyid;
                    //    // dtplu.CashCounterPlus = true;
                    //    stptp.Company_id = Companyid;


                    //    db.dtdcPlus.Add(dtplu);
                    //    db.Dtdc_Ptp.Add(stptp);

                    //}

                    //db.SaveChanges();

                    //userDetailsModel user = new userDetailsModel();

                    //user.name = userDetails.franchiseName;
                    //user.email = userDetails.emailId;
                    //user.mobileNo = userDetails.mobileNo;
                    //user.address = userDetails.address;
                    ////Because of set the Cokkies the code 
                    ////Session["DataName"] = user.name;
                    ////Session["Dataemail"] = user.email;
                    ////Session["Datacontact"] = user.mobileNo;
                    ////Session["Dataaddress"] = user.address;

                    //if (Pfcheck.emailId!=null)
                    //{
                    //    string FilePath = Server.MapPath("~/images/RegisterMailTemplete.html");
                    //    //"http://codetentacles-005-site1.htempurl.com/images/RegisterMailTemplete.html";// "D:\\MBK\\SendEmailByEmailTemplate\\EmailTemplates\\SignUp.html";
                    //    StreamReader str = new StreamReader(FilePath);
                    //    string MailText = str.ReadToEnd();
                    //    str.Close();

                    //    MailText = MailText.Replace("[newusername]", userDetails.franchiseName);

                    //    string subject = "Welcome To FrBilling Subscription";


                    //    MailMessage _mailmsg = new MailMessage();


                    //    _mailmsg.IsBodyHtml = true;
                    //    _mailmsg.From = new MailAddress("frbillingsoftware@gmail.com");
                    //    _mailmsg.To.Add(userDetails.emailId);
                    //    _mailmsg.Subject = subject;
                    //    _mailmsg.Body = MailText;

                    //    SmtpClient _smtp = new SmtpClient();

                    //    _smtp.Host = "smtp.gmail.com";

                    //    _smtp.Port = 587;


                    //    _smtp.EnableSsl = true;
                    //    _smtp.UseDefaultCredentials = false;

                    //    NetworkCredential _network = new NetworkCredential("frbillingsoftware@gmail.com", "rqaynjbevkygswkx");
                    //    _smtp.Credentials = _network;


                    //    //_smtp.Send(_mailmsg);

                    //    TempData["success"] = "Your registration has been successfully completed! To activate your account, please contact us at +91 9209764995.";
                    //    //return RedirectToAction("makePaymentPartial", "Admin", user);
                    //    return RedirectToAction("Register");
                    //    // var userAllDetails = db.registrations.Where(x => x.Pfcode == userDetails.Pfcode).FirstOrDefault();
                    //    // return Json(userAllDetails, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    TempData["error"] = "Something went wrong Please try Again!!";
                    //}
                    //  }
                    TempData["succ"] = "1";
                    //These is Temporaly Commet 
                    // return RedirectToAction("VerifyEmail", "Admin", new {pfcode=userDetails.Pfcode });

                    //  return RedirectToAction("VerifyEmail", "Admin", new { pfcode = userDetails.Pfcode });

                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                                           .Where(y => y.Count > 0)
                                           .ToList();
                }
            }
            catch(Exception e)
            {
                return View("Error");   
            }

          
            return View();


        }
        public ActionResult VerifyLogin(string pfcode)
        {
            var register = db.registrations.Where(x => x.Pfcode == pfcode).FirstOrDefault();
          
            var loginmodel = new LoginVerification
            {
                mobileNo = register.mobileNo,
                PF_Code = register.Pfcode,
                LoginOTP = ""
            };
            var otp = RandomMobileOTP(6); // Implement this method to generate OTP
                                          //  SendOtpEmail(email, otp); // Implement this method to send OTP via email
            TempData["otpSentMessage"] = "OTP has been sent to your mobile. Please check it.";
            if (register != null)
            {
                register.LoginOTP = otp;
                register.dateTime = DateTime.Now;
                register.isLoginConfirmed = false;
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();
                SendLoginOTPtoMobile(register.mobileNo, otp);
            }
           

            return View(loginmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyLogin(LoginVerification loginVerification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (loginVerification != null)
                    {
                        var register = db.registrations.Where(x => x.Pfcode == loginVerification.PF_Code).FirstOrDefault();
                        if (loginVerification.LoginOTP == register.LoginOTP)
                        {
                            DateTime registerTime = register.dateTime.Value;
                            DateTime currentTime = DateTime.Now;
                            TimeSpan timeDifference = currentTime - registerTime;
                            if (timeDifference.TotalMinutes <= 4)
                            {
                                register.mobileNo = loginVerification.mobileNo;
                                register.isLoginConfirmed = true;
                                register.password = register.password;
                                db.Entry(register).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["otpverify"] = "Login OTP Verify Successfully!!";
                                SetCookies(register.Pfcode);
                                return RedirectToAction("Index","Home");
                            }
                            else
                            {
                                TempData["timeexpired"] = "Your Time Expired Try Again";

                            }
                        }
                        else
                        {
                            TempData["invalidOTP"] = "Please Enter Valid OTP";


                        }


                        return View();

                    }
                    return View(loginVerification);
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Something Went Wrong! Try Again!!";
                return View();
            }
        }
        public ActionResult VerifyEmail(string pfcode)
        {
 

            var register=db.registrations.Where(x=>x.Pfcode==pfcode).FirstOrDefault();

            var emailModal = new EmailVerification
            {
                Email = register.emailId,
                PF_Code = register.Pfcode,
                EmailOTP = ""
            };
            return View(emailModal);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult VerifyEmail(EmailVerification emailVerification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (emailVerification != null)
                    {
                        var register = db.registrations.Where(x => x.Pfcode == emailVerification.PF_Code).FirstOrDefault();
                        if (emailVerification.EmailOTP == register.emailOTP)
                        {
                            DateTime registerTime=register.dateTime.Value;
                            DateTime currentTime = DateTime.Now;
                            TimeSpan timeDifference = currentTime - registerTime;
                            if (timeDifference.TotalMinutes <=3)
                            {
                                register.emailId = emailVerification.Email;
                                register.isEmailConfirmed = true;
                                db.Entry(register).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["otpverify"] = "Email Verify Successfully!!";
                                
                            }
                            else
                            {
                                TempData["timeexpired"] = "Your Time Expired Try Again";
                               
                            }
                        }
                        else
                        {
                            TempData["invalidOTP"] = "Please Enter Valid OTP";
                            

                        }


                        return View();

                    }
                    return View(emailVerification);
                }

                return View();
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Something Went Wrong! Try Again!!";
                return View();
            }
        }
        [HttpPost]
        public ActionResult SendLoginOTPtoM(string mobileNo,string pfcode)
        {
            var otp = RandomMobileOTP(6); // Implement this method to generate OTP
                                       //  SendOtpEmail(email, otp); // Implement this method to send OTP via email
            var register = db.registrations.Where(x => x.Pfcode == pfcode).FirstOrDefault();
            if (register != null)
            {
                register.LoginOTP = otp;
                register.dateTime = DateTime.Now;
                register.isLoginConfirmed = false;
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();

            }
            SendLoginOTPtoMobile(mobileNo, otp);

            return Json(new { success = true });
        }
        private void SendLoginOTPtoMobile(string mobileNo, string otp)
        {
           
            string message = $@"
        <html>
        <body>
            <p>Dear User,</p>

            <p>Your verification code is: {otp}</p>

            <p>Please use this code to complete the Login process.</p>

            <p>If you have any questions or need assistance, feel free to contact our support team.<br /><strong> at +91 9209764995</strong></p>

            <p>Best Regards,<br/>
            Your Application Team</p>
        </body>
        </html>
    ";

          SendWhatsappMessage sendWhatsappMessage = new SendWhatsappMessage();
            var whatsappmessage = sendWhatsappMessage.sendWhatsappMessage(mobileNo,message);
            
          
            // Add any additional logic here (e.g., logging, error handling)
        }
        [HttpPost]
        public ActionResult SendOtp(string email,string pfcode)
        {
            
            var otp = RandomString(6); // Implement this method to generate OTP
                                       //  SendOtpEmail(email, otp); // Implement this method to send OTP via email
            var register = db.registrations.Where(x => x.Pfcode == pfcode).FirstOrDefault();
            if(register != null)
            {
                register.emailOTP=otp;
                register.dateTime = DateTime.Now;
                register.isEmailConfirmed = false;
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();

            }
            SendOtpEmail(email, otp);

            return Json(new { success = true });
        }
        private void SendOtpEmail(string email, string otp)
        {
            // Construct the email body with OTP
            string emailBody = $@"
        <html>
         <head>
             <title>Action Required: Use OTP to Verify Your Account</title>
       <style>
         body {{
           font-family: Arial, sans-serif;
           margin: 0;
           padding: 0;
           background-color: #F5E8E8;
         }}

         .container {{
           max-width: 600px;
           margin: 0 auto;
           padding: 20px;
           background-color: #FFFFFF;
         }}

         h2 {{
           color: #333333;
         }}

         p {{
           color: #555555;
         }}

         table {{
           width: 100%;
         }}

         th, td {{
           padding: 10px;
           text-align: left;
           vertical-align: top;
           border-bottom: 1px solid #dddddd;
         }}

         .logo {{
           text-align: center;
           margin-bottom: 20px;
         }}

         .logo img {{
           max-width: 200px;
         }}
       </style>
            </head>
       <body>
       <div class='container'>
         <div class='logo'>
           <img src='https://frbilling.com/assets/Home/assets/images/logo.png' alt='Logo'>
         </div>
         <h4>General query from user</h4>
         <h3><strong>Dear User,</strong></h3>
         <h4>One-Time Password (OTP) for Verification</h4>
                <p>Your OTP is: <strong>{otp}</strong></p>
                <p>Please use this OTP to verify your account.</p>
                <p>If you didn't request this OTP, you can safely ignore this email.</p>
                   <p>If you have any questions or need assistance, feel free to contact our support team.<br />
                        <strong> at +91 9209764995</strong></p>

         <hr>
         <p>Thank you for your attention to this matter.</p>
         <p>Best regards,</p>
         <p><strong>Fr-Billing</strong></p>
         
       </div>
     </body>
        </html>
    ";

            // Set up the email model
            SendModel emailModel = new SendModel
            {
                toEmail = email,
                subject = "Email Verification OTP",
                body = emailBody
            };

            // Send the email using your email sending logic
            SendEmailModel sm = new SendEmailModel();
            var mailMessage = sm.MailSend(emailModel);

            // Add any additional logic here (e.g., logging, error handling)
        }
        [HttpGet]
        public ActionResult ActivateUserBackendUrl()
        {
            return View();
        }

       
        [HttpPost]
        public ActionResult ActivateUserBackendUrl(string Pfcode, int pid)
        {
            try
            {
               

                Pfcode= Pfcode.ToString().ToUpper().Trim();
                var flag = false;
                var saveSector = false;
                var Pfcheck = db.registrations.Where(x => x.Pfcode == Pfcode).FirstOrDefault();
                var package = db.Packages.Where(x => x.Pid == pid).FirstOrDefault();
                if(Pfcheck==null)
                {
                    ViewBag.ErrorMessage = "User not registered. Please register before activating your account.";

                    return View();
                }

                Pfcheck.subscriptionForInDays = package.Subcriptionforindays;
                Pfcheck.isPaid = package.isPaid;
                Pfcheck.paymentDate = DateTime.Now;

                db.Entry(Pfcheck).State = EntityState.Modified;
                db.SaveChanges();


                var uregistration = db.registrations.Where(x => x.Pfcode == Pfcode).FirstOrDefault();
                int edays = (int)Pfcheck.subscriptionForInDays;
                DateTime paymentdate = (DateTime)uregistration.paymentDate;
                DateTime expirydate = paymentdate.AddDays(edays);


                if (Pfcheck.isPaid == true && expirydate >= DateTime.Now)

                {
                    Franchisee fr=new Franchisee();
                    fr.PF_Code=Pfcode.ToUpper().Trim();
                    fr.Franchisee_Name = Pfcheck.franchiseName;
                    fr.Sendermail = Pfcheck.emailId;
                    fr.Datetime_Fr=DateTime.Now;
                    fr.password = Pfcheck.password;
                    fr.ContactNo = Pfcheck.mobileNo;
                    fr.IsGECSector = false;
                    var franchisee=db.Franchisees.Where(x => x.PF_Code == Pfcode).FirstOrDefault(); 
                    if(franchisee == null)
                    {
                        db.Franchisees.Add(fr);
                        db.SaveChanges();
                    }
                       

                    //Adding Eantries To the Sector Table
                    var sectornamelist = db.sectorNames.ToList();

                    var pfcode = (from u in db.Franchisees
                                  where u.PF_Code == Pfcode
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
                            sn.BillGecSec = null;

                            if (sn.Sector_Name == "Within city")
                            {
                                sn.Priority = 1;
                                sn.Pincode_values = "400001-400610,400615-400706,400710-401203,401205-402209";

                                sn.CashD = true;
                                sn.CashN = true;
                                sn.BillD = true;
                                sn.BillN = true;
                                sn.BillGecSec = null;
                            }

                            else if (sn.Sector_Name == "Within state")
                            {

                                sn.CashD = true;
                                sn.CashN = false;
                                sn.BillD = true;
                                sn.BillN = false;
                                sn.BillGecSec = null;
                                sn.Priority = 2;
                                sn.Pincode_values = "400000-403000,404000-450000";
                            }


                            else if (sn.Sector_Name == "North East")
                            {
                                sn.Priority = 3;
                                sn.Pincode_values = "400000-450000,360000-400000,450000-490000";

                                sn.CashD = false;
                                sn.CashN = true;
                                sn.BillD = false;
                                sn.BillN = true;
                                sn.BillGecSec = null;
                            }

                            else if (sn.Sector_Name == "Metro")
                            {
                                sn.Priority = 4;
                                sn.Pincode_values = "180000-200000";

                                sn.CashD = true;
                                sn.CashN = true;
                                sn.BillD = true;
                                sn.BillN = true;
                                sn.BillGecSec = null;
                            }



                            else if (sn.Sector_Name == "Jammu and Kashmir")
                            {
                                sn.Priority = 5;
                                sn.Pincode_values = "780000-800000,170000-180000";
                                sn.BillGecSec = null;
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
                                sn.BillGecSec = null;
                            }
                            else
                            {
                                sn.Pincode_values = "";
                            }




                            db.Sectors.Add(sn);

                                db.SaveChanges();

                        }
                    }
                    //////////////////////////////////////////////

                    var Companyid = "Cash_" + Pfcode.ToUpper();


                    var secotrs = db.Sectors.Where(m => m.Pf_code == Pfcode.ToUpper() ).ToList();

                    Company cm = new Company();
                    cm.Company_Id = Companyid;
                    cm.Pf_code = Pfcode.ToUpper().Trim();
                    cm.Phone = 1234567890;
                    cm.Company_Address = "";
                    cm.Company_Name = Companyid;
                    cm.Email = Companyid + "@gmail.com";
                    db.Companies.Add(cm);
                     db.SaveChanges();



                    var basiccompid = "BASIC_TS";

                    var basicrec = db.Ratems.Where(m => m.Company_id == "BASIC_TS").FirstOrDefault();



                    if (basicrec == null)
                    {
                        Company bs = new Company();
                        bs.Company_Id = basiccompid;
                        bs.Pf_code = null;
                        bs.Phone = 1234567890;
                        bs.Company_Address = "";
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
                        Priority pri = new Priority();
                        Dtdc_Ecommerce dtdc_Ecommerce = new Dtdc_Ecommerce();


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
                        cs.Exslab1 = 1;
                        cs.Exslab2 = 1;
                        // cs.CashCounterExpr = true;

                        pri.Company_id = basiccompid;
                        pri.Sector_Id = i.Sector_Id;
                        pri.prinoofslab = 2;

                        pri.prislab1 = 1;
                        pri.prislab2 = 1;
                        pri.prislab3 = 1;
                        pri.prislab4 = 1;

                        pri.priupto1 = 1;
                        pri.priupto2 = 1;
                        pri.priupto3 = 1;
                        pri.priupto4 = 1;

                        dtdc_Ecommerce.Company_id = basiccompid;
                        dtdc_Ecommerce.Sector_Id = i.Sector_Id;
                        dtdc_Ecommerce.EcomPslab1 = 1;
                        dtdc_Ecommerce.EcomPslab2 = 1;
                        dtdc_Ecommerce.EcomPslab3 = 1;
                        dtdc_Ecommerce.EcomPslab4 = 1;
                        dtdc_Ecommerce.EcomGEslab1 = 1;
                        dtdc_Ecommerce.EcomGEslab2 = 1;
                        dtdc_Ecommerce.EcomGEslab3 = 1;
                        dtdc_Ecommerce.EcomGEslab4 = 1;
                        dtdc_Ecommerce.EcomPupto1 = 1;
                        dtdc_Ecommerce.EcomPupto2 = 1;
                        dtdc_Ecommerce.EcomPupto3 = 1;
                        dtdc_Ecommerce.EcomPupto4 = 1;
                        dtdc_Ecommerce.EcomGEupto1 = 1;
                        dtdc_Ecommerce.EcomGEupto2 = 1;
                        dtdc_Ecommerce.EcomGEupto3 = 1;
                        dtdc_Ecommerce.EcomGEupto4 = 1;
                        dtdc_Ecommerce.NoOfSlabN = 2;
                        dtdc_Ecommerce.NoOfSlabS = 2;


                        db.Ratems.Add(dox);
                        db.Nondoxes.Add(ndox);
                        db.express_cargo.Add(cs);
                        db.Priorities.Add(pri);
                        db.Dtdc_Ecommerce.Add(dtdc_Ecommerce);


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

                    userDetailsModel user = new userDetailsModel();
                    paymentLog paymentdata = new paymentLog();

                    user.name = Pfcheck.franchiseName;
                    user.email = Pfcheck.emailId;
                    user.mobileNo = Pfcheck.mobileNo;
                    user.address = "";

                    Session["DataName"] = user.name;
                    Session["Dataemail"] = user.email;
                    Session["Datacontact"] = user.mobileNo;
                    Session["Dataaddress"] = user.address;
                    if (package != null)
                    {


                        paymentdata.Pfcode = Pfcheck.Pfcode;
                        paymentdata.ownerName = "";
                        paymentdata.totalAmount = package.Amount;
                        paymentdata.registrationId = Pfcheck.registrationId;
                        paymentdata.status = "authorized";
                        paymentdata.dateTime = DateTime.Now;
                        paymentdata.description = package.Despription;
                        paymentdata.paymentmethod = "Cash".ToUpper();
                        paymentdata.RenewalDate = DateTime.Now;
                       


                        var pdata = db.paymentLogs.Where(x => x.Pfcode == paymentdata.Pfcode).FirstOrDefault();
                        if (pdata == null)
                        {
                            try
                            {
                                db.paymentLogs.Add(paymentdata);
                                db.SaveChanges();
                            }
                            catch(Exception e)
                            {

                            }
                        }
                        





                    }
                    if (Pfcheck.isPaid==true)
                    {

                        string emailbody = $@"
                            <html>
                            <body>
                                <p>Dear {Pfcheck.franchiseName},</p>

                                <p>Congratulations! You are now subscribed to FrBilling, and we are thrilled to welcome you on board.</p>

                                <p>Your subscription details:</p>
                                <ul>
                                    <li><strong>Subscription Plan:</strong> {Pfcheck.subscriptionForInDays} Days</li>
                                    <li><strong>Subscription Start Date:</strong> {paymentdata.dateTime}</li>
                                    <li><strong>Subscription End Date:</strong> {expirydate}</li>
                                </ul>

                                <p>Thank you for choosing FrBilling. We are committed to providing you with an exceptional experience.</p>

                                <p>If you have any questions or need assistance, feel free to contact our support team at <strong>info@frbilling.com</strong> or call us at <strong>+91 9209764995<strong>.</p>

                                <p>Best Regards,<br/>
                                The FrBilling Team</p>
                            </body>
                            </html>
                        ";
                        SendModel emailModel = new SendModel
                        {
                            toEmail = Pfcheck.emailId,
                            subject = "Welcome To Fr-Billing Subscription",
                            body = emailbody
                        };
                        SendEmailModel sm = new SendEmailModel();
                        var mailmessage = sm.MailSend(emailModel);

                        // TempData["success"] = "Your registration has been successfully completed!";
                        //return RedirectToAction("makePaymentPartial", "Admin", user);
                        return RedirectToAction("AdminLogin");
                        // var userAllDetails = db.registrations.Where(x => x.Pfcode == userDetails.Pfcode).FirstOrDefault();
                        // return Json(userAllDetails, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TempData["error"] = "Something went wrong Please try Again!!";
                    }
                }
            
                else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
            }
            catch (Exception e)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
.Where(y => y.Count > 0)
.ToList();
                TempData["error"] = errors;
            }

            return RedirectToAction("AdminLogin");


        }

        private static Random random = new Random();
        public static string RandomMobileOTP(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567899876543210";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public ActionResult Pay(string paymentid)
        {
            //try
            //{
            var paymentlog = db.paymentLogs.Where(x => x.paymentLogId == "pay_KMXk1qOGUda3K6").FirstOrDefault();

            if (paymentlog != null)
            {
                ModelState.AddModelError("customErrorPay", "Pfcode already exist");
                //return PartialView("RegistrationPartialView", userDetails);
            }

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var key = "rzp_test_ORKmnTOESzja0p";
            var key_secret = "MIDsnrVFquZJY8MPVSGPbOYs";
            RazorpayClient client = new RazorpayClient(key, key_secret);

            Razorpay.Api.Payment payment = client.Payment.Fetch(paymentid);



            string status = (string)payment["status"];

            var amount = (double)payment["amount"] / 100;
            var description = (string)payment["description"];
            var email = (string)payment["email"];
            var paymentmethod = (string)payment["method"];

            var userdetails = db.registrations.Where(x => x.emailId == email).FirstOrDefault();


            if (status == "authorized")
            {

                var save = db.paymentLogSave(userdetails.Pfcode, userdetails.ownerName, amount, userdetails.registrationId, paymentid, status, DateTime.Now, description, paymentmethod);


                //var userid = Convert.ToInt64(Session["User"]);

                var Registration = db.registrations.Where(m => m.emailId == email).ToList();
                var PaymentLog = db.paymentLogs.Where(x => x.paymentLogId == paymentid).ToList();
                var grandTotal = (PaymentLog.FirstOrDefault().totalAmount + ((PaymentLog.FirstOrDefault().totalAmount * 18) / 100)).ToString();
                PaymentLog.FirstOrDefault().status = DtDc_Billing.Models.InWords.NumberToWords(grandTotal);
                // var user_id = DataSet2.FirstOrDefault().userid;

                //DataSet2.FirstOrDefault().status = AmountTowords.changeToWords(DataSet1.FirstOrDefault().plan_price.ToString());

                // PaymentLog.FirstOrDefault().totalAmount = (PaymentLog.FirstOrDefault().totalAmount / 100);

                //var email1 = DataSet2.FirstOrDefault().email_id;
                LocalReport lr = new LocalReport();

                string path = System.IO.Path.Combine(Server.MapPath("~/RdlcReport"), "Invoice.rdlc");

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                ReportDataSource rd1 = new ReportDataSource("Registration", Registration);
                ReportDataSource rd = new ReportDataSource("PaymentLog", PaymentLog);

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);

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


                string savePath = Server.MapPath("~/PDF/" + Registration.FirstOrDefault().Pfcode + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }


                //using (MailMessage mm = new MailMessage("navlakheprajkta23@gmail.com", "navlakheprajkta23@gmail.com"))
                //{
                //    mm.Subject = "DTDC subscription invoice";

                //    string Bodytext = "<html><body>Please Find Attachment</body></html>";
                //    Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                //    mm.IsBodyHtml = true;



                //    mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                //    AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");

                //    mm.Body = Bodytext;



                //    mm.Attachments.Add(attachment);

                //    SmtpClient smtp = new SmtpClient();
                //    smtp.Host = "smtp.gmail.com";
                //    smtp.EnableSsl = true;
                //    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                //    credentials.UserName = "navlakheprajkta23@gmail.com";
                //    credentials.Password = "ShubhPraje20";
                //    smtp.UseDefaultCredentials = true;
                //    smtp.Credentials = credentials;
                //    smtp.Port = 587;
                //    //smtp.Send(mm);



                //TempData["msg"] = "subscription plan activated, Invoice has been send to your mail address..!!";

                //string SubPath = "http://codetentacles-005-site1.htempurl.com/Admin/AdminLogin?isPaymentSuccess=1";
                //return Redirect(SubPath);

                var ObjData = (from d in db.paymentLogs
                               where d.paymentLogId == paymentid
                               && d.RenewalStatus == null
                               select d).FirstOrDefault();

                string strdate = Convert.ToString(ObjData.dateTime);
                string[] strarr = strdate.Split(' ');
                string date = strarr[0];
                DateTime date1 = Convert.ToDateTime(date);

                string FilePath = Server.MapPath("~/images/PaymentMailTemplete.html");
                //"http://codetentacles-005-site1.htempurl.com/images/PaymentMailTemplete.html";// "D:\\MBK\\SendEmailByEmailTemplate\\EmailTemplates\\SignUp.html";
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();

                //Repalce [newusername] = signup user name   
                MailText = MailText.Replace("[newusername]", ObjData.ownerName);
                MailText = MailText.Replace("[Date]", date1.ToString("MM/dd/yyyy"));
                MailText = MailText.Replace("[amount]", amount.ToString());


                string subject = "FrBilling Subscription Payment Details";

                //Base class for sending email  
                MailMessage _mailmsg = new MailMessage();

                Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");
                //Make TRUE because our body text is html  
                _mailmsg.IsBodyHtml = true;

                _mailmsg.Attachments.Add(attachment);
                //Set From Email ID  
                _mailmsg.From = new MailAddress("frbillingsoftware@gmail.com");

                //Set To Email ID  
                _mailmsg.To.Add(email);

                //Set Subject  
                _mailmsg.Subject = subject;

                //Set Body Text of Email   
                _mailmsg.Body = MailText;


                //Now set your SMTP   
                SmtpClient _smtp = new SmtpClient();

                //Set HOST server SMTP detail  
                _smtp.Host = "smtp.gmail.com";

                //Set PORT number of SMTP  
                _smtp.Port = 587;

                //Set SSL --> True / False  
                _smtp.EnableSsl = true;
                _smtp.UseDefaultCredentials = false;
                //Set Sender UserEmailID, Password  
                NetworkCredential _network = new NetworkCredential("frbillingsoftware@gmail.com", "rqaynjbevkygswkx");
                _smtp.Credentials = _network;

                //Send Method will send your MailMessage create above.  
                _smtp.Send(_mailmsg);

                return RedirectToAction("PaymentSuccess");
                //return Json("Success");
                //return RedirectToAction("SubscriptionPanel");
            }
            return RedirectToAction("MakePayment");
            //return Json("");
        }

        public ActionResult makePaymentPartial()
        {
            //string successMessage = TempData["success"] as string;
            //ViewBag.SuccessMessage = successMessage;

            return View();
        }

        public ActionResult PaymentSuccess()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult PaymentSuccess(string paymentSuccess = "1")
        {
            string SubPath = "http://codetentacles-005-site1.htempurl.com/Admin/AdminLogin?isPaymentSuccess=1";
            return Redirect(SubPath);
        }

        public ActionResult RenewalNotification()
        {
            return View();  
        }
        public ActionResult ErrorPage()
        {
            return View();  
        }
        public ActionResult GetConsignmentInfo()
        {
            return View();
        }
        [HttpPost]
        public async  Task<ActionResult> GetConsignmentInfo(string consignmetno)
        {
            //string apiurl = "https://tracking.dtdc.com/ctbs-tracking/customerInterface.tr?submitName=showCITrackingDetails&cType=Consignment&cnNo=" + consignmetno;
            //  string apiurl = "https://www.dtdc.com/track?trackid="+consignmetno;
            string apiurl = "https://txk.dtdc.com/ctbs-tracking/customerInterface.tr?submitName=showCITrackingDetails&cnNo=" + consignmetno.Trim() + "&cType=Consignment#";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiurl);

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Consignmentno = consignmetno;
                        string responsedata = await response.Content.ReadAsStringAsync();
                        return View((object)responsedata);

                        //  return View(responsedata);

                        // return Content(responsedata,"text/html");
                    }
                    else
                    {
                        var error = response.StatusCode + "=" + response.ReasonPhrase;
                        // return new HttpStatusCodeResult(response.StatusCode, error);
                        return View(error);

                    }
                }
                catch (Exception ex)
                {
                    return View(ex.Message);
                    //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error: " + ex.Message);
                }
            }



        }
        [HttpPost]
        public JsonResult GetConsignmentInfoInDeatils(string ConsignmnetNo)
        {
            if (ConsignmnetNo == null)
            {
                return Json(null);
            }
          //  string apiurl = "https://tracking.dtdc.com/ctbs-tracking/customerInterface.tr?submitName=getLoadMovementDetails&cnNo=" + ConsignmnetNo;
            string apiurl = "https://txk.dtdc.com/ctbs-tracking/customerInterface.tr?submitName=showCITrackingDetails&cnNo=" + ConsignmnetNo + "&cType=Consignment";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync(apiurl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string responsedata = response.Content.ReadAsStringAsync().Result;
                        return Json(responsedata);

                        //  return View(responsedata);

                        // return Content(responsedata,"text/html");
                    }
                    else
                    {
                        var error = response.StatusCode + "=" + response.ReasonPhrase;
                        // return new HttpStatusCodeResult(response.StatusCode, error);
                        return Json(error);

                    }
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                    //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Server Error: " + ex.Message);
                }
            }
        }



        [HttpGet]
        public ActionResult EmailTemplateModel(EmailTemplateModel emailTemplateModel)
        {
           
            return View();
        }
        public string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


        public async Task<List<MonthlyDataAnalysisModel>> getMonthlyDataAnalysisModel()
        {

            List<MonthlyDataAnalysisModel> getSummary = new List<MonthlyDataAnalysisModel>();

             getSummary = db.MonthlyDataAnalysis().Take(1).Select(x=> new MonthlyDataAnalysisModel
            {
                 PFCode = x.PFCode,
                 InvoiceCount = x.InvoiceCount ?? 0,
                 TotalInvoiceAmount = x.TotalInvoiceAmount ?? 0,
                 PaidAmount = x.PaidAmount ?? 0,
                 FranchiseName = x.FranchiseName,
                 OwnerName = x.OwnerName,
                 EmailId = x.Email,
                 LastMonth = x.LastMonth,
                 CashAmount = x.CashAmount ?? 0

            }).ToList();

            return getSummary;
        }

        public async Task< FinancialSummary> GetMonthlyFinancialSummary(int month, int year,string pfcode)
        {
            var summary = new FinancialSummary();
           // string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var franchiseeName = db.Franchisees.Where(x => x.PF_Code == pfcode).Select(x => x.Franchisee_Name).FirstOrDefault();

            // Assuming your DbContext is named 'DbContext'
            using (var db = new db_a92afa_frbillingEntities())
            {
                // Get all invoices for the given month and year
                var invoices = db.Invoices
                    .Where(i =>i.Pfcode==pfcode && i.invoicedate.Value.Month == month && i.invoicedate.Value.Year == year && (i.isDelete==null || i.isDelete==false))
                    .ToList();

                // Total Revenue: sum of all invoice amounts
                summary.TotalRevenue = invoices.Sum(i => i.netamount)??0;

                // Outstanding Invoices: sum of unpaid invoices
                summary.OutstandingInvoicesAmount = invoices
                    .Where(i => i.paid !=null && i.paid<i.netamount)
                    .Sum(i => i.netamount)??0;

                // Outstanding Invoices Count
                summary.OutstandingInvoicesCount = invoices
                    .Count(i => i.paid != null && i.paid < i.netamount);

                // Paid Invoices: sum of all paid invoices
                summary.InvoicesPaidAmount = invoices
                    .Where(i => i.netamount == i.paid)
                    .Sum(i => i.netamount)??0;

                // Paid Invoices Count
                summary.InvoicesPaidCount = invoices
                    .Count(i => i.netamount ==i.paid);

                // Invoices Unpaid: sum of unpaid invoices (another way to calculate unpaid)
                summary.InvoicesUnpaidAmount = invoices
                    .Where(i => i.paid == null)
                    .Sum(i => i.netamount)??0;

                // Invoices Unpaid Count
                summary.InvoicesUnpaidCount = invoices
                    .Count(i => i.paid == null);

                // Total Expense: let's assume this comes from an 'Expenses' table
                summary.TotalExpense = db.Expenses
                    .Where(e => e.Pf_Code==pfcode &&  e.Datetime_Exp.Value.Month == month && e.Datetime_Exp.Value.Year == year)
                    .Sum(e => e.Amount)??0;
            }
            summary.pfcode = pfcode;
            summary.FranchiseeName=franchiseeName;
            return summary;
        }

        // Example usage in your controller
        [HttpGet]
       
        public async Task<ActionResult> SendEmailMessageTotheOwner()
        {
            var data = await getMonthlyDataAnalysisModel();

            foreach (var singleFranchise in data)
            {
                
                    var model = new EmailTemplateModel();
                    string emailBody = RenderPartialViewToString("MonthlyStaticEmailTemplate", singleFranchise);

                      //string email = "survasesainath999@gmail.com";
                      string email = singleFranchise.EmailId;
                  //  string email = "pratikshacodetechnology@gmail.com";
                    string subject = "FR-Billing - Monthly Highlights: " + singleFranchise.FranchiseName.ToUpper() + " Metrics for "+ singleFranchise.LastMonth;
                    string body = emailBody;

                    SendEmailModel sm = new SendEmailModel();
                    string result = await sm.SendEmail(email, subject, body);

                    if (result != "Email sent successfully!")
                    {   
                        // Log the error or handle it as needed
                        Console.WriteLine($"Failed to send email to {email}. Moving to next record.");
                        continue; // Skip to the next record if email sending fails
                    }
                
            }

            return RedirectToAction("EmailTemplateModel");
        }

      
        [HttpGet]
        public ActionResult ShowFinancialChart(string pfcode)
        {
           
            var summary = new FinancialSummary();
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            var franchiseeName=db.Franchisees.Where(x=>x.PF_Code==pfcode).Select(x=>x.Franchisee_Name).FirstOrDefault();
            // Assuming your DbContext is named 'DbContext'
            using (var db = new db_a92afa_frbillingEntities())
            {
                var invoices = db.Invoices
                    .Where(i => i.Pfcode == pfcode && i.invoicedate.Value.Month == month && i.invoicedate.Value.Year == year && (i.isDelete == null || i.isDelete == false))
                    .ToList();

                summary.TotalRevenue = invoices.Sum(i => i.netamount);

                summary.OutstandingInvoicesAmount = invoices
                    .Where(i => i.paid != null && i.paid < i.netamount)
                    .Sum(i => i.netamount);

                // Outstanding Invoices Count
                summary.OutstandingInvoicesCount = invoices
                    .Count(i => i.paid != null && i.paid < i.netamount);

                // Paid Invoices: sum of all paid invoices
                summary.InvoicesPaidAmount = invoices
                    .Where(i => i.netamount == i.paid)
                    .Sum(i => i.netamount);
                
                // Paid Invoices Count
                summary.InvoicesPaidCount = invoices
                    .Count(i => i.netamount == i.paid);

                // Invoices Unpaid: sum of unpaid invoices (another way to calculate unpaid)
                summary.InvoicesUnpaidAmount = invoices
                    .Where(i => i.paid == null)
                    .Sum(i => i.netamount);

                // Invoices Unpaid Count
                summary.InvoicesUnpaidCount = invoices
                    .Count(i => i.paid == null);

                // Total Expense: let's assume this comes from an 'Expenses' table
                summary.TotalExpense = db.Expenses
                    .Where(e => e.Datetime_Exp.Value.Month == month && e.Datetime_Exp.Value.Year == year)
                    .Sum(e => e.Amount);
            }
            summary.pfcode = pfcode;
            summary.FranchiseeName = franchiseeName;
            ViewBag.DataPoints = JsonConvert.SerializeObject(new {
                TotalRevenue = summary.TotalRevenue,
                OutstandingInvoicesAmount = summary.OutstandingInvoicesAmount,
                InvoicesPaidAmount = summary.InvoicesPaidAmount,
                InvoicesUnpaidAmount = summary.InvoicesUnpaidAmount,
                TotalExpense = summary.TotalExpense


            });


           return View();
        }

        public byte[] GenerateFinancialPieChart(FinancialSummary model)
        {
            // Define the chart data
            float[] data = new float[]
            {
        (float)(model.TotalRevenue ?? 0),
        (float)(model.OutstandingInvoicesAmount ?? 0),
        (float)(model.InvoicesPaidAmount ?? 0),
        (float)(model.InvoicesUnpaidAmount ?? 0),
        (float)(model.TotalExpense ?? 0)
            };

            // Define colors for each section
            System.Drawing.Color[] colors = new System.Drawing.Color[]
            {
        System.Drawing.Color.Red,
        System.Drawing.Color.Blue,
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Green,
        System.Drawing.Color.Purple
            };

            int width = 400;
            int height = 400;

            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(System.Drawing.Color.White);

                    // Calculate the total sum of the data
                    float total = data.Sum();

                    // Define the starting angle
                    float startAngle = 0f;

                    // Draw each slice of the pie chart
                    for (int i = 0; i < data.Length; i++)
                    {
                        float sweepAngle = 360f * (data[i] / total);  // Calculate the angle for each slice
                        using (Brush brush = new SolidBrush(colors[i]))
                        {
                            graphics.FillPie(brush, 50, 50, 300, 300, startAngle, sweepAngle);
                        }
                        startAngle += sweepAngle;
                    }
                }

                // Save the image to a byte array
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
           

        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel changePassword)
        {
            var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            if (ModelState.IsValid)
            {
                var obj = db.registrations.Where(m =>m.Pfcode==PfCode && m.password.Trim() == changePassword.oldpass.Trim()).FirstOrDefault();

                if (obj != null)
                {
                    obj.password = changePassword.newpass.Trim();
                    db.registrations.Attach(obj);
                    db.Entry(obj).Property(x => x.password).IsModified = true;
                    db.SaveChanges();
                  
                    TempData["changeSuccess"] = "Your password has been successfully changed. Please Sign in again";
                    return RedirectToAction("Adminlogin", "Admin");
                }
                else
                {
                    TempData["error"] = "Invalid Old Password";
                }
            }

            return View(changePassword);
        }


        public ActionResult NewpasswordSave(string pfcode)
        {


            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var senderemail = db.Franchisees.Where(x => x.PF_Code == pfcode).Select(x => x.Sendermail).FirstOrDefault();
            var Token = new String(stringChars);

            //string Bodytext = "<html><body>Your Verification Token is -" + Tokne + " </body></html>";

            string Bodytext = "<html><body><h1>Verification Required</h1><p>Dear User,</p><p>Your Verification Token is - <strong>" + Token + "</strong></p><p>Please use this token to complete your verification process.</p><p>If you did not request this verification, please ignore this email or contact support.</p><br><p>Best Regards,</p><p><strong>Fr-Billing</strong></p><img src='https://frbilling.com/assets/Home/assets/images/logo.png' alt='Fr-Billing Logo' width='120' height='70'/></body></html>";


            SendModel model = new SendModel();
            model.subject = "Token verification for change password";
            model.toEmail = senderemail;
            model.body = Bodytext;
            SendEmailModel send = new SendEmailModel();
            send.SendEmail("pratikshacodetechnology@gmail.com",model.subject,model.body);



            registration reg = db.registrations.Where(x=>x.Pfcode==pfcode).FirstOrDefault();
            if (reg != null)
            {
                reg.ChangePassToken= Token;
              
                db.Entry(reg).Property(x => x.ChangePassToken).IsModified = true;
                db.SaveChanges();
            }


            ViewBag.sendmail = "The token ID has been sent to your registered email address; please check your inbox for it.";

            return Json("chamara", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GECimportFromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GECimportFromExcel(HttpPostedFileBase httpPostedFileBase)
        {

            var task = Task.Run(async () => await InsertGECDataAsync(httpPostedFileBase));
            return View();
        }
        public async Task InsertGECDataAsync(HttpPostedFileBase httpPostedFileBase)
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

                            var pincode = workSheet.Cells[rowIterator, 1].Value.ToString().Trim();
                            var city = workSheet.Cells[rowIterator, 2].Value.ToString();
                            var sectorName = workSheet.Cells[rowIterator, 3].Value.ToString();
                            GECSector addnew = new GECSector();

                            addnew.Pincode = pincode;
                            addnew.City = city;
                            addnew.SectorName = sectorName;
                            addnew.Priority = null;
                            addnew.Pf_code = null;
                            db.GECSectors.Add(addnew);
                            try
                            {
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
                    }

                }
            }
        }

    }
}
