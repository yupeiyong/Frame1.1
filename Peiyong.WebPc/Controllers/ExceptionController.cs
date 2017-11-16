using System.Web.Mvc;
using Peiyong.Models.Entities;


namespace Peiyong.WebPc.Controllers
{

    public class ExceptionController : Controller
    {
        public ActionResult Error(BaseException ex)
        {
            return View(ex);
        }
    }

}