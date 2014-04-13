using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCAPP.Domain.Dto.Financial
{
    public class FinancialMainDetailDto
    {
        public FinancialMainDetailDto()
        {
            BuyDate = DateTime.Now;
            RmaDate = DateTime.Now;
            ApplyRmaDate = DateTime.Now;            
        }
        #region 共同的字段
        /// <summary>
        /// 门店名称
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        ///     订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        ///     订单渠道号
        /// </summary>
        public string OrderChannelNo { get; set; }

        /// <summary>
        /// 销售单号
        /// </summary>
        public string SaleOrderNo { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public string OrderSouce { get; set; }


        /// <summary>
        /// 购买时间
        /// </summary>
        public DateTime BuyDate { get; set; }


        /// <summary>
        ///     品牌
        /// </summary>
        /// <value>The Brand.</value>
        public string Brand { get; set; }

        /// <summary>
        ///     款号
        /// </summary>
        /// <value>The style no.</value>
        public string StyleNo { get; set; }

        /// <summary>
        ///     规格
        /// </summary>
        /// <value>The standard.</value>
        public string Size { get; set; }

        /// <summary>
        ///     色码
        /// </summary>
        /// <value>The color.</value>
        public string Color { get; set; }

        /// <summary>
        ///     销售数量
        /// </summary>
        /// <value>The sale count.</value>
        public int SellCount { get; set; }

        /// <summary>
        ///     吊牌价格
        /// </summary>
        /// <value>The label price.</value>
        public double LabelPrice { get; set; }

        /// <summary>
        ///     销售价格
        /// </summary>
        /// <value>The sale price.</value>
        public double SalePrice { get; set; }

        /// <summary>
        ///     销售金额
        /// </summary>
        /// <value>The sale Total price.</value>
        public double SaleTotalPrice { get; set; }

        /// <summary>
        /// 专柜码
        /// </summary>
        public string SectionCode { get; set; }
        #endregion

        #region 
        /// <summary>
        /// 订单运费
        /// </summary>
        public decimal? OrderTransFee { get; set; }
        #endregion

        #region

        /// <summary>
        /// 退货单号
        /// </summary>
        /// <value>The rma no.</value>
        public string RMANo { get; set; }
        /// <summary>
        /// 详单类型 销售单？退货单？
        /// </summary>
        public string DetailType { get; set; }

        /// <summary>
        /// 收银流水号
        /// </summary>
        /// <value>The cash number.</value>
        public string CashNum { get; set; }

        /// <summary>
        /// 退货收银流水号
        /// </summary>
        /// <value>The cash number.</value>
        public string RmaCashNum { get; set; }

        #endregion

        #region

        /// <summary>
        /// 退货状态
        /// </summary>
        /// <value>The name of the rma cash status.</value>
        public string RmaStatusName { get; set; }

        /// <summary>
        /// 退货总数
        /// </summary>
        /// <value>The count.</value>
        public int? Count { get; set; }

        /// <summary>
        /// 退货价格
        /// </summary>
        public decimal RmaPrice { get; set; }


        /// <summary>
        /// 退货时间
        /// </summary>
        /// <value>The created date.</value>
        public DateTime RmaDate { get; set; }

        /// <summary>
        /// 退货申请时间
        /// </summary>
        public DateTime ApplyRmaDate { get; set; }
        #endregion
       

    }
}
