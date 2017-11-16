using System;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using DotNet.Utilities.Log;


namespace Peiyong.CommonController.Controllers
{
    public class BaseController:Controller
    {

        protected override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled && filterContext.Exception != null)
            {
                var controllerName = filterContext.RouteData.Values["controller"].ToString();
                var actionName = filterContext.RouteData.Values["action"].ToString();
                var areaName = filterContext.RouteData.DataTokens["area"];
                var erroMsg = $"页面未捕获的异常：Area:{areaName},Controller:{controllerName},Action:{actionName}";
               LogHelper.WriteLog(erroMsg, filterContext.Exception);
                //将状态码更新为200，否则在Web.config中配置了CustomerError后，Ajax会返回500错误导致页面不能正确显示错误信息  
                filterContext.HttpContext.Response.StatusCode = 200;
                filterContext.ExceptionHandled = true;

                Response.Redirect("~/Exception/Error");
            }
            base.OnException(filterContext);
        }

    }
}
