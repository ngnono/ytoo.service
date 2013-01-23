using Newtonsoft.Json;

namespace Passbook.Generator.Fields
{
    public class NumberField : Field
    {
        // Methods
        public NumberField(string key, string label, int value, FieldNumberStyle numberStyle)
            : base(key, label)
        {
            this.Value = value;
            this.NumberStyle = numberStyle;
        }

        protected override void WriteKeys(JsonWriter writer)
        {
            if (this.CurrencyCode != null)
            {
                writer.WritePropertyName("currencyCode");
                writer.WriteValue(this.CurrencyCode);
            }
            if (this.NumberStyle != FieldNumberStyle.Unspecified)
            {
                writer.WritePropertyName("numberStyle");
                writer.WriteValue(this.NumberStyle.ToString());
            }
        }

        protected override void WriteValue(JsonWriter writer)
        {
            writer.WriteValue(this.Value);
        }

        // Properties
        public string CurrencyCode { get; set; }

        public FieldNumberStyle NumberStyle { get; set; }

        public long Value { get; set; }
    }
}