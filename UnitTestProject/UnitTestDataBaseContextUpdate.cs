using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Peiyong.DataAccess;
using Peiyong.Models.Entities;


namespace UnitTestProject
{

    [TestClass]
    public class UnitTestDataBaseContextUpdate
    {

        [TestMethod]
        public void TestUpdate()
        {
            using (new TransactionScope())
            {
                using (var dao = new DataBaseContext())
                {
                    var users = new List<User>
                    {
                        new User { Name = "John"},
                        new User {Name = "John1"},
                        new User { Name = "John2"},
                        new User {Name = "John3"},
                        new User {Name = "John4"},
                        new User {Name = "John5"},
                        new User { Name = "John6"}
                    };
                    dao.Set<User>().AddRange(users);
                    dao.SaveChanges();
                    var user = dao.Set<User>().OrderByDescending(u => u.Id).First();
                    Assert.IsNotNull(user);

                    //先将旧数据分离
                    dao.Entry(user).State = EntityState.Detached;
                    var id = user.Id;
                    var newUser = new User {Id = id,  Name = "Rose"};
                    dao.Entry(newUser).State = EntityState.Modified;
                    dao.SaveChanges();

                    user = dao.Set<User>().AsNoTracking().FirstOrDefault(u => u.Id == id);
                    Assert.IsNotNull(user);
                    Assert.IsTrue(user.Name == newUser.Name);
                }
            }
        }

    }

}