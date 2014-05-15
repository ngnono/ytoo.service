using Intime.OPC.Modules.Dimension.Framework;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Modules.Dimension.Properties;
using Intime.OPC.Modules.Dimension.Framework.Validation;

namespace Intime.OPC.Modules.Dimension.Models
{
    /// <summary>
    /// Dimension such as brand, counter, organization etc.
    /// </summary>
    public abstract class Dimension : ValidatableBindableBase
    {
        private bool isSelected;
        private string name;

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [Display(Name = "名字")]
        [LocalizedRequired]
        [LocalizedMaxLength(64)]
        public string Name 
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

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
