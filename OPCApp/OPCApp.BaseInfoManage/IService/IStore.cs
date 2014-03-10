using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.BaseInfoManage.Models;

namespace OPCApp.BaseInfoManage.IService
{
    //门店接口定义
    public interface IStoreService
    {
        List<StoreInfo> SearchData(Store4Get store4get);
        ResultMsg SaveData(StoreInfo storeinfo);//1保存成功，2保存失败，3验证重复返回
        ResultMsg DelData(string ID);//1删除成功，2删除失败，3删除验证？？
        ResultMsg SetEnable(int Type);//1为启用，0为禁用
    }
    public class StoreService : IStoreService
    {
        public List<StoreInfo> SearchData(Store4Get store4get)
        {
            List< StoreInfo> store = new List<StoreInfo>();
            return store;
        }
        public ResultMsg SaveData(StoreInfo storeinfo)
        {
            ResultMsg msg = new ResultMsg();
            return msg;
        }
        public ResultMsg DelData(string ID)
        {
            ResultMsg msg = new ResultMsg();
            return msg;
        }
        public ResultMsg SetEnable(int type)
        {
            ResultMsg msg = new ResultMsg();
            return msg;
        }
       
    }
}
