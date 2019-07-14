using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = false)]
    public class LocalisedDocumentationAttribute : Attribute
    {
        public LocalisedDocumentationAttribute(string titleKey, string descriptionKey)
        {
            TitleKey = titleKey;
            DescriptionKey = descriptionKey;

        }

        public string TitleKey { get; }

        public string DescriptionKey { get; }

        public string Path { get; set; }
    }
}
