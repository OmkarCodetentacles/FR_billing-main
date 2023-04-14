using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Metadata_Classes;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using System.Web.Script.Serialization;
using System.IO;
using System.Net.Mail;
using ClosedXML.Excel;

namespace DtDc_Billing.Controllers
{
    [SessionTimeout]
    [SessionUserModule]
    public class EmpBookingController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        // GET: Booking

        public ActionResult CompanyList()
        {

            string pfcode = Session["pfCode"].ToString();

            return View(db.Companies.Where(m=>m.Pf_code==pfcode).ToList());
       }

        public ActionResult ConsignMent()
        {
            return View();
        }

        public JsonResult Consignmentdetails(string Cosignmentno)
        {
            db.Configuration.ProxyCreationEnabled = false;

            //string pfcode = Session["PfID"].ToString();

            //string b = pfcode.Substring(2, pfcode.Length - 2);
            //string prcode = "PR" + "" + b;

            var suggestions = (from s in db.Transactions
                               where s.Consignment_no == Cosignmentno 
                               select s).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEditConsignment(TransactionMetadata transaction)
        {


            //if (transaction.topay != "yes")
            //{
            //    transaction.Topaycharges = 0;
            //    transaction.consignee = "0";
            //    transaction.TopayAmount = 0;
            //}
            //if (transaction.cod != "yes")
            //{
            //    transaction.codAmount = 0;
            //    transaction.codcharges = 0;
            //    transaction.consigner = "0";
            //    transaction.codtotalamount = 0;
            //}

            //if (transaction.Insurance != "yes")
            //{
            //    transaction.BillAmount = 0;
            //    transaction.Risksurcharge = 0;
            //    transaction.Invoice_No = 0;
            //}

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

            int EmpId = Convert.ToInt16(Session["EmpId"]);
            string pfcode = Session["pfCode"].ToString();

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

                    tran.AdminEmp = EmpId;
                    tran.Customer_Id = transaction.Customer_Id;
                    tran.booking_date = transaction.booking_date;
                    tran.Consignment_no = transaction.Consignment_no;
                    tran.Pf_Code = pfcode;
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
                    tran.diff_weight = transaction.diff_weight;
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
                    tran.AdminEmp = EmpId;

                    /////////////////////////////

                    tran.T_id = tr.T_id;

                    db.Entry(tran).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Consignment Updated SuccessFully";
                }
                else
                {

                    Transaction tran1 = new Transaction();

                    tran1.AdminEmp = EmpId;
                    tran1.Customer_Id = transaction.Customer_Id;
                    tran1.booking_date = transaction.booking_date;
                    tran1.Consignment_no = transaction.Consignment_no;
                    tran1.Pf_Code = pfcode;
                    tran1.Pincode = transaction.Pincode;
                    tran1.Mode = transaction.Mode;
                    tran1.Weight_t = transaction.Weight_t;
                    tran1.Amount = transaction.Amount;
                    tran1.Company_id = transaction.Company_id;
                    tran1.Pf_Code = pfcode;
                    tran1.Quanntity = transaction.Quanntity;
                    tran1.Type_t = transaction.Type_t;
                    tran1.Insurance = transaction.Insurance;
                    tran1.Claimamount = transaction.Claimamount;
                    tran1.Percentage = transaction.Percentage;
                    tran1.diff_weight = transaction.diff_weight;

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
                    tran1.status_t = transaction.status_t;
                    tran1.rateperkg = transaction.rateperkg;
                    tran1.docketcharege = transaction.docketcharege;
                    tran1.fovcharge = transaction.fovcharge;
                    tran1.loadingcharge = transaction.loadingcharge;
                    tran1.odocharge = transaction.odocharge;
                    tran1.Risksurcharge = transaction.Risksurcharge;
                    tran1.Invoice_No = transaction.Invoice_No;
                    tran1.BillAmount = transaction.BillAmount;
                    tran1.tembookingdate = transaction.tembookingdate;
                    tran1.compaddress = transaction.compaddress;
                    tran1.AdminEmp = EmpId;

                    db.Transactions.Add(tran1);
                    db.SaveChanges();
                    ViewBag.Message = "Consignment Booked SuccessFully";
                }


                ModelState.Clear();

                ViewBag.success = true;

                char ch = transaction.Consignment_no[0];

                long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(1));

                consignnumber = consignnumber + 1;


                ViewBag.nextconsignment = ch + "" + consignnumber;

                ViewBag.compid = transaction.Customer_Id;

