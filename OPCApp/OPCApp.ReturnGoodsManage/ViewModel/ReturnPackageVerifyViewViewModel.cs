
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
using OPCApp.Domain.Customer;
using OPCApp.Domain.Dto;
using OPCApp.DataService.Interface.Trans;

namespace OPCApp.ReturnGoodsManage.ViewModels
{
   [Export("ReturnPackageVerifyViewViewModel", typeof(ReturnPackageVerifyViewViewModel))]
    public class ReturnPackageVerifyViewViewModel : BindableBase
    {
        private PackageReceiveDto _packageReceiveDto;
       //与包裹审核公用传输类
        public PackageReceiveDto PackageReceiveDto
        {
            get { return _packageReceiveDto; }
            set { SetProperty(ref _packageReceiveDto, value); }
        }
     
     
       public RMADto rmaDto;

       public RMADto RmaDto
       {
           get { return rmaDto; }
           set { SetProperty(ref rmaDto, value); }
       }
       private List<RMADto> _rmaDtos;

       public List<RMADto> RamList
       {
           get { return _rmaDtos; }
           set { SetProperty(ref _rmaDtos, value); }
       }

       private List<RmaDetail> rmaDetails;

       public List<RmaDetail> RmaDetailList
       {
           get { return rmaDetails; }
           set { SetProperty(ref rmaDetails, value); }
       }
       public DelegateCommand CommandSearch { get; set; }
       public DelegateCommand CommandGetRmaDetailByRma { get; set; }
       public DelegateCommand CommandTransVerifyPass { get; set; }
       public DelegateCommand CommandTransVerifyNoPass { get; set; }
       public DelegateCommand CommandSetRmaRemark { get; set; }
       public ReturnPackageVerifyViewViewModel()
       {
           CommandSearch = new DelegateCommand(SearchRma);
           PackageReceiveDto = new PackageReceiveDto();
           CommandGetRmaDetailByRma = new DelegateCommand(GetRmaDetailByRma);
           CommandTransVerifyPass = new DelegateCommand(TransVerifyPass);
           CommandTransVerifyNoPass = new DelegateCommand(TransVerifyNoPass);
           CommandSetRmaRemark = new DelegateCommand(SetRmaRemark);
       }
       public void SetRmaRemark()
       {
           string id = RmaDto.RMANo;
           var remarkWin = AppEx.Container.GetInstance<IRemark>();
           remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetRMARemark);
       }
       public void TransVerifyNoPass()
       {
           if (RamList==null)
           {
               MessageBox.Show("请选择退货单", "提示");
               return;
           }
           var rmaSelectedList = RamList.Where(e => e.IsSelected).ToList();
           if (rmaSelectedList.Count==0)
           { 
               MessageBox.Show("请选择退货单", "提示");
               return;
           }
           var falg = AppEx.Container.GetInstance<IPackageService>().TransVerifyNoPass(rmaSelectedList.Select(e=>e.RMANo).ToList());
           MessageBox.Show(falg ? "物流审核成功" : "物流审核失败", "提示");
       }
       public void TransVerifyPass()
       {
           if (RamList == null)
           {
               MessageBox.Show("请选择退货单", "提示");
               return;
           }
           var rmaSelectedList = RamList.Where(e => e.IsSelected).ToList();
           if (rmaSelectedList.Count == 0)
           {
               MessageBox.Show("请选择退货单", "提示");
               return;
           }
           var falg = AppEx.Container.GetInstance<IPackageService>().TransVerifyPass(rmaSelectedList.Select(e => e.RMANo).ToList());
           MessageBox.Show(falg ? "设置审核不通过成功" : "设置审核不通过失败", "提示");
       }

       public void GetRmaDetailByRma()
       {
           if (RmaDto == null)
           {
               MessageBox.Show("请选择退货单", "提示");
               return;
           }
           RmaDetailList = AppEx.Container.GetInstance<IPackageService>().GetRmaDetailByRma(RmaDto.RMANo).ToList();
       }

       public void SearchRma()
       {
           RamList = AppEx.Container.GetInstance<IPackageService>().GetRmaByFilter(PackageReceiveDto).ToList();
       }

    

    }
}
