using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto.ReturnGoods;
using OPCApp.ReturnGoodsManage.ViewModel;

namespace OPCApp.ReturnGoodsManage.Common
{
    public class BaseReturnGoodsSearchCommonWithRma : BindableBase
    {
        private CustomReturnGoodsUserControlViewModel _curtomReturnGoodsUcViewModel;

        private ReturnGoodsCommonSearchDto _returnGoodsCommonSearchDto;

        public BaseReturnGoodsSearchCommonWithRma()
        {
            CommandSearch = new DelegateCommand(SearchRma);
            ReturnGoodsCommonSearchDto = new ReturnGoodsCommonSearchDto();
        }

        [Import(typeof (CustomReturnGoodsUserControlViewModel))]
        public CustomReturnGoodsUserControlViewModel CustomReturnGoodsUserControlViewModel
        {
            get { return _curtomReturnGoodsUcViewModel; }
            set { SetProperty(ref _curtomReturnGoodsUcViewModel, value); }
        }

        public ReturnGoodsCommonSearchDto ReturnGoodsCommonSearchDto
        {
            get { return _returnGoodsCommonSearchDto; }
            set { SetProperty(ref _returnGoodsCommonSearchDto, value); }
        }

        public DelegateCommand CommandSearch { get; set; }

        public virtual void SearchRma()
        {
        }

        public virtual bool VerifyRmaSelected()
        {
            List<RMADto> rmaList = CustomReturnGoodsUserControlViewModel.RmaList;
            if (rmaList == null || rmaList.Count == 0)
            {
                MessageBox.Show("请选择退货单", "提示");
                return false;
            }
            return true;
        }

        public virtual List<string> GetRmoNoList()
        {
            return CustomReturnGoodsUserControlViewModel.RmaList.Where(e => e.IsSelected).Select(e => e.RMANo).ToList();
        }

        public virtual void Refresh()
        {
            List<RmaDetail> detailRma = CustomReturnGoodsUserControlViewModel.RmaDetailList;
            if (detailRma != null && detailRma.Count != 0)
            {
                CustomReturnGoodsUserControlViewModel.RmaDetailList.Clear();
            }
            SearchRma();
        }
    }
}