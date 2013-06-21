using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class AddressController : RestfulController
    {
        private ICustomerRepository _customerRepo;
        private IShippingAddressRepository _shippingRepo;
        public AddressController(ICustomerRepository customerRepo,
            IShippingAddressRepository shippingRepo)
        {
            _customerRepo = customerRepo;
            _shippingRepo = shippingRepo;
        }
        /// <summary>
        /// return current user's shipping address, limit to first 10
        /// </summary>
        /// <param name="authUser"></param>
        /// <returns></returns>
        [RestfulAuthorize]
        public RestfulResult Detail(UserModel authUser)
        {
            var linq = _customerRepo.Context.Set<ShippingAddressEntity>()
                 .Where(s => s.Status != (int)DataStatus.Deleted && s.UserId == authUser.Id)
                 .OrderByDescending(s => s.UpdateDate)
                 .Take(10).ToList()
                 .Select(s => new SelfAddressResponse().FromEntity<SelfAddressResponse>(s));
            return new RestfulResult()
            {
                Data = new PagerInfoResponse<SelfAddressResponse>(null, linq.Count()) { Items = linq.ToList() }
            };

        }
        [RestfulAuthorize]
        public ActionResult Create(CreateAddressRequest request, UserModel authUser)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            var inserted = _shippingRepo.Insert(new ShippingAddressEntity() { 
                  ShippingAddress1 = request.ShippingAddress,
                   ShippingCity = request.ShippingCity,
                   ShippingCityId = request.ShippingCityId,
                    ShippingContactPerson = request.ShippingContactPerson,
                     ShippingContactPhone = request.ShippingContactPhone,
                      ShippingProvince =request.ShippingProvince,
                      ShippingProvinceId = request.ShippingProvinceId,
                       ShippingZipCode = request.ShippingZipCode,
                        Status = (int)DataStatus.Normal,
                         UpdateDate = DateTime.Now,
                          UpdateUser = authUser.Id,
                           UserId = authUser.Id
            });
            return new RestfulResult() { 
                 Data = new SelfAddressResponse().FromEntity<SelfAddressResponse>(inserted)
            };
        }

        [RestfulAuthorize]
        public ActionResult Delete(DeleteAddressRequest request, UserModel authUser)
        {
            var addressEntity = _shippingRepo.Get(s => s.Id == request.Id && s.UserId == authUser.Id).FirstOrDefault();
            if (addressEntity == null)
            {
                return this.RenderError(result => {
                    result.Message = "地址不存在！";
                });
            }
            _shippingRepo.Delete(addressEntity);

            return this.RenderSuccess(null);
        }
    }
}
