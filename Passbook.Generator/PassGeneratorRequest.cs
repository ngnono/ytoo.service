using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Passbook.Generator.Fields;

namespace Passbook.Generator
{
    public enum PassbookImage
    {
        Icon,
        IconRetina,
        Logo,
        LogoRetina,
        Background,
        BackgroundRetina,
        Strip,
        StripRetina,
        Thumbnail,
        ThumbnailRetina
    }

    public class PassGeneratorRequest
    {


        /// <summary>
        /// 所有的图片都是必填的，因为这个生成代码里没有判断图片为空的情况，所以必须保证Icon, Logo, Background都有。
        /// </summary>
        public PassGeneratorRequest()
        {
            this.HeaderFields = new List<Field>();
            this.PrimaryFields = new List<Field>();
            this.SecondaryFields = new List<Field>();
            this.AuxiliaryFields = new List<Field>();
            this.BackFields = new List<Field>();
        }

        #region images

        private Dictionary<PassbookImage, string> _images;

        public Dictionary<PassbookImage, string> Images
        {
            get
            {
                _images = new Dictionary<PassbookImage, string>();
                if (!String.IsNullOrEmpty(this.IconFile))
                {
                    this._images.Add(PassbookImage.Icon, IconFile);
                }
                if (!String.IsNullOrEmpty(this.IconRetinaFile))
                {
                    this._images.Add(PassbookImage.IconRetina, IconRetinaFile);
                }

                if (!String.IsNullOrEmpty(this.LogoFile))
                {
                    this._images.Add(PassbookImage.Logo, LogoFile);
                }
                if (!String.IsNullOrEmpty(this.LogoRetinaFile))
                {
                    this._images.Add(PassbookImage.LogoRetina, LogoRetinaFile);
                }

                if (!String.IsNullOrEmpty(this.BackgroundFile))
                {
                    this._images.Add(PassbookImage.Background, BackgroundFile);
                }
                if (!String.IsNullOrEmpty(this.BackgroundRetinaFile))
                {
                    this._images.Add(PassbookImage.BackgroundRetina, BackgroundRetinaFile);
                }

                if (!String.IsNullOrEmpty(this.BackgroundFile))
                {
                    this._images.Add(PassbookImage.Background, BackgroundFile);
                }
                if (!String.IsNullOrEmpty(this.BackgroundRetinaFile))
                {
                    this._images.Add(PassbookImage.BackgroundRetina, BackgroundRetinaFile);
                }

                return _images;
            }


        }

        #endregion

        #region helper

        public void AddAuxiliaryField(Field field)
        {
            this.AuxiliaryFields.Add(field);
        }

        public void AddBackField(Field field)
        {
            this.BackFields.Add(field);
        }

        public void AddBarCode(string message, BarcodeType type, string encoding, string altText)
        {
            this.Barcode = new BarCode();
            this.Barcode.Type = type;
            this.Barcode.Message = message;
            this.Barcode.Encoding = encoding;
            this.Barcode.AlternateText = altText;
        }

        public void AddHeaderField(Field field)
        {
            this.HeaderFields.Add(field);
        }

        public void AddPrimaryField(Field field)
        {
            this.PrimaryFields.Add(field);
        }

        public void AddSecondaryField(Field field)
        {
            this.SecondaryFields.Add(field);
        }

        private void CloseStyleSpecificKey(JsonWriter writer)
        {
            writer.WriteEndObject();
        }

        private void OpenStyleSpecificKey(JsonWriter writer, PassGeneratorRequest request)
        {
            switch (request.Style)
            {
                case PassStyle.Generic:
                    writer.WritePropertyName("generic");
                    writer.WriteStartObject();
                    return;

                case PassStyle.BoardingPass:
                    writer.WritePropertyName("boardingPass");
                    writer.WriteStartObject();
                    return;

                case PassStyle.Coupon:
                    writer.WritePropertyName("coupon");
                    writer.WriteStartObject();
                    return;

                case PassStyle.EventTicket:
                    writer.WritePropertyName("eventTicket");
                    writer.WriteStartObject();
                    return;

                case PassStyle.StoreCard:
                    writer.WritePropertyName("eventTicket");
                    writer.WriteStartObject();
                    return;
            }
            throw new InvalidOperationException("Unsupported pass style specified");
        }

        public virtual void PopulateFields()
        {
        }

        internal void Write(JsonWriter writer)
        {
            this.PopulateFields();
            writer.WriteStartObject();
            this.WriteStandardKeys(writer, this);
            this.WriteAppearanceKeys(writer, this);
            this.OpenStyleSpecificKey(writer, this);
            this.WriteSection(writer, "headerFields", this.HeaderFields);
            this.WriteSection(writer, "primaryFields", this.PrimaryFields);
            this.WriteSection(writer, "secondaryFields", this.SecondaryFields);
            this.WriteSection(writer, "auxiliaryFields", this.AuxiliaryFields);
            this.WriteSection(writer, "backFields", this.BackFields);
            if (this.Style == PassStyle.BoardingPass)
            {
                writer.WritePropertyName("transitType");
                writer.WriteValue(this.TransitType.ToString());
            }
            this.CloseStyleSpecificKey(writer);
            this.WriteBarcode(writer, this);
            this.WriteUrls(writer, this);
            writer.WriteEndObject();
        }

