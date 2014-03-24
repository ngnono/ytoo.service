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
        [DataMember(Name="id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name="gift_card")]
        public IMSGiftCard GiftCardSaling { get; set; }
        [DataMember(Name="combos")]
        public IEnumerable<IMSCombo> ComboSaling { get; set; }
    }
    public class IMSGiftCard
    {
        public int Id { get; set; }
        public string Desc { get; set; }
        public string ImageUrl { get; set; }
    }
    public class IMSCombo
    {
        public int Id { get; set; }
        public string Desc { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
