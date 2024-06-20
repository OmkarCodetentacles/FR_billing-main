using DtDc_Billing.Entity_FR;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
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
            System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";
            System.Web.HttpContext.Current.Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            System.Web.HttpContext.Current.Response.Output.Write(objStringWriter.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();

        }
      
    }
}