using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using OPCAPP.Domain.Enums;
using OPCApp.TransManage.Models;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.TransManage.IService;
using OPCApp.DataService.Interface.Trans;
using Intime.OPC.Domain.Models;
using OPCApp.Infrastructure;
using System.ComponentModel.Composition;


namespace OPCApp.TransManage.ViewModels
{
    [Export("RMAGoodsInStoreViewModel", typeof(PrintInvoiceViewModel))]
    public class RMACashInViewModel : BindableBase
    {
        public EnumRMACashStatus SearchRMAStatus { get; set; }
        //Grid退货数据集
        private IEnumerable<OPC_RMA> rmaList;
        public IEnumerable<OPC_RMA> RMAList
        {

            get { return this.rmaList; }
            set { SetProperty(ref this.rmaList, value); }
        }

        //界面查询条件
        private OPC_RMAGet rmaget = new OPC_RMAGet();
        public OPC_RMAGet RMAGet
        {
            get { return this.rmaget; }
            set { SetProperty(ref this.rmaget, value); }
        }

        //选择上面行数据时赋值的数据集
        private OPC_RMA rmaSelect = new OPC_RMA();
        public OPC_RMA RMASelect
        {
            get { return this.rmaSelect; }
            set { SetProperty(ref this.rmaSelect, value); }

        }
        //退货明细Grid数据集
        private IEnumerable<OPC_RMADetail> rmaDetaillist;
        public IEnumerable<OPC_RMADetail> RMADetailList
        {

            get { return this.rmaDetaillist; }
            set { SetProperty(ref this.rmaDetaillist, value); }
        }

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandRMACashIn { get; set; }
        public DelegateCommand CommandFinishRMACashIn { get; set; }
        public DelegateCommand CommandGetDown { get; set; }
        public RMACashInViewModel()
        {
            //初始化命令属性
            this.CommandSearch = new DelegateCommand(this.Search);
            this.CommandRMACashIn = new DelegateCommand(this.RMACashIn);
            this.CommandFinishRMACashIn = new DelegateCommand(this.FinishRMACashIn);
            this.CommandGetDown = new DelegateCommand(this.GetDown);

            this.SearchRMAStatus = EnumRMACashStatus.NoCash;
        }

        public void RMACashIn()
        {

        }
        public void FinishRMACashIn()
        {
            try
            {
                IRMACashInService iRMACashInService = AppEx.Container.GetInstance<IRMACashInService>();
                bool bFalg = iRMACashInService.FinishRMACashIn(RMASelect);
                if (bFalg)
                {
                    MessageBox.Show("完成退货单入收银成功");
                    this.Refresh();
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
            var salesfilter = string.Format("startdate={0}&enddate={1}&orderno={2}", RMAGet.StartGoodsGetDate, RMAGet.EndGoodsGetDate, RMAGet.OrderNo);
            PageResult<OPC_RMA> re = AppEx.Container.GetInstance<IRMACashInService>().GetRMA(salesfilter, this.SearchRMAStatus);
            this.RMAList = re.Result;
        }

        public void GetDown()
        {
            RMADetailList = AppEx.Container.GetInstance<IRMACashInService>().GetRMADetailByRMANo(RMASelect.RMANo).Result;
        }



    }
}
