using Intime.OPC.Infrastructure.Validation;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Domain.Models
{
    /// <summary>
    /// Dimension such as brand, counter, organization etc.
    /// </summary>
    public abstract class Dimension : ValidatableBindableBase
    {
        private bool _isSelected;
        private string _name;

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
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        /// <summary>
        /// 是否已选择
        /// </summary>
        public bool IsSelected 
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
    }
}
