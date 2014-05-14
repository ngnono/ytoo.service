using System.Collections.Generic;

namespace Intime.OPC.Domain.Models
{
    public partial class Section
    {
        public IEnumerable<Brand> Brands { get; set; }

        public static Section Convert2Section(dynamic obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new Section()
            {
                BrandId = obj.BrandId,
                ChannelSectionId = obj.ChannelSectionId,
                ContactPerson = obj.ContactPerson,
                ContactPhone = obj.ContactPhone,
                CreateDate = obj.CreateDate,
                CreateUser = obj.CreateUser,
                Location = obj.Location,
                Name = obj.Name,
                Id = obj.Id,
                Status = obj.Status,
                StoreCode = obj.StoreCode,
                StoreId = obj.StoreId,
                UpdateDate = obj.UpdateDate,
                UpdateUser = obj.CreateUser,

            };
        }
    }
}