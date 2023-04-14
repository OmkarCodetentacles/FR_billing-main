using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    public class LoginController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Login
        public ActionResult Login()
        {            
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {

            var obj = db.Employees.Where(a => a.email.Equals(login.Username) && a.E_Password.Equals(login.Password)).FirstOrDefault();
            //var firmlist = db.FirmDetails.ToList();
            if (ModelState.IsValid)
            {
                if (obj != null)
                {
                    Session["PfID"] = obj.PF_Code.ToString();
                   // Session["firmlist"] = firmlist;
                    //Session["EmpId"]=
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(login);
        }
       

        }


}
