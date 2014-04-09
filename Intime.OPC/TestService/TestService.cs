using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Intime.OPC.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestService
{
    [TestClass]
    public class TestService<T>
    {

        protected T Service
        {
            get { return GetInstance<T>(); }

        }

        private  void RegisterMefDependencyResolver()
        {
            /**----------------------------------------
             * -- 说明：程序根据规则自动的导出部件 
             * -----------------------------------------
             * 1. 所有的继承IHttpController的类
             * 2. 命名空间中包含.Support的所有类
             * ----------------------------------------
             */
            var conventions = new ConventionBuilder();

            var lstAssemlby = new List<Assembly>();

            
            lstAssemlby.Add(Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory+ "\\Intime.OPC.Repository.dll"));
            lstAssemlby.Add(Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "\\Intime.OPC.Service.dll"));
            lstAssemlby.Add(Assembly.GetExecutingAssembly());
           
            // Export namespace {*.Support.*}
            conventions.ForTypesMatching(t => t.Namespace != null &&
                                              (t.Namespace.EndsWith(".Support") || t.Namespace.Contains(".Support.")))
                .Export()
                .ExportInterfaces();
                 container = new ContainerConfiguration()
                .WithAssemblies(lstAssemlby.Cast<Assembly>(), conventions)
                .CreateContainer();
      
   
            // 设置WebApi的DependencyResolver
           // GlobalConfiguration.Configuration.DependencyResolver = new MefDependencyResolver(container);
     
        }

        private CompositionHost container;

        protected T GetInstance<T>()
        {
            if (container==null)
            {
                RegisterMefDependencyResolver();
                MapConfig.Config();
            }
            return container.GetExport<T>();

        }

        protected void AssertList<T1>(IList<T1> lst)
        {
            Assert.IsNotNull(lst);
            Assert.AreNotEqual(0, lst.Count);
        }
    }
}