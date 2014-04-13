﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Reporting.WinForms;
using System.Collections;
using System.Reflection;
using OPCApp.Common.Extensions;


namespace OPCApp.TransManage.Print
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(IPrint))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class PrintWin : Window,IPrint
    {
        public PrintWin()
        {    
            InitializeComponent();
        }
        /// <summary>
        /// 打印接口实现
        /// </summary>
        /// <param name="xsdName">创建的数据源名称(xsd文件的名称)</param>
        /// <param name="rdlcName">报表模板的路径</param>
        /// <param name="dataName">建立数据源时起的名字</param>
        /// <param name="dt">数据集dt</param>
        public void Print(string xsdName, string rdlcName, PrintModel dtList, bool isFast)
        {
            ReportDataSource myRptDS = new ReportDataSource();
            DataTable dt = new DataTable();
            myRptDS = new ReportDataSource(xsdName, dtList.SaleDetailDT);//创建的数据源名称(xsd文件的名称),数据集
            myRptDS.Name = "SaleDetailDT";
            _reportViewer.LocalReport.DataSources.Add(myRptDS);


            myRptDS = new ReportDataSource(xsdName, dtList.SaleDT);//创建的数据源名称(xsd文件的名称),数据集
            myRptDS.Name = "SaleDT";
            _reportViewer.LocalReport.DataSources.Add(myRptDS);

            myRptDS = new ReportDataSource(xsdName, dtList.OrderDT);//创建的数据源名称(xsd文件的名称),数据集
            myRptDS.Name = "OrderDT";
            _reportViewer.LocalReport.DataSources.Add(myRptDS);

            _reportViewer.LocalReport.ReportPath = rdlcName;//报表的地址
            
            _reportViewer.RefreshReport();
            this.ShowDialog();
            
            
        }

        

        
        

        

    }
}