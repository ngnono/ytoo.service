﻿using System.ComponentModel.Composition;

namespace OPCApp.Customer.ViewModels
{
    [Export(typeof (CustomerReturnSearchRmaViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerReturnSearchRmaViewModel : CustomerReturnSearchViewModel
    {
    }
}