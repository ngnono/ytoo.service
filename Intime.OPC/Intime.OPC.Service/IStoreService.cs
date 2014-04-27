using System.Collections.Generic;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IStoreService : IService
    {
        IList<Store> GetAll();
    }
}