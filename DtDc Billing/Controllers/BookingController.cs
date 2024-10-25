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
using DtDc_Billing.CustomModel;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Transactions;
using Transaction = DtDc_Billing.Entity_FR.Transaction;
using PagedList;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Web.Util;
using System.EnterpriseServices;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;

namespace DtDc_Billing.Controllers
{
    [SessionAdminold]
    //[OutputCache(CacheProfile = "Cachefast")]
    public class BookingController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Booking
        public ActionResult ConsignMent()
        {
            // Retrieve the message from TempData
            string uploadMessage = TempData["Upload"] as string;
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.PfCode = strpfcode;

            // Pass the message to the view using ViewBag
            ViewBag.UploadMessage = uploadMessage;

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
            var pincode = transaction.Pincode;




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
                if (pincode != null)
                {
                    var checkpincode = db.Destinations.Where(x => x.Pincode == pincode).FirstOrDefault();
                    if (checkpincode == null)
                    {
                        ViewBag.notvalidpincode = "Please select Valid Pincode";
                        return PartialView("ConsignmentPartial", transaction);
                    }
                }

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
                    tran.Pincode = transaction.Pincode.Trim();
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
                    tran.Receiver = transaction.Receiver;
                    tran.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                    tran.AdminEmp = 000;
                    tran.Receiver = transaction.Receiver;
                    tran.isDelete = false;
                    tran.IsGSTConsignment = false;
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
                    tran1.Pincode = transaction.Pincode.Trim();
                    tran1.Mode = transaction.Mode;
                    tran1.Weight_t = transaction.Weight_t;
                    tran1.Amount = transaction.Amount;
                    tran1.Company_id = transaction.Company_id;
                    tran1.Quanntity = transaction.Quanntity;
                    tran1.Type_t = transaction.Type_t;
                    tran1.Insurance = transaction.Insurance;
                    tran1.Claimamount = transaction.Claimamount;
                    tran1.Percentage = transaction.Percentage;
                    tran1.Receiver = transaction.Receiver;

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
                    tran1.isDelete = false;
                    tran1.IsGSTConsignment = false;
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

                //char ch = transaction.Consignment_no[0];

                //long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(1));

                //consignnumber = consignnumber + 1;

                //var lenght = transaction.Consignment_no.Substring(1).Length;

                //ViewBag.nextconsignment = ch + "" + (consignnumber.ToString().PadLeft(lenght,'0'));


                //old logic before ecom end

                if (transaction.Consignment_no.ToLower().StartsWith("7d"))
                {
                    string ch = transaction.Consignment_no.Substring(0, 2);
                    long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(2));
                    long consignnumberadd = consignnumber + 1;
                    var lenght = transaction.Consignment_no.Substring(2).Length;

                    ViewBag.nextconsignment = ch + "" + (consignnumberadd.ToString().PadLeft(lenght, '1'));
                }

                else if (transaction.Consignment_no.ToLower().StartsWith("7x"))
                {
                    string ch = transaction.Consignment_no.Substring(0, 2);
                    long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(2));
                    consignnumber = consignnumber + 1;
                    var lenght = transaction.Consignment_no.Substring(2).Length;

