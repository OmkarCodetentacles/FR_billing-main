using DtDc_Billing.Entity_FR;
using DtDc_Billing.Metadata_Classes;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace DtDc_Billing.Models
{
    public static class ExportToExcelAll
    {
        public static void ExportToExcelAdmin(object rc)
        {
            //string pfcode = Session["pfCode"].ToString();

            var cons = rc;

            var gv = new GridView();
            gv.DataSource = cons;
            gv.DataBind();
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=ConsignmentExcel.xls");
           // System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";
            System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            System.Web.HttpContext.Current.Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            System.Web.HttpContext.Current.Response.Output.Write(objStringWriter.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();

        }


        public static void ExportFirstExcelFormat(IEnumerable<TransactionMetadata> transactions)
        {
            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add("Consignments");

                // Optional: Add headers
                worksheet.Cells[1, 1].Value = "Sr No";
                worksheet.Cells[1, 2].Value = "Consignment No";
                worksheet.Cells[1, 3].Value = "Customer Id";

                int row = 2;
                int srNo = 1;
                foreach (var transaction in transactions)
                {
                    worksheet.Cells[row, 1].Value =transaction.SrNo;
                    worksheet.Cells[row, 2].Value = transaction.Consignment_no;
                    worksheet.Cells[row, 3].Value = transaction.Customer_Id;
                    // Map additional properties as needed
                    row++;
                }

                // AutoFit columns if needed
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Buffer = true;
                // Set the response headers for an .xlsx file
                response.AddHeader("content-disposition", "attachment; filename=FirstExcelFormat.xlsx");
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.Charset = "";

                // Write the file content to the response output stream.
                using (var memoryStream = new MemoryStream())
                {
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(response.OutputStream);
                    response.Flush();
                    response.End();
                }
            }
        }


        public static void ExportSecondExcelFormat(IEnumerable<TransactionMetadata> transactions)
        {
            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add("Consignments");

                // Optional: Add headers
                worksheet.Cells[1, 1].Value = "Sr No";
                worksheet.Cells[1, 2].Value = "Consignment No";
                worksheet.Cells[1, 3].Value = "Customer Id";
                worksheet.Cells[1, 4].Value = "Chargeable Weight";
                worksheet.Cells[1, 5].Value = "Insurance Amount";
                worksheet.Cells[1, 6].Value = "Fov Amount";
                worksheet.Cells[1, 7].Value = "Fov Percentage";
                worksheet.Cells[1, 8].Value = "OtherCharges";

                int row = 2;
                int srNo = 1;
                foreach (var transaction in transactions)
                {
                    worksheet.Cells[row, 1].Value = transaction.SrNo;
                    worksheet.Cells[row, 2].Value = transaction.Consignment_no;
                    worksheet.Cells[row, 3].Value = transaction.Customer_Id;
                    worksheet.Cells[row, 4].Value = transaction.chargable_weight;
                    worksheet.Cells[row, 5].Value =0;
                    worksheet.Cells[row, 6].Value = transaction.BillAmount;
                    worksheet.Cells[row, 7].Value = transaction.Percentage;
                    worksheet.Cells[row, 8].Value = transaction.loadingcharge;
                    // Map additional properties as needed
                    row++;
                }

                // AutoFit columns if needed
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Buffer = true;
                // Set the response headers for an .xlsx file
                response.AddHeader("content-disposition", "attachment; filename=SecondExcelFormat.xlsx");
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.Charset = "";

                // Write the file content to the response output stream.
                using (var memoryStream = new MemoryStream())
                {
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(response.OutputStream);
                    response.Flush();
                    response.End();
                }
            }
        }

        public static void ExportThirdExcelFormat(IEnumerable<TransactionMetadata> transactions)
        {
            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add("Consignments");

                // Optional: Add headers
                worksheet.Cells[1, 1].Value = "Sr No";
                worksheet.Cells[1, 2].Value = "Consignment No";
                worksheet.Cells[1, 3].Value = "Chargeable Weight";
                worksheet.Cells[1, 4].Value = "Mode";
                worksheet.Cells[1, 5].Value = "Company Address";
                worksheet.Cells[1, 6].Value = "Quantity";
                worksheet.Cells[1, 7].Value = "PinCode";
                worksheet.Cells[1, 8].Value = "Booking Date(dd/MM/yyyy or dd-MM-yyyy)";
                worksheet.Cells[1, 9].Value = "Type(D or N)";
                worksheet.Cells[1, 10].Value = "Customer Id";
                worksheet.Cells[1, 11].Value = "OtherCharges";
                worksheet.Cells[1, 12].Value = "Receiver";
                worksheet.Cells[1, 13].Value = "Amount(optional)";

                int row = 2;
                int srNo = 1;
                foreach (var transaction in transactions)
                {
                    worksheet.Cells[row, 1].Value = transaction.SrNo;
                    worksheet.Cells[row, 2].Value = transaction.Consignment_no;
                    worksheet.Cells[row, 3].Value = transaction.chargable_weight;
                    worksheet.Cells[row, 4].Value = transaction.Mode;
                    worksheet.Cells[row, 5].Value = transaction.compaddress;
                    worksheet.Cells[row, 6].Value = transaction.Quanntity;
                    worksheet.Cells[row, 7].Value = transaction.Pincode;
                    worksheet.Cells[row, 8].Value = transaction.tembookingdate;
                    worksheet.Cells[row, 9].Value = transaction.Type_t;
                    worksheet.Cells[row, 10].Value = transaction.Customer_Id;
                    worksheet.Cells[row, 11].Value = transaction.loadingcharge;
                    worksheet.Cells[row, 12].Value = transaction.Receiver;
                    worksheet.Cells[row, 13].Value = transaction.Amount;
                    // Map additional properties as needed
                    row++;
                }

                // AutoFit columns if needed
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Buffer = true;
                // Set the response headers for an .xlsx file
                response.AddHeader("content-disposition", "attachment; filename=ThirdExcelFormat.xlsx");
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.Charset = "";

                // Write the file content to the response output stream.
                using (var memoryStream = new MemoryStream())
                {
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(response.OutputStream);
                    response.Flush();
                    response.End();
                }
            }
        }


    }
}