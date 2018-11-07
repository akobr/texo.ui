using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = false)]
    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string parameterKey)
        {
            ParameterKey = parameterKey;
        }

        public string ParameterKey { get; }

        public string ParameterTemplate { get; set; }

        public bool IsOptional { get; set; }

        public bool IsRepetable { get; set; }
    }
}
