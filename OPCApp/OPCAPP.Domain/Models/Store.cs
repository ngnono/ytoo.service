using System;

namespace OPCApp.Domain.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Tel { get; set; }
        public int CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int Group_Id { get; set; }
        public int Status { get; set; }
        public int Region_Id { get; set; }
        public int StoreLevel { get; set; }
        public decimal? GpsLat { get; set; }
        public decimal? GpsLng { get; set; }
        public decimal? GpsAlt { get; set; }
        public int? ExStoreId { get; set; }
        public string RMAAddress { get; set; }
        public string RMAZipCode { get; set; }
        public string RMAPerson { get; set; }
        public string RMAPhone { get; set; }
    }
}