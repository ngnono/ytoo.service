using Intime.OPC.Modules.Dimension.Common;
using Intime.OPC.Modules.Dimension.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;

namespace Intime.OPC.Modules.Dimension.ViewModels
{
    [Export(typeof(CounterViewModel))]
    public class CounterViewModel : ModalDialogViewModel<Counter>
    {
        private IQueryCriteria _queryCriteria;
        private ObservableCollection<Brand> _brands;

        public CounterViewModel()
        {
            LoadBrandsCommand = new AsyncDelegateCommand<string>(OnBrandsLoad, OnException);
            LoadMoreBrandsCommand = new AsyncDelegateCommand(OnMoreBrandsLoad, OnException);
            SelectBrandCommand = new DelegateCommand<int?>(OnBrandSelect);

            Brands = new ObservableCollection<Brand>();
        }

        [Import]
        public IService<Brand> BrandService { get; set; }

        public ObservableCollection<Brand> Brands
        {
            get { return _brands; }
            set { SetProperty(ref _brands, value); }
        }

        public ICommand LoadBrandsCommand { get; set; }

        public ICommand SelectBrandCommand { get; set; }

        public ICommand LoadMoreBrandsCommand { get; set; }

        private void OnBrandSelect(int? brandID)
        {
            if (brandID == null) return;

            if (Model.Brands == null) Model.Brands = new List<Brand>();
            var brand = Brands.Where(b => b.ID == brandID.Value).FirstOrDefault();
            if (brand.IsSelected)
            {
                Model.Brands.Add(brand);
            }
            else
            {
                Model.Brands.Remove(b => b.ID == brand.ID);
            }
        }

        private void OnBrandsLoad(string brandName)
        {
            if (string.IsNullOrEmpty(brandName))
            {
                _queryCriteria = new QueryAll { PageIndex =1, PageSize=100};
            }
            else
            {
                _queryCriteria = new QueryByName { PageIndex = 1, PageSize = 100, Name = brandName };
            }

            var brands = new ObservableCollection<Brand>(BrandService.Query(_queryCriteria));
            FilterBrands(brands);
            Brands = new ObservableCollection<Brand>(brands);

            if (Model.Brands == null) Model.Brands = new List<Brand>();

            Model.Brands.ForEach(brand =>
            {
                brand.IsSelected = true;
                int index = 0;
                PerformActionOnUIThread(() => Brands.Insert(index++, brand));
            });
        }

        private void OnMoreBrandsLoad()
        {
            if (_queryCriteria == null) return;

            _queryCriteria.PageIndex++;
            var brands = BrandService.Query(_queryCriteria);
            FilterBrands(brands);
            AppendBrands(brands);
        }

        public void AppendBrands(IEnumerable<Brand> brands)
        {
            foreach (var brand in brands)
            {
                PerformActionOnUIThread(() => Brands.Add(brand));
            }
        }

        private void FilterBrands(IList<Brand> brands)
        {
            if (Model.Brands == null || Model.Brands.Count == 0) return;

            brands.ForEach(brand =>
            {
                if (Model.Brands.Contains(b => b.ID == brand.ID))
                {
                    brand.IsSelected = true;
                }
            });

            while (true)
            {
                var brand = brands.Where(b => b.IsSelected).FirstOrDefault();
                if (brand == null) break;

                brands.Remove(brand);
            }
        }
    }
}
