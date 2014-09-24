using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Top.Api;
using Top.Api.Domain;
using Top.Api.Request;
using Top.Api.Util;
using Top.Schema.Factory;
using Top.Schema.Fields;
using Top.Schema.Rules;
using Top.Schema.Values;

namespace com.intime.fashion.data.tmall.test
{
    [TestClass]
    public class SchemaTest
    {
        private const string sandbox_url = "http://gw.api.tbsandbox.com/router/rest";
        private const string product_url = "http://gw.api.taobao.com/router/rest";

        private const string sandbox_sessionKey_container =
            "http://container.api.tbsandbox.com/container?appkey={0}";

        private const string sessionKey_container = "http://container.open.taobao.com/container?appkey=23021668&encode=utf-8205";

        private const string sandbox_appkey = "1023021668";
        private const string appKey = "23021668";
        private const string appSecret = "baf0dcd2f1ee89159dcccfab0db3368f";
        private const string sessionKey = "6101827124a157b78801f2b9d11bc9e0c2c1f3a2b4c2f5e2247396485";
        private const string sandbox_appSecret = "sandbox2f1ee89159dcccfab0db3368f";
        private const string sandbox_sessionKey = "6101917dbd7eaa6637868f8061cd05c572121559a074b593641246042";

        private long sportItemCatId = 50012037l;// 50017599L;
        private int brand_nike_id = 20578;

        private string test_picture_url =
            "http://img04.taobaocdn.com/imgextra/i4/2247396485/TB2vY7NapXXXXbTXXXXXXXXXXXX-2247396485.jpg";

        private ITopClient client;

        public SchemaTest()
        {
            this.Setup();
        }

        [TestInitialize]
        private void Setup()
        {
            client = new DefaultTopClient(product_url, appKey, appSecret);
        }

        //Nike id in tmall 20578, adidas 20579
        [TestMethod]
        public void TestProductAddSchemaGetRequest()
        {
            var request = new TmallProductAddSchemaGetRequest();
            request.BrandId = brand_nike_id;
            request.CategoryId = sportItemCatId;
            var rsp = client.Execute(request, sessionKey);
            var requiredRuleType = typeof(RequiredRule);

            var fields = SchemaReader.ReadXmlForMap(rsp.AddProductRule);
            var tmallProductAddFields = new List<Field>();
            foreach (var key in fields.Keys)
            {
                if (fields[key].GetRules().Any(t => t.GetType() == requiredRuleType && t.Value == "true"))
                {
                    tmallProductAddFields.Add(fields[key]);
                }
            }

            foreach (var field in tmallProductAddFields)
            {
                if (field.Name == "款号")
                {
                    var fd = field as InputField;
                    fd.Value = "616730101";
                    continue;
                }
                if (field.Name == "品牌")
                {
                    var fd = field as SingleCheckField;
                    fd.SetValue(new Value(brand_nike_id.ToString()));
                    continue;
                }
                if (field.Name == "上市时间")
                {
                    var fd = field as SingleCheckField;
                    fd.SetValue("20213");
                }
            }

            var imageField = fields.Values.FirstOrDefault(t => t.Id == "product_images");
            if (imageField != null)
            {
                var fd = imageField as ComplexField;
                if (fd != null)
                {
                    var imagefd = fd.GetFieldList().FirstOrDefault(x => x.Id == "product_image_0") as InputField;
                    if (imagefd != null)
                    {
                        imagefd.Value = test_picture_url;
                    }

                    ComplexValue value = new ComplexValue();
                    value.SetInputFieldValue(imagefd.Id, test_picture_url);
                    fd.SetComplexValue(value);
                    tmallProductAddFields.Add(fd);
                }

            }


            var req = new TmallProductSchemaAddRequest();
            req.BrandId = 20578l;
            req.CategoryId = sportItemCatId;
            req.XmlData = SchemaWriter.WriteParamXmlString(tmallProductAddFields);

            var xmlData = SchemaWriter.WriteParamXmlString(tmallProductAddFields);

            var addRsp = client.Execute(req, sessionKey);
            Assert.IsNotNull(rsp);
        }

