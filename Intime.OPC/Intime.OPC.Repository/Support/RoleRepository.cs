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

using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    ///     Class RoleRepository.
    /// </summary>
    public class RoleRepository : BaseRepository<OPC_AuthRole>, IRoleRepository
    {
        #region IRoleRepository Members

        /// <summary>
        ///     Sets the enable.
        /// </summary>
        /// <param name="roleID">The role identifier.</param>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetEnable(int roleID, bool enable)
        {
            using (var db = new YintaiHZhouContext())
            {
                OPC_AuthRole ent = db.OPC_AuthRole.FirstOrDefault(t => t.Id == roleID);
                if (ent != null)
                {
                    ent.IsValid = enable;
                    db.SaveChanges();

                    return true;
                }
                return false;
            }
        }

        public IList<OPC_AuthRole> All()
        {
            return Select(t => t.IsValid);
        }

        public IList<OPC_AuthRole> GetByUserID(int userID)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_AuthRoleUser.Where(t => t.OPC_AuthUserId == userID).Join(db.OPC_AuthRole,
                    t => t.OPC_AuthUserId, o => o.Id, (t, o) => o).ToList();
            }
        }

        #endregion
    }
}