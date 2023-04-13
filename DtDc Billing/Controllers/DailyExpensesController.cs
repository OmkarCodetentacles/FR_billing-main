using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionTimeout]
    [SessionUserModule]
    public class DailyExpensesController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        [SessionUserModule]
        public ActionResult Add_Expenses()
        {
            ViewBag.Pf_Code = new SelectList(db.Franchisees, "PF_Code", "F_Address");
            return View();
            
        }


        [HttpPost]
        public ActionResult Add_Expenses(Expense expense)
        {

            if (ModelState.IsValid)
            {

                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                expense.Datetime_Exp  = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);


                expense.Pf_Code = Session["pfCode"].ToString();
                expense.User_Id= Convert.ToInt64(Session["EmpId"]);
                db.Expenses.Add(expense);
                db.SaveChanges();

                //////////Alert Afte Success///
                ViewBag.Success = " Added Successfully...!!!";
                ////////////////////////////////////////
                ModelState.Clear();
                return View(new Expense());
            }

            ViewBag.Pf_Code = new SelectList(db.Franchisees, "PF_Code", "F_Address", expense.Pf_Code);
            return View(expense);
            
        }


        public ActionResult Add_Payment()
        {
            ViewBag.Pf_Code = new SelectList(db.Franchisees, "PF_Code", "F_Address");
            return View();
        }
        [HttpPost]
        public ActionResult Add_Payment(Payment payment)
        {
            if (ModelState.IsValid)
            {

                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                payment.Datetime_Pay = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                payment.Pf_Code = Session["pfCode"].ToString();
                payment.User_Id = Convert.ToInt64(Session["EmpId"]);
                db.Payments.Add(payment);
                db.SaveChanges();
                //////////Alert Afte Success///
                ViewBag.Success = " Added Successfully...!!!";
                ////////////////////////////////////////
                
                var update = db.Receipt_details.Where(m => m.Consignment_No == payment.Consignment_No).FirstOrDefault();
                update.Paid_Amount = update.Paid_Amount + payment.amount;
                db.Receipt_details.Attach(update);

                db.Entry(update).Property(x => x.Paid_Amount).IsModified = true;
                db.SaveChanges();
                ModelState.Clear();
                return View(new Payment());
                
            }

            ViewBag.Pf_Code = new SelectList(db.Franchisees, "PF_Code", "F_Address", payment.Pf_Code);
            return View(payment);
        }


        public ActionResult Add_Saving()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add_Saving(Saving saving)
        {

            if (ModelState.IsValid)
            {

                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                saving.Datetime_Sav = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                saving.Pf_Code = Session["pfCode"].ToString();
                saving.User_Id = Convert.ToInt64(Session["EmpId"]);

                db.Savings.Add(saving);
                db.SaveChanges();
                //////////Alert Afte Success///
                ViewBag.Success = " Added Successfully...!!!";
                ////////////////////////////////////////
                ModelState.Clear();
                return View(new Saving());
                
            }

           
            return View(saving);
            
        }




        [HttpGet]
        public JsonResult GetAllCreditReport()
        {

            string pfcode = Session["pfCode"].ToString();

            var entity = db.Receipt_details.Where(m => m.Pf_Code == pfcode && m.Paid_Amount < m.Charges_Total).OrderByDescending(m => m.Datetime_Cons).
    Select(e => new
      {
          e.Consignment_No,
          e.Destination,
          e.sender_phone,
          e.Sender,
          e.SenderCity,
          e.SenderPincode,
          e.Reciepents_phone,
          e.Reciepents,
          e.ReciepentsPincode,
          e.Paid_Amount,
          e.Charges_Total

      }).ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

    }
}