﻿using Intime.OPC.Modules.Dimension.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Services.Imp
{
    [Export(typeof(IBrandService))]
    public class MockBrandService : IBrandService
    {
        private IList<Brand> brands = new List<Brand>();

        public MockBrandService()
        {
            BuildMockData();
        }

        private void BuildMockData()
        {
            for (int i = 1; i < 100; i++)
            {
                brands.Add(new Brand
                {
                    ID = i,
                    Name = string.Format("Name {0}", i),
                    EnglishName = string.Format("English Name {0}", i),
                    Enabled = (i % 2) == 0,
                    Supplier = new Supplier() { ID = i, Name = string.Format("Supplier Name {0}", i) },
                    Description = string.Format("Description {0}", i)
                });
            }
        }

        public Brand Create(Models.Brand obj)
        {
            obj.ID = 3000;
            return obj;
        }

        public void Update(Models.Brand obj)
        {

        }

        public void Delete(int id)
        {
            brands.Remove(brand => brand.ID == id);
        }

        public Models.Brand Query(int id)
        {
            return brands.Where(brand => brand.ID == id).FirstOrDefault();
        }

        public IList<Models.Brand> Query(string name)
        {
            return brands.Where(brand => brand.Name.Contains(name)).ToList();
        }

        public IList<Models.Brand> QueryAll()
        {
            return brands;
        }
    }
}
