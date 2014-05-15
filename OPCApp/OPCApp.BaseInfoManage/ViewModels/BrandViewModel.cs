using Intime.OPC.Modules.Dimension.Framework;
using Intime.OPC.Modules.Dimension.Models;
using System.ComponentModel.Composition;

namespace Intime.OPC.Modules.Dimension.ViewModels
{
    [Export(typeof(BrandViewModel))]
    public class BrandViewModel : ModalDialogViewModel<Brand>
    {

    }
}
