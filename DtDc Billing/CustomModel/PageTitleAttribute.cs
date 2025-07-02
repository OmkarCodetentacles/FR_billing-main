using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.CustomModel
{
    public class PageTitleAttribute:ActionFilterAttribute
    {
        private readonly string _title;

        public PageTitleAttribute(string title)
        {
            _title = title;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.Title = _title;
            base.OnResultExecuting(filterContext);
        }
    }
}