using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Intime.OPC.Domain.Dto
{
    /// <summary>
    /// 专柜
    /// </summary>
    public class SectionDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //读
        //OPC_CHANNELMAP
        //MAPTYPE=5
        //INVALUE=SectionId

        //ChannelValue

        /// <summary>
        /// 只能新增，不能修改
        /// </summary>
        public string Code { get; set; }

        [JsonProperty(PropertyName = "ContactPhoneNumber")]
        public string ContactPhone { get; set; }

        [JsonIgnore]
        public int? Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled
        {
            get
            {
                if (Status == null)
                {
                    return null;
                }

                return Status == 1;
            }
            set
            {
                if (value == null)
                {
                    Status = null;

                }
                else
                {
                    if (value == true)
                    {
                        Status = 1;
                    }
                    else
                    {
                        Status = -1;
                    }
                }



            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<BrandDto> Brands { get; set; }
    }
}