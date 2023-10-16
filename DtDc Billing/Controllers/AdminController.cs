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

namespace DtDc_Billing.Controllers
{
    //[OutputCache(CacheProfile = "Cachefast")]
    public class AdminController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Adminsss
       
        //[SessionUserModule]
        public ActionResult AdminLogin(string ReturnUrl)
        {

            ViewBag.ReturnUrl = ReturnUrl;
            return View();
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
                _mailmsg.Body = "mail id from newsletters "+ newsletters.email;


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
                var obj = db.getLogin(login.UserName.Trim(), login.Password.Trim(), "").Select(x => new registration { registrationId = x.registrationId, userName = x.username, Pfcode = x.Pfcode, referralCode = x.referralCode }).FirstOrDefault();

                if (obj != null)
                {
                    var ObjData = (from d in db.registrations
                                   where d.Pfcode == obj.Pfcode
                                   select d).FirstOrDefault();


                    // Get the current date
                    DateTime currentDate = DateTime.Now;
                    // Add 30 days to the current date
                    DateTime newDate = ObjData.dateTime.Value.AddDays(ObjData.subscriptionForInDays ?? 0);

                    if (currentDate > newDate)
                    {
                       
                        ModelState.AddModelError("freedome", "Free demo of 30 days is expired");
                        return View();
                    }

                    DateTime After1Year;

                    var renewalstatuscheck = (from d in db.paymentLogs
                                              where d.Pfcode == obj.Pfcode.ToString()
                                              select new { d.RenewalStatus }).FirstOrDefault();

                    if (renewalstatuscheck.RenewalStatus == "1")
                    {
                        var Date = (from d in db.paymentLogs
                                    where d.Pfcode == obj.Pfcode
                                    select new
                                    {
                                        d.RenewalDate
                                    }).FirstOrDefault();

                        string strdate = Convert.ToString(Date.RenewalDate);
                        string[] strarr = strdate.Split(' ');
                        string date = strarr[0];
                        DateTime date1 = Convert.ToDateTime(date);
                        After1Year = date1.AddYears(1);
                    }
                    else
                    {
                        var Date = (from d in db.registrations
                                    where d.Pfcode == obj.Pfcode
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
                    //var firmlist = db.FirmDetails.ToList();

                    Session["After1Year"] = After1Year;

                    if (obj != null)
                    {
                        Session["Admin"] = obj.registrationId.ToString();
                        Session["UserName"] = obj.userName.ToString();
                        Session["PFCode"] = obj.Pfcode.ToString();
                       // Session["firmlist"] = firmlist;
                        string decodedUrl = "";

                        HttpCookie cookie = new HttpCookie("Cookies");
                        cookie["AdminValue"] = obj.Pfcode.ToString();

                        cookie["UserValue"] = obj.userName.ToString();
                        cookie.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(cookie);



                        cookie["referalCode"] = obj.referralCode.ToString();
                        cookie.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(cookie);


                        var objaccessPAge = (from d in db.AdminAccessPages
                                             where d.Pfcode == obj.Pfcode.ToString()
                                             select new { d.Accesspage }).FirstOrDefault();

                        var renewalstatus = (from d in db.paymentLogs
                                             where d.Pfcode == obj.Pfcode.ToString()
                                             select new { d.RenewalStatus }).FirstOrDefault();

                        if (objaccessPAge != null)
                        {
                            Session["AccessPage"] = objaccessPAge.Accesspage.ToString();
                        }
                        else
                        {
                            Session["AccessPage"] = "0";

                        }
                        if (!string.IsNullOrEmpty(ReturnUrl))
                            decodedUrl = Server.UrlDecode(ReturnUrl);

                        //Login logic...

                        if (Url.IsLocalUrl(decodedUrl))
                        {
                            return Redirect(decodedUrl);
                        }
                        else
                        {
                            if (After1Year.Date <= DateTime.Now.Date)
                            {
                                ModelState.AddModelError("LoginAuth", "Your Subscription is Expired");
                            }
                            else if (renewalstatus.RenewalStatus == "1")
                            {


                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                if ((After1Year - DateTime.Now).TotalDays < 30)
                                {
                                    if (ObjData.IsRenewalEmailDate != DateTime.Now.Date)
                                    {
                                        ObjData.IsRenewalEmail = "0";
                                        db.Entry(ObjData).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    if (ObjData.IsRenewalEmail != "1" || (ObjData.IsRenewalEmailDate == null && ObjData.IsRenewalEmailDate != DateTime.Now.Date))
                                    {

                                        ObjData.IsRenewalEmailDate = DateTime.Now.Date;
                                        ObjData.IsRenewalEmail = "1";
                                        db.Entry(ObjData).State = EntityState.Modified;
                                        db.SaveChanges();

                                        ////////send mail//////////
                                        mail mail = new mail();
                                        mail.franchiseName = ObjData.franchiseName;
                                        mail.emailId = ObjData.emailId;
                                        mail.After1Year = After1Year;
                                        sendEmail.send(mail);

                                        ///////send mail//////////


                                    }
                                }
                                return RedirectToAction("Index", "Home");

                            }
                        }
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
            // [RegularExpression("^[0-9]*$", ErrorMessage = "Pincode must be numeric")]
           
       
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
                    string[] strarr = pincodearayy[i].Split(',', '-');

                    for (int j = 0; j < strarr.Count(); j++)
                    {
                        
                        if (!Regex.Match(strarr[j], @"^(\d{6})?$").Success)
                        {
                            //TempData["PinError1"] = "Pincode must be numeric!";
                            //ModelState.AddModelError("PinError1", "Pincode must be numeric");
                            //string pfcodef = "";
                            if (franchisee.Pfcode == null)
                            {
                                franchisee.Pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

                            }
                            List<Sector> secct = (from u in db.Sectors
                                                   where u.Pf_code == franchisee.Pfcode

                                                   select u).ToList();
                            ViewBag.DataSector = secct; 
                            ViewBag.Message = "Pincode must be numeric";
                           // return View("FranchiseeList", fc);
                            return PartialView("Add_SectorPin", secct);
                            //return View(fc);
                        }


                        else
                        {
                            Sector str = db.Sectors.Find(Convert.ToInt16(sectoridarray[i]));

                            if (pincodearayy[i] == "")
                            {
                                pincodearayy[i] = null;
                            }


                            str.Pincode_values = pincodearayy[i];
                            db.Entry(str).State = EntityState.Modified;
                        }
                    }

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


                string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
                List<Sector> secct1 = (from u in db.Sectors
                                   where u.Pf_code == pfcode//franchisee.Pfcode

                                       select u).ToList();
                ViewBag.DataSector = secct1;
                return View("Add_SectorPin",secct1);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FranchiseeModel franchisee)
        {
            if (ModelState.IsValid)
            {
                Franchisee Fr = new Franchisee();
                
                Fr.PF_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();
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

                var getNewFilePath = "";
                if (franchisee.StampFilePath == null)
                {
                    getNewFilePath = db.Franchisees.Where(x => x.PF_Code == Fr.PF_Code).Select(x => x.StampFilePath).FirstOrDefault();
                }
                Fr.StampFilePath = (franchisee.StampFilePath==null || franchisee.StampFilePath == "")? getNewFilePath:franchisee.StampFilePath ;


                db.Entry(Fr).State = EntityState.Modified;
                db.SaveChanges();

                var Reg = (from d in db.registrations
                           where d.Pfcode == franchisee.PF_Code
                           select d).FirstOrDefault();

                Reg.Pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
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
                Reg.IFSC_Code = franchisee.IFSCcode;
                Reg.Branch = franchisee.Branch;
                Reg.AccountType = franchisee.Accounttype;
                Reg.InvoiceStart = franchisee.InvoiceStart;

               

                db.Entry(Reg).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "franchisee Updated  Successfully!";
                return View(franchisee);
            }
            return View(franchisee);
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

                if (myFile != null && myFile.ContentLength != 0)
                {
                    var file = Request.Files[0];
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var newFileName = Pfcode;

                        var path = Path.Combine(Server.MapPath("~/Stamps"), fileName);

                        // Get the file extension
                        string fileExtension = Path.GetExtension(path);
                        file.SaveAs(path);


                        string originalFilePath = path;
                        string newFilePath = Path.Combine(Server.MapPath("~/Stamps"), newFileName + "" + fileExtension); ;


                        // Rename the file
                        System.IO.File.Delete(newFilePath);
                        System.IO.File.Move(originalFilePath, newFilePath);


                        // Save file path in database
                        try
                        {

                            var franchises = db.Franchisees.Where(x => x.PF_Code == Pfcode).FirstOrDefault();
                            franchises.StampFilePath = "http://frbilling.com/Stamps/" + newFileName + "" + fileExtension;
                            db.SaveChanges();
                        }
                        catch(Exception e)
                        {

                        }
                    }
                }
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult AddLogo(AddlogoModel logo)
        {
            var r = new Regex(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$");
            if (!r.IsMatch(logo.file.FileName))
            {
               // ModelState.AddModelError("fileerr", "Only Image files allowed.");
               TempData["Success1"] = "Only Image files allowed!";
            }
           else
            {
                string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();
                string _FileName = "";
                string _path = "";

                if (logo.file.ContentLength > 0)
                {
                    _FileName = Path.GetFileName(logo.file.FileName);
                    _path = Server.MapPath("~/UploadedLogo/" + _FileName);
                    
                    logo.file.SaveAs(_path);
                }
              
                var lo = (from d in db.Franchisees
                           where d.PF_Code == strpf
                          select d).FirstOrDefault();

               
                lo.LogoFilePath = _FileName;

                db.Entry(lo).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success1"] = "Logo Added Successfully!";
                return RedirectToAction("Franchiseelist");
            }

            //return View("AddLogo", logo);
            return RedirectToAction("Franchiseelist");
            //return PartialView(logo);

        }


            public ActionResult ImportCsv()
        {
            return View();
        }

        public ActionResult FranchiseeList()
        {
            //long stradmin = Convert.ToInt64(Session["Admin"]);
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();
            if (strpf == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var data = (from d in db.Franchisees
                        where d.PF_Code == strpf
                        select d).FirstOrDefault();

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


            if (Fr == null)
            {
                return HttpNotFound();
            }
            ViewBag.Company = Fr;


     
            List<Sector> st = (from u in db.Sectors
                               where u.Pf_code == strpf
                               orderby u.Priority
                               select u).ToList();
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
            ViewBag.Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "Pf_Code", "Pf_Code");


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
        [SessionUserModule]
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
            }


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

        public ActionResult Register(string referral="")
        {
            string testParameter = Request.QueryString["referral"];

            if(referral != "")
            {
                var isValidReferral = db.registrations.Where(x => x.referralCode == referral && x.isPaid == true).FirstOrDefault()!= null ? true : false;

                if(isValidReferral)
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

            if (ModelState.IsValid)
            {
                var flag = false;
                var Pfcheck = db.registrations.Where(x => x.Pfcode == userDetails.Pfcode).FirstOrDefault();


                if (Pfcheck != null)
                {
                    flag = true;
                    ModelState.AddModelError("customError", "Pfcode already exist");
                   
                }

                if(userDetails.isUserNameExist == false)
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

                var save = db.registrationSave(userDetails.Pfcode.ToUpper(), userDetails.franchiseName, userDetails.emailId, DateTime.Now, userDetails.ownerName, userDetails.userName, userDetails.password, false, userDetails.mobileNo, userDetails.address, RandomString(10), userDetails.referral);

                var savefranchisee = db.FranchiseeSave(userDetails.Pfcode.ToUpper(), userDetails.franchiseName, userDetails.emailId, DateTime.Now, userDetails.ownerName, userDetails.password, userDetails.mobileNo, userDetails.address);

                //Adding Eantries To the Sector Table
                var sectornamelist = db.sectorNames.ToList();

                var pfcode = (from u in db.Franchisees
                              where u.PF_Code == userDetails.Pfcode
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


                        if (sn.Sector_Name == " Within city")
                        {
                            sn.Priority = 1;
                            sn.Pincode_values = "400001-400610,400615-400706,400710-401203,401205-402209";

                            sn.CashD = true;
                            sn.CashN = true;
                            sn.BillD = true;
                            sn.BillN = true;

                        }

                        else if (sn.Sector_Name == " Within State")
                        {

                            sn.CashD = true;
                            sn.CashN = false;
                            sn.BillD = true;
                            sn.BillN = false;

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



                        else if (sn.Sector_Name == "Jammu and Kashmir")
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

                var Companyid = "Cash_" + userDetails.Pfcode;


                var secotrs = db.Sectors.Where(m => m.Pf_code == userDetails.Pfcode).ToList();

                Company cm = new Company();
                cm.Company_Id = Companyid;
                cm.Pf_code = userDetails.Pfcode;
                cm.Phone = 1234567890;
                cm.Company_Address = userDetails.address;
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
                    bs.Company_Address = userDetails.address;
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

                userDetailsModel user = new userDetailsModel();

                user.name = userDetails.franchiseName;
                user.email = userDetails.emailId;
                user.mobileNo = userDetails.mobileNo;
                user.address = userDetails.address;

                Session["DataName"] = user.name;
                Session["Dataemail"] = user.email; 
                Session["Datacontact"] = user.mobileNo; 
                Session["Dataaddress"] = user.address;

                if (save > 0)
                {
                    string FilePath = Server.MapPath("~/images/RegisterMailTemplete.html");
                        //"http://codetentacles-005-site1.htempurl.com/images/RegisterMailTemplete.html";// "D:\\MBK\\SendEmailByEmailTemplate\\EmailTemplates\\SignUp.html";
                    StreamReader str = new StreamReader(FilePath);
                    string MailText = str.ReadToEnd();
                    str.Close();

                    MailText = MailText.Replace("[newusername]", userDetails.franchiseName);
                    
                    string subject = "Welcome To FrBilling Subscription";

                   
                    MailMessage _mailmsg = new MailMessage();

                    
                    _mailmsg.IsBodyHtml = true;        
                    _mailmsg.From = new MailAddress("frbillingsoftware@gmail.com");    
                    _mailmsg.To.Add(userDetails.emailId);              
                    _mailmsg.Subject = subject;      
                    _mailmsg.Body = MailText;

                    SmtpClient _smtp = new SmtpClient();
 
                    _smtp.Host = "smtp.gmail.com";
 
                    _smtp.Port = 587;

                  
                    _smtp.EnableSsl = true;
                    _smtp.UseDefaultCredentials = false;
                    
                    NetworkCredential _network = new NetworkCredential("frbillingsoftware@gmail.com", "rqaynjbevkygswkx");
                    _smtp.Credentials = _network;

                    
                    //_smtp.Send(_mailmsg);

                    TempData["success"] = "Register successfully";
                    return RedirectToAction("makePaymentPartial", "Admin", user);
                    // var userAllDetails = db.registrations.Where(x => x.Pfcode == userDetails.Pfcode).FirstOrDefault();
                    // return Json(userAllDetails, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["error"] = "Something went wrong";
                }
            }

            return View("Register", userDetails);


        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ01234567899876543210";
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

                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "Invoice.rdlc");

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
                               && d.RenewalStatus==null
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
                MailText = MailText.Replace("[Date]",date1.ToString("MM/dd/yyyy"));
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

            return View();
        }

        public ActionResult PaymentSuccess()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult PaymentSuccess(string paymentSuccess="1")
        {
            string SubPath = "http://codetentacles-005-site1.htempurl.com/Admin/AdminLogin?isPaymentSuccess=1";
            return Redirect(SubPath);
        }
    }
}
