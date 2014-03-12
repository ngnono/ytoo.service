
namespace Intime.OPC.WebApi.Core.MessageHandlers
{
    public interface IAppSecurityManager
    {
        /// <summary>
        /// 根据AppKey获取SecretKey
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        string GetSecretKey(string appKey);

        /// <summary>
        /// 是否开启安全验证
        /// </summary>
        bool Enabled { get; }
    }
}
