using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DtDc_Billing.Controllers
{
    public class NlogWithMVCController : Controller
    {
        public class HomeController : Controller
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            public ActionResult Index()
            {


                try
                {
                    int x = 0;
                    int y = 5;
                    int z = y / x;
                }
                catch (Exception ex)
                {
                    logger.ErrorException("Error occured in Home controller Index Action", ex);

                }
                return View();
            }

            public ActionResult About()
            {
                ViewBag.Message = "Your application description page.";

                return View();
            }

            public ActionResult Contact()
            {
                ViewBag.Message = "Your contact page.";

                return View();
            }
        }
    }
}