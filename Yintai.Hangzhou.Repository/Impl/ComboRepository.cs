﻿using com.intime.fashion.common.message;
using com.intime.fashion.common.message.Messages;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class ComboRepository : EFRepository<IMS_ComboEntity>
    {
        public ComboRepository() : base(ServiceLocator.Current.Resolve<DbContext>()) { }
        public override void Update(IMS_ComboEntity entityToUpdate)
        {
            base.Update(entityToUpdate);
  
        }

        public override Yintai.Hangzhou.Data.Models.IMS_ComboEntity Insert(Yintai.Hangzhou.Data.Models.IMS_ComboEntity entity)
        {
            var newEntity = base.Insert(entity);

            return newEntity;
        }
    }
}
