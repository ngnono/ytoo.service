using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Service.Logic
{
  public static  class RMARule
    {
      public static bool CanVoid(int status)
      {
          var voidStatus = new int[] { (int)RMAStatus.Created, (int)RMAStatus.PassConfirmed};
          return voidStatus.Any(s => s == status);
      }
    }
}
