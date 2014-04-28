﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Financial;
using OPCApp.DataService.Interface.RMA;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.Financial.ViewModels
{
    [Export("ReturnGoodsCompensateVerifyViewModel", typeof (ReturnGoodsCompensateVerifyViewModel))]
    public class ReturnGoodsCompensateVerifyViewModel : BindableBase
    {
        private PackageReceiveDto _packageReceiveDto;
        private List<RMADto> _rmaDtos;
        private List<RmaDetail> rmaDetails;
        //与包裹审核公用传输类


        public RMADto rmaDto;

        public ReturnGoodsCompensateVerifyViewModel()
        {
            CommandSearch = new DelegateCommand(SearchRma);
            PackageReceiveDto = new PackageReceiveDto();
            CommandGetRmaDetailByRma = new DelegateCommand(GetRmaDetailByRma);
            CommandFinancialVerifyPass = new DelegateCommand(FinancialVerifyPass);
            CommandFinancialVerifyNoPass = new DelegateCommand(FinancialVerifyNoPass);
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

        public List<RmaDetail> RmaDetailList
        {
            get { return rmaDetails; }
            set { SetProperty(ref rmaDetails, value); }
        }

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandGetRmaDetailByRma { get; set; }
        public DelegateCommand CommandFinancialVerifyPass { get; set; }
        public DelegateCommand CommandFinancialVerifyNoPass { get; set; }
        public DelegateCommand CommandSetRmaRemark { get; set; }

        public void SetRmaRemark()
        {
            string id = RmaDto.RMANo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetRMARemark);
        }

        public void FinancialVerifyNoPass()
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
                AppEx.Container.GetInstance<IFinancialPayVerify>()
                    .FinancialVerifyNoPass(rmaSelectedList.Select(e => e.RMANo).ToList());
            MessageBox.Show(falg ? "设置审核未通过成功" : "设置审核未通过失败", "提示");
            if (falg)
            {
                SearchRma();
            }
        }

        public void FinancialVerifyPass()
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
                AppEx.Container.GetInstance<IFinancialPayVerify>()
                    .FinancialVerifyPass(rmaSelectedList.Select(e => e.RMANo).ToList());
            MessageBox.Show(falg ? "设置审核通过成功" : "设置审核通过失败", "提示");
            if (falg)
            {
                SearchRma();
            }
        }

        public void GetRmaDetailByRma()
        {
            if (RmaDto == null)
            {
                MessageBox.Show("请选择退货单", "提示");
                return;
            }
            try
            {
                RmaDetailList = AppEx.Container.GetInstance<IPackageService>().GetRmaDetailByRma(RmaDto.RMANo).ToList();
            }
            catch { };
        }

        public void SearchRma()
        {
            if (RmaDetailList != null)
            {
                RmaDetailList.Clear();
            }
            try
            {
                RmaList = AppEx.Container.GetInstance<IFinancialPayVerify>().GetRmaByFilter(PackageReceiveDto).ToList();
            }
            catch { };
        }
    }
}