using OPCApp.Domain.Enums;

namespace OPCApp.DataService.IService
{
    public interface IRemark
    {
        void ShowRemarkWin(string id, EnumSetRemarkType type);
    }
}