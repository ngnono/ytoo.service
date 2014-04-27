﻿using Common.Logging;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Product.ProductSync.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Yintai.Hangzhou.Contract.Images;
using Yintai.Hangzhou.Service.Manager;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Processors
{
    public class ProductPicProcessor : IProductPicProcessor
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IChannelMapper _channelMapper;

        public ProductPicProcessor(IChannelMapper channelMapper)
        {
            _channelMapper = channelMapper;
        }

        public Resource Sync(string channelProductId, string channelColorId, string channelUrl, string Id, int SeqNo, DateTime WriteTime)
        {
            var productMap = _channelMapper.GetMapByChannelValue(channelProductId, ChannelMapType.ProductId);
            if (productMap == null)
            {
                Log.ErrorFormat("同步商品图片出错，商品没有同步,productId:[{0}],colorId:[{1}],url:[{2}]", channelProductId, channelColorId, channelUrl);
                return null;
            }

            var mapKey = Utils.GetProductProprtyMapKey(productMap.LocalId, channelColorId);
            var colorIdMap = _channelMapper.GetMapByChannelValue(mapKey, ChannelMapType.ColorId);
            if (colorIdMap == null)
            {
                Log.ErrorFormat("同步商品图片出错，颜色属性没有同步,productId:[{0}],colorId:[{1}],url:[{2}]", channelProductId, channelColorId, channelUrl);
                return null;
            }

            string filePath;
            FileInfor uploadFile = null;
            Resource resource = null;
            using (var db = new YintaiHZhouContext())
            {
                //查找是否已经同步图片
                var resourceExt = db.Resources.FirstOrDefault(r => r.ColorId == colorIdMap.LocalId && r.SourceId == productMap.LocalId && r.SortOrder == SeqNo && r.SourceType == 1);
                var product = db.Products.FirstOrDefault(x => x.Id == productMap.LocalId);
                if (product == null)
                {
                    return null;
                }
                // 下载远程图片
                if (resourceExt == null || resourceExt.UpdatedDate != WriteTime)
                {
                    try
                    {
                        filePath = FetchRemotePic(channelUrl);
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorFormat("同步商品图片出错，下载图片出错,productId:[{0}],colorId:[{1}],url:[{2}]", channelProductId, channelColorId, channelUrl);
                        Log.Error(ex);
                        return null;
                    }

                    //resize pics
                    var file = new FileInfo(filePath);

                    var uploadResult = FileUploadServiceManager.UploadFile(file, "product", out uploadFile, string.Empty);
                    if (uploadResult != FileMessage.Success)
                    {
                        Log.ErrorFormat("上传文件失败:{0}", filePath);
                        File.Delete(filePath);
                        return null;
                    }
                }

                if (resourceExt == null)
                {
                    var newResource = new Resource()
                    {
                        ColorId = colorIdMap.LocalId,
                        SourceId = productMap.LocalId,
                        SourceType = 1,
                        ContentSize = uploadFile.FileSize,
                        Domain = string.Empty,
                        ExtName = uploadFile.FileExtName,
                        Height = uploadFile.Height,
                        IsDefault = false,
                        Name = uploadFile.FileName,
                        Status = 1,
                        SortOrder = SeqNo,
                        Size = string.Format("{0}x{1}", uploadFile.Width, uploadFile.Height),
                        Type = (int)uploadFile.ResourceType,
                        Width = uploadFile.Width,
                        ChannelPicId = 0,
                        CreatedDate = DateTime.Now,
                        CreatedUser = SystemDefine.SystemUser,
                        UpdatedUser = SystemDefine.SystemUser,
                        UpdatedDate = DateTime.Now,
                    };
                    db.Resources.Add(newResource);
                    resource = newResource;
                }
                else
                {
                    resourceExt.ContentSize = uploadFile.FileSize;
                    resourceExt.UpdatedUser = SystemDefine.SystemUser;
                    resourceExt.UpdatedDate = DateTime.Now;
                    resourceExt.ExtName = uploadFile.FileExtName;
                    resourceExt.Height = uploadFile.Height;
                    resourceExt.IsDefault = false;
                    resourceExt.UpdatedDate = DateTime.Now;
                    resourceExt.Name = uploadFile.FileName;
                    resourceExt.Size = string.Format("{0}x{1}", uploadFile.Width, uploadFile.Height);
                    resourceExt.Type = (int)uploadFile.ResourceType;
                    resourceExt.Width = uploadFile.Width;
                    resource = resourceExt;
                }
                if (!product.IsHasImage)
                {
                    product.IsHasImage = true;
                    product.UpdatedDate = DateTime.Now;
                    //product.UpdatedUser = SystemDefine.SystemUser;
                }
                db.SaveChanges();
                return resource;
            }
        }

        private static string FetchRemotePic(string url)
        {
            var client = new HttpClient();
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tmp", DateTime.Today.ToString("yyyyMMdd"));

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var path = string.Format("{0}/{1}.jpg", directory, Guid.NewGuid());

            client.GetAsync(url)
               .ContinueWith(request =>
               {
                   var response = request.Result;
                   response.EnsureSuccessStatusCode();
                   using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                   {
                       response.Content.CopyToAsync(fileStream).Wait();
                       fileStream.Flush();
                   }
               }).Wait();

            return path;
        }
    }
}
