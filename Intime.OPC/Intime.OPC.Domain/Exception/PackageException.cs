
namespace Intime.OPC.Domain.Exception
{
    public class OpcExceptioin : System.Exception
    {
        public OpcExceptioin(string message) : base(message)
        {
        }
    }

    public class PackageException:OpcExceptioin
    {
        public PackageException(string message) : base(message)
        {
        }
    }
}
