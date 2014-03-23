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
        private IEnumerable<OPC_SaleComment> remark4list;
        public IEnumerable<OPC_SaleComment> Remark4List
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

        public void SaveRemark(int id)
        {
            OPC_SaleComment saleComment = new OPC_SaleComment();
            saleComment.SaleId = id;
            saleComment.Content = Remark.Content;
            saleComment.CreateUser = 1;
            saleComment.CreateDate = System.DateTime.Now;

            AppEx.Container.GetInstance<IRemarkService>().WriteRemark(saleComment);
           
        }

        public void OpenWinSearch(int id,int type)
        {
            string selectRemarkIdsAndType = string.Format("id={0}&type={1}", id, type);
            PageResult<OPC_SaleComment> re = AppEx.Container.GetInstance<IRemarkService>().SelectRemark(selectRemarkIdsAndType);
            Remark4List=re.Result;
        }
    }
}
