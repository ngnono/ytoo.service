using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Intime.OPC.Domain.Dto;

namespace Intime.OPC.Service.Support
{
    public class EnumService:IEnumService
    {
        public IList<Item> All(string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Config\\" + fileName+".xml";

            using (FileStream fs=File.OpenRead(path))
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Item>));
                var lst = (List<Item>)xs.Deserialize(fs);
                return lst;
            }
        }

        public IList<Item> All(Type enumType)
        {
            IList<Item> lst=new List<Item>();
            var arr = Enum.GetValues(enumType);
            foreach (Enum s in arr)
            {
                var ii = new Item();
                ii.Key = s.AsID().ToString();
                ii.Value = s.GetDescription();
                lst.Add(ii);
            }
            return lst;
        }
    }
}
