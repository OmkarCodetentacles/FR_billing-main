using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Microsoft.Reporting.WebForms;

namespace DtDc_Billing.Controllers
{

    [SessionAdminold]
    [SessionUserModule]
    public class PaymentController : Controller
    {
        private db_a71c08_elitetokenEntities db = new db_a71c08_elitetokenEntities();
        // GET: Payment
        public ActionResult InvoicePaymentList()
        {

            List<PaymentModel> list = new List<PaymentModel>();
            ViewBag.Cash = new Cash();
            ViewBag.Cheque = new Cheque();
            ViewBag.Neft = new NEFT();
            ViewBag.Credit = new CreditNote();

            //var transactions = db.Invoices.AsEnumerable();

            //return View(transactions.ToList());
            return View(list);
        }

        [HttpPost]
        public ActionResult InvoicePaymentList(string status)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            ViewBag.status = status;

            ViewBag.Cash = new Cash();
            ViewBag.Cheque = new Cheque();
            ViewBag.Neft = new NEFT();
            ViewBag.Credit = new CreditNote();

            var obj = db.getPayment(status, strpf).Select(x => new PaymentModel
            {
                invoiceno = x.invoiceno,
                total = x.total,
                fullsurchargetax = x.fullsurchargetax,
                fullsurchargetaxtotal = x.fullsurchargetaxtotal,
                servicetax = x.servicetax,
                servicetaxtotal = x.servicetaxtotal,
                othercharge = x.othercharge,
                netamount = x.netamount,
                Firm_Id = x.Firm_Id,
                Customer_Id = x.Customer_Id,
                paid = x.paid ?? 0,
                tempInvoicedate = x.tempInvoicedate,
                Royalty_charges = x.Royalty_charges,
                Docket_charges = x.Docket_charges,
                Balance = Convert.ToInt32(x.netamount) - Convert.ToInt32((x.paid ?? 0))
                // discount = x.discount,
                // totalCount = x.totalCount ?? 0
            }).ToList();



