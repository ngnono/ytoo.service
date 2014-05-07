using Intime.OPC.Modules.Dimension.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Services.Imp
{
    [Export(typeof(IBrandService))]
    public class BrandService : IBrandService
    {
        public IEnumerable<Brand> Query(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Brand> QueryAll()
        {
            throw new NotImplementedException();
        }

        public void Create(Brand obj)
        {
            throw new NotImplementedException();
        }

        public void Update(Brand obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

    }
}
