using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Extensibility.Attributes;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BeaverSoft.Texo.Core.Extensibility
{
    public class AssemblyLoader
    {
        public IEnumerable<Query> LoadConfiguration(Assembly assembly)
        {
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type.IsDefined(typeof(CommandAttribute), false))
                {

                }
            }

            return null;
        }
    }
}
