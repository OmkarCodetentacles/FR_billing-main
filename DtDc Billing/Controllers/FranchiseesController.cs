using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
    [SessionUserModule]
    public class FranchiseesController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        // GET: Franchisees
        public ActionResult Index()
        {
            return View(db.Franchisees.ToList());
        }

        // GET: Franchisees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Franchisee franchisee = db.Franchisees.Find(id);

            if (franchisee == null)
            {
                return HttpNotFound();
            }

            return View(franchisee);
        }

        // GET: Franchisees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Franchisees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Franchisee franchisee)
        {


            if (ModelState.IsValid)
            {
                db.Franchisees.Add(franchisee);
                db.SaveChanges();



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
                cm.Phone = 8149490757;
                cm.Email = "Prajkta@gmail.com";
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


                foreach (var i in secotrs)
                {
                    Ratem dox = new Ratem();
                    Nondox ndox = new Nondox();
                    express_cargo cs = new express_cargo();

                    dox.Company_id = Companyid;
                    dox.Sector_Id = i.Sector_Id;
                    //dox.CashCounter = true;

                    ndox.Company_id = Companyid;
                    ndox.Sector_Id = i.Sector_Id;
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


                return RedirectToAction("Index");
            }

            return View(franchisee);
        }

        // GET: Franchisees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Franchisee franchisee = db.Franchisees.Find(id);
            if (franchisee == null)
            {
                return HttpNotFound();
            }
            return View(franchisee);
        }

        // POST: Franchisees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "F_Id,PF_Code,F_Address,OwnerName,BranchName,GstNo,Franchisee_Name")] Franchisee franchisee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(franchisee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(franchisee);
        }

        // GET: Franchisees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Franchisee franchisee = db.Franchisees.Find(id);
            if (franchisee == null)
            {
                return HttpNotFound();
            }
            return View(franchisee);
        }

        // POST: Franchisees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Franchisee franchisee = db.Franchisees.Find(id);
            db.Entry(franchisee).State = System.Data.Entity.EntityState.Deleted;
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
