﻿using System.Collections.Generic;
using OPCApp.TransManage.Models;

namespace OPCApp.TransManage.IService
{
    //门店接口定义
    public interface IInvoice
    {
        List<Invoice> SearchData(Invoice4Get invoice4get);
        ResultMsg SetFinish(int Type); //完成打印销售单，此接口只更新订单状态，可以考虑传递不同的参数，复用
    }

    public class StoreService : IInvoice
    {
        public List<Invoice> SearchData(Invoice4Get invoice4get)
        {
            var invoice = new List<Invoice>();
            return invoice;
        }

        public ResultMsg SetFinish(int type)
        {
            var msg = new ResultMsg();
            return msg;
        }
    }
}