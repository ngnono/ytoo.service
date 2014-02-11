using com.intime.fashion.data.sync.Wgw;
using com.intime.fashion.data.sync.Wgw.Request.Builder;
using com.intime.fashion.data.sync.Wgw.Request.Item;
using com.intime.fashion.data.sync.Wgw.Response.Processor;
using System;
using System.Linq;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.fashion.data.sync.runner.Runner
{
    public class UploadTestProductRunner:Runner
    {
        protected override void Do()
        {
            Console.WriteLine("请输入测试商品ID");
            int productId;
            while (!int.TryParse(Console.ReadLine(), out productId) || !ValidateProduct(productId))
            {
                if (productId == 0)
                {
                    Console.WriteLine("请输入有效的测试商品ID，Ctrl + C 键退出");
                }
            }

            this.UploadProduct(productId);
        }

        private void UploadProduct(int productId)
        {
            using (var db = DbContextHelper.GetDbContext())
            {
                var item = db.Products.FirstOrDefault(t => t.Id == productId);

                try
                {
                    var builder = RequestParamsBuilderFactory.CreateBuilder(new AddItemRequest());
                    var result = Client.Execute<dynamic>(builder.BuildParameters(item));
                    if (result.errorCode == 0)
                    {
                        var processor = ProcessorFactory.CreateProcessor<ItemResponseProcessor>();
                        if (processor.Process(result, item.Id))
                        {
                            Console.WriteLine("成功上传商品");
                            return;
                        }
                        Logger.Error(string.Format("Failed to upload product to wgw {0}({1}) Error Message: {2}", item.Name,
                            item.Id, processor.ErrorMessage));
                    }
                    else //if (result.errorCode == 50005) //图片读取问题导致的失败实际上商品有可能已经上传成功需要补救下
                    {
                        var request = new QueryItemListRequest();
                        request.Put("defStockId", item.Id);
                        request.Put("startIndex", 0);
                        request.Put("pageSize", 1);
                        request.Put("orderType", "7");
                        var rsp = Client.Execute<dynamic>(request);
                        if (rsp.errorCode == 0)
                        {
                            var ps = ProcessorFactory.CreateProcessor<QueryItemListResponseProcessor>();
                            if (ps.Process(rsp, item))
                            {
                                Console.WriteLine("成功上传商品");
                                return;
                            }
                            Logger.Error(ps.ErrorMessage);
                        }
                        else
                        {
                            Logger.Error(string.Format("Failed to upload product to wgw {0}({1}) Error Message: {2}", item.Name,
                            item.Id, result.errorMessage));
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("Failed to upload product {0}({1}) Error Message: {2}", item.Name, item.Id, ex.Message));
                    Console.WriteLine("上传商品过程出现错误，请查看日志");
                }
            }
        }
        
        private bool ValidateProduct(int productId)
        {
            using (var db = DbContextHelper.GetDbContext())
            {
                var product = db.Products.FirstOrDefault(t => t.Id == productId);
                if (null == product)
                {
                    Console.WriteLine("无效的商品,请检查并输入正确的商品ID");
                    return false;
                }

                if (!product.Is4Sale.HasValue || !product.Is4Sale.Value)
                {
                    Console.WriteLine("商品不是上架销售状态，请检查");
                    return false;
                }

                if (!product.IsHasImage)
                {
                    Console.WriteLine("商品没有图片，不能上传，请检查");
                    return false;
                }

                if (!db.Resources.Any(r => r.Type == 1 && r.SourceId == productId))
                {
                    Console.WriteLine("商品没有图片，不能上传，请检查");
                    return false;
                }

                //var map = db.Set<ProductMapEntity>()
                //    .Join(
                //        db.Set<CategoryEntity>()
                //            .Join(
                //                db.Set<Map4Category>().Where(m => m.Channel == ConstValue.WGW_CHANNEL_NAME),
                //                c => c.ExCatCode, m => m.CategoryCode, (c, m) => c), pm => pm.ChannelCatId,
                //        c => c.ExCatId, (pm, c) => pm).FirstOrDefault(pm=>pm.ProductId == productId);
                //if (map == null)
                //{
                //    Console.WriteLine("商品没有映射关系或未映射类目，请检查");
                //    return false;
                //}

                if (!db.Inventories.Any(i => i.ProductId == productId))
                {
                    Console.WriteLine("商品没有库存信息，请检查");
                    return false;
                }
                return true;
            }
        }
    }
}
