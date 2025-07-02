using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using DtDc_Billing.Models;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Entity_FR;

namespace DtDc_Billing.Controllers
{
    [SessionUserModule]
    public class demofirmController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        // GET: demofirm
        public ActionResult Index()
        {
            return View(db.FirmDetails.ToList());
        }

        // GET: demofirm/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FirmDetail firmDetail = db.FirmDetails.Find(id);
            if (firmDetail == null)
            {
                return HttpNotFound();
            }
            return View(firmDetail);
        }

        // GET: demofirm/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: demofirm/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Firm_Id,Firm_Name")] FirmDetail firmDetail)
        {
            if (ModelState.IsValid)
            {
                db.FirmDetails.Add(firmDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(firmDetail);
        }

        // GET: demofirm/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FirmDetail firmDetail = db.FirmDetails.Find(id);
            if (firmDetail == null)
            {
                return HttpNotFound();
            }
            return View(firmDetail);
        }

        // POST: demofirm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Firm_Id,Firm_Name")] FirmDetail firmDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(firmDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(firmDetail);
        }

        // GET: demofirm/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FirmDetail firmDetail = db.FirmDetails.Find(id);
            if (firmDetail == null)
            {
                return HttpNotFound();
            }
            return View(firmDetail);
        }

        // POST: demofirm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            FirmDetail firmDetail = db.FirmDetails.Find(id);
            db.FirmDetails.Remove(firmDetail);
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
