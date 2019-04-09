using System;
using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Extensibility.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = false)]
    public class RepresentationsAttribute : Attribute
    {
        private readonly string[] representations;

        public RepresentationsAttribute(params string[] representations)
        {
            this.representations = representations ?? new string[0];
        }

        public IReadOnlyCollection<string> Representations => representations;

        public string SubjectPath { get; set; }
    }
}
