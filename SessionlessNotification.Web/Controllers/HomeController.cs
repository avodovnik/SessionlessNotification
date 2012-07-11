using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SessionlessNotification.Web.App_Start;

namespace SessionlessNotification.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Post()
        {
            SessionlessNotification.Web.App_Start.Notifications.NotificationModule.AddNotification(new Notifications.NotificationModule.NotificationMessage()
                                                                                                       {
                                                                                                           Message = "This is a notification.",
                                                                                                           Type = "info"
                                                                                                       });

            return View();
        }
    }
}
