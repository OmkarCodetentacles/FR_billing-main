using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Models;

namespace DtDc_Billing.Controllers
{
    [SessionUserModule]
    public class EmpStationeryController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Stationery
        public ActionResult Add()
        {
           // ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View();
        }


        [HttpPost]
        public ActionResult Add(Stationary stationary)
        {
            
            if (ModelState.IsValid)
            {

                stationary.Pf_code = Session["PfID"].ToString();
                db.Stationaries.Add(stationary);
                db.SaveChanges();

                //ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
                ViewBag.Message = "Stationary Added SuccessFully";
                ModelState.Clear();
                return View();
            }
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                   
                }
            }
            //ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);

            return View(stationary);
        }
    }
}