                return PartialView("ConsignmentPartial");
            }

            return PartialView("ConsignmentPartial", transaction);
        }

        public ActionResult CustomerIdAutocomplete()
        {
            //string Pfcode = Session["PfID"].ToString();
            string Pfcode = Session["pfCode"].ToString();
            var entity = db.Companies.Where(m=>m.Pf_code == Pfcode).
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


        public string pfcodevalidation(string Consignment_no)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var pf = Session["pfCode"].ToString();
            string b = pf.Substring(2, pf.Length - 2);
            string prcode = "PR" + "" + b;
            var pfval = db.Transactions.Where(m => m.Consignment_no == Consignment_no  && (m.Pf_Code == pf || m.Pf_Code == prcode)).FirstOrDefault();


            //if (pfval == null)
            //{
            //    return "0";
            //}
            //else
            //{
            //    return "1";
            //}

            if (pfval != null)
            {
                return "1";
            }
            else
            {
                return "0";
            }
            ////////////

        }


        public ActionResult EditConsignment()
        {
            ViewBag.transaction = new TransactionMetadata();

            ViewBag.talist = new List<TransactionView>();
            
            return View();
        }


        [HttpPost]
        public ActionResult EditConsignment(TransactionMetadata transaction)
        {

            //if (transaction.Insurance != "yes")
            //{
            //    transaction.BillAmount = 0;
            //    transaction.Risksurcharge = 0;
            //    transaction.Invoice_No = 0;
            //}

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


                string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

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
                    tran.status_t = transaction.status_t;
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

            var PfCode = Session["pfCode"].ToString();

            List<TransactionView> transactions =
                db.TransactionViews.Where(m =>

               (m.Customer_Id == Custid || Custid == "")

               && (m.Customer_Id != null)
                    ).ToList().Where(m => (fromdate == null || m.booking_date.Value.Date >= fromdate.Value.Date) && (todate == null || m.booking_date.Value.Date <= todate.Value.Date) /*&& m.Pf_Code==PfCode*/).OrderBy(m=>m.booking_date)
                           .ToList();



            ViewBag.totalamt = transactions.Sum(b => (b.Amount ?? 0)+ (b.Risksurcharge ?? 0) +(b.loadingcharge ?? 0));


            return PartialView("TrSearchTable", transactions);
        }



        public ActionResult Checkbookinglist()
        {
            List<TransactionView> list = new List<TransactionView>();

            return View(list);
        }

        [HttpPost]
        public ActionResult Checkbookinglist(string Fromdatetime, string ToDatetime, string Custid, string Submit)
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



            List<TransactionView> transactions =
                db.TransactionViews.Where(m =>
               (m.Customer_Id == Custid)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m=>m.booking_date)
                           .ToList();

            ViewBag.totalamt = transactions.Sum(b => b.Amount);

            if (Submit == "Export to Excel")
            {
                var import = db.TransactionViews.Where(m =>
                (m.Customer_Id == Custid)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Consignment_no, Weight = x.chargable_weight, x.Name, x.Pincode,x.compaddress, x.Sender,x.receiver, x.Type_t, x.Mode, x.Amount, BookingDate = x.tembookingdate, x.Insurance, x.Claimamount, x.Percentage, Risksurcharge = x.calinsuranceamount, Total = (x.Amount + x.calinsuranceamount) }).ToList();
                ExportToExcelAll.ExportToExcelAdmin(import);
            }

            return View(transactions);
        }


        public ActionResult Nobookinglist()
        {
            List<Transaction> list = new List<Transaction>();

           // ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code");

            return View(list);
        }

        [HttpPost]
        public ActionResult Nobookinglist(string Fromdatetime, string ToDatetime)
        {

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            //ViewBag.PfCode = new SelectList(db.Franchisees, "PF_Code", "PF_Code", PfCode);
            var PfCode = Session["pfCode"].ToString();

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


            string b = PfCode.Substring(2, PfCode.Length - 2);
            string prcode = "PR" + "" + b;

            List<Transaction> transactions =
                db.Transactions.Where(m =>
               (m.Customer_Id == null || m.Customer_Id == "") && ((m.Pf_Code == PfCode || m.Pf_Code == prcode)|| PfCode == "")
                    ).OrderBy(m => m.booking_date).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).ToList();

            
            return View(transactions);
        }


        public ActionResult MultipleBookingReceipt()
        {
            

            ViewBag.Employees = new SelectList(db.Users.Take(0), "User_Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult MultipleBookingReceipt(long Employees, string ToDatetime, string Fromdatetime, string Customer_Id)
        {

            int EmpId = Convert.ToInt16(Session["EmpId"]);
            string pfcode = Session["pfCode"].ToString();

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;

            string Pfcode = Session["pfCode"].ToString();

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




            

            ViewBag.Employees = new SelectList(db.Users, "User_Id", "Name", Employees);

            ViewBag.selectedemp = Employees;

            List<Receipt_details> rc = (from m in db.Receipt_details
                                        where m.Pf_Code == Pfcode && m.User_Id == Employees && m.Datetime_Cons != null
                                        select m).ToList()
                             .Where(x => DateTime.Compare(x.Datetime_Cons.Value.Date, fromdate.Value.Date) >= 0 && DateTime.Compare(x.Datetime_Cons.Value.Date, todate.Value.Date) <= 0)
                                .ToList();
            int count = 0;

            foreach (var i in rc)
            {
                Transaction tr = new Transaction();

                tr = db.Transactions.Where(m => m.Consignment_no == i.Consignment_No).FirstOrDefault();

                if (tr != null)
                {
                    tr.Customer_Id = Customer_Id;
                    tr.Amount = i.Charges_Total;
                    tr.Pf_Code = pfcode;
                    tr.AdminEmp = EmpId;
                    db.Entry(tr).State = EntityState.Modified;
                    db.SaveChanges();
                    count++;
                }


            }

            ViewBag.success = count + "Records Updated SuccessFully";


            return View();
        }


        public ActionResult GetUserList()
        {
            db.Configuration.ProxyCreationEnabled = false;

            string Pfcode = Session["pfCode"].ToString();


            List<User> lstuser = new List<User>();

            lstuser = db.Users.Where(m => m.PF_Code == Pfcode).ToList();

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            string result = javaScriptSerializer.Serialize(lstuser);

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult MultipleBooking()
        {

            return View();
        }

        [HttpPost]
        public ActionResult MultipleBooking(string StartingCons, string EndingCons, string Companyid)
        {
            int EmpId = Convert.ToInt16(Session["EmpId"]);
            string pfcode = Session["pfCode"].ToString();

            char stch = StartingCons[0];
            char Endch = EndingCons[0];

            long startConsignment = Convert.ToInt64(StartingCons.Substring(1));
            long EndConsignment = Convert.ToInt64(EndingCons.Substring(1));

            int countconsigmnets = 0;
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
                        transaction.AdminEmp = EmpId;
                        transaction.Pf_Code = pfcode;
                        db.Entry(transaction).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    countconsigmnets++;

                    if (countconsigmnets >= 100)
                    {
                        break;
                    }


                    ViewBag.Message = "Booking Completed Successfully";
                }
            }




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



        public ActionResult ToAddressReport()
        {
            List<TransactionView> list = new List<TransactionView>();

            return View(list);
        }

        [HttpPost]
        public ActionResult ToAddressReport(string Fromdatetime, string ToDatetime, string Custid, string Submit)
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



            List<TransactionView> transactions =
                db.TransactionViews.Where(m =>
               (m.Customer_Id == Custid)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m=>m.booking_date)
                           .ToList();

          

            if (Submit == "Export to Excel")
            {
                var import = db.TransactionViews.Where(m =>
                (m.Customer_Id == Custid)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Consignment_no, BookingDate = x.tembookingdate, x.Name, x.Pincode, x.compaddress }).ToList();
                ExportToExcelAll.ExportToExcelAdmin(import);
            }

            else if (Submit == "Email" && Custid != null)
            {

                var company = db.Companies.Where(m => m.Company_Id == Custid).FirstOrDefault();
                var senderdetails = db.Franchisees.Where(m => m.PF_Code == company.Pf_code).FirstOrDefault();

                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[6] {
                                            new DataColumn("Sr No"),
                                            new DataColumn("Consignment no"),
                                             new DataColumn("Booking Date"),
                                            new DataColumn("Destination"),
                                            new DataColumn("Pincode"),
                                            new DataColumn("Address")
            });

                var Boqs = db.TransactionViews.Where(m =>
              (m.Customer_Id == Custid)
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                           .ToList();

                int j = 1;
                foreach (var i in Boqs)
                {
                    dt.Rows.Add(j, i.Consignment_no, i.booking_date.Value.ToString("dd/MM/yyyy"), i.Name, i.Pincode, i.compaddress);
                    j++;
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        byte[] bytes = stream.ToArray();
                        stream.Close();
                        MemoryStream memoryStream = new MemoryStream(bytes);

                        using (MailMessage mm = new MailMessage(senderdetails.Sendermail, company.Email))
                        {
                            mm.Subject = "Booking Details";

                            mm.Body = string.Format("<br/> please Find Attachment");
                            Attachment attachment = new Attachment(memoryStream, "Booking.xlsx");
                            //Add Byte array as Attachment.

                            mm.Attachments.Add(attachment);
                            mm.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                            credentials.UserName = senderdetails.Sendermail;
                            credentials.Password = senderdetails.password;
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = credentials;
                            smtp.Port = 587;
                            smtp.Send(mm);
                            ViewBag.Message = "Mail Sent Successfully";
                        }
                    }
                }


            }
            return View(transactions);
        }


    }
}
