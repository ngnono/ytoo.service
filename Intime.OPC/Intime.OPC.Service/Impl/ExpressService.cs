using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service.Contract;
using System;
using System.Linq;
using System.Transactions;

namespace Intime.OPC.Service.Impl
{
    public class ExpressService:IExpressService
    {
        public void CreatePackage(ShippingSaleCreateDto package, int uid)
        {
            using (var db = new YintaiHZhouContext())
            {
                var order = db.Orders.FirstOrDefault(x => x.OrderNo == package.OrderNo);
                if (order == null)
                {
                    throw new PackageException("订单无效");
                }

                var shippingSale =
                    db.OPC_ShippingSale.FirstOrDefault(
                        x => x.OrderNo == package.OrderNo && x.ShippingCode == package.ShippingCode);
                if (shippingSale != null)
                {
                    shippingSale.ShippingCode = package.ShippingCode;
                    shippingSale.ShipViaName = package.ShipViaName;
                    shippingSale.ShipViaId = package.ShipViaID;
                    shippingSale.ShippingFee = package.ShippingFee;
                    shippingSale.UpdateDate = DateTime.Now;
                    shippingSale.UpdateUser = uid;
                    db.SaveChanges();
                }
                else
                {
                    shippingSale = new OPC_ShippingSale()
                    {
                        CreateDate = DateTime.Now,
                        CreateUser = uid,
                        UpdateDate = DateTime.Now,
                        UpdateUser = uid,
                        OrderNo = package.OrderNo,
                        ShipViaId = package.ShipViaID,
                        ShippingCode = package.ShippingCode,
                        ShippingFee = (decimal)(package.ShippingFee),
                        ShippingStatus = EnumSaleOrderStatus.PrintInvoice.AsID(),
                        ShipViaName = package.ShipViaName,
                        ShippingAddress = order.ShippingAddress,
                        ShippingContactPerson = order.ShippingContactPerson,
                        ShippingContactPhone = order.ShippingContactPhone,
                        //StoreId = section.StoreId,
                    };
                    shippingSale = db.OPC_ShippingSale.Add(shippingSale);
                    db.SaveChanges();
                }

                using (var ts = new TransactionScope())
                {
                    foreach (var saleOrderNo in package.SaleOrderIDs)
                    {
                        var saleOrder = db.OPC_Sale.FirstOrDefault(x => x.SaleOrderNo == saleOrderNo);
                        if (saleOrder == null)
                        {
                            continue;
                            //throw new PackageException("包裹中包含无效的销售单");
                        }
                        saleOrder.ShippingSaleId = shippingSale.Id;
                        saleOrder.ShipViaId = package.ShipViaID;
                        saleOrder.ShippingCode = package.ShippingCode;
                        saleOrder.ShippingStatus = shippingSale.ShippingStatus;
                        saleOrder.UpdatedDate = DateTime.Now;
                        saleOrder.UpdatedUser = uid;
                        saleOrder.Status = (int)EnumSaleOrderStatus.PrintExpress;
                        saleOrder.ShippingFee = package.ShippingFee;
                        db.SaveChanges();
                    }
                    ts.Complete();
                }
            }
        }
    }
}
