using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;


namespace UnitTestProject
{
    [TestClass]
    public class UnitTestEf
    {
        /// <summary>
        /// 查询时生成新对象(数据库类)
        /// </summary>
        [TestMethod]
        public void Test_Select_New_DataBaseClass()
        {
            using (new TransactionScope())
            {
                using (var dao = new DataBaseContext())
                {
                    var users = new List<User>
                    {
                        new User {Age = 25, Name = "John"},
                        new User {Age = 26, Name = "John1"},
                        new User {Age = 27, Name = "John2"},
                        new User {Age = 28, Name = "John3"},
                        new User {Age = 29, Name = "John4"},
                        new User {Age = 30, Name = "John5"},
                        new User {Age = 31, Name = "John6"}
                    };
                    dao.Set<User>().AddRange(users);
                    dao.SaveChanges();
                    var user = dao.Set<User>().OrderByDescending(u => u.Id).First();
                    Assert.IsNotNull(user);

                    //先将旧数据分离
                    dao.Entry(user).State = EntityState.Detached;
                    var id = user.Id;
                    var newUser = new User { Id = id, Age = 45, Name = "Rose" };
                    dao.Entry(newUser).State = EntityState.Modified;
                    dao.SaveChanges();

                    try
                    {
                        //此处会出错
                        //查询时不能构造数据库类（和数据库表映射的类）
                        var managers = dao.Set<User>().Select(u => new Manager
                        {
                            Name = u.Name
                        }).ToList();
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail();
                    }
                }
            }
        }

        /// <summary>
        /// 查询时生成新对象
        /// </summary>
        [TestMethod]
        public void Test_Select_New_Class()
        {
            using (new TransactionScope())
            {
                using (var dao = new DataBaseContext())
                {
                    var users = new List<User>
                    {
                        new User {Age = 25, Name = "John"},
                        new User {Age = 26, Name = "John1"},
                        new User {Age = 27, Name = "John2"},
                        new User {Age = 28, Name = "John3"},
                        new User {Age = 29, Name = "John4"},
                        new User {Age = 30, Name = "John5"},
                        new User {Age = 31, Name = "John6"}
                    };
                    dao.Set<User>().AddRange(users);
                    dao.SaveChanges();
                    var user = dao.Set<User>().OrderByDescending(u => u.Id).First();
                    Assert.IsNotNull(user);

                    //先将旧数据分离
                    dao.Entry(user).State = EntityState.Detached;
                    var id = user.Id;
                    var newUser = new User { Id = id, Age = 45, Name = "Rose" };
                    dao.Entry(newUser).State = EntityState.Modified;
                    dao.SaveChanges();
                    //非数据库类不会出错
                    var managers = dao.Set<User>().Select(u => new Test
                    {
                        Name = u.Name
                    }).ToList();
                }
            }
        }
        internal class Test
        {

            public string Name { get; set; }

        }
    }
}
