using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using DtDc_Billing.Models;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Entity_FR;

namespace DtDc_Billing.Controllers
{
    [SessionUserModule]
    public class DestinationsController : Controller
    {
        private db_a71c08_elitetokenEntities db = new db_a71c08_elitetokenEntities();

        // GET: Destinations
        public ActionResult Index()
        {
            return View(db.Destinations.ToList());
        }

        // GET: Destinations/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination destination = db.Destinations.Find(id);
            if (destination == null)
            {
                return HttpNotFound();
            }
            return View(destination);
        }

        // GET: Destinations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Destinations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Destination destination)
        {
            if (ModelState.IsValid)
            {
                destination.Pincode = destination.Pincode.Trim();

                var abc = db.Destinations.Where(m => m.Pincode == destination.Pincode).FirstOrDefault();

                if(abc == null)
                {
                    db.Destinations.Add(destination);
                    db.SaveChanges();
                    
                }
                else
                {
                    db.Entry(abc).State = EntityState.Detached;

                    destination.Dest_Id = abc.Dest_Id;
                    db.Entry(destination).State = EntityState.Modified;

                    db.SaveChanges();
                }

                TempData["Success"] = "Destination Added SuccessFully";

                return RedirectToAction("Destinations", "BillingReports");

                

            }

            if (!ModelState.IsValid)
                TempData["ViewData"] = ViewData;

            return RedirectToAction("Destinations", "BillingReports");
        }

        // GET: Destinations/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination destination = db.Destinations.Find(id);
            if (destination == null)
            {
                return HttpNotFound();
            }
            return View(destination);
        }

        // POST: Destinations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Dest_Id,Pincode,Name,State_")] Destination destination)
        {
            if (ModelState.IsValid)
            {
                db.Entry(destination).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(destination);
        }

        // GET: Destinations/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Destination destination = db.Destinations.Find(id);
            if (destination == null)
            {
                return HttpNotFound();
            }
            return View(destination);
        }

        // POST: Destinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Destination destination = db.Destinations.Find(id);
            db.Destinations.Remove(destination);
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
