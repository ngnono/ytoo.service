using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Passbook.Generator.Exception;

namespace Passbook.Generator.Fields
{
    public class StandardField : Field
    {
        // Methods
        public StandardField()
        {
        }

        public StandardField(string key, string label, string value)
            : base(key, label)
        {
            this.Value = value;
        }

        protected override void WriteValue(JsonWriter writer)
        {
            if (this.Value == null)
            {
                throw new RequiredFieldValueMissingException("value");
            }
            writer.WriteValue(this.Value);
        }

        // Properties
        public string Value { get; set; }
    }

 

}


