using System;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net.Config;
using WebPc;


namespace Peiyong.WebPc
{

    public class WebApiApplication : HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //初始化日志文件 
            var state = WebConfigurationManager.AppSettings["IsWriteLog"];

            //判断是否开启日志记录
            if (state == "1")
            {
                var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase +
                           WebConfigurationManager.AppSettings["log4net"];
                var fi = new FileInfo(path);
                XmlConfigurator.Configure(fi);
            }
        }

    }

}