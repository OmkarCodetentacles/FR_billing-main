using CustomerModel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DtDc_Billing.CustomModel;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Ionic.Zip;
using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI;
using System.Windows.Forms;
using ZXing.QrCode.Internal;
using static System.Net.WebRequestMethods;

namespace DtDc_Billing.Controllers
{
    [SessionAdminold]
    // [SessionUserModule]
    //[OutputCache(CacheProfile = "Cachefast")]
    public class InvoiceController : Controller
    {
        /*https://frbilling.com     currently we have remove the static link done its dynamic by applying the baseUrl property*/
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();


        // string invstart = "INV/2022-23/";
        string invstart = "INV/2023-24/";

        //[OutputCache(Duration = 600, VaryByParam = "none", Location = OutputCacheLocation.Server)]
        public ActionResult GenerateInvoice(string Invoiceno = null)
        {


            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.currentPfcode = strpfcode;

            var franchisee = db.Franchisees.Where(x => x.PF_Code == strpfcode).FirstOrDefault();
            var gst = franchisee.GstNo;
            ViewBag.GST = gst;
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
            if (strpfcode == "PF2214" || strpfcode == "PF934" || strpfcode == "PF1958" || strpfcode == "CF2024" || strpfcode == "PF2213" || strpfcode == "PF2046" || strpfcode == "PF857" || strpfcode == "1")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";


            }
            if (strpfcode == "PF1649")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";
            }

