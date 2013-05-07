using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class ResourceEntity
    {
         public string AbsoluteUrl {
            get {
                if (string.IsNullOrEmpty(Domain) &&
                    string.IsNullOrEmpty(Name))
                    return string.Empty;
                return Path.Combine(Domain, Name);
            }
        }
      
    }
}
