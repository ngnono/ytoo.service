using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Data.Models
{
    interface INotifyable
    {
        /// <summary>
        /// the entity primary key to be synced:
        ///     default is entity key 
        /// </summary>
        int NotifyId { get; }

        /// <summary>
        /// the function to notify
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void Notify(int id);
    }
}