        [TestMethod]
        public void TestProductMatchSchemaGetRequest()
        {
            var rsq = new TmallProductMatchSchemaGetRequest();
            rsq.CategoryId = sportItemCatId;
            var response = client.Execute(rsq, sessionKey);

            var list = SchemaReader.ReadXmlForMap(response.MatchResult);

            Assert.IsNotNull(response);

            var brandFiled = list["prop_20000"] as SingleCheckField;
            brandFiled.SetValue("20578");

            var skuCodeField = list["prop_13021751"] as InputField;
            skuCodeField.Value = "616730101";



            var request = new TmallProductSchemaMatchRequest();
            request.CategoryId = sportItemCatId;


            var fieldList = SchemaFactory.CreateEmptyFieldList();
            fieldList.Add(brandFiled);
            fieldList.Add(skuCodeField);

            request.Propvalues = SchemaWriter.WriteParamXmlString(fieldList);

            var rsp = client.Execute(request, sessionKey);

            if (!string.IsNullOrEmpty(rsp.MatchResult))
            {
                var r = new TmallItemAddSchemaGetRequest();
                r.CategoryId = sportItemCatId;
                r.ProductId = 22747892503L;//Convert.ToInt64(rsp.MatchResult);//
                r.Type = "b";
                r.IsvInit = true;

                var addItemRsp = client.Execute(r, sessionKey);

                var fields = SchemaReader.ReadXmlForMap(addItemRsp.AddItemResult);

                if (addItemRsp != null)
                {
                    TmallItemSchemaAddRequest ir = new TmallItemSchemaAddRequest();
                    ir.CategoryId = sportItemCatId;
                    ir.ProductId = Convert.ToInt64(rsp.MatchResult);//r.ProductId;

                    var itemAddFields = new List<Field>();
                    (fields["title"] as InputField).Value = "银泰商业泛渠道业务部测试商品,拍下不发货！";

                    itemAddFields.Add(fields["title"]);

                    (fields["sell_point"] as InputField).Value = "Wallace test!不发货、不退款，请飘过!";
                    itemAddFields.Add(fields["sell_point"]);

                    (fields["item_status"] as SingleCheckField).SetValue("2");
                    itemAddFields.Add(fields["item_status"]);

                    (fields["item_type"] as SingleCheckField).SetValue("b");
                    itemAddFields.Add(fields["item_type"]);

                    (fields["stuff_status"] as SingleCheckField).SetValue("5");
                    itemAddFields.Add(fields["stuff_status"]);

                    (fields["sub_stock"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["sub_stock"]);

                    (fields["auction_point"] as InputField).Value = "0.5";

                    itemAddFields.Add(fields["auction_point"]);

                    (fields["prop_129498239"] as SingleCheckField).SetValue("21958");
                    itemAddFields.Add(fields["prop_129498239"]);


                    (fields["has_warranty"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["has_warranty"]);

                    (fields["has_invoice"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["has_invoice"]);

                    (fields["outer_id"] as InputField).Value = "10101";
                    itemAddFields.Add(fields["outer_id"]);

                    (fields["is_xinpin"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["is_xinpin"]);

                    (fields["description"] as InputField).Value = "此处为商品描述+价格 原价233.3";
                    itemAddFields.Add(fields["description"]);

                    (fields["valid_thru"] as SingleCheckField).SetValue("7");
                    itemAddFields.Add(fields["valid_thru"]);

                    (fields["is_taobao"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["is_taobao"]);

                    (fields["is_ex"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["is_ex"]);

                    (fields["freight_payer"] as SingleCheckField).SetValue("2");
                    itemAddFields.Add(fields["freight_payer"]);

                    var location = fields["location"] as ComplexField;

                    var vl = new ComplexValue();
                    vl.SetInputFieldValue("prov", "北京市");

                    vl.SetInputFieldValue("city", "北京市朝阳区");

                    location.SetComplexValue(vl);
                    itemAddFields.Add(location);

                    (fields["price"] as InputField).Value = "200";
                    itemAddFields.Add(fields["price"]);

                    (fields["quantity"] as InputField).Value = "200";
                    itemAddFields.Add(fields["quantity"]);

                    (fields["service_version"] as InputField).Value = "10000";
                    itemAddFields.Add(fields["service_version"]);


                    (fields["lang"] as SingleCheckField).SetValue("zh_CN");
                    itemAddFields.Add(fields["lang"]);

                    var colorField = fields["prop_extend_1627207"] as MultiComplexField;

                    vl = new ComplexValue();
                    vl.SetSingleCheckFieldValue("prop_1627207", new Value("3232483"));
                    vl.SetInputFieldValue("alias_name", "自定义颜色");
                    colorField.AddComplexValue(vl);
                    itemAddFields.Add(colorField);

                    var sizeField = fields["prop_extend_20549"] as MultiComplexField;
                    vl = new ComplexValue();
                    vl.SetSingleCheckFieldValue("prop_20549", new Value("28393"));
                    vl.SetInputFieldValue("alias_name", "28.5cm");

                    sizeField.AddComplexValue(vl);

                    itemAddFields.Add(sizeField);


                    var skuField = fields["sku"] as MultiComplexField;

                    vl = new ComplexValue();
                    vl.SetSingleCheckFieldValue("prop_1627207",new Value("3232483"));
                    vl.SetSingleCheckFieldValue("prop_20549", new Value("28393"));
                    vl.SetInputFieldValue("sku_price","200");
                    vl.SetInputFieldValue("sku_quantity", "200");
                    vl.SetInputFieldValue("sku_outerId","101101");

                    skuField.AddComplexValue(vl);

                    itemAddFields.Add(skuField);

                    ir.XmlData = SchemaWriter.WriteParamXmlString(itemAddFields);

                    var itemRsp = client.Execute(ir, sessionKey);

                    Assert.IsNotNull(itemRsp);
                }
            }

            Assert.IsNotNull(rsp);
        }

        private long qingshaonianxie_catid = 50026312L;
        [TestMethod]
        public void TestItemAddSchemaRequest()
        {
            var tmallProductSchemaAddGetRequest = new TmallProductAddSchemaGetRequest();
            tmallProductSchemaAddGetRequest.CategoryId = qingshaonianxie_catid;
            tmallProductSchemaAddGetRequest.BrandId = brand_nike_id;
            var rsp = client.Execute(tmallProductSchemaAddGetRequest, sessionKey);
            var fields = SchemaReader.ReadXmlForMap(rsp.AddProductRule);

            List<Field> tmallProductAddFields = new List<Field>();

            (fields["prop_20000"] as SingleCheckField).SetValue(brand_nike_id.ToString());

            tmallProductAddFields.Add(fields["prop_20000"]);

            (fields["prop_13021751"] as InputField).Value = "616730-101";
            tmallProductAddFields.Add(fields["prop_13021751"]);

            (fields["prop_22196"] as SingleCheckField).SetValue("46240");

            tmallProductAddFields.Add(fields["prop_22196"]);

            var imageField = fields.Values.FirstOrDefault(t => t.Id == "product_images");
            if (imageField != null)
            {
                var fd = imageField as ComplexField;
                if (fd != null)
                {
                    var imagefd = fd.GetFieldList().FirstOrDefault(x => x.Id == "product_image_0") as InputField;
                    if (imagefd != null)
                    {
                        imagefd.Value = test_picture_url;
                    }

                    ComplexValue value = new ComplexValue();
                    value.SetInputFieldValue(imagefd.Id, test_picture_url);
                    fd.SetComplexValue(value);
                    tmallProductAddFields.Add(fd);
                }

            }

            var req = new TmallProductSchemaAddRequest();
            req.BrandId = tmallProductSchemaAddGetRequest.BrandId;
            req.CategoryId = tmallProductSchemaAddGetRequest.CategoryId;
            req.XmlData = SchemaWriter.WriteParamXmlString(tmallProductAddFields);

            var tpsar = client.Execute(req, sessionKey);

            Assert.IsNotNull(rsp);
        }

        [TestMethod]
        public void TestAddProduct()
        {
            var rsq = new TmallProductMatchSchemaGetRequest();
            rsq.CategoryId = qingshaonianxie_catid;
            var response = client.Execute(rsq, sessionKey);

            var list = SchemaReader.ReadXmlForMap(response.MatchResult);

            Assert.IsNotNull(response);

            var brandFiled = list["prop_20000"] as SingleCheckField;
            brandFiled.SetValue("20578");

            var skuCodeField = list["prop_13021751"] as InputField;
            skuCodeField.Value = "616730-101";



            var request = new TmallProductSchemaMatchRequest();
            request.CategoryId = qingshaonianxie_catid;


            var fieldList = SchemaFactory.CreateEmptyFieldList();
            fieldList.Add(brandFiled);
            fieldList.Add(skuCodeField);

            request.Propvalues = SchemaWriter.WriteParamXmlString(fieldList);

            var rsp = client.Execute(request, sessionKey);

            if (!string.IsNullOrEmpty(rsp.MatchResult))
            {
                var r = new TmallItemAddSchemaGetRequest();
                r.CategoryId = qingshaonianxie_catid;
                r.ProductId = Convert.ToInt64(rsp.MatchResult);//
                r.Type = "b";
                r.IsvInit = true;

                var addItemRsp = client.Execute(r, sessionKey);

                var fields = SchemaReader.ReadXmlForMap(addItemRsp.AddItemResult);

                if (addItemRsp != null)
                {
                    TmallItemSchemaAddRequest ir = new TmallItemSchemaAddRequest();
                    ir.CategoryId = sportItemCatId;
                    ir.ProductId = Convert.ToInt64(rsp.MatchResult);//r.ProductId;

                    var itemAddFields = new List<Field>();
                    (fields["title"] as InputField).Value = "银泰商业泛渠道业务部测试商品,拍下不发货！";

                    itemAddFields.Add(fields["title"]);

                    (fields["sell_point"] as InputField).Value = "Wallace test!不发货、不退款，请飘过!";
                    itemAddFields.Add(fields["sell_point"]);

                    (fields["item_status"] as SingleCheckField).SetValue("2");
                    itemAddFields.Add(fields["item_status"]);

                    (fields["item_type"] as SingleCheckField).SetValue("b");
                    itemAddFields.Add(fields["item_type"]);

                    (fields["stuff_status"] as SingleCheckField).SetValue("5");
                    itemAddFields.Add(fields["stuff_status"]);

                    (fields["sub_stock"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["sub_stock"]);

                    (fields["auction_point"] as InputField).Value = "0.5";

                    itemAddFields.Add(fields["auction_point"]);

                    (fields["prop_129498239"] as SingleCheckField).SetValue("21958");
                    itemAddFields.Add(fields["prop_129498239"]);

                    (fields["prop_122216347"] as SingleCheckField).SetValue("380120406");
                    itemAddFields.Add(fields["prop_122216347"]);

                    (fields["has_warranty"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["has_warranty"]);

                    (fields["has_invoice"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["has_invoice"]);

                    (fields["outer_id"] as InputField).Value = "10101";
                    itemAddFields.Add(fields["outer_id"]);

                    (fields["is_xinpin"] as SingleCheckField).SetValue("false");
                    itemAddFields.Add(fields["is_xinpin"]);

                    (fields["description"] as InputField).Value = "此处为商品描述+价格 原价233.3";
                    itemAddFields.Add(fields["description"]);

                    (fields["valid_thru"] as SingleCheckField).SetValue("7");
                    itemAddFields.Add(fields["valid_thru"]);

                    (fields["is_taobao"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["is_taobao"]);

                    (fields["is_ex"] as SingleCheckField).SetValue("true");
                    itemAddFields.Add(fields["is_ex"]);

                    (fields["freight_payer"] as SingleCheckField).SetValue("2");
                    itemAddFields.Add(fields["freight_payer"]);

                    var location = fields["location"] as ComplexField;

                    var vl = new ComplexValue();
                    vl.SetInputFieldValue("prov", "浙江");

                    vl.SetInputFieldValue("city", "杭州");

                    location.SetComplexValue(vl);
                    itemAddFields.Add(location);

                    (fields["price"] as InputField).Value = "200";
                    itemAddFields.Add(fields["price"]);

                    (fields["quantity"] as InputField).Value = "200";
                    itemAddFields.Add(fields["quantity"]);

                    (fields["service_version"] as InputField).Value = "10000";
                    itemAddFields.Add(fields["service_version"]);


                    (fields["lang"] as SingleCheckField).SetValue("zh_CN");
                    itemAddFields.Add(fields["lang"]);

                    var colorField = fields["prop_extend_1627207"] as MultiComplexField;

                    vl = new ComplexValue();
                    vl.SetSingleCheckFieldValue("prop_1627207", new Value("3232483"));
                    vl.SetInputFieldValue("alias_name", "自定义颜色");
                    colorField.AddComplexValue(vl);
                    itemAddFields.Add(colorField);

                    var sizeField = fields["prop_extend_20549"] as MultiComplexField;
                    vl = new ComplexValue();
                    vl.SetSingleCheckFieldValue("prop_20549", new Value("186456005"));
                    vl.SetInputFieldValue("alias_name", "40");

                    sizeField.AddComplexValue(vl);

                    itemAddFields.Add(sizeField);


                    var skuField = fields["sku"] as MultiComplexField;

                    vl = new ComplexValue();
                    vl.SetSingleCheckFieldValue("prop_1627207", new Value("3232483"));
                    vl.SetSingleCheckFieldValue("prop_20549", new Value("186456005"));
                    vl.SetInputFieldValue("sku_price", "200");
                    vl.SetInputFieldValue("sku_quantity", "200");
                    vl.SetInputFieldValue("sku_outerId", "101101");

                    skuField.AddComplexValue(vl);

                    itemAddFields.Add(skuField);

                    ir.XmlData = SchemaWriter.WriteParamXmlString(itemAddFields);

                    var itemRsp = client.Execute(ir, sessionKey);

                    Assert.IsNotNull(itemRsp);
                }
            }

            Assert.IsNotNull(rsp);
        }


        [TestMethod]
        public void TestPictureUpload()
        {
            var uploadRequest = new PictureUploadRequest
            {
                ImageInputTitle = "test title",
                Img = new FileItem(@"E:\Photo\1.jpg"),
                PictureCategoryId = 0,
                //ClientType = "client:phone"
            };
            var rsp = client.Execute(uploadRequest, sessionKey);



            Assert.IsNotNull(rsp);
        }
    }
}
