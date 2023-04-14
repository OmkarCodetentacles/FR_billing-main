using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.Linq;
using DtDc_Billing.Models;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    [SessionUserModule]
    public class CalculationController : Controller
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();
        // GET: Calculation
        [HttpPost]
        public ActionResult CalulateAmt(string Consignment, string custid, string Pincode, string mode, string qty, double charweight, string type)
        {
            double? DoxNonDoxAmt = 10;


            double highwaight = charweight;

            string pfcode = db.Companies.Where(m => m.Company_Id == custid).Select(m=>m.Pf_code).FirstOrDefault();

            List<Sector> sector = new List<Sector>();
            if (type == "N")
            {
                 sector = db.Sectors.Where(m => m.Pf_code == pfcode && m.BillN == true).OrderBy(m=>m.Priority).ToList();
            }
            else
            {
                 sector = db.Sectors.Where(m => m.Pf_code == pfcode && m.BillD == true).OrderBy(m => m.Priority).ToList();
            }

            


            double ? Amount = 0.0;

           

            string CashRate = custid;


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
                    }


                }
                if (flag == 1)
                {
                    break;
                }
            }




            if (Consignment.ToLower().StartsWith("e"))
            {
                if (mode == "CP2" || mode == "D2Z" || mode == "D12")
                {
                    Dtdc_Ptp dtdc_Ptp = null;
                    if (mode == "CP2")
                    {
                        dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("City") && m.Company_id == CashRate).FirstOrDefault();
                    }
                    else if (mode == "D2Z")
                    {
                        dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("ZONAL") && m.Company_id == CashRate).FirstOrDefault();
                    }
                    else if (mode == "D12")
                    {
                        dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("METRO") && m.Company_id == CashRate).FirstOrDefault();
                    }



                    double? amount1;


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

                        if (dtdc_Ptp.PU10to25kg == 0)
                        {
                            amount1 = dtdc_Ptp.PUpto500gm;


                            if (highwaight > 0.500)
                            {
                                double weightmod = (highwaight - 0.500) / Convert.ToDouble(0.500);

                                weightmod = Math.Ceiling(weightmod);

                                amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                            }

                        }
                    }
                    else if (highwaight > 25 && highwaight <= 50)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU25to50;

                        if (dtdc_Ptp.PU25to50 == 0)
                        {
                            amount1 = dtdc_Ptp.PUpto500gm;


                            if (highwaight > 0.500)
                            {
                                double weightmod = (highwaight - 0.500) / Convert.ToDouble(0.500);

                                weightmod = Math.Ceiling(weightmod);

                                amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                            }

                        }
                    }
                    else if (highwaight > 50 && highwaight <= 100)
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.PU50to100;

                        if (dtdc_Ptp.PU50to100 == 0)
                        {
                            amount1 = dtdc_Ptp.PUpto500gm;


                            if (highwaight > 0.500)
                            {
                                double weightmod = (highwaight - 0.500) / Convert.ToDouble(0.500);

                                weightmod = Math.Ceiling(weightmod);

                                amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                            }

                        }
                    }
                    else
                    {
                        amount1 = Math.Ceiling(highwaight) * dtdc_Ptp.Padd100kg;

                        if (dtdc_Ptp.Padd100kg == 0)
                        {
                            amount1 = dtdc_Ptp.PUpto500gm;


                            if (highwaight > 0.500)
                            {
                                double weightmod = (highwaight - 0.500) / Convert.ToDouble(0.500);

                                weightmod = Math.Ceiling(weightmod);

                                amount1 = amount1 + (dtdc_Ptp.PAdd500gm * weightmod);
                            }

                        }
                    }


                    Amount = amount1;
                }
                else if (mode == "CSP" || mode == "DZ2" || mode == "DM2")
                {
                    double? amount1;

                    Dtdc_Ptp dtdc_Ptp = null;
                    //////////////////////////////////////////////////////////////////////////////////////////////////
                    if (mode == "CSP")
                    {
                        dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("City") && m.Company_id == CashRate).FirstOrDefault();
                    }
                    else if (mode == "DZ2")
                    {
                        dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("ZONAL") && m.Company_id == CashRate).FirstOrDefault();
                    }
                    else if (mode == "DM2")
                    {
                        dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("METRO") && m.Company_id == CashRate).FirstOrDefault();
                    }
                    else
                    {
                        dtdc_Ptp = db.Dtdc_Ptp.Where(m => m.dest.Contains("NATIONAL") && m.Company_id == CashRate).FirstOrDefault();
                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////





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


                    Amount = amount1;
                }


            }
            else if (Consignment.ToLower().StartsWith("v"))
            {
                dtdcPlu dtdc_plus = null;

                if (mode == "DCP" || mode == "DC2" || mode == "DCD" || mode == "DCS" || mode == "PEC")
                {
                    dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("CITY") && m.Company_id == CashRate).FirstOrDefault();
                }
                else if (mode == "DZB" || mode == "DSF" || mode == "D2D" || mode == "DZS" || mode == "DSZ" || mode == "ZSP")
                {
                    dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("ZONAL") && m.Company_id == CashRate).FirstOrDefault();
                }
                else if (mode == "DAR" || mode == "DMB" || mode == "DM2" || mode == "DMD" || mode == "DMS" || mode == "DSM" || mode == "MSP")
                {
                    dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("METRO") && m.Company_id == CashRate).FirstOrDefault();
                }

                else if (mode == "DNB" || mode == "DNP" || mode == "DN2" || mode == "DND" || mode == "DNS")
                {
                    dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("NATIONAL") && m.Company_id == CashRate).FirstOrDefault();
                }
                else
                {
                    dtdc_plus = db.dtdcPlus.Where(m => m.destination.Contains("NATIONAL") && m.Company_id == CashRate).FirstOrDefault();
                }





                double? amount1;

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

                    if (dtdc_plus.U10to25kg == 0)
                    {
                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                }
                else if (highwaight > 25 && highwaight <= 50)
                {
                    amount1 = Math.Ceiling(highwaight) * dtdc_plus.U25to50;

                    if (dtdc_plus.U25to50 == 0)
                    {
                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                }
                else if (highwaight > 50 && highwaight <= 100)
                {
                    amount1 = Math.Ceiling(highwaight) * dtdc_plus.U50to100;

                    if (dtdc_plus.U50to100 == 0)
                    {
                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                }
                else
                {
                    amount1 = Math.Ceiling(highwaight) * dtdc_plus.add100kg;

                    if (dtdc_plus.add100kg == 0)
                    {
                        amount1 = dtdc_plus.Upto500gm;


                        if (highwaight > 0.500)
                        {
                            double weightmod = (highwaight - Convert.ToDouble(0.500)) / Convert.ToDouble(0.500);

                            weightmod = Math.Ceiling(weightmod);

                            amount1 = amount1 + (dtdc_plus.Add500gm * weightmod);
                        }

                    }
                }
                Amount = amount1;
            }
            else if (Consignment.ToLower().StartsWith("x"))
            {
                Priority prio = db.Priorities.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();

              
                    double? AirAmount = 0.0;

                    if (prio.prinoofslab == 2)
                    {
                        if (highwaight <= prio.priupto1)
                        {
                            AirAmount = Convert.ToDouble(prio.prislab1) * prio.priupto1;
                        }
                        else
                        {
                            double weightmod = (highwaight - Convert.ToDouble(prio.priupto1)) / Convert.ToDouble(prio.priupto4);

                            weightmod = Math.Ceiling(weightmod);

                            AirAmount = Convert.ToDouble((prio.prislab1 * prio.priupto1) + (prio.prislab4 * weightmod));
                        }
                    }
                    else if (prio.prinoofslab == 3)
                    {
                        if (highwaight <= prio.priupto1)
                        {
                            AirAmount = Convert.ToDouble(prio.prislab1) * prio.priupto1;
                        }
                        else if (highwaight <= prio.priupto2)
                        {
                            AirAmount = Convert.ToDouble(prio.prislab2) * Math.Ceiling(highwaight);
                        }
                        else
                        {
                            double weightmod = (highwaight - Convert.ToDouble(prio.priupto2)) / Convert.ToDouble(prio.priupto4);

                            weightmod = Math.Ceiling(weightmod);

                            AirAmount = Convert.ToDouble(prio.prislab2 + (prio.prislab4 * weightmod));
                        }



                    }
                    else if (prio.prinoofslab == 4)
                    {

                        if (highwaight <= prio.priupto1)
                        {
                            AirAmount = Convert.ToDouble(prio.prislab1) * prio.priupto1;
                        }

                        else if (highwaight <= prio.priupto2)
                        {
                            AirAmount = Convert.ToDouble(prio.prislab2) * Math.Ceiling(highwaight);
                        }
                        else if (highwaight <= prio.priupto3)
                        {
                            AirAmount = Convert.ToDouble(prio.prislab3) * Math.Ceiling(highwaight);
                        }
                        else
                        {
                            double weightmod = (highwaight - Convert.ToDouble(prio.priupto3)) / Convert.ToDouble(prio.priupto4);

                            weightmod = Math.Ceiling(weightmod);

                            // AirAmount = Convert.ToDouble(nondox.Aslab3 + (nondox.Aslab4 * weightmod));
                            AirAmount = Convert.ToDouble((prio.prislab3 * prio.priupto3) + (prio.prislab4 * weightmod));
                        }

                    }

                    Amount = AirAmount;
                }
            
            else if (mode.ToLower().StartsWith("ge"))
            {
                express_cargo express = db.express_cargo.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();

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


            }
            else if (type == "D")
            {
                Ratem dox = db.Ratems.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();

                if (dox.NoOfSlab == 2)
                {
                    if (charweight <= dox.Uptosl1)
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

                Amount = DoxNonDoxAmt;



                // return Json(new { DoxAmount = Amount });
            }
            else if (type == "N")
            {
                Nondox nondox = db.Nondoxes.Where(m => m.Sector_Id == sectorfound && m.Company_id == CashRate).FirstOrDefault();

                if (mode.ToLower().StartsWith("a"))
                {
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
                            AirAmount = Convert.ToDouble(nondox.Aslab1) * nondox.AUptosl1;
                        }
                        else if (highwaight <= nondox.AUptosl2)
                        {
                            AirAmount = Convert.ToDouble(nondox.Aslab2) * Math.Ceiling(highwaight);
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
                            AirAmount = Convert.ToDouble(nondox.Aslab2) * Math.Ceiling(highwaight);
                        }
                        else if (highwaight <= nondox.AUptosl3)
                        {
                            AirAmount = Convert.ToDouble(nondox.Aslab3) * Math.Ceiling(highwaight);
                        }
                        else
                        {
                            double weightmod = (highwaight - Convert.ToDouble(nondox.AUptosl3)) / Convert.ToDouble(nondox.AUptosl4);

                            weightmod = Math.Ceiling(weightmod);

                            // AirAmount = Convert.ToDouble(nondox.Aslab3 + (nondox.Aslab4 * weightmod));
                            AirAmount = Convert.ToDouble((nondox.Aslab3 * nondox.AUptosl3) + (nondox.Aslab4 * weightmod));
                        }

                    }

                    Amount = AirAmount;
                }
                else if (mode.ToLower().StartsWith("s"))
                {

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
                            Amountsurf = Convert.ToDouble(nondox.Sslab2) * Math.Ceiling(highwaight);
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
                            Amountsurf = Convert.ToDouble(nondox.Sslab2) * Math.Ceiling(highwaight);
                        }
                        else if (highwaight <= nondox.SUptosl3)
                        {
                            Amountsurf = Convert.ToDouble(nondox.Sslab3);
                            Amountsurf = Convert.ToDouble(nondox.Sslab3) * Math.Ceiling(highwaight);
                        }
                        else
                        {
                            double weightmod = (highwaight - Convert.ToDouble(nondox.SUptosl3)) / Convert.ToDouble(nondox.SUptosl4);

                            weightmod = Math.Ceiling(weightmod);

                            Amountsurf = Convert.ToDouble((nondox.Sslab3 * nondox.SUptosl3) + (nondox.Sslab4 * weightmod));
                        }

                    }


                    Amount = Amountsurf;
                }


                //return Json(new { nonAisr = Amount, nonsurf = Amountsurf });

            }
            //long pin = Pincode;
















            //if Json ayyay amount is null then amount set to be zero




            ///////           




            return Json(new { Amount = Amount });
        }
    }
}