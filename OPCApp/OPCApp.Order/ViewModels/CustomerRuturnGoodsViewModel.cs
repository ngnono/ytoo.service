using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Dto;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export("CustomerRuturnGoodsViewModel", typeof (CustomerRuturnGoodsViewModel))]
    public class CustomerRuturnGoodsViewModel : BindableBase
    {
    }
}
