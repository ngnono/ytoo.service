using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;


namespace OPCApp.Infrastructure
{
    public class MefContainer : IContainer
    {
        CompositionContainer _container;
        public MefContainer(AggregateCatalog aggCatalog)
        {
            _container = new CompositionContainer(aggCatalog);

        }

        public MefContainer(CompositionContainer container)
        {
            _container = container;

        }
        public IEnumerable<T> GetInstances<T>()
        {
            return _container.GetExportedValues<T>();
        }

        public T GetInstance<T>()
        {
            return _container.GetExportedValueOrDefault<T>();
        }

        public T GetInstance<T>(string key)
        {
            return _container.GetExportedValue<T>(key);
        }

        public void Dispose()
        {
            if (_container!=null)
            _container.Dispose();
        }
    }
}
