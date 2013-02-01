
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Architecture.ImageTool.Models;
using Yintai.Hangzhou.Cms.WebSiteTest.Controllers;
using Yintai.Hangzhou.Cms.WebSiteTest.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Service.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteTest.Util
{
    public class ProUploadService
    {
        private string _filePath;
        private string _Session_Key = "probulksession";
        private ProBulkUploadController _context;
        private Hangzhou.Data.Models.YintaiHzhouContext _dbContext;
        private IResourceService _resourceService;
        private static Dictionary<string, Type> cols = new Dictionary<string, Type>() { 
               { "name",typeof(string)},
               {"BrandName",typeof(string)},
               { "Description",typeof(string)},
               { "Price",typeof(decimal)},
               { "DescripOfPromotion",typeof(string)},
               { "DescripOfProBeginDate", typeof(DateTime)},
               { "DescripOfProEndDate", typeof(DateTime)},
               { "InUserId", typeof(int)},
               { "Tag", typeof(string)},
               { "Store",typeof(string)},
               { "Promotions",typeof(string)},
               { "ItemCode", typeof(string)},
               { "Subjects", typeof(string)},
               { "UploadGroupId", typeof(int)},
               { "InDate", typeof(DateTime)},
               { "Status",typeof(int)}
            };
       
        public ProUploadService(string filePath,ProBulkUploadController context)
        {
            this._filePath = filePath;
            _dbContext = new Data.Models.YintaiHzhouContext();
            _context = context;
            _resourceService = ServiceLocator.Current.Resolve<IResourceService>();
        }
        public ProUploadService():this(null,null)
        { 
            
        }
        public IEnumerable<ProductUploadInfo> Stage()
        {
            if (!EnsureProFileOnDisk())
                return null;
           var result = TransferProFileToStage();
        
           return result;
        }
        public IEnumerable<ImageUploadInfo> ImageStage()
        {
            if (!EnsureProFileOnDisk())
                return null;
            return TransferImageToStage();
        }

        private IEnumerable<ImageUploadInfo> TransferImageToStage()
        {
            
            var image = _dbContext.ResourceStages.Create();
            FileInfo imageFile = new FileInfo(_filePath);
            int jobId = _context.JobId;

            FileInfor fileInfor = _resourceService.SaveStage(imageFile, 4, SourceType.Product);
            if (fileInfor != null)
            {
                var entity = _dbContext.ResourceStages.Create();
                entity.ContentSize = fileInfor.FileSize;
                entity.ExtName = fileInfor.FileExtName;
                entity.Name = fileInfor.FileName;
                entity.Width = fileInfor.Width;
                entity.Height = fileInfor.Height;
                entity.Size = fileInfor.Width.ToString(CultureInfo.InvariantCulture) + "x" + fileInfor.Height.ToString(CultureInfo.InvariantCulture);
                entity.SortOrder = 1;
                entity.ItemCode = Path.GetFileNameWithoutExtension(_filePath);
                entity.InUser = 4;
                entity.InDate = DateTime.Now;
                entity.UploadGroupId = jobId;
                _dbContext.ResourceStages.Add(entity);
                _dbContext.SaveChanges();
                yield return new ImageUploadInfo()
                {
                    ItemCode = entity.ItemCode
                    ,
                    fileSize = entity.Size
                    ,
                    FileName = entity.Name
                    ,
                    Width = entity.Width
                    ,
                    Height = entity.Height

                };
            }
            else
            {
                 yield return null;
            }

        }
        public IEnumerable<ProductValidateResult> Validate()
        {
            return ValidateProFileOnStage();
        }

        private bool EnsureProFileOnDisk()
        {
            return !string.IsNullOrEmpty(_filePath) &&
                File.Exists(_filePath);

        }

        private IEnumerable<ProductValidateResult> ValidateProFileOnStage()
        {
           return _dbContext.Database.SqlQuery<ProductValidateResult>(@"exec dbo.ProductStageValidate @inUser,@jobId",
                 new[] {new SqlParameter("inUser", 4)
                   ,new SqlParameter("jobId",_context.JobId)//todo: get from current context later
                });

        }
        private object mapCellValueToStrongType<T>(ICell cell)
        {
            object finalValue = default(T);
            if (cell == null)
                return DBNull.Value;
            switch (cell.CellType) { 
                case CellType.BOOLEAN:
                    finalValue = cell.BooleanCellValue;
                    break;
                case CellType.STRING:
                    finalValue = cell.StringCellValue;
                    break;
                case CellType.NUMERIC:
                    finalValue = cell.ToString();
                    break;
                default:
                    break;
            }
            return finalValue;
        }
        private IEnumerable<ProductUploadInfo> TransferProFileToStage()
        {
            HSSFWorkbook hssfWB;
            int jobId = _context.JobId;
            if (jobId<=0)
            {
                //create job id first
                var job = _dbContext.ProductUploadJobs.Create();
                job.FileName = Path.GetFileName(_filePath);
                job.InDate = DateTime.Now;
                job.InUser = 4;
                _dbContext.ProductUploadJobs.Add(job);
                _dbContext.SaveChanges();
                _context.JobId = job.Id;
                jobId = job.Id;
            }
            DataTable dt = new DataTable();
          
            foreach(var col in cols.Keys)
            {
                Type colType;
                cols.TryGetValue(col,out colType);
                dt.Columns.Add(col,colType);
            }
            using (var file = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                hssfWB = new HSSFWorkbook(file);
                System.Collections.IEnumerator rows = hssfWB.GetSheetAt(0).GetRowEnumerator();

                rows.MoveNext();
                while (rows.MoveNext())
                {
                    var row = (HSSFRow)rows.Current;
                    DataRow dr = dt.NewRow();
                    int i = 0;
                    dr[i++] = mapCellValueToStrongType<string>(row.GetCell(1));
                    dr[i++] = mapCellValueToStrongType<string>(row.GetCell(7));
                    dr[i++] = mapCellValueToStrongType<string>(row.GetCell(2));
                    dr[i++] = mapCellValueToStrongType<decimal?>(row.GetCell(6));
                    dr[i++] = mapCellValueToStrongType<string>(row.GetCell(3));
                    dr[i++] = mapCellValueToStrongType<DateTime?>(row.GetCell(4));
                    dr[i++] = mapCellValueToStrongType<DateTime?>(row.GetCell(5));
                    dr[i++] = 4; //hard code here, todo, get from current context
                    dr[i++] = mapCellValueToStrongType<string>(row.GetCell(8));
                    dr[i++] = mapCellValueToStrongType<string>(row.GetCell(9));
                    dr[i++] = mapCellValueToStrongType<string>(row.GetCell(10));
                    dr[i++] = mapCellValueToStrongType<string>(row.GetCell(0));
                    dr[i++] = mapCellValueToStrongType<string>(row.GetCell(11));
                    dr[i++] = jobId;
                    dr[i++] = DateTime.Now;
                    dr[i++] = ProUploadStatus.ProductsOnStage;
                    dt.Rows.Add(dr);
                }
            }


            using (SqlConnection destinationConnection = new SqlConnection(_dbContext.Database.Connection.ConnectionString))
            {
                using (var bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    foreach (var col in cols.Keys)
                    {
                        bulkCopy.ColumnMappings.Add(col, col);
                    }
                    bulkCopy.DestinationTableName = "dbo.ProductStage";
                    destinationConnection.Open();
                    bulkCopy.WriteToServer(dt);
                    DeleteTempFile();
                }
            }
            foreach(var p in _dbContext.ProductStages.Where(o=>o.UploadGroupId==jobId))
            {
                yield return new ProductUploadInfo(){
                     Brand = p.BrandName
                       ,Descrip = p.Description
                       ,DescripOfPromotion = p.DescripOfPromotion
                       , DescripOfPromotionBeginDate = p.DescripOfProBeginDate
                       , DescripOfPromotionEndDate = p.DescripOfProEndDate
                       , ItemCode = p.ItemCode
                       , Price = p.Price
                       , PromotionIds = p.Promotions
                       , Store = p.Store
                       , Tag = p.Tag
                       , Title = p.name
                       ,SubjectIds = p.Subjects
                       ,SessionU = jobId
                };
            }
        }

        private void DeleteTempFile()
        {
            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }



        internal IEnumerable<ProductPublishResult> Publish()
        {
            var result = _dbContext.Database.SqlQuery<ProductPublishResult>(@"exec dbo.ProductStagePublish2 @inUser,@jobId"
                     , new[] { new SqlParameter("inUser",4 )
                     ,new SqlParameter("jobId",_context.JobId)});
           
            _context.JobId = 0;
            return result;

        }
    }
}