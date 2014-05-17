using System.ComponentModel.Composition;
using Intime.OPC.Modules.Dimension.Common;
using OPCApp.Domain.Models;

namespace Intime.OPC.Modules.Dimension.ViewModels
{
    [Export(typeof(BrandViewModel))]
    public class BrandViewModel : ModalDialogViewModel<Brand>
    {

    }
}
