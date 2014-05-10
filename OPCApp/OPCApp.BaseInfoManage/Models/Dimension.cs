using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Models
{
    /// <summary>
    /// Dimension such as brand, counter, organization etc.
    /// </summary>
    public abstract class Dimension : BindableBase
    {
        private bool isSelected;

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否已选择
        /// </summary>
        public bool IsSelected 
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }
    }
}
