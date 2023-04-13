using DtDc_Billing.Entity_FR;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace DtDc_Billing.Models
{
    public class SessionAdminold : ActionFilterAttribute
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


           
        }

    }
    [OutputCache(CacheProfile = "Cachefast")]
    public class SessionUserModule : ActionFilterAttribute
    {
        private db_a92afa_frbillingEntities db = new db_a92afa_frbillingEntities();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string dataAdmin = Convert.ToString(HttpContext.Current.Session["UserName"]);

            int dataUser = Convert.ToInt32(HttpContext.Current.Session["EmpId"]);


            string dataUserType = Convert.ToString(HttpContext.Current.Session["UserType"]);
            
            if (dataUserType == "")
            {
                if (dataUser != 0)
                {
                    var types = db.Users.Where(m => m.User_Id == dataUser).Select(m => m.Usertype).FirstOrDefault();

                    string[] split = types.Split(',');
                

                    if (split.Contains("CashCounter") == true)
                    {
                        filterContext.Controller.ViewBag.CurrentUserType = "CashCounter";

                        var UserModuleList = db.getUserModuleList().Select(x => new UserModuleList
                        {
                            ModuleName = x.ModuleName,
                            PF_Code = x.PF_Code,
                            UserName = x.UserName,
                            Usertype = x.Usertype,
                            User_Id = x.User_Id

                        }).Where(d => d.Usertype == "CashCounter" && d.User_Id == dataUser).ToList();



                        filterContext.Controller.ViewBag.UserModList = UserModuleList;
                        base.OnActionExecuting(filterContext);
                    }
                    if (split.Contains("Billing") == true)
                    {

                        filterContext.Controller.ViewBag.CurrentUserTypeEmp = "Billing";

                        var UserModuleList = db.getUserModuleList().Select(x => new UserModuleList
                        {
                            ModuleName = x.ModuleName,
                            PF_Code = x.PF_Code,
                            UserName = x.UserName,
                            Usertype = x.Usertype,
                            User_Id = x.User_Id

                        }).Where(d => d.Usertype == "Billing" && d.User_Id == dataUser).ToList();



                        filterContext.Controller.ViewBag.UserBillingModList = UserModuleList;
                        base.OnActionExecuting(filterContext);
                    }
                }
            }
            else
            {
                filterContext.Controller.ViewBag.CurrentUserType = "";
            }

            if (dataUser != 0)
            {

                var types = db.Users.Where(m => m.User_Id == dataUser).Select(m => m.Usertype).FirstOrDefault();
               
                string[] split = types.Split(',');

                
                //foreach(var item in split)
                //{
                //    if (item == "CashCounter")
                //    {
                if (dataUserType == "CashCounter")
                {
                    var UserModuleList = db.getUserModuleList().Select(x => new UserModuleList
                    {
                        ModuleName = x.ModuleName,
                        PF_Code = x.PF_Code,
                        UserName = x.UserName,
                        Usertype = x.Usertype,
                        User_Id = x.User_Id

                    }).Where(d => d.Usertype == "CashCounter" && d.User_Id == dataUser).ToList();



                    filterContext.Controller.ViewBag.UserModList = UserModuleList;
                    base.OnActionExecuting(filterContext);
                }

                if (dataUserType == "Billing")
                {
                    var UserModuleList = db.getUserModuleList().Select(x => new UserModuleList
                    {
                        ModuleName = x.ModuleName,
                        PF_Code = x.PF_Code,
                        UserName = x.UserName,
                        Usertype = x.Usertype,
                        User_Id = x.User_Id

                    }).Where(d => d.Usertype == "Billing" && d.User_Id == dataUser).ToList();



                    filterContext.Controller.ViewBag.UserModList = UserModuleList;
                    base.OnActionExecuting(filterContext);
                }
             

            }

            }
        }
           
}
