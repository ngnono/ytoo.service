using System.Collections;
using System.Collections.Generic;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.Financial
{

    public interface IFinancialPayVerify
    {
        //退货付款确认  
        bool ReturnGoodsPayVerify(string rmaNo,decimal money);

    }
}