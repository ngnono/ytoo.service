using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Service.Logic
{
    public class OrderRule
    {
        public static string CreateCode(int storeId)
        {
            var code = string.Concat(string.Format("1{0}", DateTime.Now.ToString("yyMMdd"))
                        , DateTime.UtcNow.Ticks.ToString().Reverse().Take(5)
                            .Aggregate(new StringBuilder(), (s, e) => s.Append(e), s => s.ToString())
                            .PadRight(5, '0'));
            IOrderRepository couponData = ServiceLocator.Current.Resolve<IOrderRepository>();
            var existingCodes = couponData.Get(c => c.OrderNo == code && c.CreateDate>=DateTime.Today).Count();
            if (existingCodes > 0)
                code = string.Concat(code, (existingCodes + 1).ToString());
            return code;
        }
        public static decimal ComputeFee(OrderRequest order)
        {
            return 0;
        }
        public static string CreateShippingCode(string storeId)
        {
            var code = string.Concat(string.Format("O{0}{1}",storeId.PadLeft(3,'0'), DateTime.Now.ToString("yyMMdd"))
                               , DateTime.UtcNow.Ticks.ToString().Reverse().Take(5)
                                   .Aggregate(new StringBuilder(), (s, e) => s.Append(e), s => s.ToString())
                                   .PadRight(5, '0'));
            IOutboundRepository outboundRepo = ServiceLocator.Current.Resolve<IOutboundRepository>();
            var existingCodes = outboundRepo.Get(c => c.OutboundNo == code && c.CreateDate >= DateTime.Today).Count();
            if (existingCodes > 0)
                code = string.Concat(code, (existingCodes + 1).ToString());
            return code;
        }

        public static string CreateRMACode()
        {
            var code = string.Concat(string.Format("R{0}", DateTime.Now.ToString("yyMMdd"))
                       , DateTime.UtcNow.Ticks.ToString().Reverse().Take(5)
                           .Aggregate(new StringBuilder(), (s, e) => s.Append(e), s => s.ToString())
                           .PadRight(5, '0'));
            IRMARepository rmadata = ServiceLocator.Current.Resolve<IRMARepository>();
            var existingCodes = rmadata.Get(c => c.RMANo == code && c.CreateDate >= DateTime.Today).Count();
            if (existingCodes > 0)
                code = string.Concat(code, (existingCodes + 1).ToString());
            return code;
        }

        public static OrderComputeResult ComputeAmount(ProductEntity linq, int quantity)
        {
            return new OrderComputeResult() { 
                 TotalAmount= linq.Price *quantity,
                  ExtendPrice = linq.Price * quantity,
                   TotalFee = 0m,
                    TotalPoints = 0,
                     TotalQuantity = quantity
            };
          
        }

        public class OrderComputeResult
        {
         
            public int TotalQuantity { get; set; }
            public int TotalPoints { get; set; }
            public decimal TotalFee {get;set;}
            public decimal ExtendPrice {get;set;}
            public decimal TotalAmount {get;set;}
             
        }



        public static OrderComputeResult ComputeFee()
        {
            return new OrderComputeResult()
            {
                TotalAmount = 0,
                ExtendPrice = 0,
                TotalFee = 0m,
                TotalPoints = 0,
                TotalQuantity = 0
            };
        }
    }
}
