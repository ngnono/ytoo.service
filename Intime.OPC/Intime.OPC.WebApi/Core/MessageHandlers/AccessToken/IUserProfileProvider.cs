using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;

namespace Intime.OPC.WebApi.Core.MessageHandlers
{
    public interface IUserProfileProvider
    {
        UserProfile GetByUserId(int userId);
    }
}
