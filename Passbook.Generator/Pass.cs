using System.IO;

namespace Passbook.Generator
{
    public class Pass
    {
        private readonly string _packagePathAndName;

        public Pass(string packagePathAndName)
        {
            this._packagePathAndName = packagePathAndName;
        }

        public byte[] GetPackage()
        {
            return File.ReadAllBytes(this._packagePathAndName);
        }
    }
}