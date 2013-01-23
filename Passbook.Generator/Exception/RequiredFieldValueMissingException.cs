using System;

namespace Passbook.Generator.Exception
{
    [Serializable]
    public class RequiredFieldValueMissingException : System.Exception
    {
        // Methods
        public RequiredFieldValueMissingException(string fieldName)
            : base("Missing key value. Every Field must have a key specified.")
        {
        }
    }
}