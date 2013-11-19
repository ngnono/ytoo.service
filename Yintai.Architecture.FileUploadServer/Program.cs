using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using System.Configuration.Install;
using System.IO;
using System.Reflection;
using Yintai.Architecture.Common.Logger;

namespace Yintai.Architecture.FileUploadServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
           
               var  _host = new ServiceHostEx(typeof(ImageTool.Impl.ImageService),  LoggerManager.Current());
                _host.Open();
                Console.Read();
        }
    }
}
