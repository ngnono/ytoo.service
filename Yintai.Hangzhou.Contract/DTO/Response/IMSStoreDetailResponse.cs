using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Contract.Response;

namespace Yintai.Hangzhou.Contract.DTO.Response
{
    [DataContract]
    public class IMSStoreDetailResponse:BaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
