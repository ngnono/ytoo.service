// ***********************************************************************
// Assembly         : 02_Intime.OPC.Service
// Author           : Liuyh
// Created          : 03-19-2014 20:11:42
//
// Last Modified By : Liuyh
// Last Modified On : 03-23-2014 12:43:21
// ***********************************************************************
// <copyright file="IRoleService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;

namespace Intime.OPC.Repository
{
    /// <summary>
    ///     Interface IRoleService
    /// </summary>
    public interface IRoleService : IService
    {
        /// <summary>
        ///     Creates the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Create(OPC_AuthRole role);

        /// <summary>
        ///     Updates the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Update(OPC_AuthRole role);

        /// <summary>
        ///     Deletes the specified role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Delete(int roleId);

        /// <summary>
        ///     Selects this instance.
        /// </summary>
        /// <returns>IList{OPC_AuthRole}.</returns>
        PageResult<OPC_AuthRole> Select(int pageIndex, int pageSize = 20);

        /// <summary>
        ///     Determines whether the specified role identifier is stop.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="bValid">if set to <c>true</c> [b valid].</param>
        /// <returns><c>true</c> if the specified role identifier is stop; otherwise, <c>false</c>.</returns>
        bool IsStop(int roleId, bool bValid);

        /// <summary>
        ///     Sets the menus.
        /// </summary>
        /// <param name="roleMenuDto">The role menu dto.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SetMenus(int roleId,int userID,IEnumerable<int> menuids );

        /// <summary>
        ///     获取用户的所有角色
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns>IEnumerable{OPC_AuthRole}.</returns>
        PageResult<OPC_AuthRole> GetByUserID(int userID, int pageIndex, int pageSize = 20);

        bool SetUsers(Domain.Dto.RoleUserDto roleUserDto);
    }
}