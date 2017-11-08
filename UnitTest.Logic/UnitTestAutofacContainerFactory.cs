using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Peiyong.Logic.Aultofac;


namespace UnitTest.Logic
{
    [TestClass]
    public class UnitTestAutofacContainerFactory
    {
        [TestMethod]
        public void TestGetRegisteringTypes()
        {
            var types = AutofacContainerFactory.GetRegisteringTypes(new[] { "Peiyong.", "DotNet." });
            Assert.IsTrue(types.Length > 0);
        }
    }
}
