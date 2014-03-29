using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
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

        public void SaveRemark(string id, int type)
        {
           
            var comment = new OPC_Comment();
            comment.RelationId = id;
            comment.Content = Remark.Content;
            comment.CreateUser = 1;
            comment.CreateDate = DateTime.Now;

            bool isSuccess = false;
            switch (type)
            {
                case 1:
                    OPC_SaleComment salecomment = Mapper.Map<OPC_Comment, OPC_SaleComment>(comment);
                    isSuccess = AppEx.Container.GetInstance<IRemarkService>().WriteSaleRemark(salecomment);
                    break;
                case 2:
                    OPC_SaleDetailsComment saledetailscomment = Mapper.Map<OPC_Comment, OPC_SaleDetailsComment>(comment);
                    isSuccess = AppEx.Container.GetInstance<IRemarkService>().WriteSaleDetailsRemark(saledetailscomment);
                    break;
                case 3:
                    OPC_OrderComment orderComment = Mapper.Map<OPC_Comment, OPC_OrderComment>(comment);
                    isSuccess = AppEx.Container.GetInstance<IRemarkService>().WriteOrderRemark(orderComment);
                    break;
            }
        }

        public void OpenWinSearch(string id, int type)
        {
            switch (type)
            {
                case 1:
                    string saleId = string.Format("saleId={0}", id);
                    var x = AppEx.Container.GetInstance<IRemarkService>();
                    PageResult<OPC_SaleComment> saleremark =
                        AppEx.Container.GetInstance<IRemarkService>().GetSaleRemark(saleId);
                    Remark4List = Mapper.Map<OPC_SaleComment, OPC_Comment>(saleremark.Result);
                    break;
                case 2:
                    string saledetailId = string.Format("saledetailId={0}", id);
                    PageResult<OPC_SaleDetailsComment> saledetailremark =
                        AppEx.Container.GetInstance<IRemarkService>().GetSaleDetailsRemark(saledetailId);
                    Remark4List = Mapper.Map<OPC_SaleDetailsComment, OPC_Comment>(saledetailremark.Result);
                    ;
                    break;
            }
        }
    }
}