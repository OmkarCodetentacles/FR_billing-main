using DtDc_Billing.Entity_FR;
using DtDc_Billing.Metadata_Classes;
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
    [SessionUserModule]
    public class DeliveryReportController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: DeliveryReport
        public ActionResult Dreport()
        {

            ViewBag.delivery = new deliverydata();

            ViewBag.talist = new List<TransactionView>();

            return View();
        }


        [HttpPost]
        public ActionResult EditDelivery(deliverydata transaction)
        {



            if (ModelState.IsValid)
            {
                deliverydata tr = db.deliverydatas.Where(m => m.consinmentno == transaction.consinmentno).FirstOrDefault();


                //string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

                //string bdate = DateTime.ParseExact(transaction.tempdelivereddate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                //transaction.delivered_date = Convert.ToDateTime(bdate);


                if (tr != null)
                {

                     db.Entry(tr).State = EntityState.Detached;


                    transaction.d_id = tr.d_id;


                    deliverydata tran = new deliverydata();
                    tran.d_id = tr.d_id;
                    tran.remarks = transaction.remarks;
                    tran.receivedby = transaction.receivedby;
                    //tran.delivered_date = transaction.delivered_date;
                    tran.tempdeliveredtime = transaction.tempdeliveredtime;
                    tran.tempdelivereddate= transaction.tempdelivereddate;
                    tran.consinmentno = transaction.consinmentno;


                    db.Entry(tran).State = EntityState.Modified;

                    db.SaveChanges();
                    ViewBag.Message = "Delivery data Updated SuccessFully";
                }
                else
                {

                    db.Entry(tr).State = EntityState.Detached;


                    


                    deliverydata tran = new deliverydata();

                    tran.remarks = transaction.remarks;
                    tran.receivedby = transaction.receivedby;
                    //tran.delivered_date = transaction.delivered_date;
                    tran.tempdeliveredtime = transaction.tempdeliveredtime;
                    tran.tempdelivereddate = transaction.tempdelivereddate;
                    tran.consinmentno = transaction.consinmentno;


                    db.deliverydatas.Add(tran);

                    db.SaveChanges();
                    ViewBag.Message = "Delivery data Added SuccessFully";
                }

                ModelState.Clear();

                ViewBag.success = true;

               



                return PartialView("EditdeliveryPartial");
            }

            return PartialView("EditdeliveryPartial", transaction);
        }





        [HttpPost]
        public ActionResult DeliverySearch(string Fromdatetime, string ToDatetime, string Custid, string status)
        {

            DateTime? fromdate = null;
            DateTime? todate = null;

            ViewBag.select = status;

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};



            if (status == "Undelivered")
            {
                status = null;
            }



            if (Fromdatetime != "")
            {

                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = null;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = null;
            }
            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }

            List<TransactionView> transactions = new List<TransactionView>();

            if (status == null)
            {
                transactions =
                   db.TransactionViews.Where(m =>

                  (m.Customer_Id == Custid || Custid == "")

                  && (m.Customer_Id != null) && m.status_t == status
                       ).ToList().Where(m => (fromdate == null || m.booking_date.Value.Date >= fromdate.Value.Date) && (todate == null || m.booking_date.Value.Date <= todate.Value.Date))
                              .ToList();

            }else if(status == "Delivered")
            {
                transactions =
                 db.TransactionViews.Where(m =>

                (m.Customer_Id == Custid || Custid == "")

                && (m.Customer_Id != null) && m.status_t != null
                     ).ToList().Where(m => (fromdate == null || m.booking_date.Value.Date >= fromdate.Value.Date) && (todate == null || m.booking_date.Value.Date <= todate.Value.Date))
                            .ToList();
            }else
            {
                transactions =
                 db.TransactionViews.Where(m =>

                (m.Customer_Id == Custid || Custid == "")

                && (m.Customer_Id != null)
                     ).ToList().Where(m => (fromdate == null || m.booking_date.Value.Date >= fromdate.Value.Date) && (todate == null || m.booking_date.Value.Date <= todate.Value.Date))
                            .ToList();
            }



            ViewBag.totalamt = transactions.Sum(b => b.Amount);


            return PartialView("deliverySearch", transactions);
        }


        public ActionResult CustomerIdAutocomplete()
        {


            var entity = db.Companies.
Select(e => new
{
    e.Company_Id
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Deliverydetails(string Cosignmentno)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var suggestions = (from s in db.deliverydatas
                               where s.consinmentno == Cosignmentno
                               select s).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

    }
}