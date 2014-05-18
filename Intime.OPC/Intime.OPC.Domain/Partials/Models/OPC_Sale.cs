using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intime.OPC.Domain.Models
{

    public partial class OPC_Sale
    {
        [NotMapped]
        public virtual Store Store { get; set; }

        [NotMapped]
        public virtual Section Section { get; set; }

        [NotMapped]
        public virtual OrderTransaction OrderTransaction { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        [NotMapped]
        public virtual Order Order { get; set; }

        [NotMapped]
        public virtual OPC_ShippingSale ShippingSale { get; set; }
    }

    
    public partial class OPC_ShippingSale 
    {
        /// <summary>
        /// 销售单
        /// </summary>
        [NotMapped]
        public virtual List<OPC_Sale> SaleOrders { get; set; }

        /// <summary>
        /// 订单商品
        /// </summary>
        [NotMapped]
        public virtual List<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static OPC_ShippingSale Convert2ShippingOrderModel(dynamic obj)
        {
            if (obj == null)
            {
                return null;
            }

            return new OPC_ShippingSale
            {
                BrandId = obj.BrandId,
                CreateDate = obj.CreateDate,
                CreateUser = obj.CreateUser,
                Id = obj.Id,
                OrderNo = obj.OrderNo,
                PrintTimes = obj.PrintTimes,
                RmaNo = obj.RmaNo,
                ShippingAddress = obj.ShippingAddress,
                ShippingCode = obj.ShippingCode,
                ShippingContactPerson = obj.ShippingContactPerson,
                ShippingContactPhone = obj.ShippingContactPhone,
                ShipViaId = obj.ShipViaId,
                ShipViaName = obj.ShipViaName,
                StoreId = obj.StoreId,
                UpdateDate = obj.UpdateDate,
                UpdateUser = obj.UpdateUser,

                //SaleOrders = obj.SaleOrders,
                //OrderItems = obj.OrderItems
            };
        }
    }
}