            if (strpfcode == "MF868")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";
            }
            if (strpfcode == "UF2679")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";
            }
            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
            string lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "00";

            if (strpfcode == "CF2024")
            {
                lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? dataInvStart + "/" + "00" + "/2024-25";

            }
            else if (strpfcode == "CF2567")
            {
                dataInvStart = "NGR";
                lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? dataInvStart + " " + 120;

            }
            if (lastInvoiceno == null)
            {
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

                else if (franchisee.PF_Code == "UF2679")
                {
                    ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);
                    if (strarrinvno[2] == "00")
                    {
                        strarrinvno[2] = "10";
                        ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2]);

                    }


                }

                else if (franchisee.PF_Code == "PF2214")
                {
                    strarrinvno = lastInvoiceno1.Split('/');
                    ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[3] + 1);

                }
                else if (franchisee.PF_Code == "CF2024")
                {
                    string incrementedNumber = "00";
                    strarrinvno = lastInvoiceno1.Split('/');
                    int number = int.Parse(strarrinvno[1]) + 1;

                    if (number < 10)
                    {
                        incrementedNumber = number.ToString().PadLeft(2, '0');

                    }
                    else
                    {
                        incrementedNumber = number.ToString();
                    }
                    ViewBag.lastInvoiceno = dataInvStart + "/" + incrementedNumber + "/2024-25";
                }
                else if (franchisee.PF_Code == "CF2567")
                {
                    strarrinvno = lastInvoiceno1.Split(' ');
                    int number = int.Parse(strarrinvno[1]) + 1;


                    ViewBag.lastInvoiceno = dataInvStart + " " + number;
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
                int newnumber = 0;
                string incrementedNumber = "00";
                if (franchisee.PF_Code == "PF2214")
                {
                    newnumber = Convert.ToInt32(strarrinvno[3]) + 1;
                    finalstring = newnumber.ToString("000");
                    ViewBag.lastInvoiceno = invstart1 + "" + finalstring;
                }
                else if (franchisee.PF_Code == "CF2024")
                {
                    newnumber = Convert.ToInt32(int.Parse(strarrinvno[1]) + 1);

                    if (newnumber < 10)
                    {
                        incrementedNumber = newnumber.ToString().PadLeft(2, '0');

                    }
                    else
                    {
                        incrementedNumber = newnumber.ToString();
                    }

                    //string incrementedNumber = newnumber.ToString().PadLeft(2, '0');
                    ViewBag.lastInvoiceno = dataInvStart + "/" + incrementedNumber + "/2024-25";
                }
                else if (franchisee.PF_Code == "CF2567")
                {
                    strarrinvno = lastInvoiceno1.Split(' ');
                    int number = int.Parse(strarrinvno[1]) + 1;


                    ViewBag.lastInvoiceno = dataInvStart + " " + number;
                }
                else
                {
                    newnumber = Convert.ToInt32(strarrinvno[2]) + 1;
                    finalstring = newnumber.ToString("000");
                    ViewBag.lastInvoiceno = invstart1 + "" + finalstring;
                }


            }

            var data = (from d in db.Invoices
                        where d.Pfcode == strpfcode
                        && d.invoiceno == Invoiceno
                        && d.isDelete != true
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
                Inv.servicetax = data.servicetax ?? 0;
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



        public JsonResult CheckComapnyGST(string Customerid)
        {
            var data = db.Companies.Where(x => x.Company_Id == Customerid).FirstOrDefault();
            if (string.IsNullOrEmpty(data.Gst_No) || data.Gst_No == "0")
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }

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
        public ActionResult ViewInvoice(string invfromdate, string Companydetails, string invtodate, string invoiceNo, string invoiceNotoDelete)
        {
            List<InvoiceModel> list = new List<InvoiceModel>();

            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string fromdate = "";

            string todate = "";
            DateTime today = DateTime.Now;

            DateTime yearBack = today.AddYears(-1);


            var monthsInRange = Enumerable.Range(0, 12).Select(i => yearBack.AddMonths(i)).ToList();


            ViewBag.invfromdate = invfromdate;
            ViewBag.invtodate = invtodate;
            ViewBag.invoiceno = invoiceNo;
           
            if ((invfromdate != null && invfromdate != "") && (invtodate != null && invtodate != ""))
            {
                fromdate = DateTime.ParseExact(invfromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy-MM-dd");
                todate = DateTime.ParseExact(invtodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy-MM-dd");
            }
            DateTime? fdate = !string.IsNullOrEmpty(invfromdate) ? DateTime.ParseExact(invfromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : (DateTime?)null;
            DateTime? tdate = !string.IsNullOrEmpty(invtodate) ? DateTime.ParseExact(invtodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : (DateTime?)null;


            var invoices = (from t in db.Invoices
                            where t.Pfcode == pfcode
                                  && t.isDelete == false
                                  && (string.IsNullOrEmpty(fromdate) || SqlFunctions.DatePart("Month", t.invoicedate) == fdate.Value.Month)
                                 && (string.IsNullOrEmpty(todate) || SqlFunctions.DatePart("Month", t.invoicedate) == tdate.Value.Month)
                                 && (string.IsNullOrEmpty(Companydetails) || t.Customer_Id==Companydetails)
                            select t).ToList();

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
                    invno = db.Invoices.Where(m => m.invoiceno == invoiceNo ).Select(m => m.invoiceno).FirstOrDefault();
                    
                }
              

                list = db.getInvoiceWithapplyFilter( fdate,tdate, companyid, strpf, invoiceNo)
                .Select(x => new InvoiceModel
                {
                    IN_Id= x.IN_Id,
                    invoiceno=x.invoiceno,
                    invoicedate=x.invoicedate,
                    periodfrom=x.periodfrom,
                    periodto=x.periodto,
                    total=x.total,
                    fullsurchargetax=x.fullsurchargetax??0,
                    fullsurchargetaxtotal=x.fullsurchargetaxtotal??0,
                    servicetax=x.servicetax??0,
                    servicetaxtotal=x.servicetaxtotal??0,
                    othercharge=x.othercharge??0,
                    netamount=x.netamount,
                    Customer_Id=x.Customer_Id,
                    paid=x.paid,
                    discount=x.discount,
                 discountper=x.discountper??0,
                 discountamount=x.discountamount??0,
                    Royalty_charges=x.Royalty_charges,
                    Docket_charges=x.Docket_charges,
                    Tempdatefrom=x.Tempdatefrom,
                    TempdateTo=x.TempdateTo,
                    tempInvoicedate=x.tempInvoicedate,
                    Address=x.Address,
                    Invoice_Lable=x.Invoice_Lable,
                    Firm_Id=x.Firm_Id,
                    totalCount=x.totalCount??0,
                    isDelete=x.isDelete
                   
                }).Where(x=>(x.isDelete==false || x.isDelete == null)).OrderByDescending(x => x.invoicedate).ToList();

                double partialtotal = 0;
                foreach(var l in list)
                {
                    //    double cash = (from inv in db.Invoices
                    //                join ca in db.Cashes on inv.invoiceno equals ca.Invoiceno
                    //                where inv.invoiceno==l.invoiceno &&
                    //                inv.Pfcode == ca.Pfcode
                    //                && inv.Pfcode == pfcode
                    //                select new 
                    //                {
                    //                    Amount = ca.C_Total_Amount,
                    //                }).Sum(x=>x.Amount)??0;
                    //    double cheque = (from inv in db.Invoices
                    //                  join ch in db.Cheques on inv.invoiceno equals ch.Invoiceno
                    //                     where inv.invoiceno == l.invoiceno &&
                    //                     inv.Pfcode == ch.Pfcode
                    //                   && inv.Pfcode == pfcode
                    //                  select new 
                    //                  {
                    //                      Amount = ch.totalAmount,

                    //                  }).Sum(x=>x.Amount)??0;
                    //    double NEFT = (from inv in db.Invoices
                    //                join ne in db.NEFTs on inv.invoiceno equals ne.Invoiceno
                    //                   where inv.invoiceno == l.invoiceno &&
                    //                   inv.Pfcode == ne.Pfcode
                    //                 && inv.Pfcode == pfcode
                    //                select new 
                    //                {
                    //                    Amount = ne.N_Total_Amount,

                    //                }).Sum(x=>x.Amount)??0;

                    //    double CreditNote = (from inv in db.Invoices
                    //                      join cn in db.CreditNotes on inv.invoiceno equals cn.Invoiceno
                    //                         where inv.invoiceno == l.invoiceno &&
                    //                         inv.Pfcode == cn.Pfcode
                    //                       && inv.Pfcode == pfcode
                    //                      select new 
                    //                      {
                    //                          Amount = cn.Cr_Amount,

                    //                      }).Sum(x=>x.Amount)??0;
                    var PartialtotalAmount = (from inv in db.Invoices
                                              join ca in db.Cashes on inv.invoiceno equals ca.Invoiceno into cashGroup
                                              join ch in db.Cheques on inv.invoiceno equals ch.Invoiceno into chequeGroup
                                              join ne in db.NEFTs on inv.invoiceno equals ne.Invoiceno into neftGroup
                                              join cn in db.CreditNotes on inv.invoiceno equals cn.Invoiceno into creditNoteGroup
                                              where inv.invoiceno==l.invoiceno &&
                                             
                                              inv.Pfcode == pfcode && (inv.isDelete == null || inv.isDelete == false)
                                              select new
                                              {
                                                  TotalAmount =
                                                      (cashGroup.Sum(x => x.C_Total_Amount) ?? 0) +
                                                      (chequeGroup.Sum(x => x.totalAmount) ?? 0) +
                                                      (neftGroup.Sum(x => x.N_Total_Amount) ?? 0) +
                                                      (creditNoteGroup.Sum(x => x.Cr_Amount) ?? 0)
                                              }).Sum(x => x.TotalAmount);

                    partialtotal += (double)PartialtotalAmount ;

                }
                

                var invoiceDashboardData = new InvoiceDataForDashBoard
                {
                    Paid = list.Where(t => t.netamount == t.paid).Sum(t => t.netamount) ?? 0,
                    Unpaid =  list.Where(t => t.paid == null || t.paid < t.netamount).Sum(t => t.netamount - (t.paid??0)) ?? 0,
                    TotalInvoice = list.Count,
                    PaidCount = list.Count(t => t.netamount == t.paid),
                    UnpaidCount = list.Count(t => t.paid == null),
                    TotalNetAmount = list.Sum(t => t.netamount) ?? 0,
                    PattialPaid = partialtotal,
                    Pattialpaidcount = list.Count(t => t.paid > 0 && t.paid < t.netamount)
                };



                // Serialize the data points for use in the view
                ViewBag.DataPoints = JsonConvert.SerializeObject(invoiceDashboardData);
                return View(list);

              

                // Calculate the data for InvoiceDataForDashBoard
           
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
            // string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            //   var InviceData = db.Invoices.Where(x => x.Pfcode == pfcode).ToList();



            // Calculate the data for InvoiceDataForDashBoard
            double partialtotalView = 0;
            foreach (var l in list)
            {
              
                var PartialtotalAmount = (from inv in db.Invoices
                                          join ca in db.Cashes on inv.invoiceno equals ca.Invoiceno into cashGroup
                                          join ch in db.Cheques on inv.invoiceno equals ch.Invoiceno into chequeGroup
                                          join ne in db.NEFTs on inv.invoiceno equals ne.Invoiceno into neftGroup
                                          join cn in db.CreditNotes on inv.invoiceno equals cn.Invoiceno into creditNoteGroup
                                          where inv.invoiceno == l.invoiceno &&

                                          inv.Pfcode == pfcode && (inv.isDelete == null || inv.isDelete == false)
                                          select new
                                          {
                                              TotalAmount =
                                                  (cashGroup.Sum(x => x.C_Total_Amount) ?? 0) +
                                                  (chequeGroup.Sum(x => x.totalAmount) ?? 0) +
                                                  (neftGroup.Sum(x => x.N_Total_Amount) ?? 0) +
                                                  (creditNoteGroup.Sum(x => x.Cr_Amount) ?? 0)
                                          }).Sum(x => x.TotalAmount);

                partialtotalView += (double)PartialtotalAmount;

            }

            var shboardData = new InvoiceDataForDashBoard
            {
                Paid = list.Where(t => t.netamount == t.paid).Sum(t => t.netamount) ?? 0,
                Unpaid = list.Where(t => t.paid == null || t.paid < t.netamount).Sum(t => t.netamount - (t.paid ?? 0)) ?? 0,

                 TotalInvoice = list.Count,
                PaidCount = list.Count(t => t.netamount == t.paid),
                UnpaidCount = list.Count(t => t.paid == null),
                TotalNetAmount = list.Sum(t => t.netamount) ?? 0,
                PattialPaid = partialtotalView,
                Pattialpaidcount = list.Count(t => t.paid > 0 && t.paid < t.netamount)
            };



            // Serialize the data points for use in the view
            ViewBag.DataPoints = JsonConvert.SerializeObject(shboardData);
            return View(list);
        }

        public ActionResult ViewDPInvoice()
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            return View(db.Invoices.Where(m => (((m.Total_Lable != null || m.Total_Lable.Length > 0) && m.Pfcode == strpf)) && m.isDelete==false).ToList());
        }

        [HttpGet]
        public ActionResult ViewSingleInvoice(string invfromdate, string invtodate, string Companydetails, string invoiceNo)
        
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            ViewBag.fromdate = invfromdate;
            ViewBag.todate=invtodate;
            ViewBag.Companydetails = Companydetails;
            ViewBag.invoiceNo = invoiceNo;
         
            var temp = db.singleinvoiceconsignments.Select(m => m.Invoice_no).ToList();


            DateTime? fromdate = null;
            DateTime? todate = null;


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            if (invfromdate != "" && invfromdate!=null)
            {

                string bdatefrom = DateTime.ParseExact(invfromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);

               
            }
            else
            {
                todate = null;
            }

            if (invtodate != "" && invtodate!=null)
            {
                string bdateto = DateTime.ParseExact(invtodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);
               
            }
            else
            {
                fromdate = null;
            }
            if (Companydetails != "")
            {
                ViewBag.Custid = Companydetails;
            }
            
            var a = (from m in db.Invoices
                     where temp.Contains(m.invoiceno) &&
                     m.Pfcode == strpf
                     && (m.isDelete==false || m.isDelete==null)
                     && ( string.IsNullOrEmpty(invfromdate) || m.invoicedate >= fromdate.Value)
                     && (string.IsNullOrEmpty(invtodate) ||  m.invoicedate <= todate.Value)
                     && ( string.IsNullOrEmpty(Companydetails) || m.Customer_Id== Companydetails)
                     && (invoiceNo==null || invoiceNo=="" || m.invoiceno== invoiceNo)
                     select m).OrderByDescending(x=>x.invoicedate).ToList();

          


            double partialtotal = 0;
            foreach (var l in a)
            {
                //double cash = (from inv in db.Invoices
                //               join ca in db.Cashes on inv.invoiceno equals ca.Invoiceno
                //               where 
                //               inv.invoiceno==l.invoiceno &&
                //               ca.Pfcode == strpf
                //               && inv.Pfcode == strpf
                //               select new
                //               {
                //                   Amount = ca.C_Total_Amount,
                //               }).Sum(x => x.Amount) ?? 0;
                //double cheque = (from inv in db.Invoices
                //                 join ch in db.Cheques on inv.invoiceno equals ch.Invoiceno
                //                 where inv.invoiceno == l.invoiceno &&
                //                 ch.Pfcode == strpf
                //                  && inv.Pfcode == strpf
                //                 select new
                //                 {
                //                     Amount = ch.totalAmount,

                //                 }).Sum(x => x.Amount) ?? 0;
                //double NEFT = (from inv in db.Invoices
                //               join ne in db.NEFTs on inv.invoiceno equals ne.Invoiceno
                //               where inv.invoiceno == l.invoiceno &&
                //               ne.Pfcode == strpf
                //                && inv.Pfcode == strpf
                //               select new
                //               {
                //                   Amount = ne.N_Total_Amount,

                //               }).Sum(x => x.Amount) ?? 0;

                //double CreditNote = (from inv in db.Invoices
                //                     join cn in db.CreditNotes on inv.invoiceno equals cn.Invoiceno
                //                     where inv.invoiceno == l.invoiceno &&
                //                     cn.Pfcode == strpf
                //                      && inv.Pfcode == strpf
                //                     select new
                //                     {
                //                         Amount = cn.Cr_Amount,

                //                     }).Sum(x => x.Amount) ?? 0;
                var PartialtotalAmount = (from inv in db.Invoices
                                          join ca in db.Cashes on inv.invoiceno equals ca.Invoiceno into cashGroup
                                          join ch in db.Cheques on inv.invoiceno equals ch.Invoiceno into chequeGroup
                                          join ne in db.NEFTs on inv.invoiceno equals ne.Invoiceno into neftGroup
                                          join cn in db.CreditNotes on inv.invoiceno equals cn.Invoiceno into creditNoteGroup
                                          where inv.invoiceno==l.invoiceno &&
                                          inv.Pfcode == strpf && (inv.isDelete == null || inv.isDelete == false)
                                          select new
                                          {
                                              TotalAmount =
                                                  (cashGroup.Sum(x => x.C_Total_Amount) ?? 0) +
                                                  (chequeGroup.Sum(x => x.totalAmount) ?? 0) +
                                                  (neftGroup.Sum(x => x.N_Total_Amount) ?? 0) +
                                                  (creditNoteGroup.Sum(x => x.Cr_Amount) ?? 0)
                                          }).Sum(x => x.TotalAmount);
                partialtotal += (double)PartialtotalAmount;

            }
            var invoiceDashboardData = new InvoiceDataForDashBoard
            {
                Paid = a.Where(t => t.netamount == t.paid).Sum(t => t.netamount) ?? 0,
                Unpaid = a.Where(t => t.paid == null || t.paid<t.netamount).Sum(t => t.netamount-t.paid) ?? 0,
                TotalInvoice = a.Count,
                PaidCount = a.Count(t => t.netamount == t.paid),
                UnpaidCount = a.Count(t => t.paid == null),
                TotalNetAmount = a.Sum(t => t.netamount) ?? 0,
                PattialPaid =partialtotal,
                Pattialpaidcount = a.Count(t => t.paid > 0 && t.paid < t.netamount)
            };
            // Serialize the data points for use in the view
            ViewBag.DataPoints = JsonConvert.SerializeObject(invoiceDashboardData);

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

            var Companies =(from t in db.TransactionViews
                            join d in db.Destinations 
                            on t.Pincode equals d.Pincode

                            where t.Customer_Id == CustomerId && t.Pf_Code == strpfcode
                           && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(t.Consignment_no)
                            && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(t.Consignment_no)
                           select new
                           {
                               Consignment_no=t.Consignment_no,
                               Name=d.Name,
                               chargable_weight =t.chargable_weight,
                               Pincode=t.Pincode,
                               Mode=t.Mode,
                               Amount=t.Amount??0,
                               tembookingdate=t.tembookingdate,
                               Insurance=t.Insurance,
                               Claimamount=t.Claimamount ?? "0",
                               Percentage=t.Percentage??"0",
                               loadingcharge=t.loadingcharge ?? 0,
                               Risksurcharge=t.Risksurcharge??0,
                               booking_date=t.booking_date,
                               BillAmount = t.BillAmount??0

                           }
                           ).ToList().Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                              .ToList(); 

                
                
                
            //    db.TransactionViews.Where(m => m.Customer_Id == CustomerId && m.Pf_Code == strpfcode && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
            //Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
            //                  .ToList();





            return Json(Companies, JsonRequestBehavior.AllowGet);

        }

        public JsonResult InvoiceTableWithoutGST(string CustomerId, string Tempdatefrom, string TempdateTo)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();



            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};

            string bdatefrom = DateTime.ParseExact(Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
            string bdateto = DateTime.ParseExact(TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");


            DateTime fromdate = Convert.ToDateTime(bdatefrom);
            DateTime todate = Convert.ToDateTime(bdateto);




            db.Configuration.ProxyCreationEnabled = false;

            var Companies = (from t in db.TransactionViews
                             join d in db.Destinations
                             on t.Pincode equals d.Pincode
                             where t.Customer_Id == CustomerId && t.Pf_Code == strpfcode
                    
                             && (t.status_t==null || t.status_t=="GST")
                            && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(t.Consignment_no)
                             select new
                             {
                                 Consignment_no = t.Consignment_no,
                                 Name = d.Name,
                                 chargable_weight = t.chargable_weight,
                                 Pincode = t.Pincode,
                                 Mode = t.Mode,
                                 Amount = t.Amount ?? 0,
                                 tembookingdate = t.tembookingdate,
                                 Insurance = t.Insurance,
                                 Claimamount = t.Claimamount ?? "0",
                                 Percentage = t.Percentage ?? "0",
                                 loadingcharge = t.loadingcharge ?? 0,
                                 Risksurcharge = t.Risksurcharge ?? 0,
                                 booking_date = t.booking_date,
                                 BillAmount=t.BillAmount??0

                             }
                           ).ToList().Where(x => DateTime.Compare(x.booking_date.Value.Date, fromdate) >= 0 && DateTime.Compare(x.booking_date.Value.Date, todate) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
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
           var  result = db.Invoices.Where(m => m.Pfcode == pfcode && (m.invoiceno != null || m.invoiceno != "") && m.isDelete==false).
                Select(m => new { m.invoiceno }).OrderBy(m => m.invoiceno).Distinct().ToList();
            if (Customer_Id != null && Customer_Id!="")
            {
                 result = db.Invoices.Where(m => m.Pfcode == pfcode && (m.invoiceno != null || m.invoiceno != "") && m.Customer_Id==Customer_Id && m.isDelete==false).
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

                string comapnycheck = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically
                if (comapnycheck == null)
                {
                    ModelState.AddModelError("comapnycheck", "Customer Id Does Not Exist");
                    return PartialView("GenerateInvoicePartial", invoice);
                }
                var checkInvocie=db.singleinvoiceconsignments.Where(x=>x.Invoice_no==invoice.invoiceno).FirstOrDefault();
                if (checkInvocie != null)
                {
                    ModelState.AddModelError("InvoiceCheck", "Invoice Number Already Exist");
                    return PartialView("GenerateInvoicePartial", invoice);

                }
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
                    invo.isDelete = false;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invo).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Updated SuccessFully";

                    /////////////////// update consignment///////////////////////
                    using (var db = new db_a92afa_frbillingEntities())
                    {
                        var Companies = db.Transactions.Where(m => m.status_t == invoice.invoiceno && m.isDelete==false).ToList();

                        Companies.ForEach(m => m.status_t = "0");
                        db.SaveChanges();


                        Companies = db.Transactions.Where(m => m.Pf_Code == strpfcode && m.Customer_Id == invoice.Customer_Id && m.isDelete==false &&(m.IsGSTConsignment==null || m.IsGSTConsignment==false )   && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no) && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                     Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

                        Companies.ForEach(m => m.status_t = invoice.invoiceno);
                        db.SaveChanges();
                    }

                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, strpfcode);
                    ///////////////////end of update consignment///////////////////////
                }
                else
                {

                    var invoi = db.Invoices.Where(m => m.tempInvoicedate == invoice.tempInvoicedate && m.Customer_Id == invoice.Customer_Id && m.Pfcode == invoice.Pfcode && m.isDelete==false).FirstOrDefault();

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
                    invo.isDelete = false;
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
                        var Companies = db.Transactions.Where(m => m.status_t == invoice.invoiceno && m.isDelete==false).ToList();

                        Companies.ForEach(m => m.status_t = "0");
                        db.SaveChanges();


                        Companies = db.Transactions.Where(m => m.Pf_Code == strpfcode && m.isDelete == false && m.Customer_Id == invoice.Customer_Id && (m.IsGSTConsignment == null || m.IsGSTConsignment == false) && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no) && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                     Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

                        Companies.ForEach(m => m.status_t = invoice.invoiceno);
                        db.SaveChanges();
                    }
                    ///////////////////end of update consignment///////////////////////
                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart1, strpfcode);

                }
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
               Request.ApplicationPath.TrimEnd('/') + "/";
                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically


                if (Pfcode != null)
                {
                    LocalReport lr = new LocalReport();


                    var dataset = db.TransactionViews.Where(m => m.Pf_Code == strpfcode && m.Customer_Id == invoice.Customer_Id && (m.IsGSTConsignment == null || m.IsGSTConsignment == false) && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no) && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                 Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                               .ToList();

                    var franchisee = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                    franchisee.FirstOrDefault().LogoFilePath = (franchisee.FirstOrDefault().LogoFilePath == null || franchisee.FirstOrDefault().LogoFilePath == "") ? baseUrl + "/assets/Dtdclogo.png" : franchisee.FirstOrDefault().LogoFilePath;

                    var dataset3 = db.Invoices.Where(m => m.invoiceno == invoice.invoiceno && m.Pfcode == strpfcode);

                    var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);
                    dataset3.FirstOrDefault().Invoice_Lable = AmountTowords.changeToWords(dataset3.FirstOrDefault().netamount.ToString());
                    string clientGst = dataset4.FirstOrDefault().Gst_No;
                    string frgst = franchisee.FirstOrDefault().GstNo;

                    franchisee.FirstOrDefault().StampFilePath = (franchisee.FirstOrDefault().StampFilePath == null || franchisee.FirstOrDefault().StampFilePath == "") ? baseUrl + "/assets/Dtdclogo.png" : franchisee.FirstOrDefault().StampFilePath;
                    string discount = dataset3.FirstOrDefault().discount;
                  
                    if (discount == "no")
                    {
                        if (clientGst != null && clientGst.Length > 4)
                        {
                            if (frgst.Substring(0, 2) == clientGst.Substring(0, 2))
                            {
                               string  path = Path.Combine(Server.MapPath("~/RdlcReport"), "PrintInvoice.rdlc");

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
                    //string path = Path.Combine(Server.MapPath("~/RdlcReport"), "InvoiceReportNew.rdlc");

                    //if (System.IO.File.Exists(path))
                    //{
                    //    lr.ReportPath = path;
                    //}

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
                        ViewBag.invoiceno = invoice.invoiceno.Replace("/", "-");
                        ViewBag.strpfcode = strpfcode;
                    }

                    var pdfPath = Server.MapPath("~/PDF/" + strpfcode);
                    // Check if the directory exists
                    if (!Directory.Exists(pdfPath))
                    {
                        // Create the directory if it doesn't exist
                        Directory.CreateDirectory(pdfPath);
                    }
                    var invoicefile = dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";
                    string savePath = Path.Combine(pdfPath,invoicefile );


                   //  savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");
                    ViewBag.savePath = savePath;
                    
                    using (FileStream stream = new FileStream(savePath, FileMode.Create))
                    {
                        stream.Write(renderByte, 0, renderByte.Length);
                    }

                    if (submit == "Email")
                    {
                        var path1 = baseUrl + "/PDF/"+strpfcode+"/" + dataset3.FirstOrDefault().Firm_Id + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

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
<p><a href='{path1}'>Download your Invoice here</a></p> <!-- Add a link to the path -->      
                                <p>If you have any questions or concerns regarding your invoice, please contact our support team.<br />
                                    <strong> at +91 82086688415</strong></p>

                                <p>Thank you for choosing Fr-Billing.</p>
                                <p>Best regards,</p>
                                <p><strong>Fr-Billing</strong></p>
                            </div>
                        </body>
                        </html>
                        ";

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


                        //string em = dataset4.FirstOrDefault().Email;
                        //var mail = SendEmailModel.SendEmail(em, "Invoice", emailBody, renderByte);
                        //if (mail == "Failed to send email")
                        //{
                        //    TempData["emailError"] = "Something Went Wrong to Sent Mail!!!";
                        //    return PartialView("GenerateInvoicePartial", invoice);
                        //}

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


                }
                else
                {
                    TempData["NUllCustomer"] = "Customer Id Does not Exists";
                    ViewBag.success = null;
                }


                ModelState.Clear();
                return PartialView("GenerateInvoicePartial", invoice);

            }
            return PartialView("GenerateInvoicePartial", invoice);
        }


        public ActionResult Download(long id)
         {
            var pfcode = Request.Cookies["cookies"]["AdminValue"].ToString();

            var invoice = db.Invoices.Where(m => m.IN_Id == id && m.Pfcode==pfcode).FirstOrDefault();
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                 Request.ApplicationPath.TrimEnd('/') + "/";
            var pdfPath = Server.MapPath("~/PDF/" + pfcode);
            var filename = invoice.invoiceno.Replace("/", "-") + ".pdf";
            string savePath = Path.Combine(pdfPath,filename);
            if (invoice != null)
            {
                if (System.IO.File.Exists(savePath))
                {

                    savePath=baseUrl+"/PDF/"+pfcode +"/"+ invoice.invoiceno.Replace("/", "-") + ".pdf";
                    return Redirect(savePath);
                }
                else
                {
                     savePath = baseUrl + "/PDF/" + invoice.invoiceno.Replace("/", "-") + ".pdf";

                    return Redirect(savePath);
                }

            }
            return Redirect("ViewInvoice");

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
                string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
                               Request.ApplicationPath.TrimEnd('/');
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
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? baseurl+"/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;
                dataset2.FirstOrDefault().StampFilePath = (dataset2.FirstOrDefault().StampFilePath == null || dataset2.FirstOrDefault().StampFilePath == "") ? baseurl+"/assets/Dtdclogo.png" : dataset2.FirstOrDefault().StampFilePath;


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
                string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
                              Request.ApplicationPath.TrimEnd('/');
                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();


                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
          Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                        .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? baseurl+"/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

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
                string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
     Request.ApplicationPath.TrimEnd('/');
                LocalReport lr = new LocalReport();



                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id)
                           .ToList().
                           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0)
                      .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                /*https://frbilling.com     currently we have remove the static link done its dynamic*/ 
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? baseurl+"/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

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
            var franchisee = db.Franchisees.Where(x => x.PF_Code == strpfcode).FirstOrDefault();
            var gst = franchisee.GstNo;
            ViewBag.GST = gst;
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
                    var franchisee = db.Franchisees.Where(x => x.PF_Code == strpfcode).FirstOrDefault();

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
                    inv.fullsurchargetaxtotal = Math.Round((double)((inv.total * Convert.ToDouble(cm.Fuel_Sur_Charge)) / 100));

                    string invoiceno = "0";

                    string finalInvoiceno="0";  
                    var dataInvStart = (from d in db.Franchisees
                                        where d.PF_Code == strpfcode
                                        select d.InvoiceStart).FirstOrDefault();

                    string invstart1 = dataInvStart + "/2023-24/";
                    //string invstart1 = "IJS/2022-23/";
                    string no = "";
                    string finalstring = "";
                    if (strpfcode == "MF868" || strpfcode == "PF1649" || strpfcode == "PF934" || strpfcode == "UF2679" || strpfcode == "CF2024" || strpfcode == "PF2214" || strpfcode == "PF1958" || strpfcode == "PF2213" || strpfcode == "PF2046" || strpfcode == "PF857")
                    {
                        dataInvStart = (from d in db.Franchisees
                                        where d.PF_Code == strpfcode
                                        select d.InvoiceStart).FirstOrDefault();
                        
                        invstart1 = dataInvStart + "/2024-25/";
                    }

                    string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                    string lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "00";
                    if (strpfcode == "CF2024")
                    {
                        lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                        lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? dataInvStart + "/" + "00" + "/2024-25";

                    }
                    else if (strpfcode == "CF2567")
                    {
                        dataInvStart = "NGR";
                        lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                        lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? dataInvStart + " " + 120;

                    }

                    //if (lastInvoiceno == null)
                    //{
                    //    string[] strarrinvno = lastInvoiceno1.Split('/');
                    //    if (strpfcode == "CF2024")
                    //    {

                    //        string incrementedNumber = "00";
                    //        string[] strarrinvno = lastInvoiceno1.Split('/');
                    //        strarrinvno = lastInvoiceno1.Split('/');
                    //        int number1 = int.Parse(strarrinvno[2]) + 1;
                    //        if (number1 < 10)
                    //        {
                    //            incrementedNumber = number1.ToString().PadLeft(2, '0');

                    //        }
                    //        else
                    //        {
                    //            incrementedNumber = number1.ToString();
                    //        }
                    //        // string incrementedNumber = number1.ToString().PadLeft(2, '0');
                    //        finalInvoiceno = dataInvStart + "/" + incrementedNumber + "/2024-25";


                    //    }
                    //    else
                    //    {
                    //        int number = Convert.ToInt32(lastInvoiceno.Substring(12));
                    //        no = lastInvoiceno.Substring(12);
                    //        finalInvoiceno = invstart1 + "" + (no + 1);
                    //    }



                    //}
                    //else
                    //{
                    //    string[] strarrinvno = lastInvoiceno1.Split('/');
                    //    string incrementedNumber = "0";
                    //    if (strpfcode== "CF2024")
                    //    {
                    //         int  newnumber = Convert.ToInt32(int.Parse(strarrinvno[2]) + 1);
                    //        if (newnumber < 10)
                    //        {
                    //            incrementedNumber = newnumber.ToString().PadLeft(2, '0');

                    //        }
                    //        else
                    //        {
                    //            incrementedNumber = newnumber.ToString();
                    //        }
                    //        //  string incrementedNumber = newnumber.ToString().PadLeft(2, '0');
                    //        finalInvoiceno = dataInvStart + "/" + (int.Parse(strarrinvno[2]) + 1) + "/2024-25";
                    //    }
                    //    else
                    //    {
                    //        int newnumber = Convert.ToInt32(strarrinvno[2]) + 1;
                    //        finalstring = newnumber.ToString("000");
                    //        finalInvoiceno = invstart1 + "" + finalstring;

                    //    }
                    //    //string val = lastInvoiceno.Substring(19, lastInvoiceno.Length - 19);


                    //}
                    if (lastInvoiceno == null)
                    {
                        string[] strarrinvno = lastInvoiceno1.Split('/');
                        if (strpfcode == "PF2214")
                        {
                            strarrinvno = lastInvoiceno1.Split('/');
                            finalInvoiceno = invstart1 + "" + (strarrinvno[3] + 1);

                        }
                        else if (strpfcode == "PF975")
                        {
                            finalInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);
                            if (strarrinvno[2] == "00")
                            {
                                strarrinvno[2] = "597";
                                finalInvoiceno = invstart1 + "" + (strarrinvno[2]);

                            }




                        }

                        else if (strpfcode == "UF2679")
                        {
                            finalInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);
                            if (strarrinvno[2] == "00")
                            {
                                strarrinvno[2] = "10";
                                finalInvoiceno = invstart1 + "" + (strarrinvno[2]);

                            }


                        }

                        else if (strpfcode == "PF2214")
                        {
                            strarrinvno = lastInvoiceno1.Split('/');
                            finalInvoiceno = invstart1 + "" + (strarrinvno[3] + 1);

                        }
                        else if (strpfcode == "CF2024")
                        {
                            string incrementedNumber = "00";
                            strarrinvno = lastInvoiceno1.Split('/');
                            int number = int.Parse(strarrinvno[1]) + 1;

                            if (number < 10)
                            {
                                incrementedNumber = number.ToString().PadLeft(2, '0');

                            }
                            else
                            {
                                incrementedNumber = number.ToString();
                            }
                            finalInvoiceno = dataInvStart + "/" + incrementedNumber + "/2024-25";
                        }
                        else if (franchisee.PF_Code == "CF2567")
                        {
                            strarrinvno = lastInvoiceno1.Split(' ');
                            int number = int.Parse(strarrinvno[1]) + 1;


                            ViewBag.lastInvoiceno = dataInvStart + " " + number;
                        }

                        else
                        {
                            finalInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);

                        }
                    }

                    else
                    {

                        string[] strarrinvno = lastInvoiceno1.Split('/');
                        //string val = lastInvoiceno.Substring(19, lastInvoiceno.Length - 19);
                        int newnumber = 0;
                        string incrementedNumber = "00";
                        if (strpfcode == "PF2214")
                        {
                            newnumber = Convert.ToInt32(strarrinvno[3]) + 1;
                            finalstring = newnumber.ToString("000");
                            ViewBag.lastInvoiceno = invstart1 + "" + finalstring;
                        }
                        else if (strpfcode == "CF2024")
                        {
                            newnumber = Convert.ToInt32(int.Parse(strarrinvno[1]) + 1);

                            if (newnumber < 10)
                            {
                                incrementedNumber = newnumber.ToString().PadLeft(2, '0');

                            }
                            else
                            {
                                incrementedNumber = newnumber.ToString();
                            }

                            //string incrementedNumber = newnumber.ToString().PadLeft(2, '0');
                            finalInvoiceno = dataInvStart + "/" + incrementedNumber + "/2024-25";
                        }
                        else if (franchisee.PF_Code == "CF2567")
                        {
                            strarrinvno = lastInvoiceno1.Split(' ');
                            int number = int.Parse(strarrinvno[1]) + 1;


                            ViewBag.lastInvoiceno = dataInvStart + " " + number;
                        }
                        else
                        {
                            newnumber = Convert.ToInt32(strarrinvno[2]) + 1;
                            finalstring = newnumber.ToString("000");
                            finalInvoiceno = invstart1 + "" + finalstring;
                        }


                    }

                    inv.invoiceno = finalInvoiceno;

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
                        else if (j.Consignment_no.ToLower().StartsWith("b"))
                        {
                            inv.Docket_charges = inv.Docket_charges + Convert.ToDouble(cm.B_Docket);
                        }
                    }

                    inv.Royalty_charges = Math.Round((double)((inv.total * Convert.ToDouble(cm.Royalty_Charges??1)) / 100));

                    inv.servicetaxtotal = Math.Round((double)(((inv.total + inv.fullsurchargetaxtotal + inv.Docket_charges + inv.Royalty_charges) * invoice.servicetax) / 100)); //((gst_total * parseFloat("0" + gst)) / 100);
                    inv.netamount = Math.Round((double)(inv.total + inv.Docket_charges + inv.Royalty_charges + inv.servicetaxtotal + inv.fullsurchargetaxtotal));
                    inv.netamount = Math.Round(inv.netamount ?? 0, 0);
                    inv.Invoice_Lable = AmountTowords.changeToWords(inv.netamount.ToString());
                    inv.Docket_charges= Math.Round((double)inv.Docket_charges);
                    if (inv.netamount > 0)
                    {

                        inv.isDelete = false;
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
            string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
                              Request.ApplicationPath.TrimEnd('/');
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
            dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? baseurl+"/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;
            dataset2.FirstOrDefault().StampFilePath = (dataset2.FirstOrDefault().StampFilePath == null || dataset2.FirstOrDefault().StampFilePath == "") ? baseurl + "/assets/Dtdclogo.png" : dataset2.FirstOrDefault().StampFilePath;

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
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            //New Updation in the Path of the Invocie PDf save
            var pdfpath = Server.MapPath("~/PDF/" + strpfcode);
            if (!Directory.Exists(pdfpath))
            {
                Directory.CreateDirectory(pdfpath);
            }
            // Construct the file name with Firm_Id and Invoice number, replacing "/" with "-"
            string fileName =  dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

            // Combine the directory path and the file name to get the full save path
            string savePath = Path.Combine(pdfpath, fileName);


            // string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id + "-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");
          //  string savePath = Path.Combine(pdfpath + dataset3.FirstOrDefault().Firm_Id + "-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");


            var path1 = baseUrl + pdfpath + dataset3.FirstOrDefault().Firm_Id + "-" + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                stream.Write(renderByte, 0, renderByte.Length);
            }

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
 <p><a href='{path1}'>Download your Invoice here</a></p> <!-- Add a link to the path -->            
