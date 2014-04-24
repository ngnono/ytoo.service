namespace OPCApp.TransManage.Print
{
    public interface IPrint
    {
        void Print(string xsdName, string rdlcName, PrintModel dtList, bool isFast);
    }
}