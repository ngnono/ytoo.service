
namespace com.intime.fashion.data.sync.Wgw.Request.Item
{
    public sealed class UpdateItemRequest : EntityRequest
    {
        public UpdateItemRequest()
            : base(null)
        {
        }

        public override string Resource
        {
            get { return "/wgwitem/wgUpdateItem.xhtml"; }
        }
    }
}