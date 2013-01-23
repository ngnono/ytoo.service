using System;
using Newtonsoft.Json;

namespace Passbook.Generator.Fields
{
    public class DateField : Field
    {
        // Methods
        public DateField(string key, string label, DateTime value, FieldDateTimeStyle dateStyle, FieldDateTimeStyle timeStyle)
            : base(key, label)
        {
            this.Value = value;
            this.DateStyle = dateStyle;
            this.TimeStyle = timeStyle;
        }

        protected override void WriteKeys(JsonWriter writer)
        {
            if (this.DateStyle != FieldDateTimeStyle.Unspecified)
            {
                writer.WritePropertyName("dateStyle");
                writer.WriteValue(this.DateStyle.ToString());
            }
            if (this.TimeStyle != FieldDateTimeStyle.Unspecified)
            {
                writer.WritePropertyName("timeStyle");
                writer.WriteValue(this.TimeStyle.ToString());
            }
            writer.WritePropertyName("isRelative");
            writer.WriteValue(this.IsRelative);
        }

        protected override void WriteValue(JsonWriter writer)
        {
            writer.WriteValue(this.Value.ToString("yyyy-MM-ddTHH:mm+08:00"));
        }

        // Properties
        public FieldDateTimeStyle DateStyle { get; set; }

        public bool IsRelative { get; set; }

        public FieldDateTimeStyle TimeStyle { get; set; }

        public DateTime Value { get; set; }
    }
}