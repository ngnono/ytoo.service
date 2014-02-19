using System;
using System.Threading;
using com.intime.fashion.data.sync.runner.Runner;
using com.intime.fashion.data.sync.Wgw.Executor;
using Common.Logging;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.fashion.data.sync.runner
{
    class Program
    {
        private const string Usages = @"请选择要执行的任务，输入序号后回车执行：
    1. 备份已上传的商品。
    2. 下架已上传的商品
    3. 上传一个测试商品
    4. 查询商品详情
    5. 查询多库存
    6. Upload all products
    7. Sync product image
    8. 同步已支付订单
    9. 同步品牌信息
    Q. 退出";

        static void Main()
        {

            ServiceLocator.Current.RegisterSingleton<Yintai.Architecture.Common.Logger.ILog, Yintai.Architecture.Common.Logger.Log4NetLog>();
            Console.Title = "银泰微购物商品上架助手";
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Yellow;
            ILog logger = LogManager.GetLogger(typeof (Program));
            Console.WriteLine("{0}已启动", Console.Title);
            Console.WriteLine(Usages);

            var input = Console.ReadLine();

            while (true)
            {
                while (string.IsNullOrEmpty(input))
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
                        Console.WriteLine("请输入微客多的商品编码:");
                        string itemId = Console.ReadLine();
                        while (string.IsNullOrEmpty(itemId))
                        {
                            Console.WriteLine("请输入微客多的商品编码:");
                            itemId = Console.ReadLine();
                        }
                        new QueryItemDetailRunner(itemId).Run();
                        break;
                    case "5":
                        new QueryMultiStockRunner().Run();
                        break;
                    case "6":
                        new ProductSyncExecutor(DateTime.Now.AddYears(-1), logger)
                            .Execute();
                        break;
                    case "7":
                        new ProductImageSyncExecutor(DateTime.Now.AddYears(-1), logger).Execute();
                        break;
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