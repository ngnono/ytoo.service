// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-19-2014 20:11:42
//
// Last Modified By : Liuyh
// Last Modified On : 03-21-2014 01:06:24
// ***********************************************************************
// <copyright file="RoleRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    /// Class RoleRepository.
    /// </summary>
    public class RoleRepository : BaseRespository<OPC_AuthRole>, IRoleRepository
    {

        /// <summary>
        /// Sets the enable.
        /// </summary>
        /// <param name="roleID">The role identifier.</param>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetEnable(int roleID, bool enable)
        {
            using (var db = new YintaiHZhouContext())
            {
                var ent = db.OPC_AuthRole.FirstOrDefault(t => t.Id == roleID);
                if (ent != null)
                {
                    ent.IsValid = enable;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    return true;
                }
                return false;
            }
        }
    }
}