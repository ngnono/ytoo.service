﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Infrastructure;

namespace OPCApp.Main.ViewModels
{
    [Export(typeof(ConfigViewModel))]
    public class ConfigViewModel : BindableBase
    {
        private UserConfig modelConfig;
        public DelegateCommand OKCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public UserConfig Model
        {
            get { return modelConfig; }
            set { SetProperty(ref modelConfig, value); }
        }

        public ConfigViewModel()
        {
            Model=new UserConfig();
            ReadConfig();
            //     OKCommand = new DelegateCommand(SaveConfig);
        }

        public void ReadConfig()
        {
            Model.Password = ConfigurationManager.AppSettings["consumerSecret"];
           Model.UserKey = ConfigurationManager.AppSettings["consumerKey"];
           Model.ServiceUrl = ConfigurationManager.AppSettings["apiAddress"];
           Model.Version = ConfigurationManager.AppSettings["version"];  
        }

        public void WriteConfig()
        {
            try
            {
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["consumerKey"].Value = Model.UserKey;
                cfa.AppSettings.Settings["consumerSecret"].Value = Model.Password;
                cfa.AppSettings.Settings["apiAddress"].Value = Model.ServiceUrl;
                cfa.AppSettings.Settings["version"].Value = Model.Version;
                cfa.Save();
                AppEx.Config.Password = Model.Password;
                AppEx.Config.ServiceUrl = Model.ServiceUrl;
                AppEx.Config.UserKey = Model.UserKey;
                AppEx.Config.Version = Model.Version;
            }
            catch (Exception Ex)
            {
                throw Ex;    
            }

        }

    }

    public class UserConfig
    {
        public string UserKey { get; set; }
        public string Password { get; set; }
        public string ServiceUrl { get; set; }
        public string Version { get; set; }
    }
}
