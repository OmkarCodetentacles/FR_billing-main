using DtDc_Billing.Entity_FR;
using DtDc_Billing.Metadata_Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Models;
using System.Data.Entity.Validation;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DtDc_Billing.Controllers
{
    [SessionAdminold]
    [OutputCache(CacheProfile = "Cachefast")]
    public class BookingController : Controller
    {
        private db_a71c08_elitetokenEntities db = new db_a71c08_elitetokenEntities();
        // GET: Booking
        public ActionResult ConsignMent()
        {
            return View();
        }

        public JsonResult Consignmentdetails(string Cosignmentno)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            db.Configuration.ProxyCreationEnabled = false;

           
            var suggestions = db.Sp_GetSingleConsignment(Cosignmentno, strpfcode).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEditConsignment(TransactionMetadata transaction)
        {

            

            if (transaction.topay != "yes")
            {
                transaction.Topaycharges = 0;
                transaction.consignee = "0";
                transaction.TopayAmount = 0;
            }
            if (transaction.cod != "yes")
            {
                transaction.codAmount = 0;
                transaction.codcharges = 0;
                transaction.consigner = "0";
                transaction.codtotalamount = 0;
            }


            ViewBag.Customerid = transaction.Customer_Id;

            if (ModelState.IsValid)
            {
                Transaction tr = db.Transactions.Where(m => m.Consignment_no == transaction.Consignment_no).FirstOrDefault();


                string[] formats = { "dd-MM-yyyy" };

                string bdate = DateTime.ParseExact(transaction.tembookingdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                transaction.booking_date = Convert.ToDateTime(bdate);

                if (tr != null)
                {



                    db.Entry(tr).State = EntityState.Detached;

                    //////////////////////////
                    Transaction tran = new Transaction();

                    tran.Customer_Id = transaction.Customer_Id;
                    tran.booking_date = transaction.booking_date;
                    tran.Consignment_no = transaction.Consignment_no.Trim();
                    tran.Pincode = transaction.Pincode;
                    tran.Mode = transaction.Mode;
                    tran.Weight_t = transaction.Weight_t;
                    tran.Amount = transaction.Amount;
                    tran.Company_id = transaction.Company_id;

                    tran.Quanntity = transaction.Quanntity;
                    tran.Type_t = transaction.Type_t;
                    tran.Insurance = transaction.Insurance;
                    tran.Claimamount = transaction.Claimamount;
                    tran.Percentage = transaction.Percentage;

                    tran.Claimamount = transaction.Claimamount;
                    tran.remark = transaction.remark;
                    tran.topay = transaction.topay;
                    tran.codAmount = transaction.codAmount;
                    tran.consignee = transaction.consigner;
                    tran.cod = transaction.cod;
                    tran.TopayAmount = transaction.TopayAmount;
                    tran.Topaycharges = transaction.Topaycharges;
                    tran.Actual_weight = transaction.Actual_weight;
                    tran.codcharges = transaction.codcharges;
                    tran.codAmount = transaction.codAmount;
                    tran.dtdcamount = transaction.dtdcamount;
                    tran.chargable_weight = transaction.chargable_weight;
                    tran.status_t = tr.status_t;
                    tran.rateperkg = transaction.rateperkg;
                    tran.docketcharege = transaction.docketcharege;
                    tran.fovcharge = transaction.fovcharge;
                    tran.loadingcharge = transaction.loadingcharge;
                    tran.odocharge = transaction.odocharge;
                    tran.Risksurcharge = transaction.Risksurcharge;
                    tran.Invoice_No = transaction.Invoice_No;
                    tran.BillAmount = transaction.BillAmount;
                    tran.tembookingdate = transaction.tembookingdate;
                    tran.codtotalamount = transaction.codtotalamount;
                    tran.consigner = transaction.consigner;
                    tran.compaddress = transaction.compaddress;
                    tran.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                    tran.AdminEmp = 000;

                    /////////////////////////////

                    tran.T_id = tr.T_id;

                    db.Entry(tran).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Consignment Updated SuccessFully";
                }
                else
                {

                    Transaction tran1 = new Transaction();

                    tran1.Customer_Id = transaction.Customer_Id;
                    tran1.booking_date = transaction.booking_date;
                    tran1.Consignment_no = transaction.Consignment_no.Trim();
                    tran1.Pincode = transaction.Pincode;
                    tran1.Mode = transaction.Mode;
                    tran1.Weight_t = transaction.Weight_t;
                    tran1.Amount = transaction.Amount;
                    tran1.Company_id = transaction.Company_id;
                    tran1.Quanntity = transaction.Quanntity;
                    tran1.Type_t = transaction.Type_t;
                    tran1.Insurance = transaction.Insurance;
                    tran1.Claimamount = transaction.Claimamount;
                    tran1.Percentage = transaction.Percentage;

                    tran1.Claimamount = transaction.Claimamount;
                    tran1.remark = transaction.remark;
                    tran1.topay = transaction.topay;
                    tran1.codAmount = transaction.codAmount;
                    tran1.consignee = transaction.consigner;
                    tran1.cod = transaction.cod;
                    tran1.TopayAmount = transaction.TopayAmount;
                    tran1.Topaycharges = transaction.Topaycharges;
                    tran1.Actual_weight = transaction.Actual_weight;
                    tran1.codcharges = transaction.codcharges;
                    tran1.codAmount = transaction.codAmount;
                    tran1.dtdcamount = transaction.dtdcamount;
                    tran1.chargable_weight = transaction.chargable_weight;
                    
                    tran1.rateperkg = transaction.rateperkg;
                    tran1.docketcharege = transaction.docketcharege;
                    tran1.fovcharge = transaction.fovcharge;
                    tran1.loadingcharge = transaction.loadingcharge;
                    tran1.odocharge = transaction.odocharge;
                    tran1.Risksurcharge = transaction.Risksurcharge;
                    tran1.Invoice_No = transaction.Invoice_No;
                    tran1.BillAmount = transaction.BillAmount;
                    tran1.tembookingdate = transaction.tembookingdate;
                    tran1.codtotalamount = transaction.codtotalamount;
                    tran1.consigner = transaction.consigner;
                    tran1.compaddress = transaction.compaddress;

                    tran1.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                    tran1.AdminEmp = 000;
                    db.Transactions.Add(tran1);
                 


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

                    Jobclass jobclass = new Jobclass();
                    jobclass.deletefromExpiry(tran1.Consignment_no);



                    ViewBag.Message = "Consignment Booked SuccessFully";
                }


                ModelState.Clear();

                ViewBag.success = true;

                char ch = transaction.Consignment_no[0];

                long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(1));

                consignnumber = consignnumber + 1;

                var lenght = transaction.Consignment_no.Substring(1).Length;

                ViewBag.nextconsignment = ch + "" + (consignnumber.ToString().PadLeft(lenght,'0'));


                return PartialView("ConsignmentPartial");
            }

            return PartialView("ConsignmentPartial", transaction);
        }

        public ActionResult CustomerIdAutocomplete()
        {

            string strpfcode= Request.Cookies["Cookies"]["AdminValue"].ToString();
            var entity = db.Companies.Select(e => new
{
    e.Company_Id,
    e.Pf_code
}).Distinct().Where(e=>e.Pf_code== strpfcode).OrderBy(e=>e.Company_Id).ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerIdReceipt(string id)
        {


            var entity = db.Companies.Where(m => m.Pf_code == id).
Select(e => new
{
    e.Company_Id
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PincodeautocompleteSender()
        {


            var entity = db.Destinations.
Select(e => new
{
    e.Pincode
}).ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerDetails(string CustomerId)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var suggestions = (from s in db.Companies
                               where s.Company_Id == CustomerId
                               select s).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditConsignment()
        {
            ViewBag.transaction = new TransactionMetadata();

            ViewBag.talist = new List<TransactionView>();

            return View();
        }

         public ActionResult weightdiffModify()
        {
            ViewBag.transaction = new TransactionMetadata();

            ViewBag.talist = new List<TransactionView>();

            return View();
        }


        [HttpPost]
        public ActionResult EditConsignment(TransactionMetadata transaction)
        {


            if (transaction.topay != "yes")
            {
                transaction.Topaycharges = 0;
                transaction.consignee = "0";
                transaction.TopayAmount = 0;                
            }
            if (transaction.cod != "yes")
            {
                transaction.codAmount = 0;
                transaction.codcharges = 0;
                transaction.consigner = "0";
                transaction.codtotalamount = 0;
            }


            if (ModelState.IsValid)
            {
                Transaction tr = db.Transactions.Where(m => m.Consignment_no == transaction.Consignment_no).FirstOrDefault();


                string[] formats = { "dd-MM-yyyy" };

                string bdate = DateTime.ParseExact(transaction.tembookingdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                transaction.booking_date = Convert.ToDateTime(bdate);


                if (tr != null)
                {

                    ; db.Entry(tr).State = EntityState.Detached;


                    transaction.T_id = tr.T_id;


                    Transaction tran = new Transaction();
                    tran.T_id = tr.T_id;
                    tran.Customer_Id = transaction.Customer_Id;
                    tran.booking_date = transaction.booking_date;
                    tran.Consignment_no = transaction.Consignment_no.Trim();
                    tran.Pincode = transaction.Pincode;
                    tran.Mode = transaction.Mode;
                    tran.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                    tran.AdminEmp = 000;
                    tran.Weight_t = transaction.Weight_t;
                    tran.Amount = transaction.Amount;
                    tran.Company_id = transaction.Company_id;
                    
                    tran.Quanntity = transaction.Quanntity;
                    tran.Type_t = transaction.Type_t;
                    tran.Insurance = transaction.Insurance;
                    tran.Claimamount = transaction.Claimamount;
                    tran.Percentage = transaction.Percentage;

                    tran.Claimamount = transaction.Claimamount;
                    tran.remark = transaction.remark;
                    tran.topay = transaction.topay;
                    tran.codAmount = transaction.codAmount;
                    tran.consignee = transaction.consigner;
                    tran.cod = transaction.cod;
                    tran.TopayAmount = transaction.TopayAmount;
                    tran.Topaycharges = transaction.Topaycharges;
                    tran.Actual_weight = transaction.Actual_weight;
                    tran.codcharges = transaction.codcharges;
                    tran.codAmount = transaction.codAmount;
                    tran.dtdcamount = transaction.dtdcamount;
                    tran.chargable_weight = transaction.chargable_weight;
                    tran.status_t = tr.status_t;
                    tran.rateperkg = transaction.rateperkg;
                    tran.docketcharege = transaction.docketcharege;
                    tran.fovcharge = transaction.fovcharge;
                    tran.loadingcharge = transaction.loadingcharge;
                    tran.odocharge = transaction.odocharge;
                    tran.Risksurcharge = transaction.Risksurcharge;
                    tran.Invoice_No = transaction.Invoice_No;
                    tran.BillAmount = transaction.BillAmount;
                    tran.tembookingdate = transaction.tembookingdate;
                    tran.compaddress = transaction.compaddress;
                    tran.codtotalamount = transaction.codtotalamount;
                    tran.consigner = transaction.consigner;

                    db.Entry(tran).State = EntityState.Modified;

                    db.SaveChanges();
                    ViewBag.Message = "Consignment Updated SuccessFully";
                }

                ModelState.Clear();

                ViewBag.success = true;

                char ch = transaction.Consignment_no[0];

                long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(1));

                consignnumber = consignnumber + 1;



                string nextconsignment = ch + "" + consignnumber;


                var con = db.Transactions.Where(m => m.Consignment_no == nextconsignment).FirstOrDefault();

                if (con != null)
                {

                    ViewBag.nextconsignment = ch + "" + consignnumber;

                }



                return PartialView("EditConsignmentPartial");
            }

            return PartialView("EditConsignmentPartial", transaction);
        }





        [HttpPost]
        public ActionResult Trtableseacrh(string Fromdatetime, string ToDatetime, string Custid)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            DateTime? fromdate = null;
            DateTime? todate = null;
            

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};
            

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

            if (Custid == "")
            {


                var obj = db.getCheckBookingListWithoutCompany(fromdate, todate, strpf).Select(x => new TransactionView
                {

                    Consignment_no = x.Consignment_no,
                    chargable_weight = x.chargable_weight,
                    Quanntity = x.Quanntity,
                    Name = x.Name,
                    Pincode = x.Pincode,
                    compaddress = x.compaddress,
                    Type_t = x.Type_t,
                    Mode = x.Mode,
                    Amount = x.Amount,
                    booking_date = x.booking_date,
                    Insurance = x.Insurance,
                    BillAmount = x.BillAmount,
                    Percentage = x.Percentage,
                    Risksurcharge = x.Risksurcharge,
                    loadingcharge = x.loadingcharge

                }).OrderBy(d => d.Consignment_no).ToList();



                ViewBag.totalamt = obj.Sum(b => (b.Amount + (b.loadingcharge ?? 0) + (b.Risksurcharge ?? 0)));


                return PartialView("TrSearchTable", obj);
            }
            else
            {
                var obj = db.getCheckBookingList(fromdate, todate, Custid, strpf).Select(x => new TransactionView
                {

                    Consignment_no = x.Consignment_no,
                    chargable_weight = x.chargable_weight,
                    Quanntity = x.Quanntity,
                    Name = x.Name,
                    Pincode = x.Pincode,
                    compaddress = x.compaddress,
                    Type_t = x.Type_t,
                    Mode = x.Mode,
                    Amount = x.Amount,
                    booking_date = x.booking_date,
                    Insurance = x.Insurance,
                    BillAmount = x.BillAmount,
                    Percentage = x.Percentage,
                    Risksurcharge = x.Risksurcharge,
                    loadingcharge = x.loadingcharge

                }).OrderBy(d => d.Consignment_no).ToList();



                ViewBag.totalamt = obj.Sum(b => (b.Amount + (b.loadingcharge ?? 0) + (b.Risksurcharge ?? 0)));


                return PartialView("TrSearchTable", obj);

            }
        }


        public ActionResult MultipleBooking()
        {

            return View();
        }

        [HttpPost]
        public ActionResult MultipleBooking(string StartingCons, string EndingCons, string Companyid)
        {
            char stch = StartingCons[0];
            char Endch = EndingCons[0];

            long startConsignment = Convert.ToInt64(StartingCons.Substring(1));
            long EndConsignment = Convert.ToInt64(EndingCons.Substring(1));

            int countconsigmnets = 0;
            var pfcode = db.Companies.Where(m => m.Company_Id == Companyid).Select(m => m.Pf_code).FirstOrDefault();

            if (stch == Endch)
            {

                for (long i = startConsignment; i <= EndConsignment; i++)
                {
                    string updateconsignment = stch + i.ToString();


                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment).FirstOrDefault();

                    if (transaction != null)
                    {

                        CalculateAmount ca = new CalculateAmount();

                        double? amt = ca.CalulateAmt(transaction.Consignment_no, Companyid, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                        transaction.Amount = amt;
                        transaction.Customer_Id = Companyid;
                        transaction.AdminEmp = 000;
                        transaction.Pf_Code = pfcode;
                        db.Entry(transaction).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    countconsigmnets++;

                    if(countconsigmnets >= 100) 
                    {
                        break;
                    }
                    
                }
            }



            ViewBag.Message = "Booking Completed Successfully";

            return View();
        }


        public ActionResult Checkbookinglist()
        {
            //List<TransactionView> list = new List<TransactionView>();
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString(); 
            
            var obj = db.getCheckBookingListAll(strpf).Select(x => new TransactionView
            {

              Consignment_no = x.Consignment_no,
              chargable_weight  = x.chargable_weight,
              Quanntity  = x.Quanntity,
              Name  = x.Name,
              Pincode  = x.Pincode,
              compaddress  = x.compaddress,
              Type_t = x.Type_t,
              Mode  = x.Mode,
              Amount  = x.Amount,
              booking_date  = x.booking_date,
              Insurance  = x.Insurance,
              BillAmount= x.BillAmount,
              Percentage  = x.Percentage,
              Risksurcharge  = x.Risksurcharge,
              loadingcharge  = x.loadingcharge

            }).ToList();

            ViewBag.totalamt = obj.Sum(b => b.Amount);

            return View(obj);
        }

        [HttpPost]
        public ActionResult Checkbookinglist(string Fromdatetime, string ToDatetime, string Custid,string Submit)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }

            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }

            if (Custid == "")
            {
                var obj = db.getCheckBookingListWithoutCompany(fromdate, todate, strpf).Select(x => new TransactionView
                {

                    Consignment_no = x.Consignment_no,
                    chargable_weight = x.chargable_weight,
                    Quanntity = x.Quanntity,
                    Name = x.Name,
                    Pincode = x.Pincode,
                    compaddress = x.compaddress,
                    Type_t = x.Type_t,
                    Mode = x.Mode,
                    Amount = x.Amount,
                    booking_date = x.booking_date,
                    Insurance = x.Insurance,
                    BillAmount = x.BillAmount,
                    Percentage = x.Percentage,
                    Risksurcharge = x.Risksurcharge,
                    loadingcharge = x.loadingcharge

                }).OrderBy(d => d.Consignment_no).ToList();

                ViewBag.totalamt = obj.Sum(b => b.Amount);

                if (Submit == "Export to Excel")
                {
                    var import = db.TransactionViews.ToList().Where(m=>m.Pf_Code==strpf).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Consignment_no, Weight = x.chargable_weight, x.Quanntity, x.Name, x.Pincode, x.compaddress, x.Type_t, x.Mode, x.Amount, BookingDate = x.tembookingdate, x.Insurance, x.Claimamount, x.Percentage, Risksurcharge = x.calinsuranceamount, Total = (x.Amount + x.calinsuranceamount) }).ToList();

                    ExportToExcelAll.ExportToExcelAdmin(import);
                }


                return View(obj);

            }
            else
            {
                var obj = db.getCheckBookingList(fromdate, todate, Custid, strpf).Select(x => new TransactionView
                {

                    Consignment_no = x.Consignment_no,
                    chargable_weight = x.chargable_weight,
                    Quanntity = x.Quanntity,
                    Name = x.Name,
                    Pincode = x.Pincode,
                    compaddress = x.compaddress,
                    Type_t = x.Type_t,
                    Mode = x.Mode,
                    Amount = x.Amount,
                    booking_date = x.booking_date,
                    Insurance = x.Insurance,
                    BillAmount = x.BillAmount,
                    Percentage = x.Percentage,
                    Risksurcharge = x.Risksurcharge,
                    loadingcharge = x.loadingcharge

                }).OrderBy(d => d.Consignment_no).ToList();

                ViewBag.totalamt = obj.Sum(b => b.Amount);

                if (Submit == "Export to Excel")
                {
                    var import = db.TransactionViews.Where(m =>
                    (m.Customer_Id == Custid)
                        ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Consignment_no, Weight = x.chargable_weight, x.Quanntity, x.Name, x.Pincode, x.compaddress, x.Type_t, x.Mode, x.Amount, BookingDate = x.tembookingdate, x.Insurance, x.Claimamount, x.Percentage, Risksurcharge = x.calinsuranceamount, Total = (x.Amount + x.calinsuranceamount) }).ToList();


                    ExportToExcelAll.ExportToExcelAdmin(import);
                }

                return View(obj);
            }

           

            


          
        }


        public ActionResult Nobookinglist()
        {
            List<Transaction> list = new List<Transaction>();
            ViewBag.PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View(list);
        }

        [HttpPost]
        public ActionResult Nobookinglist(string Fromdatetime, string ToDatetime, string PfCode,string Submit)
        {
            PfCode=Request.Cookies["Cookies"]["AdminValue"].ToString();

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            ViewBag.PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);

            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }


         

            List<Transaction> transactions =
                db.Transactions.Where(m =>
               (m.Pf_Code == PfCode  || PfCode == "") 
                    ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

            
            if (Submit == "Export to Excel")
            {
                var import = db.Transactions.Where(m =>
                (m.Pf_Code == PfCode || PfCode == "")
                    ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Pf_Code, x.Consignment_no, Weight=x.Actual_weight, x.Pincode, x.Amount,BookingDate= x.tembookingdate }).ToList();
                ExportToExcelAll.ExportToExcelAdmin(import);
            }


            return View(transactions);
        }

        public ActionResult weightdiff_Partial()
        {
            

            List<TransactionView> transactions = new List<TransactionView>();





            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            return PartialView("weightdiff_Partial");
        }

        [HttpPost]
        public ActionResult weightdiff_Partial(string Fromdatetime, string ToDatetime)
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }





            List<TransactionView> transactions =
                db.TransactionViews.Where(m =>
               (m.chargable_weight < m.diff_weight)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                           .ToList();





            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            //return View(transactions);
            return PartialView("weightdiff_Partial", transactions);
        }

        public string Checkcompany(string Customerid)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var suggestions = (from s in db.Companies
                               where s.Company_Id == Customerid
                               select s).FirstOrDefault();

            if (suggestions == null)
            {
                return "0";
            }
            else
            {
                return "1";
            }

        }



        public ActionResult MultipleBookingReceipt()
        {
            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");

            ViewBag.Employees = new SelectList(db.Users.Take(0), "User_Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult MultipleBookingReceipt(string PfCode, long Employees, string ToDatetime, string Fromdatetime, string Customer_Id)
        {



            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
                ViewBag.fromdate = Fromdatetime;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
                ViewBag.fromdate = Fromdatetime;
            }


            ViewBag.Customer_Id = Customer_Id;




            ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);

            ViewBag.Employees = new SelectList(db.Users, "User_Id", "Name", Employees);

            ViewBag.selectedemp = Employees;

            List<Receipt_details> rc = (from m in db.Receipt_details
                                        where m.Pf_Code == PfCode && m.User_Id == Employees && m.Datetime_Cons != null
                                        select m).ToList()
                             .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate.Value.Date) <= 0)
                                .ToList();
            int count = 0;
            var pfcode = db.Companies.Where(m => m.Company_Id == Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
            foreach (var i in rc)
            {
                Transaction tr = new Transaction();

                tr = db.Transactions.Where(m => m.Consignment_no == i.Consignment_No).FirstOrDefault();

                if (tr != null)
                {
                    tr.Customer_Id = Customer_Id;
                    tr.Amount = i.Charges_Total;
                    tr.AdminEmp = 000;
                    tr.Pf_Code = pfcode;
                    db.Entry(tr).State = EntityState.Modified;
                    db.SaveChanges();
                    count++;
                }


            }

            ViewBag.success = count + "Records Updated SuccessFully";


            return View();
        }

        public ActionResult InternationalCity()
        {

            var entity = db.Destinations.Where(m => m.Pincode.StartsWith("111")).
            Select(e => new
            {
                e.Name
            }).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillPincode(string Name)
        {
            var suggestions = from s in db.Destinations
                              where s.Name == Name
                              select s;

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }


        public string DeleteConsignment(string Consignment_No)
        {
            Transaction cash = db.Transactions.Where(m => m.Consignment_no == Consignment_No).FirstOrDefault();

            cash.AdminEmp = 000;

            cash.Customer_Id = null;

            db.Entry(cash).State = EntityState.Modified;

            db.SaveChanges();

            return "Consignment Deleted SuccessFully";


        }


        public ActionResult UpdateRate()
        {
            List<TransactionView> list = new List<TransactionView>();

            return View(list);
        }

        [HttpPost]
        public ActionResult UpdateRate(string Fromdatetime, string ToDatetime, string Custid, string submit)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }

            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }



            if (Custid == "")
            {
                var obj = db.getCheckBookingListWithoutCompany(fromdate, todate, strpf).Select(x => new TransactionView
                {

                    Consignment_no = x.Consignment_no,
                    chargable_weight = x.chargable_weight,
                    Quanntity = x.Quanntity,
                    Name = x.Name,
                    Pincode = x.Pincode,
                    compaddress = x.compaddress,
                    Type_t = x.Type_t,
                    Mode = x.Mode,
                    Amount = x.Amount,
                    booking_date = x.booking_date,
                    Insurance = x.Insurance,
                    BillAmount = x.BillAmount,
                    Percentage = x.Percentage,
                    Risksurcharge = x.Risksurcharge,
                    loadingcharge = x.loadingcharge

                }).OrderBy(d => d.Consignment_no).ToList();


                if (submit == "UpdateRate")
                {

                    foreach (var i in obj)
                    {

                        Transaction transaction = db.Transactions.Where(m => m.Consignment_no == i.Consignment_no && m.Pf_Code==strpf).FirstOrDefault();

                        if (transaction != null)
                        {

                            CalculateAmount ca = new CalculateAmount();
                            double? amt = 0;
                            if (transaction.Pincode != null && transaction.Pincode != "NULL")
                                amt = ca.CalulateAmt(transaction.Consignment_no, transaction.Customer_Id, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                            transaction.Amount = amt;
                            transaction.AdminEmp = 000;

                            db.Entry(transaction).State = EntityState.Modified;
                            db.SaveChanges();

                            ViewBag.successmsg = "Updated Successfully";

                        }


                    }

                }

                ViewBag.totalamt = obj.Sum(b => b.Amount);

                return View(obj);

            }
            else
            {
                var obj = db.getCheckBookingList(fromdate, todate, Custid, strpf).Select(x => new TransactionView
                {

                    Consignment_no = x.Consignment_no,
                    chargable_weight = x.chargable_weight,
                    Quanntity = x.Quanntity,
                    Name = x.Name,
                    Pincode = x.Pincode,
                    compaddress = x.compaddress,
                    Type_t = x.Type_t,
                    Mode = x.Mode,
                    Amount = x.Amount,
                    booking_date = x.booking_date,
                    Insurance = x.Insurance,
                    BillAmount = x.BillAmount,
                    Percentage = x.Percentage,
                    Risksurcharge = x.Risksurcharge,
                    loadingcharge = x.loadingcharge

                }).OrderBy(d => d.Consignment_no).ToList();

                if (submit == "UpdateRate")
                {

                    foreach (var i in obj)
                    {

                        Transaction transaction = db.Transactions.Where(m => m.Consignment_no == i.Consignment_no && m.Pf_Code == strpf).FirstOrDefault();

                        if (transaction != null)
                        {

                            CalculateAmount ca = new CalculateAmount();
                            double? amt = 0;
                            if (transaction.Pincode != null && transaction.Pincode != "NULL ")
                                amt = ca.CalulateAmt(transaction.Consignment_no, transaction.Customer_Id, transaction.Pincode, transaction.Mode, Convert.ToDouble(transaction.chargable_weight), transaction.Type_t);

                            transaction.Amount = amt;
                            transaction.AdminEmp = 000;

                            db.Entry(transaction).State = EntityState.Modified;
                            db.SaveChanges();

                            ViewBag.successmsg = "Updated Successfully";

                        }


                    }

                }

                ViewBag.totalamt = obj.Sum(b => b.Amount);

                return View(obj);
            }


         

           
        }


        public ActionResult BulkUpdation()
        {
            List<TransactionView> list = new List<TransactionView>();

            return View(list);
        }



        [HttpPost]
        public ActionResult BulkUpdation(string Fromdatetime, string ToDatetime, string Custid, string ListType, string submit)
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            if (Fromdatetime != "")
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "")
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }

            if (Custid != "")
            {
                ViewBag.Custid = Custid;
            }


            List<TransactionView> transactions = new List<TransactionView>();
            if (ListType == "UpdateConsignment")
            {
                transactions =
                    db.TransactionViews.Where(m =>
                   (m.Customer_Id == Custid)
                        ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Take(500)
                               .ToList();
            }
            else
            {
                transactions =
                   db.TransactionViews.Where(m =>
                  (m.Customer_Id == "" || m.Customer_Id == null)
                       ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Take(500)
                              .ToList();
            }

            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            return View(transactions);
        }


        public int Bulkupdatesave(string Consignment, string custid, string mode, double charweight, string type, double? amount)
        {
            Transaction tran = db.Transactions.Where(m => m.Consignment_no == Consignment).FirstOrDefault();

            if (tran != null)
            {


                tran.Customer_Id = custid;

                tran.Mode = mode;
                tran.chargable_weight = charweight;
                tran.Amount = amount;
                tran.Type_t = type;

                tran.Pf_Code = db.Companies.Where(m => m.Company_Id == custid).Select(m => m.Pf_code).FirstOrDefault();
                tran.AdminEmp = 000;
                if (type == "D")
                {
                    tran.Insurance = "no";
                    tran.BillAmount = 0;
                    tran.Risksurcharge = 0;
                    tran.Invoice_No = 0;
                }


                /////////////////////////////

                try
                {
                    db.Entry(tran).State = EntityState.Modified;
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

                return 1;
            }
            return 0;

        }


        public ActionResult ViewConsignment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ViewPartial(string consignmetno)
        {
            var obj = db.TransactionViews.Where(m => m.Consignment_no == consignmetno).FirstOrDefault();
            return PartialView("ViewPartial", obj);
        }

        ///////////////////Added on 16-05-22 //////////////////
        
       
        //[SessionTimeout]
        public ActionResult CompanyList()
        {
            string pfcode = "";

            //if (Session["UserType"] != null)
            //{
                pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            //}
            //else
            //{
            //     pfcode = Session["PfID"].ToString();
            //}


            return View(db.Companies.Where(m => m.Pf_code == pfcode).ToList());
        }



        ////////////////////////////////////

    }
}