                    ViewBag.nextconsignment = ch + "" + (consignnumber.ToString().PadLeft(lenght, '1'));
                }

                else
                {
                    char ch = transaction.Consignment_no[0];

                    long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(1));

                    consignnumber = consignnumber + 1;

                    var lenght = transaction.Consignment_no.Substring(1).Length;

                    ViewBag.nextconsignment = ch + "" + (consignnumber.ToString().PadLeft(lenght, '0'));

                }


                return PartialView("ConsignmentPartial");
            }

            return PartialView("ConsignmentPartial", transaction);
        }

        public ActionResult CustomerIdAutocomplete()
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var entity = db.Companies.
                    Select(e => new
                    {
                        e.Company_Id,
                        e.Pf_code,
                        e.Company_Name
                    }).Where(d => d.Pf_code == strpfcode).Distinct().OrderBy(d => d.Company_Id).ToList();



            //var entity = db.Companies.Select(e => new
            //        {
            //            e.Company_Id,
            //            e.Pf_code
            //        }).Distinct().Where(e=>e.Pf_code== strpfcode).OrderBy(e=>e.Company_Id).ToList();


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
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.PfCode = strpfcode;

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
                var pincode = transaction.Pincode;

                if (pincode != null)
                {
                    var checkpincode = db.Destinations.Where(x => x.Pincode == pincode).FirstOrDefault();
                    if (checkpincode == null)
                    {
                        ViewBag.notvalidpincode = "Please select Valid Pincode";
                        return PartialView("EditConsignmentPartial", transaction);
                    }
                }
                Transaction tr = db.Transactions.Where(m => m.Consignment_no == transaction.Consignment_no).FirstOrDefault();


                string[] formats = { "dd-MM-yyyy" };

                string bdate = DateTime.ParseExact(transaction.tembookingdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                transaction.booking_date = Convert.ToDateTime(bdate);


                if (tr != null)
                {

                    db.Entry(tr).State = EntityState.Detached;


                    transaction.T_id = tr.T_id;


                    Transaction tran = new Transaction();
                    tran.T_id = tr.T_id;
                    tran.Customer_Id = transaction.Customer_Id;
                    tran.booking_date = transaction.booking_date;
                    tran.Consignment_no = transaction.Consignment_no.Trim();
                    tran.Pincode = transaction.Pincode.Trim();
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
                    tran.Receiver = transaction.Receiver;
                    tran.isDelete = false;
                    db.Entry(tran).State = EntityState.Modified;

                    db.SaveChanges();
                    ViewBag.Message = "Consignment Updated SuccessFully";
                }

                ModelState.Clear();

                ViewBag.success = true;

                if (transaction.Consignment_no.ToLower().StartsWith("7d"))
                {
                    string ch = transaction.Consignment_no.Substring(0, 2);
                    long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(2));
                    long consignnumberadd = consignnumber + 1;
                    var lenght = transaction.Consignment_no.Substring(2).Length;

                    ViewBag.nextconsignment = ch + "" + (consignnumberadd.ToString().PadLeft(lenght, '1'));
                }

                else if (transaction.Consignment_no.ToLower().StartsWith("7x"))
                {
                    string ch = transaction.Consignment_no.Substring(0, 2);
                    long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(2));
                    consignnumber = consignnumber + 1;
                    var lenght = transaction.Consignment_no.Substring(2).Length;

                    ViewBag.nextconsignment = ch + "" + (consignnumber.ToString().PadLeft(lenght, '1'));
                }

                else
                {
                    char ch = transaction.Consignment_no[0];

                    long consignnumber = Convert.ToInt64(transaction.Consignment_no.Substring(1));

                    consignnumber = consignnumber + 1;

                    var lenght = transaction.Consignment_no.Substring(1).Length;

                    ViewBag.nextconsignment = ch + "" + (consignnumber.ToString().PadLeft(lenght, '0'));

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


                ViewBag.consignments = obj.Select(m => m.Consignment_no).ToList();
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
                    loadingcharge = x.loadingcharge,


                }).OrderBy(d => d.Consignment_no).ToList();


                ViewBag.consignments = obj.Select(m => m.Consignment_no).ToList();
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

            string input1 = "";
            string input2 = "";
            string stch = "";
            string Endch = "";

            // Check if the first character is a digit
            if (char.IsDigit(StartingCons[0]))
            {
                // If it's a digit, remove the first two characters
                stch = StartingCons.Substring(0, 2);
                input1 = StartingCons.Substring(2);
            }
            else
            {
                // If it's not a digit, remove the first character
                stch = StartingCons.Substring(0, 1);
                input1 = StartingCons.Substring(1);
            }


            // Check if the first character is a digit
            if (char.IsDigit(EndingCons[0]))
            {
                // If it's a digit, remove the first two characters
                Endch = EndingCons.Substring(0, 2);
                input2 = EndingCons.Substring(2);
            }
            else
            {
                // If it's not a digit, remove the first character
                Endch = EndingCons.Substring(0, 1);
                input2 = EndingCons.Substring(1);
            }


            long startConsignment = Convert.ToInt64(input1);
            long EndConsignment = Convert.ToInt64(input2);



            int countconsigmnets = 0;
            var pfcode = db.Companies.Where(m => m.Company_Id == Companyid).Select(m => m.Pf_code).FirstOrDefault();

            if (stch == Endch)
            {

                for (long i = startConsignment; i <= EndConsignment; i++)
                {
                    string updateconsignment = stch + i.ToString();


                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == updateconsignment && m.isDelete==false).FirstOrDefault();

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

                    if (countconsigmnets >= 100)
                    {
                        break;
                    }

                }
            }



            ViewBag.Message = "Booking Completed Successfully";

            return View();
        }

        public ActionResult Checkbookinglist(string Fromdatetime, string ToDatetime, string Custid, string Submit)
        {
            List<TransactionView> list = new List<TransactionView>();
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.PfCode = strpf;
            ViewBag.fromdate = Fromdatetime;
            ViewBag.todate = ToDatetime;
            ViewBag.Custid = Custid;
            //var obj = db.getCheckBookingListAll(strpf).Select(x => new TransactionView
            //{

            //  Consignment_no = x.Consignment_no,
            //  chargable_weight  = x.chargable_weight,
            //  Quanntity  = x.Quanntity,
            //  Name  = x.Name,
            //  Pincode  = x.Pincode,
            //  compaddress  = x.compaddress,
            //  Type_t = x.Type_t,
            //  Mode  = x.Mode,
            //  Amount  = x.Amount,
            //  booking_date  = x.booking_date,
            //  Insurance  = x.Insurance,
            //  BillAmount= x.BillAmount,
            //  Percentage  = x.Percentage,
            //  Risksurcharge  = x.Risksurcharge,
            //  loadingcharge  = x.loadingcharge

            //}).ToList();

            //ViewBag.totalamt = obj.Sum(b => b.Amount);
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate=DateTime.Now;
            DateTime? todate=DateTime.Now;
          
            if (Fromdatetime != "" && Fromdatetime!=null)
            {
                string bdatefrom = DateTime.ParseExact(Fromdatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

                ViewBag.fromdate = Fromdatetime;
            }
            else
            {
                fromdate = DateTime.Now;
            }

            if (ToDatetime != "" && ToDatetime!=null)
            {
                string bdateto = DateTime.ParseExact(ToDatetime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
                ViewBag.todate = ToDatetime;
            }
            else
            {
                todate = DateTime.Now;
            }

            if (Custid != "" && Custid != null)
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

                }).OrderBy(d => d.booking_date).ToList();

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
                    loadingcharge = x.loadingcharge,

                }).OrderByDescending(d => d.booking_date).ToList();

                ViewBag.totalamt = obj.Sum(b => b.Amount);

             

                return View(obj);
            }

        }


        [HttpPost]
        public ActionResult Checkbookinglist(List<TransactionView> trans, string Fromdatetime, string ToDatetime, string Custid, string Submit)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            DateTime? fromdate;
            DateTime? todate;
            if (trans != null)
            {
                ModelState.Clear();

            }
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
                    loadingcharge = x.loadingcharge,
                    Customer_Id=x.customer_id

                }).OrderBy(d => d.booking_date).ToList();

                ViewBag.totalamt = obj.Sum(b => b.Amount);

                if (Submit == "Export to Excel")
                {
                    obj = obj.OrderByDescending(b => b.booking_date).Where(x => x.isRTO == null || x.isRTO == false).ToList();
                    //var import = db.TransactionViews.ToList().Where(m=>(m.Pf_Code==strpf) &&(m.Customer_Id==null || m.Customer_Id==Custid)).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                    //    .Select(x => new { x.Consignment_no, Weight = x.chargable_weight, x.Quanntity, x.Name, x.Pincode, x.compaddress, x.Type_t, x.Mode, x.Amount, BookingDate = x.tembookingdate, x.Insurance, x.Claimamount, x.Percentage, Risksurcharge = x.calinsuranceamount, Total = (x.Amount + x.calinsuranceamount) })
                    //    .OrderByDescending(m=>m.BookingDate).ToList();
                    var data = obj.Select(x => new {
                        ConsignmentNo = x.Consignment_no,
                        Weight = x.chargable_weight,
                        x.Quanntity,
                        Destination = db.Destinations.Where(m => m.Pincode == x.Pincode).Select(m => m.Name).FirstOrDefault(),
                        Pincode = x.Pincode,
                        Address = x.compaddress,
                        Type = x.Type_t,
                        x.Mode,
                        x.Amount,
                        BookingDate = x.booking_date.Value.ToString("dd/MM/yyyy"),
                        x.Insurance,
                        x.Claimamount,
                        x.Percentage,
                        x.Risksurcharge,
                        OtherCharges = x.loadingcharge,
                        Total = Math.Round(x.Amount ?? 0 + x.Risksurcharge ?? 0 + x.loadingcharge ?? 0)



                    }).ToList();
                    ExportToExcelAll.ExportToExcelAdmin(data);
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
                    loadingcharge = x.loadingcharge,
                    Customer_Id=x.customer_id

                }).OrderByDescending(d => d.booking_date).ToList();

                ViewBag.totalamt = obj.Sum(b => b.Amount);

                if (Submit == "Export to Excel")
                {
                    //var import = db.TransactionViews.Where(m => (m.Pf_Code == strpf) &&
                    //(m.Customer_Id == null || m.Customer_Id == Custid)
                    //    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Consignment_no, Weight = x.chargable_weight, x.Quanntity, x.Name, x.Pincode, x.compaddress, x.Type_t, x.Mode, x.Amount, BookingDate = x.tembookingdate, x.Insurance, x.Claimamount, x.Percentage, Risksurcharge = x.calinsuranceamount, Total = (x.Amount + x.calinsuranceamount) }).OrderByDescending(m=>m.BookingDate).ToList();
                    obj = obj.OrderByDescending(b => b.booking_date).Where(x => x.isRTO == null || x.isRTO == false).ToList();
                    var data = obj.Select(x => new {
                        CustomerId=x.Customer_Id,
                        ConsignmentNo = x.Consignment_no,
                        Weight = x.chargable_weight,
                        x.Quanntity,
                        Destination = db.Destinations.Where(m => m.Pincode == x.Pincode).Select(m => m.Name).FirstOrDefault(),
                        Pincode = x.Pincode,
                        Address = x.compaddress,
                        Type = x.Type_t,
                        x.Mode,
                        x.Amount,
                        BookingDate = x.booking_date.Value.ToString("dd/MM/yyyy"),
                        x.Insurance,
                        x.Claimamount,
                        x.Percentage,
                        x.Risksurcharge,
                        OtherCharges = x.loadingcharge,
                        Total = Math.Round(x.Amount ?? 0 + x.Risksurcharge ?? 0 + x.loadingcharge ?? 0)



                    }).ToList();
                    ExportToExcelAll.ExportToExcelAdmin(data);
                }

                return View(obj);
            }

        }
        //Convert the Consignment for the RTO 

        public JsonResult CovertConsignmentToRTO(string consignmnets,string fromdate,string todate,string custId)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            try
            {
                string[] conno = consignmnets.Split(',');

                foreach (var con in conno)
                {
                    var trans = db.Transactions.Where(x => x.Consignment_no == con && x.Pf_Code == strpf ).FirstOrDefault();
                    if (trans != null)
                    {
                        trans.isRTO = true;
                        db.Entry(trans).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
               return Json("Success", JsonRequestBehavior.AllowGet);
             //   return Json(new { redirectUrl = Url.Action("Checkbookinglist", "Booking", new { Fromdatetime = fromdate, ToDatetime = todate, Custid = custId }) },JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Nobookinglist()
        {
            List<Transaction> list = new List<Transaction>();
            ViewBag.PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();//new SelectList(db.Franchisees, "PF_Code", "PF_Code");
            return View(list);
        }

        [HttpPost]
        public ActionResult Nobookinglist(string Fromdatetime, string ToDatetime, string PfCode, string Submit)
        {
            PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

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
               (m.Pf_Code == PfCode) && (m.Customer_Id == null || m.Customer_Id == "")
               && m.isDelete==false
                    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderByDescending(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();


            if (Submit == "Export to Excel")
            {
                //var import = db.Transactions.Where(m =>
                //(m.Pf_Code == PfCode)&& (m.Customer_Id == null || m.Customer_Id == "")
                //    ).ToList().Where(m => m.booking_date.Value.Date >= fromdate.Value.Date && m.booking_date.Value.Date <= todate.Value.Date).OrderByDescending(m => m.booking_date).ThenBy(n => n.Consignment_no).Select(x => new { x.Pf_Code, x.Consignment_no, Weight=x.Actual_weight, x.Pincode, x.Amount,BookingDate= x.tembookingdate }).ToList();
                if (transactions.Count != 0)
                {
                    ExportToExcelAll.ExportToExcelAdmin(transactions);
                }
                else
                {
                    ViewBag.ErrorMessage = "No Data Found";
                }
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
        public string checkpincode(string Pincode)
        {
            var pincode = db.Destinations.Where(x => x.Pincode == Pincode).FirstOrDefault();
            if (pincode == null)
            {
                return "NotValidPinCode";
            }
            return "Valid";
        }
        public string Checkcompany(string Customerid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            string pfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var suggestions = (from s in db.Companies
                               where s.Company_Id == Customerid && s.Pf_code == pfCode
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

            //var entity = db.Destinations.Where(m => m.Pincode.StartsWith("111")).
            //    Select(e => new
            //    {
            //        e.Name
            //    }).Distinct().ToList();

            var entity = db.Destinations.Where(x => x.Name != null && x.Name != "").
                Select(e => new
                {
                    e.Name
                }).OrderBy(e => e.Name).Distinct().ToList();



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

                        Transaction transaction = db.Transactions.Where(m => m.Consignment_no == i.Consignment_no && m.Pf_Code == strpf && m.isDelete==false).FirstOrDefault();

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

                ViewBag.totalamt = obj.Sum(b => b.Amount+b.Risksurcharge??0+b.loadingcharge??0);

                return View(obj);

            }
            else
            {
                var obj = db.getCheckBookingList(fromdate, todate, Custid, strpf).Select(x => new TransactionView
                {
                    Customer_Id = x.customer_id,
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

                        Transaction transaction = db.Transactions.Where(m => m.Consignment_no == i.Consignment_no && m.Pf_Code == strpf && m.isDelete == false).FirstOrDefault();

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

                ViewBag.totalamt = obj.Sum(b =>  b.Amount+b.Risksurcharge??0+b.loadingcharge??0);

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
            Transaction tran = db.Transactions.Where(m => m.Consignment_no == Consignment && m.isDelete == false).FirstOrDefault();

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


        [HttpGet]
        public ActionResult importFromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult importFromExcel(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase != null)
            {
                var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
                ImportConsignmentFromExcel importConsignmentFromExcel = new ImportConsignmentFromExcel();
                var damageResult = importConsignmentFromExcel.Import1Async(httpPostedFileBase, PfCode);

                TempData["success"] = "File uploaded successfully! It will take some time to reflect ";
            }
            else
            {
                TempData["error"] = "Please upload file";
            }

            return View();
        }




        [HttpGet]
        public ActionResult importFromExcelWhole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult importFromExcelWhole(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase != null)
            {
                var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
                ImportConsignmentFromExcel importConsignmentFromExcel = new ImportConsignmentFromExcel();
                var damageResult = importConsignmentFromExcel.Import2Async(httpPostedFileBase, PfCode);

                TempData["success"] = "File uploaded successfully! It will take some time to reflect ";
            }
            else
            {
                TempData["error"] = "Please upload file";
            }


            return RedirectToAction("importFromExcel");
        }


        [HttpGet]
        public ActionResult AddNewimporFromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewimporFromExcel(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase != null)
            {
                try
                {
                    var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
                    ImportConsignmentFromExcel importConsignmentFromExcel = new ImportConsignmentFromExcel();
                    var damageResult = importConsignmentFromExcel.Import3Async(httpPostedFileBase, PfCode);
                    if (damageResult == "1")
                    {
                        TempData["error"] = "Something Went Wrong\n<b style=" + "color:red" + ">May be Issue in the Excel</b>";
                    }
                    TempData["success"] = "File uploaded successfully! It will take some time to reflect ";
                }
                catch (Exception ex)
                {

                    return PartialView("~/Views/Shared/Error.cshtml");
                }
            }
            else
            {
                TempData["error"] = "Please upload file";
            }
            return RedirectToAction("importFromExcel");
        }


        [HttpGet]
        public ActionResult AddCodTopayimporFromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCodTopayimporFromExcel(HttpPostedFileBase httpPostedFileBase)
        {
            if (httpPostedFileBase != null)
            {
                try
                {
                    var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
                    ImportConsignmentFromExcel importConsignmentFromExcel = new ImportConsignmentFromExcel();
                    var damageResult = importConsignmentFromExcel.ImportCodTopayAsync(httpPostedFileBase, PfCode);
                    if (damageResult == "1")
                    {
                        TempData["error"] = "Something Went Wrong\n<b style=" + "color:red" + ">May be Issue in the Excel</b>";
                    }
                    TempData["success"] = "File uploaded successfully! It will take some time to reflect ";
                }
                catch (Exception ex)
                {

                    return PartialView("~/Views/Shared/Error.cshtml");
                }
            }
            else
            {
                TempData["error"] = "Please upload file";
            }
            return RedirectToAction("importFromExcel");
        }


        public ActionResult importTextFile()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> importTextFile(HttpPostedFileBase ImportText)
        {
            string filePath = string.Empty;

            if (ImportText != null)
            {
                string path = Server.MapPath("~/UploadsText/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + Path.GetFileName(ImportText.FileName);
                string extension = Path.GetExtension(ImportText.FileName);
                ImportText.SaveAs(filePath);

                Task.Run(() => InsertRecords(filePath, ImportText.FileName));

            }

            TempData["Upload"] = "File Uploaded Successfully!";

            return RedirectToAction("ConsignMent", "Booking");
        }


        public void InsertRecords(string filePath, string Filename)
        {
            List<Transaction> Tranjaction = new List<Transaction>();



            //Read the contents of CSV file.
            string csvData = System.IO.File.ReadAllText(filePath);

            var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            //Execute a loop over the rows.
            int i = 0;
            foreach (string row in csvData.Split('\n'))
            {
                i++;
                if (i <= 2)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(row))
                {

                    string[] values = row.Split('"');


                    Transaction tr = new Transaction();

                    string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};
                    string bdate = DateTime.ParseExact(values[10].Replace("~", "").Trim('\''), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");



                    tr.Consignment_no = values[1].Trim('\'').Trim();
                    tr.Pf_Code = values[3].Trim('\'');
                    tr.Actual_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                    tr.Mode = values[5].Trim('\'');
                    tr.Quanntity = Convert.ToInt16(values[8].Trim('\''));
                    tr.Pincode = values[9].Trim('\'');
                    tr.booking_date = Convert.ToDateTime(bdate);
                    tr.tembookingdate = values[10].Trim('\'');
                    tr.dtdcamount = Convert.ToDouble(values[11].Replace("~", "").Trim('\''));
                    tr.chargable_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                    tr.diff_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                    tr.topay = "no";
                    tr.cod = "no";
                    //tr.Insurance = "no";
                    tr.Type_t = values[16].Trim('\'');
                    tr.BillAmount = Convert.ToDouble(values[21].Replace("~", "").Trim('\''));
                    tr.isDelete = false;
                    if (tr.BillAmount == 0.00)
                    {
                        tr.Insurance = "nocoverage";
                    }
                    else
                    {
                        tr.Insurance = "ownerrisk";
                    }


                    Transaction insertupdate = db.Transactions.Where(m => m.Consignment_no == tr.Consignment_no && m.Pf_Code == PfCode).FirstOrDefault();





                    if (insertupdate == null)
                    {
                        // db.Entry(insertupdate).State = EntityState.Detached;

                        db.Transactions.Add(tr);
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

                    }
                    else
                    {
                        insertupdate.Pf_Code = values[3].Trim('\'');
                        insertupdate.dtdcamount = Convert.ToDouble(values[11].Replace("~", "").Trim('\''));
                        insertupdate.diff_weight = Convert.ToDouble(values[4].Replace("~", "").Trim('\''));
                        insertupdate.Consignment_no = insertupdate.Consignment_no.Trim();

                        insertupdate.BillAmount = Convert.ToDouble(values[21].Replace("~", "").Trim('\''));
                        insertupdate.Insurance = tr.Insurance;
                        insertupdate.isDelete = false;
                        db.Entry(insertupdate).State = EntityState.Modified;

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

                        // db.SaveChanges();

                    }




                }


            }


        }


        [HttpPost]
        public async Task<ActionResult> InternationalimportTextFile(HttpPostedFileBase ImportText)
        {

            string filePath = string.Empty;

            if (ImportText != null)
            {
                string path = Server.MapPath("~/UploadsText/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + Path.GetFileName(ImportText.FileName);
                string extension = Path.GetExtension(ImportText.FileName);
                ImportText.SaveAs(filePath);

                Task.Run(() => InsertRecords(filePath, ImportText.FileName));

            }

            TempData["Upload"] = "File Uploaded Successfully!";
            return RedirectToAction("ConsignMent", "Booking");
        }




        //Sr.No Consignment No Chargable Weight Mode    Company Address Quanntity Pincode BookingDate(dd-MM-YYYY)    Type Customer Id other changes Receiver

        public ActionResult ExportCSV()
        {
            string csvContent = "SrNo,ConsignmentNo,ChargableWeight,Mode,CompanyAddress,Quantity,Pincode,BookingDate(dd/MM/yyyy)/(dd-MM-yyyy),Type,CustomerId,OtherCharges,Receiver\n";

            byte[] csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);

            return File(csvBytes, "text/csv", "UploadNewCSV.csv");
        }
        [HttpPost]
        public ActionResult ImportCSV(HttpPostedFileBase file)
        {



            if (file != null)
            {
                var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

                var passcsvdata = SaveBookingDatabyCSVFile(file, PfCode);

                TempData["success"] = "File uploaded successfully! It will take some time to reflect ";
            }
            else
            {
                TempData["error"] = "Please upload file";
            }
            return RedirectToAction("importFromExcel");


        }
        public string SaveBookingDatabyCSVFile(HttpPostedFileBase httpPostedFileBase, string PfCode)
        {
            var damageResult = Task.Run(() => asyncAddNewImportFromCSVFile(httpPostedFileBase, PfCode));

            return damageResult.ToString();
        }
        public async Task<string> asyncAddNewImportFromCSVFile(HttpPostedFileBase file, string strpfcode)
        {
            string[] formats = { "dd-MM-yyyy", "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d-MM-yyyy", "dd-M-yyyy" };
            if (file != null && file.ContentLength > 0)
            {
                using (StreamReader reader = new StreamReader(file.InputStream))
                {
                    // Skip the header row
                    reader.ReadLine();

                    // Process CSV data and store in the database
                    while (!reader.EndOfStream)
                    {
                        Transaction tran = new Transaction();
                        string line = reader.ReadLine();
                        string[] fields = line.Split(',');

                        //// Assuming fields are in the correct order as per the model
                        //var newBooking = new ExportBookingCSV
                        //{
                        //    SrNo = int.Parse(fields[0]),
                        //    ConsignmentNo = fields[1],
                        //    ChargableWeight = double.Parse(fields[2]), // Parse as double
                        //    Mode = fields[3],
                        //    CompanyAddres = fields[4],
                        //    Quanntity = int.Parse(fields[5]),
                        //    Pincode = (fields[6]),



                        //    BookingDate = DateTime.ParseExact(fields[7], formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MM-yyyy"),

                        //    Type = fields[8],
                        //    CustomerId = fields[9],
                        //    otherchanges = float.Parse(fields[10]),
                        //    Receiver = fields[11]
                        //};

                        tran.Consignment_no = fields[1];
                        tran.chargable_weight = double.Parse(fields[2]);
                        tran.Mode = fields[3];
                        tran.compaddress = fields[4];
                        tran.Quanntity = int.TryParse(fields[5], out int quantity) ? quantity : 0;
                        tran.Pincode = fields[6];
                        tran.isDelete = false;
                        string dateformat = fields[7];

                        tran.tembookingdate = DateTime.ParseExact(fields[7], formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        tran.Type_t = fields[8];
                        tran.Customer_Id = fields[9];
                        tran.loadingcharge = float.TryParse(fields[10], out float charge) ? charge : 0.0f;

                        tran.Receiver = fields[11];


                        Transaction transaction = db.Transactions.Where(m => m.Consignment_no == tran.Consignment_no && m.Pf_Code == strpfcode).FirstOrDefault();
                        var Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id && m.Pf_code == strpfcode).Select(m => m.Pf_code).FirstOrDefault();
                        string bdatestring;
                        if (Pf_Code != null)
                        {

                            if (transaction != null)
                            {

                                CalculateAmount ca = new CalculateAmount();

                                double? amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);

                                transaction.Amount = amt;
                                transaction.Customer_Id = tran.Customer_Id;

                                transaction.Consignment_no = tran.Consignment_no;
                                transaction.chargable_weight = tran.chargable_weight;
                                transaction.Mode = tran.Mode;
                                transaction.compaddress = tran.compaddress;
                                transaction.Quanntity = tran.Quanntity;
                                transaction.Pincode = tran.Pincode;
                                bdatestring = DateTime.ParseExact(tran.tembookingdate.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                                transaction.booking_date = Convert.ToDateTime(bdatestring);
                                transaction.Type_t = tran.Type_t;
                                transaction.tembookingdate = tran.tembookingdate;
                                transaction.Pf_Code = db.Companies.Where(m => m.Company_Id == transaction.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                transaction.AdminEmp = 000;
                                transaction.loadingcharge = tran.loadingcharge;
                                tran.Receiver = tran.Receiver;
                                tran.isDelete = false;

                                db.Entry(transaction).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                CalculateAmount ca = new CalculateAmount();


                                double? amt = ca.CalulateAmt(tran.Consignment_no, tran.Customer_Id, tran.Pincode, tran.Mode, Convert.ToDouble(tran.chargable_weight), tran.Type_t);

                                tran.Amount = amt;

                                string bdate = DateTime.ParseExact(tran.tembookingdate.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                                tran.tembookingdate = tran.tembookingdate;
                                tran.booking_date = Convert.ToDateTime(bdate);
                                
                                tran.Pf_Code = db.Companies.Where(m => m.Company_Id == tran.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();
                                tran.AdminEmp = 000;
                                tran.isDelete=false;
                                db.Transactions.Add(tran);
                                db.SaveChanges();

                            }

                        }



                    }
                }

                return "1";
            }
            else
            {
                return "0";
            }
        }

        [HttpGet]
        public ActionResult RecycleConsignment()
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var list =db.Transactions.Where(x=>x.Pf_Code==strpfcode && x.isDelete==true).ToList();
            return View(list);
        }
            public ActionResult RestoreConsignment(string consignmentno)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var data =db.Transactions.Where(x=>x.Consignment_no==consignmentno && x.Pf_Code==strpfcode).FirstOrDefault();
            if (data != null)
            {
                data.isDelete= false;
                db.Entry(data).State=EntityState.Modified;  
                db.SaveChanges();
                TempData["Message"] = "Consignment Restore Successfully";
                return RedirectToAction("RecycleConsignment");

            }
            TempData["Message"] = "Something Went Wrong";

            return RedirectToAction("RecycleConsignment");
        }

        [HttpGet]
        public ActionResult UploadPincodeFromExcel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadPincodeFromExcel(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    try
                    {
                        var PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
                        ImportPincodeFromExcel importPincode = new ImportPincodeFromExcel();
                        var damageResult = importPincode.ImportPincodeAsync(file, PfCode);
                        if (damageResult == "1")
                        {
                            TempData["error"] = "Something Went Wrong\n<b style=" + "color:red" + ">May be Issue in the Excel</b>";
                        }
                        TempData["success"] = "File uploaded successfully! It will take some time to reflect ";
                        ModelState.Clear();
                    }
                    catch (Exception ex)
                    {

                        return PartialView("~/Views/Shared/Error.cshtml");
                    }
                }
                else
                {
                    TempData["error"] = "Please upload file";
                }
                return View();

            }
            
            return View();

        }
    }
}