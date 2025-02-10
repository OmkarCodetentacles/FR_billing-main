using DocumentFormat.OpenXml.ExtendedProperties;
using DtDc_Billing.Controllers;
using DtDc_Billing.CustomModel;
using DtDc_Billing.Entity_FR;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Models
{
    public class ImportComapnyFromExcel
    {
        public static db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        public string ImportComapnyAsync(HttpPostedFileBase httpPostedFileBase, string PfCode)
        {
            try
            {
                var damageResult = Task.Run(() => asyncAddCompanyimporFromExcel(httpPostedFileBase, PfCode));

                return damageResult.ToString();
            }
            catch (Exception ex)
            {
                throw new RedirectException(ex.Message);
            }
        }


        public static async Task<string> asyncAddCompanyimporFromExcel(HttpPostedFileBase httpPostedFileBase, string PfCode)
        {


            if (httpPostedFileBase != null)
            {


                HttpPostedFileBase file = httpPostedFileBase;
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                   // BookingController admin = new BookingController();
                    var getPfcode = PfCode;



                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var comp = new DtDc_Billing.Entity_FR.Company();


                            try
                            {
                                if (workSheet.Cells[rowIterator, 2]?.Value?.ToString() != null)
                                {

                                    var companyid = workSheet.Cells[rowIterator, 2]?.Value?.ToString().Trim();
                                    var abc = db.Companies.Where(m => m.Company_Id.ToLower() == companyid.ToLower() && m.Pf_code==PfCode).FirstOrDefault();

                                   

                                        comp.Company_Id = workSheet.Cells[rowIterator, 2]?.Value?.ToString();
                                        comp.Company_Name = workSheet.Cells[rowIterator, 3]?.Value?.ToString();
                                        comp.Company_Address = workSheet.Cells[rowIterator, 4]?.Value?.ToString();
                                        comp.Phone = Convert.ToInt64(workSheet.Cells[rowIterator, 5]?.Value);
                                        comp.Email = workSheet.Cells[rowIterator, 6]?.Value?.ToString();
                                        comp.Insurance = Convert.ToDouble(workSheet.Cells[rowIterator, 7]?.Value ?? 0);
                                        comp.Minimum_Risk_Charge = Convert.ToDouble(workSheet.Cells[rowIterator, 8]?.Value ?? 0);
                                        comp.Other_Details = workSheet.Cells[rowIterator, 9]?.Value?.ToString();
                                        comp.Topay_Charge = Convert.ToDouble(workSheet.Cells[rowIterator, 10]?.Value ?? 0);
                                        comp.Cod_Charge = Convert.ToDouble(workSheet.Cells[rowIterator, 11]?.Value ?? 0);
                                        comp.Fuel_Sur_Charge = Convert.ToDouble(workSheet.Cells[rowIterator, 12]?.Value ?? 0);
                                        comp.Gec_Fuel_Sur_Charge = Convert.ToDouble(workSheet.Cells[rowIterator, 13]?.Value ?? 0);
                                        comp.Royalty_Charges = Convert.ToDouble(workSheet.Cells[rowIterator, 14]?.Value ?? 0);
                                        comp.Gst_No = workSheet.Cells[rowIterator, 15]?.Value?.ToString();
                                        comp.Pan_No = workSheet.Cells[rowIterator, 16]?.Value?.ToString();
                                        comp.DueDays = Convert.ToInt32(workSheet.Cells[rowIterator, 17]?.Value ?? 0);
                                        comp.D_Docket = Convert.ToDouble(workSheet.Cells[rowIterator, 18]?.Value ?? 0);
                                        comp.P_Docket = Convert.ToDouble(workSheet.Cells[rowIterator, 19]?.Value ?? 0);
                                        comp.E_Docket = Convert.ToDouble(workSheet.Cells[rowIterator, 20]?.Value ?? 0);
                                        comp.V_Docket = Convert.ToDouble(workSheet.Cells[rowIterator, 21]?.Value ?? 0);
                                        comp.I_Docket = Convert.ToDouble(workSheet.Cells[rowIterator, 22]?.Value ?? 0);
                                        comp.N_Docket = Convert.ToDouble(workSheet.Cells[rowIterator, 23]?.Value ?? 0);
                                        comp.G_Docket = Convert.ToDouble(workSheet.Cells[rowIterator, 24]?.Value ?? 0);
                                        comp.B_Docket = Convert.ToDouble(workSheet.Cells[rowIterator,25]?.Value ?? 0);
                                        comp.Datetime_Comp = DateTime.Now;
                                        comp.Pf_code=getPfcode.Trim();

                                        if(!string.IsNullOrEmpty(comp.Company_Id) || !string.IsNullOrEmpty(comp.Company_Name) || !string.IsNullOrEmpty(comp.Phone.ToString()) || !string.IsNullOrEmpty(comp.Email) || !string.IsNullOrEmpty(comp.Company_Address))
                                        {
                                           // Special characters validation regex
                                             string specialCharacters = @".\/<>@#%*&()";
                                            Regex regex = new Regex("[" + Regex.Escape(specialCharacters) + "]");

                                            // Check Company_Id
                                            if (regex.IsMatch(companyid))
                                            {
                                                //// Handle invalid input, e.g., skip the row or log an error
                                                //throw new Exception("Company_Id contains invalid characters.");
                                                continue;
                                            }
                                            else
                                            {
                                            
                                            if(abc == null)
                                            {
                                                db.Companies.Add(comp);
                                                db.SaveChanges();

                                                var secotrs = db.Sectors.Where(m => m.Pf_code == getPfcode && m.BillGecSec != true).ToList();
                                                var pfcompany = db.Companies.Where(m => m.Pf_code == getPfcode && (!m.Company_Id.StartsWith("Cash"))).Select(x => x.Company_Id).FirstOrDefault();
                                                
                                                var basicdox = db.Ratems.Where(m => m.Company_id == pfcompany).ToArray();
                                                var basicnon = db.Nondoxes.Where(m => m.Company_id == pfcompany).ToArray();
                                                var express = db.express_cargo.Where(m => m.Company_id == pfcompany).ToArray();
                                                var basicplu = db.dtdcPlus.Where(m => m.Company_id == pfcompany).ToArray();
                                                var basicptp = db.Dtdc_Ptp.Where(m => m.Company_id == pfcompany).ToArray();
                                                var basicprio=db.Priorities.Where(m=>m.Company_id==pfcompany).ToArray();
                                                var basicecom = db.Dtdc_Ecommerce.Where(m => m.Company_id == pfcompany).ToArray();
                                                int j = 0;

                                                foreach (var i in secotrs)
                                                {
                                                    Ratem dox = new Ratem();
                                                    Nondox ndox = new Nondox();
                                                    express_cargo cs = new express_cargo();
                                                    Priority pri = new Priority();
                                                    Dtdc_Ecommerce dtdc_Ecommerce = new Dtdc_Ecommerce();

                                                    dox.Company_id = companyid;
                                                    dox.Sector_Id = i.Sector_Id;
                                                    dox.NoOfSlab = 2;

                                                    dox.slab1 = basicdox[j].slab1;
                                                    dox.slab2 = basicdox[j].slab2;
                                                    dox.slab3 = basicdox[j].slab3;
                                                    dox.slab4 = basicdox[j].slab4;

                                                    dox.Uptosl1 = basicdox[j].Uptosl1;
                                                    dox.Uptosl2 = basicdox[j].Uptosl2;
                                                    dox.Uptosl3 = basicdox[j].Uptosl3;
                                                    dox.Uptosl4 = basicdox[j].Uptosl4;

                                                    ndox.Company_id= companyid;
                                                    ndox.Sector_Id = i.Sector_Id;
                                                    ndox.NoOfSlabN = 2;
                                                    ndox.NoOfSlabS = 2;

                                                    ndox.Aslab1 = basicnon[j].Aslab1;
                                                    ndox.Aslab2 = basicnon[j].Aslab2;
                                                    ndox.Aslab3 = basicnon[j].Aslab3;
                                                    ndox.Aslab4 = basicnon[j].Aslab4;


                                                    ndox.Sslab1 = basicnon[j].Sslab1;
                                                    ndox.Sslab2 = basicnon[j].Sslab2;
                                                    ndox.Sslab3 = basicnon[j].Sslab3;
                                                    ndox.Sslab4 = basicnon[j].Sslab4;

                                                    ndox.AUptosl1 = basicnon[j].AUptosl1;
                                                    ndox.AUptosl2 = basicnon[j].AUptosl2;
                                                    ndox.AUptosl3 = basicnon[j].AUptosl3;
                                                    ndox.AUptosl4 = basicnon[j].AUptosl4;

                                                    ndox.SUptosl1 = basicnon[j].SUptosl1;
                                                    ndox.SUptosl2 = basicnon[j].SUptosl2;
                                                    ndox.SUptosl3 = basicnon[j].SUptosl3;
                                                    ndox.SUptosl4 = basicnon[j].SUptosl4;


                                                    cs.Company_id = companyid;
                                                    cs.Sector_Id = i.Sector_Id;

                                                    cs.Exslab1 = express[j].Exslab1;
                                                    cs.Exslab2 = express[j].Exslab2;

                                                    pri.Company_id = companyid;
                                                    pri.Sector_Id = i.Sector_Id;
                                                    pri.prinoofslab = 2;

                                                    pri.prislab1 = basicprio[j].prislab1;
                                                    pri.prislab2 = basicprio[j].prislab2;
                                                    pri.prislab3 = basicprio[j].prislab3;
                                                    pri.prislab4 = basicprio[j].prislab4;

                                                    pri.priupto1 = basicprio[j].priupto1;
                                                    pri.priupto2 = basicprio[j].priupto2;
                                                    pri.priupto3 = basicprio[j].priupto3;
                                                    pri.priupto4 = basicprio[j].priupto4;

                                                    cs.Company_id = companyid;
                                                    cs.Sector_Id = i.Sector_Id;

                                                    cs.Exslab1 = express[j].Exslab1;
                                                    cs.Exslab2 = express[j].Exslab2;

                                                    dtdc_Ecommerce.Company_id = companyid;
                                                    dtdc_Ecommerce.Sector_Id = i.Sector_Id;
                                                    dtdc_Ecommerce.EcomPslab1 = basicecom[j].EcomPslab1;
                                                    dtdc_Ecommerce.EcomPslab2 = basicecom[j].EcomPslab2;
                                                    dtdc_Ecommerce.EcomPslab3 = basicecom[j].EcomPslab3;
                                                    dtdc_Ecommerce.EcomPslab4 = basicecom[j].EcomPslab4;
                                                    dtdc_Ecommerce.EcomGEslab1 = basicecom[j].EcomGEslab1;
                                                    dtdc_Ecommerce.EcomGEslab2 = basicecom[j].EcomGEslab2;
                                                    dtdc_Ecommerce.EcomGEslab3 = basicecom[j].EcomGEslab3;
                                                    dtdc_Ecommerce.EcomGEslab4 = basicecom[j].EcomGEslab4;
                                                    dtdc_Ecommerce.EcomPupto1 = basicecom[j].EcomPupto1;
                                                    dtdc_Ecommerce.EcomPupto2 = basicecom[j].EcomPupto2;
                                                    dtdc_Ecommerce.EcomPupto3 = basicecom[j].EcomPupto3;
                                                    dtdc_Ecommerce.EcomPupto4 = basicecom[j].EcomPupto4;
                                                    dtdc_Ecommerce.EcomGEupto1 = basicecom[j].EcomGEupto1;
                                                    dtdc_Ecommerce.EcomGEupto2 = basicecom[j].EcomGEupto2;
                                                    dtdc_Ecommerce.EcomGEupto3 = basicecom[j].EcomGEupto3;
                                                    dtdc_Ecommerce.EcomGEupto4 = basicecom[j].EcomGEupto4;
                                                    dtdc_Ecommerce.NoOfSlabN = 2;
                                                    dtdc_Ecommerce.NoOfSlabS = 2;


                                                    db.Ratems.Add(dox);
                                                    db.Nondoxes.Add(ndox);
                                                    db.express_cargo.Add(cs);
                                                    db.Priorities.Add(pri);
                                                    db.Dtdc_Ecommerce.Add(dtdc_Ecommerce);

                                                    j++;

                                                }

                                                int p = 0;

                                           

                                                for (int i = 0; i < 5; i++)
                                                {

                                                    dtdcPlu dtplu = new dtdcPlu();
                                                    Dtdc_Ptp stptp = new Dtdc_Ptp();

                                                    if (i == 0)
                                                    {
                                                        dtplu.destination = "City Plus";
                                                        stptp.dest = "City";
                                                    }
                                                    else if (i == 1)
                                                    {
                                                        dtplu.destination = "Zonal Plus/Blue";
                                                        stptp.dest = "Zonal";

                                                    }
                                                    else if (i == 2)
                                                    {
                                                        dtplu.destination = "Metro Plus/Blue";
                                                        stptp.dest = "Metro";
                                                    }
                                                    else if (i == 3)
                                                    {
                                                        dtplu.destination = "National Plus/Blue";
                                                        stptp.dest = "National";
                                                    }
                                                    else if (i == 4)
                                                    {
                                                        dtplu.destination = "Regional Plus";
                                                        stptp.dest = "Regional";
                                                    }

                                                    dtplu.Company_id = companyid;

                                                    dtplu.Upto500gm = basicplu[p].Upto500gm;
                                                    dtplu.U10to25kg = basicplu[p].U10to25kg;
                                                    dtplu.U25to50 = basicplu[p].U25to50;
                                                    dtplu.U50to100 = basicplu[p].U50to100;
                                                    dtplu.add100kg = basicplu[p].add100kg;
                                                    dtplu.Add500gm = basicplu[p].Add500gm;


                                                    stptp.Company_id = companyid;
                                                    stptp.PUpto500gm = basicptp[p].PUpto500gm;
                                                    stptp.PAdd500gm = basicptp[p].PAdd500gm;
                                                    stptp.PU10to25kg = basicptp[p].PU10to25kg;
                                                    stptp.PU25to50 = basicptp[p].PU25to50;
                                                    stptp.Padd100kg = basicptp[p].Padd100kg;
                                                    stptp.PU50to100 = basicptp[p].PU50to100;

                                                    stptp.P2Upto500gm = basicptp[p].P2Upto500gm;
                                                    stptp.P2Add500gm = basicptp[p].P2Add500gm;
                                                    stptp.P2U10to25kg = basicptp[p].P2U10to25kg;
                                                    stptp.P2U25to50 = basicptp[p].P2U25to50;
                                                    stptp.P2add100kg = basicptp[p].P2add100kg;
                                                    stptp.P2U50to100 = basicptp[p].P2U50to100;

                                                    db.dtdcPlus.Add(dtplu);
                                                    db.Dtdc_Ptp.Add(stptp);

                                                    p++;

                                                }
                                                db.SaveChanges();

                                              




                                            }
                                            else
                                            {

                                                abc.Company_Name = comp.Company_Name;
                                                abc.Company_Address = comp.Company_Address;
                                                abc.Phone = comp.Phone;
                                                abc.Email = comp.Email;
                                                abc.Insurance = comp.Insurance;
                                                abc.Minimum_Risk_Charge = comp.Minimum_Risk_Charge;
                                                abc.Other_Details = comp.Other_Details;
                                                abc.Topay_Charge = comp.Topay_Charge;
                                                abc.Cod_Charge = comp.Cod_Charge;
                                                abc.Fuel_Sur_Charge = comp.Fuel_Sur_Charge;
                                                abc.Gec_Fuel_Sur_Charge = comp.Gec_Fuel_Sur_Charge;
                                                abc.Royalty_Charges = comp.Royalty_Charges;
                                                abc.Gst_No = comp.Gst_No;
                                                abc.Pan_No = comp.Pan_No;
                                                abc.DueDays = comp.DueDays;
                                                abc.D_Docket = comp.D_Docket;
                                                abc.P_Docket = comp.P_Docket;
                                                abc.E_Docket = comp.E_Docket;
                                                abc.V_Docket = comp.V_Docket;
                                                abc.I_Docket = comp.I_Docket;
                                                abc.N_Docket = comp.N_Docket;
                                                abc.G_Docket = comp.G_Docket;
                                                abc.Datetime_Comp = DateTime.Now;
                                                abc.Pf_code = getPfcode.Trim();

                                                db.Entry(abc).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }

                                        }


                                    }
                                   
                                }
                                }
                            catch (Exception ex)
                            {
                                throw new RedirectException(ex.Message);
                            }
                        }
                    }

                }
            }
            return "1";
        }

    }
}