using System.Linq;
using System.Text;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.fashion.data.sync.Wgw.Request.Builder
{
    public class UpdateItemRequestParamsBuilder:ItemRequestParamsBuilder
    {
        public UpdateItemRequestParamsBuilder(ISyncRequest request) : base(request)
        {
        }

        public override ISyncRequest BuildParameters(object entity)
        {
            var item = entity as ProductEntity;
            if (item != null)
            {
                Request.Put(ParamName.Param_ItemId, GetMappedItemId(item.Id));
            }

            return base.BuildParameters(entity); ;
        }

        private string GetMappedItemId(int id)
        {
            using (var db = GetDbContext())
            {
                var map = db.Map4Products.FirstOrDefault(m => m.ProductId == id && m.Channel == ConstValue.WGW_CHANNEL_NAME);
                return map == null ? string.Empty : map.ChannelProductId;
            }
        }

        protected override void BuildProductImages(ProductEntity entity)
        {
#if !DEBUG
            using (var db = GetDbContext())
            {
                int idx = 0;
                var moreImg = new StringBuilder();
                foreach (var img in db.Resources.Where(t => t.SourceId == entity.Id && t.Type == 1).OrderByDescending(t=>t.IsDefault))
                {
                    var url = string.Format("{0}/{1}_320x0.jpg", WgwConfigHelper.Image_BaseUrl, img.Name);
                    if (idx > 5)
                    {
                        moreImg.Append("url").Append("|");
                    }
                    else
                    {
                        Request.Put(string.Format("uploadPicInfo{0}", ++idx), url);
                    }
                }
                var moreImgStr = moreImg.ToString();
                var len = moreImgStr.Length;
                if (len > 0)
                {
                    Request.Put("moreImg", moreImgStr.Substring(0, len - 1));
                }
            }
#else
            Request.Put("uploadPicInfo1", "http://ec4.images-amazon.com/images/I/51yQ0l-qvkL._AA135_.jpg");
            Request.Put("uploadPicInfo2", "http://ec8.images-amazon.com/images/I/41dBoxFHlyL._AA135_.jpg");
#endif
        }
    }
}
