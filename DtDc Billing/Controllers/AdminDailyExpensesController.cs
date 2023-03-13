using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
   

    public class AdminDailyExpensesController : Controller
    {
        private db_a71c08_elitetokenEntities db = new db_a71c08_elitetokenEntities();

       
        public ActionResult Add_Expenses()
        {

            var Cat = new List<SelectListItem>
    {
        new SelectListItem{ Text="Select", Value = "" },
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

            ViewBag.Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "F_Address");
            return View();
            
        }


        [HttpPost]
        public ActionResult Add_Expenses(ExpenseModel expense)
        {

            var Cat = new List<SelectListItem>
    {
        new SelectListItem{ Text="Select", Value = "" },
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


            if (ModelState.IsValid)
            {


                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
               
                Expense ex = new Expense();


                ex.Amount = expense.Amount;
                ex.Rason = expense.Rason;
                ex.Category = expense.Category;
                ex.Pf_Code = expense.Pf_Code;           
                ex.User_Id = expense.User_Id;
                ex.Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();
                ex.Datetime_Exp = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                //expense.User_Id= Convert.ToInt64(Session["EmpId"]);
                db.Expenses.Add(ex);
                db.SaveChanges();

                //////////Alert Afte Success///
                ViewBag.Success = " Added Successfully...!!!";
                ////////////////////////////////////////
                ModelState.Clear();
                return View();
            }

            ViewBag.Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "F_Address", expense.Pf_Code);
            return View(expense);
            
        }


        public ActionResult Add_Payment()
        {
            ViewBag.Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "F_Address");
            return View();
        }
        [HttpPost]
        public ActionResult Add_Payment(CashCounterPaymentModel payment)
        {
            if (ModelState.IsValid)
            {

                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
               
                Payment pay = new Payment();

                pay.Consignment_No = payment.Consignment_No;
                pay.amount = payment.amount;
                pay.Description_ = payment.Description_;
                pay.User_Id = payment.User_Id;
                pay.Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();
                pay.Datetime_Pay = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                //payment.User_Id = Convert.ToInt64(Session["EmpId"]);
                db.Payments.Add(pay);
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
                return View();
                
            }

            ViewBag.Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "F_Address", payment.Pf_Code);
            return View(payment);
        }


        public ActionResult Add_Saving()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add_Saving(CashCounterSavingModel saving)
        {

            if (ModelState.IsValid)
            {

                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                


                Saving Sav = new Saving();

                Sav.Saving_amount = saving.Saving_amount;
                Sav.Rason = saving.Rason;
                Sav.User_Id = saving.User_Id;
                Sav.Datetime_Sav = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                Sav.Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();

                db.Savings.Add(Sav);
                db.SaveChanges();
                //////////Alert Afte Success///
                ViewBag.Success = " Added Successfully...!!!";
                ////////////////////////////////////////
                ModelState.Clear();
                return View();
                
            }

           
            return View(saving);
            
        }




        [HttpGet]
        public JsonResult GetAllCreditReport()
        {

            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

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