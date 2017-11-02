using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTestAsyncAwait
    {
        [TestMethod]
        public void Test_Thread()
        {
            Console.WriteLine("主线程测试开始..");
            Thread th = new Thread(ThMethod);
            th.Start();
            Console.WriteLine("主线程测试结束..");
            Thread.Sleep(10000);
        }

        static void ThMethod()
        {
            Console.WriteLine("异步执行开始");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("异步执行" + i.ToString() + "..");
                Thread.Sleep(1000);
            }
            Console.WriteLine("异步执行完成");
        }

        [TestMethod]
        public void Test_Async_Task()
        {
            Console.WriteLine("主线程测试开始..");
            AsyncMethod();
            Console.WriteLine("主线程测试结束..");
            Thread.Sleep(100000);
        }

        static async void AsyncMethod()
        {
            Console.WriteLine("开始异步代码");
            var result = await MyMethod();
            Console.WriteLine("异步代码执行完毕");
        }


        static async Task<int> MyMethod()
        {
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine("异步执行" + i.ToString() + "..");
                await Task.Delay(1000); //模拟耗时操作
            }
            return 0;
        }
    }
}
