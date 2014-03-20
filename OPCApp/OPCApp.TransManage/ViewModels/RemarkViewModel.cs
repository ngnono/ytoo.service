using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.TransManage.Models;
using System.Windows;

namespace OPCApp.TransManage.ViewModels
{
    public class RemarkViewModel : BindableBase
    {
        public DelegateCommand CommandSave { get; set; }//保存
        public DelegateCommand CommandBack { get; set; }//返回
        Remark remark = new Remark();
        public Remark Remark
        {
            get { return this.remark; }
            set { SetProperty(ref this.remark, value); }
        }

        public void SaveRemark()
        {
            string x = Remark.Content;
        }
    }
}
