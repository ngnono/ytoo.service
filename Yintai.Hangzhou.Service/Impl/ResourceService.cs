using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web;
using Yintai.Architecture.ImageTool.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Service.Impl
{
    public class ResourceService : BaseService, Contract.IResourceService
    {
        private readonly IResourceRepository _resourceRepository;

        public ResourceService(IResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        #region methods

        /// <summary>
        /// 获取指定文件夹
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        private static string[] GetFolder(int sourceId,
                                       SourceType sourceType)
        {
            switch (sourceType)
            {
                case SourceType.BrandLogo:
                case SourceType.CustomerPortrait:
                    var useridstr = sourceId.ToString(CultureInfo.InvariantCulture);
                    var n = String.Empty;
                    if (useridstr.Length < 6)
                    {
                        for (var i = 0; i < (6 - useridstr.Length); i++)
                        {
                            n += "0";
                        }

                        useridstr = n + useridstr;
                    }

                    var folders = new string[2];
                    folders[0] = useridstr.Substring(0, 3);
                    folders[1] = useridstr.Substring(3, 3);

                    return folders;
                default:
                    return new string[0];
            }
        }

        #endregion

        /// <summary>
        /// 存储
        /// </summary>
        /// <param name="files">文件流</param>
        /// <param name="createdUid">created userid</param>
        /// <param name="defaultNum"></param>
        /// <param name="sourceId">来源ID</param>
        /// <param name="sourceType">来源类型</param>
        /// <returns></returns>
        public List<ResourceEntity> Save(HttpFileCollectionBase files, int createdUid, int defaultNum, int sourceId,
                                         SourceType sourceType)
        {
            if (files == null || files.Count == 0)
            {
                return new List<ResourceEntity>(0);
            }

            var list = new List<ResourceEntity>(files.Count);
            var count = 0;
            //检查扩展名
            foreach (string upload in files)
            {
                if (!files[upload].HasFile()) continue;

                /*
                 *  1.获取存储的文件名
                 *  2.存储文件
                 *  3.存储到数据库
                 *  4.返回
                //*/
                FileInfor fileInfor;

                var fileuploadResult = FileUploadServiceManager.UploadFile(files[upload], sourceType.ToString().ToLower(),
                                                            out fileInfor, GetFolder(sourceId, sourceType));

                if (fileuploadResult == FileMessage.Success)
                {
                    //存储到数据库
                    var entity = new ResourceEntity
                    {
                        CreatedDate = DateTime.Now,
                        CreatedUser = createdUid,
                        IsDefault = count == defaultNum,
                        Name = fileInfor.FileName, //+ "." + fileInfor.FileExtName,//这里要注意
                        ExtName = fileInfor.FileExtName,
                        Width = fileInfor.Width,
                        Height = fileInfor.Height,
                        ContentSize = fileInfor.FileSize,
                        Size = fileInfor.Width.ToString(CultureInfo.InvariantCulture) + "x" + fileInfor.Height.ToString(CultureInfo.InvariantCulture),
                        SortOrder = count,
                        SourceId = sourceId,
                        SourceType = (int)sourceType,
                        Status = 1,
                        Type = (int)ResourceType.Image,
                        UpdatedDate = DateTime.Now,
                        UpdatedUser = createdUid,
                        Domain = String.Empty
                    };
                    //switch (fileInfor.ResourceType)
                    //{
                    //    case ResourceType.Image:
                    //        //entity.Domain = ConfigManager.GetHttpApiImagePath();
                    //        break;
                    //    case ResourceType.Sound:
                    //        //entity.Domain = UploadFileConfigManager.GetHttpApiSoundPath();
                    //        break;
                    //    case ResourceType.Video:
                    //        break;
                    //}

                    list.Add(entity);
                }
                else
                {
                    continue;
                }

                count++;
            }

            if (list.Count > 0)
            {
                return _resourceRepository.Insert(list);
            }

            return new List<ResourceEntity>(0);
        }

        public ResourceEntity Get(int resourceId)
        {
            return _resourceRepository.GetItem(resourceId);
        }

        public List<ResourceEntity> Get(int sourceId, SourceType sourceType)
        {
            return _resourceRepository.GetList((int)sourceType, sourceId);
        }

        public ResourceEntity Del(int resourceId)
        {
            var entity = Get(resourceId);
            if (entity == null)
            {
                return null;
            }

            //图片没有做物理删除



            _resourceRepository.Delete(entity);

            return entity;
        }


        /*

        public List<ResourceEntity> Save(HttpFileCollectionBase files, int userid, int defaultNum, int sourceId, SourceType sourceType)
        {
            if (files == null || files.Count == 0)
            {
                return new List<ResourceEntity>(0);
            }

            var list = new List<ResourceEntity>(files.Count);

            var count = 0;
            //检查扩展名
            foreach (string upload in files)
            {
                if (!files[upload].HasFile()) continue;

                var type = files[upload].ContentType;
                //var mimeType = files[upload].ContentType;
                var fileStream = files[upload].InputStream;
                //var fileName = Path.GetFileName(files[upload].FileName);
                //bytes
                var fileLength = files[upload].ContentLength;
                var fileData = new byte[fileLength];
                fileStream.Read(fileData, 0, fileLength);

                var fileInfo = new FileInfo(files[upload].FileName);
                ResourceEntity entity = null;
                switch (fileInfo.Extension.ToLower())
                {
                    case ".jpg":
                    case ".jpe":
                    case ".gif":
                    case ".jpeg":
                    case ".png":
                        entity = SaveEntity(ResourceType.Image, userid, defaultNum == count, count, sourceId,
                                                sourceType, files[upload], fileInfo.Extension);
                        break;
                    default:
                        entity = SaveEntity(userid, defaultNum == count, count, sourceId, sourceType, files[upload]);
                        break;
                }
                if (entity != null)
                {
                    list.Add(entity);
                }
                count++;
            }

            if (list.Count > 0)
            {
                return _resourceRepository.Insert(list);
            }

            return new List<ResourceEntity>();
        }

        //*/
    }
}
