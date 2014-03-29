using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using OPCAPP.Domain.Enums;
using  OPCApp.Domain.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Infrastructure;

namespace OPCApp.TransManage.ViewModels
{
    [Export(typeof (RemarkViewModel))]
    public class RemarkViewModel : BindableBase
    {
        public string id;
        private OPC_SaleComment remark = new OPC_SaleComment();
        private IEnumerable<OPC_Comment> remark4list;
        public DelegateCommand CommandSave { get; set; } //保存
        public DelegateCommand CommandBack { get; set; } //返回

        //Grid数据集

        public IEnumerable<OPC_Comment> Remark4List
        {
            get { return remark4list; }
            set { SetProperty(ref remark4list, value); }
        }

        //保存数据库传递的值

        public OPC_SaleComment Remark
        {
            get { return remark; }
            set { SetProperty(ref remark, value); }
        }

        public void SaveRemark(string id, EnumSetRemarkType type)
        {
           
            var comment = new OPC_Comment();
            comment.RelationId = id;
            comment.Content = Remark.Content;
            comment.CreateUser = 1;
            comment.CreateDate = DateTime.Now;

            bool isSuccess = false;
            switch (type)
            {
                case EnumSetRemarkType.SetSaleRemark:
                    OPC_SaleComment salecomment = Mapper.Map<OPC_Comment, OPC_SaleComment>(comment);
                    isSuccess = AppEx.Container.GetInstance<IRemarkService>().WriteSaleRemark(salecomment);
                    break;
                case EnumSetRemarkType.SetSaleDetailRemark:
                    OPC_SaleDetailsComment saledetailscomment = Mapper.Map<OPC_Comment, OPC_SaleDetailsComment>(comment);
                    isSuccess = AppEx.Container.GetInstance<IRemarkService>().WriteSaleDetailsRemark(saledetailscomment);
                    break;
                case EnumSetRemarkType.SetOrderRemark:
                    OPC_OrderComment orderComment = Mapper.Map<OPC_Comment, OPC_OrderComment>(comment);
                    isSuccess = AppEx.Container.GetInstance<IRemarkService>().WriteOrderRemark(orderComment);
                    break;
            }
            if (!isSuccess)
            {
                MessageBox.Show("保存备注失败", "提示");
            }
        }

        public void OpenWinSearch(string id, EnumSetRemarkType type)
        {
            switch (type)
            {
                case EnumSetRemarkType.SetSaleRemark:
                    PageResult<OPC_SaleComment> saleRemark =
                        AppEx.Container.GetInstance<IRemarkService>().GetSaleRemark( string.Format("saleId={0}", id));
                    Remark4List = Mapper.Map<OPC_SaleComment, OPC_Comment>(saleRemark.Result);
                    break;
                case EnumSetRemarkType.SetSaleDetailRemark:
                    PageResult<OPC_SaleDetailsComment> saleDetailRemark =
                        AppEx.Container.GetInstance<IRemarkService>().GetSaleDetailsRemark( string.Format("saledetailId={0}", id));
                    Remark4List = Mapper.Map<OPC_SaleDetailsComment, OPC_Comment>(saleDetailRemark.Result);
                    ;
                    break;
                case EnumSetRemarkType.SetOrderRemark:
                    PageResult<OPC_SaleDetailsComment> orderRemark =
                        AppEx.Container.GetInstance<IRemarkService>().GetSaleDetailsRemark(string.Format("saledetailId={0}", id));
                    Remark4List = Mapper.Map<OPC_SaleDetailsComment, OPC_Comment>(orderRemark.Result);
                    ;
                    break;
            }
        }
    }
}