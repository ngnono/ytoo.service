using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Infrastructure;
using OPCApp.Domain.ReturnGoods;
using OPCApp.Domain.Customer;
using OPCApp.ReturnGoodsManage.Common;
using OPCApp.ReturnGoodsManage.Print;
using OPCApp.Domain.Models;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
    [Export(typeof (ReturnGoodsEntryPrintViewModel))]
    public class ReturnGoodsEntryPrintViewModel : BaseReturnGoodsSearchCommonWithRma
    {
        private List<RmaDetail> _rmaDetails;
        private RMADto _rmaDto;
        private List<RMADto> _rmaDtoList;
        private RmaExpressDto _rmaExpressDto;

        public ReturnGoodsEntryPrintViewModel()
        {
            CommandPrintPreview = new DelegateCommand(PrintPreView);
            CommandPrintReturnGoodsConfirm = new DelegateCommand(ReturnGoodsConfirm);
        }

        public DelegateCommand CommandPrintReturnGoodsConfirm { get; set; }

        public DelegateCommand CommandPrintPreview { get; set; }

        public RMADto RMADto
        {
            get { return _rmaDto; }
            set { SetProperty(ref _rmaDto, value); }
        }


        public List<RmaDetail> RmaDetailList
        {
            get { return _rmaDetails; }
            set { SetProperty(ref _rmaDetails, value); }
        }

        public List<RMADto> RMADtoList
        {
            get { return _rmaDtoList; }
            set { SetProperty(ref _rmaDtoList, value); }
        }

        private void ReturnGoodsConfirm()
        {
            if (VerifyRmaSelected())
            {
                List<string> rmaNos = GetRmoNoList();
                bool falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().PrintReturnGoodsComplete(rmaNos);
                MessageBox.Show(falg ? "设置打印完成" : "设置打印完成失败", "提示");
                if (falg)
                {
                    Refresh();
                }
            }
        }

        private bool PrintCommon(bool falg = false)
        {
            if (!VerifyRmaSelected())
            {
                MessageBox.Show("请勾选要打印预览的退货单", "提示");
                return false;
            }
            IPrint pr = new PrintWin();
            string xsdName = "InvoiceDataSet";
            string rdlcName = "Print//PrintRMA.rdlc";

            var invoiceModel = new PrintModel();

            var salelist = new List<OPC_RMA>();
            //SaleSelected.TransName = SaleSelected.IfTrans;
            //salelist.Add(SaleSelected);
            //invoiceModel.RmaDT = salelist;
            //invoiceModel.RMADetailDT = InvoiceDetail4List;
            pr.Print(xsdName, rdlcName, invoiceModel, falg);
            return true;

        }

        private void PrintPreView()
        {
            if (VerifyRmaSelected())
            {
                List<string> rmaNos = GetRmoNoList();
                //bool falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().PrintReturnGoods(rmaNos);
                //MessageBox.Show(falg ? "打印退货单成功" : "打印退货单失败", "提示");


            }
        }

        public override void SearchRma()
        {
            try
            {
                CustomReturnGoodsUserControlViewModel.RmaList =
                    AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>()
                        .GetRmaForReturnPrintDoc(ReturnGoodsCommonSearchDto)
                        .ToList();
            }
            catch
            {
            }
        }
    }
}