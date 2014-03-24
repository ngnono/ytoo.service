using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.TransManage.Models;
using System.Windows;
using Intime.OPC.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.DataService.Interface.Trans;
using OPCApp.TransManage.IService;
using System.ComponentModel.Composition;

namespace OPCApp.TransManage.ViewModels
{
    [Export(typeof(RemarkViewModel))]
    public class RemarkViewModel : BindableBase
    {
        public string id;
        public DelegateCommand CommandSave { get; set; }//保存
        public DelegateCommand CommandBack { get; set; }//返回

        //Grid数据集
        private IEnumerable<OPC_Comment> remark4list;
        public IEnumerable<OPC_Comment> Remark4List
        {
            get { return this.remark4list; }
            set { SetProperty(ref this.remark4list, value); }
        }

        //保存数据库传递的值
        OPC_SaleComment remark = new OPC_SaleComment();
        public OPC_SaleComment Remark
        {
            get { return this.remark; }
            set { SetProperty(ref this.remark, value); }
        }

        public void SaveRemark(string id,int type)
        {
            OPC_Comment comment = new OPC_Comment();
            comment.RelationId = id;
            comment.Content = Remark.Content;
            comment.CreateUser = 1;
            comment.CreateDate = System.DateTime.Now;
            
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

            }
           
        }

        public void OpenWinSearch(string id,int type)
        {
            
            switch (type)
            {
                case 1:
                    string saleId = string.Format("saleId={0}", id);
                    var x = AppEx.Container.GetInstance<IRemarkService>();
                    PageResult<OPC_SaleComment> saleremark = AppEx.Container.GetInstance<IRemarkService>().GetSaleRemark(saleId);
                    Remark4List = Mapper.Map<OPC_SaleComment, OPC_Comment>(saleremark.Result);
                    break;
                case 2:
                    string saledetailId = string.Format("saledetailId={0}", id);
                    PageResult<OPC_SaleDetailsComment> saledetailremark = AppEx.Container.GetInstance<IRemarkService>().GetSaleDetailsRemark(saledetailId);
                    Remark4List = Mapper.Map<OPC_SaleDetailsComment, OPC_Comment>(saledetailremark.Result);;
                    break;

            }
            
            
            
        }
    }
}
