using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.WebSupport.Mvc;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Architecture.Common.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    [AdminAuthorize]
    public class ProBulkUploadController : UserController
    {
        private string _fileFullPath;
        private const string _Session_Key = "probulksessionid";
        private IBrandRepository _brandRepo;
        private ITagRepository _tagRepo;
        private IStoreRepository _storeRepo;
        private ICategoryPropertyRepository _propertyRepo;
        private ICategoryPropertyValueRepository _valueRepo;
        public ProBulkUploadController(IBrandRepository brandRepo,
            ITagRepository tagRepo,
            IStoreRepository storeRepo,
            ICategoryPropertyRepository propertyRepo,
            ICategoryPropertyValueRepository valueRepo
           )
        {
            _brandRepo = brandRepo;
            _tagRepo = tagRepo;
            _storeRepo = storeRepo;
            _propertyRepo = propertyRepo;
            _valueRepo = valueRepo;
        }

        private string StorageRoot
        {
            get { return Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BulkFileFolder"]),CurrentUser.CustomerId.ToString()); } //Path should! always end with '/'
        }
        public int JobId
        {
            get
            {
                int jobID;
                int.TryParse(this.ControllerContext.HttpContext.Request.Cookies[_Session_Key].Value, out jobID);
                return jobID;
            }
            set
            {
                if (HttpContext.Response.Cookies[_Session_Key] != null)
                    HttpContext.Response.Cookies[_Session_Key].Value = value.ToString();
                else
                    HttpContext.Response.Cookies.Add(new HttpCookie(_Session_Key, value.ToString()));

            }
        }
        public ActionResult Display(int? id)
        {
            PrepareBulkUpload();

            if (id != null)
                JobId = id.Value;
            ProUploadService helpService = new ProUploadService(this);
            var job = helpService.Job(id);
            ViewBag.UploadedProducts = helpService.List(id);
            ViewBag.UploadedImages = helpService.ListImages(id);
            SetGroupIdIfNeed(id);
            return View(job);
        }
        public ActionResult List(PagerRequest request)
        {
            int totalCount;
            var jobs = new ProUploadService(this).JobList(request.PageIndex, request.PageSize, out totalCount);
            return View(new Pager<ProductUploadJob>(request, totalCount)
            {
                Data = jobs
            });
        }
        public ActionResult Delete(int? id)
        {
            if (id != null &&
                id.Value > 0)
                new ProUploadService(this).Delete(id.Value);

            return RedirectToAction("List");
        }
        public ActionResult Detail(int? uiid, int? groupId)
        {
            ViewBag.JobId = groupId;
            if (uiid != null &&
                uiid.Value > 0)
                return View(new ProUploadService(this).UploadItemDetail(uiid.Value));
            return RedirectToAction("List");
        }
        [HttpPost]
        public ActionResult Detail(ProductUploadInfo updatedModel, int? groupId)
        {
            if (ModelState.IsValid)
            {
                var model = new ProUploadService(this).ItemDetailUpdate(updatedModel);
                return View(model);
            }
            else
                return View(updatedModel);

        }
        public ActionResult DeleteItem(int id, int groupId)
        {
            new ProUploadService(this).DeleteItem(id);
            return RedirectToAction("Display", new { id = groupId });
        }
        private void SetGroupIdIfNeed(int? groupId)
        {
            if (groupId == null ||
                groupId.Value == 0)
                return;
            JobId = groupId.Value;
        }

        private void PrepareBulkUpload()
        {
            if (HttpContext.Response.Cookies[_Session_Key] != null)
                HttpContext.Response.Cookies[_Session_Key].Value = "0";
        }

        public PartialViewResult Validate()
        {
            var valResult = new ProUploadService(this).Validate();
            return PartialView("_ValidatePartial", valResult);
        }

        public PartialViewResult Publish()
        {
            var pubResult = new ProUploadService(this).Publish().ToArray();
            return PartialView("_PublishPartial", pubResult);
        }
        [HttpPost]
        public JsonResult Upload()
        {
            HttpContextBase context = ControllerContext.HttpContext;
            UploadFile(context);
            ProductUploadInfo[] array = new ProUploadService(_fileFullPath, this).Stage().ToArray<ProductUploadInfo>();
            return Json(array).EnsureContentType(context.Request);
        }
        [HttpPost]
        public JsonResult UploadImage()
        {
            if (!EnsureJobIdContext())
                throw new Exception("还没有导入商品");
            HttpContextBase context = ControllerContext.HttpContext;
            UploadFile(context);

            return Json(new ProUploadService(_fileFullPath, this).ImageStage().ToArray()).EnsureContentType(context.Request);
        }

        public ActionResult Template(int? tagId)
        {

            var basicInfoSheetName = ProUploadService.BASIC_SHEET;
            var moreInfoSheetName = ProUploadService.MORE_SHEET;
            var supportSheetName = "不要修改";

            var headerLabels = new Dictionary<string, dynamic>() { 
                {"商品代码",new {dataformat=0,width=10}},
                {"商品名称",new {dataformat=0,width=20}},
                {"描述",new {dataformat=0,width=50}},
                {"吊牌价",new {dataformat=2,width=8}},
            {"现价",new {dataformat=2,width=8}},
            {"品牌名",new {dataformat=0,width=20}},
            { "分类名",new {dataformat=0,width=20}},
            {"门店名",new {dataformat=0,width=20}},
            { "促销活动编码",new {dataformat=0,width=20}},
            { "专题编码（多个以,分割)",new {dataformat=0,width=20}},
             {"可销售",new {dataformat=2,width=5}},
             {"专柜货号",new {dataformat=0,width=10}}
            };
            var workbook = new HSSFWorkbook();
            var headerLabelCellStyle = workbook.CreateCellStyle();
            headerLabelCellStyle.BorderBottom = BorderStyle.THIN;
            headerLabelCellStyle.BorderLeft = BorderStyle.THIN;
            headerLabelCellStyle.BorderRight = BorderStyle.THIN;
            headerLabelCellStyle.BorderTop = BorderStyle.THIN;
            headerLabelCellStyle.WrapText = true;
            var headerLabelFont = workbook.CreateFont();
            headerLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
            headerLabelCellStyle.SetFont(headerLabelFont);
            //set support sheet
            var supportSheet = workbook.CreateSheet(supportSheetName);
            workbook.SetSheetHidden(workbook.GetSheetIndex(supportSheet), true);
            Func<int, dynamic, int> supportFill = (rowIndex, data) =>
            {
                var brandRow = supportSheet.CreateRow(rowIndex++);
                var brandCodeCol = brandRow.CreateCell(1);
                brandCodeCol.SetCellType(CellType.STRING);
                brandCodeCol.SetCellValue(data.Id);
                var brandNameCol = brandRow.CreateCell(2);
                brandNameCol.SetCellType(CellType.STRING);
                brandCodeCol.SetCellValue(data.Name);
                return rowIndex;
            };
            int brandRowIndex = 0;
            foreach (var brand in _brandRepo.Get(b => b.Status != (int)DataStatus.Deleted).OrderBy(b => b.Name).Select(b => new { Id = b.Id, Name = b.Name }))
            {
                brandRowIndex = supportFill(brandRowIndex, brand);
            }
            int tagRowIndex = brandRowIndex;
            var tagLinq = _tagRepo.Get(b => b.Status != (int)DataStatus.Deleted).OrderBy(b => b.Name).Select(b => new { Id = b.Id, Name = b.Name });
            if (tagId.HasValue)
                tagLinq=tagLinq.Where(t => t.Id == tagId.Value);
            foreach (var tag in tagLinq.OrderBy(t=>t.Name))
            {
                tagRowIndex = supportFill(tagRowIndex, tag);
            }
            int storeRowIndex = tagRowIndex;
            foreach (var store in _storeRepo.Get(b => b.Status != (int)DataStatus.Deleted).OrderBy(b => b.Name).Select(b => new { Id = b.Id, Name = b.Name }))
            {
                storeRowIndex = supportFill(storeRowIndex, store);
            }

            //set basic sheet 
            var sheet1 = workbook.CreateSheet(basicInfoSheetName);
            //workbook.SetSheetOrder(basicInfoSheetName, 0);

            var rowFirst = sheet1.CreateRow(0);
            Action<int, string, dynamic> cellSetting = (cellindex, desc, option) =>
            {
                var cell = rowFirst.CreateCell(cellindex);
                cell.SetCellType(CellType.STRING);
                cell.SetCellValue(desc);
                cell.CellStyle = headerLabelCellStyle;
                sheet1.SetColumnWidth(cellindex, option.width * 255);

                var currentCellStyle = workbook.CreateCellStyle();
                currentCellStyle.DataFormat = (short)option.dataformat;
                sheet1.SetDefaultColumnStyle(cellindex, currentCellStyle);
            };
            int index = 0;
            foreach (var key in headerLabels.Keys)
            {
                cellSetting(index++, key, headerLabels[key]);
            }
            //set constraint
            DVConstraint brandConstaint = DVConstraint.CreateFormulaListConstraint(string.Format("'{0}'!$B$1:$B${1}", supportSheetName, brandRowIndex));
            CellRangeAddressList brandaddressList = new CellRangeAddressList(1, 1000, 5, 5);
            HSSFDataValidation branddataValidation = new HSSFDataValidation(brandaddressList, brandConstaint);
            branddataValidation.SuppressDropDownArrow = false;
            sheet1.AddValidationData(branddataValidation);

            DVConstraint tagConstaint = DVConstraint.CreateFormulaListConstraint(string.Format("'{0}'!$B${1}:$B${2}", supportSheetName, brandRowIndex + 1, tagRowIndex));
            CellRangeAddressList tagaddressList = new CellRangeAddressList(1, 1000, 6, 6);
            HSSFDataValidation tagdataValidation = new HSSFDataValidation(tagaddressList, tagConstaint);
            tagdataValidation.SuppressDropDownArrow = false;
            sheet1.AddValidationData(tagdataValidation);

            DVConstraint storeConstaint = DVConstraint.CreateFormulaListConstraint(string.Format("'{0}'!$B${1}:$B${2}", supportSheetName, tagRowIndex + 1, storeRowIndex));
            CellRangeAddressList storeaddressList = new CellRangeAddressList(1, 1000, 7, 7);
            HSSFDataValidation storedataValidation = new HSSFDataValidation(storeaddressList, storeConstaint);
            storedataValidation.SuppressDropDownArrow = false;
            sheet1.AddValidationData(storedataValidation);

            DVConstraint is4saleConstaint = DVConstraint.CreateExplicitListConstraint(new string[]{ProUploadService.IS_4SALE_YES,ProUploadService.IS_4SALE_NO});
            CellRangeAddressList is4saleaddressList = new CellRangeAddressList(1, 1000, 10, 10);
            HSSFDataValidation is4saledataValidation = new HSSFDataValidation(is4saleaddressList, is4saleConstaint);
            is4saledataValidation.SuppressDropDownArrow = false;
            sheet1.AddValidationData(is4saledataValidation);

            //set sheet2
            if (tagId.HasValue)
            {
                //create property value sheet
                int propertyRowIndex = storeRowIndex;
                var propertyLinq = _propertyRepo.Get(b => b.Status != (int)DataStatus.Deleted && b.CategoryId == tagId.Value).OrderBy(b => b.PropertyDesc).Select(b => new { Id = b.Id, Name = b.PropertyDesc });
                if (propertyLinq.Count() > 0)
                {
                    foreach (var property in propertyLinq)
                    {
                        propertyRowIndex = supportFill(propertyRowIndex, property);
                    }
                    int valueRowIndex = propertyRowIndex;
                    foreach (var property in propertyLinq)
                    {
                        int fromValueIndex = valueRowIndex;
                        foreach (var pvalue in _valueRepo.Get(b => b.Status != (int)DataStatus.Deleted && b.PropertyId == property.Id).Select(p => new { Id = p.Id, Name = p.ValueDesc }))
                        {
                            valueRowIndex = supportFill(valueRowIndex, pvalue);
                        }
                        var rName = workbook.CreateName();
                        rName.RefersToFormula = string.Format("'{0}'!$B${1}:$B${2}", supportSheetName, fromValueIndex + 1, valueRowIndex);
                        rName.NameName = property.Name;
                    }
                    var moreheaderLabels = new Dictionary<string, dynamic>() { 
                {"商品代码",new {dataformat=2,width=10}},
                 {"属性名",new {dataformat=2,width=20}},
                 {"属性值",new {dataformat=2,width=5}}
                };
                    var sheet2 = workbook.CreateSheet(moreInfoSheetName);
                    var morerowFirst = sheet2.CreateRow(0);
                    int moreCellIndex = 0;
                    foreach (var key in moreheaderLabels.Keys)
                    {
                        var cell = morerowFirst.CreateCell(moreCellIndex++);
                        cell.SetCellType(CellType.STRING);
                        cell.SetCellValue(key);
                        cell.CellStyle = headerLabelCellStyle;
                        sheet2.SetColumnWidth(moreCellIndex, moreheaderLabels[key].width * 255);

                        var currentCellStyle = workbook.CreateCellStyle();
                        currentCellStyle.DataFormat = (short)moreheaderLabels[key].dataformat;
                        sheet2.SetDefaultColumnStyle(moreCellIndex, currentCellStyle);

                    }
                    for (int i = 3; i < 12; i++)
                    {
                        var cell = morerowFirst.CreateCell(i++);
                        cell.SetCellType(CellType.STRING);
                        cell.SetCellValue(string.Empty);
                        cell.CellStyle = headerLabelCellStyle;
                        sheet2.SetColumnWidth(moreCellIndex++, 5 * 255);
                    }
                    //set merge cell for last cell
                    sheet2.AddMergedRegion(new CellRangeAddress(0,0,2,12));
                    //set constraint
                    DVConstraint codeConstaint = DVConstraint.CreateFormulaListConstraint(string.Format("'{0}'!$A${1}:$A${2}", basicInfoSheetName, 2, 1000));
                    CellRangeAddressList codeaddressList = new CellRangeAddressList(1, 1000, 0, 0);
                    HSSFDataValidation codedataValidation = new HSSFDataValidation(codeaddressList, codeConstaint);
                    codedataValidation.SuppressDropDownArrow = false;
                    sheet2.AddValidationData(codedataValidation);

                    DVConstraint pConstaint = DVConstraint.CreateFormulaListConstraint(string.Format("'{0}'!$B${1}:$B${2}", supportSheetName, storeRowIndex + 1, propertyRowIndex));
                    CellRangeAddressList paddressList = new CellRangeAddressList(1, 1000, 1, 1);
                    HSSFDataValidation pdataValidation = new HSSFDataValidation(paddressList, pConstaint);
                    pdataValidation.SuppressDropDownArrow = false;
                    sheet2.AddValidationData(pdataValidation);

                    for (int i = 1; i < 1000; i++)
                    {
                        //set constraint
                        DVConstraint pvConstaint = DVConstraint.CreateFormulaListConstraint(string.Format("INDIRECT($B${0})", i + 1));
                        CellRangeAddressList pvaddressList = new CellRangeAddressList(i, i, 2, 12);
                        HSSFDataValidation pvdataValidation = new HSSFDataValidation(pvaddressList, pvConstaint);
                        pvdataValidation.SuppressDropDownArrow = false;
                        sheet2.AddValidationData(pvdataValidation);
                    }
                }
            }
            workbook.SetActiveSheet(workbook.GetSheetIndex(sheet1));

            var ms = new MemoryStream();
            workbook.Write(ms);
            ms.Position = 0;
            var downloadName = tagId.HasValue ? string.Format("商品上传模版-{0}.xls", tagId.Value) : "商品上传模块.xls";
            return File(ms, "application/vnd.ms-excel", downloadName);

        }


        private bool EnsureJobIdContext()
        {
            return JobId > 0;
        }
        // Upload file to the server
        private void UploadFile(HttpContextBase context)
        {
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;
            UploadWholeFile(context, statuses);
            /*
            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }
            */
        }

        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContextBase context, List<FilesStatus> statuses)
        {
            if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var inputStream = context.Request.Files[0].InputStream;
            var fullName = StorageRoot + Path.GetFileName(fileName);

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            _fileFullPath = fullName;
        }

        // Upload entire file
        private void UploadWholeFile(HttpContextBase context, List<FilesStatus> statuses)
        {
            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];
                if (!System.IO.Directory.Exists(StorageRoot))
                    System.IO.Directory.CreateDirectory(StorageRoot);
                var fullPath = Path.Combine(StorageRoot, Path.GetFileName(file.FileName));

                file.SaveAs(fullPath);

                _fileFullPath = fullPath;
            }

        }
    }
}
