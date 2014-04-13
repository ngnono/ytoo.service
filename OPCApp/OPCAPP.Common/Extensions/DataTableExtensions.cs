using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Common.Extensions
{
    public static class DataTableExtensions
    {
        public static DataTable ToDataTable<TResult>(this IEnumerable<TResult> value) where TResult : class
        {
            //创建属性的集合
            List<PropertyInfo> pList = new List<PropertyInfo>();
            Type type = typeof(TResult);
            DataTable dt = new DataTable();
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            try
            {
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        //创建一个DataRow实例
                        DataRow row = dt.NewRow();
                        pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                        dt.Rows.Add(row);
                    }
                }
            }
            catch
            {

            }
            return dt;
        }
    }
}
