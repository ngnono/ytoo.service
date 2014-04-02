using com.intime.fashion.common.IT;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class SMSController:RestfulController
    {
        static readonly Regex regex = new Regex(@"^(13[0-9]|15[0-9]|18[0-9])\d{8}$",RegexOptions.Compiled);
        [RestfulAuthorize]
        public ActionResult Send(string phone, string text)
        {
#if !DEBUG
            if (string.IsNullOrEmpty(text))
            {
                return this.RenderError(r => r.Message = "text is empty");
            }
            if (!regex.IsMatch(phone))
            {
                return this.RenderError(r => r.Message = "Incorrect phone number");
            }

            dynamic sendRsp = null;

            if (!ITServiceHelper.SendHttpMessage(new Request(new {phone, text}), r => sendRsp = r.Data, null) ||
                sendRsp == null || sendRsp.GetHashCode() != 200)
            {
                return this.RenderError(r => r.Message = "Failed to send sms message, please try again.");
            }
#endif
            return this.RenderSuccess<dynamic>(null);
        }
    }
}
