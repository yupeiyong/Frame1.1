using System;
using System.Web.Mvc;
using Peiyong.CommonController.Controllers;


namespace Peiyong.WebPc.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            var x = Convert.ToInt16("ddddd");
            return View();
        }
    }
}
