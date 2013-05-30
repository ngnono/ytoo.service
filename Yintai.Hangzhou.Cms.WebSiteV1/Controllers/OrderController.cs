using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    [AdminAuthorize]
    public class OrderController:UserController
    {
        private IOrderRepository _orderRepo;
        private IUserAuthRepository _userAuthRepo;
        private ICustomerRepository _customerRepo;
        private IOrderLogRepository _orderLogRepo;
        private IOrderItemRepository _orderItemRepo;
        private IInboundPackRepository _inboundRepo;
        public OrderController(IOrderRepository orderRepo,
            IUserAuthRepository userAuthRepo,
            ICustomerRepository customerRepo,
            IOrderLogRepository orderLogRepo,
            IOrderItemRepository orderItemRepo,
            IInboundPackRepository inboundRepo)
        {
            _orderRepo = orderRepo;
            _userAuthRepo = userAuthRepo;
            _customerRepo = customerRepo;
            _orderLogRepo = orderLogRepo;
            _orderItemRepo = orderItemRepo;
            _inboundRepo = inboundRepo;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(OrderSearchOption search, PagerRequest request)
        {
            int totalCount;
            search.CurrentUser = CurrentUser.CustomerId;
            search.CurrentUserRole = CurrentUser.Role;
            var dbContext = _orderRepo.Context;
            var linq = dbContext.Set<OrderEntity>().Where(p => (string.IsNullOrEmpty(search.OrderNo) || p.OrderNo == search.OrderNo) &&
                (!search.CustomerId.HasValue || p.CustomerId == search.CustomerId.Value) &&
                (!search.Status.HasValue || p.Status == (int)search.Status.Value) &&
                     (!search.Store.HasValue || p.StoreId == search.Store.Value) &&
                      (!search.Brand.HasValue || p.BrandId == search.Brand.Value) &&
                      (!search.FromDate.HasValue || p.CreateDate>=search.FromDate.Value) &&
                      (!search.ToDate.HasValue || p.CreateDate<=search.ToDate.Value) &&
                p.Status !=(int)DataStatus.Deleted);
            linq = _userAuthRepo.AuthFilter(linq, search.CurrentUser, search.CurrentUserRole) as IQueryable<OrderEntity>;

            var linq2 = linq.Join(dbContext.Set<UserEntity>().Where(u => u.Status != (int)DataStatus.Deleted),
                    o => o.CustomerId,
                    i => i.Id,
                    (o, i) => new { O = o,C =i})
                .GroupJoin(dbContext.Set<ShipViaEntity>().Where(s => s.Status != (int)DataStatus.Deleted),
                    o => o.O.ShippingVia,
                    i => i.Id,
                    (o, i) => new { O = o.O,C =o.C, S = i.FirstOrDefault() }).OrderByDescending(l => l.O.CreateDate);
            
           
            totalCount = linq2.Count();

            var skipCount = (request.PageIndex - 1) * request.PageSize;

            var linq3 = skipCount == 0 ? linq2.Take(request.PageSize) : linq2.Skip(skipCount).Take(request.PageSize);
            

            var vo = from l in linq3.ToList()
                     select new OrderViewModel().FromEntity<OrderViewModel>(l.O, p =>
                     {
                         p.ShippingViaMethod = l.S;
                         p.Customer = new CustomerViewModel().FromEntity<CustomerViewModel>(l.C);
                     });

            var v = new Pager<OrderViewModel>(request, totalCount) { Data = vo.ToList() };
            ViewBag.SearchOptions = search;
            return View(v);
        }

        public ActionResult Details(string orderNo)
        {
            var order = _orderRepo.Get(o => o.OrderNo == orderNo).FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentNullException("orderNo");
            }
            var model = new OrderViewModel().FromEntity<OrderViewModel>(order, o => {
                o.Customer = new CustomerViewModel().FromEntity<CustomerViewModel>(_customerRepo.Find(o.CustomerId));
                o.Logs = _orderLogRepo.Get(l => l.OrderNo == o.OrderNo).Select(l => new OrderLogViewModel().FromEntity<OrderLogViewModel>(l));
                o.Items =_orderRepo.Context.Set<OrderItemEntity>().Where(oi=>oi.OrderNo==o.OrderNo && oi.Status !=(int)DataStatus.Deleted)
                        .Select(oi=>new OrderItemViewModel().FromEntity<OrderItemViewModel>(oi));
                o.ShippingViaMethod = _orderRepo.Context.Set<ShipViaEntity>().Where(s => s.Id == o.ShippingVia).FirstOrDefault();
                o.StoreName = _orderRepo.Context.Set<StoreEntity>().Where(s => s.Id == o.StoreId).FirstOrDefault().Name;
               
            });
            return View(model);
        }
        public ActionResult ChangeStoreItem(string orderNo)
        {
            var order = _orderRepo.Context.Set<OrderItemEntity>().Where(o=>o.OrderNo==orderNo)
                .GroupJoin(_orderRepo.Context.Set<ResourceEntity>().Where(r=>r.Status!=(int)DataStatus.Deleted && r.SourceType==(int)SourceType.Product),
                    o=>o.ProductId,
                    i=>i.SourceId,
                    (o,i)=>new {O=o,R=i.FirstOrDefault()});
            var model = order.ToList().Select(o => new OrderItemViewModel().FromEntity<OrderItemViewModel>(o.O, p=>{
                p.ProductResource =new ResourceViewModel().FromEntity<ResourceViewModel>(o.R);
            }));
            ViewBag.OrderNo = orderNo;
            return View(model);
        }
        [HttpPost]
        public ActionResult ChangeStoreItem(IEnumerable<OrderItemViewModel> items,string orderNo)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in items)
                {
                    item.ProductResource = new ResourceViewModel().FromEntity<ResourceViewModel>(_orderRepo.Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.SourceId == item.ProductId).FirstOrDefault());
                }
                ViewBag.OrderNo = orderNo;
                return View(items);
            }
            using (var ts = new TransactionScope())
            {
                foreach (var item in items)
                {
                    var itemEntity = _orderItemRepo.Find(item.Id);
                    itemEntity.StoreItemNo = item.StoreItemNo;
                    itemEntity.StoreItemDesc = item.StoreItemDesc;
                    itemEntity.UpdateDate = DateTime.Now;
                    itemEntity.UpdateUser = CurrentUser.CustomerId;
                    _orderItemRepo.Update(itemEntity);
                }
                var orderEntity = _orderRepo.Find(orderNo);
                orderEntity.Status = (int)OrderStatus.AgentConfirmed;
                orderEntity.UpdateDate = DateTime.Now;
                orderEntity.UpdateUser = CurrentUser.CustomerId;
                _orderRepo.Update(orderEntity);
                _orderLogRepo.Insert(new OrderLogEntity() { 
                     OrderNo = orderNo,
                      CreateDate = DateTime.Now,
                      CreateUser = CurrentUser.CustomerId,
                       CustomerId = CurrentUser.CustomerId,
                        Operation = "专柜确认商品编码。",
                        Type=(int)OrderOpera.FromOperator
                });
                ts.Complete();
            }
            return View("Details", new {OrderNo = orderNo });
        }
        public ActionResult Shipping(string orderNo)
        {
            var order = _orderRepo.Context.Set<OrderItemEntity>().Where(o => o.OrderNo == orderNo).FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentNullException("orderNo");
            }
            var model = new OrderViewModel().FromEntity<OrderViewModel>(order);
            ViewBag.OrderNo = orderNo;
            return View(model);
        }
        [HttpPost]
        public ActionResult Shipping(OrderViewModel item, string orderNo)
        {
            item.Status = (int)OrderStatus.Shipped;
            if (!ModelState.IsValid)
            {
                var order = _orderRepo.Context.Set<OrderItemEntity>().Where(o => o.OrderNo == orderNo).FirstOrDefault();
                ViewBag.OrderNo = orderNo;
                return View(new OrderViewModel().FromEntity<OrderViewModel>(order));
            }
            using (var ts = new TransactionScope())
            {

                var itemEntity = _orderRepo.Get(o => o.OrderNo == orderNo).First();
                itemEntity.ShippingVia = item.ShippingVia;
                itemEntity.ShippingNo = item.ShippingNo;
                itemEntity.Status = (int)OrderStatus.Shipped;
                itemEntity.UpdateDate = DateTime.Now;
                itemEntity.UpdateUser = CurrentUser.CustomerId;
                _orderRepo.Update(itemEntity);

                _orderLogRepo.Insert(new OrderLogEntity()
                {
                    OrderNo = orderNo,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser.CustomerId,
                    CustomerId = CurrentUser.CustomerId,
                    Operation = "填写快递信息并发货。",
                    Type = (int)OrderOpera.FromOperator
                });
                ts.Complete();
            }
            return View("Details", new { OrderNo = orderNo });
        }

        public ActionResult Reject(string orderNo)
        {
            var order = _orderRepo.Context.Set<OrderItemEntity>().Where(o => o.OrderNo == orderNo).FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentNullException("orderNo");
            }
            ViewBag.OrderNo = orderNo;
            return View(new InboundPackViewModel());
        }
        [HttpPost]
        public ActionResult Reject(InboundPackViewModel item, string orderNo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.OrderNo = orderNo;
                return View(item);
            }
            using (var ts = new TransactionScope())
            {
                //step1：update order status

                var itemEntity = _orderRepo.Get(o => o.OrderNo == orderNo).First();
                itemEntity.Status = (int)OrderStatus.CustomerConfirmed;
                itemEntity.UpdateDate = DateTime.Now;
                itemEntity.UpdateUser = CurrentUser.CustomerId;
                _orderRepo.Update(itemEntity);
                //step2: persist inbound package
                _inboundRepo.Insert(new InboundPackageEntity() { 
                     CreateDate = DateTime.Now,
                     CreateUser = CurrentUser.CustomerId,
                      ShippingNo = item.ShippingNo,
                       ShippingVia = item.ShippingVia,
                        SourceNo = orderNo,
                        SourceType = (int)InboundType.CustomerReject
                       
                });

                //step3: log it
                _orderLogRepo.Insert(new OrderLogEntity()
                {
                    OrderNo = orderNo,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser.CustomerId,
                    CustomerId = CurrentUser.CustomerId,
                    Operation = "客户拒收。",
                    Type = (int)OrderOpera.FromOperator
                });
                ts.Complete();
            }
            return View("Details", new { OrderNo = orderNo });
        }
        public ActionResult PreparePack(string orderNo)
        {
            var orderEntity = _orderRepo.Get(o => o.OrderNo == orderNo).First();
            using (var ts = new TransactionScope())
            {
                orderEntity.Status = (int)OrderStatus.PreparePack;
                orderEntity.UpdateUser = CurrentUser.CustomerId;
                orderEntity.UpdateDate = DateTime.Now;
                _orderRepo.Update(orderEntity);
                _orderLogRepo.Insert(new OrderLogEntity() {
                    OrderNo = orderNo,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser.CustomerId,
                    CustomerId = CurrentUser.CustomerId,
                    Operation = "准备打包。",
                    Type = (int)OrderOpera.FromCustomer
                });
                ts.Complete();
            }
            return View("Details", new { OrderNo = orderNo });
        }
        public ActionResult Received(string orderNo)
        {
            var orderEntity = _orderRepo.Get(o => o.OrderNo == orderNo).First();
            using (var ts = new TransactionScope())
            {
                orderEntity.Status = (int)OrderStatus.CustomerReceived;
                orderEntity.UpdateUser = CurrentUser.CustomerId;
                orderEntity.UpdateDate = DateTime.Now;
                _orderRepo.Update(orderEntity);
                _orderLogRepo.Insert(new OrderLogEntity()
                {
                    OrderNo = orderNo,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser.CustomerId,
                    CustomerId = CurrentUser.CustomerId,
                    Operation = "客户已签收。",
                    Type = (int)OrderOpera.FromCustomer
                });
                ts.Complete();
            }
            return View("Details", new { OrderNo = orderNo });
        }
        public ActionResult Convert2Sale(string orderNo)
        {
            var orderEntity = _orderRepo.Get(o => o.OrderNo == orderNo).First();
            using (var ts = new TransactionScope())
            {
                orderEntity.Status = (int)OrderStatus.Convert2Sales;
                orderEntity.UpdateUser = CurrentUser.CustomerId;
                orderEntity.UpdateDate = DateTime.Now;
                _orderRepo.Update(orderEntity);
                _orderLogRepo.Insert(new OrderLogEntity() {
                    OrderNo = orderNo,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser.CustomerId,
                    CustomerId = CurrentUser.CustomerId,
                    Operation = "转为销售。",
                    Type = (int)OrderOpera.FromCustomer
                });
                ts.Complete();
            }
            return View("Details", new { OrderNo = orderNo });
        }

        [HttpPost]
        public ActionResult Void(string orderNo)
        {
            var order = _orderRepo.Get(o => o.OrderNo == orderNo).First();

            using (var ts = new TransactionScope())
            {

                order.Status = (int)OrderStatus.Void;
                order.UpdateDate = DateTime.Now;
                order.UpdateUser = CurrentUser.CustomerId;
                _orderRepo.Update(order);

                _orderLogRepo.Insert(new OrderLogEntity()
                {
                    OrderNo = orderNo,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser.CustomerId,
                    CustomerId = CurrentUser.CustomerId,
                    Operation = "作废订单。",
                    Type = (int)OrderOpera.FromOperator
                });
                ts.Complete();
            }
            return View("Details", new { OrderNo = orderNo });
        }

        #region Reports to print
        public ActionResult PrintOrder(string orderNo)
        {
            if (string.IsNullOrEmpty(orderNo))
                throw new ArgumentNullException("orderNo");
            var confirmModel = _orderRepo.Get(o => o.OrderNo == orderNo).FirstOrDefault();
            if (null == confirmModel)
                throw new ArgumentException("订单号不存在！");
            var sectionModel = _orderRepo.Context.Set<SectionEntity>().Where(s => s.StoreId == confirmModel.StoreId && s.BrandId == confirmModel.BrandId).FirstOrDefault();
            if (null == sectionModel)
                throw new ArgumentException("专柜不存在！");
            var confirmItemsModel = _orderItemRepo.Get(o => o.OrderNo == orderNo && o.Status != (int)DataStatus.Deleted);
            return RenderReport("confirmorderreport", r =>
            {
                r.Database.Tables[0].SetDataSource(new[] { 
                    new OrderConfirmReportViewModel(){
                         Memo = confirmModel.Memo,
                          NeedInvoice = confirmModel.NeedInvoice.ToReportString(),
                           OrderNo = confirmModel.OrderNo,
                            SecionPerson = sectionModel.ContactPerson,
                             SectionName = sectionModel.Name,
                              SectionPhone = sectionModel.ContactPhone,
                               ShippingAddress = confirmModel.ShippingAddress,
                                ShippingContactPerson = confirmModel.ShippingContactPerson,
                                 ShippingContactPhone = confirmModel.ShippingContactPhone,
                                  ShippingZipCode = confirmModel.ShippingZipCode,
                                   SectionId = sectionModel.Id.ToString()
                    }
                });
                r.Database.Tables[1].SetDataSource(confirmItemsModel.Select(i => new OrderConfirmItemReportViewModel().FromEntity<OrderConfirmItemReportViewModel>(i)));
            });
            
        }
        #endregion
    }
}
