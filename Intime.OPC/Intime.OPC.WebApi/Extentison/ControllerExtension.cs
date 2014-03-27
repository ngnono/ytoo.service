using Intime.OPC.Common.Logger;

namespace System.Web.Http
{
    public static class ControllerExtension
    {
        public static ILog GetLog(this ApiController controller)
        {
            return LoggerManager.Current();
        }
    }
}