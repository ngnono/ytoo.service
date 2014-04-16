using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;

using Common.Logging;
using Intime.OPC.Domain.Models;
using System;
using Intime.OPC.Job.Product.ProductSync.Models;
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

        public Resource Sync(string channelProductId, string channelColorId, string channelUrl)
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

            // 判断是否需要同步图片
            if (Synced(colorIdMap.LocalId, channelUrl))
            {
                Log.InfoFormat("商品图片已经同步，远程的图片地址没有发生变化无需同步,productId:[{0}],colorId:[{1}],url:[{2}]", channelProductId, channelColorId, channelUrl);
                return null;
            }

            string filePath;

            // 下载远程图片
            try
            {
                filePath = FetchRemotePic(channelUrl);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("同步商品图片出错，下载图片出错,productId:[{0}],colorId:[{1}],url:[{2}]", channelProductId, channelColorId, channelUrl);
                return null;
            }

            //resize pics
            var file = new FileInfo(filePath);
            FileInfor uploadFile;
            var uploadResult = FileUploadServiceManager.UploadFile(file, "product", out uploadFile, string.Empty);
            if (uploadResult != FileMessage.Success)
            {
                Log.ErrorFormat("上传文件失败:{0}", filePath);
                File.Delete(filePath);
                return null;
            }

            using (var db = new YintaiHZhouContext())
            {
                //查找是否已经同步图片
                var resourceExt = db.Resources.FirstOrDefault(r => r.ColorId == colorIdMap.LocalId);

                if (resourceExt == null)
                {
                    var newResource = new Resource()
                    {
                        ColorId = colorIdMap.LocalId,
                        SourceId = 0,
                        SourceType = 0,
                        ContentSize = uploadFile.FileSize,
                        CreatedDate = DateTime.Now,
                        CreatedUser = SystemDefine.SystemUser,
                        Domain = string.Empty,
                        ExtName = uploadFile.FileExtName,
                        Height = uploadFile.Height,
                        IsDefault = false,
                        UpdatedDate = DateTime.Now,
                        Name = uploadFile.FileName,
                        Status = 0,
                        SortOrder = 0,
                        Size = string.Format("{0}x{1}", uploadFile.Width, uploadFile.Height),
                        Type = (int)uploadFile.ResourceType,
                        Width = uploadFile.Width,
                        ChannelPicId = 0
                    };
                    db.Resources.Add(newResource);
                    db.SaveChanges();

                    //保存映射关系
                    _channelMapper.CreateMap(new ChannelMap()
                    {
                        LocalId = colorIdMap.LocalId,
                        ChannnelValue = channelUrl,
                        MapType = ChannelMapType.ProductPic
                    });

                    return newResource;
                }


                resourceExt.ContentSize = uploadFile.FileSize;
                resourceExt.CreatedDate = DateTime.Now;
                resourceExt.CreatedUser = SystemDefine.SystemUser;

                resourceExt.ExtName = uploadFile.FileExtName;
                resourceExt.Height = uploadFile.Height;
                resourceExt.IsDefault = false;
                resourceExt.UpdatedDate = DateTime.Now;
                resourceExt.Name = uploadFile.FileName;

                resourceExt.Size = string.Format("{0}x{1}", uploadFile.Width, uploadFile.Height);
                resourceExt.Type = (int)uploadFile.ResourceType;
                resourceExt.Width = uploadFile.Width;

                db.SaveChanges();

                // 更新最后的图片地址
                _channelMapper.UpdateMapByLocal(colorIdMap.LocalId.ToString(CultureInfo.InvariantCulture), ChannelMapType.ProductPic, channelUrl);

                return resourceExt;
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


        private bool Synced(int colorId, string channelUrl)
        {
            using (var db = new YintaiHZhouContext())
            {
                //查找是否已经同步图片
                var resourceExt = db.Resources.FirstOrDefault(r => r.ColorId == colorId);

                if (resourceExt == null)
                {
                    return false;
                }

                var resourceMap = _channelMapper.GetMapByLocal(colorId.ToString(CultureInfo.InvariantCulture), ChannelMapType.ProductPic);
                if (resourceMap == null)
                {
                    return false;
                }

                return resourceMap.ChannnelValue == channelUrl;
            }
        }
    }
}
