using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using DtDc_Billing.Models;

namespace DtDc_Billing.Controllers
{
    
    public class RateMasterCashController : Controller
    {
        db_a71c08_elitetokenEntities db = new db_a71c08_elitetokenEntities();
        // GET: RateMasterCash


        public ActionResult demo()
        {
            List<Company> list = new List<Company>();

            foreach(var i in db.Franchisees)
            {
                var Company = db.Companies.Where(m => m.Franchisee.PF_Code == i.PF_Code).FirstOrDefault();
                list.Add(Company);
            }

            

            return View();
        }

        [SessionTimeout]
        public ActionResult EditCompanyCashCounter()
        {

            return View(db.Companies.ToList());
        }

        [SessionUserModule]
        public ActionResult EditCompanyCash()
        {
            
            var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString(); //Take It From Session

            var Company = db.Companies.Where(m => m.Company_Id == "Cash_" + PfCode).FirstOrDefault();

            TempData["CompanyId"] = Company.Company_Id;

            return RedirectToAction("Index", "RateMasterCash");
        }





        [SessionTimeout]
        [SessionUserModule]
        public ActionResult Index()
        {
            var CompanyId = TempData.Peek("CompanyId").ToString();

            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == CompanyId).FirstOrDefault();





            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId && m.Sector.CashD == true).OrderBy(m => m.Sector.Priority).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId && m.Sector.CashN == true).OrderBy(m => m.Sector.Priority).ToList();

            ViewBag.Plus = db.dtdcPlus.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.Ptp = db.Dtdc_Ptp.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.Cargo = db.express_cargo.Where(m => m.Company_id == CompanyId && m.Sector.CashD == true).Include(e => e.Sector).OrderBy(m => m.Sector.Priority).ToList();



            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult CashCounterDox(FormCollection fc)
        {

            //////////Showing Already Selected Slabs //////////

            var CompanyId = TempData.Peek("CompanyId").ToString();

            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == CompanyId).FirstOrDefault();


            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            /////////////////
            if (Session["EmpId"] == null || Session["pfCode"] == null)
            {
                ViewBag.Session = "Your Session Time Out Please Login";
            }
            else
            {

                if (ModelState.IsValid)
                {

                    var rateidarray = fc.GetValues("item.Rete_Id");
                    var slab1arayy = fc.GetValues("item.slab1");
                    var slab2arayy = fc.GetValues("item.slab2");
                    var slab3arayy = fc.GetValues("item.slab3");
                    var slab4arayy = fc.GetValues("item.slab4");
                    var uptoarray = fc.GetValues("Upto");
                    var noofslab = fc.GetValues("item.NoOfSlab");

                    var sectoridarray = fc.GetValues("item.Sector_Id");

                    for (int i = 0; i < rateidarray.Count(); i++)
                    {
                        if (slab1arayy[i] == "")
                        {
                            slab1arayy[i] = "0";
                        }
                        if (slab2arayy[i] == "")
                        {
                            slab2arayy[i] = "0";
                        }
                        if (slab3arayy[i] == "")
                        {
                            slab3arayy[i] = "0";
                        }
                        if (slab4arayy[i] == "")
                        {
                            slab4arayy[i] = "0";
                        }
                    }
                    for (int i = 0; i < uptoarray.Count(); i++)
                    {
                        if (uptoarray[i] == "")
                        {
                            uptoarray[i] = "0";
                        }
                    }



                    for (int i = 0; i < rateidarray.Count(); i++)
                    {

                        Ratem rm = db.Ratems.Find(Convert.ToInt16(rateidarray[i]));

                        rm.slab1 = Convert.ToDouble(slab1arayy[i]);
                        rm.slab2 = Convert.ToDouble(slab2arayy[i]);
                        rm.slab3 = Convert.ToDouble(slab3arayy[i]);
                        rm.slab4 = Convert.ToDouble(slab4arayy[i]);
                        rm.Uptosl1 = Convert.ToDouble(uptoarray[0]);
                        rm.Uptosl2 = Convert.ToDouble(uptoarray[1]);
                        rm.Uptosl3 = Convert.ToDouble(uptoarray[2]);
                        rm.Uptosl4 = Convert.ToDouble(uptoarray[3]);
                        rm.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                        rm.NoOfSlab = Convert.ToInt16(noofslab[0]);
                        rm.Company_id = TempData.Peek("CompanyId").ToString();
                        rm.CashCounter = true;



                        db.Entry(rm).State = EntityState.Modified;
                        db.SaveChanges();


                    }

                    var compid = TempData.Peek("CompanyId").ToString();

                    ViewBag.Message = "Dox Updated SuccessFully";

                    @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == compid).FirstOrDefault();

                    return PartialView("CashCounterDox", db.Ratems.Where(m => m.Company_id == compid && m.Sector.CashD == true).OrderBy(m => m.Sector.Priority).ToList());
                }
            }

                return PartialView("CashCounterDox", fc);
            

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult CashCounterNonDox(FormCollection fc)
        {


            //////////Showing Already Selected Slabs //////////

            var CompanyId = TempData.Peek("CompanyId").ToString();

            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == CompanyId).FirstOrDefault();


            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            /////////////////

            if (Session["EmpId"] == null || Session["pfCode"] == null)
            {
                ViewBag.Session = "Your Session Time Out Please Login";
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var Non_IDarray = fc.GetValues("item.Non_ID");
                    var Aslab1arayy = fc.GetValues("item.Aslab1");
                    var Aslab2arayy = fc.GetValues("item.Aslab2");
                    var Aslab3arayy = fc.GetValues("item.Aslab3");
                    var Aslab4arayy = fc.GetValues("item.Aslab4");
                    var Sslab1arayy = fc.GetValues("item.Sslab1");
                    var Sslab2arayy = fc.GetValues("item.Sslab2");
                    var Sslab3arayy = fc.GetValues("item.Sslab3");
                    var Sslab4arayy = fc.GetValues("item.Sslab4");

                    var Auptoarray = fc.GetValues("AUpto");
                    var Suptoarray = fc.GetValues("SUpto");
                    var sectoridarray = fc.GetValues("item.Sector_Id");
                    var NoofslabN = fc.GetValues("item.NoOfSlabN");
                    var NoofslabS = fc.GetValues("item.NoOfSlabS");



                    for (int i = 0; i < Non_IDarray.Count(); i++)
                    {
                        if (Aslab1arayy[i] == "")
                        {
                            Aslab1arayy[i] = "0";
                        }
                        if (Aslab2arayy[i] == "")
                        {
                            Aslab2arayy[i] = "0";
                        }
                        if (Aslab3arayy[i] == "")
                        {
                            Aslab3arayy[i] = "0";
                        }
                        if (Aslab4arayy[i] == "")
                        {
                            Aslab4arayy[i] = "0";
                        }
                        if (Sslab1arayy[i] == "")
                        {
                            Sslab1arayy[i] = "0";
                        }
                        if (Sslab2arayy[i] == "")
                        {
                            Sslab2arayy[i] = "0";
                        }
                        if (Sslab3arayy[i] == "")
                        {
                            Sslab3arayy[i] = "0";
                        }
                        if (Sslab4arayy[i] == "")
                        {
                            Sslab4arayy[i] = "0";
                        }
                    }
                    for (int i = 0; i < Auptoarray.Count(); i++)
                    {
                        if (Auptoarray[i] == "")
                        {
                            Auptoarray[i] = "0";
                        }
                        if (Suptoarray[i] == "")
                        {
                            Suptoarray[i] = "0";
                        }
                    }




                    for (int i = 0; i < Non_IDarray.Count(); i++)
                    {

                        Nondox rm = db.Nondoxes.Find(Convert.ToInt16(Non_IDarray[i]));

                        rm.Aslab1 = Convert.ToDouble(Aslab1arayy[i]);
                        rm.Aslab2 = Convert.ToDouble(Aslab2arayy[i]);
                        rm.Aslab3 = Convert.ToDouble(Aslab3arayy[i]);
                        rm.Aslab4 = Convert.ToDouble(Aslab4arayy[i]);
                        rm.Sslab1 = Convert.ToDouble(Sslab1arayy[i]);
                        rm.Sslab2 = Convert.ToDouble(Sslab2arayy[i]);
                        rm.Sslab3 = Convert.ToDouble(Sslab3arayy[i]);
                        rm.Sslab4 = Convert.ToDouble(Sslab4arayy[i]);
                        rm.AUptosl1 = Convert.ToDouble(Auptoarray[0]);
                        rm.AUptosl2 = Convert.ToDouble(Auptoarray[1]);
                        rm.AUptosl3 = Convert.ToDouble(Auptoarray[2]);
                        rm.AUptosl4 = Convert.ToDouble(Auptoarray[3]);
                        rm.SUptosl1 = Convert.ToDouble(Suptoarray[0]);
                        rm.SUptosl2 = Convert.ToDouble(Suptoarray[1]);
                        rm.SUptosl3 = Convert.ToDouble(Suptoarray[2]);
                        rm.SUptosl4 = Convert.ToDouble(Suptoarray[3]);
                        rm.Company_id = TempData.Peek("CompanyId").ToString();
                        rm.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                        rm.NoOfSlabN = Convert.ToInt16(NoofslabN[0]);
                        rm.NoOfSlabS = Convert.ToInt16(NoofslabS[0]);
                        rm.CashCounterNon = true;

                        db.Entry(rm).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    var compid = TempData.Peek("CompanyId").ToString();

                    ViewBag.Message = "NonDox Updated SuccessFully";

                    @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == compid).FirstOrDefault();

                    ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == compid).ToList();



                    return PartialView("CashCounterNonDox", db.Nondoxes.Where(m => m.Company_id == compid && m.Sector.CashN == true).OrderBy(m => m.Sector.Priority).ToList());

                }
            }
            return PartialView("CashCounterNonDox", fc);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult CashCounterPlus(float? go149, float? go99, FormCollection fc)
        {

            //////////Showing Already Selected Slabs //////////

            var CompanyId = TempData.Peek("CompanyId").ToString();

            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == CompanyId).FirstOrDefault();


            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            /////////////////
            if (Session["EmpId"] == null || Session["pfCode"] == null)
            {
                ViewBag.Session = "Your Session Time Out Please Login";
            }
            else
            {

                if (ModelState.IsValid)
                {

                    var plus_idarray = fc.GetValues("item.plus_id");
                    var Upto500gmarray = fc.GetValues("item.Upto500gm");
                    var U10to25kgarayy = fc.GetValues("item.U10to25kg");
                    var U25to50arayy = fc.GetValues("item.U25to50");
                    var U50to100arayy = fc.GetValues("item.U50to100");
                    var add100kgarayy = fc.GetValues("item.add100kg");
                    var Add500gmarayy = fc.GetValues("item.Add500gm");

                    for (int i = 0; i < plus_idarray.Count(); i++)
                    {
                        if (Upto500gmarray[i] == "")
                        {
                            Upto500gmarray[i] = "0";
                        }
                        if (U10to25kgarayy[i] == "")
                        {
                            U10to25kgarayy[i] = "0";
                        }
                        if (U25to50arayy[i] == "")
                        {
                            U25to50arayy[i] = "0";
                        }
                        if (U50to100arayy[i] == "")
                        {
                            U50to100arayy[i] = "0";
                        }
                        if (add100kgarayy[i] == "")
                        {
                            add100kgarayy[i] = "0";
                        }
                        if (Add500gmarayy[i] == "")
                        {
                            Add500gmarayy[i] = "0";
                        }
                    }

                    for (int i = 0; i < plus_idarray.Count(); i++)
                    {
                        dtdcPlu rm = db.dtdcPlus.Find(Convert.ToInt16(plus_idarray[i]));

                        rm.Upto500gm = Convert.ToDouble(Upto500gmarray[i]);
                        rm.U10to25kg = Convert.ToDouble(U10to25kgarayy[i]);
                        rm.U25to50 = Convert.ToDouble(U25to50arayy[i]);
                        rm.U50to100 = Convert.ToDouble(U50to100arayy[i]);
                        rm.add100kg = Convert.ToDouble(add100kgarayy[i]);
                        rm.Add500gm = Convert.ToDouble(Add500gmarayy[i]);
                        rm.CashCounterPlus = true;

                        db.Entry(rm).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var compid = TempData.Peek("CompanyId").ToString();

                    ViewBag.Message = "Dtdc Plus Updated SuccessFully";

                    //@ViewBag.Slabs = db.Dtdc_Ptp.Where(m => m.Company_id == compid).FirstOrDefault();

                    return PartialView("CashCounterPlus", db.dtdcPlus.Where(m => m.Company_id == compid).ToList());
                }
            }
            return PartialView("CashCounterPlus", fc);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult CashCounterExpress(float? Upto, FormCollection fc)
        {


            //////////Showing Already Selected Slabs //////////

            var CompanyId = TempData.Peek("CompanyId").ToString();

            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == CompanyId).FirstOrDefault();


            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            /////////////////
            var compid = TempData.Peek("CompanyId").ToString();

            if (Session["EmpId"] == null || Session["pfCode"] == null)
            {
                ViewBag.Session = "Your Session Time Out Please Login";
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var Exp_Idarray = fc.GetValues("item.Exp_Id");
                    var Exslab1array = fc.GetValues("item.Exslab1");
                    var Exslab2arayy = fc.GetValues("item.Exslab2");
                    var Sector_Idarayy = fc.GetValues("item.Sector_Id");


                    for (int i = 0; i < Exp_Idarray.Count(); i++)
                    {
                        if (Exslab1array[i] == "")
                        {
                            Exslab1array[i] = "0";
                        }
                        if (Exslab2arayy[i] == "")
                        {
                            Exslab2arayy[i] = "0";
                        }

                    }

                    for (int i = 0; i < Exp_Idarray.Count(); i++)
                    {

                        express_cargo rm = db.express_cargo.Find(Convert.ToInt16(Exp_Idarray[i]));

                        rm.Exslab1 = Convert.ToDouble(Exslab1array[i]);
                        rm.Exslab2 = Convert.ToDouble(Exslab2arayy[i]);
                        rm.CashCounterExpr = true;

                        ViewBag.Message = "Express Cargo Updated SuccessFully";

                        db.Entry(rm).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return PartialView("CashCounterExpress", db.express_cargo.Where(m => m.Company_id == compid && m.Sector.CashD == true).OrderBy(m => m.Sector.Priority).ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult CashCounterPtp(FormCollection fc)
        {

            //////////Showing Already Selected Slabs //////////

            var CompanyId = TempData.Peek("CompanyId").ToString();

            @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == CompanyId).FirstOrDefault();

            @ViewBag.Slabs1 = db.Nondoxes.Where(m => m.Company_id == CompanyId).FirstOrDefault();


            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            /////////////////
            if (Session["EmpId"] == null || Session["pfCode"] == null)
            {
                ViewBag.Session = "Your Session Time Out Please Login";
            }
            else
            {

                if (ModelState.IsValid)
                {

                    var Ptp_idarray = fc.GetValues("item.ptp_id");
                    var PUpto500gmarray = fc.GetValues("item.PUpto500gm");
                    var PAdd500gmarayy = fc.GetValues("item.PAdd500gm");
                    var PU10to25kgarayy = fc.GetValues("item.PU10to25kg");
                    var PU25to50arayy = fc.GetValues("item.PU25to50");
                    var PU50to100arayy = fc.GetValues("item.PU50to100");
                    var Padd100kgarayy = fc.GetValues("item.Padd100kg");

                    var P2Upto500gmarray = fc.GetValues("item.P2Upto500gm");
                    var P2Add500gmarayy = fc.GetValues("item.P2Add500gm");
                    var P2U10to25kgarayy = fc.GetValues("item.P2U10to25kg");
                    var P2U25to50arayy = fc.GetValues("item.P2U25to50");
                    var P2U50to100arayy = fc.GetValues("item.P2U50to100");
                    var P2add100kgarayy = fc.GetValues("item.P2add100kg");


                    for (int i = 0; i < Ptp_idarray.Count(); i++)
                    {
                        if (PUpto500gmarray[i] == "")
                        {
                            PUpto500gmarray[i] = "0";
                        }
                        if (PAdd500gmarayy[i] == "")
                        {
                            PAdd500gmarayy[i] = "0";
                        }
                        if (PU10to25kgarayy[i] == "")
                        {
                            PU10to25kgarayy[i] = "0";
                        }
                        if (PU25to50arayy[i] == "")
                        {
                            PU25to50arayy[i] = "0";
                        }
                        if (PU50to100arayy[i] == "")
                        {
                            PU50to100arayy[i] = "0";
                        }
                        if (Padd100kgarayy[i] == "")
                        {
                            Padd100kgarayy[i] = "0";
                        }
                        if (P2Upto500gmarray[i] == "")
                        {
                            P2Upto500gmarray[i] = "0";
                        }
                        if (P2Add500gmarayy[i] == "")
                        {
                            P2Add500gmarayy[i] = "0";
                        }
                        if (P2U10to25kgarayy[i] == "")
                        {
                            P2U10to25kgarayy[i] = "0";
                        }
                        if (P2U25to50arayy[i] == "")
                        {
                            P2U25to50arayy[i] = "0";
                        }
                        if (P2U50to100arayy[i] == "")
                        {
                            P2U50to100arayy[i] = "0";
                        }
                        if (P2add100kgarayy[i] == "")
                        {
                            P2add100kgarayy[i] = "0";
                        }
                    }

                    for (int i = 0; i < Ptp_idarray.Count(); i++)
                    {

                        Dtdc_Ptp rm = db.Dtdc_Ptp.Find(Convert.ToInt16(Ptp_idarray[i]));


                        rm.PUpto500gm = Convert.ToDouble(PUpto500gmarray[i]);
                        rm.PAdd500gm = Convert.ToDouble(PAdd500gmarayy[i]);
                        rm.PU10to25kg = Convert.ToDouble(PU10to25kgarayy[i]);
                        rm.PU25to50 = Convert.ToDouble(PU25to50arayy[i]);
                        rm.PU50to100 = Convert.ToDouble(PU50to100arayy[i]);
                        rm.Padd100kg = Convert.ToDouble(Padd100kgarayy[i]);
                        rm.P2Upto500gm = Convert.ToDouble(P2Upto500gmarray[i]);
                        rm.P2Add500gm = Convert.ToDouble(P2Add500gmarayy[i]);
                        rm.P2U10to25kg = Convert.ToDouble(P2U10to25kgarayy[i]);
                        rm.P2U25to50 = Convert.ToDouble(P2U25to50arayy[i]);
                        rm.P2U50to100 = Convert.ToDouble(P2U50to100arayy[i]);
                        rm.P2add100kg = Convert.ToDouble(P2add100kgarayy[i]);


                        db.Entry(rm).State = EntityState.Modified;
                        db.SaveChanges();




                    }

                    var compid = TempData.Peek("CompanyId").ToString();

                    ViewBag.Message = "DtdcPtp Updated SuccessFully";

                    @ViewBag.Slabs = db.Dtdc_Ptp.Where(m => m.Company_id == compid).FirstOrDefault();

                    return PartialView("CashCounterPtp", db.Dtdc_Ptp.Where(m => m.Company_id == compid).Include(e => e.Sector).ToList());

                }
            }
            return PartialView("CashCounterPtp", fc);



        }

    }
}