﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yintai.Hangzhou.Cms.WebSiteTest.Util;


namespace Yintai.Hangzhou.Cms.WebSiteTest.Controllers
{
    public class ProBulkUploadController : Controller
    {
        private string _fileFullPath;
        private const string _Session_Key = "probulksessionid";
        public ProBulkUploadController()
        {
        }

        private string StorageRoot
        {
            get { return Path.Combine(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BulkFileFolder"])); } //Path should! always end with '/'
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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Display()
        {
            PrepareBulkUpload();
            return View();
        }

        private void PrepareBulkUpload()
        {
            if (HttpContext.Response.Cookies[_Session_Key] != null)
                HttpContext.Response.Cookies[_Session_Key].Value = "0" ;
        }
       
        public PartialViewResult Validate()
        {
            var valResult = new ProUploadService(null,this).Validate().ToArray();
            return PartialView("_ValidatePartial",valResult);
        }
        public PartialViewResult Publish()
        {
            var pubResult = new ProUploadService(null,this).Publish().ToArray();
            return PartialView("_PublishPartial",pubResult);
        }
        [HttpPost]
        public JsonResult Upload()
        {
            HttpContextBase context = ControllerContext.HttpContext;        
            UploadFile(context);
            ProductUploadInfo[] array = new ProUploadService(_fileFullPath, this).Stage().ToArray<ProductUploadInfo>();
            return Json(array);
        }
        [HttpPost]
        public JsonResult UploadImage()
        {
            if (!EnsureJobIdContext())
               throw new Exception("还没有导入商品");
            HttpContextBase context = ControllerContext.HttpContext;
            UploadFile(context);
            
            return Json(new ProUploadService(_fileFullPath,this).ImageStage().ToArray());
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

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }

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

                var fullPath = StorageRoot + Path.GetFileName(file.FileName);

                file.SaveAs(fullPath);
              
                _fileFullPath = fullPath;
            }

        }
    }
}
