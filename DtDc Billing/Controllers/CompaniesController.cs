using System;
using System.Collections.Generic;
using System.Data;
using DtDc_Billing.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Entity_FR;

namespace DtDc_Billing.Controllers
{
    [SessionUserModule]
    public class CompaniesController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        // GET: Companies
        public ActionResult Index()
        {
            var companies = db.Companies.Include(c => c.Franchisee);
            return View(companies.ToList());
        }

        // GET: Companies/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "F_Address");
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Company_Id,c_id,Phone,Email,Insurance,Minimum_Risk_Charge,Other_Details,Fuel_Sur_Charge,Topay_Charge,Cod_Charge,Gec_Fuel_Sur_Charge,Pf_code,Company_Address,Company_Name")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "F_Address", company.Pf_code);
            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(string CompanyId)
        {

            TempData["CompanyId"] = CompanyId;

            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            return PartialView("Editdox", db.Ratems.Where(m => m.Company_id == CompanyId).ToList());
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Company_Id,c_id,Phone,Email,Insurance,Minimum_Risk_Charge,Other_Details,Fuel_Sur_Charge,Topay_Charge,Cod_Charge,Gec_Fuel_Sur_Charge,Pf_code,Company_Address,Company_Name")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "F_Address", company.Pf_code);
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
