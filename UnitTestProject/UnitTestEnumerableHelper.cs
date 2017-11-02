using System;
using System.Collections.Generic;
using System.Linq;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;


namespace UnitTestProject
{
    [TestClass]
    public class UnitTestEnumerableHelper
    {
        [TestMethod]
        public void TestConvertDataTable()
        {
            var users = new List<User>()
            {
                new User {Name="John",Id=1 },
                new User {Name="John1",Id=2 },
                new User {Name="John2",Id=3 },
                new User {Name="John3",Id=4 },
                new User {Name="John4",Id=5 },
                new User {Name="John5",Id=6 },
                new User {Name="John6",Id=7 },
            };
            var table = users.ConvertDataTable();
        }


        [TestMethod]
        public void TestConvertDataTable2()
        {
            var users = new List<Student>()
            {
                new Student {Age=25,Name="John",Id=1 },
                new Student {Age=26,Name="John1",Id=2 },
                new Student {Age=27,Name="John2",Id=3 },
                new Student {Age=28,Name="John3",Id=4 },
                new Student {Age=29,Name="John4",Id=5 },
                new Student {Age=30,Name="John5",Id=6 },
                new Student {Age=31,Name="John6",Id=7 },
            };
            var table = users.ConvertDataTable(properties =>
            {
                var property = properties.FirstOrDefault(p => p.Name == "Id");
                properties.Remove(property);
                properties.Insert(0, property);
            });
        }

    }

    public class Person
    {

        public long Id { get; set; }

    }

    public class Student : Person
    {

        public string Name { get; set; }

        public int Age { get; set; }

    }
}
