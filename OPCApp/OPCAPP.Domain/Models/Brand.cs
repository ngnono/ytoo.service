using System;

namespace OPCApp.Domain.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string Description { get; set; }
        public int CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public string Logo { get; set; }
        public string WebSite { get; set; }
        public int Status { get; set; }
        public string Group { get; set; }
        public int? ChannelBrandId { get; set; }
    }
}