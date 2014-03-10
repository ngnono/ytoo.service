using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using OPCApp.BaseInfoManage.Views;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.BaseInfoManage.IService;
using OPCApp.Domain.BaseInfo;

namespace OPCApp.BaseInfoManage.ViewModels
{
    class OrgManageViewModel : BindableBase
    {
        
        public DelegateCommand CommandSearch { get; set; }//查询
        public DelegateCommand CommandBack { get; set; }//返回
        public DelegateCommand CommandInputLSH { get; set; }//打印销售单
        public DelegateCommand CommandFinishPrintXSD { get; set; }//完成打印销售单
        public DelegateCommand CommandSaveDDBZ { get; set; }//保存订单备注
        public DelegateCommand CommandSaveXSDDBZ { get; set; }//保存销售订单备注


        //修改的时赋值的数据集
        private OrgInfo orginfo;
        public OrgInfo OrgInfo
        {
            get { return this.orginfo; }
            set { SetProperty(ref this.orginfo, value); }

        }
        

    }
}
