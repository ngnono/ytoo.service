using com.intime.fashion.common.Wxpay;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using ZXing;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    [AdminAuthorize]
    public class BarCodeController : UserController
    {

        public ActionResult Generate(int sourceId,int packageType, int storeId, int sectionId)
        {
            var wxnativeUrl = new WxNativeUrl() { 
                 ProductId = string.Format("{3}-{0}-{1}-{2}",sourceId,storeId,sectionId,packageType)
            };

            var image = GenerateQRCode(wxnativeUrl.Encode());
            var memory = new MemoryStream();
             image.Save(memory,ImageFormat.Png);
             memory.Seek(0, SeekOrigin.Begin);
            return File(memory,"image/png");
        }

        public ActionResult Create()
        {
            var stores = Context.Set<StoreEntity>().Where(s => s.Status == (int)DataStatus.Normal
                            && s.ExStoreId.HasValue).OrderBy(s => s.Name)
                         .ToList().Select(s => new SelectListItem() { 
                             Text = s.Name,
                              Value = s.Id.ToString(),
                               Selected = false
                         });
            ViewBag.Stores = stores;
            ViewBag.Packages = new SelectListItem[] { 
                 new SelectListItem(){Text = WxPackageType.Product.ToString(), Value = ((int)WxPackageType.Product).ToString()},
                 new SelectListItem(){Text = WxPackageType.Sku.ToString(), Value = ((int)WxPackageType.Sku).ToString()}
            };
            return View();
            
        }
        [HttpPost]
        public ActionResult SectionListP(SectionSearchViewModel search, PagerRequest request)
        {
            int totalCount;
          
            var linq = Context.Set<SectionEntity>().Where(s=>s.StoreId ==search.StoreId && s.Status !=(int)DataStatus.Deleted && s.ChannelSectionId.HasValue)
                      ;
           

            totalCount = linq.Count();

            var skipCount = (request.PageIndex - 1) * request.PageSize;

            linq = skipCount == 0 ? linq.OrderBy(l => l.Id).Take(request.PageSize) : linq.OrderBy(l => l.Id).Skip(skipCount).Take(request.PageSize);


            var vo =  linq.ToList().Select(l=>new SectionViewModel().FromEntity<SectionViewModel>(l));

            var v = new Pager<SectionViewModel>(request, totalCount) { Data = vo.ToList() };
            return Json(v);
        }
          [HttpPost]
        public ActionResult ProductListP(ProductSearchViewModel search, PagerRequest request)
        {
            int totalCount;
            var linq = Context.Set<ProductEntity>().Where(p => p.SkuCode == search.SkuCode && p.Status != (int)DataStatus.Deleted)
                        .Join(Context.Set<InventoryEntity>(), o => o.Id, i => i.ProductId, (o, i) => new { P = o, I = i })
                        .Join(Context.Set<ProductPropertyValueEntity>(), o => o.I.PColorId, i => i.Id, (o, i) => new { P = o.P, I = o.I, Color = i })
                        .Join(Context.Set<ProductPropertyValueEntity>(), o => o.I.PSizeId, i => i.Id, (o, i) => new { P = o.P, I = o.I, Color = o.Color, Size = i });

            totalCount = linq.Count();

            var skipCount = (request.PageIndex - 1) * request.PageSize;

            linq = skipCount == 0 ? linq.OrderBy(l => l.P.Id).Take(request.PageSize) : linq.OrderBy(l => l.P.Id).Skip(skipCount).Take(request.PageSize);


            var vo = new List<dynamic>();
            foreach (var l in linq)
            {
                vo.Add(new { 
                     Id = l.P.Id,
                     Name = l.P.Name,
                     ColorValueName = l.Color.ValueDesc,
                     SizeValueName = l.Size.ValueDesc,
                     Amount = l.I.Amount,
                     GenerateId = search.PackageType==(int)WxPackageType.Product?l.P.Id:l.I.Id,
                     PackageType = search.PackageType


                });
            }

            var v = new Pager<dynamic>(request, totalCount) { Data = vo.ToList() };
            return Json(v);
        }

        private Bitmap GenerateQRCode(string input)
        {
            var writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = new ZXing.Common.EncodingOptions() { 
                 Width=600,
                 Height = 600,
                  
            };
           
            return writer.Write(input);
        }
    }
}