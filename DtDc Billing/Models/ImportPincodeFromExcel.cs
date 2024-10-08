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
    public class ImportPincodeFromExcel
    {
        public static db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        public string ImportPincodeAsync(HttpPostedFileBase httpPostedFileBase, string PfCode)
        {
            try
            {
                var damageResult = Task.Run(() => asyncAddPincodeImportFromExcel(httpPostedFileBase, PfCode));

                return damageResult.ToString();
            }
            catch (Exception ex)
            {
                throw new RedirectException(ex.Message);
            }
        }


        public static async Task<string> asyncAddPincodeImportFromExcel(HttpPostedFileBase httpPostedFileBase, string PfCode)
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
                            var des = new DtDc_Billing.Entity_FR.Destination();


                            try
                            {

                                des.Pincode = workSheet.Cells[rowIterator, 2]?.Value?.ToString()?.Trim() ?? null;
                                des.Name = workSheet.Cells[rowIterator, 3]?.Value?.ToString().Trim()??null;
                                if (des.Pincode != null && des.Name != null)
                                {
                                    var destination=db.Destinations.Where(x=>x.Pincode==des.Pincode).FirstOrDefault();
                                    if (destination == null)
                                    {
                                        des.Name=des.Name.ToUpper();
                                        db.Destinations.Add(des);
                                        db.SaveChanges();

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