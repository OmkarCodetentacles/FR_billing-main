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
    public class sectorNamesController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        // GET: sectorNames
        public ActionResult Index()
        {
            return View(db.sectorNames.ToList());
        }

        // GET: sectorNames/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sectorName sectorName = db.sectorNames.Find(id);
            if (sectorName == null)
            {
                return HttpNotFound();
            }
            return View(sectorName);
        }

        // GET: sectorNames/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: sectorNames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sname_id,sname")] sectorName sectorName)
        {
            if (ModelState.IsValid)
            {
                db.sectorNames.Add(sectorName);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sectorName);
        }

        // GET: sectorNames/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sectorName sectorName = db.sectorNames.Find(id);
            if (sectorName == null)
            {
                return HttpNotFound();
            }
            return View(sectorName);
        }

        // POST: sectorNames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sname_id,sname")] sectorName sectorName)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sectorName).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sectorName);
        }

        // GET: sectorNames/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sectorName sectorName = db.sectorNames.Find(id);
            if (sectorName == null)
            {
                return HttpNotFound();
            }
            return View(sectorName);
        }

        // POST: sectorNames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sectorName sectorName = db.sectorNames.Find(id);
            db.sectorNames.Remove(sectorName);
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
