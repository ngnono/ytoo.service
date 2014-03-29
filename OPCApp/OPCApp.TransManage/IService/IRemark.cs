using OPCAPP.Domain.Enums;

namespace OPCApp.TransManage.IService
{
    public interface IRemark
    {
        void ShowRemarkWin(string id, EnumSetRemarkType type);
    }
}