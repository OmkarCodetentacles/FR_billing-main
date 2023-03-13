using DtDc_Billing.Entity_FR;
using DtDc_Billing.CustomModel;
using DtDc_Billing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionAdmin]  
    public class StationeryController : Controller
    {
        private db_a71c08_elitetokenEntities db = new db_a71c08_elitetokenEntities();
              

        // GET: Stationery
        public ActionResult Add()
        {

            return View();
        }


        [HttpPost]
        public ActionResult Add(StationaryModel stationary)
        {
            string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


            if (ModelState.IsValid)
            {

                string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

                var dataFid = (from d in db.Franchisees
                              where d.PF_Code == strpf
                               select d.F_Id).FirstOrDefault();

                Stationary St = new Stationary();

                St.Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString();
                St.startno = stationary.startno;
                St.endno = stationary.endno;
                St.noofbooks = stationary.noofbooks;
                St.noofleafs = stationary.noofleafs;
                St.Invoiceno = stationary.Invoiceno;
                St.Status = 0;
                string invdate = DateTime.ParseExact(stationary.temprecdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                St.Expiry_Date = Convert.ToDateTime(invdate);

                St.tempExpiry_Date = stationary.tempExpiry_Date;
                St.temprecdate = stationary.temprecdate;
                St.recieptdate = stationary.recieptdate;
                St.fid = Convert.ToInt32(dataFid);
                db.Stationaries.Add(St);
                db.SaveChanges();        

                ViewBag.Message = "Stationary Added SuccessFully";
                ModelState.Clear();
                return View();

            }

          
            return View(stationary);
        }



        public ActionResult Issue()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Issue(StationaryIssueModel issueModel)
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            if (ModelState.IsValid)
            {
                Issue issue = new Issue();

                issue.Pf_code = pfcode;

                issue.startno = issueModel.startno;
                issue.endno = issueModel.endno;
                issue.noofleafs = issueModel.noofleafs;
                issue.Inssuedate = issueModel.Inssuedate;
                issue.Comapny_Id = issueModel.Comapny_Id;
                issue.EmployeeName = issueModel.EmployeeName;

                db.Issues.Add(issue);
                db.SaveChanges();
                ViewBag.Message = "Issue Added SuccessFully";

                ModelState.Clear();

                return View();
            }

            return View(issueModel);
        }


        [HttpGet]
        public ActionResult Remaining()
        {
            List<RemainingModel> list = new List<RemainingModel>();

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "All", Value = "All" });

            items.Add(new SelectListItem { Text = "Remaining", Value = "Remaining" });

            items.Add(new SelectListItem { Text = "RemainingDone", Value = "RemainingDone" });

            ViewBag.RemainingType = items;

            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.PfCode = new SelectList(db.Franchisees.Where(d=>d.PF_Code == pfcode), "PF_Code", "PF_Code");
            //ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            //return View(st);
            return View(list);
        }


        [HttpPost]
        public ActionResult Remaining(string PfCode, string RemainingType)
        {


            //var st = db.Stationaries.Where(m => m.Pf_code == PfCode || PfCode == "").ToList();

            List<string> str = new List<string>();

            PfCode= Request.Cookies["Cookies"]["AdminValue"].ToString();

            ViewBag.PfCode = new SelectList(db.Franchisees.Where(d => d.PF_Code == PfCode), "PF_Code", "PF_Code");

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "All", Value = "All" });

            items.Add(new SelectListItem { Text = "Remaining", Value = "Remaining" });

            items.Add(new SelectListItem { Text = "RemainingDone", Value = "RemainingDone" });

            ViewBag.RemainingType = items;

            //if (PfCode == "")
            //{
            //    var obj = db.getRemainingAll().Select(x => new RemainingModel
            //    {


            //        startno = x.startno,
            //        endno = x.endno,
            //        Expiry_Date = x.Expiry_Date,
            //        temprecdate = x.temprecdate,
            //        totalCount = x.totalCOUNTER ?? 0

            //    }).ToList();



            //    ViewBag.type = RemainingType;

            //    return View(obj);
            //}
            //else
            //{
                var obj = db.getRemaining(PfCode).Select(x => new RemainingModel
                {


                    startno = x.startno,
                    endno = x.endno,
                    Expiry_Date = x.Expiry_Date,
                    temprecdate = x.temprecdate,
                    totalCount = x.totalCOUNTER ?? 0

                }).ToList();


                ViewBag.type = RemainingType;
                return View(obj);


            //}

            //return View();



        }


        public JsonResult RemainingConsignments(string startno, string endno)
        {





            List<string> Consignments = new List<string>();




            char stch = startno[0];
            char Endch = endno[0];

            long startConsignment = Convert.ToInt64(startno.Substring(1));
            long EndConsignment = Convert.ToInt64(endno.Substring(1));



            for (long i = startConsignment; i <= EndConsignment; i++)
            {
                string updateconsignment = stch + i.ToString();


                Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();



                if (transaction == null || transaction.Customer_Id == null || transaction.Customer_Id.Length == 0)
                {
                    Consignments.Add(updateconsignment);
                }








            }

            return Json(Consignments, JsonRequestBehavior.AllowGet);

        }


        public ActionResult IsseueRemaining()
        {
            var st = db.Issues.ToList();

            List<string> str = new List<string>();


            foreach (var j in st)
            {

                int counter = 0;

                char stch = j.startno[0];
                char Endch = j.endno[0];

                long startConsignment = Convert.ToInt64(j.startno.Substring(1));
                long EndConsignment = Convert.ToInt64(j.endno.Substring(1));



                for (long i = startConsignment; i <= EndConsignment; i++)
                {
                    string updateconsignment = stch + i.ToString();


                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();


                    if (transaction != null && transaction.Customer_Id != null && transaction.Customer_Id.Length > 1)
                    {
                        counter++;
                    }


                }


                str.Add(counter.ToString());
                counter = 0;




            }

            ViewBag.str = str.ToArray();

            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View(st);

        }

        public ActionResult Employeeautocomplete()
        {


            var entity = db.Issues.
Select(e => new
{
    e.EmployeeName
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditStationary(long id)
        {
            Stationary stationary = db.Stationaries.Find(id);

            if (stationary == null)
            {
                return HttpNotFound();
            }
            ViewBag.Pf_code = new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
            return View(stationary);


        }


        [HttpPost]
        public ActionResult EditStationary(StationaryModel stationary)
        {

            string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

            if (ModelState.IsValid)
            {


                var dataFid = (from d in db.Franchisees
                               where d.PF_Code == Request.Cookies["Cookies"]["AdminValue"].ToString()
                               select d.Firm_Id).FirstOrDefault();

                Stationary St = new Stationary();

                St.Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString();
                St.startno = stationary.startno;
                St.endno = stationary.endno;
                St.noofbooks = stationary.noofbooks;
                St.noofleafs = stationary.noofleafs;
                St.Invoiceno = stationary.Invoiceno;
                St.Status = 0;
                string invdate = DateTime.ParseExact(stationary.temprecdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                St.Expiry_Date = Convert.ToDateTime(invdate);

                St.tempExpiry_Date = stationary.tempExpiry_Date;
                St.temprecdate = stationary.temprecdate;
                St.recieptdate = stationary.recieptdate;
                St.fid = Convert.ToInt32(dataFid);             
                db.SaveChanges();

                // db.Entry(stationary).State = System.Data.Entity.EntityState.Modified;

                ViewBag.Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString(); //new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
                ViewBag.Message = "Stationary Updated SuccessFully";
                ModelState.Clear();
                return View();

            }

            ViewBag.Pf_code = Request.Cookies["Cookies"]["AdminValue"].ToString(); // new SelectList(db.Franchisees, "PF_Code", "PF_Code", stationary.Pf_code);
            return View(stationary);


        }






    }
}
