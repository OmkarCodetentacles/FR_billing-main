using DtDc_Billing.Entity_FR;
using DtDc_Billing.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionUserModule]
    public class CashBookingController : Controller
    {
        db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        //Add_History add_History = new Add_History();
        // GET: Booking
        [SessionTimeout]
        [SessionUserModule]
        public ActionResult Printreceipt()
        {
            return View();
        }

        [SessionTimeout]
        [HttpPost]
        public ActionResult Printreceipt(Receipt_details reciept_Details, string Submit, bool? DisableDisc)
        {

            var consignmentno = reciept_Details.Consignment_No;



            ////credit Amount logic///

            reciept_Details.Paid_Amount =  Convert.ToInt16(reciept_Details.Credit_Amount);
           
            /////////////////////////


            ViewBag.Insuarance = reciept_Details.Shipmenttype;
            ViewBag.yesinsurance = reciept_Details.Insurance;

            var consignment = db.Receipt_details.Where(m => m.Consignment_No == reciept_Details.Consignment_No).FirstOrDefault();

            if (consignment != null)
            {
                ModelState.AddModelError("Consignment", "Consignment Allready Exist");
            }






            string savePath = "";

            if (ModelState.IsValid)
            {

                reciept_Details.Pf_Code = Session["pfCode"].ToString();
                reciept_Details.User_Id = Convert.ToInt64(Session["EmpId"]);


                /////////////////////gettting current time


                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                reciept_Details.Datetime_Cons = localTime;

                //////////////////////////////////////////////////////////////////////////

                
                db.Receipt_details.Add(reciept_Details);
                db.SaveChanges();

                if (Submit == "Sms")
                {

                    callapi(reciept_Details.sender_phone, reciept_Details.Consignment_No, reciept_Details.Charges_Total, reciept_Details.Destination);
                }

               

                if (Submit == "Print")
                {
                    //Printcashcounter(consignmentno);


                    //receipt.Consignment_No;//receipt_.Consignment_No;
                    //string consignmnetno = id;
                    //string consignmnetno = "P61118465";
                    LocalReport lr = new LocalReport();





                    //Company company = db.Companies.Where(m => m.Company_Id == CompanyId).FirstOrDefault();

                    //var dataset2 = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

                    //var dataset3 = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();



                    var Recieptdetails = db.Receipt_details.Where(m => m.Consignment_No == consignmentno);//.ToList();

                    var barcode = GenerateBarCode(consignmentno);

                    string path = Path.Combine(Server.MapPath("~/RdlcReport"), "cashcounter.rdlc");

                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }

                    ReportDataSource rd = new ReportDataSource("Recieptdetails", Recieptdetails);




                    lr.DataSources.Add(rd);

                    string reportType = "PDF";
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

                    //ViewBag.pdf = false;
                    //ViewBag.pdf = true;

                    savePath = Server.MapPath("~/ConsignmentPDF/" + "Recieptdetails-" + Recieptdetails.FirstOrDefault().Consignment_No.Replace("/", "-") + ".pdf");

                    using (FileStream stream = new FileStream(savePath, FileMode.Create))
                    {
                        stream.Write(renderByte, 0, renderByte.Length);
                    }

                    // return Redirect(savePath);
                    
                    Download(consignmentno);
                }

                /////////////////Setting Next  Consignment number

                //char ch = reciept_Details.Consignment_No[1];

                //string ch = reciept_Details.Consignment_No;

                String value = reciept_Details.Consignment_No;
                int startIndex = 0;
                int endindex = 2;
                int length = value.Length;

                String substring = value.Substring(startIndex, endindex);

                string strno = value.Remove(startIndex, endindex);

                
                //long consignnumber = Convert.ToInt64(reciept_Details.Consignment_No.Substring(1));
                int consignnumber =Convert.ToInt32(strno);

                consignnumber = consignnumber + 1;

                // ViewBag.nextconsignment = ch + "" + consignnumber;

                ViewBag.nextconsignment = substring + "" + consignnumber;

                //ViewBag.nextconsignment = consignnumber;

                ///// Adding Wallet Points To Phone Number///
                //WalletPoint AddPoints = db.WalletPoints.Where(m => m.MobileNo == reciept_Details.sender_phone).FirstOrDefault();


                ////////************** subtracting Discount //////////////////////



                /////////////////////////////////////////

                //if (AddPoints == null)
                //{
                //    float? points = 0;

                //    WalletPoint wp = new WalletPoint();

                //    wp.MobileNo = reciept_Details.sender_phone;

                //    if (DisableDisc != true)
                //    {
                //        if (reciept_Details.Consignment_No.ToUpper().StartsWith("N"))
                //        {
                //            points = (float)(0.025) * reciept_Details.Charges_Total;
                //        }
                //        else
                //        {
                //            points = (float)(0.05) * reciept_Details.Charges_Total;
                //        }
                //    }
                //    else
                //    {
                //        points = 0;
                //    }
                //    wp.Wallet_Money = points;

                //    wp.Datetime_Wa = GetLocalTime.GetDateTime();

                //    db.WalletPoints.Add(wp);
                //    add_History.addHistory("Added", reciept_Details.Pf_Code, reciept_Details.User_Id, wp.Wallet_Money, reciept_Details.Consignment_No, reciept_Details.sender_phone);
                //    MemberShip(reciept_Details.sender_phone);

                //}
                //else
                //{
                //    AddPoints.Wallet_Money = AddPoints.Wallet_Money - reciept_Details.Discount;

                //    float? points = 0;

                //    if (reciept_Details.Discount > 0)
                //    {
                //        add_History.addHistory("Redeemed", reciept_Details.Pf_Code, reciept_Details.User_Id, reciept_Details.Discount, reciept_Details.Consignment_No, reciept_Details.sender_phone);
                //    }

                //    if (DisableDisc != true)
                //    {
                //        if (reciept_Details.Consignment_No.ToUpper().StartsWith("N"))
                //        {

                //            var add = (float)(0.025) * reciept_Details.Charges_Total;

                //            AddPoints.Wallet_Money = (float)AddPoints.Wallet_Money + add;

                //            PointsAdded(reciept_Details.sender_phone, add, AddPoints.Wallet_Money);

                //            add_History.addHistory("Added", reciept_Details.Pf_Code, reciept_Details.User_Id, add, reciept_Details.Consignment_No, reciept_Details.sender_phone);

                //        }
                //        else
                //        {
                //            var add = (float)(0.05) * reciept_Details.Charges_Total;

                //            AddPoints.Wallet_Money = (float)AddPoints.Wallet_Money + add;

                //            PointsAdded(reciept_Details.sender_phone, add, AddPoints.Wallet_Money);

                //            add_History.addHistory("Added", reciept_Details.Pf_Code, reciept_Details.User_Id, add, reciept_Details.Consignment_No, reciept_Details.sender_phone);
                //        }
                //    }



                //    db.Entry(AddPoints).State = EntityState.Modified;

                //}
                //db.SaveChanges();

                //////////Alert After Added Cosingmnt///
                ViewBag.Success = "Consignment Added Successfully...!!!";
                //////////////////////////////////////

               

                ModelState.Clear();


                //return View(new Receipt_details());
                //return View(Printcashcounter(consignmentno));
               
            }

            //ViewBag.WalletPopints = reciept_Details.Discount;
            ViewBag.Discount = reciept_Details.Discount;

           
          
            //return View();
            return View(reciept_Details);



        }

        public ActionResult Download(string id)
        {

            //var invoice = db.Invoices.Where(m => m.IN_Id == id).FirstOrDefault();
            
            var Recieptdetails = db.Receipt_details.Where(m => m.Consignment_No == id);//.ToList();

            //string companyname = db.Companies.Where(m => m.Company_Id == invoice.Customer_Id).Select(m => m.Company_Id).FirstOrDefault().ToString();
            string savePath = Server.MapPath("~/ConsignmentPDF/" + "Recieptdetails-" + Recieptdetails.FirstOrDefault().Consignment_No.Replace("/", "-") + ".pdf");

            //string savePath = "http://admin.infantjesussolutions.in/PDF/" + invoice.Firm_Id + "-" + invoice.invoiceno.Replace("/", "-") + ".pdf";

            return Redirect(savePath);

        }

        public ActionResult SenderPhoneAutocomplete()
        {


            var entity = db.Receipt_details.
Select(e => new
{
    e.sender_phone
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReceipentsPhoneAutocomplete()
        {


            var entity = db.Receipt_details.
Select(e => new
{
    e.Reciepents_phone
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReceipentsCityAutocomplete()
        {

            var entity = db.Destinations.
Select(e => new
{
    e.Name
}).Distinct().ToList();


            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillSenderdetails(string sender_phone)
        {

            db.Configuration.ProxyCreationEnabled = false;


            var suggestions = (from s in db.Receipt_details
                               where s.sender_phone == sender_phone
                               select s).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillReceipentsdetails(string phone)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var suggestions = (from s in db.Receipt_details
                               where s.Reciepents_phone == phone
                               select s).FirstOrDefault();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillDestination(string pincode)
        {
            var suggestions = from s in db.Destinations
                              where s.Pincode == pincode
                              select s;

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FillSenderCityState(string pincode)
        {
            var suggestions = from s in db.Destinations
                              where s.Pincode == pincode
                              select s;

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FillRecepentsPincode(string Name)
        {
            var suggestions = from s in db.Destinations
                              where s.Name == Name
                              select s;

            return Json(suggestions, JsonRequestBehavior.AllowGet);
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

        public string Walletamount(string sender_phone)
        {
            WalletPoint suggestions = (from s in db.WalletPoints
                                       where s.MobileNo == sender_phone
                                       select s).FirstOrDefault();

            if (suggestions == null)
            {
                return "0";
            }
            else
            {
                return suggestions.Wallet_Money.ToString();
            }




        }


        public JsonResult Calculation(float Actualwaight, float VolumetricWaight, string ShipmentType, string Pincode)
        {
            double? DoxNonDoxAmt = 10;

            string pfcode = Session["pfCode"].ToString();

            List<JsonArrayCalc> jsonarray = new List<JsonArrayCalc>();

            List<Sector> sector = new List<Sector>();

            if (ShipmentType == "D")
            {
                sector = db.Sectors.Where(m => m.Pf_code == pfcode && m.CashD == true).ToList();
            }
            else if (ShipmentType == "N")
            {
                sector = db.Sectors.Where(m => m.Pf_code == pfcode &&  m.CashN == true).ToList();
            }

            float highwaight;

            if (Actualwaight > VolumetricWaight)
            {
                highwaight = Actualwaight;
            }
            else
            {
                highwaight = VolumetricWaight;
            }

            string CashRate = string.Concat("Cash_", pfcode);


            int sectorfound = (db.Sectors.Where(m => m.Sector_Name == "Rest Of India" && m.Pf_code == pfcode).Select(m => m.Sector_Id).FirstOrDefault());

            int flag = 0;
            foreach (var i in sector)
            {
                string[] sectarray = i.Pincode_values.Split(',');

                foreach (var m in sectarray)
                {
                    if (m.Contains("-"))
                    {
                        string[] pinarr = m.Split('-');

                        if (Convert.ToInt64(Pincode) >= Convert.ToInt64(pinarr[0]) && Convert.ToInt64(Pincode) <= Convert.ToInt64(pinarr[1]))
                        {
                            sectorfound = i.Sector_Id;
                            flag = 1;
                            break;

                        }
                    }
                    else if (m == Pincode)
                    {
                        sectorfound = i.Sector_Id;
                        flag = 1;
                        break;
                    }


                }
                if (flag == 1)
                {
                    break;
                }
            }



            if (ShipmentType == "D")
            {
               
                Ratem dox = db.Ratems.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();

                if (dox.NoOfSlab == 2)
                {
                    if (highwaight <= dox.Uptosl1)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab1);
                    }
                    else
                    {
                        // 0.500 /  (2 - 0.25)

                        double weightmod = (highwaight - Convert.ToDouble(dox.Uptosl1)) / Convert.ToDouble(dox.Uptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        DoxNonDoxAmt = Convert.ToDouble(dox.slab1 + (dox.slab4 * weightmod));
                    }
                }
                else if (dox.NoOfSlab == 3)
                {
                    if (highwaight <= dox.Uptosl1)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab1);
                    }
                    else if (highwaight <= dox.Uptosl2)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab2);
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(dox.Uptosl2)) / Convert.ToDouble(dox.Uptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        DoxNonDoxAmt = Convert.ToDouble(dox.slab2 + (dox.slab4 * weightmod));
                    }



                }
                else if (dox.NoOfSlab == 4)
                {

                    if (highwaight <= dox.Uptosl1)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab1);
                    }

                    else if (highwaight <= dox.Uptosl2)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab2);
                    }

                    else if (highwaight <= dox.Uptosl3)
                    {
                        DoxNonDoxAmt = Convert.ToDouble(dox.slab3);
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(dox.Uptosl3)) / Convert.ToDouble(dox.Uptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        DoxNonDoxAmt = Convert.ToDouble(dox.slab3 + (dox.slab4 * weightmod));
                    }

                }



                JsonArrayCalc js = new JsonArrayCalc();
                js.name = "Doxamount";
                js.Amount = DoxNonDoxAmt;
                jsonarray.Add(js);

                // return Json(new { DoxAmount = Amount });
            }
             if (ShipmentType == "N")
            {
                Nondox nondox = db.Nondoxes.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();

                double? AirAmount = 0.0;

                if (nondox.NoOfSlabN == 2)
                {
                    if (highwaight <= nondox.AUptosl1)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab1) * nondox.AUptosl1;
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.AUptosl1)) / Convert.ToDouble(nondox.AUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        AirAmount = Convert.ToDouble((nondox.Aslab1 * nondox.AUptosl1) + (nondox.Aslab4 * weightmod));
                    }
                }
                else if (nondox.NoOfSlabN == 3)
                {
                    if (highwaight <= nondox.AUptosl1)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab1);
                    }
                    else if (highwaight <= nondox.AUptosl2)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab2);
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.AUptosl2)) / Convert.ToDouble(nondox.AUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        AirAmount = Convert.ToDouble(nondox.Aslab2 + (nondox.Aslab4 * weightmod));
                    }



                }
                else if (nondox.NoOfSlabN == 4)
                {

                    if (highwaight <= nondox.AUptosl1)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab1) * nondox.AUptosl1;
                    }

                    else if (highwaight <= nondox.AUptosl2)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab2);
                    }
                    else if (highwaight <= nondox.AUptosl3)
                    {
                        AirAmount = Convert.ToDouble(nondox.Aslab3);
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.AUptosl3)) / Convert.ToDouble(nondox.AUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        AirAmount = Convert.ToDouble(nondox.Aslab3 + (nondox.Aslab4 * weightmod));
                    }

                }



                JsonArrayCalc js = new JsonArrayCalc();
                js.name = "AirAmount";
                js.Amount = AirAmount;
                jsonarray.Add(js);

                ///////////////////////////////////Air Surface /////////////////////////////

                Nondox nondox1 = db.Nondoxes.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();
                double? Amountsurf = 0.0;
                //double? Amount1;
                //double? Min_Weight;

                if (nondox.NoOfSlabS == 2)
                {
                    if (highwaight <= nondox.SUptosl1)
                    {
                        Amountsurf = Convert.ToDouble(nondox.Sslab1) * nondox.SUptosl1;
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.SUptosl1)) / Convert.ToDouble(nondox.SUptosl4);

                        weightmod = Math.Ceiling(weightmod);


                        Amountsurf = Convert.ToDouble((nondox.Sslab1 * nondox.SUptosl1) + (nondox.Sslab4 * weightmod));
                    }
                }
                else if (nondox.NoOfSlabS == 3)
                {
                    if (highwaight <= nondox.SUptosl1)
                    {
                        //Amountsurf = Convert.ToDouble(nondox.Sslab1);
                        Amountsurf = Convert.ToDouble(nondox.Sslab1) * nondox.SUptosl1;
                    }
                    else if (highwaight <= nondox.SUptosl2)
                    {
                        // Amountsurf = Convert.ToDouble(nondox.Sslab2);
                        Amountsurf = Convert.ToDouble(nondox.Sslab2) * nondox.SUptosl2;
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.SUptosl2)) / Convert.ToDouble(nondox.SUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        Amountsurf = Convert.ToDouble((nondox.Sslab2 * nondox.SUptosl2) + (nondox.Sslab4 * weightmod));
                    }


                }
                else if (nondox.NoOfSlabS == 4)
                {

                    if (highwaight <= nondox.SUptosl1)
                    {
                        //Amountsurf = Convert.ToDouble(nondox.Sslab1);
                        Amountsurf = Convert.ToDouble(nondox.Sslab1) * nondox.SUptosl1;

                    }

                    else if (highwaight <= nondox.SUptosl2)
                    {
                        // Amountsurf = Convert.ToDouble(nondox.Sslab2);
                        Amountsurf = Convert.ToDouble(nondox.Sslab2) * nondox.SUptosl2;
                    }
                    else if (highwaight <= nondox.SUptosl3)
                    {
                        Amountsurf = Convert.ToDouble(nondox.Sslab3);
                        Amountsurf = Convert.ToDouble(nondox.Sslab3) * nondox.SUptosl3;
                    }
                    else
                    {
                        double weightmod = (highwaight - Convert.ToDouble(nondox.SUptosl3)) / Convert.ToDouble(nondox.SUptosl4);

                        weightmod = Math.Ceiling(weightmod);

                        Amountsurf = Convert.ToDouble((nondox.Sslab3 * nondox.SUptosl3) + (nondox.Sslab4 * weightmod));
                    }

                }




                JsonArrayCalc jssurf = new JsonArrayCalc();
                jssurf.name = "Amountsurf";
                jssurf.Amount = Amountsurf;
                jsonarray.Add(jssurf);

                //return Json(new { nonAisr = Amount, nonsurf = Amountsurf });

            }




            //long pin = Pincode;

            List<Service_list> service_List = db.Service_list.Where(m => m.Pincode == Pincode).ToList();



            for (int i = 0; i < service_List.Count; i++)
            {

                double? amount1;
                double? amount2;

                if (service_List[i].Service_ == "CITY PRIMETIME PLUS - 12:00")
                {
                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("City") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.PUpto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - 0.500) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.Padd100kg;
                    }



                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "CITY PRIMETIME PLUS - 2 PM")
                {

                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("City") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.P2Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.P2Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2add100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "ZONAL PRIMETIME PLUS 12.00")
                {

                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("ZONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.PUpto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.Padd100kg;
                    }


                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "ZONAL PRIMETIME PLUS - 2 PM")
                {
                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("ZONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.P2Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.P2Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2add100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "METRO PRIMETIME PLUS-12.00")
                {
                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("METRO") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.PUpto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.Padd100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "METRO PRIMETIME PLUS - 2 PM")
                {
                    Dtdc_Ptp dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("METRO") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_Ptp.P2Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_Ptp.P2Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.P2add100kg;
                    }


                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "CITY OFFICE COLLECT PLUS" || service_List[i].Service_ == "CITY OFFICE COLLECT PLUS")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("CITY") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);

                }
                else if (service_List[i].Service_ == "ZONAL OFFICE COLLECT PLUS" || service_List[i].Service_ == "ZONAL BLUE" || service_List[i].Service_ == "ZONAL OFFICE COLLECT BLUE" || service_List[i].Service_ == "ZONAL OFFICE COLLECT PLUS" || service_List[i].Service_ == "ZONAL PLUS" || service_List[i].Service_ == "ZONAL PLUS PEC" || service_List[i].Service_ == "SPECIAL ZONAL 2 BLUE" || service_List[i].Service_ == "SPECIAL ZONAL 2 PLUS" || service_List[i].Service_ == "ZONAL GREEN" || service_List[i].Service_ == "ZONAL OFFICE COLLECT GREEN" || service_List[i].Service_ == "ZONAL BLUE PEC" || service_List[i].Service_ == "SPECIAL ZONAL BLUE" || service_List[i].Service_ == "SPECIAL ZONAL PLUS" || service_List[i].Service_ == "SPECIAL ZONAL PLUS PEC" || service_List[i].Service_ == "SPECIAL ZONAL BLUE PEC" || service_List[i].Service_ == "SPECIAL ZONAL GREEN" || service_List[i].Service_ == "PEC ZONAL BLUE 2- SPECIAL" || service_List[i].Service_ == "PEC ZONAL PLUS 2- SPECIAL")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("ZONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }
                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "METRO BLUE" || service_List[i].Service_ == "METRO OFFICE COLLECT BLUE" || service_List[i].Service_ == "METRO OFFICE COLLECT PLUS" || service_List[i].Service_ == "METRO PLUS" || service_List[i].Service_ == "METRO PLUS PEC")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("Metro") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }
                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "NATIONAL BLUE" || service_List[i].Service_ == "NATIONAL OFFICE COLLECT BLUE" || service_List[i].Service_ == "NATIONAL OFFICE COLLECT PLUS" || service_List[i].Service_ == "NATIONAL PLUS" || service_List[i].Service_ == "NATIONAL BLUE PEC" || service_List[i].Service_ == "NATIONAL GREEN" || service_List[i].Service_ == "NATIONAL OFFICE COLLECT GREEN" || service_List[i].Service_ == "NATIONAL PLUS PEC")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("NATIONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }
                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }
                else if (service_List[i].Service_ == "REGIONAL PLUS" || service_List[i].Service_ == "REGIONAL PLUS PEC")
                {
                    dtdcPlu dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("REGIONAL") && m.Company_id == CashRate).FirstOrDefault();



                    if (highwaight <= 10)
                    {

                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                    else if (highwaight > 10 && highwaight <= 25)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U10to25kg;
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;
                    }

                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = amount1;
                    jsonarray.Add(js);
                }

                else if (service_List[i].Service_ == "GROUND EXPRESS CARGO")
                {
                    express_cargo express = db.express_cargo.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();
                    double? Amount;
                    double? Amount1;
                    double? Min_Weight;


                    {
                        if (highwaight <= 50)
                        {
                            Amount = Convert.ToDouble(express.Exslab1);
                        }
                        else
                        {


                            Amount = Math.Ceiling(highwaight) * express.Exslab2;

                        }




                    }

                    //return Json(new { ExpAmount = Amount });
                    JsonArrayCalc js = new JsonArrayCalc();
                    js.name = service_List[i].Service_;
                    js.Amount = Amount;
                    jsonarray.Add(js);
                }

            }


            //if Json ayyay amount is null then amount set to be zero

            List<JsonArrayCalc> jsonarrayForsendBack = new List<JsonArrayCalc>();

            foreach (JsonArrayCalc i in jsonarray)
            {
                if (i.Amount == null)
                {
                    i.Amount = 0;
                }
                jsonarrayForsendBack.Add(i);

            }


            ///////

            return Json(jsonarrayForsendBack, JsonRequestBehavior.AllowGet);
        }



        public string consignmentval(string Consignment)
        {
            string Consignmentno = db.Receipt_details.Where(m => m.Consignment_No == Consignment.ToUpper()).Select(m => m.Consignment_No).FirstOrDefault();

            if (Consignmentno == null)
            {
                return "1";

            }
            else
            {
                return "0";
            }
        }






        public ActionResult ConsignmentDetails()
        {

            return View();
        }


        [SessionTimeout]
        [HttpPost]
        public ActionResult ConsignmentDetails(Receipt_details reciept_Details, string Submit, bool? DisableDisc, string texens)
        {
            
            ////credit Amount logic///
                reciept_Details.Paid_Amount = Convert.ToInt16(reciept_Details.Credit_Amount);
             /////////////////////////
             
            ViewBag.Insuarance = reciept_Details.Shipmenttype;
            ViewBag.yesinsurance = reciept_Details.Insurance;

            var consignment = db.Receipt_details.Where(m => m.Consignment_No == reciept_Details.Consignment_No).FirstOrDefault();

            ViewBag.dates = consignment.Datetime_Cons;
            if (consignment == null)
            {
                ModelState.AddModelError("Consignment", "Consignment Dosent Exist");
            }
            else if (ModelState.IsValid)
            {
                reciept_Details.Pf_Code = Session["pfCode"].ToString();
                reciept_Details.User_Id = Convert.ToInt64(Session["EmpId"]);

                if (texens == "true")
                {
                    
                    reciept_Details.Charges_Amount = Convert.ToInt16(reciept_Details.Charges_Amount);
                    reciept_Details.Charges_Service = Convert.ToInt16(reciept_Details.Charges_Service);
                    reciept_Details.Risk_Surcharge = Convert.ToInt16(reciept_Details.Risk_Surcharge);
                    reciept_Details.Service_Tax = Convert.ToInt16(reciept_Details.Service_Tax);
                    reciept_Details.Discount = Convert.ToInt16(reciept_Details.Discount);
                    reciept_Details.Addition_charge = Convert.ToInt16(reciept_Details.Addition_charge);
                    reciept_Details.Charges_Total = Convert.ToInt16(reciept_Details.Charges_Total);
                    reciept_Details.Credit_Amount = Convert.ToInt16(reciept_Details.Credit_Amount);
                    ////////////////////
                    reciept_Details.Credit = consignment.Credit;
                    reciept_Details.secure_Pack = consignment.secure_Pack;
                    reciept_Details.Passport = consignment.Passport;
                    reciept_Details.OfficeSunday = consignment.OfficeSunday;
                }
                else
                {
                    reciept_Details.Charges_Amount = consignment.Charges_Amount;
                    reciept_Details.Charges_Service = consignment.Charges_Service;
                    reciept_Details.Risk_Surcharge = consignment.Risk_Surcharge;
                    reciept_Details.Service_Tax = consignment.Service_Tax;
                    reciept_Details.Discount = consignment.Discount;
                    reciept_Details.Addition_charge = consignment.Addition_charge;
                    reciept_Details.Charges_Total = consignment.Charges_Total;
                 }

                reciept_Details.secure_Pack = consignment.secure_Pack;
                reciept_Details.Passport = consignment.Passport;
                reciept_Details.OfficeSunday = consignment.OfficeSunday;



                reciept_Details.Datetime_Cons = consignment.Datetime_Cons;



                reciept_Details.Receipt_Id = consignment.Receipt_Id;

                db.Entry(consignment).State = EntityState.Detached;
                //////////////////////////////////////////////////////////////////////////


                db.Entry(reciept_Details).State=EntityState.Modified;
                db.SaveChanges();

              
                ViewBag.Success = "Consignment Updated Successfully...!!!";
                ////////////////////////////////////////

                ModelState.Clear();
                return View(new Receipt_details());
            }
            //ViewBag.WalletPopints = reciept_Details.Discount;
            

            return View(reciept_Details);
        }


        public JsonResult Consignment(string Consignment_No)
        {
            db.Configuration.ProxyCreationEnabled = false;

            Receipt_DetailsModel receipt = new Receipt_DetailsModel();

            Receipt_details rc = (from u in db.Receipt_details
                                  where u.Consignment_No == Consignment_No
                                  select u).FirstOrDefault();

            receipt.CreateDateString = rc.Datetime_Cons.Value.Date.ToString("dd/MM/yyyy");

            return Json(rc, JsonRequestBehavior.AllowGet);
        }


        public ActionResult callapi(string phno, string consinmentno, float? amount, string Destination)
        {
            string pfcode = Session["pfCode"].ToString();
            Franchisee branchname = db.Franchisees.Where(m => m.PF_Code == pfcode).FirstOrDefault();


          
            string url = "http://smsc.biz/httpapi/send?username=santoshmaske99@outlook.com&password=97sai22&sender_id=DTDCFR&route=T&phonenumber=" + phno + "&message=Dear%20customer%2Cyour%20cn%20no%20is%20" + consinmentno + "%20for%20Rs%20" + amount + ".%20Thank%20you%20for%20using%20DTDC%20" + branchname.BranchName + "%20branch%20Track%20%40%20www.dtdc.com%20" + branchname.ContactNo + "";

         


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:56851/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {


            }
            else
            {

            }


            return View();

        }





        public ActionResult MemberShip(string phno)
        {
            string pfcode = Session["pfCode"].ToString();
            Franchisee branchname = db.Franchisees.Where(m => m.PF_Code == pfcode).FirstOrDefault();


            string url = "http://sms.googlymart.com/httpapi/send?username=santoshmaske99@outlook.com&password=97sai22&sender_id=DTDCFR&route=T&phonenumber=" + phno + "&message=Hi%20Congrats%20on%20becoming%20the%20member%20of%20DTDC%20VEEREXPRESS.For%20more%20details%20www.veerexpress.in";


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:56851/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {


            }
            else
            {

            }


            return View();

        }



        public ActionResult PointsAdded(string phno, float? points, float? totalpoints)
        {
            string pfcode = Session["pfCode"].ToString();
            Franchisee branchname = db.Franchisees.Where(m => m.PF_Code == pfcode).FirstOrDefault();


            string url = "http://sms.googlymart.com/httpapi/send?username=santoshmaske99@outlook.com&password=97sai22&sender_id=DTDCFR&route=T&phonenumber=" + phno + "&message=Hi%2C%20" + points + "%20points%20added%20in%20DTDC%20VEEREXPRESS%20wallet.%20Curr.%20balance%20is%20" + totalpoints + "%20points";

          


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:56851/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {


            }
            else
            {

            }


            return View();

        }

        public string RedeemDiscount(string sender_phone, float? amount, string Otp)
        {
            WalletPoint suggestions = (from s in db.WalletPoints
                                       where s.MobileNo == sender_phone
                                       select s).FirstOrDefault();


            string Otp1 = db.RedeemOtps.Where(u => u.Mobile_No == sender_phone).OrderByDescending(m => m.Datetime).Select(u => u.Otp).Take(1).FirstOrDefault();
            //(from u in db.RedeemOtps
            //             orderby u.Datetime descending
            //             where u.Mobile_No == sender_phone
            //             select u.Otp.Take(1))FirstOrDefault();



            if (suggestions != null)
            {
                if (suggestions.Wallet_Money < amount)
                {
                    return "0";
                }
                else if (Otp1 != Otp)
                {
                    return "2";
                }
                else
                {
                    return "1";
                }
            }
            else
            {
                return "0";
            }

        }


        public string SendOtp(string sender_phone)
        {

            WalletPoint suggestions = (from s in db.WalletPoints
                                       where s.MobileNo == sender_phone
                                       select s).FirstOrDefault();

            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");

            string Otp = r;



            string url = "http://sms.googlymart.com/httpapi/send?username=santoshmaske99@outlook.com&password=97sai22&sender_id=DTDCFR&route=T&phonenumber=" + sender_phone + "&message=Hi%2C%20to%20Redeem%20Your%20points%20from%20DTDC%20VEEREXPRESS%20wallet%20OTP%20" + Otp;

    


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:56851/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {


            }
            else
            {

            }






            RedeemOtp rd = new RedeemOtp();

            rd.Otp = Otp;
            rd.Mobile_No = sender_phone;
            rd.verified = false;

            rd.Datetime = GetLocalTime.GetDateTime();

            db.RedeemOtps.Add(rd);
            db.SaveChanges();

            return "Success";
        }

        #region BulkBooking
        [SessionTimeout]
        public ActionResult BulkBooking()
        {
            return View();
        }

        [SessionTimeout]
        [HttpPost]
        public ActionResult BulkBooking(Receipt_details reciept_Details, string Submit, bool? DisableDisc)
        {


            ////credit Amount logic///

            reciept_Details.Paid_Amount = Convert.ToInt16(reciept_Details.Credit_Amount);

            /////////////////////////


            ViewBag.Insuarance = reciept_Details.Shipmenttype;
            ViewBag.yesinsurance = reciept_Details.Insurance;

            var consignment = db.Receipt_details.Where(m => m.Consignment_No == reciept_Details.Consignment_No).FirstOrDefault();

            if (consignment != null)
            {
                ModelState.AddModelError("Consignment", "Consignment Allready Exist");
            }







            if (ModelState.IsValid)
            {

                reciept_Details.Pf_Code = Session["pfCode"].ToString();
                reciept_Details.User_Id = Convert.ToInt64(Session["EmpId"]);


                /////////////////////gettting current time


                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                reciept_Details.Datetime_Cons = localTime;

                //////////////////////////////////////////////////////////////////////////


                db.Receipt_details.Add(reciept_Details);
                db.SaveChanges();

                if (Submit == "Sms")
                {

                    callapi(reciept_Details.sender_phone, reciept_Details.Consignment_No, reciept_Details.Charges_Amount, reciept_Details.Destination);
                }
                /////////////////Setting Next  Consignment number

                char ch = reciept_Details.Consignment_No[0];

                long consignnumber = Convert.ToInt64(reciept_Details.Consignment_No.Substring(1));

                consignnumber = consignnumber + 1;

                ViewBag.nextconsignment = ch + "" + consignnumber;



                ///// Adding Wallet Points To Phone Number///
                //WalletPoint AddPoints = db.WalletPoints.Where(m => m.MobileNo == reciept_Details.sender_phone).FirstOrDefault();


                ////////************** subtracting Discount //////////////////////



                /////////////////////////////////////////

                //if (AddPoints == null)
                //{
                //    WalletPoint wp = new WalletPoint();

                //    wp.MobileNo = reciept_Details.sender_phone;

                //    if (DisableDisc != true)
                //    {
                //        if (reciept_Details.Consignment_No.ToUpper().StartsWith("N"))
                //        {
                //            wp.Wallet_Money = (float)(0.025) * reciept_Details.Charges_Total;
                //        }
                //        else
                //        {
                //            wp.Wallet_Money = (float)(0.05) * reciept_Details.Charges_Total;
                //        }
                //    }
                //    else
                //    {
                //        wp.Wallet_Money = 0;
                //    }
                //    db.WalletPoints.Add(wp);
                //    MemberShip(reciept_Details.sender_phone);

                //}
                //else
                //{
                //    AddPoints.Wallet_Money = AddPoints.Wallet_Money - reciept_Details.Discount;

                //    if (DisableDisc != true)
                //    {
                //        if (reciept_Details.Consignment_No.ToUpper().StartsWith("N"))
                //        {

                //            var add = (float)(0.025) * reciept_Details.Charges_Total;

                //            AddPoints.Wallet_Money = (float)AddPoints.Wallet_Money + add;

                //            PointsAdded(reciept_Details.sender_phone, add, AddPoints.Wallet_Money);

                //        }
                //        else
                //        {
                //            var add = (float)(0.05) * reciept_Details.Charges_Total;

                //            AddPoints.Wallet_Money = (float)AddPoints.Wallet_Money + add;

                //            PointsAdded(reciept_Details.sender_phone, add, AddPoints.Wallet_Money);
                //        }
                //    }


                //    db.Entry(AddPoints).State = EntityState.Modified;

                //}
                //db.SaveChanges();

                //////////Alert After Added Cosingmnt///
                ViewBag.Success = "Consignment Added Successfully...!!!";
                ////////////////////////////////////////


                return View();
            }
            //ViewBag.WalletPopints = reciept_Details.Discount;
            ViewBag.Discount = reciept_Details.Discount;

            return View(reciept_Details);
        }
        #endregion


        public ActionResult RateCalculater()
        {
            return View();
        }


        [HttpPost]
        public JsonResult AjaxMethod(string id)
        {
            Receipt_details re = new Receipt_details
            {
                Consignment_No = id,
                //DateTime = DateTime.Now.ToString()
            };

            return Json(re);
        }

        
        public ActionResult Printcashcounter(string id)
        //public void Printcashcounter(string id)
        {

            //receipt.Consignment_No;//receipt_.Consignment_No;
            string consignmnetno = id;
            //string consignmnetno = "P61118465";
             LocalReport lr = new LocalReport();


            


            //Company company = db.Companies.Where(m => m.Company_Id == CompanyId).FirstOrDefault();

            //var dataset2 = db.Ratems.Where(m => m.Company_id == CompanyId).ToList();

            //var dataset3 = db.Nondoxes.Where(m => m.Company_id == CompanyId).ToList();



            var Recieptdetails = db.Receipt_details.Where(m => m.Consignment_No == consignmnetno).ToList();

            var barcode = GenerateBarCode(consignmnetno);

            string path = Path.Combine(Server.MapPath("~/RdlcReport"), "cashcounter.rdlc");

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }

            ReportDataSource rd = new ReportDataSource("Recieptdetails", Recieptdetails);
           



            lr.DataSources.Add(rd);
         
            string reportType = "PDF";
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

            //ViewBag.pdf = false;
            //ViewBag.pdf = true;

            string savePath = Server.MapPath("~/ConsignmentPDF/" + "Recieptdetails-" + Recieptdetails.FirstOrDefault().Consignment_No.Replace("/", "-") + ".pdf");

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                stream.Write(renderByte, 0, renderByte.Length);
            }

            return Redirect(savePath);

            //return File(renderByte, mimeType);

            //return View();
        }


        public ActionResult GenerateBarCode()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GenerateBarCode(string barcode)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Bitmap bitMap = new Bitmap(barcode.Length * 40, 80))
                {
                    using (Graphics graphics = Graphics.FromImage(bitMap))
                    {
                        Font oFont = new Font("IDAutomationHC39M", 16);
                        PointF point = new PointF(2f, 2f);
                        SolidBrush whiteBrush = new SolidBrush(Color.White);
                        graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);
                        SolidBrush blackBrush = new SolidBrush(Color.DarkBlue);
                        graphics.DrawString("*" + barcode + "*", oFont, blackBrush, point);
                    }

                    bitMap.Save(memoryStream, ImageFormat.Jpeg);

                    ViewBag.BarcodeImage = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            return View();
        }

        


         public string CheckTempPass(string temppass, string consignmentno)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var Pass = "Veer#1234";

            var checkcon = db.Receipt_details.Where(x => x.Consignment_No == consignmentno).ToList();

            if (temppass == Pass && checkcon != null)
            {
                return "1";
            }
            else
            {
                return "0";
            }

        }

    }
}