// ***********************************************************************
// Assembly         : 00_Intime.OPC.Domain
// Author           : Liuyh
// Created          : 04-01-2014 22:22:41
//
// Last Modified By : Liuyh
// Last Modified On : 04-01-2014 22:24:15
// ***********************************************************************
// <copyright file="UserDto.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;

namespace Intime.OPC.Domain.Dto
{
    /// <summary>
    /// Class UserDto.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDto"/> class.
        /// </summary>
        public UserDto()
        {
            SectionID = new List<int>();
            StoreIDs = new List<int>();
        }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the section i ds.
        /// </summary>
        /// <value>The section i ds.</value>
        public IList<int> SectionID { get; set; }
        /// <summary>
        /// Gets or sets the store i ds.
        /// </summary>
        /// <value>The store i ds.</value>
        public IList<int> StoreIDs { get; set; }
    }

    public class ChangePasswordDto
    {
        public int UserID { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}