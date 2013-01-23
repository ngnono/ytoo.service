using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Service
{
    public abstract class BaseService
    {
        public static ILog Logger;

        public MappingManagerV2 MappingManager { get; set; }



        protected BaseService()
        {
            MappingManager = ServiceLocator.Current.Resolve<MappingManagerV2>();
        }

        static BaseService()
        {
            Logger = LoggerManager.Current();
        }
    }
}