<p>If you have any questions or concerns regarding your invoice, please contact our support team.<br />
                                    <strong> at +91  8208668841</strong></p>

                                <p>Thank you for choosing OnTrack.</p>
                                <p>Best regards,</p>
                                <p><strong>OnTrack Express</strong></p>
                            </div>
                        </body>
                        </html>
                        ";

                //string em = dataset4.FirstOrDefault().Email;
                //var mail = SendEmailModel.SendEmail(em, "Invoice", emailBody, renderByte);
                //if (mail == "Failed to send email")
                //{
                //    TempData["emailError"] = "Something Went Wrong to Sent Mail!!!";
                //}


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

                string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
                                       Request.ApplicationPath.TrimEnd('/');

                Invoice inc = db.Invoices.Where(m => m.invoiceno == myParameter && m.Firm_Id == firmid).FirstOrDefault();

                string Pfcode = db.Companies.Where(m => m.Company_Id == inc.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();

                var dataset = db.TransactionViews.Where(m => m.Customer_Id == inc.Customer_Id)
                           .ToList().
                           Where(x => DateTime.Compare(x.booking_date.Value.Date, inc.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, inc.periodto.Value.Date) <= 0)
                      .ToList();


                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);//https://frbilling.com
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? baseurl+"/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

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

            string invstart1 = dataInvStart + "/2024-25/";

            ViewBag.Zipinv = invstart1;

            return View();
        }

        [HttpPost]
        public ActionResult InvoiceZip(string frominv, string toinv)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            string fileType = "application/octet-stream";
            var flenght = frominv.Length;
            var tlength=toinv.Length;

            var ftoInt = Convert.ToInt32(frominv);
            var tToInt=Convert.ToInt32(toinv);  



            var outputStream = new MemoryStream();


            using (ZipFile zipFile = new ZipFile())
            {



                for (int i = ftoInt; i <= tToInt; i++)
                {
           

                    var dataInvStart = (from d in db.Franchisees
                                        where d.PF_Code == strpfcode
                                        select d.InvoiceStart).FirstOrDefault();

                    string invstart1 = dataInvStart + "/2024-25/";

                    if (strpfcode == "PF2214" || strpfcode == "PF934" || strpfcode == "PF1958" || strpfcode == "CF2024" || strpfcode == "PF2213" || strpfcode == "PF2046" || strpfcode == "PF857" || strpfcode == "PF1649" || strpfcode == "MF868" || strpfcode == "UF2679")
                    {
                        

                        invstart1 = dataInvStart + "/2024-25/";


                    }
                    var paddedInvoiceNumber = i.ToString().PadLeft(flenght, '0');
                    var pdfPath = Server.MapPath("~/PDF/" + strpfcode);
                    var filename = invstart1.Replace("/", "-") + paddedInvoiceNumber + ".pdf";
                    string filePath = Path.Combine(pdfPath, filename);

                    if (System.IO.File.Exists(filePath))
                    {
                        zipFile.AddFile(filePath, "Invoices");
                    }
                    else 
                    {
                        filePath = Server.MapPath("/PDF/" + invstart1.Replace("/", "-") + paddedInvoiceNumber + ".pdf");
                        if (System.IO.File.Exists(filePath))
                        {
                            zipFile.AddFile(filePath, "Invoices");
                        }
                        //else
                            
                        //{
                        //    var pPath = Server.MapPath("~/PDF/" + strpfcode + "/GSTInvoice");
                            
                        //    var invoicefile = invstart1.Replace("/", "-") + paddedInvoiceNumber + ".pdf";
                        //     filePath = Path.Combine(pPath, invoicefile);
                        //    //filePath = Server.MapPath("/PDF/" + invstart1.Replace("/", "-") + paddedInvoiceNumber + ".pdf");
                        //    if (System.IO.File.Exists(filePath))
                        //    {
                        //        zipFile.AddFile(filePath, "Invoices");
                        //    }
                        //}
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
            ViewBag.currentPfcode=strpfcode;
            var franchisee = db.Franchisees.Where(x => x.PF_Code == strpfcode).FirstOrDefault();
            var gst = franchisee.GstNo;
            ViewBag.GST = gst;
            var dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

            string invstart1 = dataInvStart + "/2023-24/";
            if (strpfcode == "PF2214" || strpfcode== "CF2024" || strpfcode== "PF2213" || strpfcode== "PF2046" || strpfcode== "PF857" || strpfcode == "1")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";


            }
            if (strpfcode == "PF1649")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";
            }

            if (strpfcode == "MF868")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";
            }

            if (strpfcode == "UF2679")
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

            if (strpfcode == "CF2024")
            {
                lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode && franchisee.InvoiceYear == "2024-25").OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode && franchisee.InvoiceYear == "2024-25").OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? dataInvStart + "/" + "00" + "/2024-25";

            }
              else if (strpfcode == "CF2567")
            {
                dataInvStart = "NGR";
                lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? dataInvStart + " " + 120;

            }


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
                else if (franchisee.PF_Code == "UF2679")
                {
                    ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2] + 1);
                    if (strarrinvno[2] == "00")
                    {
                        strarrinvno[2] = "10";
                        ViewBag.lastInvoiceno = invstart1 + "" + (strarrinvno[2]);

                    }

                }
                else if (franchisee.PF_Code == "CF2024")
                {
                    string incrementedNumber = "00";
                    strarrinvno = lastInvoiceno1.Split('/');
                    int number = int.Parse(strarrinvno[1]) + 1;
                    if (strpfcode == "CF2024")
                 
                        if (number < 10)
                        {
                            incrementedNumber = number.ToString().PadLeft(2, '0');

                        }
                        else
                        {
                            incrementedNumber = number.ToString();
                        }
                       
               //     string incrementedNumber = number.ToString().PadLeft(2, '0');
                    ViewBag.lastInvoiceno = dataInvStart +"/"+ incrementedNumber + "/2024-25";
                }
                else if (franchisee.PF_Code == "CF2567")
                {
                    strarrinvno = lastInvoiceno1.Split(' ');
                    int number = int.Parse(strarrinvno[1]) + 1;


                    ViewBag.lastInvoiceno = dataInvStart + " " + number;
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
                    finalstring = newnumber.ToString("000");
                    ViewBag.lastInvoiceno = invstart1 + "" + finalstring;
                }
                else if (franchisee.PF_Code == "CF2024")
                {
                    newnumber = Convert.ToInt32(strarrinvno[1]) + 1;
                    string incrementedNumber = "00";
                        if (newnumber < 10)
                        {
                            incrementedNumber = newnumber.ToString().PadLeft(2, '0');

                        }
                        else
                        {
                            incrementedNumber = newnumber.ToString();
                        }
                      
                       
                    //string incrementedNumber = newnumber.ToString().PadLeft(2, '0');
                    ViewBag.lastInvoiceno = dataInvStart+"/" + incrementedNumber+ "/2024-25";
                }
                else if (franchisee.PF_Code == "CF2567")
                {
                    strarrinvno = lastInvoiceno1.Split(' ');
                    int number = int.Parse(strarrinvno[1]) + 1;


                    ViewBag.lastInvoiceno = dataInvStart + " " + number;
                }
                else
                {
                    newnumber = Convert.ToInt32(strarrinvno[2]) + 1;
                    finalstring = newnumber.ToString("000");
                    ViewBag.lastInvoiceno = invstart1 + "" + finalstring;
                }

               
            }



            Invoice inv = db.Invoices.Where(m => m.invoiceno == Invoiceno && m.Pfcode == strpfcode).FirstOrDefault();



            if (Invoiceno != null)
            {
                ViewBag.consignmnts = string.Join(",", db.singleinvoiceconsignments.Where(m => m.Invoice_no == Invoiceno).Select(m => m.Consignment_no).ToArray());
            }


            var data = (from d in db.Invoices
                        where d.Pfcode == strpfcode
                        && d.invoiceno == Invoiceno
                        && d.isDelete!=true
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
                Inv.servicetax = data.servicetax??0;
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
            string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
                               Request.ApplicationPath.TrimEnd('/');
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


                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, inv.Pfcode);


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
                    invo.isDelete = false;
                    invo.periodfrom = Convert.ToDateTime(bdatefrom);
                    invo.periodto = Convert.ToDateTime(bdateto);
                    invo.invoicedate = Convert.ToDateTime(invdate);
                    invo.Pfcode = strpfcode;
                    invo.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    invo.Royalty_charges = invoice.Royalty_charges;
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



                    ViewBag.nextinvoice = GetmaxInvoiceno(invstart, strpfcode);

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
                    invo.Royalty_charges = invoice.Royalty_charges;
                    invo.periodfrom = Convert.ToDateTime(bdatefrom);
                    invo.periodto = Convert.ToDateTime(bdateto);
                    invo.invoicedate = Convert.ToDateTime(invdate);
                    invo.Pfcode = strpfcode;
                    invo.isDelete = false;

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
                    var transaction = db.Transactions.Where(m => Companies.Contains(m.Consignment_no) && (m.IsGSTConsignment == null || m.IsGSTConsignment == false) && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList();

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
                    TransactionView temp = db.TransactionViews.Where(m => m.Consignment_no == c.Consignment_no && (m.IsGSTConsignment == null || m.IsGSTConsignment == false) && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).FirstOrDefault();
                    dataset.Add(temp);
                }

                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);//https://frbilling.com
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? baseurl+"/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

                var dataset3 = db.Invoices.OrderByDescending(m => m.invoiceno == invoice.invoiceno && m.Pfcode == strpfcode);

                var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

                string clientGst = dataset4.FirstOrDefault().Gst_No;
                string frgst = dataset2.FirstOrDefault().GstNo;
                string discount = dataset3.FirstOrDefault().discount;
                if (discount=="no")
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



                //ViewBag.pdf = true;
                //ViewBag.invoiceno = invoice.invoiceno;
                ViewBag.pdf = true;
                ViewBag.invoiceno = invoice.invoiceno.Replace("/", "-");
                ViewBag.strpfcode = strpfcode;

                var pdfPath = Server.MapPath("~/PDF/" + strpfcode);
                if (!Directory.Exists(pdfPath))
                {
                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(pdfPath);
                }
                var invoicefile = dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";
                //string savePath = Server.MapPath("~/PDF/" + dataset3.FirstOrDefault().Firm_Id + dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf");
                string savePath = Path.Combine(pdfPath,invoicefile );

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



                ModelState.Clear();


                return PartialView("GenerateInvoiceSinglePartial", invoice);

            }


            return PartialView("GenerateInvoiceSinglePartial", invoice);
        }


        public JsonResult InvoiceTableSingle(string[] array, string Customerid)
         {

            //  List<Transaction> Companies = new List<Transaction>();
            var result = new List<object>();

            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            db.Configuration.ProxyCreationEnabled = false;
            if (array != null)
            {
                foreach (var i in array.Distinct().ToArray())
                {

                    //   Transaction tr = db.Transactions.Where(m => m.Consignment_no == i.Trim() && m.Pf_Code == strpfcode && m.Customer_Id == Customerid).FirstOrDefault();
                    //for showing the Destination
                
                    var tr = db.Transactions
    .Join(db.Destinations,
          transaction => transaction.Pincode,
          destination => destination.Pincode,
          (transaction, destination) => new { transaction, destination })
    .Where(joined => joined.transaction.Consignment_no == i.Trim()
                     && joined.transaction.Pf_Code == strpfcode
                     && joined.transaction.Customer_Id == Customerid
                     && joined.transaction.isDelete == false
                                        

                     && !db.GSTInvoiceConsignments.Select(b => b.Consignment_no).Contains(i.Trim()))// Moved to the right position
    .Select(joined => new { Transaction = joined.transaction, Name = joined.destination.Name })
    .FirstOrDefault();

                    if (tr != null)
                    {
                        //Companies.Add(tr);

                        result.Add(tr);
                    }

                }
            }


            //return Json(Companies, JsonRequestBehavior.AllowGet);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ReportsinglePrinterMethod(string myParameter, long firmid) //on view call thise method
        {
            {

                LocalReport lr = new LocalReport();

                string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
                         Request.ApplicationPath.TrimEnd('/');

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
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ?  baseurl+"/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

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
                string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
                             Request.ApplicationPath.TrimEnd('/');
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



                var dataset2 = db.Franchisees.Where(x => x.PF_Code == Pfcode);//https://frbilling.com
                dataset2.FirstOrDefault().LogoFilePath = (dataset2.FirstOrDefault().LogoFilePath == null || dataset2.FirstOrDefault().LogoFilePath == "") ? baseurl+"/assets/Dtdclogo.png" : dataset2.FirstOrDefault().LogoFilePath;

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

        public string GetmaxInvoiceno(string invstart2, string strpfcode)
        {
            var franchisee = db.Franchisees.Where(x => x.PF_Code == strpfcode).FirstOrDefault();
            var gst = franchisee.GstNo;
            ViewBag.GST = gst;

            //string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == pfcode).Select(m => m.invoiceno).FirstOrDefault();
            ////string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Firm_Id== firmid).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart + 00;
            //string lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == pfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "00";

            // int number = Convert.ToInt32(lastInvoiceno.Substring(12));




            //if (lastInvoiceno == null)
            //{
            //    string[] strarrinvno = lastInvoiceno1.Split('/');
            //    string lastInvoice = invstart1 + "" + (strarrinvno[2] + 1);
            //    return lastInvoice;
            //}

            //else
            //{

            //    string[] invno = lastInvoiceno.Split('/');
            //    int newnumber = Convert.ToInt32(invno[2]) + 1;
            //    string finalstring = newnumber.ToString("000");
            //    string lastInvoice = invstart1 + finalstring;
            //    return lastInvoice;
            //}
            string lastInvoice ="";
            
            var dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

            // INV/1411/2024-25/001
            // PF2214


            string invstart1 = dataInvStart + "/2023-24/";
            string no = "";
            string finalstring = "";
            if (strpfcode == "PF2214" || strpfcode == "PF934" || strpfcode == "PF1958" || strpfcode == "CF2024" || strpfcode == "PF2213" || strpfcode == "PF2046" || strpfcode == "PF857")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";


            }
            if (strpfcode == "PF1649")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";
            }

            if (strpfcode == "MF868")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";
            }
            if (strpfcode == "UF2679")
            {
                dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

                invstart1 = dataInvStart + "/2024-25/";
            }
            string lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
            string lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(invstart1) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "00";

            if (franchisee.PF_Code == "CF2024")
            {
                lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode && franchisee.InvoiceYear == "2024-25").OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode && franchisee.InvoiceYear == "2024-25").OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? dataInvStart + "/" + "00" + "/2024-25";

            }
            else if (franchisee.PF_Code == "CF2567")
            {
                dataInvStart = "NGR";
                lastInvoiceno = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault();
                lastInvoiceno1 = db.Invoices.Where(m => m.invoiceno.StartsWith(dataInvStart) && m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? dataInvStart + " " + 120;

            }
            if (lastInvoiceno == null)
            {
                string[] strarrinvno = lastInvoiceno1.Split('/');
                if (strpfcode== "PF2214")
                {
                    strarrinvno = lastInvoiceno1.Split('/');
                    lastInvoice = invstart1 + "" + (strarrinvno[3] + 1);

                }
                else if (strpfcode == "PF975")
                {
                    lastInvoice = invstart1 + "" + (strarrinvno[2] + 1);
                    if (strarrinvno[2] == "00")
                    {
                        strarrinvno[2] = "597";
                        lastInvoice = invstart1 + "" + (strarrinvno[2]);

                    }




                }

                else if (strpfcode == "UF2679")
                {
                    lastInvoice = invstart1 + "" + (strarrinvno[2] + 1);
                    if (strarrinvno[2] == "00")
                    {
                        strarrinvno[2] = "10";
                        lastInvoice = invstart1 + "" + (strarrinvno[2]);

                    }


                }

                else if (franchisee.PF_Code == "PF2214")
                {
                    strarrinvno = lastInvoiceno1.Split('/');
                    lastInvoice = invstart1 + "" + (strarrinvno[3] + 1);

                }
                else if (franchisee.PF_Code == "CF2024")
                {
                    string incrementedNumber = "00";
                    strarrinvno = lastInvoiceno1.Split('/');
                    int number = int.Parse(strarrinvno[1]) + 1;

                    if (number < 10)
                    {
                        incrementedNumber = number.ToString().PadLeft(2, '0');

                    }
                    else
                    {
                        incrementedNumber = number.ToString();
                    }
                    lastInvoice = dataInvStart + "/" + incrementedNumber + "/2024-25";
                }
                else if (franchisee.PF_Code == "CF2567")
                {
                    strarrinvno = lastInvoiceno1.Split(' ');
                    int number = int.Parse(strarrinvno[1]) + 1;


                    lastInvoice = dataInvStart + " " + number;
                }
                else
                {
                    lastInvoice = invstart1 + "" + (strarrinvno[2] + 1);

                }
            }

            else
            {

                string[] strarrinvno = lastInvoiceno1.Split('/');
                //string val = lastInvoiceno.Substring(19, lastInvoiceno.Length - 19);
                int newnumber = 0;
                string incrementedNumber = "00";
                if (franchisee.PF_Code == "PF2214")
                {
                    newnumber = Convert.ToInt32(strarrinvno[3]) + 1;
                    finalstring = newnumber.ToString("000");
                    lastInvoice = invstart1 + "" + finalstring;
                }
                else if (franchisee.PF_Code == "CF2024")
                {
                    newnumber = Convert.ToInt32(int.Parse(strarrinvno[1]) + 1);

                    if (newnumber < 10)
                    {
                        incrementedNumber = newnumber.ToString().PadLeft(2, '0');

                    }
                    else
                    {
                        incrementedNumber = newnumber.ToString();
                    }

                    //string incrementedNumber = newnumber.ToString().PadLeft(2, '0');
                    lastInvoice = dataInvStart + "/" + incrementedNumber + "/2024-25";
                }
                else if (franchisee.PF_Code == "CF2567")
                {
                    strarrinvno = lastInvoiceno1.Split(' ');
                    int number = int.Parse(strarrinvno[1]) + 1;


                    lastInvoice = dataInvStart + " " + number;
                }
                else
                {
                    newnumber = Convert.ToInt32(strarrinvno[2]) + 1;
                    finalstring = newnumber.ToString("000");
                    lastInvoice = invstart1 + "" + finalstring;
                }


            }

            return lastInvoice;
        }

     
        [HttpGet]
        public string DownloadByInvNo(string invoiceno)
        {
            string PfCode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            string baseurl = Request.Url.Scheme + "://" + Request.Url.Authority +
                     Request.ApplicationPath.TrimEnd('/');
            var invoice = db.Invoices.Where(m => m.invoiceno == invoiceno && m.Pfcode == PfCode).FirstOrDefault();

            string companyname = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Company_Id).FirstOrDefault().ToString();

            var pdffileName = invoice.invoiceno.Replace("/", "-") + ".pdf";
            //https://frbilling.com/PDF/DFRB-2023-24-144.pdf
            string savePath = baseurl+"/PDF/" + pdffileName;

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




        [HttpGet]
        public ActionResult RecycleInvoice()
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var list = db.Invoices.Where(x => x.Pfcode == strpfcode && x.isDelete == true).ToList();
            return View(list);
        }
        public ActionResult RestoreInvoice(string invoiceno)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var data = db.Invoices.Where(x => x.invoiceno == invoiceno && x.Pfcode==strpfcode).FirstOrDefault();
            if (data != null)
            {
                data.isDelete = false;
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Invoice Restore Successfully";

                return RedirectToAction("RecycleInvoice");

            }
            TempData["Message"] = "Something Went Wrong";

            return RedirectToAction("RecycleInvoice");
        }



        //Generate Invocie Withot GST

        [HttpGet]
        public ActionResult ViewInvoiceWithoutGST(string invfromdate, string invtodate, string Companydetails, string invoiceNo, int? InvoiceId, bool isDelete = false)

        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            ViewBag.fromdate = invfromdate;
            ViewBag.todate = invtodate;
            ViewBag.Companydetails = Companydetails;
            ViewBag.invoiceNo = invoiceNo;
            if (isDelete)
            {
                var checkInvoiceNo = db.GSTInvoices.Where(x => x.IN_Id == InvoiceId && x.Pfcode == strpf).FirstOrDefault();
                if (checkInvoiceNo == null)
                {
                    TempData["error"] = "Invalid Invoice No";

                }

                db.GSTInvoices.Remove(checkInvoiceNo);
                db.SaveChanges();

                //checkInvoiceNo.isDelete=true;
                //  db.Entry(checkInvoiceNo).State = EntityState.Modified;
                var signle = db.GSTInvoiceConsignments.Where(x => x.InvoiceNo == invoiceNo).ToList();
                foreach (var i in signle)
                {
                    db.GSTInvoiceConsignments.Remove(i);
                    db.SaveChanges();
                }
                foreach (var i in signle)
                {
                    var tran = db.Transactions.Where(x => x.Consignment_no == i.Consignment_no).FirstOrDefault();
                    tran.IsGSTConsignment = false;
                    tran.status_t = null;
                    db.Entry(tran).State = EntityState.Modified;
                    db.SaveChanges();
                }

                TempData["success"] = "Invoice Number " + invoiceNo + "  Deleted successfully";
                ViewBag.invoiceno = "";
                invoiceNo = "";
            }

            var temp = db.GSTInvoiceConsignments.Select(m => m.InvoiceNo).ToList();


            DateTime? fromdate = null;
            DateTime? todate = null;


            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};


            if (invfromdate != "" && invfromdate != null)
            {

                string bdatefrom = DateTime.ParseExact(invfromdate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                fromdate = Convert.ToDateTime(bdatefrom);


            }
            else
            {
                todate = null;
            }

            if (invtodate != "" && invtodate != null)
            {
                string bdateto = DateTime.ParseExact(invtodate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                todate = Convert.ToDateTime(bdateto);

            }
            else
            {
                fromdate = null;
            }
            if (Companydetails != "")
            {
                ViewBag.Custid = Companydetails;
            }

            var a = (from m in db.GSTInvoices
                     where /*temp.Contains(m.invoiceno) &&*/
                     m.Pfcode == strpf
                     && (m.isDelete == false || m.isDelete == null)
                     && (string.IsNullOrEmpty(invfromdate) || m.invoicedate >= fromdate.Value)
                     && (string.IsNullOrEmpty(invtodate) || m.invoicedate <= todate.Value)
                     && (string.IsNullOrEmpty(Companydetails) || m.Customer_Id == Companydetails)
                     && (invoiceNo == null || invoiceNo == "" || m.invoiceno == invoiceNo)
                     select m).OrderByDescending(x => x.invoicedate).ToList();



            return View(a);

        }

        public ActionResult GenerateInvoiceWithoutGST(int InvoiceID = 0)
        {


            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            ViewBag.currentPfcode = strpfcode;

            var dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.InvoiceStart).FirstOrDefault();

            string invstart1 = strpfcode + "/2024-25/";
            string no = "";
            string finalstring = "";

            string lastInvoiceno = db.GSTInvoices.Where(m => m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "0";
            string[] strarrinvno = lastInvoiceno.Split('/');

            int number = Convert.ToInt32(strarrinvno[2]) + 1;
            ViewBag.lastInvoiceno = invstart1 + "/" + number;

            var data = (from d in db.GSTInvoices
                        where d.Pfcode == strpfcode
                        && d.IN_Id == InvoiceID
                        && d.isDelete == false
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
                Inv.servicetax = data.servicetax ?? 0;
                Inv.servicetaxtotal = data.servicetaxtotal;
                Inv.othercharge = data.othercharge;
                Inv.netamount = data.netamount;
                Inv.Customer_Id = data.Customer_Id;
              
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
              
                Inv.Pfcode = data.Pfcode;

                return View(Inv);
            }

            return View();


        }
        [HttpPost]
        public ActionResult SaveInvoiceWithoutGST(InvoiceModel invoice, string submit)
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var dataInvStart = (from d in db.Franchisees
                                where d.PF_Code == strpfcode
                                select d.PF_Code).FirstOrDefault();

            string invstart1= strpfcode + "/2024-25/";
            if (invoice.invoiceno == null)
            {
                string Invoiceno = db.GSTInvoices.Where(m => m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "0";
                string[] invno = Invoiceno.Split('/');

                int num = Convert.ToInt32(invno[2]) + 1;

                invoice.invoiceno = invstart1 + num;
            }
            if (invoice.discount == "yes")
            {
                ViewBag.disc = invoice.discount;
            }
            if (ModelState.IsValid)
            {

                string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

                string comapnycheck = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault(); /// take dynamically
                if (comapnycheck == null)
                {
                    ModelState.AddModelError("comapnycheck", "Customer Id Does Not Exist");
                }

                GSTInvoice inv = db.GSTInvoices.Where(m => m.invoiceno == invoice.invoiceno && m.Pfcode == strpfcode).FirstOrDefault();


                if (inv != null)
                {
                    string bdatefrom = DateTime.ParseExact(invoice.Tempdatefrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
                    string bdateto = DateTime.ParseExact(invoice.TempdateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    string invdate = DateTime.ParseExact(invoice.tempInvoicedate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                    double netAmt = Convert.ToDouble(inv.netamount);

                    invoice.periodfrom = Convert.ToDateTime(bdatefrom);
                    invoice.periodto = Convert.ToDateTime(bdateto);
                    invoice.invoicedate = Convert.ToDateTime(invdate);

                    GSTInvoice invo = new GSTInvoice();
                    invo.IN_Id = inv.IN_Id;
                    invo.invoiceno = invoice.invoiceno;
                    invo.total = invoice.total;
                    invo.fullsurchargetax = invoice.fullsurchargetax;
                    invo.fullsurchargetaxtotal = invoice.fullsurchargetaxtotal;
                    invo.servicetax = 0;
                    invo.servicetaxtotal = 0;
                    invo.othercharge = invoice.othercharge;
                    invo.netamount = invoice.netamount;
                    invo.Customer_Id = invoice.Customer_Id;

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

                    invo.periodfrom = Convert.ToDateTime(bdatefrom);
                    invo.periodto = Convert.ToDateTime(bdateto);
                    invo.invoicedate = Convert.ToDateTime(invdate);
                    invo.Pfcode = strpfcode;
                    invo.isDelete = false;
                    invoice.Invoice_Lable = AmountTowords.changeToWords(invoice.netamount.ToString());
                    db.Entry(inv).State = EntityState.Detached;
                    db.Entry(invo).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Invoice Updated SuccessFully";

                    /////////////////// update consignment///////////////////////
                    using (var db = new db_a92afa_frbillingEntities())
                    {


                        //   Companies = db.Transactions.Where(m => m.Pf_Code == strpfcode && m.Customer_Id == invoice.Customer_Id && m.isDelete == false && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                        //Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no).ToList();

                        //   Companies.ForEach(m => m.status_t = invoice.invoiceno m.IsGSTConsignment = true);
                        //   db.SaveChanges();

                        var Companies = db.Transactions.Where(m => m.Pf_Code == strpfcode
                                         && m.Customer_Id == invoice.Customer_Id
                                         && m.isDelete == false
                                         && (m.status_t==null || m.status_t=="GST")
                                       
                                         && !db.singleinvoiceconsignments.Select(b => b.Consignment_no)
                                         .Contains(m.Consignment_no))
                            .ToList()
                            .Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0
                                        && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0)
                            .OrderBy(m => m.booking_date)
                            .ThenBy(n => n.Consignment_no)
                            .ToList();

                        Companies.ForEach(m => {
                            m.IsGSTConsignment = true;
                            m.status_t = "GST";
                        });
                        db.SaveChanges();
                        foreach (var i in Companies.Select(x => x.Consignment_no))
                        {
                            GSTInvoiceConsignment upsc = db.GSTInvoiceConsignments.Where(m => m.Consignment_no == i).FirstOrDefault();

                            if (upsc == null)
                            {

                                GSTInvoiceConsignment sc = new GSTInvoiceConsignment();

                                sc.Consignment_no = i.Trim();
                                sc.InvoiceNo = invoice.invoiceno;
                                sc.Pfcode = strpfcode;
                                db.GSTInvoiceConsignments.Add(sc);
                                db.SaveChanges();

                            }
                            else
                            {
                                upsc.InvoiceNo = invoice.invoiceno;
                                db.Entry(upsc).State = EntityState.Modified;
                                db.SaveChanges();

                            }



                        }
                    }
                    string lastInvoiceno = db.GSTInvoices.Where(m => m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "0";
                    string[] strarrinvno = lastInvoiceno.Split('/');

                    int number = Convert.ToInt32(strarrinvno[2]) + 1;
                    //  ViewBag.lastInvoiceno = invstart1 + "/" + number;
                    //  ViewBag.nextinvoice = GetmaxInvoiceno(invstart, strpfcode);
                    ViewBag.nextinvoice = invstart1 + number;
                    ///////////////////end of update consignment///////////////////////
                }
                else
                {

                    var invoi = db.GSTInvoices.Where(m => m.tempInvoicedate == invoice.tempInvoicedate && m.Customer_Id == invoice.Customer_Id && m.Pfcode == invoice.Pfcode && m.isDelete == false).FirstOrDefault();

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





                    invoice.invoiceno = invoice.invoiceno;

                    GSTInvoice invo = new GSTInvoice();

                    invo.invoiceno = invoice.invoiceno;
                    invo.total = invoice.total;
                    invo.fullsurchargetax = invoice.fullsurchargetax;
                    invo.fullsurchargetaxtotal = invoice.fullsurchargetaxtotal;
                    invo.servicetax = 0;
                    invo.servicetaxtotal = 0;
                    invo.othercharge = invoice.othercharge;
                    invo.netamount = invoice.netamount;
                    invo.Customer_Id = invoice.Customer_Id;

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

                    invo.isDelete = false;
                    invo.periodfrom = Convert.ToDateTime(bdatefrom);
                    invo.periodto = Convert.ToDateTime(bdateto);
                    invo.invoicedate = Convert.ToDateTime(invdate);
                    invo.Pfcode = strpfcode;



                    db.GSTInvoices.Add(invo);
                    db.SaveChanges();

                    ViewBag.success = "Invoice Added SuccessFully";


                    /////////////////// update consignment///////////////////////
                    using (var db = new db_a92afa_frbillingEntities())
                    {
                        var Companies = db.Transactions.Where(m => m.Pf_Code == strpfcode
                                     && m.Customer_Id == invoice.Customer_Id
                                     && m.isDelete == false
                                     && (m.status_t == null || m.status_t == "GST")
                                     && m.IsGSTConsignment==true
                                     && !db.singleinvoiceconsignments.Select(b => b.Consignment_no)
                                     .Contains(m.Consignment_no))
                        .ToList()
                        .Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0
                                    && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0)
                        .OrderBy(m => m.booking_date)
                        .ThenBy(n => n.Consignment_no)
                        .ToList();

                        Companies.ForEach(m => {
                            m.IsGSTConsignment = true;
                            m.status_t = "GST";
                        });
                        db.SaveChanges();
                        foreach (var i in Companies.Select(x => x.Consignment_no))
                        {
                            GSTInvoiceConsignment upsc = db.GSTInvoiceConsignments.Where(m => m.Consignment_no == i).FirstOrDefault();

                            if (upsc == null)
                            {

                                GSTInvoiceConsignment sc = new GSTInvoiceConsignment();

                                sc.Consignment_no = i.Trim();
                                sc.InvoiceNo = invoice.invoiceno;
                                sc.Pfcode = strpfcode;
                                db.GSTInvoiceConsignments.Add(sc);
                                db.SaveChanges();

                            }



                        }
                    }


                    ///////////////////end of update consignment///////////////////////
                    ///
                    string lastInvoiceno = db.GSTInvoices.Where(m => m.Pfcode == strpfcode).OrderByDescending(m => m.IN_Id).Take(1).Select(m => m.invoiceno).FirstOrDefault() ?? invstart1 + "0";
                    string[] strarrinvno = lastInvoiceno.Split('/');

                    int number = Convert.ToInt32(strarrinvno[2]) + 1;
                    //  ViewBag.lastInvoiceno = invstart1 + "/" + number;
                    //  ViewBag.nextinvoice = GetmaxInvoiceno(invstart, strpfcode);
                    ViewBag.nextinvoice = invstart1 + number;
                    //ViewBag.nextinvoice = GetmaxInvoiceno(invstart1, strpfcode);

                }
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
               Request.ApplicationPath.TrimEnd('/') + "/";
                string Pfcode = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Pf_code).FirstOrDefault();


                if (Pfcode != null)
                {
                    LocalReport lr = new LocalReport();


                    var dataset = db.TransactionViews.Where(m => m.Pf_Code == strpfcode && m.IsGSTConsignment==true && db.GSTInvoiceConsignments.Where(x=>x.InvoiceNo==invoice.invoiceno).Select(b => b.Consignment_no).Contains(m.Consignment_no) && m.Customer_Id == invoice.Customer_Id && !db.singleinvoiceconsignments.Select(b => b.Consignment_no).Contains(m.Consignment_no)).ToList().
                 Where(x => DateTime.Compare(x.booking_date.Value.Date, invoice.periodfrom.Value.Date) >= 0 && DateTime.Compare(x.booking_date.Value.Date, invoice.periodto.Value.Date) <= 0).OrderBy(m => m.booking_date).ThenBy(n => n.Consignment_no)
                               .ToList();

                    var franchisee = db.Franchisees.Where(x => x.PF_Code == Pfcode);
                    franchisee.FirstOrDefault().LogoFilePath = (franchisee.FirstOrDefault().LogoFilePath == null || franchisee.FirstOrDefault().LogoFilePath == "") ? baseUrl + "/assets/Dtdclogo.png" : franchisee.FirstOrDefault().LogoFilePath;

                    var dataset3 = db.GSTInvoices.Where(m => m.invoiceno == invoice.invoiceno && m.Pfcode == strpfcode);

                    var dataset4 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);
                    dataset3.FirstOrDefault().Invoice_Lable = AmountTowords.changeToWords(dataset3.FirstOrDefault().netamount.ToString());
                    string clientGst = dataset4.FirstOrDefault().Gst_No;
                    string frgst = franchisee.FirstOrDefault().GstNo;

                    franchisee.FirstOrDefault().StampFilePath = (franchisee.FirstOrDefault().StampFilePath == null || franchisee.FirstOrDefault().StampFilePath == "") ? baseUrl + "/assets/Dtdclogo.png" : franchisee.FirstOrDefault().StampFilePath;
                    string discount = dataset3.FirstOrDefault().discount;


                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "InvoiceWithoutGST.rdlc");


                    lr.ReportPath = path;


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
                        ViewBag.invoiceno = invoice.invoiceno.Replace("/", "-");
                        ViewBag.strpfcode = strpfcode;
                    }

                    var pdfPath = Server.MapPath("~/PDF/" + strpfcode+"/GSTInvoice");
                    // Check if the directory exists
                    if (!Directory.Exists(pdfPath))
                    {
                        // Create the directory if it doesn't exist
                        Directory.CreateDirectory(pdfPath);
                    }
                    var invoicefile = dataset3.FirstOrDefault().invoiceno.Replace("/", "-") + ".pdf";
                    string savePath = Path.Combine(pdfPath, invoicefile);

                    ViewBag.savePath = savePath;
                    ViewBag.url = baseUrl + "/PDF/" + strpfcode + "/GSTInvoice/" + invoicefile;

                    using (FileStream stream = new FileStream(savePath, FileMode.Create))
                    {
                        stream.Write(renderByte, 0, renderByte.Length);
                    }


                }
                else
                {
                    TempData["NUllCustomer"] = "Customer Id Does not Exists";
                    ViewBag.success = null;
                }


                ModelState.Clear();
                return PartialView("GenerateInvoiceWithoutGST", invoice);

            }
            return PartialView("GenerateInvoiceWithoutGST", invoice);
        }


        public ActionResult DownloadGSTInvoice(long id)
        {
            var pfcode = Request.Cookies["cookies"]["AdminValue"].ToString();

            var invoice = db.GSTInvoices.Where(m => m.IN_Id == id && m.Pfcode == pfcode).FirstOrDefault();
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                 Request.ApplicationPath.TrimEnd('/') + "/";
            var pdfPath = Server.MapPath("~/PDF/" + pfcode+ "/GSTInvoice/");
            var filename = invoice.invoiceno.Replace("/", "-") + ".pdf";
            string savePath = Path.Combine(pdfPath, filename);
            if (invoice != null)
            {
                if (System.IO.File.Exists(savePath))
                {

                    savePath = baseUrl + "/PDF/" + pfcode + "/GSTInvoice/" + invoice.invoiceno.Replace("/", "-") + ".pdf";
                    return Redirect(savePath);
                }
               
            }
            return Redirect("ViewInvoiceWithoutGST");

        }
        [HttpGet]
        public ActionResult GetInvoiceDataForDashboard()
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

         //   var InviceData = db.Invoices.Where(x => x.Pfcode == pfcode).ToList();
         
            DateTime today = DateTime.Now;

            DateTime yearBack = today.AddYears(-1);

            
            var monthsInRange = Enumerable.Range(0, 12).Select(i => yearBack.AddMonths(i)).ToList();


            var invoices = (from t in db.Invoices
                            where t.Pfcode == pfcode
                                  && t.isDelete == false
                                  && SqlFunctions.DatePart("Month",t.invoicedate)==today.Month
                            select t).ToList();

            // Calculate the data for InvoiceDataForDashBoard
            var invoiceDashboardData = new InvoiceDataForDashBoard
            {
                Paid = invoices.Where(t => t.netamount==t.paid).Sum(t => t.netamount) ?? 0,
                Unpaid = invoices.Where(t => t.paid==null).Sum(t => t.netamount) ?? 0,
                TotalInvoice = invoices.Count,
                PaidCount = invoices.Count(t => t.netamount==t.paid),
                UnpaidCount = invoices.Count(t => t.paid==null),
                TotalNetAmount = invoices.Sum(t => t.netamount) ?? 0,
                PattialPaid = invoices.Where(t => t.paid>0 && t.paid<t.netamount).Sum(t => t.netamount) ?? 0,
                Pattialpaidcount = invoices.Count(t => t.paid > 0 && t.paid < t.netamount)
            };

            

            // Serialize the data points for use in the view
            ViewBag.DataPoints = JsonConvert.SerializeObject(invoiceDashboardData);

            return View();
        }

        public ActionResult DeleteInvoice(int invoiceid, string invfromdate, string Companydetails, string invtodate, string invoiceNo)
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
                var checkInvoiceNo = db.Invoices.Where(x => x.IN_Id == invoiceid && x.Pfcode == pfcode).FirstOrDefault();
                if (checkInvoiceNo == null)
                {
                    TempData["error"] = "Invalid Invoice No";
                //    public ActionResult ViewInvoice(string invfromdate, string Companydetails, string invtodate, string invoiceNo)

                return RedirectToAction("ViewInvoice", new { invfromdate = invfromdate, invtodate = invtodate, invoiceNo ="", Companydetails=Companydetails });
                }
         

                //db.Invoices.Remove(checkInvoiceNo);
                checkInvoiceNo.isDelete = true;
                db.Entry(checkInvoiceNo).State = EntityState.Modified;


                db.SaveChanges();
                TempData["success"] = checkInvoiceNo.invoiceno + " Delete successfully!";

            return RedirectToAction("ViewInvoice", new { invfromdate = invfromdate, invtodate = invtodate, invoiceNo = "", Companydetails = Companydetails });


        }
        public ActionResult DeleteSingleInvoice(int invoiceid, string invfromdate, string Companydetails, string invtodate, string invoiceNo)
        {
            string pfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var checkInvoiceNo = db.Invoices.Where(x => x.IN_Id == invoiceid && x.Pfcode == pfcode).FirstOrDefault();
            if (checkInvoiceNo == null)
            {
                TempData["error"] = "Invalid Invoice No";

             //   public ActionResult ViewSingleInvoice(string invfromdate, string invtodate, string Companydetails, string invoiceNo)


                return RedirectToAction("ViewSingleInvoice", new { invfromdate = invfromdate, invtodate = invtodate, invoiceNo = "", Companydetails=Companydetails });
            }


            db.Invoices.Remove(checkInvoiceNo);
            db.SaveChanges();

            //checkInvoiceNo.isDelete=true;
            //  db.Entry(checkInvoiceNo).State = EntityState.Modified;
            var signle = db.singleinvoiceconsignments.Where(x => x.Invoice_no == invoiceNo).ToList();
            foreach (var i in signle)
            {
                db.singleinvoiceconsignments.Remove(i);
                db.SaveChanges();
            }

            TempData["success"] = "Invoice Number " + invoiceNo + "  Deleted successfully";
         

            return RedirectToAction("ViewSingleInvoice", new { invfromdate = invfromdate, invtodate = invtodate, invoiceNo = "", Companydetails=Companydetails });


        }
    }
}