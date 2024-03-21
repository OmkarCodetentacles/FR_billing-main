using CustomerModel;
using DtDc_Billing.CustomModel;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Ionic.Zip;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI;
using static System.Net.WebRequestMethods;

namespace DtDc_Billing.Controllers
{
    [SessionAdminold]
   // [SessionUserModule]
    //[OutputCache(CacheProfile = "Cachefast")]
    public class InvoiceController : Controller
    {

        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();


        // string invstart = "INV/2022-23/";
        string invstart = "INV/2023-24/";

        //[OutputCache(Duration = 600, VaryByParam = "none", Location = OutputCacheLocation.Server)]
        public ActionResult GenerateInvoice(string Invoiceno = null)
        {


            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var franchisee=db.Franchisees.Where(x=>x.PF_Code == strpfcode).FirstOrDefault(); 
            //if (Firm_Id == 1)
            //{
            var dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();
           
            // INV/1411/2024-25/001
            // PF2214


            string invstart1 = dataInvStart + "/2023-24/";
            string no = "";
            string finalstring = "";
            if (strpfcode == "PF2214")
            {
                dataInvStart = (from d in db.Franchisees
                                    where d.PF_Code == strpfcode
                                    select d.InvoiceStart).FirstOrDefault();

              invstart1 = dataInvStart + "/2024-25/";


            }
          

            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
            string lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "00";

           
            if (lastInvoiceno == null)
            {
                string[] strarrinvno = lastInvoiceno1.Split('/');
                if(franchisee.PF_Code== "PF2214")
                {
                    strarrinvno = lastInvoiceno1.Split('/');
                    ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[3] + 1);

                }
                else if(franchisee.PF_Code == "PF975")
                {
                    ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);
                    if (strarrinvno[2] == "00")
                    {
                        strarrinvno[2] = "597";
                        ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2]);

                    }




                }
                else
                {
                    ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);

                }
            }

            else
            {

                string[] strarrinvno = lastInvoiceno1.Split('/');
                //string val = lastInvoiceno.Substring(19, lastInvoiceno.Length - 19);
                int newnumber=0;
                if (franchisee.PF_Code == "PF2214")
                {
                    newnumber = Convert.ToInt32(strarrinvno[3]) + 1;
                }
                
                else
                {
                    newnumber = Convert.ToInt32(strarrinvno[2]) + 1;
                }

                finalstring = newnumber.ToString("000");
                ViewBag.lastInvoiceno = invstart1 + "" + finalstring;
            }

            var data = (from d in db.Invoices
                        where d.Pfcode == strpfcode
                        && d.invoiceno == Invoiceno
                        select d).FirstOrDefault();

            if (data != null)
            {
                InvoiceModel Inv = new InvoiceModel();


                Inv.invoiceno = data.invoiceno;
                Inv.invoicedate = data.invoicedate;
                Inv.periodfrom = data.periodfrom;
                Inv.periodto = data.periodto;
                Inv.total = data.total;
                Inv.fullsurchargetax = data.fullsurchargetax;
                Inv.fullsurchargetaxtotal = data.fullsurchargetaxtotal;
                Inv.servicetax = data.servicetax;
                Inv.servicetaxtotal = data.servicetaxtotal;
                Inv.othercharge = data.othercharge;
                Inv.netamount = data.netamount;
                Inv.Customer_Id = data.Customer_Id;
                Inv.fid = data.fid;
                Inv.annyear = data.annyear;
                Inv.paid = data.paid;
                Inv.status = data.status;
                Inv.discount = data.discount;
                Inv.discountper = data.discountper;
                Inv.discountamount = data.discountamount;
                Inv.servicecharges = data.servicecharges;
                Inv.Royalty_charges = data.Royalty_charges;
                Inv.Docket_charges = data.Docket_charges;
                Inv.Tempdatefrom = data.Tempdatefrom;
                Inv.TempdateTo = data.TempdateTo;
                Inv.tempInvoicedate = data.tempInvoicedate;
                Inv.Address = data.Address;
                Inv.Invoice_Lable = data.Invoice_Lable;
                Inv.Total_Lable = data.Total_Lable;
                Inv.Royalti_Lable = data.Royalti_Lable;
                Inv.Docket_Lable = data.Docket_Lable;
                Inv.Amount4 = data.Amount4;
                Inv.Amount4_Lable = data.Amount4_Lable;
                Inv.Pfcode = data.Pfcode;

                return View(Inv);
            }

            return View();


        }

        public ActionResult getFirm()
        {
            return Json(db.FirmDetails.Select(x => new
            {
                Firm_Id = x.Firm_Id,
                Firm_Name = x.Firm_Name
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DpInvoice(long Firm_Id = 1, string Invoiceno = null)
        {

            if (Firm_Id == 1)
            {
                string invstart1 = "IFS/21-22/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else if (Firm_Id == 2)
            {
                string invstart1 = "SHE/21-22/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else if (Firm_Id == 3)
            {
                string invstart1 = "ATE/21-22/";
                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + 000;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart1 + "" + (number + 1);
            }
            else
            {

                string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
                int number = Convert.ToInt32(lastInvoiceno.Substring(10));

                ViewBag.lastInvoiceno = invstart + "" + (number + 1);
            }

            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno && m.Firm_Id == Firm_Id).FirstOrDefault();

            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;


            if (Invoiceno != null && Invoiceno.StartsWith("INV/20-21/"))
            {
                return RedirectToAction("GenerateInvoiceLastYear", new { Invoiceno = Invoiceno });
            }
            else
            {
                return View(inv);
            }
        }
        // GET: Invoice
        //[HttpGet]
        //public ActionResult ViewInvoice()
        //{
        //    string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

        //    List<InvoiceModel> list = new List<InvoiceModel>();
        //    //ViewBag.PfCode = new SelectList(db.Franchisees.Where(d=>d.PF_Code== strpf), "PF_Code", "PF_Code");
        //    //ViewBag.FirmDetails = new SelectList(db.FirmDetails, "Firm_Id", "Firm_Name");
        //    return View(list);

        //}
        [HttpGet]
        public ActionResult ViewInvoice(string invfromdate, string Companydetails, string invtodate, string invoiceNo,string invoiceNotoDelete, bool isDelete = false)
        {
            List<InvoiceModel> list = new List<InvoiceModel>();
      

            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();
           

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string fromdate = "";

            string todate = "";

            if (isDelete)
            {
                string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
                var checkInvoiceNo = db.Invoices.Where(x => x.invoiceno == invoiceNotoDelete && x.Pfcode == pfcode).FirstOrDefault();
                if (checkInvoiceNo == null)
                {
                    TempData["error"] = "Invalid Invoice No";

                }

                db.Invoices.Remove(checkInvoiceNo);
                db.SaveChanges();
                TempData["success"] = invoiceNotoDelete + " Delete successfully!";
            }
            if ((invfromdate != null && invfromdate!="") && (invtodate!=null && invtodate!=""))
            {
                fromdate = DateTime.ParseExact(invfromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy-MM-dd");
                todate = DateTime.ParseExact(invtodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy-MM-dd");
            }

            ViewBag.invfromdate = invfromdate;
            ViewBag.invtodate = invtodate;
            ViewBag.invoiceno = invoiceNo;

            ViewBag.Companydetails = Companydetails;//new SelectList(db.Companies, "Company_Id", "Company_Name");
            
            if (strpf != null && strpf!="")
            {
                var companyid = "";
                var invno = "";
                if (Companydetails != null && Companydetails != "")
                {
                    var comp = db.Companies.Where(m => m.Company_Id == Companydetails).FirstOrDefault();

                    companyid = comp.Company_Id;
                }
                if (invoiceNo != null && invoiceNo != "")
                {
                    invno = db.Invoices.Where(m => m.invoiceno == invoiceNo).Select(m => m.invoiceno).FirstOrDefault();
                    
                }
                DateTime? fdate = !string.IsNullOrEmpty(invfromdate) ? DateTime.ParseExact(invfromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : (DateTime?)null;
                DateTime? tdate = !string.IsNullOrEmpty(invtodate) ? DateTime.ParseExact(invtodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : (DateTime?)null;
 

                list = db.getInvoiceWithapplyFilter( fdate,tdate, companyid, strpf, invoiceNo)
                .Select(x => new InvoiceModel
                {
                    IN_Id= x.IN_Id,
                    invoiceno=x.invoiceno,
                    invoicedate=x.invoicedate,
                    periodfrom=x.periodfrom,
                    periodto=x.periodto,
                    total=x.total,
                    fullsurchargetax=x.fullsurchargetax,
                    fullsurchargetaxtotal=x.fullsurchargetaxtotal,
                    servicetax=x.servicetax,
                    servicetaxtotal=x.servicetaxtotal,
                    othercharge=x.othercharge,
                    netamount=x.netamount,
                    Customer_Id=x.Customer_Id,
                    paid=x.paid,
                    discount=x.discount,
                    Royalty_charges=x.Royalty_charges,
                    Docket_charges=x.Docket_charges,
                    Tempdatefrom=x.Tempdatefrom,
                    TempdateTo=x.TempdateTo,
                    tempInvoicedate=x.tempInvoicedate,
                    Address=x.Address,
                    Invoice_Lable=x.Invoice_Lable,
                    Firm_Id=x.Firm_Id,
                    totalCount=x.totalCount??0
                }).OrderBy(x => x.invoicedate).ToList();
                return View(list);
            }
            //if (Companydetails != "" && Companydetails != null)
            //{
            //    var comp = db.Companies.Where(m => m.Company_Id == Companydetails).FirstOrDefault();
            //    ViewBag.Companyid = comp.Company_Id;



            //    list = db.getInvoice(DateTime.Parse(fromdate), DateTime.Parse(todate), comp.Company_Id, strpf).Select(x => new InvoiceModel
            //    {

            //        IN_Id = x.IN_Id,
            //        invoiceno = x.invoiceno,
            //        invoicedate = x.invoicedate,
            //        periodfrom = x.periodfrom,
            //        periodto = x.periodto,
            //        total = x.total,
            //        fullsurchargetax = x.fullsurchargetax,
            //        fullsurchargetaxtotal = x.fullsurchargetaxtotal,
            //        servicetax = x.servicetax,
            //        servicetaxtotal = x.servicetaxtotal,
            //        othercharge = x.othercharge,
            //        netamount = x.netamount,
            //        Customer_Id = x.Customer_Id,
            //        paid = x.paid,
            //        discount = x.discount,
            //        Royalty_charges = x.Royalty_charges,
            //        Docket_charges = x.Docket_charges,
            //        Tempdatefrom = x.Tempdatefrom,
            //        TempdateTo = x.TempdateTo,
            //        tempInvoicedate = x.tempInvoicedate,
            //        Address = x.Address,
            //        Invoice_Lable = x.Invoice_Lable,
            //        Firm_Id = x.Firm_Id,
            //        totalCount = x.totalCount ?? 0
            //    }).ToList();
            //    return View(list);
            //}
            //else
            //{
            //    list = db.getInvoiceWithoutCompany(DateTime.Parse(fromdate), DateTime.Parse(todate), strpf).Select(x => new InvoiceModel
            //    {

            //        IN_Id = x.IN_Id,
            //        invoiceno = x.invoiceno,
            //        invoicedate = x.invoicedate,
            //        periodfrom = x.periodfrom,
            //        periodto = x.periodto,
            //        total = x.total,
            //        fullsurchargetax = x.fullsurchargetax,
            //        fullsurchargetaxtotal = x.fullsurchargetaxtotal,
            //        servicetax = x.servicetax,
            //        servicetaxtotal = x.servicetaxtotal,
            //        othercharge = x.othercharge,
            //        netamount = x.netamount,
            //        Customer_Id = x.Customer_Id,
            //        paid = x.paid,
            //        discount = x.discount,
            //        Royalty_charges = x.Royalty_charges,
            //        Docket_charges = x.Docket_charges,
            //        Tempdatefrom = x.Tempdatefrom,
            //        TempdateTo = x.TempdateTo,
            //        tempInvoicedate = x.tempInvoicedate,
            //        Address = x.Address,
            //        Invoice_Lable = x.Invoice_Lable,
            //        Firm_Id = x.Firm_Id,
            //        totalCount = x.totalCount ?? 0
            //    }).ToList();

            //    return View(list);
            //}

            return View(list);
        }

        public ActionResult ViewDPInvoice()
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            return View(db.Invoices.Where(m => (((m.Total_Lable != null || m.Total_Lable.Length > 0) && m.Pfcode == strpf))).ToList());
        }

        [HttpGet]
        public ActionResult ViewSingleInvoice(string invoiceNo, bool isDelete = false)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            if (isDelete)
            {
                var checkInvoiceNo = db.Invoices.Where(x => x.invoiceno == invoiceNo && x.Pfcode == strpf).FirstOrDefault();
                if (checkInvoiceNo == null)
                {
                    TempData["error"] = "Invalid Invoice No";

                }

                db.Invoices.Remove(checkInvoiceNo);
                db.SaveChanges();
                TempData["success"] = "Delete successfully";
            }

            var temp = db.singleinvoiceconsignments.Select(m => m.Invoice_no).ToList();



            var a = (from member in db.Invoices
                     where temp.Contains(member.invoiceno) && member.Pfcode == strpf
                     select member).ToList();



            return View(a);

        }

        public JsonResult InvoiceTable(string CustomerId, string Tempdatefrom, string TempdateTo)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();



            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);




            db.Configuration.ProxyCreationEnabled = false;

            var Companies = db.TransactionViews.Where(m => m.Customer_Id == CustomerId && m.Pf_Code == strpfcode && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                              .ToList();





            return Json(Companies, JsonRequestBehavior.AllowGet);

        }

        public JsonResult InvoiceDetails(string CustomerId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Companies = db.Companies.Where(m => m.Company_Id == CustomerId).FirstOrDefault();


            return Json(Companies, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CustomerIdAutocompleteForViewInvocie()
        {

            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var entity = db.Companies.Where(m => m.Pf_code == strpf).
Select(e => new
{
    e.Company_Id
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InvoiceNumberAutocompleteForViewInvocie(string Customer_Id)
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
           var  result = db.Invoices.Where(m => m.Pfcode == pfcode && (m.invoiceno != null || m.invoiceno != "")).
                Select(m => new { m.invoiceno }).OrderBy(m => m.invoiceno).Distinct().ToList();
            if (Customer_Id != null && Customer_Id!="")
            {
                 result = db.Invoices.Where(m => m.Pfcode == pfcode && (m.invoiceno != null || m.invoiceno != "") && m.Customer_Id==Customer_Id).
                Select(m => new { m.invoiceno }).OrderBy(m => m.invoiceno).Distinct().ToList();
            }
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveInvoice(InvoiceModel invoice, string submit)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

            string invstart1 = dataInvStart + "/2023-24/";

            if (invoice.discount == "yes")
            {
                ViewBag.disc = invoice.discount;
            }


            if (ModelState.IsValid)
            {

                string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


                Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno && m.Pfcode == strpfcode).FirstOrDefault();


                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    double netAmt = Convert.ToDouble(inv.netamount);

                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);


                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, strpfcode);


                    Invoice invo = new Invoice();


                    invo.IN_Id = inv.IN_Id;
                    invo.invoiceno = invoice.invoiceno;
                    invo.total = invoice.total;
                    invo.fullsurchargetax = invoice.fullsurchargetax;
                    invo.fullsurchargetaxtotal = invoice.fullsurchargetaxtotal;
                    invo.servicetax = invoice.servicetax;
                    invo.servicetaxtotal = invoice.servicetaxtotal;
                    invo.othercharge = invoice.othercharge;
                    invo.netamount = invoice.netamount;
                    invo.Customer_Id = invoice.Customer_Id;
                    invo.fid = invoice.fid;
                    invo.annyear = invoice.annyear;
                    invo.paid = invoice.paid;
                    invo.status = invoice.status;
                    invo.discount = invoice.discount;
                    invo.discountper = invoice.discountper;
                    invo.discountamount = invoice.discountamount;
                    invo.servicecharges = invoice.servicecharges;
                    invo.Royalty_charges = invoice.Royalty_charges;
                    invo.Docket_charges = invoice.Docket_charges;
                    invo.Tempdatefrom = invoice.Tempdatefrom;
                    invo.TempdateTo = invoice.TempdateTo;
                    invo.tempInvoicedate = invoice.tempInvoicedate;
                    invo.Address = invoice.Address;
                    invo.Total_Lable = invoice.Total_Lable;
                    invo.Royalti_Lable = invoice.Royalti_Lable;
                    invo.Docket_Lable = invoice.Docket_Lable;
                    invo.Amount4 = invoice.Amount4;
                    invo.Amount4_Lable = invoice.Amount4_Lable;

                    invo.periodfrom = Convert.ToDateTime(bdatefrom);
                    invo.periodto = Convert.ToDateTime(bdateto);
                    invo.invoicedate = Convert.ToDateTime(invdate);
                    invo.Pfcode = strpfcode;

                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invo).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Updated SuccessFully";

                    /////////////////// update consignment///////////////////////
                    using (var db = new db_a92afa_frbillingEntities())
                    {
                        var Companies = db.Transactions.Where(m => m.status_t == invoice.invoiceno).ToList();

                        Companies.ForEach(m => m.status_t = "0");
                        db.SaveChanges();


                        Companies = db.Transactions.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                     Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

                        Companies.ForEach(m => m.status_t = invoice.invoiceno);
                        db.SaveChanges();
                    }
                    ///////////////////end of update consignment///////////////////////
                }
                else
                {

                    var invoi = db.Invoices.Where(m => m.tempInvoicedate == invoice.tempInvoicedate && m.Customer_Id == invoice.Customer_Id && m.Pfcode == invoice.Pfcode).FirstOrDefault();

                    if (invoi != null)
                    {
                        ModelState.AddModelError("invoi", "Invoice is already Generated");
                    }

                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);



                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart1, strpfcode);

                    invoice.invoiceno = invoice.invoiceno;

                    Invoice invo = new Invoice();

                    invo.invoiceno = invoice.invoiceno;
                    invo.total = invoice.total;
                    invo.fullsurchargetax = invoice.fullsurchargetax;
                    invo.fullsurchargetaxtotal = invoice.fullsurchargetaxtotal;
                    invo.servicetax = invoice.servicetax;
                    invo.servicetaxtotal = invoice.servicetaxtotal;
                    invo.othercharge = invoice.othercharge;
                    invo.netamount = invoice.netamount;
                    invo.Customer_Id = invoice.Customer_Id;
                    invo.fid = invoice.fid;
                    invo.annyear = invoice.annyear;
                    invo.paid = invoice.paid;
                    invo.status = invoice.status;
                    invo.discount = invoice.discount;
                    invo.discountper = invoice.discountper;
                    invo.discountamount = invoice.discountamount;
                    invo.servicecharges = invoice.servicecharges;
                    invo.Royalty_charges = invoice.Royalty_charges;
                    invo.Docket_charges = invoice.Docket_charges;
                    invo.Tempdatefrom = invoice.Tempdatefrom;
                    invo.TempdateTo = invoice.TempdateTo;
                    invo.tempInvoicedate = invoice.tempInvoicedate;
                    invo.Address = invoice.Address;
                    invo.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    invo.Total_Lable = invoice.Total_Lable;
                    invo.Royalti_Lable = invoice.Royalti_Lable;
                    invo.Docket_Lable = invoice.Docket_Lable;
                    invo.Amount4 = invoice.Amount4;
                    invo.Amount4_Lable = invoice.Amount4_Lable;

                    invo.periodfrom = Convert.ToDateTime(bdatefrom);
                    invo.periodto = Convert.ToDateTime(bdateto);
                    invo.invoicedate = Convert.ToDateTime(invdate);
                    invo.Pfcode = strpfcode;



                    db.Invoices.Add(invo);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";

                
                    /////////////////// update consignment///////////////////////
                    using (var db = new db_a92afa_frbillingEntities())
                    {
                        var Companies = db.Transactions.Where(m => m.status_t == invoice.invoiceno).ToList();

                        Companies.ForEach(m => m.status_t = "0");
                        db.SaveChanges();


                        Companies = db.Transactions.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                     Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

                        Companies.ForEach(m => m.status_t = invoice.invoiceno);
                        db.SaveChanges();
                    }
                    ///////////////////end of update consignment///////////////////////

                }

                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                LocalReport lr = new LocalReport();


                var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
             Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                           .ToList();

                var franchisee = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                franchisee.FirstOrDefault().LogoFilePath = (franchisee.FirstOrDefault().LogoFilePath == null || franchisee.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : franchisee.FirstOrDefault().LogoFilePath;

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = franchisee.FirstOrDefault().GstNo;

                franchisee.FirstOrDefault().LogoFilePath = (franchisee.FirstOrDefault().LogoFilePath == null || franchisee.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : franchisee.FirstOrDefault().LogoFilePath;
                string discount = dataset3.FirstOrDefault().discount;
                if (discount == "no")
                {
                    if (clientGst != null && clientGst.Length > 4)
                    {
                        if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }

                        }
                        else
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }

                else if (discount == "yes")
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountPrint.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }


                lr.EnableExternalImages = true;
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisee", franchisee);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);



                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
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


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );

                ViewBag.pdf = false;

                if (submit == "Generate")
                {
                    ViewBag.pdf = true;
                    ViewBag.invoiceno = invoice.invoiceno.Replace("/","-");
                }
                string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");
                ViewBag.savePath = savePath;
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }

                if (submit == "Email")
                {
                    string emailBody = $@"
                        <html>
                        <head>
                            <title>Your Invoice has been Generated</title>
                            <style>
                                body {{
                                    font-family: Arial, sans-serif;
                                    margin: 0;
                                    padding: 0;
                                    background-color: #F5E8E8;
                                }}

                                .container {{
                                    max-width: 600px;
                                    margin: 0 auto;
                                    padding: 20px;
                                    background-color: #FFFFFF;
                                }}

                                h2 {{
                                    color: #333333;
                                }}

                                p {{
                                    color: #555555;
                                }}

                                table {{
                                    width: 100%;
                                }}

                                th, td {{
                                    padding: 10px;
                                    text-align: left;
                                    vertical-align: top;
                                    border-bottom: 1px solid #dddddd;
                                }}

                                .logo {{
                                    text-align: center;
                                    margin-bottom: 20px;
                                }}

                                .logo img {{
                                    max-width: 200px;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='logo'>
                                    <img src='https://frbilling.com/assets/Home/assets/images/logo.png' alt='Logo'>
                                </div>
                                <h4>Your Invoice has been Generated</h4>
                                <h3><strong>Dear Customer,</strong></h3>
                                <p>We are pleased to inform you that your invoice has been successfully generated.</p>
                                <p>Please find the details below:</p>
                                <!-- Include invoice details as a table or any other format you prefer -->

                                <hr>
                                <p>If you have any questions or concerns regarding your invoice, please contact our support team.<br />
                                    <strong> at +91 9209764995</strong></p>

                                <p>Thank you for choosing Fr-Billing.</p>
                                <p>Best regards,</p>
                                <p><strong>Fr-Billing</strong></p>
                            </div>
                        </body>
                        </html>
                        ";

                    var path = "https://www.frbilling.com/PDF/" + dataset3.FirstOrDefault().Firm_Id + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";
                    //Set up the email model
                    SendModel emailModel = new SendModel
                    {

                        toEmail = dataset4.FirstOrDefault().Email,
                        subject = "Invoice",
                        body = emailBody,
                        filepath = savePath
                    };

                    // Send the email using your email sending logic





                    SendEmailModel sm = new SendEmailModel();
                    var mailMessage = sm.MailSend(emailModel);



                    //    MemoryStream memoryStream = new MemoryStream(renderByte);



                    //    using (MailMessage mm = new MailMessage("billingdtdc48@gmail.com", dataset4.FirstOrDefault().Email))
                    //    {







                    //        mm.Subject = "Invoice";

                    //        string Bodytext = "<html><body>Please Find Attachment</body></html>";
                    //        Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                    //        mm.IsBodyHtml = true;



                    //        mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                    //        // Add plain text view
                    //        AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                    //        mm.AlternateViews.Add(plainView);

                    //        // Add HTML view
                    //        AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(Bodytext, null, "text/html");
                    //        mm.AlternateViews.Add(htmlView);

                    //        // Add Byte array as Attachment.
                    //        mm.Attachments.Add(attachment);
                    //        SmtpClient smtp = new SmtpClient();
                    //        smtp.Host = "smtp.gmail.com";
                    //        smtp.EnableSsl = true;
                    //        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                    //        credentials.UserName = "frbillingsoftware@gmail.com";
                    //        credentials.Password = "dtdcmf1339";
                    //        smtp.UseDefaultCredentials = true;
                    //        NetworkCredential _network = new NetworkCredential("frbillingsoftware@gmail.com", "rqaynjbevkygswkx");
                    //        smtp.Credentials = _network;

                    //        smtp.Credentials = credentials;
                    //        smtp.Port = 587;
                    //        smtp.Send(mm);
                    //    }

                }






                return PartialView("GenerateInvoicePartial", invoice);

            }
            return PartialView("GenerateInvoicePartial", invoice);
        }


        public ActionResult Download(long id)
        {

            var invoice = db.Invoices.Where(m => m.IN_Id == id).FirstOrDefault();

            string companyname = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Company_Id).FirstOrDefault().ToString();

            string savePath = "https://frbilling.com/PDF/" + invoice.invoiceno.Replace("/", "-") + ".pdf";

            return Redirect(savePath);

        }

        [HttpPost]
        public ActionResult SaveDpInvoice(Invoice invoice, string submit)
        {


            if (invoice.Total_Lable == null)
            {
                ModelState.AddModelError("Total_Lable", "Label Required");
            }


            var firm = db.FirmDetails.Where(m => m.Firm_Id == invoice.Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;



            if (ModelState.IsValid)
            {

                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno && m.Firm_Id == invoice.Firm_Id).FirstOrDefault();


                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, invoice.Pfcode);


                    invoice.IN_Id = inv.IN_Id;

                    invoice.invoiceno = invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.discountper = 0;
                    invoice.discountamount = 0;
                    invoice.discount = "no";
                    invoice.othercharge = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Updated SuccessFully";
                }
                else
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);




                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, invoice.Pfcode);

                    invoice.invoiceno = invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";

                }

                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                LocalReport lr = new LocalReport();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id)
                              .ToList().
                              Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                         .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno);

                var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }

                lr.EnableExternalImages = true;

                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
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

                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );


                //if (submit == "Generate")
                //{
                ViewBag.pdf = true;
                ViewBag.invoiceno = invoice.invoiceno;
                // }


                if (submit == "Email")
                {

                    MemoryStream memoryStream = new MemoryStream(renderByte);

                    using (MailMessage mm = new MailMessage("Mailid@gmail.com", dataset4.FirstOrDefault().Email))
                    {
                        mm.Subject = "Invoice";

                        string Bodytext = "<html><body>Please Find Attachment</body></html>";
                        Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                        mm.IsBodyHtml = true;



                        mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                        AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                        // mm.Body = Bodytext;
                        mm.Body = Bodytext;

                        //Add Byte array as Attachment.

                        mm.Attachments.Add(attachment);

                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                        credentials.UserName = "Mailid@gmail.com";
                        credentials.Password = "password";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = credentials;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }




                }

                return PartialView("DpInvoicePartial", invoice);

            }
            return PartialView("DpInvoicePartial", invoice);
        }

        [HttpPost]
        public ActionResult SaveInvoiceLastYear(Invoice invoice, string submit)
        {


            if (invoice.Total_Lable == null)
            {
                ModelState.AddModelError("Total_Lable", "Label Required");
            }

            var firm = db.FirmDetails.Where(m => m.Firm_Id == invoice.Firm_Id).FirstOrDefault();

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;

            if (ModelState.IsValid)
            {

                string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

                Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno).FirstOrDefault();


                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);

                    invoice.IN_Id = inv.IN_Id;

                    invoice.invoiceno = invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.discountper = 0;
                    invoice.discountamount = 0;
                    invoice.discount = "no";
                    invoice.othercharge = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Added SuccessFully";

                    ViewBag.nextinvoice = GetmaxInvoiceno("INV/17-18/", invoice.Pfcode);
                }
                else
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);

                    invoice.invoiceno = invoice.invoiceno;

                    invoice.fullsurchargetaxtotal = 0;
                    invoice.fullsurchargetax = 0;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";
                    ViewBag.nextinvoice = GetmaxInvoiceno("INV/17-18/", invoice.Pfcode);
                }

                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                LocalReport lr = new LocalReport();

                //if (submit == "Generate")
                //{
                ViewBag.pdf = true;
                ViewBag.invoiceno = invoice.invoiceno;
                // }

                return PartialView("GenerateInvoiceLastYearPartial", invoice);

            }
            return PartialView("GenerateInvoiceLastYearPartial", invoice);
        }

        [HttpGet]
        public ActionResult ReportPrinterMethod(string myParameter, long firmid)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();


                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
          Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                        .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id == firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }





                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);


                lr.EnableExternalImages = true;

                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
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
            }

        }

        [HttpGet]
        public ActionResult DpReportPrinterMethod(string myParameter, long firmid)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id)
                           .ToList().
                           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0)
                      .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id == firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }





                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);


                lr.EnableExternalImages = true;


                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
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
            }

        }

        public ActionResult MultipleInvoice()
        {

            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            ViewBag.Complist = db.Companies.Where(m => !(m.Company_Id.StartsWith("Cash_")) && !(m.Company_Id.StartsWith("BASIC_TS")) && m.Pf_code == strpfcode).Select(m => m.Company_Id).ToList();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MultipleInvoice(string[] Companies, Invoice invoice, string submit)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            ViewBag.Complist = db.Companies.Where(m => !(m.Company_Id.StartsWith("Cash_")) && !(m.Company_Id.StartsWith("BASIC_TS")) && m.Pf_code == strpfcode).Select(m => m.Company_Id).ToList();


            if (ModelState.IsValid)
            {


                Task.Run(() => MultipleInvoiceAsyncMethod(Companies, invoice, submit));

                ViewBag.Success = "All Invoices Generated SuccessFully";
            }


            return View();
        }

        public void MultipleInvoiceAsyncMethod(string[] Companies, Invoice invoice, string submit)
        {
            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string invoicedate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);
            DateTime invdate = Convert.ToDateTime(invoicedate);

            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            foreach (var i in Companies)
            {
                var invoi = db.Invoices.Where(m => m.tempInvoicedate == invoice.tempInvoicedate && m.Pfcode == strpfcode && m.Customer_Id == i).FirstOrDefault();

                if (invoi == null)
                {
                    Company cm = db.Companies.Where(m => m.Company_Id == i && m.Pf_code == strpfcode).FirstOrDefault();

                    var TrList = db.TransactionViews.Where(m => m.Customer_Id == i).ToList().
                   Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0)
                                     .ToList();


                    Invoice inv = new Invoice();



                    double? AmountTotal = TrList.Sum(m => m.Amount ?? 0);

                    double? RisksurchargeTotal = TrList.Sum(m => m.Risksurcharge ?? 0);

                    double? OtherchargeTotal = TrList.Sum(m => m.loadingcharge ?? 0);

                    inv.total = AmountTotal + RisksurchargeTotal + OtherchargeTotal;

                    inv.fullsurchargetax = cm.Fuel_Sur_Charge ?? 0;

                    inv.periodfrom = fromdate;
                    inv.servicetax = invoice.servicetax;
                    inv.periodto = todate;
                    inv.invoicedate = invdate;
                    inv.Tempdatefrom = invoice.Tempdatefrom;
                    inv.TempdateTo = invoice.TempdateTo;
                    inv.tempInvoicedate = invoice.tempInvoicedate;
                    inv.Address = db.Companies.Where(m => m.Company_Id == i).Select(m => m.Company_Address).FirstOrDefault();
                    inv.Customer_Id = i;
                    inv.Pfcode = strpfcode;
                    inv.fullsurchargetaxtotal = ((inv.total * Convert.ToDouble(cm.Fuel_Sur_Charge)) / 100);

                    string invoiceno = "0";


                    var dataInvStart = (from d in db.Franchisees
                                        where d.PF_Code == strpfcode
                                        select d.InvoiceStart).FirstOrDefault();

                    string invstart1 = dataInvStart + "/2023-24/";
                    //string invstart1 = "IJS/2022-23/";
                    string no = "";
                    string finalstring = "";

                    string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                    string lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "00";

                    if (lastInvoiceno == null)
                    {
                        int number = Convert.ToInt32(lastInvoiceno.Substring(12));
                        no = lastInvoiceno.Substring(12);
                        ViewBag.lastInvoiceno = invstart1 + "" + (no + 1);
                    }

                    else
                    {

                        string[] strarrinvno = lastInvoiceno1.Split('/');
                        //string val = lastInvoiceno.Substring(19, lastInvoiceno.Length - 19);
                        int newnumber = Convert.ToInt32(strarrinvno[2]) + 1;
                        finalstring = newnumber.ToString("000");
                        ViewBag.lastInvoiceno = invstart1 + "" + finalstring;
                    }

                    inv.invoiceno = lastInvoiceno;

                    inv.Firm_Id = invoice.Firm_Id;
                    inv.discount = "no";


                    inv.Docket_charges = 0;

                    foreach (var j in TrList)
                    {
                        if (j.Consignment_no.ToLower().StartsWith("d"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.D_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("m"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.P_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("e"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.E_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("v"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.V_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("i"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.I_Docket);
                        }
                        else if (j.Consignment_no.ToLower().StartsWith("n"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.N_Docket);
                        }

                        else if (j.Consignment_no.ToLower().StartsWith("g"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.G_Docket);
                        }
                    }

                    inv.Royalty_charges = ((inv.total * Convert.ToDouble(cm.Royalty_Charges)) / 100);

                    inv.servicetaxtotal = (((inv.total + inv.fullsurchargetaxtotal + inv.Docket_charges + inv.Royalty_charges) * invoice.servicetax) / 100); //((gst_total * parseFloat("0" + gst)) / 100);
                    inv.netamount = inv.total + inv.Docket_charges + inv.Royalty_charges + inv.servicetaxtotal + inv.fullsurchargetaxtotal;
                    inv.netamount = Math.Round(inv.netamount ?? 0, 0);
                    inv.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    if (inv.netamount > 0)
                    {
                        db.Invoices.Add(inv);
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


                        /****************For Billed unbilled ******************/
                        using (var db = new db_a92afa_frbillingEntities())
                        {
                            var Companies1 = db.Transactions.Where(m => m.status_t == inv.invoiceno).ToList();

                            Companies1.ForEach(m => m.status_t = "0");
                            db.SaveChanges();


                            Companies1 = db.Transactions.Where(m => m.Customer_Id == inv.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                         Where(x => DateTime.Compare(x.booking_date.Value.Date, inv.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inv.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

                            Companies1.ForEach(m => m.status_t = inv.invoiceno);
                            db.SaveChanges();
                        }

                        /****************For Billed unbilled ******************/
                        //if (submit == "Email")
                        //{
                        SendMailInvoiceMultiple(inv, submit);
                        // }
                    }
                }


                Notification nt = new Notification();

                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                nt.dateN = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                nt.Message = "From Company Id" + Companies.FirstOrDefault() + "to" + Companies.LastOrDefault() + "Invoices Generated SuccessFully";
                nt.Status = false;

                db.Notifications.Add(nt);
                db.SaveChanges();
            }

        }

        public void SendMailInvoiceMultiple(Invoice invoice, string submit)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            LocalReport lr = new LocalReport();

            string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

            //var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no))
            //                 .ToList().
            //                 Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
            //            .ToList();


            var dataset = db.TransactionViews.Where(m => m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                          .ToList();


            var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
            dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

            var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno && m.Pfcode == strpfcode);

            var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

            string clientGst = dataset4.FirstOrDefault().Gst_No;
            string frgst = dataset2.FirstOrDefault().GstNo;


            if (clientGst != null && clientGst.Length > 4)
            {
                if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }

                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }
            }
            else
            {
                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
            }




            ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
            ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
            ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
            ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

            lr.EnableExternalImages = true;

            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);
            lr.DataSources.Add(rd3);

            string reportType = "pdf";
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


            renderByte = lr.Render
          (reportType,
          deviceInfo,
          out mimeType,
          out encoding,
          out fileNameExte,
          out streams,
          out warnings
          );

            if (submit == "Email")
            {
                MemoryStream memoryStream = new MemoryStream(renderByte);

                using (MailMessage mm = new MailMessage("Mailid@gmail.com", dataset4.FirstOrDefault().Email))
                {
                    mm.Subject = "Invoice";

                    string Bodytext = "<html><body>Please Find Attachment</body></html>";
                    Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                    mm.IsBodyHtml = true;



                    mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                    AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                    // mm.Body = Bodytext;
                    mm.Body = Bodytext;

                    //Add Byte array as Attachment.

                    mm.Attachments.Add(attachment);

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                    credentials.UserName = "Mailid@gmail.com";
                    credentials.Password = "password";
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = credentials;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }


            }


            string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id + "-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                stream.Write(renderByte, 0, renderByte.Length);
            }



        }

        [HttpGet]
        public string SavepdInvoice(string myParameter)
        {
            {
                string Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString(); ;

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Pfcode == Pf_Code).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();


                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
            Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                          .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Pfcode == Pf_Code);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;
                string discount = dataset3.FirstOrDefault().discount;

                if (discount == "no")
                {
                    if (clientGst != null && clientGst.Length > 4)
                    {
                        if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }

                        }
                        else
                        {
                            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                            if (System.IO.File.Exists(path))
                            {
                                lr.ReportPath = path;
                            }
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }

                }

                else if (discount == "yes")
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DiscountPrint.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }


                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisee", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);


                lr.EnableExternalImages = true;

                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                lr.EnableExternalImages = true;

                string reportType = "pdf";
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


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );



                string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }


                if (!string.IsNullOrEmpty(savePath))
                {
                    // Redirect to a new action that will open the PDF in a new tab
                    var get =  RedirectToAction("OpenPdfInNewTab", new { savePath });
                }

                return dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

            }

        }

        [HttpGet]
        public string SavepdDpInvoice(string myParameter, long firmid)
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id)
                           .ToList().
                           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0)
                      .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id == firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "DpPrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }





                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);

                lr.EnableExternalImages = true;

                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
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


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );



                string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id + "-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }

                return dataset3.FirstOrDefault().Firm_Id + "-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

            }

        }

        [HttpGet]
        public ActionResult InvoiceZip()
        {

            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

            string invstart1 = dataInvStart + "/2023-24/";

            ViewBag.Zipinv = invstart1;

            return View();
        }

        [HttpPost]
        public ActionResult InvoiceZip(int frominv, int toinv)
        {


            string fileType = "application/octet-stream";



            var outputStream = new MemoryStream();


            using (ZipFile zipFile = new ZipFile())
            {



                for (int i = frominv; i <= toinv; i++)
                {
                    string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

                    var dataInvStart = (from d in db.Franchisees
                                        where d.PF_Code == strpfcode
                                        select d.InvoiceStart).FirstOrDefault();

                    string invstart1 = dataInvStart + "/2023-24/";

                    string filePath = Server.MapPath("/PDF/" + invstart1.Replace("/","-")+ i + ".pdf");

                    if (System.IO.File.Exists(filePath))
                    {
                        zipFile.AddFile(filePath, "Invoices");
                    }


                }

                Response.ClearContent();
                Response.ClearHeaders();

                //Set zip file name
                Response.AppendHeader("content-disposition", "attachment; filename=Invoices.zip");

                //Save the zip content in output stream
                zipFile.Save(outputStream);
            }

            //Set the cursor to start position
            outputStream.Position = 0;

            //Dispance the stream
            return new FileStreamResult(outputStream, fileType);
        }

        public ActionResult GenerateInvoiceSingle(string Invoiceno = null)
        {


            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var franchisee = db.Franchisees.Where(x => x.PF_Code == strpfcode).FirstOrDefault();

            var dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

            string invstart1 = dataInvStart + "/2023-24/";
            if (strpfcode == "PF2214")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";


            }
            //string invstart1 = "IJS/2022-23/";
            string no = "";
            string finalstring = "";




            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
            string lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "00";

            if (lastInvoiceno == null)
            {
                //string[] strarrinvno = lastInvoiceno1.Split('/');

                //ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);
                string[] strarrinvno = lastInvoiceno1.Split('/');
                if (franchisee.PF_Code == "PF2214")
                {
                    strarrinvno = lastInvoiceno1.Split('/');
                    ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[3] + 1);

                }
                else if (franchisee.PF_Code == "PF975")
                {
                    ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);
                    if (strarrinvno[2] == "00")
                    {
                        strarrinvno[2] = "597";
                        ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2]);

                    }




                }
                else
                {
                    ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);

                }
            }

            else
            {

                //string[] strarrinvno = lastInvoiceno1.Split('/');
                ////string val = lastInvoiceno.Substring(19, lastInvoiceno.Length - 19);
                //int newnumber = Convert.ToInt32(strarrinvno[2]) + 1;
                //finalstring = newnumber.ToString("000");
                //ViewBag.lastInvoiceno = invstart1 + "" + finalstring;
                string[] strarrinvno = lastInvoiceno1.Split('/');
                //string val = lastInvoiceno.Substring(19, lastInvoiceno.Length - 19);
                int newnumber = 0;
                if (franchisee.PF_Code == "PF2214")
                {
                    newnumber = Convert.ToInt32(strarrinvno[3]) + 1;
                }

                else
                {
                    newnumber = Convert.ToInt32(strarrinvno[2]) + 1;
                }

                finalstring = newnumber.ToString("000");
                ViewBag.lastInvoiceno = invstart1 + "" + finalstring;
            }



            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno && m.Pfcode == strpfcode).FirstOrDefault();



            if (Invoiceno != null)
            {
                ViewBag.consignmnts = string.Join(",", db.singleinvoiceconsignments.Where(m => m.Invoice_no == Invoiceno).Select(m => m.Consignment_no).ToArray());
            }


            var data = (from d in db.Invoices
                        where d.Pfcode == strpfcode
                        && d.invoiceno == Invoiceno
                        select d).FirstOrDefault();

            if (data != null)
            {
                InvoiceModel Inv = new InvoiceModel();


                Inv.invoiceno = data.invoiceno;
                Inv.invoicedate = data.invoicedate;
                Inv.periodfrom = data.periodfrom;
                Inv.periodto = data.periodto;
                Inv.total = data.total;
                Inv.fullsurchargetax = data.fullsurchargetax;
                Inv.fullsurchargetaxtotal = data.fullsurchargetaxtotal;
                Inv.servicetax = data.servicetax;
                Inv.servicetaxtotal = data.servicetaxtotal;
                Inv.othercharge = data.othercharge;
                Inv.netamount = data.netamount;
                Inv.Customer_Id = data.Customer_Id;
                Inv.fid = data.fid;
                Inv.annyear = data.annyear;
                Inv.paid = data.paid;
                Inv.status = data.status;
                Inv.discount = data.discount;
                Inv.discountper = data.discountper;
                Inv.discountamount = data.discountamount;
                Inv.servicecharges = data.servicecharges;
                Inv.Royalty_charges = data.Royalty_charges;
                Inv.Docket_charges = data.Docket_charges;
                Inv.Tempdatefrom = data.Tempdatefrom;
                Inv.TempdateTo = data.TempdateTo;
                Inv.tempInvoicedate = data.tempInvoicedate;
                Inv.Address = data.Address;
                Inv.Invoice_Lable = data.Invoice_Lable;
                Inv.Total_Lable = data.Total_Lable;
                Inv.Royalti_Lable = data.Royalti_Lable;
                Inv.Docket_Lable = data.Docket_Lable;
                Inv.Amount4 = data.Amount4;
                Inv.Amount4_Lable = data.Amount4_Lable;
                Inv.Pfcode = data.Pfcode;

                return View(Inv);
            }

            return View();


        }

        [HttpPost]
        public ActionResult SaveSingleInvoice(InvoiceModel invoice, string submit, string consignments)
        {

            ViewBag.consignmnts = consignments;

            if (invoice.discount == "yes")
            {
                ViewBag.disc = invoice.discount;
            }

            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            if (ModelState.IsValid)
            {

                string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


                Invoice inv = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno && m.Pfcode == strpfcode).FirstOrDefault();



                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    double netAmt = Convert.ToDouble(inv.netamount);

                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);


                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, strpfcode);


                    Invoice invo = new Invoice();


                    invo.IN_Id = inv.IN_Id;
                    invo.invoiceno = invoice.invoiceno;
                    invo.total = invoice.total;
                    invo.fullsurchargetax = invoice.fullsurchargetax;
                    invo.fullsurchargetaxtotal = invoice.fullsurchargetaxtotal;
                    invo.servicetax = invoice.servicetax;
                    invo.servicetaxtotal = invoice.servicetaxtotal;
                    invo.othercharge = invoice.othercharge;
                    invo.netamount = invoice.netamount;
                    invo.Customer_Id = invoice.Customer_Id;
                    invo.fid = invoice.fid;
                    invo.annyear = invoice.annyear;
                    invo.paid = invoice.paid;
                    invo.status = invoice.status;
                    invo.discount = invoice.discount;
                    invo.discountper = invoice.discountper;
                    invo.discountamount = invoice.discountamount;
                    invo.servicecharges = invoice.servicecharges;
                    invo.Royalty_charges = invoice.Royalty_charges;
                    invo.Docket_charges = invoice.Docket_charges;
                    invo.Tempdatefrom = invoice.Tempdatefrom;
                    invo.TempdateTo = invoice.TempdateTo;
                    invo.tempInvoicedate = invoice.tempInvoicedate;
                    invo.Address = invoice.Address;
                    invo.Total_Lable = invoice.Total_Lable;
                    invo.Royalti_Lable = invoice.Royalti_Lable;
                    invo.Docket_Lable = invoice.Docket_Lable;
                    invo.Amount4 = invoice.Amount4;
                    invo.Amount4_Lable = invoice.Amount4_Lable;

                    invo.periodfrom = Convert.ToDateTime(bdatefrom);
                    invo.periodto = Convert.ToDateTime(bdateto);
                    invo.invoicedate = Convert.ToDateTime(invdate);
                    invo.Pfcode = strpfcode;
                    invo.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());

                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invo).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Updated SuccessFully";
                }
                else
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    double netAmt = Convert.ToDouble(invoice.netamount);

                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);



                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, invoice.Pfcode);

                    invoice.invoiceno = invoice.invoiceno;


                    Invoice invo = new Invoice();

                    invo.invoiceno = invoice.invoiceno;
                    invo.total = invoice.total;
                    invo.fullsurchargetax = invoice.fullsurchargetax;
                    invo.fullsurchargetaxtotal = invoice.fullsurchargetaxtotal;
                    invo.servicetax = invoice.servicetax;
                    invo.servicetaxtotal = invoice.servicetaxtotal;
                    invo.othercharge = invoice.othercharge;
                    invo.netamount = invoice.netamount;
                    invo.Customer_Id = invoice.Customer_Id;
                    invo.fid = invoice.fid;
                    invo.annyear = invoice.annyear;
                    invo.paid = invoice.paid;
                    invo.status = invoice.status;
                    invo.discount = invoice.discount;
                    invo.discountper = invoice.discountper;
                    invo.discountamount = invoice.discountamount;
                    invo.servicecharges = invoice.servicecharges;
                    invo.Royalty_charges = invoice.Royalty_charges;
                    invo.Docket_charges = invoice.Docket_charges;
                    invo.Tempdatefrom = invoice.Tempdatefrom;
                    invo.TempdateTo = invoice.TempdateTo;
                    invo.tempInvoicedate = invoice.tempInvoicedate;
                    invo.Address = invoice.Address;
                    invo.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    invo.Total_Lable = invoice.Total_Lable;
                    invo.Royalti_Lable = invoice.Royalti_Lable;
                    invo.Docket_Lable = invoice.Docket_Lable;
                    invo.Amount4 = invoice.Amount4;
                    invo.Amount4_Lable = invoice.Amount4_Lable;

                    invo.periodfrom = Convert.ToDateTime(bdatefrom);
                    invo.periodto = Convert.ToDateTime(bdateto);
                    invo.invoicedate = Convert.ToDateTime(invdate);
                    invo.Pfcode = strpfcode;


                    db.Invoices.Add(invo);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";

                }




                string[] cons = consignments.Split(',');

                foreach (var i in cons)
                {
                    singleinvoiceconsignment upsc = db.singleinvoiceconsignments.Where(m => m.Consignment_no == i).FirstOrDefault();

                    if (upsc == null)
                    {

                        singleinvoiceconsignment sc = new singleinvoiceconsignment();

                        sc.Consignment_no = i.Trim();
                        sc.Invoice_no = invoice.invoiceno;
                        db.singleinvoiceconsignments.Add(sc);
                        db.SaveChanges();

                    }




                }

                /////////////////// update consignment///////////////////////
                using (var db = new db_a92afa_frbillingEntities())
                {


                    List<string> Companies = db.singleinvoiceconsignments.Where(m => m.Invoice_no == invoice.invoiceno).Select(m => m.Consignment_no).ToList();
                    var transaction = db.Transactions.Where(m => Companies.Contains(m.Consignment_no)).ToList();

                    transaction.ForEach(m => m.status_t = invoice.invoiceno);
                    db.SaveChanges();
                }
                ///////////////////end of update consignment///////////////////////

                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                LocalReport lr = new LocalReport();




                List<TransactionView> dataset = new List<TransactionView>();

                var consigmfromsingle = db.singleinvoiceconsignments.Where(m => m.Invoice_no == invoice.invoiceno);




                foreach (var c in consigmfromsingle)
                {
                    TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no).FirstOrDefault();
                    dataset.Add(temp);
                }

                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno && m.Pfcode == strpfcode);

                var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }




                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisee", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);


                lr.EnableExternalImages = true;
                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
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


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );



                ViewBag.pdf = true;
                ViewBag.invoiceno = invoice.invoiceno;
                string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }

                if (submit == "Email")
                {

                    MemoryStream memoryStream = new MemoryStream(renderByte);
                    using (MailMessage mm = new MailMessage("billingdtdc48@gmail.com", dataset4.FirstOrDefault().Email))
                    {
                        mm.Subject = "Invoice";

                        string Bodytext = "<html><body>Please Find Attachment</body></html>";
                        Attachment attachment = new Attachment(memoryStream, "Invoice.pdf");

                        mm.IsBodyHtml = true;



                        mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                        AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(Bodytext, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                        // mm.Body = Bodytext;
                        mm.Body = Bodytext;

                        //Add Byte array as Attachment.

                        mm.Attachments.Add(attachment);

                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                        credentials.UserName = "billingdtdc48@gmail.com";
                        credentials.Password = "dtdcmf1339";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = credentials;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }

                }






                return PartialView("GenerateInvoiceSinglePartial", invoice);

            }


            return PartialView("GenerateInvoiceSinglePartial", invoice);
        }


        public JsonResult InvoiceTableSingle(string[] array, string Customerid)
        {

            List<Transaction> Companies = new List<Transaction>();

            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            db.Configuration.ProxyCreationEnabled = false;
            if (array != null)
            {
                foreach (var i in array.Distinct().ToArray())
                {

                    Transaction tr = db.Transactions.Where(m => m.Consignment_no == i.Trim() && m.Pf_Code == strpfcode && m.Customer_Id == Customerid).FirstOrDefault();

                    if (tr != null)
                    {
                        Companies.Add(tr);
                    }

                }
            }


            return Json(Companies, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult ReportsinglePrinterMethod(string myParameter, long firmid) //on view call thise method
        {
            {

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();


                List<TransactionView> dataset = new List<TransactionView>();

                var consigmfromsingle = db.singleinvoiceconsignments.Where(m => m.Invoice_no == myParameter);




                foreach (var c in consigmfromsingle)
                {
                    TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no).FirstOrDefault();
                    dataset.Add(temp);
                }




                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Firm_Id == firmid);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }





                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisees", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);


                lr.EnableExternalImages = true;
                //  lr.SetParameters(new ReportParameter[] { parSum });

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
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
            }

        }

        [HttpGet]
        public string SavesinglepdInvoice(string myParameter)
        {
            {
                string Pf_Code = Request.Cookies["Cookies"]["AdminValue"].ToString();

                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Pfcode == Pf_Code).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();


                List<TransactionView> dataset = new List<TransactionView>();

                var consigmfromsingle = db.singleinvoiceconsignments.Where(m => m.Invoice_no == myParameter);




                foreach (var c in consigmfromsingle)
                {
                    TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no).FirstOrDefault();
                    if (temp != null)

                        dataset.Add(temp);
                }



                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? "https://frbilling.com/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == inc.invoiceno && m.Pfcode == Pf_Code);

                var dataset4 = db.Companies.Where(m => m.Company_Id == inc.Customer_Id);


                /////////////////Total//////////////

                /////////////////Total//////////////

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;


                if (clientGst != null && clientGst.Length > 4)
                {
                    if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }

                    }
                    else
                    {
                        string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoiceIGST.rdlc");

                        if (System.IO.File.Exists(path))
                        {
                            lr.ReportPath = path;
                        }
                    }
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                }





                ////////////////////////////////////
                ReportDataSource rd = new ReportDataSource("PrintInvoice", dataset);
                ReportDataSource rd1 = new ReportDataSource("franchisee", dataset2);
                ReportDataSource rd2 = new ReportDataSource("invoice", dataset3);
                ReportDataSource rd3 = new ReportDataSource("comp", dataset4);

                //  ReportParameter[] allPar = new ReportParameter[1]; // create parameters array
                //  ReportParameter parSum = new ReportParameter("Dcno", dcno);

                lr.EnableExternalImages = true;

                //  lr.SetParameters(new ReportParameter[] { parSum }SavesinglepdInvoice);

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);
                lr.DataSources.Add(rd3);

                string reportType = "pdf";
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


                renderByte = lr.Render
              (reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExte,
              out streams,
              out warnings
              );




                string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    stream.Write(renderByte, 0, renderByte.Length);
                }

                return dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

            }

        }

        public ActionResult GenerateInvoiceLastYear(long Firm_Id = 1, string Invoiceno = null)
        {

            string invstart = "INV/17-18/";


            //string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart)).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
            //int number = Convert.ToInt32(lastInvoiceno.Substring(10));

            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno).FirstOrDefault();

            //ViewBag.lastInvoiceno = number + 1;

            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart) && m.Firm_Id == Firm_Id).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 0;
            int number = Convert.ToInt32(lastInvoiceno.Substring(10));


            ViewBag.lastInvoiceno = invstart + "" + (number + 1);


            var firm = db.FirmDetails.Where(m => m.Firm_Id == Firm_Id).FirstOrDefault();


            ViewBag.Firm_Name = new SelectList(db.FirmDetails, "Firm_Id", "Firm_Name", Firm_Id.ToString());

            ViewBag.firmname = firm.Firm_Name;
            ViewBag.firmid = firm.Firm_Id;

            return View(inv);
        }

        public string GetmaxInvoiceno(string invstart1, string pfcode)
        {

            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == pfcode).Select(m => m.invoiceno).FirstOrDefault();
            //string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id== firmid).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 00;
            string lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == pfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "00";

            // int number = Convert.ToInt32(lastInvoiceno.Substring(12));




            if (lastInvoiceno == null)
            {
                string[] strarrinvno = lastInvoiceno1.Split('/');
                string lastInvoice = invstart1 + "" + (strarrinvno[2] + 1);
                return lastInvoice;
            }

            else
            {

                string[] invno = lastInvoiceno.Split('/');
                int newnumber = Convert.ToInt32(invno[2]) + 1;
                string finalstring = newnumber.ToString("000");
                string lastInvoice = invstart1 + finalstring;
                return lastInvoice;
            }


        }

     
        [HttpGet]
        public string DownloadByInvNo(string invoiceno)
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var invoice = db.Invoices.Where(m => m.invoiceno == invoiceno && m.Pfcode == PfCode).FirstOrDefault();

            string companyname = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Company_Id).FirstOrDefault().ToString();

            var pdffileName = invoice.invoiceno.Replace("/", "-") + ".pdf";
            //https://frbilling.com/PDF/DFRB-2023-24-144.pdf
            string savePath = "https://frbilling.com/PDF/" + pdffileName;

            return savePath;
          
        }

        //[HttpGet]
        //public ActionResult Delete()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public ActionResult Delete(string invoiceNo, string invfromdate, string Companydetails, string invtodate)
        //{
        //    string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
        //    var checkInvoiceNo = db.Invoices.Where(x => x.invoiceno == invoiceNo && x.Pfcode == pfcode).FirstOrDefault();
        //    if (checkInvoiceNo == null)
        //    {
        //        TempData["error"] = "Invalid Invoice No";
        //        return RedirectToAction("ViewInvoice", "Invoice", new { invfromdate = invfromdate, Companydetails = Companydetails, invtodate = invtodate }, "POST");

        //    }

        //    db.Invoices.Remove(checkInvoiceNo);
        //    db.SaveChanges();
        //    TempData["success"] = "Delete successfully";
        //    return RedirectToAction("ViewInvoice", "Invoice", new { invfromdate = invfromdate, Companydetails = Companydetails, invtodate = invtodate });
        //}
    }
}