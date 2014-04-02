using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.TransManage.ViewModels
{
    [Export("RMAGoodsInStoreViewModel", typeof (PrintInvoiceViewModel))]
    public class RMACashInViewModel : BindableBase
    {
        //Grid退货数据集
        private IEnumerable<OPC_RMADetail> rmaDetaillist;
        private IEnumerable<OPC_RMA> rmaList;
        private OPC_RMA rmaSelect = new OPC_RMA();

        //界面查询条件
        private OPC_RMAGet rmaget = new OPC_RMAGet();

        public RMACashInViewModel()
        {
            //初始化命令属性
            CommandSearch = new DelegateCommand(Search);
            CommandRMACashIn = new DelegateCommand(RMACashIn);
            CommandFinishRMACashIn = new DelegateCommand(FinishRMACashIn);
            CommandGetDown = new DelegateCommand(GetDown);

            SearchRMAStatus = EnumRMACashStatus.NoCash;
        }

        public EnumRMACashStatus SearchRMAStatus { get; set; }

        public IEnumerable<OPC_RMA> RMAList
        {
            get { return rmaList; }
            set { SetProperty(ref rmaList, value); }
        }

        public OPC_RMAGet RMAGet
        {
            get { return rmaget; }
            set { SetProperty(ref rmaget, value); }
        }

        //选择上面行数据时赋值的数据集

        public OPC_RMA RMASelect
        {
            get { return rmaSelect; }
            set { SetProperty(ref rmaSelect, value); }
        }

        //退货明细Grid数据集

        public IEnumerable<OPC_RMADetail> RMADetailList
        {
            get { return rmaDetaillist; }
            set { SetProperty(ref rmaDetaillist, value); }
        }

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandRMACashIn { get; set; }
        public DelegateCommand CommandFinishRMACashIn { get; set; }
        public DelegateCommand CommandGetDown { get; set; }

        public void RMACashIn()
        {
        }

        public void FinishRMACashIn()
        {
            try
            {
                var iRMACashInService = AppEx.Container.GetInstance<IRMACashInService>();
                bool bFalg = iRMACashInService.FinishRMACashIn(RMASelect);
                if (bFalg)
                {
                    MessageBox.Show("完成退货单入收银成功");
                    Refresh();
                }
                else
                {
                    MessageBox.Show("操作失败");
                }
            }
            catch
            {
            }
        }

        public void Search()
        {
            Refresh();
        }

        public void Refresh()
        {
            string salesfilter = string.Format("startdate={0}&enddate={1}&orderno={2}", RMAGet.StartGoodsGetDate,
                RMAGet.EndGoodsGetDate, RMAGet.OrderNo);
            PageResult<OPC_RMA> re = AppEx.Container.GetInstance<IRMACashInService>()
                .GetRMA(salesfilter, SearchRMAStatus);
            RMAList = re.Result;
        }

        public void GetDown()
        {
            RMADetailList = AppEx.Container.GetInstance<IRMACashInService>().GetRMADetailByRMANo(RMASelect.RMANo).Result;
        }
    }
}