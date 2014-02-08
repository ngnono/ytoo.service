using System;
using System.Threading;
using com.intime.fashion.data.sync.runner.Runner;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.fashion.data.sync.runner
{
    class Program
    {
        private const string Usages = @"请选择要执行的任务，输入序号后回车执行：
    1. 备份已上传的商品。
    2. 下架已上传的商品
    3. 上传一个测试商品
    4. 上传商品
    5. 同步商品库存
    6. 下架商品
    7. 上架商品
    8. 同步已支付订单
    9. 同步品牌信息
    Q. 退出";

        static void Main()
        {

            ServiceLocator.Current.RegisterSingleton<Yintai.Architecture.Common.Logger.ILog, Yintai.Architecture.Common.Logger.Log4NetLog>();
            Console.Title = "银泰微购物商品上架助手";
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("{0}已启动", Console.Title);
            Console.WriteLine(Usages);

            var input = Console.ReadLine();

            while (true)
            {
                while(string.IsNullOrEmpty(input))
                {
                    Console.WriteLine(Usages);
                    input = Console.ReadLine();
                }
                var c = input.ToUpper().Trim();

                switch (c)
                {
                    case "Q":
                        Console.WriteLine("退出执行...");
                        Thread.Sleep(300);
                        return;
                    case "1":
                        new LoadOnlineItemsRunner().Run();
                        break;
                    case "2":
                        new DownItemRunner().Run();
                        break;
                    case "3":
                        new UploadTestProductRunner().Run();
                        break;
                    case "4":
                        return;
                    case "5":
                        return;
                    case "6":
                        return;
                    case "7":
                        return;
                    case "8":
                        new LoadPaidOrderRunner().Run();
                        break;
                    case "9":
                        new MapBrandRunner().Run();
                        break;
                    default:
                        Console.WriteLine(Usages);
                        break;

                }
                input = Console.ReadLine();
            }

        }
    }
}