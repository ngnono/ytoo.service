using System;

namespace Yintai.Hangzhou.Model.ESModel
{
    public class ESStock
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int ColorValueId { get; set; }

        public int SizeValueId { get; set; }

        public int Amount { get; set; }

        public decimal Price { get; set; }

        public decimal LabelPrice { get; set; }

        private DateTime _updatedDate;
        public DateTime UpdateDate
        {
            get { return _updatedDate.ToUniversalTime(); }
            set { _updatedDate = value; }
        }
    }
}
