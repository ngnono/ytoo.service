using Newtonsoft.Json;
using Passbook.Generator.Exception;

namespace Passbook.Generator.Fields
{
    public abstract class Field
    {
        // Methods
        public Field()
        {
        }

        public Field(string key, string label)
        {
            this.Key = key;
            this.Label = label;
        }

        public Field(string key, string label, string changeMessage, FieldTextAlignment textAligment)
        {
            this.Key = key;
            this.Label = label;
            this.ChangeMessage = changeMessage;
            this.TextAlignment = textAligment;
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(this.Key))
            {
                throw new RequiredFieldValueMissingException("key");
            }
        }

        public void Write(JsonWriter writer)
        {
            this.Validate();
            writer.WriteStartObject();
            writer.WritePropertyName("key");
            writer.WriteValue(this.Key);
            if (!string.IsNullOrEmpty(this.ChangeMessage))
            {
                writer.WritePropertyName("changeMessage");
                writer.WriteValue(this.ChangeMessage);
            }
            if (!string.IsNullOrEmpty(this.Label))
            {
                writer.WritePropertyName("label");
                writer.WriteValue(this.Label);
            }
            if (this.TextAlignment != FieldTextAlignment.Unspecified)
            {
                writer.WritePropertyName("textAlignment");
                writer.WriteValue(this.TextAlignment);
            }
            this.WriteKeys(writer);
            writer.WritePropertyName("value");
            this.WriteValue(writer);
            writer.WriteEndObject();
        }

        protected virtual void WriteKeys(JsonWriter writer)
        {
        }

        protected abstract void WriteValue(JsonWriter writer);

        // Properties
        public string ChangeMessage { get; set; }

        public string Key { get; set; }

        public string Label { get; set; }

        public FieldTextAlignment TextAlignment { get; set; }
    }
}