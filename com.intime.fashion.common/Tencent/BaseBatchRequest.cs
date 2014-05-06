using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace com.intime.fashion.common.Tencent
{
    public class BaseBatchRequest
    {
        [XmlIgnore]
        public string EncodedFormat
        {
            get
            {
                var xs = new XmlSerializer(this.GetType());
                StringBuilder builder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Encoding = Encoding.UTF8;
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                using (var stringWriter = XmlWriter.Create(builder, settings))
                {
                    //builder.Append("<?xml version=\"1.0\" encoding=\"GB2312\" ?>");
                    xs.Serialize(stringWriter, this, ns);
                }
                var baseEncodedXml = Convert.ToBase64String(Config.DEFAULT_ENCODE.GetBytes(builder.ToString()));
               // var rawXml = "<root><client_ip>101.225.56.37</client_ip><op_code>1013</op_code><op_name>batch_draw</op_name><op_passwd>ab9c9b0a0881957ab2c9e684abed5191</op_passwd><op_time>20140506162057SSS</op_time><op_user>1218868101999</op_user><package_id>201404091</package_id><record_set><record><serial>1</serial><rec_bankacc>7887234</rec_bankacc><bank_type>1010</bank_type><rec_name>jkik</rec_name><pay_amt>1</pay_amt><acc_type>1</acc_type><desc>银泰分成</desc><recv_mobile /></record><record><serial>2</serial><rec_bankacc>877722334</rec_bankacc><bank_type>1010</bank_type><rec_name>kjk</rec_name><pay_amt>1</pay_amt><acc_type>1</acc_type><desc>银泰分成</desc><recv_mobile /></record></record_set><sp_id>1218868101</sp_id><total_amt>2</total_amt><total_num>2</total_num></root>";
                //var baseEncodedXml = Convert.ToBase64String(Config.DEFAULT_ENCODE.GetBytes(rawXml));
                var md5Xml = CommonUtil.MD5_Encode(baseEncodedXml, Config.DEFAULT_ENCODE).ToLower();
                var signedXml = CommonUtil.MD5_Encode(string.Concat(md5Xml, Config.PARTER_KEY), Config.DEFAULT_ENCODE).ToLower();
                return string.Format("content={0}&abstract={1}",HttpUtility.UrlEncode(baseEncodedXml,Config.DEFAULT_ENCODE), HttpUtility.UrlEncode(signedXml,Config.DEFAULT_ENCODE));
            }
        }
    }
}
