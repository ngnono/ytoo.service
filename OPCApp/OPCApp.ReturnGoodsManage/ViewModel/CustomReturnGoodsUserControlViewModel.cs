using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.RMA;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Enums;
using OPCApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
  [Export(typeof(CustomReturnGoodsUserControlViewModel))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
   public class CustomReturnGoodsUserControlViewModel : BindableBase
    {
       
             private List<RMADto> _rmaDtos;            
             private List<RmaDetail> rmaDetails;

             public RMADto rmaDto;

             public CustomReturnGoodsUserControlViewModel()
             {
                 CommandGetRmaSaleDetailByRma = new DelegateCommand(GetRmaSaleDetailByRma);
                 CommandSetRmaRemark = new DelegateCommand(SetRmaRemark);
             }
             //退货单备注
             public void SetRmaRemark()
             {
                 string id = RmaDto.RMANo;
                 var remarkWin = AppEx.Container.GetInstance<IRemark>();
                 remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetRMARemark);
             }           
            
         /// <summary>
         /// 退货单
         /// </summary>
             public RMADto RmaDto
             {
                 get { return rmaDto; }
                 set { SetProperty(ref rmaDto, value); }
             }

         /// <summary>
         /// 退货单列表
         /// </summary>
             public List<RMADto> RmaList
             {
                 get { return _rmaDtos; }
                 set { SetProperty(ref _rmaDtos, value); }
             }

         /// <summary>
         /// 退货详单列表
         /// </summary>
             public List<RmaDetail> RmaDetailList
             {
                 get { return rmaDetails; }
                 set { SetProperty(ref rmaDetails, value); }
             }
          
             public DelegateCommand CommandGetRmaSaleDetailByRma { get; set; }
             public DelegateCommand CommandSetRmaRemark { get; set; }

             public void GetRmaSaleDetailByRma()
             {
                 if (rmaDto != null)
                 {
                     RmaDetailList = AppEx.Container.GetInstance<IPackageService>().GetRmaDetailByRma(rmaDto.RMANo).ToList();
                 }
             }                     

         }

    
}
