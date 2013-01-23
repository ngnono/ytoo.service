using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Yintai.Hangzhou.Contract.Response
{
    public interface IResponse
    {
    }

    [DataContract]
    public abstract class BaseResponse : IResponse
    {
    }
}
