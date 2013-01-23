using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Passbook.Generator.Fields;

namespace Passbook.Generator.WebTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }

    public class PassController : Controller
    {

        public ActionResult Coupon()
        {
            PassGenerator generator = new PassGenerator();

            CouponPassGeneratorRequest request = new CouponPassGeneratorRequest();
            request.Identifier = "pass.com.aosca.fashion";
            request.CertThumbprint = ConfigurationManager.AppSettings["PassBookCertificateThumbprint"].Replace(" ", String.Empty);
            request.CertLocation = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine;
            request.SerialNumber = "1212123";
            request.Description = "My first pass";
            request.OrganizationName = "Tomas McGuinness";
            request.TeamIdentifier = "VM9DY9H897";
            request.LogoText = "My Pass";
            request.BackgroundColor = "rgb(0,0,0)";
            request.ForegroundColor = "rgb(255,255,255)";

            // override icon and icon retina
            //request.IconFile = Server.MapPath("~/image/icons/icon.png");
            //request.IconRetinaFile = Server.MapPath("~/image/icons/icon@2x.png");
            //request.LogoFile = Server.MapPath("~/image/icons/logo.png");
            //request.LogoRetinaFile = Server.MapPath("~/image/icons/logo@2x.png");

            //request.BackgroundFile = Server.MapPath("~/image/icons/background.png");
            //request.BackgroundRetinaFile = Server.MapPath("~/image/icons/background@2x.png");

            request.Images.Add(PassbookImage.Icon, System.IO.File.ReadAllBytes(Server.MapPath("~/image/icons/icon.png")));
            request.Images.Add(PassbookImage.IconRetina, System.IO.File.ReadAllBytes(Server.MapPath("~/image/icons/icon@2x.png")));

            request.AddBarCode("019278476234232342245", BarcodeType.PKBarcodeFormatPDF417, "UTF-8", "019278476234232342245");

            byte[] generatedPass = generator.Generate(request);



            return new FileContentResultV2(generatedPass, "application/vnd.apple.pkpass",(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture) + ".pkpass"));
        }
    }

    public class FileContentResultV2 : FileContentResult
    {
        public FileContentResultV2(byte[] fileContents, string contentType)
            : base(fileContents, contentType)
        {
        }

        public FileContentResultV2(byte[] fileContents, string contentType, string fileName)
            : base(fileContents, contentType)
        {
            FileDownloadName = fileName;
        }
    }


    public class CouponPassGeneratorRequest : PassGeneratorRequest
    {
        public CouponPassGeneratorRequest()
        {
            this.Style = PassStyle.Coupon;
        }

        public override void PopulateFields()
        {
            this.AddPrimaryField(new NumberField("discount", "Discount", 1, FieldNumberStyle.PKNumberStylePercent));
        }
    }
}
