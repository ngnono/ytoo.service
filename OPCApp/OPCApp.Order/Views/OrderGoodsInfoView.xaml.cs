// ***********************************************************************
// Assembly         : OPCApp.Order
// Author           : Liuyh
// Created          : 03-09-2014 22:15:01
//
// Last Modified By : Liuyh
// Last Modified On : 02-21-2014 22:50:48
// ***********************************************************************
// <copyright file="OrderGoodsInfoView.xaml.cs" company="liuyh">
//     Copyright (c) liuyh. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Windows.Controls;
using OPCApp.Order.ViewModels;

/// <summary>
/// The Views namespace.
/// </summary>

namespace OPCApp.Order.Views
{
    /// <summary>
    ///     OrderGoodsInfo.xaml 的交互逻辑
    /// </summary>
    //[Export("OrderGoodsInfo")]
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class OrderGoodsInfoView : UserControl
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderGoodsInfoView" /> class.
        /// </summary>
        public OrderGoodsInfoView()
        {
            InitializeComponent();
        }

        //[Import]
        /// <summary>
        ///     Sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public OrderGoodsInfoViewModel ViewModel
        {
            set { DataContext = value; }
        }
    }
}