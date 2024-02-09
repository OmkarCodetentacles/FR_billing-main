using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DtDc_Billing.CustomModel;
using System.Threading.Tasks;
using System.Net.Mail;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    [SessionUserModule]
    public class CustomerController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        [HttpGet]
        public ActionResult AddImage()
        {
            ViewBag.Current = db.popupimages.OrderByDescending(m => m.imageid).Select(m => m.ImagePath).Take(1).FirstOrDefault();

            return View();
        }


        [HttpPost]
        public ActionResult AddImage(HttpPostedFileBase file)
        {

            if (file != null)
            {
                string extension = System.IO.Path.GetExtension(file.FileName);

                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    string random = GetLetter();
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/Content/PopupImg/"), random + pic);
                    file.SaveAs(path);

                    string str = "http://admin.mexpress.co.in/Content/PopupImg/" + random + pic;

                    path.Replace("\\", "/");


                    popupimage popupimage = db.popupimages.OrderByDescending(m => m.imageid).Take(1).FirstOrDefault();

                    popupimage.ImagePath = str;
                    popupimage.Upload_Date = GetLocalTime.GetDateTime();

                    db.Entry(popupimage).State = EntityState.Modified;



                    db.SaveChanges();

                    ViewBag.Success = "Image Uploaded SuccessFully";
                }
                else
                {
                    ModelState.AddModelError("Error", "Please ImageFile jpg,jpeg OR png format");
                }

            }
            else
            {
                ModelState.AddModelError("Error", "Please Select Image");
            }

            ViewBag.Current = db.popupimages.OrderByDescending(m => m.imageid).Select(m => m.ImagePath).Take(1).FirstOrDefault();

            return View();
        }


        [HttpGet]
        public ActionResult DeleteAddImage()
        {

            popupimage popupimage = db.popupimages.Take(1).FirstOrDefault();

            popupimage.ImagePath = null;
            popupimage.Upload_Date = GetLocalTime.GetDateTime();

            db.Entry(popupimage).State = EntityState.Modified;

            db.SaveChanges();


            return RedirectToAction("AddImage");
        }



        public ActionResult Complaint()
        {

            return View(db.Complaints.ToList().OrderBy(m => m.C_Status));

        }


        public ActionResult ComplaintDetails(long Complaint_ID)
        {

            Complaint link = db.Complaints.Where(m => m.Complaint_ID == Complaint_ID).FirstOrDefault();

            ViewBag.replyadmin = db.ReplyAdmins.Where(m => m.Complaint_ID == Complaint_ID).ToList();
            return View(link);
        }


        public ActionResult Reply(ReplyAdmin replyAdmin)
        {
            db.ReplyAdmins.Add(replyAdmin);
            replyAdmin.AdminOrUser = "Admin";
            replyAdmin.Replydate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("ComplaintDetails", "Customer", new { @Complaint_ID = replyAdmin.Complaint_ID });
        }


        // GET : RegisterComplaint


        public ActionResult Addstatus(string C_Status, long Complaint_ID)
        {
            var comp = new Complaint() { Complaint_ID = Complaint_ID };
            comp.C_Status = C_Status;
            db.Complaints.Attach(comp);
            db.Entry(comp).Property(x => x.C_Status).IsModified = true;
            db.SaveChanges();
            return RedirectToAction("ComplaintDetails", "Customer", new { @Complaint_ID = Complaint_ID });
        }

        [HttpGet]
        public ActionResult Holidays()
        {

            return View(db.Holidays.ToList());
        }

        [HttpGet]
        public ActionResult AddHoliday()
        {
            var years = Enumerable
      .Range(DateTime.Now.Year, 15)
      .Select(year => new SelectListItem
      {
          Value = year.ToString(CultureInfo.InvariantCulture),
          Text = year.ToString(CultureInfo.InvariantCulture)
      });


            var months = DateTimeFormatInfo
      .InvariantInfo
      .MonthNames
      .TakeWhile(monthName => monthName != String.Empty)
      .Select((monthName, index) => new SelectListItem
      {
          Value = (index + 1).ToString(CultureInfo.InvariantCulture),
          Text = string.Format("({0}) {1}", index + 1, monthName)
      });
            List<SelectListItem> states = new List<SelectListItem>();
            //states.Add(new SelectListItem { Text = "Select", Value = "Select" });
            states.Add(new SelectListItem { Text = "ANDAMAN & NIKOBAR ISLANDS", Value = "ANDAMAN & NIKOBAR ISLANDS" });
            states.Add(new SelectListItem { Text = "ANDHRA PRADESH", Value = "ANDHRA PRADESH" });
            states.Add(new SelectListItem { Text = "ARUNACHAL PRADESH", Value = "ARUNACHAL PRADESH" });
            states.Add(new SelectListItem { Text = "ASSAM", Value = "ASSAM" });
            states.Add(new SelectListItem { Text = "BIHAR", Value = "BIHAR" });
            states.Add(new SelectListItem { Text = "CHANDIGARH", Value = "CHANDIGARH" });
            states.Add(new SelectListItem { Text = "CHHATTISGARH", Value = "CHHATTISGARH" });
            states.Add(new SelectListItem { Text = "DADRA & NAGAR HAVELI", Value = "DADRA & NAGAR HAVELI" });
            states.Add(new SelectListItem { Text = "DAMAN & DIU", Value = "DAMAN & DIU" });
            states.Add(new SelectListItem { Text = "GOA", Value = "GOA" });
            states.Add(new SelectListItem { Text = "GUJARAT", Value = "GUJARAT" });
            states.Add(new SelectListItem { Text = "HARYANA", Value = "HARYANA" });
            states.Add(new SelectListItem { Text = "HIMACHAL PRADESH", Value = "HIMACHAL PRADESH" });
            states.Add(new SelectListItem { Text = "JAMMU & KASHMIR", Value = "JAMMU & KASHMIR" });
            states.Add(new SelectListItem { Text = "JHARKHAND", Value = "JHARKHAND" });
            states.Add(new SelectListItem { Text = "KARNATAKA", Value = "KARNATAKA" });
            states.Add(new SelectListItem { Text = "KERALA", Value = "KERALA" });
            states.Add(new SelectListItem { Text = "LAKSHADWEEP", Value = "LAKSHADWEEP" });
            states.Add(new SelectListItem { Text = "MADHYA PRADESH", Value = "MADHYA PRADESH" });
            states.Add(new SelectListItem { Text = "MAHARASHTRA", Value = "MAHARASHTRA" });
            states.Add(new SelectListItem { Text = "MANIPUR", Value = "MANIPUR" });
            states.Add(new SelectListItem { Text = "MEGHALAYA", Value = "MEGHALAYA" });
            states.Add(new SelectListItem { Text = "MIZORAM", Value = "MIZORAM" });
            states.Add(new SelectListItem { Text = "NAGALAND", Value = "NAGALAND" });
            states.Add(new SelectListItem { Text = "NCT OF DELHI", Value = "NCT OF DELHI" });
            states.Add(new SelectListItem { Text = "ORISSA", Value = "ORISSA" });
            states.Add(new SelectListItem { Text = "PUDUCHERRY", Value = "PUDUCHERRY" });
            states.Add(new SelectListItem { Text = "PUNJAB", Value = "PUNJAB" });
            states.Add(new SelectListItem { Text = "RAJASTHAN", Value = "RAJASTHAN" });
            states.Add(new SelectListItem { Text = "SIKKIM", Value = "SIKKIM" });
            states.Add(new SelectListItem { Text = "TAMIL NADU", Value = "TAMIL NADU" });
            states.Add(new SelectListItem { Text = "TRIPURA", Value = "TRIPURA" });
            states.Add(new SelectListItem { Text = "UTTAR PRADESH", Value = "UTTAR PRADESH" });
            states.Add(new SelectListItem { Text = "UTTARAKHAND", Value = "UTTARAKHAND" });
            states.Add(new SelectListItem { Text = "WEST BENGAL", Value = "WEST BENGAL" });



            ViewBag.H_Month = months;
            ViewBag.H_Year = years;
            ViewBag.State = states;

            return View();
        }

        [HttpPost]
        public ActionResult AddHoliday(Holiday holiday)
        {

            List<SelectListItem> years = Enumerable
      .Range(DateTime.Now.Year, 15)
      .Select(year => new SelectListItem
      {
          Value = year.ToString(CultureInfo.InvariantCulture),
          Text = year.ToString(CultureInfo.InvariantCulture)
      }).ToList();

            var selectedyear = years.Where(x => x.Value == holiday.H_Year.ToString()).First();
            selectedyear.Selected = true;

            List<SelectListItem> months = DateTimeFormatInfo
      .InvariantInfo
      .MonthNames
      .TakeWhile(monthName => monthName != String.Empty)
      .Select((monthName, index) => new SelectListItem
      {
          Value = (index + 1).ToString(CultureInfo.InvariantCulture),
          Text = string.Format("({0}) {1}", index + 1, monthName)
      }).ToList();

            var selectedmonth = months.Where(x => x.Value == holiday.H_Month).First();
            selectedmonth.Selected = true;


            List<SelectListItem> states = new List<SelectListItem>();
            //states.Add(new SelectListItem { Text = "Select", Value = "Select" });
            states.Add(new SelectListItem { Text = "ANDAMAN & NIKOBAR ISLANDS", Value = "ANDAMAN & NIKOBAR ISLANDS" });
            states.Add(new SelectListItem { Text = "ANDHRA PRADESH", Value = "ANDHRA PRADESH" });
            states.Add(new SelectListItem { Text = "ARUNACHAL PRADESH", Value = "ARUNACHAL PRADESH" });
            states.Add(new SelectListItem { Text = "ASSAM", Value = "ASSAM" });
            states.Add(new SelectListItem { Text = "BIHAR", Value = "BIHAR" });
            states.Add(new SelectListItem { Text = "CHANDIGARH", Value = "CHANDIGARH" });
            states.Add(new SelectListItem { Text = "CHHATTISGARH", Value = "CHHATTISGARH" });
            states.Add(new SelectListItem { Text = "DADRA & NAGAR HAVELI", Value = "DADRA & NAGAR HAVELI" });
            states.Add(new SelectListItem { Text = "DAMAN & DIU", Value = "DAMAN & DIU" });
            states.Add(new SelectListItem { Text = "GOA", Value = "GOA" });
            states.Add(new SelectListItem { Text = "GUJARAT", Value = "GUJARAT" });
            states.Add(new SelectListItem { Text = "HARYANA", Value = "HARYANA" });
            states.Add(new SelectListItem { Text = "HIMACHAL PRADESH", Value = "HIMACHAL PRADESH" });
            states.Add(new SelectListItem { Text = "JAMMU & KASHMIR", Value = "JAMMU & KASHMIR" });
            states.Add(new SelectListItem { Text = "JHARKHAND", Value = "JHARKHAND" });
            states.Add(new SelectListItem { Text = "KARNATAKA", Value = "KARNATAKA" });
            states.Add(new SelectListItem { Text = "KERALA", Value = "KERALA" });
            states.Add(new SelectListItem { Text = "LAKSHADWEEP", Value = "LAKSHADWEEP" });
            states.Add(new SelectListItem { Text = "MADHYA PRADESH", Value = "MADHYA PRADESH" });
            states.Add(new SelectListItem { Text = "MAHARASHTRA", Value = "MAHARASHTRA" });
            states.Add(new SelectListItem { Text = "MANIPUR", Value = "MANIPUR" });
            states.Add(new SelectListItem { Text = "MEGHALAYA", Value = "MEGHALAYA" });
            states.Add(new SelectListItem { Text = "MIZORAM", Value = "MIZORAM" });
            states.Add(new SelectListItem { Text = "NAGALAND", Value = "NAGALAND" });
            states.Add(new SelectListItem { Text = "NCT OF DELHI", Value = "NCT OF DELHI" });
            states.Add(new SelectListItem { Text = "ORISSA", Value = "ORISSA" });
            states.Add(new SelectListItem { Text = "PUDUCHERRY", Value = "PUDUCHERRY" });
            states.Add(new SelectListItem { Text = "PUNJAB", Value = "PUNJAB" });
            states.Add(new SelectListItem { Text = "RAJASTHAN", Value = "RAJASTHAN" });
            states.Add(new SelectListItem { Text = "SIKKIM", Value = "SIKKIM" });
            states.Add(new SelectListItem { Text = "TAMIL NADU", Value = "TAMIL NADU" });
            states.Add(new SelectListItem { Text = "TRIPURA", Value = "TRIPURA" });
            states.Add(new SelectListItem { Text = "UTTAR PRADESH", Value = "UTTAR PRADESH" });
            states.Add(new SelectListItem { Text = "UTTARAKHAND", Value = "UTTARAKHAND" });
            states.Add(new SelectListItem { Text = "WEST BENGAL", Value = "WEST BENGAL" });

            var selectedstate = states.Where(x => x.Value == holiday.State).First();
            selectedstate.Selected = true;


            ViewBag.H_Month = months;
            ViewBag.H_Year = years;
            ViewBag.State = states;

            if (ModelState.IsValid)
            {

                db.Holidays.Add(holiday);
                //holiday.H_datetime = holiday.H_datetime.Value.Date;
                holiday.H_datetime = DateTime.Now;
                db.SaveChanges();
                TempData["Success"] = "Holiday Added SuccessFully";
                return RedirectToAction("Holidays");
            }
            return View(holiday);

        }


        public ActionResult EditHoliday(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Holiday holiday = db.Holidays.Find(id);

            if (holiday == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> years = Enumerable
      .Range(DateTime.Now.Year, 15)
      .Select(year => new SelectListItem
      {
          Value = year.ToString(CultureInfo.InvariantCulture),
          Text = year.ToString(CultureInfo.InvariantCulture)
      }).ToList();

            var selectedyear = years.Where(x => x.Value == holiday.H_Year.ToString()).First();
            selectedyear.Selected = true;

            List<SelectListItem> months = DateTimeFormatInfo
      .InvariantInfo
      .MonthNames
      .TakeWhile(monthName => monthName != String.Empty)
      .Select((monthName, index) => new SelectListItem
      {
          Value = (index + 1).ToString(CultureInfo.InvariantCulture),
          Text = string.Format("({0}) {1}", index + 1, monthName)
      }).ToList();

            var selectedmonth = months.Where(x => x.Value == holiday.H_Month).First();
            selectedmonth.Selected = true;


            List<SelectListItem> states = new List<SelectListItem>();
            states.Add(new SelectListItem { Text = "Select", Value = "Select" });
            states.Add(new SelectListItem { Text = "ANDAMAN & NIKOBAR ISLANDS", Value = "ANDAMAN & NIKOBAR ISLANDS" });
            states.Add(new SelectListItem { Text = "ANDHRA PRADESH", Value = "ANDHRA PRADESH" });
            states.Add(new SelectListItem { Text = "ARUNACHAL PRADESH", Value = "ARUNACHAL PRADESH" });
            states.Add(new SelectListItem { Text = "ASSAM", Value = "ASSAM" });
            states.Add(new SelectListItem { Text = "BIHAR", Value = "BIHAR" });
            states.Add(new SelectListItem { Text = "CHANDIGARH", Value = "CHANDIGARH" });
            states.Add(new SelectListItem { Text = "CHHATTISGARH", Value = "CHHATTISGARH" });
            states.Add(new SelectListItem { Text = "DADRA & NAGAR HAVELI", Value = "DADRA & NAGAR HAVELI" });
            states.Add(new SelectListItem { Text = "DAMAN & DIU", Value = "DAMAN & DIU" });
            states.Add(new SelectListItem { Text = "GOA", Value = "GOA" });
            states.Add(new SelectListItem { Text = "GUJARAT", Value = "GUJARAT" });
            states.Add(new SelectListItem { Text = "HARYANA", Value = "HARYANA" });
            states.Add(new SelectListItem { Text = "HIMACHAL PRADESH", Value = "HIMACHAL PRADESH" });
            states.Add(new SelectListItem { Text = "JAMMU & KASHMIR", Value = "JAMMU & KASHMIR" });
            states.Add(new SelectListItem { Text = "JHARKHAND", Value = "JHARKHAND" });
            states.Add(new SelectListItem { Text = "KARNATAKA", Value = "KARNATAKA" });
            states.Add(new SelectListItem { Text = "KERALA", Value = "KERALA" });
            states.Add(new SelectListItem { Text = "LAKSHADWEEP", Value = "LAKSHADWEEP" });
            states.Add(new SelectListItem { Text = "MADHYA PRADESH", Value = "MADHYA PRADESH" });
            states.Add(new SelectListItem { Text = "MAHARASHTRA", Value = "MAHARASHTRA" });
            states.Add(new SelectListItem { Text = "MANIPUR", Value = "MANIPUR" });
            states.Add(new SelectListItem { Text = "MEGHALAYA", Value = "MEGHALAYA" });
            states.Add(new SelectListItem { Text = "MIZORAM", Value = "MIZORAM" });
            states.Add(new SelectListItem { Text = "NAGALAND", Value = "NAGALAND" });
            states.Add(new SelectListItem { Text = "NCT OF DELHI", Value = "NCT OF DELHI" });
            states.Add(new SelectListItem { Text = "ORISSA", Value = "ORISSA" });
            states.Add(new SelectListItem { Text = "PUDUCHERRY", Value = "PUDUCHERRY" });
            states.Add(new SelectListItem { Text = "PUNJAB", Value = "PUNJAB" });
            states.Add(new SelectListItem { Text = "RAJASTHAN", Value = "RAJASTHAN" });
            states.Add(new SelectListItem { Text = "SIKKIM", Value = "SIKKIM" });
            states.Add(new SelectListItem { Text = "TAMIL NADU", Value = "TAMIL NADU" });
            states.Add(new SelectListItem { Text = "TRIPURA", Value = "TRIPURA" });
            states.Add(new SelectListItem { Text = "UTTAR PRADESH", Value = "UTTAR PRADESH" });
            states.Add(new SelectListItem { Text = "UTTARAKHAND", Value = "UTTARAKHAND" });
            states.Add(new SelectListItem { Text = "WEST BENGAL", Value = "WEST BENGAL" });

            var selectedstate = states.Where(x => x.Value == holiday.State).First();
            selectedstate.Selected = true;


            ViewBag.H_Month = months;
            ViewBag.H_Year = years;
            ViewBag.State = states;


            return View(holiday);
        }

        // POST: Holidays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult EditHoliday([Bind(Include = "Holiday_ID,H_Year,State,H_Month,H_datetime,Holiday_Name")] Holiday holiday)
        {
            List<SelectListItem> years = Enumerable
      .Range(DateTime.Now.Year, 15)
      .Select(year => new SelectListItem
      {
          Value = year.ToString(CultureInfo.InvariantCulture),
          Text = year.ToString(CultureInfo.InvariantCulture)
      }).ToList();

            var selectedyear = years.Where(x => x.Value == holiday.H_Year.ToString()).First();
            selectedyear.Selected = true;

            List<SelectListItem> months = DateTimeFormatInfo
      .InvariantInfo
      .MonthNames
      .TakeWhile(monthName => monthName != String.Empty)
      .Select((monthName, index) => new SelectListItem
      {
          Value = (index + 1).ToString(CultureInfo.InvariantCulture),
          Text = string.Format("({0}) {1}", index + 1, monthName)
      }).ToList();

            var selectedmonth = months.Where(x => x.Value == holiday.H_Month).First();
            selectedmonth.Selected = true;


            List<SelectListItem> states = new List<SelectListItem>();
            states.Add(new SelectListItem { Text = "Select", Value = "Select" });
            states.Add(new SelectListItem { Text = "ANDAMAN & NIKOBAR ISLANDS", Value = "ANDAMAN & NIKOBAR ISLANDS" });
            states.Add(new SelectListItem { Text = "ANDHRA PRADESH", Value = "ANDHRA PRADESH" });
            states.Add(new SelectListItem { Text = "ARUNACHAL PRADESH", Value = "ARUNACHAL PRADESH" });
            states.Add(new SelectListItem { Text = "ASSAM", Value = "ASSAM" });
            states.Add(new SelectListItem { Text = "BIHAR", Value = "BIHAR" });
            states.Add(new SelectListItem { Text = "CHANDIGARH", Value = "CHANDIGARH" });
            states.Add(new SelectListItem { Text = "CHHATTISGARH", Value = "CHHATTISGARH" });
            states.Add(new SelectListItem { Text = "DADRA & NAGAR HAVELI", Value = "DADRA & NAGAR HAVELI" });
            states.Add(new SelectListItem { Text = "DAMAN & DIU", Value = "DAMAN & DIU" });
            states.Add(new SelectListItem { Text = "GOA", Value = "GOA" });
            states.Add(new SelectListItem { Text = "GUJARAT", Value = "GUJARAT" });
            states.Add(new SelectListItem { Text = "HARYANA", Value = "HARYANA" });
            states.Add(new SelectListItem { Text = "HIMACHAL PRADESH", Value = "HIMACHAL PRADESH" });
            states.Add(new SelectListItem { Text = "JAMMU & KASHMIR", Value = "JAMMU & KASHMIR" });
            states.Add(new SelectListItem { Text = "JHARKHAND", Value = "JHARKHAND" });
            states.Add(new SelectListItem { Text = "KARNATAKA", Value = "KARNATAKA" });
            states.Add(new SelectListItem { Text = "KERALA", Value = "KERALA" });
            states.Add(new SelectListItem { Text = "LAKSHADWEEP", Value = "LAKSHADWEEP" });
            states.Add(new SelectListItem { Text = "MADHYA PRADESH", Value = "MADHYA PRADESH" });
            states.Add(new SelectListItem { Text = "MAHARASHTRA", Value = "MAHARASHTRA" });
            states.Add(new SelectListItem { Text = "MANIPUR", Value = "MANIPUR" });
            states.Add(new SelectListItem { Text = "MEGHALAYA", Value = "MEGHALAYA" });
            states.Add(new SelectListItem { Text = "MIZORAM", Value = "MIZORAM" });
            states.Add(new SelectListItem { Text = "NAGALAND", Value = "NAGALAND" });
            states.Add(new SelectListItem { Text = "NCT OF DELHI", Value = "NCT OF DELHI" });
            states.Add(new SelectListItem { Text = "ORISSA", Value = "ORISSA" });
            states.Add(new SelectListItem { Text = "PUDUCHERRY", Value = "PUDUCHERRY" });
            states.Add(new SelectListItem { Text = "PUNJAB", Value = "PUNJAB" });
            states.Add(new SelectListItem { Text = "RAJASTHAN", Value = "RAJASTHAN" });
            states.Add(new SelectListItem { Text = "SIKKIM", Value = "SIKKIM" });
            states.Add(new SelectListItem { Text = "TAMIL NADU", Value = "TAMIL NADU" });
            states.Add(new SelectListItem { Text = "TRIPURA", Value = "TRIPURA" });
            states.Add(new SelectListItem { Text = "UTTAR PRADESH", Value = "UTTAR PRADESH" });
            states.Add(new SelectListItem { Text = "UTTARAKHAND", Value = "UTTARAKHAND" });
            states.Add(new SelectListItem { Text = "WEST BENGAL", Value = "WEST BENGAL" });

            var selectedstate = states.Where(x => x.Value == holiday.State).First();
            selectedstate.Selected = true;


            ViewBag.H_Month = months;
            ViewBag.H_Year = years;
            ViewBag.State = states;


            if (ModelState.IsValid)
            {
                db.Entry(holiday).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Holiday Updated SuccessFully";

                return RedirectToAction("Holidays");
            }
            return View(holiday);
        }



        public ActionResult DeleteHoliday(long id)
        {
            Holiday holiday = db.Holidays.Find(id);
            db.Holidays.Remove(holiday);
            db.SaveChanges();
            TempData["Success"] = "Holiday Deleted SuccessFully";
            return RedirectToAction("Holidays");
        }


        public static string GetLetter()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }


        public JsonResult GetComplaints()
        {

            db.Configuration.ProxyCreationEnabled = false;

            var Complaints = db.Complaints.Where(m => m.C_Status == "open").OrderByDescending(m => m.Complaint_ID).ToList();

            return Json(Complaints, JsonRequestBehavior.AllowGet);

        }

        ///////////////////// Raising Issue added on date:-11/04/22



        [HttpGet]
        public ActionResult RaiseIssue()
        {

            return View();
        }

        [HttpPost]
        public ActionResult RaiseIssue(DtDc_Billing.CustomModel.AddRaiseIssueModel raise)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    string _FileName = "";
                    string _path = "";

                    if (raise.file.ContentLength > 0)
                    {
                        _FileName = Path.GetFileName(raise.file.FileName);
                        _path = Server.MapPath("~/UploadedFiles/") + _FileName;
                        raise.file.SaveAs(_path);
                    }

                    RaiseIssue ra = new RaiseIssue();

                    ra.RaiseIssue_Name = raise.RaiseIssue_Name;
                    ra.Domain_Name = raise.Domain_Name;
                    ra.RaiseIssue_datetime = DateTime.Now.Date;
                    ra.RaiseIssue_Description = raise.RaiseIssue_Description;
                    ra.RaiseIssue_ScreenShotFile = "/UploadedFiles/" + _FileName;
                    ra.RaiseIssue_FileName = _FileName;
                    ra.RaiseIssue_Status = 0;

                    db.RaiseIssues.Add(ra);

                    //////////// ----- Mail code Added on 11-04-22

                    using (MailMessage mm = new MailMessage("frbillingsoftware@gmail.com", "swapnilcodetentacles@gmail.com"))
                    {

                        string fileName = Path.GetFileName(raise.file.FileName);
                        mm.Attachments.Add(new Attachment(raise.file.InputStream, fileName));

                        mm.Subject = "Issue Raised";
                        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                        var stringChars = new char[6];
                        var random = new Random();

                        for (int i = 0; i < stringChars.Length; i++)
                        {
                            stringChars[i] = chars[random.Next(chars.Length)];
                        }

                        var Tokne = new String(stringChars);


                        string Bodytext = "<html><body><h1>Your Issue is Raised</h1><br/> RaiseIssue Name= " + raise.RaiseIssue_Name + "<br/>Domain Name = " + raise.Domain_Name + "<br/> RaiseIssue Description= " + raise.RaiseIssue_Description + "</body></html>";

                        mm.IsBodyHtml = true;



                        mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                        AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                        // mm.Body = Bodytext;
                        mm.Body = Bodytext;

                        //Add Byte array as Attachment.

                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        smtp.Port = 587;
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                        smtp.UseDefaultCredentials = false;
                        credentials.UserName = "frbillingsoftware@gmail.com";
                        credentials.Password = "frbcodetentacles@123";


                        //smtp.UseDefaultCredentials = true;

                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Credentials = credentials;

                        //smtp.Send(mm);


                        db.SaveChanges();

                        ViewBag.Message = "File Uploaded and Raise Issue Mail send to Your Emailid !!";
                        ModelState.Clear();
                        return View();


                    }

                    //return RedirectToAction("RaiseIssue");
                }
            }

            catch (Exception ex)
            {
                // ViewBag.Message = "File Not uploaded and Raise Issue failed!!";
                ViewBag.Message = ex.Message;
            }
            return View(raise);
        }



        [HttpGet]
        public ActionResult RaiseIssueStatus()
        {

            return View(db.RaiseIssues.ToList());
        }


        [HttpGet]
        public ActionResult AdminSideRaiseIssueStatus()
        {

            return View(db.RaiseIssues.ToList());
        }

        public ActionResult EditRaiseIssue(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RaiseIssue raise = new RaiseIssue();

            RaiseIssue raiseIssue = db.RaiseIssues.Find(id);

            if (raiseIssue == null)
            {
                return HttpNotFound();
            }


            List<SelectListItem> RaiseIssue_Status = new List<SelectListItem>();


            RaiseIssue_Status.Add(new SelectListItem { Text = "Pending Issue", Value = "0" });
            RaiseIssue_Status.Add(new SelectListItem { Text = "Issue Resolved", Value = "1" });

            var data = (from d in db.RaiseIssues
                        where d.RaiseIssue_ID == id
                        select new { d.RaiseIssue_Status }).First();


            foreach (var item in RaiseIssue_Status)
            {

                if (data.RaiseIssue_Status == Convert.ToInt32(item.Value))
                {
                    item.Selected = true;
                }

            }

            ViewBag.Status = RaiseIssue_Status;


            return View(raiseIssue);
        }

        // POST: RaiseIssues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult EditRaiseIssue(RaiseIssue raise, int Status)
        {



            string _FileName = "";
            string _path = "";

            //if (raise.file != null)
            //{
            //    _FileName = Path.GetFileName(raise.file.FileName);
            //    _path = Server.MapPath("~/UploadedFiles/") + _FileName;
            //    raise.file.SaveAs(_path);


            //    raise.RaiseIssue_ScreenShotFile = "/UploadedFiles/" + _FileName;
            //    raise.RaiseIssue_FileName = _FileName;

            //}

            raise.RaiseIssue_Status = Status;

            db.Entry(raise).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = "Raise Issue Updated SuccessFully";

            return RedirectToAction("AdminSideRaiseIssueStatus");


        }



        public ActionResult DeleteRaiseIssue(long id)
        {
            RaiseIssue raiseIssue = db.RaiseIssues.Find(id);
            db.RaiseIssues.Remove(raiseIssue);
            db.SaveChanges();
            TempData["Success"] = "Raise Issues Deleted SuccessFully";
            return RedirectToAction("AdminSideRaiseIssueStatus");
        }




        /////////////////////////////////////
        ///
        ////// Display Notification Added on 14-4-22
        [HttpGet]
        public ActionResult Notification()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Notification(DtDc_Billing.CustomModel.AddNotification notification)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    Notification note = new Notification();

                    note.Message = notification.Message;
                    note.url_path = notification.url_path;
                    note.dateN = notification.dateN;
                    note.description = notification.description;
                    note.Status = true;
                 

                    db.Notifications.Add(note);
                    db.SaveChanges();

                        ViewBag.Message = "Notification Added Successfully!!";
                        ModelState.Clear();
                        return View();


                    }

                }
           

            catch (Exception ex)
            {
               
                ViewBag.Message = ex.Message;
            }
            return View(notification);
        }


        [HttpGet]
        public ActionResult AssignModuleToUser()
        {
            List<SelectListItem> UserName = new List<SelectListItem>();
            List<SelectListItem> UserType = new List<SelectListItem>();

            var username = (from d in db.Users
                                select new { d.Name,d.Usertype, d.User_Id}).ToList();

            foreach(var item in username)
            { 
              UserName.Add(new SelectListItem { Text = item.Name, Value = Convert.ToString(item.User_Id) });
              UserType.Add(new SelectListItem { Text = item.Usertype, Value = Convert.ToString(item.User_Id) });
            }

            ViewBag.UserName = UserName;
          
            ViewBag.UserType = UserType;


            return View();
        }

    }
}
