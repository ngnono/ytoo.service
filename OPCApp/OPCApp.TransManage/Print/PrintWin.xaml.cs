using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Reporting.WinForms;
using OPCApp.Domain.Models;

namespace OPCApp.TransManage.Print
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    [Export(typeof (IPrint))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PrintWin : Window, IPrint
    {
        public PrintWin()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     打印接口实现
        /// </summary>
        /// <param name="xsdName">创建的数据源名称(xsd文件的名称)</param>
        /// <param name="rdlcName">报表模板的路径</param>
        /// <param name="dataName">建立数据源时起的名字</param>
        /// <param name="dt">数据集dt</param>
        public void Print(string xsdName, string rdlcName, PrintModel dtList, bool isFast)
        {
            var myRptDS = new ReportDataSource();
            myRptDS = new ReportDataSource(xsdName, dtList.SaleDetailDT); //创建的数据源名称(xsd文件的名称),数据集
            myRptDS.Name = "SaleDetailDT";
            _reportViewer.LocalReport.DataSources.Add(myRptDS);


            myRptDS = new ReportDataSource(xsdName, dtList.SaleDT); //创建的数据源名称(xsd文件的名称),数据集
            myRptDS.Name = "SaleDT";
            _reportViewer.LocalReport.DataSources.Add(myRptDS);

            myRptDS = new ReportDataSource(xsdName, dtList.OrderDT); //创建的数据源名称(xsd文件的名称),数据集
            myRptDS.Name = "OrderDT";
            _reportViewer.LocalReport.DataSources.Add(myRptDS);

            _reportViewer.LocalReport.ReportPath = rdlcName; //报表的地址

            _reportViewer.RefreshReport();
            ShowDialog();
        }

        public void PrintExpress(string rdlcName, PrintExpressModel printExpressModel)
        {
            var myRptDs = new ReportDataSource("PrintExpressModel",new List<PrintExpressModel>{ printExpressModel});
            _reportViewer.LocalReport.DataSources.Add(myRptDs);
            _reportViewer.LocalReport.ReportPath = rdlcName; //报表的地址
            _reportViewer.RefreshReport();
            ShowDialog();
        }

        public void PrintFHD(string rdlcName,Order order, OPC_Sale opcSale,List<OPC_SaleDetail> listOpcSaleDetails )
        {
            var myRptDs = new ReportDataSource("FHD",new List<OPC_Sale>(){opcSale});
            _reportViewer.LocalReport.DataSources.Add(myRptDs);

            myRptDs = new ReportDataSource("OrderDT",new List<Order>(){ order});
            _reportViewer.LocalReport.DataSources.Add(myRptDs);

            myRptDs = new ReportDataSource("SaleDetailDT", listOpcSaleDetails);
            _reportViewer.LocalReport.DataSources.Add(myRptDs);

            _reportViewer.LocalReport.ReportPath = rdlcName; //报表的地址
            _reportViewer.RefreshReport();
            ShowDialog();
        }

    }
}