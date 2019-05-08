using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method, 
        Inherited = false, 
        AllowMultiple = true)]
    public class DocumentationAttribute : Attribute
    {
        public DocumentationAttribute(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; }

        public string Description { get; }

        public string Path { get; set; }
    }
}
