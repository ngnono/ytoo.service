﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.common.config
{
   public  class RabbitClientConfiguration: CommonConfigurationBase
    {
        private static RabbitClientConfiguration instance = new RabbitClientConfiguration();
        private RabbitClientConfiguration() { }
        public static RabbitClientConfiguration Current
        {
            get
            {
                return instance;
            }
        }

        public string Host { get { return GetItem("host"); } }
        public string QueueName { get { return GetItem("queue"); } }

        protected override string SectionName
        {
            get
            {
                return "rabbit";
            }
        }
    }
}
