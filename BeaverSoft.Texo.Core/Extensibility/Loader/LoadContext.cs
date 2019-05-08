using System;
using BeaverSoft.Texo.Core.Configuration;

namespace BeaverSoft.Texo.Core.Extensibility.Loader
{
    public class LoadContext
    {
        public Query.Builder Configuration { get; set; }

        public Type Type { get; set; }
    }
}
