﻿using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class BrandService : BaseService<Brand>, IBrandService
    {
        public BrandService(IBrandRepository brandRepository) : base(brandRepository)
        {
        }

        public IList<Brand> GetAll()
        {
            return ((IBrandRepository)_repository).GetAll();
        }
    }
}