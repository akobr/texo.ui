using System;

namespace BeaverSoft.Texo.Core.Extensibility.Loader
{
    public interface ITypeFactory
    {
        TClass BuildType<TClass>();
    }
}
