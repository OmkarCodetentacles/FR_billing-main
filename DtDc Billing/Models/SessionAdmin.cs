using DtDc_Billing.Entity_FR;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;



namespace DtDc_Billing.Models
{
    public class SessionAdmin : ActionFilterAttribute
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["Admin"] == null)
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

    }
   
}
