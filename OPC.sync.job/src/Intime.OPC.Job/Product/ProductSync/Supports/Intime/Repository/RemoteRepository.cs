using System;
using System.Collections.Generic;
using System.Linq;

using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.O2O.ApiClient.Domain;
using Intime.O2O.ApiClient.Request;
using Intime.O2O.ApiClient.Response;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository.DTO;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository
{
    public class RemoteRepository : IRemoteRepository
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private string _dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private readonly IApiClient _apiClient;

        static RemoteRepository()
        {
            AutoMapper.Mapper.CreateMap<Section, SectionDto>();
            AutoMapper.Mapper.CreateMap<Brand, BrandDto>();
            AutoMapper.Mapper.CreateMap<ProductImage, ProductImageDto>();
            AutoMapper.Mapper.CreateMap<Store, StoreDto>();
            AutoMapper.Mapper.CreateMap<O2O.ApiClient.Domain.Product, ProductDto>();
        }

        public RemoteRepository()
        {
            _apiClient = new DefaultApiClient();
        }

        public RemoteRepository(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public SupplierDto GetSupplierById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SupplierDto> GetSupplierList(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public StoreDto GetStoreById(string id)
        {
            var result = _apiClient.Post(new GetStoreByIdRequest()
            {
                Data = new GetStoreByIdRequestData()
                {
                    StoreNo = id
                }
            });

            if (!result.Status)
            {
                Log.ErrorFormat("获取门店信息出错,message:{0}", result.Message);
                return null;
            }

            if (result.Data == null || string.IsNullOrEmpty(result.Data.StoreNo))
            {
                Log.ErrorFormat("获取门店信息为空,message:{0}", result.Message);
                return null;
            }

            return AutoMapper.Mapper.Map<Store, StoreDto>(result.Data);
        }

        public IEnumerable<StoreDto> GetStoreList(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public BrandDto GetBrandById(string id)
        {
            var result = _apiClient.Post(new GetBrandByIdRequest()
            {
                Data = new GetBrandByIdRequestData()
                {
                    BrandId = id
                }
            });

            if (!result.Status)
            {
                Log.ErrorFormat("获取品牌信息出错,message:[{0}]", result.Message);
                return null;
            }

            if (result.Data == null || string.IsNullOrEmpty(result.Data.BrandId))
            {
                Log.ErrorFormat("获取品牌信息为空,message:{0}", result.Message);
                return null;
            }

            return AutoMapper.Mapper.Map<Brand, BrandDto>(result.Data);
        }

        public IEnumerable<BrandDto> GetBrandList(int pageIndex, int pageSize, DateTime lastUpdateDateTime)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductDto> GetProductList(int pageIndex, int pageSize, DateTime lastUpdateDateTime)
        {
            var result = _apiClient.Post(new GetProductsRequest()
            {
                Data = new GetProductsRequestData()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    LastUpdate = lastUpdateDateTime.ToString(_dateTimeFormat)
                }
            });

            if (!result.Status)
            {
                Log.ErrorFormat("获取商品列表信息出错,message:{0}", result.Message);
                return null;
            }

            var products = AutoMapper.Mapper.Map<IEnumerable<O2O.ApiClient.Domain.Product>, IEnumerable<ProductDto>>(result.Data);
            return products.GroupBy(x => x.ProductId, x => x).Select(product => product.ToList().OrderByDescending(x => x.WriteTime).FirstOrDefault());
            
        }

        public SectionDto GetSectionById(string sectionid, string storeNo)
        {
            var result = _apiClient.Post(new GetSectionByIdRequest()
            {
                Data = new GetSectionByIdRequestData()
                {
                    SectionId = sectionid,
                    StoreNo = storeNo
                }
            });

            if (!result.Status)
            {
                Log.ErrorFormat("获取门店信息出错,message:{0}", result.Message);
                return null;
            }

            if (result.Data == null || string.IsNullOrEmpty(result.Data.CounterId))
            {
                Log.ErrorFormat("获取门店信息为空,message:{0}", result.Message);
                return null;
            }

            return AutoMapper.Mapper.Map<Section, SectionDto>(result.Data);
        }

        public IEnumerable<ProductImageDto> GetProudctImages(int pageIndex, int pageSize, DateTime lastUpdateDateTime)
        {
            var result = _apiClient.Post(new GetProductImagesRequest()
            {
                Data = new GetProductImagesRequestData()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    LastUpdate = lastUpdateDateTime.ToString(_dateTimeFormat)
                }
            });

            if (!result.Status)
            {
                Log.ErrorFormat("获取商品图片列表信息出错,message:{0}", result.Message);
                return null;
            }

//            return AutoMapper.Mapper.Map<IEnumerable<ProductImage>, IEnumerable<ProductImageDto>>(result.Data);
            return AutoMapper.Mapper.Map<IEnumerable<ProductImage>, IEnumerable<ProductImageDto>>(result.Data).GroupBy(x => x.GroupByKey, x => x).Select(x => x.ToList().OrderByDescending(y => y.WriteTime).FirstOrDefault());

        }
    }
}
