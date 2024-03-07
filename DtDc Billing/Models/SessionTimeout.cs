using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DtDc_Billing.Models
{
    public class SessionTimeout : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["Cookies"]["AdminValue"].ToString() == null)
                {
                    filterContext.Result = new RedirectToRouteResult(
                          new RouteValueDictionary(
                              new
                              {
                                  controller = "Admin",
                                  action = "AdminLogin",

                                  returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                              }));

                }

                base.OnActionExecuting(filterContext);
            }
            catch (Exception ex)
            {

                filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary(
                          new
                          {
                              controller = "Admin",
                              action = "CookiesExpires",

                              returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                          }));



                base.OnActionExecuting(filterContext);

            }

        }
            
            //HttpContext ctx = HttpContext.Current;
            //if (HttpContext.Current.Session["EmpId"] == null || HttpContext.Current.Session["pfCode"] == null)
            //{
            //    filterContext.Result = new RedirectToRouteResult(
            //          new RouteValueDictionary(
            //              new
            //              {
            //                  //controller = "Employee",
            //                  //action = "EmpLogin",
            //                  controller = "Admin",
            //                 action = "AdminLogin",
                           
            //                  returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
            //              }));
            //}



           // base.OnActionExecuting(filterContext);
        
    }
}