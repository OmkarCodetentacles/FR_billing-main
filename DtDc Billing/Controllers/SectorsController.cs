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
    public class SectorsController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        // GET: Sectors
        public ActionResult Index()
        {
            var sectors = db.Sectors.Include(s => s.Franchisee);
            return View(sectors.ToList());
        }

        public ActionResult Add_SectorPin()
        {
            string Pf = "PF12343"; /*Session["PfID"].ToString();*/


            List<Sector> st = (from u in db.Sectors
                               where u.Pf_code == Pf
                               && u.Pincode_values == null
                               select u).ToList();

            return View(st);
        }
        [HttpPost]
        public ActionResult Add_SectorPin(FormCollection fc)
        {
            string Pf = "PF12343";

            var sectoridarray = fc.GetValues("item.Sector_Id");
            var pincodearayy = fc.GetValues("item.Pincode_values");


            for (int i = 0; i < sectoridarray.Count(); i++)
            {

                Sector str = db.Sectors.Find(Convert.ToInt16(sectoridarray[i]));

                if (pincodearayy[i] == "")
                {
                    pincodearayy[i] = null;
                }

               
                str.Pincode_values = pincodearayy[i];
                db.Entry(str).State = EntityState.Modified;
               
            }

            int result = pincodearayy.Count(s => s == null);

            if (result >0)
            {
                ModelState.AddModelError("PinError", "All Fields Are Compulsary");

                List<Sector> stt = (from u in db.Sectors
                                   where u.Pf_code == Pf
                                   && u.Pincode_values == null
                                   select u).ToList();
                return View(stt);
            }
            else
            {
                db.SaveChanges();
            }


                List<Sector> st = (from u in db.Sectors
                               where u.Pf_code == Pf
                              
                               select u).ToList();

            return View(st);
        }



        // GET: Sectors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sector sector = db.Sectors.Find(id);
            if (sector == null)
            {
                return HttpNotFound();
            }
            return View(sector);
        }

        // GET: Sectors/Create
        public ActionResult Create()
        {
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "F_Address");
            return View();
        }

        // POST: Sectors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sector_Id,Sector_Name,Pf_code,Pincode_values")] Sector sector)
        {
            if (ModelState.IsValid)
            {
                db.Sectors.Add(sector);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "F_Address", sector.Pf_code);
            return View(sector);
        }

        // GET: Sectors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sector sector = db.Sectors.Find(id);
            if (sector == null)
            {
                return HttpNotFound();
            }
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "F_Address", sector.Pf_code);
            return View(sector);
        }

        // POST: Sectors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Sector_Id,Sector_Name,Pf_code,Pincode_values")] Sector sector)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sector).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "F_Address", sector.Pf_code);
            return View(sector);
        }

        // GET: Sectors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sector sector = db.Sectors.Find(id);
            if (sector == null)
            {
                return HttpNotFound();
            }
            return View(sector);
        }

        // POST: Sectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sector sector = db.Sectors.Find(id);
            db.Sectors.Remove(sector);
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
