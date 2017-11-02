using System;
using System.Data.Entity.Migrations;
using DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;


namespace UnitTestProject
{

    [TestClass]
    public class UnitTestDataInit
    {

        [TestMethod]
        public void Test_DataInit()
        {
            using (var dao = new DataBaseContext())
            {
                var user = new User
                {
                    Name = "555555555",
                    LoginTime=DateTime.Now
                };
                dao.Set<User>().AddOrUpdate(user);
                dao.SaveChanges();
            }
        }

    }

}