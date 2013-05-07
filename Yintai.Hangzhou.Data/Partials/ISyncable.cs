using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yintai.Hangzhou.Data.Models
{
    public interface ISyncable
    {
        /// <summary>
        /// the type name to be synced:
        ///     this name should be equal to the {:type=>#{typename}}
        /// </summary>
        string TypeName { get;  }
        /// <summary>
        /// the entity primary key to be synced:
        ///     default is entity key 
        /// </summary>
        int SyncId { get;}

        /// <summary>
        /// the function to compose the sync message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        object Composing();
    }
}
