namespace OPCApp.TransManage.IService
{
    //封装的打印接口,从界面上取得数值，打印时不与后台交互
    public interface IPrintService
    {
        void ViewAndPrint(int type);

        void FastPrint(int type);
    } //

    public class PrintService : IPrintService
    {
        public void ViewAndPrint(int type)
        {
        }

        public void FastPrint(int type)
        {
        }
    }
}