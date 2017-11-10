using System;
using System.IO;
using System.Web.Configuration;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.NotNet.Utilities
{
    [TestClass]
    public class UnitTestBase
    {

        public UnitTestBase()
        {
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
