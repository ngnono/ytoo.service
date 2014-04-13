using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCAPP.Domain.Dto.ReturnGoods
{
    //退货入收银   退货入库 打印退货单 导购退货收货查询 已完成退货查询
  public  class ReturnGoodsCommonSearchDto
    {
      public ReturnGoodsCommonSearchDto()
      {
          StartDate=DateTime.Now;
          EndDate = DateTime.Now;
      }

      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public string OrderNo { get; set; }
      public override string ToString()
      {
          return string.Format("StartDate={0}&EndDate={1}&OrderNo={2}&pageIndex={3}&pageSize={4}",StartDate,EndDate,OrderNo,1,300);
      }
    }
}
