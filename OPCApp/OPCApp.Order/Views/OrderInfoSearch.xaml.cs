// ***********************************************************************
// Assembly         : OPCApp.Order
// Author           : Liuyh
// Created          : 03-09-2014 22:15:01
//
// Last Modified By : Liuyh
// Last Modified On : 02-22-2014 00:05:30
// ***********************************************************************
// <copyright file="OrderInfoSearch.xaml.cs" >
//     Copyright (c) Liuyh. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OPCApp.Order.ViewModels;

/// <summary>
/// The Views namespace.
/// </summary>
namespace OPCApp.Order.Views
{
    //[Export("OrderInfoSearch2")]
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    /// <summary>
    /// OrderInfoSearch.xaml 的交互逻辑
    /// </summary>
    public partial class OrderInfoSearch : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderInfoSearch"/> class.
        /// </summary>
        public OrderInfoSearch()
        {
            InitializeComponent();
        }

        //[Import]
        /// <summary>
        /// Sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public OrderInfoSearchViewModel  ViewModel
        {
            set { this.DataContext = value; }
        }
    }


}
