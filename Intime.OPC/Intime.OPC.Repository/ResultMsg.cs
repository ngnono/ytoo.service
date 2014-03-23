using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Repository
{
   public  class ResultMsg
    {
     

       public bool IsSuccess { get;  set; }
       public string Msg { get;  set; }

       public object Data { get; set; }

       public static ResultMsg Success(string msg = "",object data=null)
       {
         return   new ResultMsg
         {
             IsSuccess = true,
             Data = data,
             Msg=msg
         };

       }

       public static ResultMsg Failure(string msg,object data=null)
       {
           return new ResultMsg
           {
               IsSuccess = false,
               Data =null,
               Msg = msg
           };
       }
    }


}
