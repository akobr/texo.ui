using System;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method, 
        Inherited = false, 
        AllowMultiple = false)]
    public class DocumentationAttribute : Attribute
    {
        public DocumentationAttribute(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; }

        public string Description { get; }

        public string SubjectPath { get; set; }
    }
}
