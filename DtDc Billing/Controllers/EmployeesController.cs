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
using System.Web.Security;

namespace DtDc_Billing.Controllers
{
    [SessionUserModule]
    public class EmployeesController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.Franchisee);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.PF_Code = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Emp_Id,Emp_Name,email,Phone,PF_Code,Emp_Role")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PF_Code = new SelectList(db.Franchisees, "PF_Code", "F_Address", employee.PF_Code);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.PF_Code = new SelectList(db.Franchisees, "PF_Code", "F_Address", employee.PF_Code);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Emp_Id,Emp_Name,email,Phone,PF_Code,Emp_Role")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PF_Code = new SelectList(db.Franchisees, "PF_Code", "F_Address", employee.PF_Code);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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

        public ActionResult EmpLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        [HttpPost]
        public ActionResult EmpLogin(EmpLogin emplogin, string ReturnUrl)
        {
            var obj = db.Users.Where(a => a.Email.ToLower().Equals(emplogin.UserName.ToLower()) && a.Password_U.Equals(emplogin.Password) && a.Usertype.Contains("CashCounter")).FirstOrDefault();

            if (obj != null)
            {
                Session["EmpId"] = obj.User_Id.ToString();
                Session["pfCode"] = obj.PF_Code.ToString();
                Session["EmpName"] = obj.Name.ToString();


                string decodedUrl = "";
                if (!string.IsNullOrEmpty(ReturnUrl))
                    decodedUrl = Server.UrlDecode(ReturnUrl);

                //Login logic...

                if (Url.IsLocalUrl(decodedUrl))
                {
                    return Redirect(decodedUrl);
                }
                else
                {
                    return RedirectToAction("Printreceipt", "Booking");
                }


            }
            else
            {
                ModelState.AddModelError("LoginAuth", "Username ro Password Is Incorrect");
            }

            return View(emplogin);
        }


        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Adminlogin", "Admin");

        }

    }
}
