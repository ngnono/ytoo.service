﻿using Intime.OPC.Modules.Dimension.ViewModels;
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

namespace Intime.OPC.Modules.Dimension.Views
{
    /// <summary>
    /// Interaction logic for CounterView.xaml
    /// </summary>
    public partial class CounterView : UserControl
    {
        public CounterView()
        {
            InitializeComponent();
        }

        private void OnListBoxScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = (ScrollViewer)(((RoutedEventArgs)(e)).OriginalSource);
            if (scrollViewer.VerticalOffset != 0 && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                var viewModel = (CounterViewModel)DataContext;
                viewModel.LoadMoreBrandsCommand.Execute(null);

                scrollViewer.InvalidateVisual();
                scrollViewer.UpdateLayout();
            }
        }
    }
}
