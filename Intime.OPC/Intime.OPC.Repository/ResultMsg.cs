using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Repository
{
   public  class ResultMsg
    {
       private static ResultMsg resultMsg=new ResultMsg();
       public bool IsSuccess { get; set; }
       public string Msg { get; set; }

       public object Data { get; set; }

       public static ResultMsg Success(string msg = "")
       {
           resultMsg.IsSuccess = true;
           resultMsg.Msg = msg;
           return resultMsg;
       }

       public static ResultMsg Failure(string msg = "")
       {
           resultMsg.IsSuccess = false;
           resultMsg.Msg = msg;
           return resultMsg;
       }
    }


}
