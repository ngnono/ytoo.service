using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Reporting.WinForms;
using OPCApp.Domain.Models;
using System;

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
        public void Print(string xsdName, string rdlcName, PrintModel dtList, bool isPrint=false)
        {
            try
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
                if (isPrint)
                {
                    PrintOnly(_reportViewer.LocalReport);
                }
                else
                {
                    _reportViewer.RefreshReport();
                    ShowDialog();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        public void PrintExpress(string rdlcName, PrintExpressModel printExpressModel, bool isPrint = false)
        {
            try
            { 
                var myRptDs = new ReportDataSource("PrintExpressModel",new List<PrintExpressModel>{ printExpressModel});
                _reportViewer.LocalReport.DataSources.Add(myRptDs);
                _reportViewer.LocalReport.ReportPath = rdlcName; //报表的地址
                if (isPrint)
                {
                    PrintOnly(_reportViewer.LocalReport);
                }
                else
                {
                    _reportViewer.RefreshReport();
                    ShowDialog();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        public void PrintFHD(string rdlcName,Order order, OPC_Sale opcSale,List<OPC_SaleDetail> listOpcSaleDetails, bool isPrint=false)
        {
            try
            {
                var myRptDs = new ReportDataSource("FHD", new List<OPC_Sale>() { opcSale });
                _reportViewer.LocalReport.DataSources.Add(myRptDs);

                myRptDs = new ReportDataSource("OrderDT", new List<Order>() { order });
                _reportViewer.LocalReport.DataSources.Add(myRptDs);

                myRptDs = new ReportDataSource("SaleDetailDT", listOpcSaleDetails);
                _reportViewer.LocalReport.DataSources.Add(myRptDs);

                _reportViewer.LocalReport.ReportPath = rdlcName; //报表的地址
                if (isPrint)
                {
                    PrintOnly(_reportViewer.LocalReport);
                }
                else
                {
                    _reportViewer.RefreshReport();
                    ShowDialog();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }


        private void PrintOnly(LocalReport report)
        {
            try
            { 
                report.Refresh();
                string deviceInfo =
                  "<DeviceInfo>" +
                  "  <OutputFormat>EMF</OutputFormat>" +
                  "  <PageWidth>8.5in</PageWidth>" +
                  "  <PageHeight>11in</PageHeight>" +
                  "  <MarginTop>0.25in</MarginTop>" +
                  "  <MarginLeft>0.25in</MarginLeft>" +
                  "  <MarginRight>0.25in</MarginRight>" +
                  "  <MarginBottom>0.25in</MarginBottom>" +
                  "</DeviceInfo>";
                Warning[] warnings;
                //将报表的内容按照deviceInfo指定的格式输出到CreateStream函数提供的Stream中。
                report.Render("Image", deviceInfo, CreateStream, out warnings);
                Print();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        //声明一个Stream对象的列表用来保存报表的输出数据
        //LocalReport对象的Render方法会将报表按页输出为多个Stream对象。
        private List<Stream> m_streams=new List<Stream>();
        //用来提供Stream对象的函数，用于LocalReport对象的Render方法的第三个参数。
        private Stream CreateStream(string name, string fileNameExtension,
          Encoding encoding, string mimeType, bool willSeek)
        {
            //如果需要将报表输出的数据保存为文件，请使用FileStream对象。
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        //用来记录当前打印到第几页了
         private int m_currentPageIndex;
          
         private void Print()
         {
             m_currentPageIndex = 0;
          
             if (m_streams == null || m_streams.Count == 0)
                 return;
             //声明PrintDocument对象用于数据的打印
             PrintDocument printDoc = new PrintDocument();
             //指定需要使用的打印机的名称，使用空字符串""来指定默认打印机
             printDoc.PrinterSettings.PrinterName = "";
             //判断指定的打印机是否可用
             if (!printDoc.PrinterSettings.IsValid)
             {
                 MessageBox.Show("没有发现打印机");
                 return;
             }
             //声明PrintDocument对象的PrintPage事件，具体的打印操作需要在这个事件中处理。
             printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
             //执行打印操作，Print方法将触发PrintPage事件。
             printDoc.Print();
         }
         private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            //Metafile对象用来保存EMF或WMF格式的图形，
            //我们在前面将报表的内容输出为EMF图形格式的数据流。
            m_streams[m_currentPageIndex].Position = 0;

            Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);

            //指定是否横向打印
            ev.PageSettings.Landscape = false;
            //这里的Graphics对象实际指向了打印机
            ev.Graphics.DrawImage(pageImage, 0, 0);
            m_streams[m_currentPageIndex].Close();
            m_currentPageIndex++;
            //设置是否需要继续打印
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }
    }
}