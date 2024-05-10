using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]
   // [SessionUserModule]
    //[OutputCache(CacheProfile = "Cachefast")]
    public class RateMasterController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        // GET: RateMaster
        public ActionResult Index(string id)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            //var CompanyId= TempData.Peek("CompanyId").ToString();
            ViewBag.companyid = Server.UrlDecode(Request.Url.Segments[3]);
            id = id.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = id;

            //Company company = db.Companies.Where(m => m.Company_Id == CompanyId).FirstOrDefault();

            var company = (from d in db.Companies
                        where d.Pf_code == strpfcode
                        && d.Company_Id == CompanyId
                        select d).FirstOrDefault();

            CompanyModel Comp = new CompanyModel(); 

            Comp.Pf_code = company.Pf_code;
            Comp.Company_Id = company.Company_Id;
            Comp.Phone = company.Phone;
            Comp.Email = company.Email;
            Comp.Insurance = company.Insurance;
            Comp.Minimum_Risk_Charge = company.Minimum_Risk_Charge;
            Comp.Other_Details = company.Other_Details;
            Comp.Fuel_Sur_Charge = company.Fuel_Sur_Charge;
            Comp.Topay_Charge = company.Topay_Charge;
            Comp.Cod_Charge = company.Cod_Charge;
            Comp.Gec_Fuel_Sur_Charge = company.Gec_Fuel_Sur_Charge;
            Comp.Company_Address = company.Company_Address;
            Comp.Company_Name = company.Company_Name;
            Comp.Datetime_Comp = company.Datetime_Comp;
            Comp.Gst_No = company.Gst_No;
            Comp.Pan_No = company.Pan_No;
            Comp.Royalty_Charges = company.Royalty_Charges;
            Comp.D_Docket = company.D_Docket;
            Comp.P_Docket = company.P_Docket;
            Comp.E_Docket = company.E_Docket;
            Comp.V_Docket = company.V_Docket;
            Comp.I_Docket = company.I_Docket;
            Comp.N_Docket = company.N_Docket;
            //Comp.Password = company.Password;
           //Comp.Username = company.Username;
            Comp.G_Docket = company.G_Docket;


            ViewBag.Company = Comp;//db.Companies.Where(m=>m.Company_Id== CompanyId).FirstOrDefault();

            var getDox = db.Ratems.Where(m => m.Company_id == CompanyId && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList();
            ViewBag.Dox = getDox;
            @ViewBag.Slabs = getDox.FirstOrDefault();


            var getNonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).OrderBy(m => m.Sector.Priority).ToList();
            ViewBag.NonDoxAir = getNonDox.Where(m => m.Sector.BillNonAir == true).FirstOrDefault();
            ViewBag.NonDoxSur=getNonDox.Where(m=>m.Sector.BillNonSur == true).FirstOrDefault();
            ViewBag.nonDoxAirCount = getNonDox.Where(x => x.Sector.BillNonAir == true).Count();
            ViewBag.nonDoxSurCount = getNonDox.Where(x => x.Sector.BillNonSur == true).Count();

            ViewBag.NonDox = getNonDox;

            @ViewBag.Slabs1 = getNonDox.FirstOrDefault();

            var getPrio = db.Priorities.Where(m => m.Company_id == CompanyId && m.Sector.BillPriority == true).OrderBy(m => m.Sector.Priority).ToList();

            ViewBag.Priority = getPrio;

            @ViewBag.Slabspri = getPrio.FirstOrDefault();


            ViewBag.Plus = db.dtdcPlus.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.Ptp = db.Dtdc_Ptp.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.Cargo = db.express_cargo.Where(m => m.Company_id == CompanyId && m.Sector.BillExpCargo == true).Include(e => e.Sector).OrderBy(m => m.Sector.Priority).ToList();



            
            var getEcom = db.Dtdc_Ecommerce.Where(m => m.Company_id == CompanyId).Include(e => e.Sector).OrderBy(m => m.Sector.Priority).ToList();


             var getEcomCount = getEcom.Where(x => x.Sector.BillEcomPrio == true).Count();
            ViewBag.EcomPCount = getEcomCount;

            if (getEcomCount > 0)
            {
                ViewBag.comP = getEcom.Where(x => x.Sector.BillEcomPrio == true).FirstOrDefault();
            }



            var EcomGECount = getEcom.Where(x => x.Sector.BillEcomGE == true).Count();
            ViewBag.EcomGECount = EcomGECount;

            if (EcomGECount > 0)
            {
                ViewBag.comGC = getEcom.Where(x => x.Sector.BillEcomGE == true).FirstOrDefault();
            }


            ViewBag.Dtdc_Ecommerce = getEcom;
            


            //<-------------risk surch charge dropdown--------------->
            double? selectedval = db.Companies.Where(m => m.Company_Id == CompanyId).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "0", Value = "0" });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            items.Add(new SelectListItem { Text = "100", Value = "100" });

            if (selectedval == null)
            {
                var selected = items.Where(x => x.Value == "0").First();
                selected.Selected = true;
            }
            else
            {


                var selected = items.Where(x => x.Value == selectedval.ToString()).First();
                selected.Selected = true;
            }

            ViewBag.Minimum_Risk_Charge = items;

            //<-------------risk surch charge dropdown--------------->

            ViewBag.Pf_code = company.Pf_code;//new SelectList(db.Franchisees, "PF_Code", "PF_Code", company.Pf_code);

            return View();
        }

        [HttpGet]
        public ActionResult EditCompanyRateMaster()
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var data = (from d in db.Companies
                        where d.Pf_code == strpfcode
                        // && d. == stradmin
                        select new CompanyModel
                        {
                            Company_Id=d.Company_Id,
                            Pf_code = d.Pf_code,
                            Company_Name = d.Company_Name,
                            Phone = d.Phone,
                            Email=d.Email,
                            Company_Address = d.Company_Address
                        }).ToList();

            return View(data);
        }

        public string saveMissingSectors(string pfCode, string companyId)
        {
            var secotrs = db.Sectors.Where(m => m.Pf_code == pfCode).ToList();

            var getDoxList = db.Ratems.Where(x => x.Company_id == companyId).ToList();
            var getNonDoxList = db.Nondoxes.Where(x => x.Company_id == companyId).ToList();
            var getDtdcPlusList = db.dtdcPlus.Where(x => x.Company_id == companyId).ToList();
            var getDtdc_PtpList = db.Dtdc_Ptp.Where(x => x.Company_id == companyId).ToList();
            var getPrioritiesList = db.Priorities.Where(x => x.Company_id == companyId).ToList();
            var getExpress_cargoList = db.express_cargo.Where(x => x.Company_id == companyId).ToList();
            var getDtdc_EcommerceList = db.Dtdc_Ecommerce.Where(x => x.Company_id == companyId).ToList();

            //DOX table start
            // Find sector IDs present in secotrs but not in getDoxList
            var sectorsToAdd = secotrs.Where(x => x.BillD == true).Select(s => s.Sector_Id).Where(sectorId => !getDoxList.Select(d => d.Sector_Id).Contains(sectorId));

            var noOfSlab = getDoxList.Count() == 0 ? 2: getDoxList.FirstOrDefault().NoOfSlab;
            // Add new entries to Ratems table
            foreach (var sectorId in sectorsToAdd)
            {
                var newRatemEntry = new Ratem
                {
                    Sector_Id = sectorId,
                    slab1 = 1,
                    slab2 = 1,
                    slab3 = 1,
                    slab4 = 1,
                    Uptosl1 = 1,
                    Uptosl2 = 1,
                    Uptosl3 = 1,
                    Uptosl4 = 1,
                    Company_id = companyId,
                    NoOfSlab = noOfSlab
                    // Set other properties as needed
                };

                db.Ratems.Add(newRatemEntry);
            }

            // Find sector IDs present in getDoxList but not in secotrs
            var sectorsToRemove = getDoxList
     .Select(d => d.Sector_Id)
     .Where(sectorId => sectorId.HasValue && !secotrs.Where(x => x.BillD == true).Select(s => s.Sector_Id).Contains(sectorId.Value))
     .ToList();



            // Remove entries from Ratems table
            foreach (var sectorId in sectorsToRemove)
            {
                var ratemToRemove = db.Ratems.FirstOrDefault(x => x.Sector_Id == sectorId);
                if (ratemToRemove != null)
                {
                    db.Ratems.Remove(ratemToRemove);
                }
            }

            // Save changes to the database
            db.SaveChanges();

            //DOX table end 

            //NONDOX table start

            var sectorsToAddNonDox = secotrs.Select(s => s.Sector_Id).Where(sectorId => !getNonDoxList.Select(d => d.Sector_Id).Contains(sectorId));

            var noOfSlabNonDoxN = getNonDoxList.Count()== 0 ? 2 : getNonDoxList.FirstOrDefault().NoOfSlabN;
            var noOfSlabNonDoxS = getNonDoxList.Count() == 0 ? 2 :getNonDoxList.FirstOrDefault().NoOfSlabS;

            foreach (var sectorId in sectorsToAddNonDox)
            {
                var newNonDoxEntry = new Nondox
                {
                    Sector_Id = sectorId,
                    Company_id = companyId,
                    NoOfSlabN = noOfSlabNonDoxN,
                    NoOfSlabS = noOfSlabNonDoxS

                };

                db.Nondoxes.Add(newNonDoxEntry);
            }


            var sectorsToRemoveNonDox = getNonDoxList
     .Select(d => d.Sector_Id)
     .Where(sectorId => sectorId.HasValue && !secotrs.Where(x => x.BillD == true).Select(s => s.Sector_Id).Contains(sectorId.Value))
     .ToList();


            foreach (var sectorId in sectorsToRemove)
            {
                var nonDoxToRemove = db.Nondoxes.FirstOrDefault(x => x.Sector_Id == sectorId);
                if (nonDoxToRemove != null)
                {
                    db.Nondoxes.Remove(nonDoxToRemove);
                }
            }

            db.SaveChanges();

            //NONDOX table end 


            //EXPRESS CARGO table start

            var sectorsToAddEC = secotrs.Where(x => x.BillExpCargo == true).Select(s => s.Sector_Id).Where(sectorId => !getExpress_cargoList.Select(d => d.Sector_Id).Contains(sectorId));

            foreach (var sectorId in sectorsToAddEC)
            {
                var newECEntry = new express_cargo
                {
                    Sector_Id = sectorId,
                    Company_id = companyId,
                };

                db.express_cargo.Add(newECEntry);
            }

            var sectorsToRemoveEC = getExpress_cargoList
     .Select(d => d.Sector_Id)
     .Where(sectorId => sectorId.HasValue && !secotrs.Where(x => x.BillExpCargo == true).Select(s => s.Sector_Id).Contains(sectorId.Value))
     .ToList();


            foreach (var sectorId in sectorsToRemoveEC)
            {
                var ratemToRemove = db.express_cargo.FirstOrDefault(x => x.Sector_Id == sectorId);
                if (ratemToRemove != null)
                {
                    db.express_cargo.Remove(ratemToRemove);
                }
            }

            db.SaveChanges();

            //EXPRESS CARGO table end 


            //PRIORITY table start

            var sectorsToAddPri = secotrs.Where(x => x.BillPriority == true).Select(s => s.Sector_Id).Where(sectorId => !getPrioritiesList.Select(d => d.Sector_Id).Contains(sectorId));

            foreach (var sectorId in sectorsToAddPri)
            {
                var newPrioEntry = new Priority
                {
                    Sector_Id = sectorId,
                    Company_id = companyId,
                };

                db.Priorities.Add(newPrioEntry);
            }

            var getSector = secotrs.Where(x => x.BillPriority == true).ToList();

            var sectorsToRemovePrio = getPrioritiesList
     .Select(d => d.Sector_Id)
     .Where(sectorId => sectorId.HasValue && !getSector.Select(s => s.Sector_Id).Contains(sectorId.Value))
     .ToList();


             foreach (var sectorId in sectorsToRemovePrio)
            {
                var ratemToRemovePrio = db.Priorities.FirstOrDefault(x => x.Sector_Id == sectorId);
                if (ratemToRemovePrio != null)
                {
                    db.Priorities.Remove(ratemToRemovePrio);
                }
            }

            db.SaveChanges();

            //EXPRESS CARGO table end


            //Ecommerce table start

            var sectorsToAddECom = secotrs.Select(s => s.Sector_Id).Where(sectorId => !getDtdc_EcommerceList.Select(d => d.Sector_Id).Contains(sectorId));

            var noOfSlabEcomN = getDtdc_EcommerceList.Count() == 0 ? 2 : getDtdc_EcommerceList.FirstOrDefault().NoOfSlabN;
            var noOfSlabEcomS = getDtdc_EcommerceList.Count() == 0 ? 2 : getDtdc_EcommerceList.FirstOrDefault().NoOfSlabS;

            foreach (var sectorId in sectorsToAddECom)
            {
                var newEcomEntry = new Dtdc_Ecommerce
                {
                    Sector_Id = sectorId,
                    Company_id = companyId,
                    NoOfSlabN = noOfSlabEcomN,
                    NoOfSlabS = noOfSlabEcomS
                };

                db.Dtdc_Ecommerce.Add(newEcomEntry);
            }

            var sectorsToRemoveEcom = getDtdc_EcommerceList
     .Select(d => d.Sector_Id)
     .Where(sectorId => sectorId.HasValue && !secotrs.Where(x => (x.BillEcomGE == true || x.BillEcomPrio == true)).Select(s => s.Sector_Id).Contains(sectorId.Value))
     .ToList();


            foreach (var sectorId in sectorsToRemoveEcom)
            {
                var ecomToRemovePrio = db.Dtdc_Ecommerce.FirstOrDefault(x => x.Sector_Id == sectorId);
                if (ecomToRemovePrio != null)
                {
                    db.Dtdc_Ecommerce.Remove(ecomToRemovePrio);
                }
            }

            db.SaveChanges();

            //Ecommerce table end
            return "";
        }

        public ActionResult EditCompanyRate(string Id)
        {
            //  Id = Id.Replace("__", "&").Replace("xdotx", "."); ;
            var Idd = Id.Replace("__", "&").Replace("xdotx", "."); ;
           
            TempData["CompanyId"] = Id;

            var pfcode = db.Companies.Where(m => m.Company_Id == Idd).FirstOrDefault();
            var prio = db.Priorities.Where(m => m.Company_id == Idd && m.Sector.BillPriority == true).ToList();

            if (prio.Count() == 0)
            {
                var secotrs = db.Sectors.Where(m => m.Pf_code == pfcode.Pf_code && m.BillPriority == true).ToList();
                foreach (var i in secotrs)
                {
                    Priority pr = new Priority();
                    pr.prislab1 = 1;
                    pr.prislab2 = 1;
                    pr.prislab3 = 1;
                    pr.prislab4  = 1;

                    pr.priupto1 = 1;
                    pr.priupto2 = 1;
                    pr.priupto3 = 1;
                    pr.priupto4 = 1;

                    pr.prinoofslab = 2;

                    pr.Company_id = Id;
                    pr.Sector_Id = i.Sector_Id;
                    db.Priorities.Add(pr);
                    db.SaveChanges();


                    i.BillPriority = true;
                    i.BillNonAir = true;
                    i.BillNonSur = true;
                    i.BillEcomPrio = true;
                    i.BillEcomGE = true;
                    db.Entry(i).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }

            var ecom = db.Dtdc_Ecommerce.Where(m => m.Company_id == Id).ToList();

            if (ecom.Count == 0)
            {
                var secotrs = db.Sectors.Where(m => m.Pf_code == pfcode.Pf_code && (m.BillEcomGE == true || m.BillEcomPrio == true)).ToList();
                foreach (var i in secotrs)
                {
                    Dtdc_Ecommerce rm = new Dtdc_Ecommerce();
                    rm.EcomPslab1 = 1;
                    rm.EcomPslab2 = 1;
                    rm.EcomPslab3 = 1;
                    rm.EcomPslab4 = 1;

                    rm.EcomGEslab1 = 1;
                    rm.EcomGEslab2 = 1;
                    rm.EcomGEslab3 = 1;
                    rm.EcomGEslab4 = 1;

                    rm.EcomPupto1 = 1;
                    rm.EcomPupto2 = 1;
                    rm.EcomPupto3 = 1;
                    rm.EcomPupto4 = 1;

                    rm.EcomGEupto1 = 1;
                    rm.EcomGEupto2 = 1;
                    rm.EcomGEupto3 = 1;
                    rm.EcomGEupto4 = 1;

                    rm.NoOfSlabN = 2;
                    rm.NoOfSlabS = 2;
                    rm.Company_id = Id;
                    rm.Sector_Id = i.Sector_Id;
                    db.Dtdc_Ecommerce.Add(rm);
                    db.SaveChanges();
                }
            }

            saveMissingSectors(pfcode.Pf_code, Idd);
            return RedirectToAction("Index", "RateMaster", new { id = Id });
        }

        public ActionResult AddCompany()
        {
            ViewBag.Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "PF_Code");

            TempData["ShowLoader"] = true;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "0", Value = "0", Selected = true });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            items.Add(new SelectListItem { Text = "100", Value = "100" });

            ViewBag.Minimum_Risk_Charge = items;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterCompany(CompanyModel empmodel, float[] slab1arr, float[] slab2arr, float[] slab3arr, float[] slab4arr, float[] Upto, int[] Sector_Id, int? only, string selected_tab)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            //string Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "PF_Code");

            ViewBag.sr = db.Sectors.ToList();

            if(empmodel.Company_Id!=null)
            {
                empmodel.Company_Id = empmodel.Company_Id.Trim();

            }

            var abc = db.Companies.Where(m => m.Company_Id.ToLower() == empmodel.Company_Id.ToLower()).FirstOrDefault();

            if (abc != null)
            {
                ModelState.AddModelError("C_IdError", "Company Id Already Exist");
                TempData["Error"] = "Company Id Already Exist";
            }
            //Take PfCode From Session//
            if (ModelState.IsValid)
            {
                // Business Logic
                TempData["ShowLoader"] = true;


                Company comp = new Company();

                comp.Company_Id = empmodel.Company_Id;
                comp.Phone = empmodel.Phone;
                comp.Email = empmodel.Email;
                comp.Insurance = empmodel.Insurance;
                comp.Minimum_Risk_Charge = empmodel.Minimum_Risk_Charge;
                comp.Other_Details = empmodel.Other_Details;
                comp.Fuel_Sur_Charge = empmodel.Fuel_Sur_Charge;
                comp.Topay_Charge = empmodel.Topay_Charge;
                comp.Cod_Charge = empmodel.Cod_Charge;
                comp.Gec_Fuel_Sur_Charge = empmodel.Gec_Fuel_Sur_Charge;
                comp.Pf_code = empmodel.Pf_code;
                comp.Company_Address = empmodel.Company_Address;
                comp.Company_Name = empmodel.Company_Name;
                comp.Datetime_Comp = empmodel.Datetime_Comp;
                comp.Gst_No = empmodel.Gst_No;
                comp.Pan_No = empmodel.Pan_No;
                comp.Royalty_Charges = empmodel.Royalty_Charges;
                comp.D_Docket = empmodel.D_Docket;
                comp.P_Docket = empmodel.P_Docket;
                comp.E_Docket = empmodel.E_Docket;
                comp.V_Docket = empmodel.V_Docket;
                comp.I_Docket = empmodel.I_Docket;
                comp.N_Docket = empmodel.N_Docket;
                //comp.Password = empmodel.Password;
                //comp.Username = empmodel.Username;
                comp.G_Docket = empmodel.G_Docket;

                ViewBag.Message = "Sucess or Failure Message";
                ModelState.Clear();
                TempData["CompanyId"] = empmodel.Company_Id;
                db.Companies.Add(comp);
                db.SaveChanges();


            

                var secotrs = db.Sectors.Where(m => m.Pf_code == empmodel.Pf_code).ToList();            

               

                //var basicdox = db.Ratems.Where(m => m.Company_id == "BASIC_TS").ToArray();
                //var basicnon = db.Nondoxes.Where(m => m.Company_id == "BASIC_TS").ToArray();
                //var express = db.express_cargo.Where(m => m.Company_id == "BASIC_TS").ToArray();
                //var prio = db.Priorities.Where(m => m.Company_id == "BASIC_TS").ToArray();
                int j = 0;

                foreach (var i in secotrs)
                {
                    Ratem dox = new Ratem();
                    Nondox ndox = new Nondox();
                    express_cargo cs = new express_cargo();
                    Priority pri = new Priority();
                    Dtdc_Ecommerce dtdc_Ecommerce = new Dtdc_Ecommerce();

                    dox.Company_id = empmodel.Company_Id;
                    dox.Sector_Id = i.Sector_Id;
                    dox.NoOfSlab = 2;

                    dox.slab1 = 1;
                    dox.slab2 = 1;
                    dox.slab3 = 1;
                    dox.slab4 = 1;

                    dox.Uptosl1 =1;
                    dox.Uptosl2 = 1;
                    dox.Uptosl3 =1;
                    dox.Uptosl4 = 1;

                    ndox.Company_id = empmodel.Company_Id;
                    ndox.Sector_Id = i.Sector_Id;
                    ndox.NoOfSlabN = 2;
                    ndox.NoOfSlabS = 2;

                    ndox.Aslab1 = 1;
                    ndox.Aslab2 =1;
                    ndox.Aslab3 = 1;
                    ndox.Aslab4 = 1;


                    ndox.Sslab1 = 1;
                    ndox.Sslab2 = 1;
                    ndox.Sslab3 = 1;
                    ndox.Sslab4 =1;

                    ndox.AUptosl1 = 1;
                    ndox.AUptosl2 = 1;
                    ndox.AUptosl3 = 1;
                    ndox.AUptosl4 =1;

                    ndox.SUptosl1 =1;
                    ndox.SUptosl2 =1;
                    ndox.SUptosl3 = 1;
                    ndox.SUptosl4 =1;

                    pri.Company_id = empmodel.Company_Id;
                    pri.Sector_Id = i.Sector_Id;
                    pri.prinoofslab = 2;

                   pri.prislab1 = 1;
                   pri.prislab2 =1;
                   pri.prislab3 =1;
                  pri.prislab4 = 1;

                  pri.priupto1 =1;
                  pri.priupto2 = 1;
                 pri.priupto3 = 1;
                 pri.priupto4 = 1;

                    cs.Company_id = empmodel.Company_Id;
                    cs.Sector_Id = i.Sector_Id;

                    cs.Exslab1 = 1;
                    cs.Exslab2 = 1;

                    dtdc_Ecommerce.Company_id = empmodel.Company_Id;
                    dtdc_Ecommerce.Sector_Id = i.Sector_Id;
                    dtdc_Ecommerce.EcomPslab1 = 1;
                    dtdc_Ecommerce.EcomPslab2 = 1;
                    dtdc_Ecommerce.EcomPslab3 = 1;
                    dtdc_Ecommerce.EcomPslab4 = 1;
                    dtdc_Ecommerce.EcomGEslab1 = 1;
                    dtdc_Ecommerce.EcomGEslab2 = 1;
                    dtdc_Ecommerce.EcomGEslab3 = 1;
                    dtdc_Ecommerce.EcomGEslab4 = 1;
                    dtdc_Ecommerce.EcomPupto1 = 1;
                    dtdc_Ecommerce.EcomPupto2 = 1;
                    dtdc_Ecommerce.EcomPupto3 = 1;
                    dtdc_Ecommerce.EcomPupto4 = 1;
                    dtdc_Ecommerce.EcomGEupto1 = 1;
                    dtdc_Ecommerce.EcomGEupto2 = 1;
                    dtdc_Ecommerce.EcomGEupto3 = 1;
                    dtdc_Ecommerce.EcomGEupto4 = 1;
                    dtdc_Ecommerce.NoOfSlabN = 2;
                    dtdc_Ecommerce.NoOfSlabS = 2;


                    db.Ratems.Add(dox);
                    db.Nondoxes.Add(ndox);
                    db.express_cargo.Add(cs);
                    db.Priorities.Add(pri);
                    db.Dtdc_Ecommerce.Add(dtdc_Ecommerce);

                    j++;

                }

                int p = 0;

                var basicplu = db.dtdcPlus.Where(m => m.Company_id == "Basic_Ts").ToArray();
                var basicptp = db.Dtdc_Ptp.Where(m => m.Company_id == "Basic_Ts").ToArray();


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

                    dtplu.Company_id = empmodel.Company_Id;

                    dtplu.Upto500gm = basicplu[p].Upto500gm;
                    dtplu.U10to25kg = basicplu[p].U10to25kg;
                    dtplu.U25to50 = basicplu[p].U25to50;
                    dtplu.U50to100 = basicplu[p].U50to100;
                    dtplu.add100kg = basicplu[p].add100kg;
                    dtplu.Add500gm = basicplu[p].Add500gm;


                    stptp.Company_id = empmodel.Company_Id;
                    stptp.PUpto500gm = basicptp[p].PUpto500gm;
                    stptp.PAdd500gm = basicptp[p].PAdd500gm;
                    stptp.PU10to25kg = basicptp[p].PU10to25kg;
                    stptp.PU25to50 = basicptp[p].PU25to50;
                    stptp.Padd100kg = basicptp[p].Padd100kg;
                    stptp.PU50to100 = basicptp[p].PU50to100;

                    stptp.P2Upto500gm = basicptp[p].P2Upto500gm;
                    stptp.P2Add500gm = basicptp[p].P2Add500gm;
                    stptp.P2U10to25kg = basicptp[p].P2U10to25kg;
                    stptp.P2U25to50 = basicptp[p].P2U25to50;
                    stptp.P2add100kg = basicptp[p].P2add100kg;
                    stptp.P2U50to100 = basicptp[p].P2U50to100;

                    db.dtdcPlus.Add(dtplu);
                    db.Dtdc_Ptp.Add(stptp);

                    p++;

                }


                db.SaveChanges();

                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id).FirstOrDefault();                

                ViewBag.Company = new Company();

                ViewBag.Dox = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id).ToList();



                ViewBag.SuccessCompany = "Company Added SuccessFully";


                double? selectedval = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


                List<SelectListItem> items = new List<SelectListItem>();

                items.Add(new SelectListItem { Text = "0", Value = "0" });
                items.Add(new SelectListItem { Text = "50", Value = "50" });
                items.Add(new SelectListItem { Text = "100", Value = "100" });

                if (selectedval == null)
                {
                    var selected = items.Where(x => x.Value == "0").First();
                    selected.Selected = true;
                }
                else
                {


                    var selected = items.Where(x => x.Value == selectedval.ToString()).First();
                    selected.Selected = true;
                }



                ViewBag.Minimum_Risk_Charge = items;


                TempData["Success"] = "Company Added SuccessFully!";
                TempData["ShowLoader"] = false;


                return RedirectToAction("Index","RateMaster", new { id=empmodel.Company_Id });


            }

            ViewBag.Company = new CompanyModel();

            //ViewBag.SuccessCompany = "Company Failed";To Ramain On Same Tab


            double? selectedval1 = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "0", Value = "0" });
            items1.Add(new SelectListItem { Text = "50", Value = "50" });
            items1.Add(new SelectListItem { Text = "100", Value = "100" });

            if (selectedval1 == null)
            {
                var selected = items1.Where(x => x.Value == "0").First();
                selected.Selected = true;
            }
            else
            {
              

                var selected = items1.Where(x => x.Value == selectedval1.ToString()).First();
                selected.Selected = true;
            }

            ViewBag.Minimum_Risk_Charge = items1;



            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

            ViewBag.Pf_code = empmodel.Pf_code;//new SelectList(db.Franchisees, "PF_Code", "PF_Code",empmodel.Pf_code);

            return View("AddCompany", empmodel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterEditCompany(CompanyModel empmodel, float[] slab1arr, float[] slab2arr, float[] slab3arr, float[] slab4arr, float[] Upto, int[] Sector_Id, int? only, string selected_tab)
        {
            ViewBag.sr = db.Sectors.ToList();


            var abc = db.Companies.Where(m => m.Company_Id.ToLower() == empmodel.Company_Id.ToLower()).FirstOrDefault();

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                TempData["ShowLoader"] = true;


                // Business Logic
                Company comp = new Company();

                comp.Company_Id = empmodel.Company_Id;
                comp.Phone = empmodel.Phone;
                comp.Email = empmodel.Email;
                comp.Insurance = empmodel.Insurance;
                comp.Minimum_Risk_Charge = empmodel.Minimum_Risk_Charge;
                comp.Other_Details = empmodel.Other_Details;
                comp.Fuel_Sur_Charge = empmodel.Fuel_Sur_Charge;
                comp.Topay_Charge = empmodel.Topay_Charge;
                comp.Cod_Charge = empmodel.Cod_Charge;
                comp.Gec_Fuel_Sur_Charge = empmodel.Gec_Fuel_Sur_Charge;
                comp.Pf_code = empmodel.Pf_code;
                comp.Company_Address = empmodel.Company_Address;
                comp.Company_Name = empmodel.Company_Name;
                comp.Datetime_Comp = empmodel.Datetime_Comp;
                comp.Gst_No = empmodel.Gst_No;
                comp.Pan_No = empmodel.Pan_No;
                comp.Royalty_Charges = empmodel.Royalty_Charges;
                comp.D_Docket = empmodel.D_Docket;
                comp.P_Docket = empmodel.P_Docket;
                comp.E_Docket = empmodel.E_Docket;
                comp.V_Docket = empmodel.V_Docket;
                comp.I_Docket = empmodel.I_Docket;
                comp.N_Docket = empmodel.N_Docket;
                //comp.Password = empmodel.Password;
                //comp.Username = empmodel.Username;
                comp.G_Docket = empmodel.G_Docket;

            
                ModelState.Clear();
                TempData["CompanyId"] = empmodel.Company_Id;


                var local = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).FirstOrDefault();

                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(comp).State = EntityState.Modified;
                db.SaveChanges();
             
                //<-------------risk surch charge dropdown--------------->

                double? selectedval = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


                List<SelectListItem> items = new List<SelectListItem>();

                items.Add(new SelectListItem { Text = "0", Value = "0" });
                items.Add(new SelectListItem { Text = "50", Value = "50" });
                items.Add(new SelectListItem { Text = "100", Value = "100" });

                if (selectedval == null)
                {
                    var selected = items.Where(x => x.Value == "0").First();
                    selected.Selected = true;
                }
                else
                {
                   

                    var selected = items.Where(x => x.Value == selectedval.ToString()).First();
                    selected.Selected = true;
                }



                ViewBag.Minimum_Risk_Charge = items;




                //<-------------risk surch charge dropdown--------------->
                //updating all tables pf code

                //int[] secotrs = db.Sectors.Where(m => m.Pf_code == empmodel.Pf_code).Select(m=>m.Sector_Id).ToArray();


                //int [] doxlist = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id).Select(m => m.Rete_Id).ToArray();
                //int [] nonlist = db.Nondoxes.Where(m => m.Company_id == empmodel.Company_Id).Select(m => m.Non_ID).ToArray();
                //int [] cslist = db.express_cargo.Where(m => m.Company_id == empmodel.Company_Id).Select(m => m.Exp_Id).ToArray();


                //int j = 0;

                //int doxcnt= doxlist.Count();
                //int nondoxcnt=nonlist.Count();
                //int escnt=cslist.Count();  

                //for(int i = 0; i < doxcnt; i++)
                //{
                //    Ratem dox = new Ratem();
                //    int d = doxlist[i];
                //    dox=db.Ratems.Where(m=>m.Rete_Id==d).FirstOrDefault();
                //    dox.Sector_Id = secotrs[i];
                //    db.Entry(dox).State=EntityState.Modified;
                //    db.SaveChanges();

                //}
                //for(int k = 0; k < nondoxcnt; k++)
                //{
                //    Nondox ndox = new Nondox();
                //    int d = nonlist[k];
                //    ndox = db.Nondoxes.Where(m => m.Non_ID == d).FirstOrDefault();
                //    ndox.Sector_Id = secotrs[k];
                //    db.Entry(ndox).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
                //for(int l = 0; l < escnt; l++)
                //{
                //    express_cargo cs = new express_cargo();
                //    int d= cslist[l];
                //    cs = db.express_cargo.Where(m => m.Exp_Id == d).FirstOrDefault();
                //    cs.Sector_Id= secotrs[l];
                //    db.Entry(cs).State = EntityState.Modified;  
                //    db.SaveChanges();   
                //}
                //for(int i=0;i < cnt;i++)
                //{                 

                //    Ratem dox = new Ratem();
                //    Nondox ndox = new Nondox();
                //    express_cargo cs = new express_cargo();

                //    int d = doxlist[i], n = nonlist[i], ex=cslist[i];

                //    dox = db.Ratems.Where(m => m.Rete_Id == d).FirstOrDefault();
                //    ndox = db.Nondoxes.Where(m => m.Non_ID ==n).FirstOrDefault();
                //    cs = db.express_cargo.Where(m => m.Exp_Id == ex).FirstOrDefault();

                //    dox.Sector_Id = secotrs[i];
                //    ndox.Sector_Id = secotrs[i];
                //    cs.Sector_Id = secotrs[i];


                //    db.Entry(dox).State = EntityState.Modified;
                //    db.Entry(ndox).State = EntityState.Modified;
                //    db.Entry(cs).State = EntityState.Modified;



                //    db.SaveChanges();

                //    j++;

                //}


              
              

                ViewBag.Message = "Company Updated SuccessFully";
                TempData["ShowLoader"] = false;
                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == empmodel.Company_Id).FirstOrDefault();

                ViewBag.Company = new Company();


                ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();


                ViewBag.SuccessCompany = "Company Added SuccessFully";

                ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", empmodel.Pf_code);

                return PartialView("RateMasterEditCompany", empmodel);

            }

            double? selectedval1 = db.Companies.Where(m => m.Company_Id == empmodel.Company_Id).Select(m => m.Minimum_Risk_Charge).FirstOrDefault();


            List<SelectListItem> items1 = new List<SelectListItem>();

            items1.Add(new SelectListItem { Text = "0", Value = "0" });
            items1.Add(new SelectListItem { Text = "50", Value = "50" });
            items1.Add(new SelectListItem { Text = "100", Value = "100" });

            if (selectedval1 == null)
            {
                var selected = items1.Where(x => x.Value == "0").First();
                selected.Selected = true;
            }
            else
            {
                //foreach (var item in items)
                //{
                //    if (item.Value == selectedval.ToString())
                //    {
                //        item.Selected = true;
                //        break;
                //    }
                //}

                var selected = items1.Where(x => x.Value == selectedval1.ToString()).First();
                selected.Selected = true;
            }

            ViewBag.Minimum_Risk_Charge = items1;

            ViewBag.Company = new Company();

            //ViewBag.SuccessCompany = "Company Failed";To Ramain On Same Tab

            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == "Bala").ToList();

            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", empmodel.Pf_code);

            return PartialView("RatemasterEditCompany", empmodel);
        }

        [HttpPost]
        public ActionResult RateMasterEcommerce(FormCollection fc, string comppid)
        {

            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {

                var ecom_idarray = fc.GetValues("item.Ecom_id");
                var ecom_idarray_Non_ID_GC = fc.GetValues("Non_ID_GC");
                

                var EcomPslab1 = fc.GetValues("item.EcomPslab1");
                var EcomPslab2 = fc.GetValues("item.EcomPslab2");
                var EcomPslab3 = fc.GetValues("item.EcomPslab3");
                var EcomPslab4 = fc.GetValues("item.EcomPslab4");

                var EcomGEslab1 = fc.GetValues("item.EcomGEslab1");
                var EcomGEslab2 = fc.GetValues("item.EcomGEslab2");
                var EcomGEslab3 = fc.GetValues("item.EcomGEslab3");
                var EcomGEslab4 = fc.GetValues("item.EcomGEslab4");

                //var EcomPupto1 = fc.GetValues("item.EcomPupto1");
                //var EcomPupto2 = fc.GetValues("item.EcomPupto2");
                //var EcomPupto3 = fc.GetValues("item.EcomPupto3");
                //var EcomPupto4 = fc.GetValues("item.EcomPupto4");

                //var EcomGEupto1 = fc.GetValues("item.EcomGEupto1");
                //var EcomGEupto2 = fc.GetValues("item.EcomGEupto2");
                //var EcomGEupto3 = fc.GetValues("item.EcomGEupto3");
                //var EcomGEupto4 = fc.GetValues("item.EcomGEupto4");

                var Auptoarray = fc.GetValues("AUpto");
                var Suptoarray = fc.GetValues("SUpto");

                var NoOfSlabN = fc.GetValues("item.NoOfSlabN");
                var NoOfSlabS = fc.GetValues("item.NoOfSlabS");


                for (int i = 0; i < ecom_idarray.Count(); i++)
                {
                    if (EcomPslab1[i] == "")
                    {
                        EcomPslab1[i] = "0";
                    }
                    if (EcomPslab2[i] == "")
                    {
                        EcomPslab2[i] = "0";
                    }
                    if (EcomPslab3[i] == "")
                    {
                        EcomPslab3[i] = "0";
                    }
                    if (EcomPslab4[i] == "")
                    {
                        EcomPslab4[i] = "0";
                    }
                    
                }

                for(int k = 0; k < ecom_idarray_Non_ID_GC.Count(); k++)
                {
                    if (EcomGEslab1[k] == "")
                    {
                        EcomGEslab1[k] = "0";
                    }
                    if (EcomGEslab2[k] == "")
                    {
                        EcomGEslab2[k] = "0";
                    }
                    if (EcomGEslab3[k] == "")
                    {
                        EcomGEslab3[k] = "0";
                    }
                    if (EcomGEslab4[k] == "")
                    {
                        EcomGEslab4[k] = "0";
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

                Dtdc_Ecommerce rm = new Dtdc_Ecommerce();

                for (int i = 0; i < ecom_idarray.Count(); i++)
                {


                     rm = db.Dtdc_Ecommerce.Find(Convert.ToInt16(ecom_idarray[i]));


                    rm.EcomPslab1 = Convert.ToDouble(EcomPslab1[i]);
                    rm.EcomPslab2 = Convert.ToDouble(EcomPslab2[i]);
                    rm.EcomPslab3 = Convert.ToDouble(EcomPslab3[i]);
                    rm.EcomPslab4 = Convert.ToDouble(EcomPslab4[i]);

                    rm.EcomPupto1 = Convert.ToDouble(Auptoarray[0]);
                    rm.EcomPupto2 = Convert.ToDouble(Auptoarray[1]);
                    rm.EcomPupto3 = Convert.ToDouble(Auptoarray[2]);
                    rm.EcomPupto4 = Convert.ToDouble(Auptoarray[3]);

                    rm.NoOfSlabN = Convert.ToInt16(NoOfSlabN[0]);
                   
                   

                }

                for (int i = 0; i < ecom_idarray_Non_ID_GC.Count(); i++)
                {

                    rm = db.Dtdc_Ecommerce.Find(Convert.ToInt16(ecom_idarray_Non_ID_GC[i]));
                    rm.EcomGEslab1 = Convert.ToDouble(EcomGEslab1[i]);
                    rm.EcomGEslab2 = Convert.ToDouble(EcomGEslab2[i]);
                    rm.EcomGEslab3 = Convert.ToDouble(EcomGEslab3[i]);
                    rm.EcomGEslab4 = Convert.ToDouble(EcomGEslab4[i]);
                    rm.EcomGEupto1 = Convert.ToDouble(Suptoarray[0]);
                    rm.EcomGEupto2 = Convert.ToDouble(Suptoarray[1]);
                    rm.EcomGEupto3 = Convert.ToDouble(Suptoarray[2]);
                    rm.EcomGEupto4 = Convert.ToDouble(Suptoarray[3]);
                    rm.NoOfSlabS = Convert.ToInt16(NoOfSlabS[0]);

                   
                }
                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();

                    var compid = comppid;

                ViewBag.Message = "E-commerce Updated SuccessFully";


                var getEcom = db.Dtdc_Ecommerce.Where(m => m.Company_id == CompanyId).Include(e => e.Sector).OrderBy(m => m.Sector.Priority).ToList();


                var getEcomCount = getEcom.Where(x => x.Sector.BillEcomPrio == true).Count();
                ViewBag.EcomPCount = getEcomCount;

                if (getEcomCount > 0)
                {
                    ViewBag.comP = getEcom.Where(x => x.Sector.BillEcomPrio == true).FirstOrDefault();
                }



                var EcomGECount = getEcom.Where(x => x.Sector.BillEcomGE == true).Count();
                ViewBag.EcomGECount = EcomGECount;

                if (EcomGECount > 0)
                {
                    ViewBag.comGC = getEcom.Where(x => x.Sector.BillEcomGE == true).FirstOrDefault();
                }


                ViewBag.Dtdc_Ecommerce = getEcom;

                return PartialView("RateMasterEcommerce", getEcom);

            }

            return PartialView("RateMasterEcommerce", fc);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterDox(int? only, FormCollection fc, float[] slab1, string comppid)
        
        
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //} 
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}
            // var CompanyId = TempData.Peek("CompanyId").ToString();


            ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


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
                    rm.Company_id = CompanyId;




                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();


                }

                var compid = comppid;

                ViewBag.Message = "Dox Updated SuccessFully";
                @ViewBag.Slabs = db.Ratems.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RatemasterDox", db.Ratems.Where(m => m.Company_id == compid &&  m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());
            }
            return PartialView("RatemasterDox", fc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterNonDox(int? only, FormCollection fc, string comppid)
        {

            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
         
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}
            //if (only == 2)
            //{
            //    //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            //}

            if (ModelState.IsValid)
            {
                TempData["ShowLoader"] = true;

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
                var NoofslabN= fc.GetValues("NonNoOfSlabN");
                //var NoofslabS = fc.GetValues("item.NoOfSlabS");
                  var NoofslabS = fc.GetValues("NonNoOfSlabS");



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

                   if(rm.Sector.BillNonAir==true && rm.Sector.BillNonSur == true)
                    {
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
                        rm.Company_id = CompanyId;
                        rm.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                        rm.NoOfSlabN = Convert.ToInt16(NoofslabN[0]);
                        rm.NoOfSlabS = Convert.ToInt16(NoofslabS[0]);

                        db.Entry(rm).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                   
                }


                var compid = comppid;

                ViewBag.Message = "NonDox Updated SuccessFully";
                TempData["ShowLoader"] = false;
             
                var getNonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).OrderBy(m => m.Sector.Priority).ToList();
                ViewBag.NonDoxAir = getNonDox.Where(m => m.Sector.BillNonAir == true).FirstOrDefault();
                ViewBag.NonDoxSur = getNonDox.Where(m => m.Sector.BillNonSur == true).FirstOrDefault();
                ViewBag.nonDoxAirCount = getNonDox.Where(x => x.Sector.BillNonAir == true).Count();
                ViewBag.nonDoxSurCount = getNonDox.Where(x => x.Sector.BillNonSur == true).Count();

                ViewBag.NonDox = getNonDox;
                @ViewBag.Slabs1 = getNonDox.FirstOrDefault();
                //&& m.Sector.BillN == true

                return PartialView("RatemasterNonDox", getNonDox);

            }
            return PartialView("RatemasterNonDox", fc);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterPlus(float? go149, float? go99, FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {
                TempData["ShowLoader"] = true;
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

                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var compid = comppid;

                ViewBag.Message = "Dtdc Plus Updated SuccessFully";
                TempData["ShowLoader"] = false;
                @ViewBag.Slabs = db.Dtdc_Ptp.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RatemasterPlus", db.dtdcPlus.Where(m => m.Company_id == compid).ToList());
            }
            return PartialView("RatemasterPlus", fc);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterPtp(FormCollection fc, string comppid)
        {
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {
                TempData["ShowLoader"] = true;

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
                    rm.PU25to50= Convert.ToDouble(PU25to50arayy[i]);
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

                var compid = comppid;

                ViewBag.Message = "DtdcPtp Updated SuccessFully";
                TempData["ShowLoader"] = false;
                @ViewBag.Slabs = db.Dtdc_Ptp.Where(m => m.Company_id == compid).FirstOrDefault();

                return PartialView("RatemasterPtp", db.Dtdc_Ptp.Where(m => m.Company_id == compid).Include(e => e.Sector).ToList());

            }

            return PartialView("RatemasterPtp", fc);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult RatemasterCargo(float? Upto, FormCollection fc, string comppid)
        {

            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;
            if (ModelState.IsValid)
            {
                TempData["ShowLoader"] = true;
                var Exp_Idarray = fc.GetValues("item.Exp_Id");
                var Exslab1array = fc.GetValues("item.Exslab1");
                var Exslab2arayy = fc.GetValues("item.Exslab2");
                var Sector_Idarayy = fc.GetValues("item.Sector_Id");


                for (int i = 0; i < Exslab1array.Count(); i++)
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

                for (int i = 0; i < Exslab1array.Count(); i++)
                {

                    express_cargo rm = db.express_cargo.Find(Convert.ToInt16(Exp_Idarray[i]));

                    rm.Exslab1 = Convert.ToDouble(Exslab1array[i]);
                    rm.Exslab2 = Convert.ToDouble(Exslab2arayy[i]);

                    ViewBag.Message = "Express Cargo Updated SuccessFully";
                    TempData["ShowLoader"] = false;
                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return PartialView("RateMasterExpressCargo", db.express_cargo.Where(m=>m.Company_id== comppid && m.Sector.BillD == true).OrderBy(m => m.Sector.Priority).ToList());

        }


        [HttpPost]
        public ActionResult Priority(int? only, FormCollection fc, float[] slab1, string comppid)
        {
            //var CompanyId = TempData.Peek("CompanyId").ToString();
            comppid = comppid.Replace("__", "&").Replace("xdotx", "."); ;
            var CompanyId = comppid;
            ViewBag.companyid = comppid;

            //ViewBag.Dox = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            //ViewBag.NonDox = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();


            if (ModelState.IsValid)
            {

                var rateidarray = fc.GetValues("item.pri_id");
                var slab1arayy = fc.GetValues("item.prislab1");
                var slab2arayy = fc.GetValues("item.prislab2");
                var slab3arayy = fc.GetValues("item.prislab3");
                var slab4arayy = fc.GetValues("item.prislab4");
                var uptoarray = fc.GetValues("Upto");
                var noofslab = fc.GetValues("item.prinoofslab");

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

                    Priority pr = db.Priorities.Find(Convert.ToInt16(rateidarray[i]));

                    pr.prislab1 = Convert.ToDouble(slab1arayy[i]);
                    pr.prislab2 = Convert.ToDouble(slab2arayy[i]);
                    pr.prislab3 = Convert.ToDouble(slab3arayy[i]);
                    pr.prislab4 = Convert.ToDouble(slab4arayy[i]);
                    pr.priupto1 = Convert.ToDouble(uptoarray[0]);
                    pr.priupto2 = Convert.ToDouble(uptoarray[1]);
                    pr.priupto3 = Convert.ToDouble(uptoarray[2]);
                    pr.priupto4 = Convert.ToDouble(uptoarray[3]);
                    pr.Sector_Id = Convert.ToInt16(sectoridarray[i]);
                    pr.prinoofslab = Convert.ToInt16(noofslab[0]);
                    pr.Company_id = CompanyId;




                    db.Entry(pr).State = EntityState.Modified;
                    db.SaveChanges();


                }

                var compid = comppid;

                ViewBag.Message = "Priority Updated SuccessFully";

                var getPrio = db.Priorities.Where(m => m.Company_id == compid && m.Sector.BillPriority == true).OrderBy(m => m.Sector.Priority).ToList();
                @ViewBag.Slabspri = getPrio.FirstOrDefault();

                return PartialView("Priority", getPrio);
            }
            return PartialView("Priority", fc);
        }



        public ActionResult RateMaster()
        {
            List<Sector> sr = db.Sectors.ToList();

            return View(sr);
        }

        [HttpPost]
        public ActionResult RateMaster(float [] slab1arr, float[] slab2arr, float[] slab3arr, float[] slab4arr, float[] Upto, int[] Sector_Id, int ? only)
        {
            if(only==2)
            {
                //tO clear array//Array.Clear(slab3arr, 0, slab3arr.Length);
                //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            }
            if (only == 2)
            {
                //tO clear array//Array.Clear(slab4arr, 0, slab3arr.Length);
            }



            for (int i = 0; i < Sector_Id.Count(); i++)
            {

                Ratem rm = new Ratem();

                rm.slab1 = slab1arr[i];
                rm.slab2 = slab2arr[i];
                rm.slab3 = slab3arr[i];
                rm.slab4 = slab4arr[i];                
                rm.Sector_Id= Sector_Id[i];

                db.Ratems.Add(rm);
                db.SaveChanges();      


            }


            return View();
        }


        [HttpGet]
        public ActionResult ReportPrinterMethod(string id)
        {
            {

                LocalReport lr = new LocalReport();

                var CompanyId = id;


                Company company = db.Companies.Where(m => m.Company_Id == CompanyId).FirstOrDefault();

                var dataset2 = db.Ratems.Where(m => m.Company_id == CompanyId && m.Sector.BillD==true).OrderBy(x=>x.Sector.Priority).ToList();

                var dataset3 = db.Nondoxes.Where(m => m.Company_id == CompanyId).OrderBy(m=>m.Sector.Priority).ToList();

                var dataset4 = (from a in db.Sectors
                                join ab in db.Ratems on a.Sector_Id equals ab.Sector_Id
                                where ab.Company_id == CompanyId && a.BillD == true
                                orderby a.Priority
                                select new
                                {
                                    a.BillD,
                                    a.BillN,
                                    a.CashD,
                                    a.CashN,
                                    a.Sector_Name,
                                    ab.slab1,
                                    ab.slab2,
                                    ab.slab3,
                                    ab.slab4,
                                    ab.Uptosl1,
                                    ab.Uptosl2,
                                    ab.Uptosl3,
                                    ab.Uptosl4,
                                    ab.NoOfSlab
                                }).ToList();

                var dataset5 = (from a in db.Sectors
                                join ab in db.Nondoxes on a.Sector_Id equals ab.Sector_Id
                                where ab.Company_id == CompanyId /*&& a.BillN == true*/
                                orderby a.Priority
                                select new

                                {
                                    a.BillNonAir,
                                    a.BillNonSur,

                                    a.Sector_Name,
                                    ab.Aslab1,
                                    ab.Aslab2,
                                    ab.Aslab3,
                                    ab.Aslab4,
                                    ab.Sslab1,
                                    ab.Sslab2,
                                    ab.Sslab3,
                                    ab.Sslab4,
                                    ab.AUptosl1,
                                    ab.AUptosl2,
                                    ab.AUptosl3,
                                    ab.AUptosl4,
                                    ab.SUptosl1,
                                    ab.SUptosl2,
                                    ab.SUptosl3,
                                    ab.SUptosl4,
                                    ab.NoOfSlabN,
                                    ab.NoOfSlabS
                                }).ToList();

                var dataset6 = db.dtdcPlus.Where(m => m.Company_id == CompanyId).ToList();

                var dataset7 = db.Dtdc_Ptp.Where(m => m.Company_id == CompanyId).ToList();

                var DataSet8 =(from a in db.Sectors
                 join ab in db.Priorities on a.Sector_Id equals ab.Sector_Id
                 where ab.Company_id == CompanyId && a.BillPriority== true
                 orderby a.Priority
                 select new
                 {
                     a.BillPriority,
                     a.Sector_Name,
                     ab.priupto1,
                     ab.priupto2,
                     ab.priupto3,
                     ab.priupto4,
                     ab.prislab1,
                     ab.prislab2,
                     ab.prislab3,
                   
                     ab.prinoofslab
                 }).ToList();

                var DataSet9 =(from a in db.Sectors
                 join ab in db.express_cargo on a.Sector_Id equals ab.Sector_Id
                 where ab.Company_id == CompanyId/* && a.BillN == true */&& a.BillExpCargo== true   
                 orderby a.Priority
                 select new
                 {

                     a.Sector_Name,
                     ab.Exslab1,
                     ab.Exslab2,
                   
                 }).ToList();

              // var DataSet10= db.Dtdc_Ecommerce.Where(m => m.Company_id == CompanyId).Include(e => e.Sector).OrderBy(m => m.Sector.Priority).ToList();

                var DataSet10= (from a in db.Sectors
                                join ab in db.Dtdc_Ecommerce on a.Sector_Id equals ab.Sector_Id
                                where ab.Company_id==CompanyId orderby a.Priority
                                select new
                                {
                                   a.Sector_Name,
                                   a.Pf_code,
                                   a.BillEcomPrio,
                                   a.BillEcomGE,
                                   ab.Sector_Id,
                                   ab.Ecom_id,
                                   ab.EcomPslab1,
                                   ab.EcomPslab2,
                                   ab.EcomPslab3,
                                   ab.EcomPslab4,
                                   ab.EcomGEslab1,
                                   ab.EcomGEslab2,
                                   ab.EcomGEslab3,
                                   ab.EcomGEslab4,
                                   ab.Company_id,
                                   ab.EcomPupto1,
                                   ab.EcomPupto2,
                                   ab.EcomPupto3,
                                   ab.EcomPupto4,
                                   ab.EcomGEupto1,
                                   ab.EcomGEupto2,
                                   ab.EcomGEupto3,
                                   ab.EcomGEupto4,
                                   ab.NoOfSlabN,
                                   ab.NoOfSlabS,
                                
                                }
                                
                                ).ToList();
               
               
                string Pfcode = company.Pf_code;

                //var logo = db.Franchisees.Where(m => m.PF_Code == Pfcode).FirstOrDefault();
                //logo.LogoFilePath = (logo.LogoFilePath== null || logo.LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : logo.LogoFilePath;


                var dataset = db.Franchisees.Where(m => m.PF_Code == Pfcode).ToList();
                dataset.FirstOrDefault().LogoFilePath = (dataset.FirstOrDefault().LogoFilePath == null || dataset.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset.FirstOrDefault().LogoFilePath;

                var dataset1 = db.Companies.Where(m => m.Company_Id == CompanyId).ToList();

                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "QuotationReport.rdlc");

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                lr.Refresh();

                lr.EnableExternalImages = true;

                ReportDataSource rd = new ReportDataSource("DataSet", dataset);
                ReportDataSource rd1 = new ReportDataSource("DataSet1", dataset1);
                ReportDataSource rd2 = new ReportDataSource("DataSet2", dataset2);
                ReportDataSource rd3 = new ReportDataSource("DataSet3", dataset3);
                ReportDataSource rd4 = new ReportDataSource("DataSet4", dataset4);
                ReportDataSource rd5 = new ReportDataSource("DataSet5", dataset5);
                ReportDataSource rd6 = new ReportDataSource("DataSet6", dataset6);
                ReportDataSource rd7 = new ReportDataSource("DataSet7", dataset7);
                ReportDataSource rd8 = new ReportDataSource("DataSet8", DataSet8);
                ReportDataSource rd9 = new ReportDataSource("DataSet9", DataSet9);
                ReportDataSource rd10 = new ReportDataSource("DataSet10", DataSet10);

                //if (logo.LogoFilePath == null)
                //{
                //    ReportParameter rp = new ReportParameter("img_logo", "file:///"+Server.MapPath("~/UploadedLogo/goeasy.png"));
                //    lr.SetParameters(rp);
                //}
                //else
                //{ 

                // // ReportParameter rp = new ReportParameter("img_logo", "file:///" + logo.LogoFilePath);

              //   ReportParameter rp= new ReportParameter("img_logo", "file:///" + logo.LogoFilePath);
              //      lr.SetParameters(rp);
              //  }
                

                lr.Refresh();

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);
                lr.DataSources.Add(rd4);
                lr.DataSources.Add(rd5);
                lr.DataSources.Add(rd6);
                lr.DataSources.Add(rd7);
                lr.DataSources.Add(rd8);
                lr.DataSources.Add(rd9);
                lr.DataSources.Add(rd10);

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExte;

                string deviceInfo =
                    "<DeviceInfo>" +
                    "<OutputFormat>" + "pdf" + "</OutputFormat>" +
                    "<PageHeight>11in</PageHeight>" +
                   "<Margintop>0.1in</Margintop>" +
                     "<Marginleft>0.1in</Marginleft>" +
                      "<Marginright>0.1in</Marginright>" +
                       "<Marginbottom>0.5in</Marginbottom>" +
                       "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderByte;
                //try
                //{
                    renderByte = lr.Render
            (reportType,
            deviceInfo,
            out mimeType,
            out encoding,
            out fileNameExte,
            out streams,
            out warnings
            );

                    return File(renderByte, mimeType);
                //}
                //catch(Exception ex ) {

                //    return RedirectToAction("Index", new { id=id});
                
                //}

              
            }
        }
    }
}