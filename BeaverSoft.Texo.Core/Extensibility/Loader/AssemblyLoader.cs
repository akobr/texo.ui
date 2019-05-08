using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Extensibility.Attributes;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BeaverSoft.Texo.Core.Extensibility.Loader
{
    public class AssemblyLoader
    {
        public IEnumerable<Query> LoadConfiguration(Assembly assembly)
        {
            if (!assembly.IsDefined(typeof(CommandLibraryAttribute), false))
            {
                return Enumerable.Empty<Query>();
            }

            Dictionary<string, LoadContext> commands = new Dictionary<string, LoadContext>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, Query.Builder> queries = new Dictionary<string, Query.Builder>(StringComparer.OrdinalIgnoreCase);
            

            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type.IsDefined(typeof(CommandAttribute), false))
                {
                    
                }

                if (type.IsDefined(typeof(QueryAttribute), false))
                {

                }
            }

            return null;
        }
    }
}
