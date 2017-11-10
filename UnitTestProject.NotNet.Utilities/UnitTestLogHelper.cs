using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using DotNet.Utilities.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.NotNet.Utilities
{
    [TestClass]
    public class UnitTestLogHelper:UnitTestBase
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var t = new StackTrace(1, true);
            //var type=t.GetFrame(0).GetMethod().DeclaringType;

            //var xx=log4net.LogManager.GetLogger(type);
            ////在应用程序启动时运行的代码
            //log4net.Config.XmlConfigurator.Configure();

            
            //log4net.ILog log = log4net.LogManager.GetLogger("AppLogger2");
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Conn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Categories WHERE CategoryID= 10 ", con);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message,ex);
                return;
            }

        }
    }
}
