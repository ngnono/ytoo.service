using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Data.Models
{
    public interface IAccountable
    {

        /// <summary>
        /// the account's user Id
        /// </summary>
        int AccountUserId { get; }

        /// <summary>
        /// the action to call triggering account syncing
        /// </summary>
        void AccountSyncing(int userId);
    }


}
