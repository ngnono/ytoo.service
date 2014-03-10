using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Runtime.Serialization.Json;
using OPCApp.BaseInfoManage.Views;
using Newtonsoft.Json;
using Microsoft.Practices.Prism.Mvvm;
using System.Net.Http;
using OPCApp.Common;
using OPCApp.BaseInfoManage.IService;
using OPCApp.Domain.BaseInfo;

namespace OPCApp.BaseInfoManage.ViewModels
{
    public class StoreManageViewModel : BindableBase
    {
        IStoreService IStore = new StoreService();
        public StoreManage4ModifyViewModel orgmodel = new StoreManage4ModifyViewModel();
        public DelegateCommand CommandSearch { get; set; }//查询
        public DelegateCommand CommandBack { get; set; }//返回
        public DelegateCommand CommandAdd { get; set; }//增加
        public DelegateCommand CommandModify { get; set; }//完成打印销售单
        public DelegateCommand CommandSaveDDBZ { get; set; }//保存订单备注
        public DelegateCommand CommandSaveXSDDBZ { get; set; }//保存销售订单备注

        //获取传递查询条件的值
        private Store4Get store4get;
        public Store4Get Store4Get
        {
            get { return this.store4get; }
            set { SetProperty(ref this.store4get, value); }

        }

        //修改的时赋值的数据集
        private StoreInfo editinfo;
        public StoreInfo EditInfo
        {
            get { return this.editinfo; }
            set { SetProperty(ref this.editinfo, value); }
           
        }

        //Grid数据集
        private List<StoreInfo> storeinfo;
        public List<StoreInfo> StoreInfo
        {

            get { return this.storeinfo; }
            set { SetProperty(ref this.storeinfo, value); }
        }

        //新增实现
        private void CommandAddExecute()
        {
            StoreInfo EditInfo = new StoreInfo();
            showAddWin(EditInfo);
        }

        //查询实现
        private void CommandSearchExecute()
        {
            IStore.SearchData(Store4Get);
        }

        
        //修改实现
        public void CommandModifyExecute()
        {
            showAddWin(EditInfo);
            
        }

        //单条删除实现
        public void CommandDeleteExecute()
        {
            string ID = EditInfo.Id;
            IStore.DelData(ID);
        }
        public void showAddWin(StoreInfo EditInfo)
        {
            StoreManage4Modify win = new OPCApp.BaseInfoManage.Views.StoreManage4Modify();
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.sv.SaveInfo = EditInfo;
            if (win.ShowDialog()==true)
            {
                ResultMsg backList = IStore.SaveData(win.sv.SaveInfo);
                if (backList.IsSuccess == true)
                {
                    MessageBox.Show("保存成功");
                    SearchData();
                }
                else if (backList.IsSuccess == false)
                {
                    MessageBox.Show(backList.Msg);
                }
                
            }
        }
        public StoreManageViewModel()
        {
            //初始化命令属性
            CommandSearch = new DelegateCommand(new Action(CommandSearchExecute));
            CommandBack = new DelegateCommand(new Action(CommandSearchExecute));

            CommandAdd = new DelegateCommand(new Action(CommandAddExecute));
            CommandModify = new DelegateCommand(new Action(CommandModifyExecute));
            CommandSaveXSDDBZ = new DelegateCommand(new Action(CommandSearchExecute));
        }

        
        public void SearchData()
        {
            //
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync("http://111.207.166.196:8086/api/user").Result;

                
                if (response.IsSuccessStatusCode)
                {
                    StoreInfo = response.Content.ReadAsAsync<List<StoreInfo>>().Result;

                }
            }
            catch
            {

            }
            /*
            string jsonString = "[{\"Id\":28,\"Name\":\"张三\"},{\"Id\":29,\"Name\":\"张4\"},{\"Id\":33,\"Name\":\"张5\"}]";

            
            StoreInfo =JsonExtension.FromJson_<List<StoreInfo>>(jsonString);
            */
            
        }
        
    }

    //弹出的窗口相关
    public class StoreManage4ModifyViewModel : BindableBase
    {
        public DelegateCommand CommandSave { get; set; }//保存
        public DelegateCommand CommandBack { get; set; }//返回
        StoreInfo saveinfo = new StoreInfo();
        public StoreInfo SaveInfo
        {
            get { return this.saveinfo; }
            set { SetProperty(ref this.saveinfo, value); }
        }
    }
}
