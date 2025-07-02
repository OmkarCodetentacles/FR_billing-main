using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    
    public class ChildActionsController : Controller
    {
        // GET: ChildActions

        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();


        public ActionResult Modaldata(string url)
        {

         
            ViewBag.firmcount = db.FirmDetails.Count();
          
            ViewBag.Firmname = db.FirmDetails.Select(m => m.Firm_Id).FirstOrDefault();
               
            ViewBag.Firm_Id = new SelectList(db.FirmDetails, "Firm_Id", "Firm_Name");
            //ViewBag.Firm_Id = Session["firmlist"] as List<FirmDetail>;
            ViewBag.url = url;
            return PartialView("Modaldata");

        }



    }


}