            return View(obj);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cash(Cash cash)
        {
            if (ModelState.IsValid)
            {
                string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

                var cashb = db.Invoices.Where(m => m.invoiceno == cash.Invoiceno && m.Pfcode == strpf).FirstOrDefault();

                double balance = Math.Round(Convert.ToDouble(cashb.netamount)) - Convert.ToDouble(cashb.paid);

                if (cash.C_Total_Amount > balance)
                {
                    ModelState.AddModelError("InvAmt", "Amount Is Greater Than Balance");
                }
                else
                {
                    cashb.paid = Convert.ToDouble(cashb.paid) + Convert.ToDouble(cash.C_Total_Amount);
                    db.Entry(cashb).State = EntityState.Modified;
                    cash.Pfcode = strpf;
                    db.Cashes.Add(cash);
                    db.SaveChanges();

                    return Json(new { RedirectUrl = Url.Action("InvoicePaymentList") });
                }
            }

            return PartialView("CashPartial", cash);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cheque(Cheque cheque)
        {
            if (ModelState.IsValid)
            {
                string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

                var cashb = db.Invoices.Where(m => m.invoiceno == cheque.Invoiceno && m.Pfcode == strpf).FirstOrDefault();

                double balance = Math.Round(Convert.ToDouble(cashb.netamount)) - Convert.ToDouble(cashb.paid);

                if (cheque.totalAmount > balance)
                {
                    ModelState.AddModelError("InvAmt", "Amount Is Greater Than Balance");
                }
                else
                {
                    cashb.paid = Convert.ToDouble(cashb.paid) + Convert.ToDouble(cheque.totalAmount);
                    cheque.Pfcode = strpf;
                    db.Entry(cashb).State = EntityState.Modified;


                    db.Cheques.Add(cheque);
                    db.SaveChanges();
                    return Json(new { RedirectUrl = Url.Action("InvoicePaymentList") });
                }
            }

            return PartialView("ChequePartial", cheque);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Neft(NEFT nEFT)
        {
            if (ModelState.IsValid)
            {
                string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

                var cashb = db.Invoices.Where(m => m.invoiceno == nEFT.Invoiceno && m.Pfcode == strpf).FirstOrDefault();

                double balance = Math.Round(Convert.ToDouble(cashb.netamount)) - Convert.ToDouble(cashb.paid);

                if (nEFT.N_Total_Amount > balance)
                {
                    ModelState.AddModelError("InvAmt", "Amount Is Greater Than Balance");
                }
                else
                {
                    cashb.paid = Convert.ToDouble(cashb.paid) + Convert.ToDouble(nEFT.N_Total_Amount);
                    db.Entry(cashb).State = EntityState.Modified;
                    nEFT.Pfcode = strpf;
                    db.NEFTs.Add(nEFT);
                    db.SaveChanges();
                    return Json(new { RedirectUrl = Url.Action("InvoicePaymentList") });
                }
            }

            return PartialView("NeftPartial", nEFT);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreditNote(CreditNote creditNote)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var cashb = db.Invoices.Where(m => m.invoiceno == creditNote.Invoiceno && m.Pfcode == strpf).FirstOrDefault();

            //TempData["Firmid"] = Firm_Id;
            TempData["Customer_Id"] = cashb.Customer_Id;
         
            if (ModelState.IsValid)
            {


                double balance = Math.Round(Convert.ToDouble(cashb.netamount)) - Convert.ToDouble(cashb.paid);

                if (creditNote.Cr_Amount > balance)
                {
                    ModelState.AddModelError("InvAmt", "Amount Is Greater Than Balance");
                }
                else
                {
                    cashb.paid = Convert.ToDouble(cashb.paid) + Convert.ToDouble(creditNote.Cr_Amount);
                    creditNote.Creditnoteno = "MH" + RandomString(8);
                    db.Entry(cashb).State = EntityState.Modified;
                    creditNote.Pfcode = strpf;
                    db.CreditNotes.Add(creditNote);
                    db.SaveChanges();





                    LocalReport lr = new LocalReport();

                var DataSet1 = db.Franchisees.Where(x => x.Firm_Id == cashb.Firm_Id);

                var DataSet2 = db.Invoices.OrderByDescending(m => m.invoiceno == cashb.invoiceno);

                var DataSet3 = db.Companies.Where(m => m.Company_Id == cashb.Customer_Id);

                    DataSet2.FirstOrDefault().Amount4_Lable = AmountTowords.changeToWords(creditNote.Cr_Amount.ToString());
                    DataSet2.FirstOrDefault().Address = creditNote.Creditnoteno;
                    DataSet2.FirstOrDefault().Tempdatefrom = creditNote.tempch_date.ToString().Replace("-","/");
                    DataSet2.FirstOrDefault().Amount4 = creditNote.Cr_Amount;


                string path = Path.Combine(Server.MapPath("~/RdlcReport"), "ReceiptCreditNote.rdlc");

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }



                lr.EnableExternalImages = true;
                ReportDataSource rd = new ReportDataSource("DataSet1", DataSet1);
                ReportDataSource rd1 = new ReportDataSource("DataSet2", DataSet2);
                ReportDataSource rd2 = new ReportDataSource("DataSet3", DataSet3);

                lr.DataSources.Add(rd);
                lr.DataSources.Add(rd1);
                lr.DataSources.Add(rd2);


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
                    //return File(renderByte, mimeType);


                    string savePath = Server.MapPath("~/PDF/" + DataSet2.FirstOrDefault().Customer_Id + "_CreditNoteReceipt" + ".pdf");

                    using (FileStream stream = new FileStream(savePath, FileMode.Create))
                    {
                        stream.Write(renderByte, 0, renderByte.Length);
                    }
                    //return File(renderByte, mimeType);
                }

                TempData["success"] = "pdf";
                return Json(new { RedirectUrl = Url.Action("InvoicePaymentList") });
            }
            return PartialView("CreditNotePartial", creditNote);
        }


        public string PrintReceipt(long id)
        {
            var creditnote = db.CreditNotes.Where(m => m.Cr_id == id).FirstOrDefault();
            var invoice = db.Invoices.Where(m => m.invoiceno == creditnote.Invoiceno.Trim()).FirstOrDefault();
            LocalReport lr = new LocalReport();

            var DataSet1 = db.Franchisees.Where(x => x.Firm_Id == invoice.Firm_Id);

            var DataSet2 = db.Invoices.OrderByDescending(m => m.invoiceno == creditnote.Invoiceno);

            var DataSet3 = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id);

           
            DataSet2.FirstOrDefault().Amount4_Lable = AmountTowords.changeToWords(creditnote.Cr_Amount.ToString());
            DataSet2.FirstOrDefault().Address = creditnote.Creditnoteno;
            DataSet2.FirstOrDefault().Tempdatefrom = creditnote.tempch_date.ToString().Replace("-", "/");
            DataSet2.FirstOrDefault().Amount4 = creditnote.Cr_Amount;

            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "ReceiptCreditNote.rdlc");

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }



