using System;
using Intime.OPC.Domain.Base;
using Intime.OPC.Repository;

namespace Intime.OPC.Service
{
    public class BaseService<T> : IService, ICanAdd<T>, ICanUpdate<T>,ICanDelete where T : class, IEntity
    {
        protected IRepository<T> _repository;
        public BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }

        public  bool Update(T t)
        {
            return _repository.Update(t);
        }

        public  bool Add(T t)
        {
            return _repository.Update(t);
        }

        public  bool DeleteById(int id)
        {
            return _repository.Delete(id);
        }

        public int UserId { get; set; }
    }
}