        private void WriteAppearanceKeys(JsonWriter writer, PassGeneratorRequest request)
        {
            writer.WritePropertyName("foregroundColor");
            writer.WriteValue(request.ForegroundColor);
            writer.WritePropertyName("backgroundColor");
            writer.WriteValue(request.BackgroundColor);
        }

        private void WriteBarcode(JsonWriter writer, PassGeneratorRequest request)
        {
            if (this.Barcode != null)
            {
                writer.WritePropertyName("barcode");
                writer.WriteStartObject();
                writer.WritePropertyName("format");
                writer.WriteValue(request.Barcode.Type.ToString());
                writer.WritePropertyName("message");
                writer.WriteValue(request.Barcode.Message);
                writer.WritePropertyName("messageEncoding");
                writer.WriteValue(request.Barcode.Encoding);
                writer.WritePropertyName("altText");
                writer.WriteValue(request.Barcode.AlternateText);
                writer.WriteEndObject();
            }
        }

        private void WriteSection(JsonWriter writer, string sectionName, List<Field> fields)
        {
            writer.WritePropertyName(sectionName);
            writer.WriteStartArray();
            foreach (Field field in fields)
            {
                field.Write(writer);
            }
            writer.WriteEndArray();
        }

        private void WriteStandardKeys(JsonWriter writer, PassGeneratorRequest request)
        {
            writer.WritePropertyName("passTypeIdentifier");
            writer.WriteValue(request.Identifier);
            writer.WritePropertyName("formatVersion");
            writer.WriteValue(request.FormatVersion);
            writer.WritePropertyName("serialNumber");
            writer.WriteValue(request.SerialNumber);
            writer.WritePropertyName("description");
            writer.WriteValue(request.Description);
            writer.WritePropertyName("organizationName");
            writer.WriteValue(request.OrganizationName);
            writer.WritePropertyName("teamIdentifier");
            writer.WriteValue(request.TeamIdentifier);
            writer.WritePropertyName("logoText");
            writer.WriteValue(request.LogoText);
        }

        private void WriteUrls(JsonWriter writer, PassGeneratorRequest request)
        {
            if (!string.IsNullOrEmpty(request.AuthenticationToken))
            {
                writer.WritePropertyName("authenticationToken");
                writer.WriteValue(request.AuthenticationToken);
                writer.WritePropertyName("webServiceURL");
                writer.WriteValue(request.WebServiceUrl);
            }
        }

        #endregion

        // Properties
        public string AuthenticationToken { get; set; }

        public List<Field> AuxiliaryFields { get; private set; }

        public List<Field> BackFields { get; private set; }

        public string BackgroundColor { get; set; }

        public string BackgroundFile { get; set; }

        public string BackgroundRetinaFile { get; set; }

        public BarCode Barcode { get; private set; }

        public StoreLocation CertLocation { get; set; }

        /// <summary>
        /// 是你的windows里面的pass证书的指纹。获取方式：在windows证书管理里“个人”的证书下面双击这个pass的证书，在属性页面，所有的属性里，拉到最下面，把指纹的值复制出来，删掉里面的空格，一堆数字加字母的串就是了。大小写不敏感。
        /// </summary>
        public string CertThumbprint { get; set; }

        public string Description { get; set; }

        public object ForegroundColor { get; set; }

        /// <summary>
        ///  = 1; 这个值的定义在Web项目的示例代码里居然没有，但是少了这个safari会提示无法下载该文件
        /// </summary>
        public int FormatVersion { get; set; }

        public List<Field> HeaderFields { get; private set; }

        public string IconFile { get; set; }

        public string IconRetinaFile { get; set; }

        /// <summary>
        /// request.Identifier是你的pass type id的定义，就是pass.com.yourdomain.xxx的样式。
        /// </summary>
        public string Identifier { get; set; }

        public string LogoFile { get; set; }

        public string LogoRetinaFile { get; set; }

        public string LogoText { get; set; }

        public string OrganizationName { get; set; }

        public List<Field> PrimaryFields { get; private set; }

        public List<Field> SecondaryFields { get; private set; }

        /// <summary>
        /// 全局不能重复，也就是同一个Identifier下不允许出现重复的SerialNumber。
        /// </summary>
        public string SerialNumber { get; set; }

        public PassStyle Style { get; set; }

        public bool SuppressStripeShine { get; set; }

        /// <summary>
        /// 是你上面的pass type id生成后的前缀，类似于FE93CED9这样的，通常是系统生成给你的。
        /// </summary>
        public string TeamIdentifier { get; set; }

        public TransitType TransitType { get; set; }

        public string WebServiceUrl { get; set; }
    }
}