            lr.EnableExternalImages = true;
            ReportDataSource rd = new ReportDataSource("DataSet1", DataSet1);
            ReportDataSource rd1 = new ReportDataSource("DataSet2", DataSet2);
            ReportDataSource rd2 = new ReportDataSource("DataSet3", DataSet3);

            lr.DataSources.Add(rd);
            lr.DataSources.Add(rd1);
            lr.DataSources.Add(rd2);


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
            //return File(renderByte, mimeType);


            string savePath = Server.MapPath("~/PDF/" + DataSet2.FirstOrDefault().Customer_Id + "_CreditNoteReceipt" + ".pdf");

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                stream.Write(renderByte, 0, renderByte.Length);
            }
            return DataSet2.FirstOrDefault().Customer_Id + "_CreditNoteReceipt" + ".pdf";
        }

        public ActionResult AddCodPayment()
        {
            ViewBag.Cod = new addcodamount();

            ViewBag.Codlist = new List<TransactionView>();


            return View();
        }




        [HttpPost]
        public ActionResult CodSearch(string Custid)
        {
            ViewBag.Custid = Custid;

            List<TransactionView> transactions = (from u in db.TransactionViews
                                                  where u.cod == "yes" &&
                                                  !db.addcodamounts.Any(f => f.consinment_no == u.Consignment_no)
                                                  select u).ToList();

            return PartialView("CodSearchPartial", transactions);


        }

        [HttpPost]
        public ActionResult EditCod(addcodamount addcodamount)
        {



            if (ModelState.IsValid)
            {
                db.addcodamounts.Add(addcodamount);
                db.SaveChanges();

                ViewBag.Message = "Cod Payment Added SuccessFully";

                return PartialView("EditCodPartial");
            }

            return PartialView("EditCodPartial", addcodamount);
        }



        public ActionResult AddTopayPayment()
        {
            ViewBag.Topay = new addtopayamount();

            ViewBag.Codlist = new List<TransactionView>();


            return View();
        }



        [HttpPost]
        public ActionResult TopaySearch(string Custid)
        {
            ViewBag.Custid = Custid;

            List<TransactionView> transactions = (from u in db.TransactionViews
                                                  where u.topay == "yes" &&
                                                  !db.addtopayamounts.Any(f => f.consinmentno == u.Consignment_no)
                                                  select u).ToList();

            return PartialView("TopaySearchPartial", transactions);


        }

        [HttpPost]
        public ActionResult EditTopay(addtopayamount addtopayamount)
        {



            if (ModelState.IsValid)
            {
                db.addtopayamounts.Add(addtopayamount);
                db.SaveChanges();

                ViewBag.Message = "Topay Payment Added SuccessFully";

                return PartialView("EditTopayPartial");
            }

            return PartialView("EditTopayPartial", addtopayamount);
        }

        public ActionResult CustomerIdAutocomplete()
        {

            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            var entity = db.Companies.
Select(e => new
{
    e.Company_Id,
    e.Pf_code
}).Where(d=>d.Pf_code== strpfcode).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PaymentTrack()
        {
            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();
            List<PaymentTrack> track = new List<PaymentTrack>();

            var cash = (from inv in db.Invoices
                        join ca in db.Cashes on inv.invoiceno equals ca.Invoiceno
                        where  inv.Pfcode == ca.Pfcode
                        && inv.Pfcode== strpfcode
                        select new PaymentTrack
                        {
                            Amount = ca.C_Total_Amount,
                            Invoiceno = ca.Invoiceno,
                            invoicedate = inv.invoicedate,
                            Customer_Id = inv.Customer_Id,
                            Modeofpayment = "Cash",
                            netamount = inv.netamount,
                            tempinserteddate = ca.tempinserteddate,
                            id = ca.Cash_id,
                            remark="",
                            

                        }).ToList();

            var cheque = (from inv in db.Invoices
                          join ch in db.Cheques on inv.invoiceno equals ch.Invoiceno
                          where inv.Pfcode == ch.Pfcode
                           && inv.Pfcode == strpfcode
                          select new PaymentTrack
                          {
                              Amount = ch.totalAmount,
                              Invoiceno = ch.Invoiceno,
                              invoicedate = inv.invoicedate,
                              Customer_Id = inv.Customer_Id,
                              Modeofpayment = "Cheque",
                              netamount = inv.netamount,
                              tempinserteddate = ch.tempch_date,
                              id = ch.Cheque_id,
                              remark=ch.branch_Name,
                              
                          }).ToList();

            var NEFT = (from inv in db.Invoices
                        join ne in db.NEFTs on inv.invoiceno equals ne.Invoiceno
                        where inv.Pfcode == ne.Pfcode
                         && inv.Pfcode == strpfcode
                        select new PaymentTrack
                        {
                            Amount = ne.N_Total_Amount,
                            Invoiceno = ne.Invoiceno,
                            invoicedate = inv.invoicedate,
                            Customer_Id = inv.Customer_Id,
                            Modeofpayment = "NEFT",
                            netamount = inv.netamount,
                            tempinserteddate = ne.tempneftdate,
                            id = ne.Neft_id,
                            remark=ne.Transaction_Id,
                           
                        }).ToList();

            var CreditNote = (from inv in db.Invoices
                              join cn in db.CreditNotes on inv.invoiceno equals cn.Invoiceno
                              where inv.Pfcode == cn.Pfcode
                               && inv.Pfcode == strpfcode
                              select new PaymentTrack
                              {
                                  Amount = cn.Cr_Amount,
                                  Invoiceno = cn.Invoiceno,
                                  invoicedate = inv.invoicedate,
                                  Customer_Id = inv.Customer_Id,
                                  Modeofpayment = "CreditNote",
                                  netamount = inv.netamount,
                                  tempinserteddate = cn.tempch_date,
                                  id = cn.Cr_id,
                                  remark=cn.Creditnoteno,
                                  
                              }).ToList();

            track.AddRange(cash);
            track.AddRange(cheque);
            track.AddRange(NEFT);
            track.AddRange(CreditNote);



            return View(track.OrderBy(m => m.Invoiceno).ToList());
        }


        [HttpPost]
        public ActionResult PaymentTrack(string Custid,DateTime Fromdatetime, DateTime ToDatetime)
        {
            List<PaymentTrack> track = new List<PaymentTrack>();

            string strpfcode = Request.Cookies["Cookies"]["AdminValue"].ToString();

            ViewBag.fromdate = Fromdatetime.ToString("MM/dd/yyyy");
            ViewBag.todate = ToDatetime.ToString("MM/dd/yyyy");
            ViewBag.Custid = Custid;
          

            string[] formats = {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                   "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"};
     

                var objCash = db.getPaymentTrackCash(Fromdatetime, ToDatetime, Custid, strpfcode).Select(x => new PaymentTrack
                {


                    Amount = x.Amount,
                    Invoiceno = x.Invoiceno,
                    invoicedate = x.invoicedate,
                    Customer_Id = x.Customer_Id,
                    Modeofpayment = x.Modeofpayment,
                    netamount = x.netamount,
                    tempinserteddate = x.tempinserteddate,
                    id = x.id,
                    remark = x.remark,
                    

                }).ToList();

            var objCheque = db.getPaymentTrackCheque(Fromdatetime,ToDatetime, Custid, strpfcode).Select(x => new PaymentTrack
            {


                Amount = x.Amount,
                Invoiceno = x.Invoiceno,
                invoicedate = x.invoicedate, 
                Customer_Id = x.Customer_Id,
                Modeofpayment = x.Modeofpayment,
                netamount = x.netamount,
                tempinserteddate = x.tempinserteddate,
                id = x.id,
                remark = x.remark,
               
            }).ToList();

            var objNeft = db.getPaymentTrackNEFT(Fromdatetime,ToDatetime, Custid, strpfcode).Select(x => new PaymentTrack
            {


                Amount = x.Amount,
                Invoiceno = x.Invoiceno,
                invoicedate = x.invoicedate,
                Customer_Id = x.Customer_Id,
                Modeofpayment = x.Modeofpayment,
                netamount = x.netamount,
                tempinserteddate = x.tempinserteddate,
                id = x.id,
                remark = x.remark,
              
            }).ToList();

            var objcr = db.getPaymentTrackCreditNote(Fromdatetime, ToDatetime, Custid, strpfcode).Select(x => new PaymentTrack
            {


                Amount = x.Amount,
                Invoiceno = x.Invoiceno,
                invoicedate = x.invoicedate,
                Customer_Id = x.Customer_Id,
                Modeofpayment = x.Modeofpayment,
                netamount = x.netamount,
                tempinserteddate = x.tempinserteddate,
                id = x.id,
                remark = x.remark,
              

            }).ToList();


            track.AddRange(objCash);
            track.AddRange(objCheque);
            track.AddRange(objNeft);
            track.AddRange(objcr);
            return View(track);
          

            //return View();



        }

        public ActionResult CashEdit(int id)
        {
            var obj = db.Cashes.Where(m => m.Cash_id == id).FirstOrDefault();
            var inv = db.Invoices.Where(m => m.invoiceno == obj.Invoiceno && m.Pfcode == obj.Pfcode).FirstOrDefault();
            ViewBag.netamount = inv.netamount;
            ViewBag.existingvalue = obj.C_Total_Amount;
            ViewBag.paidamt = inv.paid;
            ViewBag.ctotalamt = obj.C_Total_Amount;
            //ViewBag.firmid = Firm_Id;
            return View(obj);
        }

        [HttpPost]
        public ActionResult CashEdit(Cash cash, double amountval)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

           
            var invoicedetails = db.Invoices.Where(m => m.invoiceno == cash.Invoiceno && m.Pfcode == strpf).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (cash.C_Total_Amount > 0)
                {
                    if (cash.C_Total_Amount <= amountval + (invoicedetails.netamount - invoicedetails.paid))
                    {

                        string[] formats = { "dd-MM-yyyy" };

                        //string bdate = DateTime.ParseExact(Convert.ToString(cash.tempinserteddate), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        //cash.inserteddate = Convert.ToDateTime(bdate);
                        cash.Pfcode = strpf;
                        db.Entry(cash).State = EntityState.Modified;
                        db.SaveChanges();
                        invoicedetails.paid = invoicedetails.paid - amountval + cash.C_Total_Amount;

                        db.Entry(invoicedetails).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Updatedsuccss"] = "Updated successfully!";

                    }
                }
            }
            ViewBag.netamount = invoicedetails.netamount;
            ViewBag.existingvalue = amountval;
            ViewBag.paidamt = invoicedetails.paid;
            ViewBag.firmid = cash.Firm_Id;


            return View();
        }

        public ActionResult ChequeEdit(int id)
        {
            var obj = db.Cheques.Where(m => m.Cheque_id == id).FirstOrDefault();
            var inv = db.Invoices.Where(m => m.invoiceno == obj.Invoiceno && m.Pfcode==obj.Pfcode).FirstOrDefault();
            ViewBag.netamount = inv.netamount;
            ViewBag.existingvalue = obj.totalAmount;
            ViewBag.paidamt = inv.paid;
            ViewBag.ctotalamt = obj.totalAmount;
            //ViewBag.firmid = Firm_Id;
            return View(obj);
        }

        [HttpPost]
        public ActionResult ChequeEdit(Cheque cheque, double amountval)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var invoicedetails = db.Invoices.Where(m => m.invoiceno == cheque.Invoiceno && m.Pfcode == strpf).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (cheque.totalAmount > 0)
                {
                    if (cheque.totalAmount <= amountval + (invoicedetails.netamount - invoicedetails.paid))
                    {

                        string[] formats = { "dd-MM-yyyy" };

                        //string bdate = DateTime.ParseExact(cheque.tempch_date.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        //cheque.ch_date = Convert.ToDateTime(bdate);
                        cheque.Pfcode = strpf;
                        db.Entry(cheque).State = EntityState.Modified;
                        db.SaveChanges();
                        invoicedetails.paid = invoicedetails.paid - amountval + cheque.totalAmount;

                        db.Entry(invoicedetails).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Updatedsuccss"] = "Updated successfully!";

                    }
                }
            }
            ViewBag.netamount = invoicedetails.netamount;
            ViewBag.existingvalue = amountval;
            ViewBag.paidamt = invoicedetails.paid;
            ViewBag.firmid = cheque.Firm_Id;

            return View();
        }

        [HttpGet]
        public ActionResult NEFTEdit(int id)
        {
            var obj = db.NEFTs.Where(m => m.Neft_id == id).FirstOrDefault();
            var inv = db.Invoices.Where(m => m.invoiceno == obj.Invoiceno && m.Pfcode == obj.Pfcode).FirstOrDefault();
            ViewBag.netamount = inv.netamount;
            ViewBag.existingvalue = obj.N_Total_Amount;
            ViewBag.paidamt = inv.paid;
            ViewBag.ctotalamt = obj.N_Total_Amount;
            //ViewBag.firmid = Firm_Id;
            return View(obj);
        }

        [HttpPost]
        public ActionResult NEFTEdit(NEFT nEFT, double amountval)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var invoicedetails = db.Invoices.Where(m => m.invoiceno == nEFT.Invoiceno && m.Pfcode== strpf).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (nEFT.N_Total_Amount > 0)
                {
                    if (nEFT.N_Total_Amount <= amountval + (invoicedetails.netamount - invoicedetails.paid))
                    {

                        string[] formats = { "dd-MM-yyyy" };

                        //string bdate = DateTime.ParseExact(nEFT.tempneftdate.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        //nEFT.neftdate = Convert.ToDateTime(bdate);
                        nEFT.Pfcode = strpf;

                        db.Entry(nEFT).State = EntityState.Modified;
                        db.SaveChanges();
                        invoicedetails.paid = invoicedetails.paid - amountval + nEFT.N_Total_Amount;
                        db.Entry(invoicedetails).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Updatedsuccss"] = "Updated successfully!";

                    }
                }
            }
            ViewBag.netamount = invoicedetails.netamount;
            ViewBag.existingvalue = amountval;
            ViewBag.paidamt = invoicedetails.paid;
            ViewBag.firmid = nEFT.Firm_Id;

            return View();
        }


        public ActionResult CreditEdit(int id)
        {
            var obj = db.CreditNotes.Where(m => m.Cr_id == id).FirstOrDefault();
            var inv = db.Invoices.Where(m => m.invoiceno == obj.Invoiceno && m.Pfcode==obj.Pfcode).FirstOrDefault();
            ViewBag.netamount = inv.netamount;
            ViewBag.existingvalue = obj.Cr_Amount;
            ViewBag.paidamt = inv.paid;
            ViewBag.ctotalamt = obj.Cr_Amount;
            //ViewBag.firmid = Firm_Id;
            return View(obj);
        }

        [HttpPost]
        public ActionResult CreditEdit(CreditNote credit, double amountval)
        {
            string strpf = Request.Cookies["Cookies"]["AdminValue"].ToString();

            var invoicedetails = db.Invoices.Where(m => m.invoiceno == credit.Invoiceno && m.Pfcode == strpf).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (credit.Cr_Amount > 0)
                {
                    if (credit.Cr_Amount <= amountval + (invoicedetails.netamount - invoicedetails.paid))
                    {

                        string[] formats = { "dd-MM-yyyy" };

                        //string bdate = DateTime.ParseExact(credit.tempch_date.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");

                        //credit.cr_date = Convert.ToDateTime(bdate);
                        credit.Pfcode = strpf;

                        db.Entry(credit).State = EntityState.Modified;
                        db.SaveChanges();
                        invoicedetails.paid = invoicedetails.paid - amountval + credit.Cr_Amount;
                        db.Entry(invoicedetails).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Updatedsuccss"] = "Updated successfully!";

                    }
                }
            }
            ViewBag.netamount = invoicedetails.netamount;
            ViewBag.existingvalue = amountval;
            ViewBag.paidamt = invoicedetails.paid;

            return View();
        }
    }
}