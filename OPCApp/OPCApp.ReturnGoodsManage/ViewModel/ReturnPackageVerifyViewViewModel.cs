using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.RMA;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.ReturnGoodsManage.ViewModels
{
    [Export(typeof (ReturnPackageVerifyViewViewModel))]
    public class ReturnPackageVerifyViewViewModel : BindableBase
    {
        private PackageReceiveDto _packageReceiveDto;
        private List<RMADto> _rmaDtos;
        private List<RmaDetail> rmaDetails;
        //与包裹审核公用传输类


        public RMADto rmaDto;

        public ReturnPackageVerifyViewViewModel()
        {
            CommandSearch = new DelegateCommand(SearchRma);
            PackageReceiveDto = new PackageReceiveDto();
            CommandGetRmaDetailByRma = new DelegateCommand(GetRmaDetailByRma);
            CommandTransVerifyPass = new DelegateCommand(TransVerifyPass);
            CommandTransVerifyNoPass = new DelegateCommand(TransVerifyNoPass);
            CommandSetRmaRemark = new DelegateCommand(SetRmaRemark);
        }

        public PackageReceiveDto PackageReceiveDto
        {
            get { return _packageReceiveDto; }
            set { SetProperty(ref _packageReceiveDto, value); }
        }

        public RMADto RmaDto
        {
            get { return rmaDto; }
            set { SetProperty(ref rmaDto, value); }
        }

        public List<RMADto> RmaList
        {
            get { return _rmaDtos; }
            set { SetProperty(ref _rmaDtos, value); }
        }

        public List<RmaDetail> RmaDetailLs
        {
            get { return rmaDetails; }
            set { SetProperty(ref rmaDetails, value); }
        }

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandGetRmaDetailByRma { get; set; }
        public DelegateCommand CommandTransVerifyPass { get; set; }
        public DelegateCommand CommandTransVerifyNoPass { get; set; }
        public DelegateCommand CommandSetRmaRemark { get; set; }

        public void SetRmaRemark()
        {
            string id = RmaDto.RMANo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetRMARemark);
        }

        public void TransVerifyNoPass()
        {
            if (RmaList == null)
            {
                MessageBox.Show("请选择退货单", "提示");
                return;
            }
            List<RMADto> rmaSelectedList = RmaList.Where(e => e.IsSelected).ToList();
            if (rmaSelectedList.Count == 0)
            {
                MessageBox.Show("请选择退货单", "提示");
                return;
            }
            bool falg =
                AppEx.Container.GetInstance<IPackageService>()
                    .TransVerifyNoPass(rmaSelectedList.Select(e => e.RMANo).ToList());
            MessageBox.Show(falg ? "设置审核不通过成功" : "设置审核不通过失败", "提示");
            if (falg)
            {
                if (RmaList != null)
                    RmaList.Clear();
                if (RmaDetailLs != null)
                    RmaDetailLs.Clear();
                SearchRma();
            }
        }

        public void TransVerifyPass()
        {
            if (RmaList == null)
            {
                MessageBox.Show("请选择退货单", "提示");
                return;
            }
            List<RMADto> rmaSelectedList = RmaList.Where(e => e.IsSelected).ToList();
            if (rmaSelectedList.Count == 0)
            {
                MessageBox.Show("请选择退货单", "提示");
                return;
            }
            bool falg =
                AppEx.Container.GetInstance<IPackageService>()
                    .TransVerifyPass(rmaSelectedList.Select(e => e.RMANo).ToList());
            MessageBox.Show(falg ? "物流审核成功" : "物流审核失败", "提示");
            if (falg)
            {
                if (RmaList != null)
                    RmaList.Clear();
                if (RmaDetailLs != null)
                    RmaDetailLs.Clear();
                SearchRma();
            }

        }

        public void GetRmaDetailByRma()
        {
            if (rmaDto != null)
            {
                RmaDetailLs = AppEx.Container.GetInstance<IPackageService>().GetRmaDetailByRma(rmaDto.RMANo).ToList();
            }

            //if (RmaDto == null)
            //{
            //    MessageBox.Show("请选择退货单", "提示");
            //    return;
            //}
            //RmaDetailList = AppEx.Container.GetInstance<IPackageService>().GetRmaDetailByRma(RmaDto.RMANo).ToList();
        }

        public void SearchRma()
        {
            RmaList = AppEx.Container.GetInstance<IPackageService>().GetRmaByFilter(PackageReceiveDto).ToList();
        }
    }
}