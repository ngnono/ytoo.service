﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.IService;
using OPCApp.Domain.Enums;
using OPCApp.TransManage.ViewModels;

namespace OPCApp.TransManage.Views
{
    /// <summary>
    ///     RemarkWin.xaml 的交互逻辑
    ///     封装的录入备注的接口
    /// </summary>
    [Export(typeof (IRemark))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RemarkWin : IRemark
    {
        private bool isCancel;

        [ImportingConstructor]
        public RemarkWin(RemarkViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            ViewModel.CommandSave = new DelegateCommand(CommandSaveExecute);
            ViewModel.CommandBack = new DelegateCommand(CommandBackExecute);
        }

        public RemarkViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as RemarkViewModel; }
        }

        public void ShowRemarkWin(string id, EnumSetRemarkType type)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ViewModel.OpenWinSearch(id, type);
            if (ShowDialog() == true)
            {
                //ViewModel.SaveRemark(id, type);
            }
        }

        public void CommandBackExecute()
        {
            DialogResult = false;
            isCancel = false;
            Close();
        }

        private void CommandSaveExecute()
        {
            if (String.IsNullOrEmpty(ViewModel.RemarkContent))
            {
                MessageBox.Show("请填写备注信息", "提示");
                isCancel = true;
            }
            else
            {
                ViewModel.SaveRemark();
                //DialogResult = true;
                //ViewModel.Remark.Content = "";
                isCancel = true;
            }

          //  Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = isCancel;
            base.OnClosing(e);
            isCancel = false;
        }
    }
}