using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Models
{
    public class SectionClone
    {
        public string Location { get; set; }
        public string ContactPhone { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> BrandId { get; set; }
        public Nullable<int> StoreId { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUser { get; set; }
        public string ContactPerson { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string StoreCode { get; set; }
        public Nullable<int> ChannelSectionId { get; set; }
        public IEnumerable<BrandClone> Brands { get; set; }
    }

    public class OPC_SupplierInfoClone
    {
        public int Id { get; set; }
        public string SupplierNo { get; set; }
        public string SupplierName { get; set; }
        public Nullable<int> StoreId { get; set; }
        public string Corporate { get; set; }
        public string Contact { get; set; }
        public string Contract { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string FaxNo { get; set; }
        public string TaxNo { get; set; }
        public int Status { get; set; }
        public string Memo { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public IEnumerable<BrandClone> Brands { get; set; }
    }

    public partial class Brand
    {
        public IEnumerable<Section> Sections { get; set; }

        public IEnumerable<OPC_SupplierInfo> Suppliers { get; set; }

        public static Brand Convert2Brand(dynamic obj)
        {
            if (obj == null)
            {
                return null;
            }

            //return AutoMapper.Mapper.DynamicMap<Brand>(obj);


            return new Brand()
            {
                Id = obj.Id,
                Name = obj.Name,
                EnglishName = obj.EnglishName,
                ChannelBrandId = obj.ChannelBrandId,
                CreatedDate = obj.CreatedDate,
                CreatedUser = obj.CreatedUser,
                Description = obj.Description,
                Group = obj.Group,
                Logo = obj.Logo,
                Status = obj.Status,
                UpdatedDate = obj.UpdatedDate,
                UpdatedUser = obj.CreatedUser,
                WebSite = obj.WebSite
            };
        }
    }

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

    public partial class OPC_SupplierInfo
    {
        public IEnumerable<Brand> Brands { get; set; }

        public static OPC_SupplierInfo Convert2Supplier(dynamic obj)
        {
            if (obj == null)
            {
                return null;
            }

            var item = new OPC_SupplierInfo
            {
                Id = obj.Id,
                SupplierName = obj.SupplierName,
                SupplierNo = obj.SupplierNo,
                Brands = new List<Brand>(),
                Address = obj.Address,
                Contact = obj.Contact,
                Corporate = obj.Corporate,
                CreatedDate = obj.CreatedDate,
                Contract = obj.Contract,
                CreatedUser = obj.CreatedUser,
                FaxNo = obj.FaxNo,
                Memo = obj.Memo,
                Status = obj.Status,
                StoreId = obj.StoreId,
                TaxNo = obj.TaxNo,
                Telephone = obj.Telephone,
                UpdatedDate = obj.UpdatedDate,
                UpdatedUser = obj.UpdatedUser,


            };

            return item;
        }
    }

    public partial class IMS_SectionBrand
    {
        public int Id { get; set; }

        public int SectionId { get; set; }

        public int BrandId { get; set; }
    }
}
