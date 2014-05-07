using Intime.OPC.Modules.Dimension.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Services.Imp
{
    [Export(typeof(ICounterService))]
    public class CounterService : ICounterService
    {
        public void Create(Counter obj)
        {
            throw new NotImplementedException();
        }

        public void Update(Counter obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Counter Query(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Counter> Query(string name)
        {
            throw new NotImplementedException();
        }

        public IList<Counter> QueryAll()
        {
            throw new NotImplementedException();
        }

    }
}
